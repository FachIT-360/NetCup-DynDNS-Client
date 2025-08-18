// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class ResponseDataLogin : ResponseMessageBase
    {
    #region Properties

        [JsonPropertyName("responsedata")]
        public SessionId ResponseData { get; set; } = null!;

    #endregion
    }
}
