// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.Abstract
{
    public abstract class ResponseMessageBase
    {
    #region Properties

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; } = null!;

        [JsonProperty(PropertyName = "clientrequestid")]
        public string? ClientRequestId { get; set; }

        [JsonProperty(PropertyName = "longmessage")]
        public string? LongMessage { get; set; }

        [JsonProperty(PropertyName = "shortmessage")]
        public string ShortMessage { get; set; } = null!;

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = null!;

        [JsonProperty(PropertyName = "statuscode")]
        public uint StatusCode { get; set; }

        [JsonProperty(PropertyName = "serverrequestid")]
        private string ServerRequestId { get; set; } = null!;

    #endregion
    }
}
