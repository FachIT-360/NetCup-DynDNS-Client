// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SourceGenerationContext.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models;

namespace FachIT360.Utils.Dns.NetCup.JsonSerializerContext
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(RequestActionLogin))]
    [JsonSerializable(typeof(RequestActionInfoDnsRecords))]
    [JsonSerializable(typeof(RequestActionUpdateDnsRecords))]
    [JsonSerializable(typeof(ResponseDataLogin))]
    [JsonSerializable(typeof(ResponseDataInfoDnsRecords))]
    [JsonSerializable(typeof(ResponseDataUpdateDnsRecords))]
    internal partial class SourceGenerationContext : System.Text.Json.Serialization.JsonSerializerContext { }
}
