// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class RequestActionLogin : RequestMessageBase
    {
    #region Properties

        [JsonProperty(PropertyName = "param")]
        public Login Param { get; set; } = new();

    #endregion
    }
}
