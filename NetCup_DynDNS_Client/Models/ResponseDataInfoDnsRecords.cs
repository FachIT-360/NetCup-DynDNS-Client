// --------------------------------------------------------------------------------------------------------------------
// SPDX License Identifier: MIT
// Copyright (c) 2025 by FachIT 360 - Marcus Reinhart
// See the LICENSE file in the project root directory for details.
// --------------------------------------------------------------------------------------------------------------------

using FachIT360.Utils.Dns.NetCup.Models.Abstract;
using FachIT360.Utils.Dns.NetCup.Models.ResponseDataModels;

using Newtonsoft.Json;

namespace FachIT360.Utils.Dns.NetCup.Models
{
    public class ResponseDataInfoDnsRecords : ResponseMessageBase
    {
    #region Properties

        [JsonProperty(PropertyName = "responsedata")]
        public DnsRecords? ResponseData { get; set; }

    #endregion
    }
}
