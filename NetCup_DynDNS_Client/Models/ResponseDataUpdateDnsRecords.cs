// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.JsonConverter;
using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class ResponseDataUpdateDnsRecords : ResponseMessageBase
    {
    #region Properties

        [JsonPropertyName("responsedata")]
        [JsonConverter(typeof(ResponseDataDnsRecordsConverter))]
        public DnsRecords? ResponseData { get; set; }

    #endregion
    }
}
