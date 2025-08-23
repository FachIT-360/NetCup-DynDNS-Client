// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
// --------------------------------------------------------------------------------------------------------------------

using System.Buffers;
using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

namespace FachIT360.Utils.Dns.NetCup.JsonConverter
{
    public class ResponseDataDnsRecordsConverter : JsonConverter<DnsRecords>
    {
    #region Methods

        /// <summary>Reads and converts the JSON to type <typeparamref name = "T" />.</summary>
        /// <param name = "reader">The reader.</param>
        /// <param name = "typeToConvert">The type to convert.</param>
        /// <param name = "options">An object that specifies serialization options to use.</param>
        /// <returns>The converted value.</returns>
        [UnconditionalSuppressMessage(
            "AOT", "IL3050:Calling members annotated with 'RequiresDynamicCodeAttribute' may break functionality when AOT compiling.",
            Justification = "<Pending>")]
        [UnconditionalSuppressMessage(
            "Trimming",
            "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code",
            Justification = "<Pending>")]
        public override DnsRecords? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            if (reader.TokenType == JsonTokenType.String)
            {
                using var document = JsonDocument.ParseValue(ref reader);
                return null;
            }
            
            if (reader.TokenType == JsonTokenType.StartObject)
            {
                
                using var document = JsonDocument.ParseValue(ref reader);
                var root = document.RootElement;
                
                var apiSessionId = root.GetProperty("apisessionid").GetString();

                return string.IsNullOrWhiteSpace(apiSessionId) ? null : new DnsRecords();
            }

            return null;
        }

        /// <summary>Writes a specified value as JSON.</summary>
        /// <param name = "writer">The writer to write to.</param>
        /// <param name = "value">The value to convert to JSON.</param>
        /// <param name = "options">An object that specifies serialization options to use.</param>
        public override void Write(Utf8JsonWriter writer, DnsRecords? value, JsonSerializerOptions options)
        {
            writer.WriteStringValue(value?.ToString());
        }

    #endregion
    }
}
