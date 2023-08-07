using System;

namespace Lykke.Snow.AuditService.SqlRepositories.Entities
{
    public class AuditEventEntity
    {
        public string Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
        public string CorporateActionsId { get; set; }
        public string Username { get; set; }
        public string ActionType { get; set; }
        public string ActionTypeDetails { get; set; }
        public string DataType { get; set; }
        public string DataReference { get; set; }
        public string DataDiff { get; set; }
    }
}