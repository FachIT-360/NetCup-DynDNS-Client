// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.Abstract
{
    public abstract class ResponseMessageBase
    {
    #region Properties

        [JsonPropertyName("action")]
        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; } = null!;

        [JsonPropertyName("clientrequestid")]
        [JsonProperty(PropertyName = "clientrequestid")]
        public string? ClientRequestId { get; set; }

        [JsonPropertyName("longmessage")]
        [JsonProperty(PropertyName = "longmessage")]
        public string? LongMessage { get; set; }

        [JsonPropertyName("shortmessage")]
        [JsonProperty(PropertyName = "shortmessage")]
        public string ShortMessage { get; set; } = null!;

        [JsonPropertyName("status")]
        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("statuscode")]
        [JsonProperty(PropertyName = "statuscode")]
        public uint StatusCode { get; set; }

        [JsonPropertyName("serverrequestid")]
        [JsonProperty(PropertyName = "serverrequestid")]
        private string ServerRequestId { get; set; } = null!;

    #endregion
    }
}
