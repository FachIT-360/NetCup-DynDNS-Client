// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Collections;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Text.Json;

using FachIT360.Utils.Dns.NetCup.Models;
using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

using SourceGenerationContext = FachIT360.Utils.Dns.NetCup.JsonSerializerContext.SourceGenerationContext;

namespace FachIT360.Utils.Dns.NetCup.Services
{
    public class NetCupDynDnsApiClient
    {
    #region Constants - Static fields - Fields

        private readonly Dictionary<string, List<string>> _domains    = new();
        private readonly Login                            _apiLoginCredentials;
        private readonly HttpClient                       _httpClient;
        private readonly ILogger<NetCupDynDnsApiClient>   _log;

    #endregion

    #region Constructors and Destructors

        public NetCupDynDnsApiClient(ILogger<NetCupDynDnsApiClient> log,
                                     HttpClient httpClient,
                                     IConfiguration config)
        {
            try
            {
                _log        = log;
                _httpClient = httpClient;

                _apiLoginCredentials = new Login
                                       {
                                           ApiKey = !string.IsNullOrWhiteSpace(config["NetCupApi:Login:ApiKey"])
                                                        ? config["NetCupApi:Login:ApiKey"]!
                                                        : !string.IsNullOrWhiteSpace(config["NETCUP_LOGIN_APIKEY"])
                                                            ? config["NETCUP_LOGIN_APIKEY"]!
                                                            : throw new ArgumentNullException(
                                                                  nameof(config), "The ApiKey parameter in the config is not set."),
                                           ApiPassword = !string.IsNullOrWhiteSpace(config["NetCupApi:Login:ApiPassword"])
                                                             ? config["NetCupApi:Login:ApiPassword"]!
                                                             : !string.IsNullOrWhiteSpace(config["NETCUP_LOGIN_APIPASSWORD"])
                                                                 ? config["NETCUP_LOGIN_APIPASSWORD"]!
                                                                 : throw new ArgumentNullException(
                                                                       nameof(config), "The ApiPassword parameter in the config is not set."),
                                           CustomerNumber = !string.IsNullOrWhiteSpace(config["NetCupApi:Login:CustomerNumber"])
                                                                ? uint.Parse(config["NetCupApi:Login:CustomerNumber"]!)
                                                                : !string.IsNullOrWhiteSpace(config["NETCUP_LOGIN_CUSTOMERNUMBER"])
                                                                    ? uint.Parse(config["NETCUP_LOGIN_CUSTOMERNUMBER"]!)
                                                                    : throw new ArgumentNullException(
                                                                          nameof(config), "The CustomerNumber parameter in the config is not set.")
                                       };

                // Get domains from appsettings
                foreach (var domain in config.GetSection("NetCupApi:Domains").GetChildren())
                {
                    _domains.Add(domain.Key, new List<string>(domain.GetChildren()
                                                                    .Where(item => item.Value != null)
                                                                    .Select(item => item.Value!)
                                                                    .Distinct()));
                }

                // Get domains from environment variables
                foreach (var environmentVariable in Environment
                                                    .GetEnvironmentVariables()
                                                    .Cast<DictionaryEntry>()
                                                    .Where(environmentVariable => environmentVariable.Key.ToString()!.StartsWith("NETCUP_DOMAINS")))
                {
                    if (environmentVariable.Value == null)
                    {
                        continue;
                    }

                    // Parse domain elements from environment variable
                    var domainElements = environmentVariable
                                         .Value
                                         .ToString()!
                                         .ToLower()
                                         .Split('|')
                                         .ToList();

                    // Get the domain name
                    var newEnvDomainName = domainElements[0];

                    // Get the dns records
                    var newEnvDnsRecords = domainElements.Skip(1).ToList();

                    // Add domain if not exists in _domains
                    if (!_domains.Keys.Contains(newEnvDomainName))
                    {
                        _domains.Add(newEnvDomainName, new List<string>());
                    }

                    // Get the current domain from _domains
                    var existingDomain = _domains.Single(item => item.Key == newEnvDomainName);

                    // Get a merged collection of dns records
                    newEnvDnsRecords = existingDomain.Value
                                                     .Union(newEnvDnsRecords)
                                                     .ToList();

                    // Clear the dns records
                    existingDomain.Value.Clear();

                    // Add the merged dns records
                    existingDomain.Value.AddRange(newEnvDnsRecords);
                }
            }
            catch (Exception ex)
            {
                log.LogError(
                    $"Error while initialize NetCupDynDnsApiClient. Check the exception details in this log event. Exception Message: {ex.Message}");

                throw;
            }
        }

    #endregion

    #region Methods

        public async Task UpdateDynDns()
        {
            var currentPublicIps = await GetCurrentPublicIp();

            if (!currentPublicIps.Any())
            {
                return;
            }

            var apiSessionId = await Login();

            if (apiSessionId != null)
            {
                foreach (var domain in _domains)
                {
                    var domainDnsRecords = await GetInfoDnsRecords(apiSessionId, domain.Key);

                    if (domainDnsRecords == null)
                    {
                        continue;
                    }

                    var recordNamesToUpdate = domain.Value;

                    if (CurrentPublicIpChanged(currentPublicIps, domainDnsRecords, recordNamesToUpdate, out var recordsToUpdate))
                    {
                        try
                        {
                            var action = new RequestActionUpdateDnsRecords
                                         {
                                             Action = "updateDnsRecords",
                                             Param = new UpdateDnsRecords
                                                     {
                                                         ApiKey         = _apiLoginCredentials.ApiKey,
                                                         ApiSessionId   = apiSessionId,
                                                         CustomerNumber = _apiLoginCredentials.CustomerNumber,
                                                         DomainName     = domain.Key,
                                                         DnsRecordSet = new DnsRecords
                                                                        {
                                                                            DnsRecordArray = [.. recordsToUpdate]
                                                                        }
                                                     }
                                         };

                            var actionJson = JsonSerializer.Serialize(action, SourceGenerationContext.Default.RequestActionUpdateDnsRecords);

                            using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                            {
                                request.Version = HttpVersion.Version20;
                                request.Content = new StringContent(actionJson, Encoding.UTF8, "application/json");

                                using (var response = await _httpClient.SendAsync(request))
                                {
                                    if (response.IsSuccessStatusCode)
                                    {
                                        var responseData = JsonSerializer
                                            .Deserialize(await response.Content.ReadAsStringAsync(),
                                                         SourceGenerationContext.Default.ResponseDataUpdateDnsRecords);

                                        if (responseData is { Status: "success" })
                                        {
                                            _log.LogInformation($"The dns records update for domain \"{domain.Key}\" was successful.");

                                            continue;
                                        }

                                        _log.LogInformation($"The dns records update for domain \"{domain.Key}\" was failed.");
                                    }
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            _log.LogError($"Error while dns records update. Exception Message: {ex.Message}");
                        }
                    }
                }

                await Logout(apiSessionId);
            }
        }

        private bool CurrentPublicIpChanged(List<IPAddress> currentPublicIps,
                                            ResponseDataInfoDnsRecords infoDnsRecords,
                                            IEnumerable<string> recordNamesToUpdate,
                                            out List<DnsRecord> recordsToUpdate)
        {
            var hasChanged = false;
            recordsToUpdate = [];

            if (infoDnsRecords.ResponseData == null)
            {
                return false;
            }

            // Get dns records that should update
            foreach (var dnsRecord in infoDnsRecords
                                      .ResponseData
                                      .DnsRecordArray
                                      .Where(item => recordNamesToUpdate.Contains(item.Hostname)))
            {
                // Check if destination is parsable to IPAddress
                if (!IPAddress.TryParse(dnsRecord.Destination, out var destinationIp))
                {
                    continue;
                }

                // Check an IP Address of hosts has changed
                foreach (var currentIp in currentPublicIps.Where(currentPublicIp => currentPublicIp.AddressFamily == destinationIp.AddressFamily))
                {
                    if (currentIp.ToString() == destinationIp.ToString())
                    {
                        _log.LogDebug($"The ip of dns record \"{dnsRecord.Hostname}\" with type \"{dnsRecord.Type}\" has not changed.");

                        continue;
                    }

                    _log.LogInformation(
                        $"The ip of dns record \"{dnsRecord.Hostname}\" with type \"{dnsRecord.Type}\" has changed to \"{currentIp}\".");

                    dnsRecord.Destination = currentIp.ToString();

                    hasChanged = true;

                    recordsToUpdate.Add(dnsRecord);
                }
            }

            return hasChanged;
        }

        private async Task<List<IPAddress>> GetCurrentPublicIp()
        {
            try
            {
                var publicAddresses = new List<IPAddress>();

                if (IPAddress.TryParse(await _httpClient.GetStringAsync("https://api.ipify.org"), out var currentIpAddressV4))
                {
                    _log.LogDebug($"The current public ip v4 address is: {currentIpAddressV4}");
                    publicAddresses.Add(currentIpAddressV4);
                }

                if (IPAddress.TryParse(await _httpClient.GetStringAsync("https://api64.ipify.org"), out var currentIpAddressV6))
                {
                    if (currentIpAddressV6.AddressFamily == AddressFamily.InterNetworkV6)
                    {
                        _log.LogDebug($"The current public ip v6 address is: {currentIpAddressV6}");
                        publicAddresses.Add(currentIpAddressV6);
                    }
                }

                return publicAddresses;
            }
            catch (Exception ex)
            {
                _log.LogError($"Error while getting current public ip. Exception Message: {ex.Message}");

                return [];
            }
        }

        private async Task<ResponseDataInfoDnsRecords?> GetInfoDnsRecords(string apiSessionId, string domain)
        {
            try
            {
                var action = new RequestActionInfoDnsRecords
                             {
                                 Action = "infoDnsRecords",
                                 Param = new InfoDnsRecords
                                         {
                                             ApiKey         = _apiLoginCredentials.ApiKey,
                                             ApiSessionId   = apiSessionId,
                                             CustomerNumber = _apiLoginCredentials.CustomerNumber,
                                             DomainName     = domain
                                         }
                             };

                var actionJson = JsonSerializer.Serialize(action, SourceGenerationContext.Default.RequestActionInfoDnsRecords);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, "application/json");

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonSerializer.Deserialize(
                                jsonResponse, SourceGenerationContext.Default.ResponseDataInfoDnsRecords);

                            if (responseData is { Status: "success" })
                            {
                                _log.LogDebug($"Requesting dns records for domain \"{domain}\" was successful.");

                                return responseData;
                            }

                            _log.LogError($"Requesting dns records for domain \"{domain}\" was failed.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error while request domain dns records. Exception Message: {ex.Message}");
            }

            return null;
        }

        private async Task<string?> Login()
        {
            try
            {
                var action = new RequestActionLogin
                             {
                                 Action = "login",
                                 Param  = _apiLoginCredentials
                             };

                var actionJson = JsonSerializer.Serialize(action, SourceGenerationContext.Default.RequestActionLogin);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, "application/json");

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = JsonSerializer
                                .Deserialize(await response.Content.ReadAsStringAsync(),
                                             SourceGenerationContext.Default.ResponseDataLogin);

                            if (responseData is { Status: "success" })
                            {
                                _log.LogDebug("The api login was successful.");

                                return responseData.ResponseData.ApiSessionId;
                            }

                            if (responseData != null && responseData.ShortMessage != "success")
                            {
                                _log.LogError($"The api login was failed. Reason: {responseData.ShortMessage}");
                            }
                            else
                            {
                                _log.LogError("The api login was failed.");
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError($"Error while API login. Exception Message: {ex.Message}");
            }

            return null;
        }

        private async Task Logout(string apiSessionId)
        {
            try
            {
                var action = new RequestActionLogout
                             {
                                 Action = "logout",
                                 Param = new Logout
                                         {
                                             ApiKey         = _apiLoginCredentials.ApiKey,
                                             ApiSessionId   = apiSessionId,
                                             CustomerNumber = _apiLoginCredentials.CustomerNumber
                                         }
                             };

                var actionJson = JsonSerializer.Serialize(action, SourceGenerationContext.Default.RequestActionLogout);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, "application/json");

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var responseData = JsonSerializer
                                .Deserialize(await response.Content.ReadAsStringAsync(),
                                             SourceGenerationContext.Default.ResponseDataLogout);

                            if (responseData is { Status: "success" })
                            {
                                _log.LogDebug("The api logout was successful.");

                                return;
                            }

                            _log.LogError("The api logout was failed.");
                        }
                    }
                }
            }

            catch (Exception ex)

            {
                _log.LogError($"Error while API login. Exception Message: {ex.Message}");
            }
        }

    #endregion
    }
}
