// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum.ActionTypes;
using MarginTrading.Backend.Contracts.Rfq;

namespace Lykke.Snow.AuditService.Client.Model.Request.Rfq
{
    /// <summary>
    /// Request class for listing RFQ audit events.
    /// </summary>
    public class GetRfqAuditEventsRequest
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
        /// Action type for the audit events
        /// </summary>
        public AuditEventType? ActionType { get; set; }
        
        /// <summary>
        /// Refined edit action type
        /// </summary>  
        public RfqRefinedEditActionType? RefinedEditActionType { get; set; }
        
        /// <summary>
        /// Rfq state
        /// </summary>
        public RfqOperationState? State { get; set; }
    }
}