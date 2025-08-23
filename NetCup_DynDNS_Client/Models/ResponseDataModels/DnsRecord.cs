// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class DnsRecord
    {
    #region Properties

        [JsonPropertyName("deleterecord")]
        [JsonProperty(PropertyName = "deleterecord")]
        public bool? DeleteRecord { get; set; }

        [JsonPropertyName("destination")]
        [JsonProperty(PropertyName = "destination")]
        public string Destination { get; set; } = null!;

        [JsonPropertyName("hostname")]
        [JsonProperty(PropertyName = "hostname")]
        public string Hostname { get; set; } = null!;

        [JsonPropertyName("id")]
        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("priority")]
        [JsonProperty(PropertyName = "priority")]
        public string? Priority { get; set; }

        [JsonPropertyName("state")]
        [JsonProperty(PropertyName = "state")]
        public string? State { get; set; }

        [JsonPropertyName("type")]
        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = null!;

    #endregion
    }
}
