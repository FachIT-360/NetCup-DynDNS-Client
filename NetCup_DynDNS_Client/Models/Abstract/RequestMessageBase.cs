// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.Abstract
{
    public abstract class RequestMessageBase
    {
    #region Properties

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; } = null!;

    #endregion
    }
}
