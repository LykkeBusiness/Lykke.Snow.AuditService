using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
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

        public string GetStateInJson(RfqEvent evt)
        {
            return evt.RfqSnapshot.ToJson();
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