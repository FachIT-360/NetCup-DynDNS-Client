// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

namespace FachIT360.Utils.Dns.NetCup.Models.RequestParamModels
{
    public class UpdateDnsRecords
    {
    #region Properties

        [JsonPropertyName("apikey")]
        public string ApiKey { get; set; } = null!;

        [JsonPropertyName("apisessionid")]
        public string ApiSessionId { get; set; } = null!;

        [JsonPropertyName("customernumber")]
        public uint CustomerNumber { get; set; }

        [JsonPropertyName("dnsrecordset")]
        public DnsRecords DnsRecordSet { get; set; } = null!;

        [JsonPropertyName("domainname")]
        public string DomainName { get; set; } = null!;

    #endregion
    }
}
