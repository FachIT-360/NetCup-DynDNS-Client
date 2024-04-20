// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestActionUpdateDnsRecords.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;
using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class RequestActionUpdateDnsRecords : RequestMessageBase
    {
        [JsonPropertyName("param")]
        public UpdateDnsRecords Param { get; set; } = null!;
    }
}
