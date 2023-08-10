using System.Threading.Tasks;
using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Exceptions;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class RfqAuditTrailService : IRfqAuditTrailService
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IAuditObjectStateRepository _auditObjectStateRepository;
        private readonly IObjectDiffService _objectDiffService;

        public RfqAuditTrailService(IAuditEventRepository auditEventRepository,
            IAuditObjectStateRepository auditObjectStateRepository, 
            IObjectDiffService objectDiffService)
        {
            _auditEventRepository = auditEventRepository;
            _auditObjectStateRepository = auditObjectStateRepository;
            _objectDiffService = objectDiffService;
        }

        public AuditModel<AuditDataType> GetAuditEvent(RfqEvent rfqEvent, string jsonDiff)
        {
        //    string username = "SYSTEM";
        //    
        //    if(rfqEvent.RfqSnapshot.OriginatorType == MarginTrading.Backend.Contracts.Rfq.RfqOriginatorType.Investor)
        //        username = rfqEvent.RfqSnapshot.CreatedBy;
        //        
        //    if(rfqEvent.RfqSnapshot.State == MarginTrading.Backend.Contracts.Rfq.RfqOperationState.)
        //
            var auditEvent = new AuditModel<AuditDataType>()
            {
                Timestamp = rfqEvent.RfqSnapshot.LastModified,
                CorrelationId = rfqEvent.RfqSnapshot.CausationOperationId,
                UserName = rfqEvent.RfqSnapshot.CreatedBy,
                Type = rfqEvent.EventType == RfqEventTypeContract.New ? AuditEventType.Creation : AuditEventType.Edition,
                ActionTypeDetails = rfqEvent.RfqSnapshot.State.ToString(),
                DataType = AuditDataType.Rfq,
                DataReference = rfqEvent.RfqSnapshot.Id,
                DataDiff = jsonDiff
            };

            return auditEvent;
        }

        public async Task ProcessRfqEvent(RfqEvent rfqEvent)
        {
            if(rfqEvent.EventType == RfqEventTypeContract.New)
            {
                var auditObjectState = new AuditObjectState(dataType: AuditDataType.Rfq, dataReference: rfqEvent.RfqSnapshot.Id,
                    stateInJson: rfqEvent.RfqSnapshot.ToJson(), lastModified: rfqEvent.RfqSnapshot.LastModified);

                await _auditObjectStateRepository.AddOrUpdate(auditObjectState);

                var diff = _objectDiffService.GenerateNewJsonDiff(rfqEvent.RfqSnapshot);

                var auditEvent = GetAuditEvent(rfqEvent, diff);

                await _auditEventRepository.AddAsync(auditEvent);
            }
            else if(rfqEvent.EventType == RfqEventTypeContract.Update)
            {
                var existingObject = await _auditObjectStateRepository.GetByDataReferenceAsync(dataType: AuditDataType.Rfq, dataReference: rfqEvent.RfqSnapshot.Id);
                
                if(existingObject == null)
                {
                    throw new AuditObjectNotFoundException(dataType: AuditDataType.Rfq.ToString(), dataReference: rfqEvent.RfqSnapshot.Id);
                }

                var diff = _objectDiffService.GetJsonDiff(existingObject.StateInJson, rfqEvent.RfqSnapshot.ToJson());

                var auditEvent = GetAuditEvent(rfqEvent, diff);

                var auditObjectState = new AuditObjectState(dataType: AuditDataType.Rfq, dataReference: rfqEvent.RfqSnapshot.Id,
                    stateInJson: rfqEvent.RfqSnapshot.ToJson(), lastModified: rfqEvent.RfqSnapshot.LastModified);

                await _auditObjectStateRepository.AddOrUpdate(auditObjectState);

                await _auditEventRepository.AddAsync(auditEvent);
            }
        }
    }
}