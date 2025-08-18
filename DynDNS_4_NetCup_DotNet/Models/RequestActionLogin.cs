// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class RequestActionLogin : RequestMessageBase
    {
    #region Properties

        [JsonPropertyName("param")]
        public Login Param { get; set; } = new();

    #endregion
    }
}
