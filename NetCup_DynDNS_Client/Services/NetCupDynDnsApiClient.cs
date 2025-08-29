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

namespace FachIT360.Utils.Dns.NetCup.Services
{
    public class NetCupDynDnsApiClient
    {
    #region Constants - Static fields - Fields

        private readonly HttpClient                     _httpClient;
        private readonly ILogger<NetCupDynDnsApiClient> _log;
        private readonly IOptionsMonitor<NetCupApi>     _netCupApiOptions;

        private const string JsonMimeType = "application/json";
        private const string SuccessState = "success";

    #endregion

    #region Constructors and Destructors

        public NetCupDynDnsApiClient(ILogger<NetCupDynDnsApiClient> log,
                                     IOptionsMonitor<NetCupApi> netCupApiOptions,
                                     HttpClient httpClient)
        {
            _log                              = log;
            _httpClient                       = httpClient;
            _httpClient.DefaultRequestVersion = HttpVersion.Version20;
            _httpClient.BaseAddress           = netCupApiOptions.CurrentValue.EndpointUrl;
            _netCupApiOptions                 = netCupApiOptions;
        }

    #endregion

    #region Methods

        public async Task<ResponseDataInfoDnsRecords?> GetDnsRecordsForDomainAsync(string apiSessionId, string domain)
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
                    request.Content = new StringContent(actionJson, Encoding.UTF8, JsonMimeType);

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataInfoDnsRecords>(jsonResponse);

                            if (responseData is { Status: SuccessState })
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

        public async Task<string?> LoginAsync(string apiKey, string apiPassword, uint customerNumber)
        {
            return await LoginAsync(new Login { ApiKey = apiKey, ApiPassword = apiPassword, CustomerNumber = customerNumber });
        }

        public async Task<string?> LoginAsync(Login login)
        {
            try
            {
                var action = new RequestActionLogin
                             {
                                 Action = "login",
                                 Param  = login
                             };

                var actionJson = JsonConvert.SerializeObject(action);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, JsonMimeType);

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataLogin>(jsonResponse);

                            if (responseData is { Status: SuccessState })
                            {
                                _log.LogDebug("The api login was successful.");

                                return responseData.ResponseData?.ApiSessionId;
                            }

                            if (responseData != null && responseData.LongMessage != SuccessState)
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

        public async Task LogoutAsync(string apiSessionId)
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

                var actionJson = JsonConvert.SerializeObject(action);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, JsonMimeType);

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataLogout>(jsonResponse);

                            if (responseData is { Status: SuccessState })
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

        public async Task StartSyncDnsRecords()
        {
            var currentPublicIps = await GetCurrentPublicIp();

            if (currentPublicIps.Count == 0)
            {
                return;
            }

            var apiSessionId = await LoginAsync(_netCupApiOptions.CurrentValue.Login);

            if (apiSessionId != null)
            {
                foreach (var (domainName, recordNamesToUpdate) in _netCupApiOptions.CurrentValue.Domains)
                {
                    // Get DNS records for domain
                    var domainDnsRecords = await GetDnsRecordsForDomainAsync(apiSessionId, domainName);

                    if (CurrentPublicIpChanged(currentPublicIps, domainDnsRecords, recordNamesToUpdate, out var recordsToUpdate))
                    {
                        await UpdateDnsRecordsAsync(apiSessionId, domainName, recordsToUpdate);
                    }
                }

                await LogoutAsync(apiSessionId);
            }
        }

        private bool CurrentPublicIpChanged(List<IPAddress> currentPublicIps,
                                            ResponseDataInfoDnsRecords? infoDnsRecords,
                                            IEnumerable<string> recordNamesToUpdate,
                                            out List<DnsRecord> recordsToUpdate)
        {
            var hasChanged = false;
            recordsToUpdate = [];

            if (infoDnsRecords?.ResponseData == null)
            {
                return hasChanged;
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
                        _log.LogDebug("The ip of dns record \"{DnsRecordHostname}\" with type \"{DnsRecordType}\" has not changed.",
                                      dnsRecord.Hostname, dnsRecord.Type);

                        continue;
                    }

                    _log.LogInformation(
                        "The ip of dns record \"{DnsRecordHostname}\" with type \"{DnsRecordType}\" has changed to \"{CurrentIp}\".",
                        dnsRecord.Hostname, dnsRecord.Type, currentIp);

                    dnsRecord.Destination = currentIp.ToString();

                    hasChanged = true;

                    recordsToUpdate.Add(dnsRecord);
                }
            }

            return hasChanged;
        }

        /// <summary>
        ///     Get current public ip addresses.
        /// </summary>
        /// <returns>Returns all public IP addresses in IPv4 and / or IPv6 if available.</returns>
        private async Task<List<IPAddress>> GetCurrentPublicIp()
        {
            try
            {
                var publicAddresses = new List<IPAddress>();

                if (IPAddress.TryParse(await _httpClient.GetStringAsync(_netCupApiOptions.CurrentValue.MyIp4ApiUrl), out var currentIpAddressV4))
                {
                    _log.LogDebug("The current public ip v4 address is: {CurrentIpAddressV4}", currentIpAddressV4);
                    publicAddresses.Add(currentIpAddressV4);
                }

                if (IPAddress.TryParse(await _httpClient.GetStringAsync(_netCupApiOptions.CurrentValue.MyIp6ApiUrl), out var currentIpAddressV6) &&
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

        /// <summary>
        ///     Update DNS records for a domain.
        /// </summary>
        /// <param name = "apiSessionId">
        ///     The api session id that gets from <see cref = "LoginAsync(Login)" /> or
        ///     <see cref = "LoginAsync(string,string,uint)" />.
        /// </param>
        private async Task UpdateDnsRecordsAsync(string apiSessionId, string domainName, List<DnsRecord> recordsToUpdate)
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
                                             DomainName     = domainName,
                                             DnsRecordSet = new DnsRecords
                                                            {
                                                                DnsRecordArray = [.. recordsToUpdate]
                                                            }
                                         }
                             };

                var actionJson = JsonConvert.SerializeObject(action);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, JsonMimeType);

                    using (var response = await _httpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataUpdateDnsRecords>(jsonResponse);

                            if (responseData is { Status: SuccessState })
                            {
                                _log.LogInformation("The dns records update for domain \"{DomainKey}\" was successful.", domainName);

                                return;
                            }

                            _log.LogInformation("The dns records update for domain \"{DomainKey}\" was failed.", domainName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while dns records update. Exception Message: {ExMessage}", ex.Message);
            }
        }

    #endregion
    }
}
