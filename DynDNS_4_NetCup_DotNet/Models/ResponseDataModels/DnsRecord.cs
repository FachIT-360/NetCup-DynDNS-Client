// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class DnsRecord
    {
    #region Properties

        [JsonPropertyName("deleterecord")]
        public bool? DeleteRecord { get; set; }

        [JsonPropertyName("destination")]
        public string Destination { get; set; } = null!;

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; } = null!;

        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("priority")]
        public string? Priority { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }

        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

    #endregion
    }
}
