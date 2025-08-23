// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Net;
using System.Net.Sockets;
using System.Text;

using FachIT360.Utils.Dns.NetCup.Models;
using FachIT360.Utils.Dns.NetCup.Models.AppSettings;
using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

using Microsoft.Extensions.Options;

using Newtonsoft.Json;

using JsonSerializer = System.Text.Json.JsonSerializer;
using SourceGenerationContext = FachIT360.Utils.Dns.NetCup.JsonSerializerContext.SourceGenerationContext;

namespace FachIT360.Utils.Dns.NetCup.Services
{
    public class NetCupDynDnsApiClient
    {
    #region Constants - Static fields - Fields

        private readonly Dictionary<string, List<string>> _domains = new();
        private readonly HttpClient                       _httpClient;
        private readonly ILogger<NetCupDynDnsApiClient>   _log;
        private readonly IOptionsMonitor<NetCupApi>       _netCupApiOptions;

    #endregion

    #region Constructors and Destructors

        public NetCupDynDnsApiClient(ILogger<NetCupDynDnsApiClient> log,
                                     IOptionsMonitor<NetCupApi> netCupApiOptions,
                                     HttpClient httpClient)
        {
            try
            {
                _log                              = log;
                _httpClient                       = httpClient;
                _httpClient.DefaultRequestVersion = HttpVersion.Version20;
                _httpClient.BaseAddress           = netCupApiOptions.CurrentValue.EndpointUrl;
                _netCupApiOptions                 = netCupApiOptions;
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
                foreach (var domain in _netCupApiOptions.CurrentValue.Domains)
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
                                                         ApiKey         = _netCupApiOptions.CurrentValue.Login.ApiKey!,
                                                         ApiSessionId   = apiSessionId,
                                                         CustomerNumber = _netCupApiOptions.CurrentValue.Login.CustomerNumber!.Value,
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
                    _log.LogDebug("The current public ip v4 address is: {CurrentIpAddressV4}", currentIpAddressV4);
                    publicAddresses.Add(currentIpAddressV4);
                }

                if (IPAddress.TryParse(await _httpClient.GetStringAsync("https://api64.ipify.org"), out var currentIpAddressV6) &&
                    currentIpAddressV6.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    _log.LogDebug("The current public ip v6 address is: {CurrentIpAddressV6}", currentIpAddressV6);
                    publicAddresses.Add(currentIpAddressV6);
                }

                return publicAddresses;
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while getting current public ip. Exception Message: {ExMessage}", ex.Message);

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
                                             ApiKey         = _netCupApiOptions.CurrentValue.Login.ApiKey!,
                                             ApiSessionId   = apiSessionId,
                                             CustomerNumber = _netCupApiOptions.CurrentValue.Login.CustomerNumber!.Value,
                                             DomainName     = domain
                                         }
                             };

                var actionJson = JsonConvert.SerializeObject(action);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, "application/json");

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataInfoDnsRecords>(jsonResponse);

                            if (responseData is { Status: "success" })
                            {
                                _log.LogDebug("Requesting dns records for domain \"{Domain}\" was successful.", domain);

                                return responseData;
                            }

                            _log.LogError("Requesting dns records for domain \"{Domain}\" was failed.", domain);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while request domain dns records. Exception Message: {ExMessage}", ex.Message);
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
                                 Param  = _netCupApiOptions.CurrentValue.Login
                             };

                var actionJson = JsonConvert.SerializeObject(action);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, "application/json");

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataLogin>(jsonResponse);

                            if (responseData is { Status: "success" })
                            {
                                _log.LogDebug("The api login was successful.");

                                return responseData.ResponseData?.ApiSessionId;
                            }

                            if (responseData != null && responseData.LongMessage != "success")
                            {
                                _log.LogError("The api login was failed. Reason: {ResponseDataLongMessage}", responseData.LongMessage);
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
                _log.LogError(ex, "Error while API login. Exception Message: {ExMessage}", ex.Message);
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
                                             ApiKey         = _netCupApiOptions.CurrentValue.Login.ApiKey!,
                                             ApiSessionId   = apiSessionId,
                                             CustomerNumber = _netCupApiOptions.CurrentValue.Login.CustomerNumber!.Value
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
                _log.LogError(ex, "Error while API login. Exception Message: {ExMessage}", ex.Message);
            }
        }

    #endregion
    }
}
