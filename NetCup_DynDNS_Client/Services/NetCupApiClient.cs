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
    public class NetCupApiClient
    {
    #region Constants - Static fields - Fields

        private readonly ILogger<NetCupApiClient>   _log;

        private const string JsonMimeType = "application/json";
        private const string SuccessState = "success";

    #endregion

    #region Constructors and Destructors

        public NetCupApiClient(ILogger<NetCupApiClient> log,
                               HttpClient httpClient)
        {
            _log                             = log;
            HttpClient                       = httpClient;
            HttpClient.DefaultRequestVersion = HttpVersion.Version20;
        }

    #endregion

    #region Properties

        public HttpClient HttpClient { get; }

        public string NetCupApiKey { get; set; } = null!;

        public uint NetCupCustomerNumber { get; set; }

    #endregion

    #region Methods

        /// <summary>
        ///     Get current public ip addresses.
        /// </summary>
        /// <returns>Returns all public IP addresses in IPv4 and / or IPv6 if available.</returns>
        public async Task<List<IPAddress>> GetCurrentPublicIpAsync(string ipv4ApiAddress, string ipv6ApiAddress)
        {
            try
            {
                var publicAddresses = new List<IPAddress>();

                if (IPAddress.TryParse(await HttpClient.GetStringAsync(ipv4ApiAddress), out var currentIpAddressV4))
                {
                    _log.LogDebug("The current public ip v4 address is: {CurrentIpAddressV4}", currentIpAddressV4);
                    publicAddresses.Add(currentIpAddressV4);
                }

                if (IPAddress.TryParse(await HttpClient.GetStringAsync(ipv6ApiAddress), out var currentIpAddressV6) &&
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
        ///     Get all DNS Records for a given domain
        /// </summary>
        /// <param name = "apiSessionId">
        ///     The api session id that gets from <see cref = "LoginAsync(Login)" /> or
        ///     <see cref = "LoginAsync(string,string,uint)" />.
        /// </param>
        /// <param name = "domain">The source domain for requested DNS records.</param>
        /// <returns>
        ///     Returns an instance of <see cref = "ResponseDataInfoDnsRecords" /> that encapsulate a collection of all DNS
        ///     records.
        /// </returns>
        public async Task<ResponseDataInfoDnsRecords?> GetDnsRecordsForDomainAsync(string apiSessionId, string domain)
        {
            try
            {
                var action = new RequestActionInfoDnsRecords
                             {
                                 Action = "infoDnsRecords",
                                 Param = new InfoDnsRecords
                                         {
                                             ApiKey         = NetCupApiKey,
                                             ApiSessionId   = apiSessionId,
                                             CustomerNumber = NetCupCustomerNumber,
                                             DomainName     = domain
                                         }
                             };

                var actionJson = JsonConvert.SerializeObject(action);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, JsonMimeType);

                    using (var response = await HttpClient.SendAsync(request))
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

        /// <summary>
        ///     Login to the NetCup API
        /// </summary>
        /// <param name = "apiKey">The API Key for NetCup API</param>
        /// <param name = "apiPassword">The API Password for NetCup API</param>
        /// <param name = "customerNumber">The NetCup Customer Number</param>
        /// <returns>Returns the ResponseDataLogin.ResponseData.ApiSessionId for followed API requests.</returns>
        public async Task<string?> LoginAsync(string apiKey, string apiPassword, uint customerNumber)
        {
            return await LoginAsync(new Login { ApiKey = apiKey, ApiPassword = apiPassword, CustomerNumber = customerNumber });
        }

        /// <summary>
        ///     Login to the NetCup API
        /// </summary>
        /// <param name = "login">NetCup API Credentials</param>
        /// <returns>Returns the ResponseDataLogin.ResponseData.ApiSessionId for followed API requests.</returns>
        public async Task<string?> LoginAsync(Login login)
        {
            try
            {
                NetCupApiKey         = login.ApiKey!;
                NetCupCustomerNumber = login.CustomerNumber ??= 0;

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

                    using (var response = await HttpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataLogin>(jsonResponse);

                            if (responseData is { Status: SuccessState })
                            {
                                _log.LogDebug(
                                    "{ResponseLongMessage} | Response Code: {ResponseCode} | State: {Status} | Server request ID: {ServerRequestId}",
                                    responseData.LongMessage, responseData.StatusCode, responseData.Status, responseData.ServerRequestId);

                                return responseData.ResponseData?.ApiSessionId;
                            }

                            _log.LogError("{ResponseLongMessage}", responseData?.LongMessage ?? "Not set!");
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

        public async Task<bool> LogoutAsync(string? apiSessionId)
        {
            try
            {
                if (apiSessionId == null)
                {
                    _log.LogError("ApiSessionId is null");
                
                    return false;
                }
                
                var action = new RequestActionLogout
                             {
                                 Action = "logout",
                                 Param = new Logout
                                         {
                                             ApiKey         = NetCupApiKey,
                                             ApiSessionId   = apiSessionId,
                                             CustomerNumber = NetCupCustomerNumber
                                         }
                             };

                var actionJson = JsonConvert.SerializeObject(action);

                using (var request = new HttpRequestMessage(HttpMethod.Post, ""))
                {
                    request.Version = HttpVersion.Version20;
                    request.Content = new StringContent(actionJson, Encoding.UTF8, JsonMimeType);

                    using (var response = await HttpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataLogout>(jsonResponse);

                            if (responseData is { Status: SuccessState })
                            {
                                _log.LogDebug(
                                    "{ResponseLongMessage} | Response Code: {ResponseCode} | State: {Status} | Server request ID: {ServerRequestId}",
                                    responseData.LongMessage, responseData.StatusCode, responseData.Status, responseData.ServerRequestId);

                                return true;
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

            return false;
        }

        /// <summary>
        ///     Update DNS records for a domain.
        /// </summary>
        /// <param name = "apiSessionId">
        ///     The api session id that gets from <see cref = "LoginAsync(Login)" /> or
        ///     <see cref = "LoginAsync(string,string,uint)" />.
        /// </param>
        /// <param name = "domainName">The domain name of updates.</param>
        /// <param name = "recordsToUpdate">The records to update.</param>
        /// <returns>Returns true if the update was successful, otherwise false.</returns>
        public async Task<ResponseDataUpdateDnsRecords?> UpdateDnsRecordsAsync(string apiSessionId, string domainName,
                                                                               List<DnsRecord> recordsToUpdate)
        {
            try
            {
                var action = new RequestActionUpdateDnsRecords
                             {
                                 Action = "updateDnsRecords",
                                 Param = new UpdateDnsRecords
                                         {
                                             ApiKey         = NetCupApiKey,
                                             ApiSessionId   = apiSessionId,
                                             CustomerNumber = NetCupCustomerNumber,
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

                    using (var response = await HttpClient.SendAsync(request))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            var jsonResponse = await response.Content.ReadAsStringAsync();

                            var responseData = JsonConvert.DeserializeObject<ResponseDataUpdateDnsRecords>(jsonResponse);

                            if (responseData is { Status: SuccessState })
                            {
                                _log.LogInformation(
                                    "{ResponseLongMessage} | Domainname: {DomainName} | Response Code: {ResponseCode} | State: {Status} | Server request ID: {ServerRequestId}",
                                    responseData.LongMessage, domainName, responseData.StatusCode, responseData.Status, responseData.ServerRequestId);

                                return responseData;
                            }

                            _log.LogInformation("The dns records update for domain \"{DomainName}\" was failed.", domainName);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _log.LogError(ex, "Error while dns records update. Exception Message: {ExMessage}", ex.Message);
            }

            return null;
        }

    #endregion
    }
}
