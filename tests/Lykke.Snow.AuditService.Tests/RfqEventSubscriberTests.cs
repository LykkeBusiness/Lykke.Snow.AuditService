using System.Threading.Tasks;
using MarginTrading.Backend.Contracts.Events;
using Xunit;

namespace Lykke.Snow.AuditService.Tests
{
    public class RfqEventSubscriberTests
    {
        [Theory]
        public Task ProcessMessageAsync_ShouldPassTheEvent_ToRfqAuditTrailService(RfqEvent evt)
        {
            return Task.CompletedTask;
        }
    }
}