// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DnsRecord.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class DnsRecord
    {
        [JsonPropertyName("id")]
        public string Id { get; set; } = null!;

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; } = null!;

        [JsonPropertyName("type")]
        public string Type { get; set; } = null!;

        [JsonPropertyName("priority")]
        public string? Priority { get; set; }

        [JsonPropertyName("destination")]
        public string Destination { get; set; } = null!;

        [JsonPropertyName("deleterecord")]
        public bool? DeleteRecord { get; set; }

        [JsonPropertyName("state")]
        public string? State { get; set; }
    }
}
