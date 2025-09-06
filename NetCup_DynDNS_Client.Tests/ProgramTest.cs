// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

using FachIT360.Utils.Dns.NetCup;
using FachIT360.Utils.Dns.NetCup.Models;
using FachIT360.Utils.Dns.NetCup.Models.AppSettings;
using FachIT360.Utils.Dns.NetCup.Services;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetCup_DynDNS_Client.Tests
{
    [TestClass]
    [TestSubject(typeof(Program))]
    public class ProgramTest
    {
    #region Constants - Static fields - Fields

        private static IHost                  _host        = null!;
        private static HostApplicationBuilder _hostBuilder = null!;

    #endregion

    #region Methods

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _hostBuilder = Host.CreateApplicationBuilder();

            _hostBuilder.Services.Configure<NetCupApi>(_hostBuilder.Configuration.GetSection("NetCupApi"));

            _hostBuilder.Services.AddHttpClient<NetCupApiClient>((sp, client) =>
                                                                 {
                                                                     client.BaseAddress = sp.GetRequiredService<IOptionsMonitor<NetCupApi>>()
                                                                                            .CurrentValue.EndpointUrl;
                                                                     client.DefaultRequestVersion = HttpVersion.Version20;
                                                                 });

            _hostBuilder.Services.AddSingleton<WorkerTask>();

            _host = _hostBuilder.Build();

            _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiKey =
                _hostBuilder.Configuration["NETCUP_LOGIN_APIKEY"];

            Assert.IsFalse(string.IsNullOrWhiteSpace(_host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiKey));

            _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiPassword =
                _hostBuilder.Configuration["NETCUP_LOGIN_APIPASSWORD"];

            Assert.IsFalse(string.IsNullOrWhiteSpace(_host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.ApiPassword));

            _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.CustomerNumber =
                Convert.ToUInt32(_hostBuilder.Configuration["NETCUP_LOGIN_CUSTOMERNUMBER"]);

            Assert.IsTrue(_host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue.Login.CustomerNumber.HasValue);
        }

        [TestMethod]
        public async Task ApiGetDnsRecordsForDomainAsyncTest()
        {
            // Get NetCupApiClient
            var config          = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;
            var netCupApiClient = GetNetCupApiClient(config.EndpointUrl!.ToString());
            Assert.IsNotNull(netCupApiClient);
            Assert.IsNotNull(netCupApiClient.HttpClient);
            Assert.IsNotNull(netCupApiClient.HttpClient.BaseAddress);
            Assert.AreEqual(config.EndpointUrl, netCupApiClient.HttpClient.BaseAddress);

            // Login to NetCup API and get API Session ID
            var apiSessionId = await netCupApiClient.LoginAsync(config.Login);
            Assert.IsNotNull(apiSessionId);

            // Get DNS records for domain
            var result = await netCupApiClient.GetDnsRecordsForDomainAsync(apiSessionId, config.Domains.First().Key);
            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ResponseData);
            Assert.IsNotEmpty(result.ResponseData.DnsRecordArray);

            // Logout from NetCup API
            var isLoggedOut = await netCupApiClient.LogoutAsync(apiSessionId);
            Assert.IsTrue(isLoggedOut);
        }

        [TestMethod]
        public async Task ApiLoginAsyncTest()
        {
            // Get NetCupApiClient
            var config          = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;
            var netCupApiClient = GetNetCupApiClient(config.EndpointUrl!.ToString());
            Assert.IsNotNull(netCupApiClient);
            Assert.IsNotNull(netCupApiClient.HttpClient);
            Assert.IsNotNull(netCupApiClient.HttpClient.BaseAddress);
            Assert.AreEqual(config.EndpointUrl, netCupApiClient.HttpClient.BaseAddress);

            // Login to NetCup API with wrong credentials
            var apiSessionId = await netCupApiClient.LoginAsync("wrongApiKey", "wrongPassword", 0);
            Assert.IsNull(apiSessionId);

            // Get NetCupApiClient with a wrong API URL
            var wrongNetCupApiClient = GetNetCupApiClient("https://WrongApiUrl.com");
            Assert.IsNotNull(wrongNetCupApiClient);
            Assert.IsNotNull(wrongNetCupApiClient.HttpClient);
            Assert.IsNotNull(wrongNetCupApiClient.HttpClient.BaseAddress);
            Assert.AreEqual(new Uri("https://WrongApiUrl.com"), wrongNetCupApiClient.HttpClient.BaseAddress);

            // Force connection exception with a wrong API URL
            apiSessionId = await wrongNetCupApiClient.LoginAsync("wrongApiKey", "wrongPassword", 0);
            Assert.IsNull(apiSessionId);
        }

        [TestMethod]
        public async Task ApiLogoutAsyncTest()
        {
            // Get NetCupApiClient
            var config          = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;
            var netCupApiClient = GetNetCupApiClient(config.EndpointUrl!.ToString());
            Assert.IsNotNull(netCupApiClient);
            Assert.IsNotNull(netCupApiClient.HttpClient);
            Assert.IsNotNull(netCupApiClient.HttpClient.BaseAddress);
            Assert.AreEqual(config.EndpointUrl, netCupApiClient.HttpClient.BaseAddress);

            // Login to NetCup API and get API Session ID
            var apiSessionId = await netCupApiClient.LoginAsync(
                                   config.Login.ApiKey!,
                                   config.Login.ApiPassword!,
                                   config.Login.CustomerNumber!.Value);

            Assert.IsNotNull(apiSessionId);

            // Test successful LogOut
            var isLoggedOut = await netCupApiClient.LogoutAsync(apiSessionId);
            Assert.IsTrue(isLoggedOut);
            
            // Get NetCupApiClient with a wrong API URL
            var wrongNetCupApiClient = GetNetCupApiClient("https://WrongApiUrl.com");
            Assert.IsNotNull(wrongNetCupApiClient);
            Assert.IsNotNull(wrongNetCupApiClient.HttpClient);
            Assert.IsNotNull(wrongNetCupApiClient.HttpClient.BaseAddress);
            Assert.AreEqual(new Uri("https://WrongApiUrl.com"), wrongNetCupApiClient.HttpClient.BaseAddress);
            
            // Force connection exception with a wrong API URL
            isLoggedOut = await wrongNetCupApiClient.LogoutAsync(apiSessionId);
            Assert.IsFalse(isLoggedOut);
        }

        [TestMethod]
        public async Task ApiUpdateDnsRecordsAsyncTest()
        {
            // Get NetCupApiClient
            var config          = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;
            var netCupApiClient = GetNetCupApiClient(config.EndpointUrl!.ToString());
            Assert.IsNotNull(netCupApiClient);
            Assert.IsNotNull(netCupApiClient.HttpClient);
            Assert.IsNotNull(netCupApiClient.HttpClient.BaseAddress);
            Assert.AreEqual(config.EndpointUrl, netCupApiClient.HttpClient.BaseAddress);

            // Login to NetCup API and get API Session ID
            var apiSessionId = await netCupApiClient.LoginAsync(config.Login);
            Assert.IsNotNull(apiSessionId);

            // Get current DNS records for restore
            var currentDnsRecordsForRestore = await netCupApiClient.GetDnsRecordsForDomainAsync(apiSessionId, config.Domains.First().Key);
            Assert.IsNotNull(currentDnsRecordsForRestore);
            Assert.IsNotNull(currentDnsRecordsForRestore.ResponseData);
            Assert.IsNotEmpty(currentDnsRecordsForRestore.ResponseData.DnsRecordArray);

            var result = await netCupApiClient.UpdateDnsRecordsAsync(apiSessionId, config.Domains.First().Key,
                                                                     GetDnsRecordsMockData("202.202.202.202", "202.202.202.202")
                                                                         .ResponseData?.DnsRecordArray.ToList() ?? []);

            Assert.IsNotNull(result);

            await Task.Delay(1000, CancellationToken.None);

            // Restore DNS records
            var result2 = await netCupApiClient.UpdateDnsRecordsAsync(apiSessionId, config.Domains.First().Key,
                                                                      currentDnsRecordsForRestore.ResponseData?.DnsRecordArray
                                                                                                 .Where(item => item.Hostname is "*" or "@")
                                                                                                 .ToList() ?? []);

            Assert.IsNotNull(result2);

            // Logout
            var isLoggedOut = await netCupApiClient.LogoutAsync(apiSessionId);
            Assert.IsTrue(isLoggedOut);
        }

        [TestMethod]
        public async Task BootstrapTest()
        {
            var host = await Program.Bootstrap(Array.Empty<string>());
            Assert.IsNotNull(host);

            var apiClient = host.Services.GetRequiredService<NetCupApiClient>();
            Assert.IsNotNull(apiClient);
        }

        [TestMethod]
        public void CurrentPublicIpChangedAsyncTest()
        {
            var workerTask   = _host.Services.GetRequiredService<WorkerTask>();
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;

            // Enforce a fake IP change
            var fakePublicIp = new List<IPAddress>([IPAddress.Parse("192.168.127.12")]);

            // Comes usually from NetCupApiClient.GetDnsRecordsForDomainAsync() Method
            var domainDnsRecords = GetDnsRecordsMockData("202.61.232.217", "202.61.232.217");

            var ipChanged =
                workerTask.CurrentPublicIpChanged(fakePublicIp, domainDnsRecords, netCupConfig.Domains.First().Value, out var recordsToUpdate);

            Assert.IsTrue(ipChanged);
            Assert.AreSame(fakePublicIp[0].ToString(), recordsToUpdate[0].Destination);
            Assert.AreSame(fakePublicIp[0].ToString(), recordsToUpdate[1].Destination);

            ipChanged =
                workerTask.CurrentPublicIpChanged(fakePublicIp, null, netCupConfig.Domains.First().Value, out _);

            Assert.IsFalse(ipChanged);
        }

        private static ResponseDataInfoDnsRecords GetDnsRecordsMockData(string destIp1, string destIp2)
        {
            return new ResponseDataInfoDnsRecords
                   {
                       Action          = "infoDnsRecords",
                       ClientRequestId = "",
                       LongMessage     = "DNS Records for this zone were found.",
                       ShortMessage    = "DNS records found",
                       Status          = "success",
                       StatusCode      = 2000,
                       ResponseData = new()
                                      {
                                          DnsRecordArray =
                                          [
                                              new()
                                              {
                                                  DeleteRecord = false,
                                                  Destination  = destIp1,
                                                  Hostname     = "*",
                                                  Id           = "77098486",
                                                  Priority     = "0",
                                                  State        = "yes",
                                                  Type         = "A"
                                              },
                                              new()
                                              {
                                                  DeleteRecord = false,
                                                  Destination  = destIp2,
                                                  Hostname     = "@",
                                                  Id           = "77098487",
                                                  Priority     = "0",
                                                  State        = "yes",
                                                  Type         = "A"
                                              }
                                          ]
                                      }
                   };
        }

        private static NetCupApiClient GetNetCupApiClient(string baseUrl)
        {
            var logger = _host.Services.GetRequiredService<ILogger<NetCupApiClient>>();

            var httpClient = new HttpClient
                             {
                                 BaseAddress = new Uri(baseUrl),
                                 DefaultRequestVersion = HttpVersion.Version20,
                             };

            return new NetCupApiClient(logger, httpClient);
        }

    #endregion
    }
}
