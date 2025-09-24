// --------------------------------------------------------------------------------------------------------------------
// SPDX License Identifier: MIT
// Copyright (c) 2025 by FachIT 360 - Marcus Reinhart
// See the LICENSE file in the project root directory for details.
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

        [JsonProperty(PropertyName = "serverrequestid")]
        public string ServerRequestId { get; set; } = null!;

        [JsonProperty(PropertyName = "shortmessage")]
        public string ShortMessage { get; set; } = null!;

        [JsonProperty(PropertyName = "status")]
        public string Status { get; set; } = null!;

        [JsonProperty(PropertyName = "statuscode")]
        public uint StatusCode { get; set; }

    #endregion
    }
}
