using System;

using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.SqlRepositories.Entities
{
    public class AuditEventEntity : IAuditModel<AuditDataType>
    {
        public int Id { get; set; }
        public DateTime Timestamp { get; set; }
        public string CorrelationId { get; set; }
        public string UserName { get; set; }
        public AuditEventType Type { get; set; }
        public string ActionTypeDetails { get; set; }
        public AuditDataType DataType { get; set; }
        public string DataReference { get; set; }
        public string DataDiff { get; set; }
    }
}