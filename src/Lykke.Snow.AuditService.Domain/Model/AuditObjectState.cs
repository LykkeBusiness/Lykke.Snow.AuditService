// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.AuditService.Domain.Enum;

namespace Lykke.Snow.AuditService.Domain.Model
{
    /// <summary>
    /// An entity represents current state of the entities which audit events are stored for.
    /// </summary>
    public class AuditObjectState
    {
        public string DataReference { get; }
        public AuditDataType DataType { get; }
        public string StateInJson { get; }
        public DateTime LastModified { get; }
        
        public AuditObjectState(AuditDataType dataType, string dataReference, string stateInJson, DateTime lastModified)
        {
            DataReference = dataReference;
            DataType = dataType;
            StateInJson = stateInJson;
            LastModified = lastModified;
        }
    }
}