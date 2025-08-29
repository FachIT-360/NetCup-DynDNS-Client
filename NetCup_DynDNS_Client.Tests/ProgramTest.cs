// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Linq;
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
        private static HostApplicationBuilder _hostBuilder = null!;
        private static IHost _host = null!;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            _hostBuilder = Host.CreateApplicationBuilder();
            
            _hostBuilder.Services.Configure<NetCupApi>(_hostBuilder.Configuration.GetSection("NetCupApi"));
            _hostBuilder.Services.AddHttpClient<NetCupDynDnsApiClient>();

            _host = _hostBuilder.Build();
        }
        
        [TestMethod]
        public async Task BootstrapTest()
        {
            var host = await Program.Bootstrap(Array.Empty<string>());
            Assert.IsNotNull(host);
        }
        
        [TestMethod]
        public async Task UpdateDnsRecordsAsyncTest()
        {
            var httpClient = _host.Services.GetRequiredService<NetCupDynDnsApiClient>();
            var netCupConfig = _host.Services.GetRequiredService<IOptionsMonitor<NetCupApi>>().CurrentValue;
            
            var apiSessionId = await httpClient.LoginAsync(netCupConfig.Login);
            
            Assert.IsNotNull(apiSessionId);
            
            await httpClient.GetDnsRecordsForDomainAsync(apiSessionId, netCupConfig.Domains.First().Key);
            
            await httpClient.LogoutAsync(apiSessionId);
        }
    }
}
