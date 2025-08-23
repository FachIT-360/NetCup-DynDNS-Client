// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class SessionId
    {
    #region Properties

        [JsonPropertyName("apisessionid")]
        [JsonProperty(PropertyName = "apisessionid")]
        public string ApiSessionId { get; set; }

    #endregion
    }
}
