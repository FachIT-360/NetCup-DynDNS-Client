// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestActionGetDnsInfoSet.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;
using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class RequestActionInfoDnsRecords : RequestMessageBase
    {
        [JsonPropertyName("param")]
        public InfoDnsRecords Param { get; set; } = null!;
    }
}
