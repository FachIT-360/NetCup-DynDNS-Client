// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models.RequestParamModels
{
    public class InfoDnsRecords
    {
    #region Properties

        [JsonPropertyName("apikey")]
        public string ApiKey { get; set; } = null!;

        [JsonPropertyName("apisessionid")]
        public string ApiSessionId { get; set; } = null!;

        [JsonPropertyName("customernumber")]
        public uint CustomerNumber { get; set; }

        [JsonPropertyName("domainname")]
        public string DomainName { get; set; } = null!;

    #endregion
    }
}
