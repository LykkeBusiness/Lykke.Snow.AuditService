using System.Collections;
using System.Collections.Generic;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.Services;
using MarginTrading.Backend.Contracts.Events;
using MarginTrading.Backend.Contracts.Rfq;

using Moq;
using Xunit;

namespace Lykke.Snow.AuditService.Tests
{
    class RfqEventUsernameTestData : IEnumerable<object[]>
    {
        public IEnumerator<object[]> GetEnumerator()
        {
            yield return new object[] { new RfqEvent { RfqSnapshot = new RfqContract{}}, "SYSTEM" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class RfqAuditTrailServiceTests
    {
        //[Theory]
        //[ClassData(typeof(RfqEventUsernameTestData))]
        //public void GetEventUserName_ShouldReturnCorrectUsername_ForTheRfqEvent(RfqEvent rfqEvent, string expectedUsername)
        //{
        //}

        private RfqAuditTrailService CreateSut(IAuditEventRepository? auditEventRepositoryArg = null,
            IAuditObjectStateRepository? auditObjectStateRepositoryArg = null,
            IObjectDiffService? objectDiffServiceArg = null)
        {
            var auditEventRepository = new Mock<IAuditEventRepository>().Object;
            var auditObjectStateRepository = new Mock<IAuditObjectStateRepository>().Object;
            var objectDiffService = new Mock<IObjectDiffService>().Object;

            if(auditEventRepositoryArg != null)
            {
                auditEventRepository = auditEventRepositoryArg;
            }
            if(auditObjectStateRepositoryArg != null)
            {
                auditObjectStateRepository = auditObjectStateRepositoryArg;
            }
            if(objectDiffServiceArg != null)
            {
                objectDiffService = objectDiffServiceArg;
            }
            
            return new RfqAuditTrailService(auditEventRepository, auditObjectStateRepository, objectDiffService);
        }
    }
}
