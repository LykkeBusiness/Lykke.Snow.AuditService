// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Services;
using MarginTrading.Backend.Contracts.Events;
using MarginTrading.Backend.Contracts.Rfq;

namespace Lykke.Snow.AuditService.DomainServices.AuditEventMappers
{
    public class RfqAuditEventMapper : IAuditEventMapper<RfqEvent>
    {
        public AuditDataType GetAuditDataType(RfqEvent evt)
        {
            return AuditDataType.Rfq;
        }

        public string GetDataReference(RfqEvent evt)
        {
            return evt.RfqSnapshot.Id;
        }

        public string GetStateInJson(RfqEvent evt)
        {
            return evt.RfqSnapshot.ToJson();
        }

        public AuditModel<AuditDataType> MapAuditEvent(RfqEvent rfqEvent, string diff)
        {
            var username = GetEventUsername(rfqEvent);

            var auditEvent = new AuditModel<AuditDataType>()
            {
                Timestamp = rfqEvent.RfqSnapshot.LastModified,
                CorrelationId = rfqEvent.RfqSnapshot.CausationOperationId,
                UserName = username,
                Type = rfqEvent.EventType == RfqEventTypeContract.New ? AuditEventType.Creation : AuditEventType.Edition,
                AuditEventTypeDetails = rfqEvent.RfqSnapshot.State.ToString(),
                DataType = AuditDataType.Rfq,
                DataReference = rfqEvent.RfqSnapshot.Id,
                DataDiff = diff
            };
            
            return auditEvent;
        }

        public string GetEventUsername(RfqEvent rfqEvent)
        {
            string username = string.Empty;
            
            if(rfqEvent.RfqSnapshot.OriginatorType == RfqOriginatorType.Investor || rfqEvent.RfqSnapshot.OriginatorType == RfqOriginatorType.OnBehalf)
                username = rfqEvent.RfqSnapshot.CreatedBy;
            else
                username = "SYSTEM";
                
            return username;
        }
    }
}