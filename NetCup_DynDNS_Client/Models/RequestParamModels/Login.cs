// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.RequestParamModels
{
    public class Login
    {
    #region Properties

        [JsonProperty(PropertyName = "apikey")]
        public string? ApiKey { get; set; }

        [JsonProperty(PropertyName = "apipassword")]
        public string? ApiPassword { get; set; }

        [JsonProperty(PropertyName = "customernumber")]
        public uint? CustomerNumber { get; set; }

    #endregion
    }
}
