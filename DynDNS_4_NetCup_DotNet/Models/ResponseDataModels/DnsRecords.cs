// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DnsRecords.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels
{
    public class DnsRecords
    {
        [JsonPropertyName("dnsrecords")]
        public DnsRecord[] DnsRecordArray { get; set; } = [];
    }
}
