// --------------------------------------------------------------------------------------------------------------------
// SPDX License Identifier: MIT
// Copyright (c) 2025 by FachIT 360 - Marcus Reinhart
// See the LICENSE file in the project root directory for details.
// --------------------------------------------------------------------------------------------------------------------

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models.Abstract
{
    public abstract class RequestMessageBase
    {
    #region Properties

        [JsonProperty(PropertyName = "action")]
        public string Action { get; set; } = null!;

    #endregion
    }
}
