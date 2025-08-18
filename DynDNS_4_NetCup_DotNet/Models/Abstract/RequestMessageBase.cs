// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models.Abstract
{
    public abstract class RequestMessageBase
    {
    #region Properties

        [JsonPropertyName("action")]
        public string Action { get; set; } = null!;

    #endregion
    }
}
