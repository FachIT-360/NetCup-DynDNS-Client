// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class DnsRecords
    {
    #region Properties

        [JsonPropertyName("dnsrecords")]
        [JsonProperty(PropertyName = "dnsrecords")]
        public DnsRecord[] DnsRecordArray { get; set; } = [];

    #endregion
    }
}
