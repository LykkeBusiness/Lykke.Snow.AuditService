// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace Lykke.Snow.AuditService.Client.Model.Proposal
{
    
    internal static class ClientRequestBuilderExample
    {
        public static GetAuditEventsRequest GetEvents()
        {
            return new GetAuditEventsRequest
            {
                // common filters
                StartDateTime = new DateTime(2021, 1, 1),
                EndDateTime = new DateTime(2021, 1, 2),
                UserName = "user1",
                CorrelationId = "correlationId1",
                ActionType = Audit.AuditEventType.Creation,
                ReferenceId = "referenceId1",

                // domain filters
                //DomainFilters = new DomainFiltersContract()
                //{
                //    Filters = new Dictionary<AuditDataTypeContract, List<JsonDiffFilterContract>>
                //    {
                //        {
                //            // take ALL rfq domain audit events
                //            AuditDataTypeContract.Rfq, new List<JsonDiffFilterContract>()
                //        },
                //        {
                //            // take Assets domain audit events ...
                //            AuditDataTypeContract.CorporateActions,
                //            new List<JsonDiffFilterContract>
                //            {
                //                new JsonDiffFilterContract() { PropertyName = "State" },
                //                new JsonDiffFilterContract() { PropertyName = "Price", Value = 10 }
                //            }
                //        }
                //    }
                //}
                
                
                
            };
        }
    }
}