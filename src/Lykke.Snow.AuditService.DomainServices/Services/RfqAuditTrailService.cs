using System.Threading.Tasks;
using Common;
using JsonDiffPatchDotNet;
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

        public RfqAuditTrailService(IAuditEventRepository auditEventRepository, 
            IAuditObjectStateRepository auditObjectStateRepository)
        {
            _auditEventRepository = auditEventRepository;
            _auditObjectStateRepository = auditObjectStateRepository;
        }

        public async Task ProcessRfqEvent(RfqEvent rfqEvent)
        {
            if(rfqEvent.EventType == RfqEventTypeContract.New)
            {
                var auditObjectState = new AuditObjectState(dataType: AuditDataType.Rfq, dataReference: rfqEvent.RfqSnapshot.Id,
                    stateInJson: rfqEvent.RfqSnapshot.ToJson(), lastModified: rfqEvent.RfqSnapshot.LastModified);

                await _auditObjectStateRepository.AddOrUpdate(auditObjectState);

                var oldStateJson = "{}";
                var newStateJson = rfqEvent.RfqSnapshot.ToJson();

                var jdp = new JsonDiffPatch();
                var jsonDiff = jdp.Diff(oldStateJson, newStateJson);

                var auditEvent = new AuditModel<AuditDataType>()
                {
                    Timestamp = rfqEvent.RfqSnapshot.LastModified,
                    CorrelationId = rfqEvent.RfqSnapshot.CausationOperationId,
                    UserName = rfqEvent.RfqSnapshot.CreatedBy,
                    Type = AuditEventType.Creation,
                    ActionTypeDetails = rfqEvent.RfqSnapshot.State.ToString(),
                    DataType = AuditDataType.Rfq,
                    DataReference = rfqEvent.RfqSnapshot.Id,
                    DataDiff = jsonDiff
                };

                // TODO: is it idempotent? what happens if event is consumed more than once?
                await _auditEventRepository.AddAsync(auditEvent);
            }
            else if(rfqEvent.EventType == RfqEventTypeContract.Update)
            {
                var existingObject = await _auditObjectStateRepository.GetByDataReferenceAsync(dataType: AuditDataType.Rfq, dataReference: rfqEvent.RfqSnapshot.Id);
                
                if(existingObject == null)
                {
                    throw new AuditObjectNotFoundException(dataType: AuditDataType.Rfq.ToString(), dataReference: rfqEvent.RfqSnapshot.Id);
                }

                var oldStateJson = existingObject.StateInJson;
                var newStateJson = rfqEvent.RfqSnapshot.ToJson();

                var jdp = new JsonDiffPatch();
                var jsonDiff = jdp.Diff(oldStateJson, newStateJson);

                var auditEvent = new AuditModel<AuditDataType>()
                {
                    Timestamp = rfqEvent.RfqSnapshot.LastModified,
                    CorrelationId = rfqEvent.RfqSnapshot.CausationOperationId,
                    UserName = rfqEvent.RfqSnapshot.CreatedBy,
                    Type = AuditEventType.Edition,
                    ActionTypeDetails = rfqEvent.RfqSnapshot.State.ToString(),
                    DataType = AuditDataType.Rfq,
                    DataReference = rfqEvent.RfqSnapshot.Id,
                    DataDiff = jsonDiff
                };

                var auditObjectState = new AuditObjectState(dataType: AuditDataType.Rfq, dataReference: rfqEvent.RfqSnapshot.Id,
                    stateInJson: rfqEvent.RfqSnapshot.ToJson(), lastModified: rfqEvent.RfqSnapshot.LastModified);

                await _auditObjectStateRepository.AddOrUpdate(auditObjectState);

                await _auditEventRepository.AddAsync(auditEvent);
            }
        }
    }
}