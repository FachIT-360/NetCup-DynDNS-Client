// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.RequestParamModels
{
    public class InfoDnsRecords
    {
    #region Properties

        [JsonPropertyName("apikey")]
        [JsonProperty(PropertyName = "apikey")]
        public string? ApiKey { get; set; } = null!;

        [JsonPropertyName("apisessionid")]
        [JsonProperty(PropertyName = "apisessionid")]
        public string ApiSessionId { get; set; } = null!;

        [JsonPropertyName("customernumber")]
        [JsonProperty(PropertyName = "customernumber")]
        public uint? CustomerNumber { get; set; }

        [JsonPropertyName("domainname")]
        [JsonProperty(PropertyName = "domainname")]
        public string DomainName { get; set; } = null!;

    #endregion
    }
}
