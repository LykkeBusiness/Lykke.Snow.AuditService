// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;

namespace Lykke.Snow.AuditService.Client.Model.Proposal
{
    /// <summary>
    /// 
    /// </summary>
    public interface IAuditDomainDataTypeFilter
    {
        /// <summary>
        /// jsonDiff filter
        /// </summary>
        public Func<string, bool>? Filter { get; set; }
    }
    
    /// <summary>
    /// 
    /// </summary>
    public class AuditDomainDataTypeFilter : IAuditDomainDataTypeFilter
    {
        /// <inheritdoc />
        public Func<string, bool>? Filter { get; set; }
    }
}