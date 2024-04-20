// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NetCupApi.cs" company="REGIOCAST GmbH & Co. KG">
// (C) 2024 by REGIOCAST GmbH und Co. KG
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;

namespace FachIT360.Utils.Dns.NetCup.Models.Settings
{
    public class NetCupApi
    {
        public string[] Domains { get; init; } = [];

        public Login Login { get; init; } = null!;
    }
}
