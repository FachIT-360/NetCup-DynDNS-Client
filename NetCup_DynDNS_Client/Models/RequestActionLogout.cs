// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class RequestActionLogout : RequestMessageBase
    {
    #region Properties

        [JsonProperty(PropertyName = "param")]
        public Logout Param { get; set; } = new();

    #endregion
    }
}
