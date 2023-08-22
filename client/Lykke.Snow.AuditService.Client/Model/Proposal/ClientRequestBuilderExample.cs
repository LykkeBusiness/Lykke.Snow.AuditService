// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;

using Newtonsoft.Json.Linq;

namespace Lykke.Snow.AuditService.Client.Model.Proposal
{
    internal static class FilterExamples 
    {
        public static AuditDomainDataTypeFilter QuoteIdChanged = new AuditDomainDataTypeFilter
        {
            Filter = jsonDiff => jsonDiff.Contains("QuoteId")
        };
        
        public static AuditDomainDataTypeFilter RfqFilterInitiated = new AuditDomainDataTypeFilter
        {
            Filter = jsonDiff => jsonDiff.Contains("State: Initiated")
        };

        public static AuditDomainDataTypeFilter ChangeNameToJohnFilter = new AuditDomainDataTypeFilter
        {
            Filter = jsonDiff =>
            {
                var jobject = JObject.Parse(jsonDiff);
                var firstNameToken = jobject["FirstName"];

                if (firstNameToken == null)
                    return false;

                return (firstNameToken.Children().Count() == 2) &&
                       (firstNameToken.Children().Last().ToString() == "John");
            }
        };
    }
    
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
                AuditEventType = AuditEventType.Create,
                ReferenceId = "referenceId1",

                // domain filters
                DomainFilters = new Dictionary<AuditDomain, IAuditDomainFilter?>
                {
                    {
                        // take Core domain audit events ...
                        AuditDomain.Core, new AuditDomainFilter
                        {
                            DataTypeFilters = new Dictionary<string, IAuditDomainDataTypeFilter?>
                            {
                                // ... but inside domain take only Rfq initiated events ...
                                { "Rfq", FilterExamples.RfqFilterInitiated },
                                // ... and only quote changed events ...
                                { "Quote", FilterExamples.QuoteIdChanged },
                                // ... and all order events
                                { "Order", null }
                            }
                        }
                    },
                    {
                        // take ALL axle domain audit events
                        AuditDomain.Axle, null
                    },
                    {
                        // take Assets domain audit events ...
                        AuditDomain.Assets,
                        new AuditDomainFilter
                        {
                            DataTypeFilters = new Dictionary<string, IAuditDomainDataTypeFilter?>
                            {
                                // ... but inside domain take only Person events where name changed to John
                                { "Person", FilterExamples.ChangeNameToJohnFilter }
                            }
                        }
                    }
                }
            };
        }
    }
}