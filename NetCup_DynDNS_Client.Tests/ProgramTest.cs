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
                                                                     client.BaseAddress = sp.GetRequiredService<IOptionsMonitor<NetCupApi>>()
                                                                                            .CurrentValue.EndpointUrl);
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
            var domainDnsRecords = GetDnsRecordsMockData();
            
            var ipChanged =
                workerTask.CurrentPublicIpChanged(fakePublicIp, domainDnsRecords, netCupConfig.Domains.First().Value, out var recordsToUpdate);
            
            Assert.IsTrue(ipChanged);
            Assert.AreSame(fakePublicIp[0].ToString(), recordsToUpdate[0].Destination);
            Assert.AreSame(fakePublicIp[0].ToString(), recordsToUpdate[1].Destination);
            
            ipChanged =
                workerTask.CurrentPublicIpChanged(fakePublicIp, null, netCupConfig.Domains.First().Value, out _);
            
            Assert.IsFalse(ipChanged);
        }
        
        [TestMethod]
        public async Task GetCurrentPublicIpAsync()
        {
            var workerTask = _host.Services.GetRequiredService<WorkerTask>();
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;

            // TODO: Test that respect IPv6 not implemented yet
            var result = await workerTask.GetCurrentPublicIpAsync(netCupConfig.MyIp4ApiUrl, netCupConfig.MyIp6ApiUrl);
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);
        }

        [TestMethod]
        public async Task GetDnsRecordsForDomainAsyncTest()
        {
            var httpClient   = _host.Services.GetRequiredService<NetCupApiClient>();
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;

            var apiSessionId = await httpClient.LoginAsync(netCupConfig.Login);

            Assert.IsNotNull(apiSessionId);

            var result = await httpClient.GetDnsRecordsForDomainAsync(apiSessionId, netCupConfig.Domains.First().Key);

            Assert.IsNotNull(result);
            Assert.IsNotNull(result.ResponseData);
            Assert.IsNotEmpty(result.ResponseData.DnsRecordArray);

            var isLoggedOut = await httpClient.LogoutAsync(apiSessionId);
            Assert.IsTrue(isLoggedOut);
        }

        [TestMethod]
        public async Task LoginAsyncTest()
        {
            var apiClient = _host.Services.GetRequiredService<NetCupApiClient>();

            var apiSessionId = await apiClient.LoginAsync("wrongApiKey", "wrongPassword", 0);
            Assert.IsNull(apiSessionId);

            // Force connection exception
            var wrongNetCupApiClient = GetNetCupApiClient("https://WrongApiUrl.com");

            apiSessionId = await wrongNetCupApiClient.LoginAsync("wrongApiKey", "wrongPassword", 0);
            Assert.IsNull(apiSessionId);
        }

        [TestMethod]
        public async Task LogoutAsyncTest()
        {
            var httpClient   = _host.Services.GetRequiredService<NetCupApiClient>();
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;

            // Login
            var apiSessionId = await httpClient.LoginAsync(
                                   netCupConfig.Login.ApiKey!,
                                   netCupConfig.Login.ApiPassword!,
                                   netCupConfig.Login.CustomerNumber!.Value);

            Assert.IsNotNull(apiSessionId);

            // Test successful LogOut
            var isLoggedOut = await httpClient.LogoutAsync(apiSessionId);
            Assert.IsTrue(isLoggedOut);
        }

        [TestMethod]
        public async Task UpdateDnsRecordsAsyncTest()
        {
            var httpClient   = _host.Services.GetRequiredService<NetCupApiClient>();
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;

            // Login
            var apiSessionId = await httpClient.LoginAsync(netCupConfig.Login);
            Assert.IsNotNull(apiSessionId);

            var result = await httpClient.UpdateDnsRecordsAsync(apiSessionId, netCupConfig.Domains.First().Key,
                                                                GetDnsRecordsMockData().ResponseData?.DnsRecordArray.ToList() ?? []);

            Assert.IsNotNull(result);
            
            // Logout
            var isLoggedOut = await httpClient.LogoutAsync(apiSessionId);
            Assert.IsTrue(isLoggedOut);
        }

        private static ResponseDataInfoDnsRecords GetDnsRecordsMockData()
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
                                                  Destination  = "202.61.232.217",
                                                  Hostname     = "*",
                                                  Id           = "77098486",
                                                  Priority     = "0",
                                                  State        = "yes",
                                                  Type         = "A"
                                              },
                                              new()
                                              {
                                                  DeleteRecord = false,
                                                  Destination  = "202.61.232.217",
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

        private NetCupApiClient GetNetCupApiClient(string baseUrl)
        {
            var logger = _host.Services.GetRequiredService<ILogger<NetCupApiClient>>();

            var httpClient = new HttpClient
                             {
                                 BaseAddress = new Uri(baseUrl)
                             };

            return new NetCupApiClient(logger, httpClient);
        }

    #endregion
    }
}
