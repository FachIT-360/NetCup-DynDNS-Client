// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RequestActionLogin.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using System.Text.Json.Serialization;

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class RequestActionLogin : RequestMessageBase
    {
        [JsonPropertyName("param")]
        public Login Param { get; set; } = new();
    }
}
