// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class DnsRecords
    {
    #region Properties

        [JsonProperty(PropertyName = "dnsrecords")]
        public DnsRecord[] DnsRecordArray { get; set; } = [];

    #endregion
    }
}
