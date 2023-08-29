// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Threading.Tasks;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using Microsoft.Extensions.Logging;

namespace Lykke.Snow.AuditService.DomainServices.Services
{
    public class AuditEventProcessor : IAuditEventProcessor
    {
        private readonly IAuditEventRepository _auditEventRepository;
        private readonly IAuditObjectStateRepository _auditObjectStateRepository;
        private readonly IObjectDiffService _objectDiffService;
        private readonly IAuditObjectStateFactory _auditObjectStateFactory;
        private readonly ILogger<AuditEventProcessor> _logger;

        public AuditEventProcessor(IAuditEventRepository auditEventRepository,
            IAuditObjectStateRepository auditObjectStateRepository,
            IObjectDiffService objectDiffService,
            IAuditObjectStateFactory auditObjectStateFactory,
            ILogger<AuditEventProcessor> logger)
        {
            _auditEventRepository = auditEventRepository;
            _auditObjectStateRepository = auditObjectStateRepository;
            _objectDiffService = objectDiffService;
            _auditObjectStateFactory = auditObjectStateFactory;
            _logger = logger;

        }

        public async Task ProcessEvent<T>(T evt, IAuditEventMapper<T> eventMapper)
        {
            var auditDataType = eventMapper.GetAuditDataType(evt);
            var dataReference = eventMapper.GetDataReference(evt);
            
            var oldState = await _auditObjectStateRepository.GetByDataReferenceAsync(auditDataType, dataReference);
            var newState = eventMapper.GetStateInJson(evt);

            var diff = GetJsonDiff(oldState == null ? "{}" : oldState.StateInJson, newState);

            var auditModel =  eventMapper.MapAuditEvent(evt, diff);

            var newAuditObjectState = _auditObjectStateFactory.Create(
                        auditDataType: auditModel.DataType, 
                        currentStateInJson: newState, 
                        dataReference: auditModel.DataReference, 
                        lastModified: auditModel.Timestamp);

            await _auditEventRepository.AddAsync(auditModel);
            
            if(oldState != null && newAuditObjectState.LastModified < oldState.LastModified)
            {
                _logger.LogWarning("Timestamp for entity state is older than the existing state - the event has been ignored. \n Existing object: {@ExistingObject} \n New object: {@NewObject}",
                    oldState, newAuditObjectState);
                
                return;
            }

            await _auditObjectStateRepository.AddOrUpdate(newAuditObjectState);
        }
        
        public string GetJsonDiff(string oldState, string newState)
        {
            if(string.IsNullOrEmpty(oldState))
                return newState;
            
            return _objectDiffService.GetJsonDiff(oldState, newState);
        }
    }
}