// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.AuditEventMappers;
using Lykke.Snow.AuditService.Settings;
using Lykke.Snow.AuditService.Subscribers;
using Lykke.Snow.Common.Correlation.RabbitMq;

using MarginTrading.Backend.Contracts.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.AuditService.Tests
{
    class RfqEventTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] 
            {
                new RfqEvent
                {
                    EventType = RfqEventTypeContract.New,
                    BrokerId = "Spain",
                    RfqSnapshot = new MarginTrading.Backend.Contracts.Rfq.RfqContract()
                }
            };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }
    public class RfqEventSubscriberTests
    {
        private readonly IAuditEventMapper<RfqEvent> _rfqAuditEventMapper = new RfqAuditEventMapper(new Mock<IObjectDiffService>().Object);

        [Theory]
        [ClassData(typeof(RfqEventTestData))]
        public async Task ProcessMessageAsync_ShouldPassTheEvent_ToRfqAuditTrailService(RfqEvent evt)
        {
            var mockAuditEventProcessor = new Mock<IAuditEventProcessor>();
            
            var sut = CreateSut(mockAuditEventProcessor.Object);
            
            await sut.ProcessMessageAsync(evt);
            
            mockAuditEventProcessor.Verify(x => x.ProcessEvent(evt, _rfqAuditEventMapper), Times.Once);
        }
        
        private RfqEventSubscriber CreateSut(IAuditEventProcessor? auditEventProcessorArg = null)
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var mockLogger = new Mock<ILogger<RfqEventSubscriber>>();
            var subscriptionSettings = new SubscriptionSettings();
            
            IAuditEventProcessor auditEventProcessor = new Mock<IAuditEventProcessor>().Object;
            
            if(auditEventProcessorArg != null)
            {
                auditEventProcessor = auditEventProcessorArg;
            }
            
            return new RfqEventSubscriber(auditEventProcessor, mockLoggerFactory.Object, subscriptionSettings, mockLogger.Object, _rfqAuditEventMapper, new RabbitMqCorrelationManager(new Common.Correlation.CorrelationContextAccessor()));
        }
    }
}