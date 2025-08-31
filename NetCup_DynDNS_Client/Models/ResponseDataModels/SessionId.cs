// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class SessionId
    {
    #region Properties

        [JsonProperty(PropertyName = "apisessionid")]
        public string? ApiSessionId { get; set; }

    #endregion
    }
}
