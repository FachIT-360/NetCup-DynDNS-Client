// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class RequestActionInfoDnsRecords : RequestMessageBase
    {
    #region Properties

        [JsonPropertyName("param")]
        [JsonProperty(PropertyName = "param")]
        public InfoDnsRecords Param { get; set; } = null!;

    #endregion
    }
}
