// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class ResponseDataLogin : ResponseMessageBase
    {
    #region Properties

        [JsonProperty(PropertyName = "responsedata")]
        //[JsonConverter(typeof(ResponseDataSessionIdConverter))]
        public SessionId? ResponseData { get; set; }

    #endregion
    }
}
