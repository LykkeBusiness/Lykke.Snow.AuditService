using System;

using Lykke.Snow.Audit;

using MarginTrading.Backend.Contracts.Rfq;

namespace Lykke.Snow.AuditService.Client.Model.Request
{
    /// <summary>
    /// Request class for listing RFQ audit events.
    /// </summary>
    public class GetRfqAuditTrailRequest
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
        /// Rfq status
        /// </summary>
        public RfqOperationState? Status { get; set; }
    }
}