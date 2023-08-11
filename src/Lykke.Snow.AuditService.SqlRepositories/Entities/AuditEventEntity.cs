// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

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
        public string AuditEventTypeDetails { get; set; }
        public AuditDataType DataType { get; set; }
        public string DataReference { get; set; }
        public string DataDiff { get; set; }

    }
}