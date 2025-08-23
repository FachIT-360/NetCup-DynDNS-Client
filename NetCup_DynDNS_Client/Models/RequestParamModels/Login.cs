// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.RequestParamModels
{
    public class Login
    {
    #region Properties

        [JsonPropertyName("apikey")]
        [JsonProperty(PropertyName = "apikey")]
        public string? ApiKey { get; set; }

        [JsonPropertyName("apipassword")]
        [JsonProperty(PropertyName = "apipassword")]
        public string? ApiPassword { get; set; } = null!;

        [JsonPropertyName("customernumber")]
        [JsonProperty(PropertyName = "customernumber")]
        public uint? CustomerNumber { get; set; }

    #endregion
    }
}
