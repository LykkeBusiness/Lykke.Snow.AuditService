// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.Audit;

namespace Lykke.Snow.AuditService.Client.Model
{
    /// <summary>
    /// Request class for listing audit events.
    /// </summary>
    public class GetAuditEventsRequest<TActionTypeDetails> 
    {
        /// <summary>
        /// Starting point in time for the audit events
        /// </summary>
        public DateTime? StartDateTime { get; set; }

        /// <summary>
        /// End point in time for the audit events
        /// </summary>
        public DateTime? EndDateTime { get; set; }

        /// <summary>
        /// CorrelationId of the Audit event
        /// </summary>
        public string? CorrelationId { get; set; }

        /// <summary>
        /// Reference of the Audit event (DataReference)
        /// </summary>
        public string? ReferenceID { get; set; }

        /// <summary>
        /// Username of the Audit event
        /// </summary>
        public string? UserName { get; set; }

        /// <summary>
        /// Action type for the audit events
        /// </summary>
        public AuditEventType? ActionType { get; set; }

        /// <summary>
        /// Action type details
        /// </summary>
        public TActionTypeDetails AuditEventTypeDetails { get; set; } = default!;
    }
}