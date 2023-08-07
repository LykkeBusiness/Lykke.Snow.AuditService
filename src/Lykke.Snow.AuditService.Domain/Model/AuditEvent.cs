using System;

namespace Lykke.Snow.AuditService.Domain.Model
{
    // TODO: consider making DataType a T
    public class AuditEvent
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
        public string? CorporateActionsId { get; set; }
        public string Username { get; set; }
        public string ActionType { get; set; }
        public string ActionTypeDetails { get; set; }
        public string DataType { get; set; }
        public string DataReference { get; set; }
        public string DataDiff { get; set; }
        
        public AuditEvent(string id, DateTime timestamp, string correlationId, string username, string actionType,
            string actionTypeDetails, string dataType, string dataReference, string dataDiff, string? corporateActionsId = null)
        {
            Id = id;
            Timestamp = timestamp;
            CorrelationId = correlationId;
            CorporateActionsId = corporateActionsId;
            Username = username;
            ActionType = actionType;
            ActionTypeDetails = actionTypeDetails;
            DataType = dataType;
            DataReference = dataReference;
            DataDiff = dataDiff;
        }
    }
}