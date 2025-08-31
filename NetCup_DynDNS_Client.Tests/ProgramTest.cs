// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

using FachIT360.Utils.Dns.NetCup;
using FachIT360.Utils.Dns.NetCup.Models.AppSettings;
using FachIT360.Utils.Dns.NetCup.Services;

using JetBrains.Annotations;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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

            _hostBuilder.Services.AddHttpClient<NetCupDynDnsApiClient>((sp, client) =>
                                                                           client.BaseAddress = sp.GetRequiredService<IOptionsMonitor<NetCupApi>>()
                                                                                                  .CurrentValue.EndpointUrl);

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
        }

        [TestMethod]
        public async Task CurrentPublicIpChangedAsyncTest()
        {
            var httpClient   = _host.Services.GetRequiredService<NetCupDynDnsApiClient>();
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;

            var apiSessionId = await httpClient.LoginAsync(netCupConfig.Login);
            Assert.IsNotNull(apiSessionId);

            // Enforce a fake IP change
            var fakePublicIp = new List<IPAddress>(new[] { IPAddress.Parse("192.168.127.12") });

            var publicIps = await httpClient.GetCurrentPublicIpAsync();

            Assert.IsNotNull(publicIps);
            Assert.IsNotEmpty(publicIps);

            var domainDnsRecords = await httpClient.GetDnsRecordsForDomainAsync(apiSessionId, netCupConfig.Domains.First().Key);
            _ = httpClient.CurrentPublicIpChanged(publicIps, domainDnsRecords, netCupConfig.Domains.First().Value, out var recordsToUpdate);

            var isLoggedOut = await httpClient.LogoutAsync(apiSessionId);
            Assert.IsTrue(isLoggedOut);
        }

        [TestMethod]
        public async Task GetCurrentPublicIpAsync()
        {
            var httpClient   = _host.Services.GetRequiredService<NetCupDynDnsApiClient>();
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;

            var apiSessionId = await httpClient.LoginAsync(netCupConfig.Login);
            Assert.IsNotNull(apiSessionId);

            var result = await httpClient.GetCurrentPublicIpAsync();
            Assert.IsNotNull(result);
            Assert.IsNotEmpty(result);

            var isLoggedOut = await httpClient.LogoutAsync(apiSessionId);

            Assert.IsTrue(isLoggedOut);
        }

        [TestMethod]
        public async Task GetDnsRecordsForDomainAsyncTest()
        {
            var httpClient   = _host.Services.GetRequiredService<NetCupDynDnsApiClient>();
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
        public async Task LogoutAsyncTest()
        {
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;
            var httpClient   = _host.Services.GetRequiredService<NetCupDynDnsApiClient>();

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

    #endregion
    }
}
