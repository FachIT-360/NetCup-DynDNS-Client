// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System;
using System.Runtime.InteropServices.JavaScript;
using System.Threading.Tasks;

using FachIT360.Utils.Dns.NetCup;

using JetBrains.Annotations;

using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NetCup_DynDNS_Client.Tests
{
    [TestClass]
    [TestSubject(typeof(Program))]
    public class ProgramTest
    {

        [TestMethod]
        public async Task BootstrapTest()
        {
            var host = await Program.Bootstrap(Array.Empty<string>());
            Assert.IsNotNull(host);
        }
    }
}
