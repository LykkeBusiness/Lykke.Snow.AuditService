using System;

using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.SqlRepositories.Entities
{
    public class AuditObjectStateEntity
    {
        public int Oid { get; set; }
        public AuditDataType DataType { get; set; }
        public string DataReference { get; set; }
        public string StateInJson { get; set; }
        public DateTime LastModified { get; set; }
    }
}