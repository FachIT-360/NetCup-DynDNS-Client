// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class DnsRecord
    {
    #region Properties

        [JsonProperty(PropertyName = "deleterecord")]
        public bool? DeleteRecord { get; set; }

        [JsonProperty(PropertyName = "destination")]
        public string Destination { get; set; } = null!;

        [JsonProperty(PropertyName = "hostname")]
        public string Hostname { get; set; } = null!;

        [JsonProperty(PropertyName = "id")]
        public string Id { get; set; } = null!;

        [JsonProperty(PropertyName = "priority")]
        public string? Priority { get; set; }

        [JsonProperty(PropertyName = "state")]
        public string? State { get; set; }

        [JsonProperty(PropertyName = "type")]
        public string Type { get; set; } = null!;

    #endregion
    }
}
