// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DnsRecordInfo.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

namespace FachIT360.Utils.Dns.NetCup.Models.RequestParamModels
{
    public class UpdateDnsRecords
    {
        [JsonPropertyName("domainname")]
        public string DomainName { get; set; } = null!;

        [JsonPropertyName("customernumber")]
        public uint CustomerNumber { get; set; }

        [JsonPropertyName("apikey")]
        public string ApiKey { get; set; } = null!;

        [JsonPropertyName("apisessionid")]
        public string ApiSessionId { get; set; } = null!;

        [JsonPropertyName("dnsrecordset")]
        public DnsRecords DnsRecordSet { get; set; } = null!;
    }
}
