// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models.Abstract
{
    public abstract class ResponseMessageBase
    {
    #region Properties

        [JsonPropertyName("action")]
        public string Action { get; set; } = null!;

        [JsonPropertyName("clientrequestid")]
        public string? ClientRequestId { get; set; }

        [JsonPropertyName("longmessage")]
        public string? LongMessage { get; set; }

        [JsonPropertyName("shortmessage")]
        public string ShortMessage { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("statuscode")]
        public uint StatusCode { get; set; }

        [JsonPropertyName("serverrequestid")]
        private string ServerRequestId { get; set; } = null!;

    #endregion
    }
}
