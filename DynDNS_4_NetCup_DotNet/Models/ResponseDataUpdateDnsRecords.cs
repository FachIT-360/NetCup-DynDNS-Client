// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseDataUpdateDnsRecords.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class ResponseDataUpdateDnsRecords : ResponseMessageBase
    {
        [JsonPropertyName("responsedata")]
        public DnsRecords ResponseData { get; set; }
    }
}
