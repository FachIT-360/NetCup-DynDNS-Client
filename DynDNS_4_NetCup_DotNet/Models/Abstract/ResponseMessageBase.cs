// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseMessageBase.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models.Abstract
{
    public abstract class ResponseMessageBase
    {
        [JsonPropertyName("serverrequestid")]
        private string ServerRequestId { get; set; } = null!;

        [JsonPropertyName("clientrequestid")]
        public string? ClientRequestId { get; set; }

        [JsonPropertyName("action")]
        public string Action { get; set; } = null!;

        [JsonPropertyName("status")]
        public string Status { get; set; } = null!;

        [JsonPropertyName("statuscode")]
        public uint StatusCode { get; set; }

        [JsonPropertyName("shortmessage")]
        public string ShortMessage { get; set; } = null!;

        [JsonPropertyName("longmessage")]
        public string? LongMessage { get; set; }
    }
}
