// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.JsonConverter;
using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class ResponseDataLogin : ResponseMessageBase
    {
    #region Properties

        [JsonPropertyName("responsedata")]
        [JsonProperty(PropertyName = "responsedata")]
        //[JsonConverter(typeof(ResponseDataSessionIdConverter))]
        public SessionId? ResponseData { get; set; }

    #endregion
    }
}
