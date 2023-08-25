// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;
using MarginTrading.Backend.Contracts.Events;
using MarginTrading.Backend.Contracts.Rfq;
using Newtonsoft.Json.Linq;

namespace Lykke.Snow.AuditService.DomainServices.AuditEventMappers
{
    public class RfqAuditEventMapper : IAuditEventMapper<RfqEvent>
    {
        private readonly IObjectDiffService _objectDiffService;

        public RfqAuditEventMapper(IObjectDiffService objectDiffService)
        {
            _objectDiffService = objectDiffService;
        }

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

        public AuditEventType GetAuditEventType(RfqEvent evt, string diffWithPreviousState)
        {
            if(evt.EventType == RfqEventTypeContract.New)
                return AuditEventType.Creation;
            
            var diffObject = JObject.Parse(diffWithPreviousState);
            
            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                new JsonDiffFilter("State")
            };

            // State has changed
            if(_objectDiffService.CheckJsonProperties(diffObject.Properties(), jsonDiffFilters))
            {
                return AuditEventType.StatusChanged;
            }
            
            return AuditEventType.Edition;
        }

        public AuditModel<AuditDataType> MapAuditEvent(RfqEvent rfqEvent, string diffWithPreviousState)
        {
            var username = GetEventUsername(rfqEvent);

            var auditEvent = new AuditModel<AuditDataType>()
            {
                Timestamp = rfqEvent.RfqSnapshot.LastModified,
                CorrelationId = rfqEvent.RfqSnapshot.CausationOperationId,
                UserName = username,
                Type = GetAuditEventType(rfqEvent, diffWithPreviousState), 
                AuditEventTypeDetails = rfqEvent.RfqSnapshot.State.ToString(),
                DataType = GetAuditDataType(rfqEvent),
                DataReference = GetDataReference(rfqEvent),
                DataDiff = diffWithPreviousState
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