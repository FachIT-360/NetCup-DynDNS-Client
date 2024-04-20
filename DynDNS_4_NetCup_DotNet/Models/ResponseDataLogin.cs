// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ResponseDataApiSessionId.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;
using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class ResponseDataLogin : ResponseMessageBase
    {
        [JsonPropertyName("responsedata")]
        public SessionId ResponseData { get; set; } = null!;
    }
}
