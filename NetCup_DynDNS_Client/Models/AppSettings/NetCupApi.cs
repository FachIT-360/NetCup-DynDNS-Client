// --------------------------------------------------------------------------------------------------------------------
// (C) 2025 by FachIT360 - Marcus Reinhart
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

        public TimeSpan? RequestInterval { get; set; } = TimeSpan.FromMinutes(5);

    #endregion
    }
}
