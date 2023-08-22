// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;

namespace Lykke.Snow.AuditService.Client.Model.Proposal
{
    /// <summary>
    /// 
    /// </summary>
    public class GetAuditEventsRequest
    {
        /// <summary>
        /// 
        /// </summary>
        public DateTime? StartDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? EndDateTime { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? UserName { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? CorrelationId { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public AuditEventType? AuditEventType { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string? ReferenceId { get; set; }
        /// <summary>
        /// AuditDomain (e.g. Assets, Axle, Meteor etc.) to filter mapping
        /// </summary>
        public Dictionary<AuditDomain, IAuditDomainFilter?>? DomainFilters { get; set; }
    }
}