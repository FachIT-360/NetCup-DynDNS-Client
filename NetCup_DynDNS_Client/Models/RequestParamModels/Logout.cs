// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.RequestParamModels
{
    public class Logout
    {
    #region Properties

        [JsonProperty(PropertyName = "apikey")]
        public string ApiKey { get; set; } = null!;

        [JsonProperty(PropertyName = "apisessionid")]
        public string ApiSessionId { get; set; } = null!;

        [JsonProperty(PropertyName = "customernumber")]
        public uint CustomerNumber { get; set; }

    #endregion
    }
}
