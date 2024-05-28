// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Threading.Tasks;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.AuditEventMappers;
using Lykke.Snow.AuditService.MessageHandlers;
using MarginTrading.Backend.Contracts.Events;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.AuditService.Tests
{
    public class RfqEventSubscriberTests
    {
        private readonly IAuditEventMapper<RfqEvent> _rfqAuditEventMapper = new RfqAuditEventMapper(new Mock<IObjectDiffService>().Object);

        [Theory]
        [ClassData(typeof(RfqEventTestData))]
        public async Task ProcessMessageAsync_ShouldPassTheEvent_ToRfqAuditTrailService(RfqEvent evt)
        {
            var mockAuditEventProcessor = new Mock<IAuditEventProcessor>();
            
            var sut = CreateSut(mockAuditEventProcessor.Object);
            
            await sut.Handle(evt);
            
            mockAuditEventProcessor.Verify(x => x.ProcessEvent(evt, _rfqAuditEventMapper), Times.Once);
        }

        private RfqEventHandler CreateSut(IAuditEventProcessor? auditEventProcessorArg = null)
        {
            var mockLogger = new Mock<ILogger<RfqEventHandler>>();
            var auditEventProcessor = auditEventProcessorArg ?? new Mock<IAuditEventProcessor>().Object;

            return new RfqEventHandler("brokerId", mockLogger.Object, auditEventProcessor, _rfqAuditEventMapper);
        }
    }
}