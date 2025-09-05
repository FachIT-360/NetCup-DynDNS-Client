// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;

using FachIT360.Utils.Dns.NetCup.Models;
using FachIT360.Utils.Dns.NetCup.Models.AppSettings;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

using Microsoft.Extensions.Options;

namespace FachIT360.Utils.Dns.NetCup.Services
{
    public class WorkerTask(IServiceProvider serviceProvider, ILogger<WorkerTask> log)
    {
    #region Methods

        public bool CurrentPublicIpChanged(List<IPAddress> currentPublicIps,
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
                if (!IPAddress.TryParse(dnsRecord.Destination, out var dnsRecordIp))
                {
                    continue;
                }

                // Check an IP Address of hosts has changed
                foreach (var currentPublicIp in currentPublicIps.Where(currentPublicIp => currentPublicIp.AddressFamily == dnsRecordIp.AddressFamily))
                {
                    if (currentPublicIp.Equals(dnsRecordIp))
                    {
                        log.LogDebug("The ip of dns record \"{DnsRecordHostname}\" with type \"{DnsRecordType}\" has not changed.",
                                     dnsRecord.Hostname, dnsRecord.Type);

                        continue;
                    }

                    log.LogInformation(
                        "The ip of dns record \"{DnsRecordHostname}\" with type \"{DnsRecordType}\" has changed to \"{CurrentPublicIp}\".",
                        dnsRecord.Hostname, dnsRecord.Type, currentPublicIp);

                    dnsRecord.Destination = currentPublicIp.ToString();
                    recordsToUpdate.Add(dnsRecord);
                    hasChanged = true;
                }
            }

            return hasChanged;
        }

        /// <summary>
        ///     Get current public ip addresses.
        /// </summary>
        /// <returns>Returns all public IP addresses in IPv4 and / or IPv6 if available.</returns>
        public async Task<List<IPAddress>> GetCurrentPublicIpAsync(string ipv4ApiAddress, string ipv6ApiAddress)
        {
            try
            {
                var publicAddresses = new List<IPAddress>();
                
                var httpClientIpV4 = new HttpClient();
                httpClientIpV4.BaseAddress = new(ipv4ApiAddress, UriKind.Absolute);
                    
                if (IPAddress.TryParse(await httpClientIpV4.GetStringAsync((string?)null), out var currentIpAddressV4) &&
                    currentIpAddressV4.AddressFamily == AddressFamily.InterNetwork)
                {
                    log.LogDebug("The current public ip v4 address is: {CurrentIpAddressV4}", currentIpAddressV4);
                    publicAddresses.Add(currentIpAddressV4);
                }

                var httpClientIpV6 = new HttpClient();
                httpClientIpV6.BaseAddress = new(ipv6ApiAddress, UriKind.Absolute);
                if (IPAddress.TryParse(await httpClientIpV6.GetStringAsync((string?)null), out var currentIpAddressV6) &&
                    currentIpAddressV6.AddressFamily == AddressFamily.InterNetworkV6)
                {
                    log.LogDebug("The current public ip v6 address is: {CurrentIpAddressV6}", currentIpAddressV6);
                    publicAddresses.Add(currentIpAddressV6);
                }
                
                return publicAddresses;
            }
            catch (Exception ex)
            {
                log.LogError(ex, "Error while getting current public ip. Exception Message: {ExMessage}", ex.Message);

                return [];
            }
        }

        [ExcludeFromCodeCoverage]
        public async Task StartSyncDnsRecordsAsync(CancellationToken stoppingToken)
        {
            await using (var scope = serviceProvider.CreateAsyncScope())
            {
                var netCupApiClient  = scope.ServiceProvider.GetRequiredService<NetCupApiClient>();
                var netCupApiOptions = scope.ServiceProvider.GetRequiredService<IOptionsMonitor<NetCupApi>>();

                var apiSessionId = await netCupApiClient.LoginAsync(netCupApiOptions.CurrentValue.Login);

                var currentPublicIpAddresses =
                    await netCupApiClient.GetCurrentPublicIpAsync(netCupApiOptions.CurrentValue.MyIp4ApiUrl,
                                                                  netCupApiOptions.CurrentValue.MyIp6ApiUrl);

                if (apiSessionId == null)
                {
                    log.LogError("ApiSessionId is null");

                    return;
                }

                foreach (var (domainName, recordNamesToUpdate) in netCupApiOptions.CurrentValue.Domains)
                {
                    // Get DNS records for domain
                    var currentDomainDnsRecords = await netCupApiClient.GetDnsRecordsForDomainAsync(apiSessionId, domainName);

                    if (CurrentPublicIpChanged(currentPublicIpAddresses, currentDomainDnsRecords, recordNamesToUpdate, out var recordsToUpdate))
                    {
                        await netCupApiClient.UpdateDnsRecordsAsync(apiSessionId, domainName, recordsToUpdate);
                    }
                }

                await netCupApiClient.LogoutAsync(apiSessionId);

                await Task.Delay(netCupApiOptions.CurrentValue.RequestInterval!.Value, stoppingToken);
            }
        }

    #endregion
    }
}
