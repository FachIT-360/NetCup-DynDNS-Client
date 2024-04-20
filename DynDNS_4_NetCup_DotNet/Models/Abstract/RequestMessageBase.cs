// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestMessage.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

namespace FachIT360.Utils.Dns.NetCup.Models.Abstract
{
    public abstract class RequestMessageBase
    {
        [JsonPropertyName("action")]
        public string Action { get; set; } = null!;
    }
}
