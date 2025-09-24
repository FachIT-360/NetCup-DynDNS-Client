// --------------------------------------------------------------------------------------------------------------------
// SPDX License Identifier: MIT
// Copyright (c) 2025 by FachIT 360 - Marcus Reinhart
// See the LICENSE file in the project root directory for details.
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.RequestParamModels;

namespace FachIT360.Utils.Dns.NetCup.Models.AppSettings
{
    public class NetCupApi
    {
    #region Properties

        public Dictionary<string, List<string>> Domains { get; set; } = [];

        public Uri? EndpointUrl { get; set; } = null!;

        public Login Login { get; set; } = new();

        public string MyIp4ApiUrl { get; set; } = null!;

        public string MyIp6ApiUrl { get; set; } = null!;

        public TimeSpan? RequestInterval { get; set; } = TimeSpan.FromMinutes(5);

    #endregion
    }
}
