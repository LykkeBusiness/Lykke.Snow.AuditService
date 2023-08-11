using System.Threading.Tasks;
using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Exceptions;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using MarginTrading.Backend.Contracts.Events;
using MarginTrading.Backend.Contracts.Rfq;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class RfqAuditTrailService : IRfqAuditTrailService
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IAuditObjectStateRepository _auditObjectStateRepository;
        private readonly IObjectDiffService _objectDiffService;
        private readonly IAuditObjectStateFactory _auditObjectStateFactory;

        public RfqAuditTrailService(IAuditEventRepository auditEventRepository,
            IAuditObjectStateRepository auditObjectStateRepository,
            IObjectDiffService objectDiffService,
            IAuditObjectStateFactory auditObjectStateFactory)
        {
            _auditEventRepository = auditEventRepository;
            _auditObjectStateRepository = auditObjectStateRepository;
            _objectDiffService = objectDiffService;
            _auditObjectStateFactory = auditObjectStateFactory;
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

        public AuditModel<AuditDataType> GetAuditEvent(RfqEvent rfqEvent, string username, string jsonDiff)
        {
            var auditEvent = new AuditModel<AuditDataType>()
            {
                Timestamp = rfqEvent.RfqSnapshot.LastModified,
                CorrelationId = rfqEvent.RfqSnapshot.CausationOperationId,
                UserName = username,
                Type = rfqEvent.EventType == RfqEventTypeContract.New ? AuditEventType.Creation : AuditEventType.Edition,
                AuditEventTypeDetails = rfqEvent.RfqSnapshot.State.ToString(),
                DataType = AuditDataType.Rfq,
                DataReference = rfqEvent.RfqSnapshot.Id,
                DataDiff = jsonDiff
            };

            return auditEvent;
        }

        public async Task ProcessRfqEvent(RfqEvent rfqEvent)
        {
            var username = GetEventUsername(rfqEvent);

            if(rfqEvent.EventType == RfqEventTypeContract.New)
            {
                var auditObjectState = _auditObjectStateFactory.Create(AuditDataType.Rfq, 
                    rfqEvent.RfqSnapshot.Id, 
                    rfqEvent.RfqSnapshot.ToJson(), 
                    rfqEvent.RfqSnapshot.LastModified);

                await _auditObjectStateRepository.AddOrUpdate(auditObjectState);

                var diff = GetRfqJsonDiff(rfqEvent, existingObject: null);

                var auditEvent = GetAuditEvent(rfqEvent, username, diff);

                await _auditEventRepository.AddAsync(auditEvent);
            }
            else if(rfqEvent.EventType == RfqEventTypeContract.Update)
            {
                var existingObject = await _auditObjectStateRepository.GetByDataReferenceAsync(dataType: AuditDataType.Rfq, dataReference: rfqEvent.RfqSnapshot.Id);
                
                if(existingObject == null)
                {
                    throw new AuditObjectNotFoundException(dataType: AuditDataType.Rfq.ToString(), dataReference: rfqEvent.RfqSnapshot.Id);
                }
                
                var diff = GetRfqJsonDiff(rfqEvent, existingObject);

                var auditEvent = GetAuditEvent(rfqEvent, username, diff);

                var auditObjectState = _auditObjectStateFactory.Create(AuditDataType.Rfq,
                    rfqEvent.RfqSnapshot.Id,
                    rfqEvent.RfqSnapshot.ToJson(),
                    rfqEvent.RfqSnapshot.LastModified);

                await _auditObjectStateRepository.AddOrUpdate(auditObjectState);

                await _auditEventRepository.AddAsync(auditEvent);
            }
        }

        public string GetRfqJsonDiff(RfqEvent rfqEvent, AuditObjectState? existingObject)
        {
            if(existingObject == null)
                return _objectDiffService.GenerateNewJsonDiff(rfqEvent.RfqSnapshot);

            var diff = _objectDiffService.GetJsonDiff(existingObject.StateInJson, rfqEvent.RfqSnapshot.ToJson());
            
            return diff;
        }
    }
}