using System.Threading.Tasks;
using Common;
using JsonDiffPatchDotNet;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using MarginTrading.Backend.Contracts.Events;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class RfqAuditTrailService : IRfqAuditTrailService
    {
        private readonly IAuditEventRepository _auditEventRepository;

        public RfqAuditTrailService(IAuditEventRepository auditEventRepository)
        {
            _auditEventRepository = auditEventRepository;
        }

        public async Task ProcessRfqEvent(RfqEvent rfqEvent)
        {
            if(rfqEvent.EventType == RfqEventTypeContract.New)
            {
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
                // TODO: load the 'last' state of the event
                // 
                var oldStateJson = "{}";
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

                await _auditEventRepository.AddAsync(auditEvent);
            }
        }
    }
}