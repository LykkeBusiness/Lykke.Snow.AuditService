using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.Settings;
using Lykke.Snow.AuditService.Subscribers;
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
        [Theory]
        [ClassData(typeof(RfqEventTestData))]
        public async Task ProcessMessageAsync_ShouldPassTheEvent_ToRfqAuditTrailService(RfqEvent evt)
        {
            var mockRfqAuditTrailService = new Mock<IRfqAuditTrailService>();
            
            var sut = CreateSut(mockRfqAuditTrailService.Object);
            
            await sut.ProcessMessageAsync(evt);
            
            mockRfqAuditTrailService.Verify(x => x.ProcessRfqEvent(evt), Times.Once);
        }
        
        private RfqEventSubscriber CreateSut(IRfqAuditTrailService? rfqAuditTrailServiceArg = null)
        {
            var mockLoggerFactory = new Mock<ILoggerFactory>();
            var subscriptionSettings = new SubscriptionSettings();
            
            IRfqAuditTrailService rfqAuditTrailService = new Mock<IRfqAuditTrailService>().Object;
            
            if(rfqAuditTrailServiceArg != null)
            {
                rfqAuditTrailService = rfqAuditTrailServiceArg;
            }
            
            return new RfqEventSubscriber(mockLoggerFactory.Object, subscriptionSettings, rfqAuditTrailService);
        }
    }
}