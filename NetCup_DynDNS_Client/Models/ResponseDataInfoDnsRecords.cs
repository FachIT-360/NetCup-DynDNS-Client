// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class ResponseDataInfoDnsRecords : ResponseMessageBase
    {
    #region Properties

        [JsonProperty(PropertyName = "responsedata")]
        public DnsRecords? ResponseData { get; set; }

    #endregion
    }
}
