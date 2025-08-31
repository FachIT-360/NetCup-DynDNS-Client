// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.RequestParamModels
{
    public class UpdateDnsRecords
    {
    #region Properties

        [JsonProperty(PropertyName = "apikey")]
        public string ApiKey { get; set; } = null!;

        [JsonProperty(PropertyName = "apisessionid")]
        public string ApiSessionId { get; set; } = null!;

        [JsonProperty(PropertyName = "customernumber")]
        public uint CustomerNumber { get; set; }

        [JsonProperty(PropertyName = "dnsrecordset")]
        public DnsRecords DnsRecordSet { get; set; } = null!;

        [JsonProperty(PropertyName = "domainname")]
        public string DomainName { get; set; } = null!;

    #endregion
    }
}
