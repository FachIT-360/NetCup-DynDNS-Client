// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models;

namespace FachIT360.Utils.Dns.NetCup.JsonSerializerContext
{
    [JsonSourceGenerationOptions(WriteIndented = true)]
    [JsonSerializable(typeof(RequestActionLogin))]
    [JsonSerializable(typeof(RequestActionLogout))]
    [JsonSerializable(typeof(RequestActionInfoDnsRecords))]
    [JsonSerializable(typeof(RequestActionUpdateDnsRecords))]
    [JsonSerializable(typeof(ResponseDataLogin))]
    [JsonSerializable(typeof(ResponseDataLogout))]
    [JsonSerializable(typeof(ResponseDataInfoDnsRecords))]
    [JsonSerializable(typeof(ResponseDataUpdateDnsRecords))]
    internal partial class SourceGenerationContext : System.Text.Json.Serialization.JsonSerializerContext { }
}
