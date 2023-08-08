using System;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Domain.Model
{
    public class AuditObjectState
    {
        public string DataReference { get; set; }
        public AuditDataType DataType { get; set; }
        public string StateInJson { get; set; }
        public DateTime LastModified { get; set; }
        
        public AuditObjectState(AuditDataType dataType, string dataReference, string stateInJson, DateTime lastModified)
        {
            DataReference = dataReference;
            DataType = dataType;
            StateInJson = stateInJson;
            LastModified = lastModified;
        }
    }
}