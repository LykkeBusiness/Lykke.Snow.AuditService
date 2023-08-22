// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;

namespace Lykke.Snow.AuditService.Client.Model.Proposal
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuditDomainFilter
    {
        /// <summary>
        /// AuditDomainDataType (e.g. Rfq, Order, Regulation) to filter mapping
        /// </summary>
        public Dictionary<string, IAuditDomainDataTypeFilter?> DataTypeFilters { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class AuditDomainFilter : IAuditDomainFilter
    {
        /// <inheritdoc />
        public Dictionary<string, IAuditDomainDataTypeFilter?> DataTypeFilters { get; set; } = new Dictionary<string, IAuditDomainDataTypeFilter?>();
    }
}