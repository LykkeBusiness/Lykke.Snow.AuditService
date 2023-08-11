using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Exceptions;
using Lykke.Snow.AuditService.Domain.Model;
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
            yield return new object[] { new RfqEvent { RfqSnapshot = new RfqContract{ OriginatorType = RfqOriginatorType.Investor, CreatedBy = "investor-1" }}, "investor-1" };
            yield return new object[] { new RfqEvent { RfqSnapshot = new RfqContract{ OriginatorType = RfqOriginatorType.OnBehalf, CreatedBy = "support-user" }}, "support-user" };
            yield return new object[] { new RfqEvent { RfqSnapshot = new RfqContract{ OriginatorType = RfqOriginatorType.System, CreatedBy = "investor-2" }}, "SYSTEM" };
        }

        IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
    }

    public class RfqAuditTrailServiceTests
    {
        [Theory]
        [ClassData(typeof(RfqEventUsernameTestData))]
        public void GetEventUserName_ShouldReturnCorrectUsername_ForTheRfqEvent(RfqEvent rfqEvent, string expectedUsername)
        {
            var sut = CreateSut();
            
            var actual = sut.GetEventUsername(rfqEvent);
            
            Assert.Equal(expectedUsername, actual);
        }
        
        [Fact]
        public void GetAuditEvent_ShouldMapFields_AndGenerateAuditModel()
        {
            var now = DateTime.UtcNow;
            var operationId = "op-id-1";
            var id = "id-1";
            var username = "username-1";
            var jsonDiff = "json-diff-1";

            var rfqEvent = new RfqEvent
            {
                BrokerId = "Spain",
                EventType = RfqEventTypeContract.New,
                RfqSnapshot = new RfqContract 
                {
                    LastModified = now,
                    CausationOperationId = operationId,
                    State = RfqOperationState.Initiated,
                    Id = id
                }
            };
            
            var sut = CreateSut();
            
            var actual = sut.GetAuditEvent(rfqEvent, username, jsonDiff);
            
            Assert.Equal(now, actual.Timestamp);
            Assert.Equal(operationId, actual.CorrelationId);
            Assert.Equal(username, actual.UserName);
            Assert.Equal(AuditEventType.Creation, actual.Type);
            Assert.Equal(RfqOperationState.Initiated.ToString(), actual.AuditEventTypeDetails);
            Assert.Equal(AuditDataType.Rfq, actual.DataType);
            Assert.Equal(id, actual.DataReference);
            Assert.Equal(jsonDiff, actual.DataDiff);
        }
        
        /// <summary>
        /// Test method that verifies expected method calls from ProcessRfqEvent() 
        /// when EventType = 'Creation'
        /// </summary>
        [Fact]
        public async void ProcessRfqEvent_Creation_VerifyExpectedMethodCalls()
        {
            var now = DateTime.UtcNow;
            var operationId = "op-id-1";
            var id = "id-1";

            var rfqEvent = new RfqEvent
            {
                BrokerId = "Spain",
                EventType = RfqEventTypeContract.New,
                RfqSnapshot = new RfqContract 
                {
                    LastModified = now,
                    CausationOperationId = operationId,
                    State = RfqOperationState.Initiated,
                    Id = id
                }
            };
            
            
            var mockAuditEventRepository = new Mock<IAuditEventRepository>();
            var mockAuditObjectStateFactory = new Mock<IAuditObjectStateFactory>();
            var mockAuditObjectStateRepository = new Mock<IAuditObjectStateRepository>();
            var mockObjectDiffService = new Mock<IObjectDiffService>();

            var jsonState = rfqEvent.RfqSnapshot.ToJson();
            
            var sut = CreateSut(mockAuditEventRepository.Object, 
                mockAuditObjectStateRepository.Object,
                mockObjectDiffService.Object, 
                mockAuditObjectStateFactory.Object);
            
            await sut.ProcessRfqEvent(rfqEvent);
            
            // Verify that AuditObjectStateFactory has been called with expected arguments
            mockAuditObjectStateFactory.Verify(x => x.Create(It.Is<AuditDataType>(type => type == AuditDataType.Rfq),
                It.Is<string>(dataReference => dataReference == id), 
                It.Is<string>(state => state == jsonState), 
                It.Is<DateTime>(lastModified => lastModified == rfqEvent.RfqSnapshot.LastModified)), Times.Once);
            
            // Verify that new mockObjectState is saved - AddOrUpdate() is called.
            mockAuditObjectStateRepository.Verify(x => x.AddOrUpdate(It.IsAny<AuditObjectState>()), Times.Once);
            
            // Verify that ObjectDiffService.GenerateNewJsonDiff() is called instead of GetJsonDiff()
            mockObjectDiffService.Verify(x => x.GenerateNewJsonDiff(It.IsAny<RfqContract>()), Times.Once);
            mockObjectDiffService.Verify(x => x.GetJsonDiff(It.IsAny<object>(), It.IsAny<object>()), Times.Never);
            
            // Verify that auditEventRepository.AddAsync() is called
            mockAuditEventRepository.Verify(x => x.AddAsync(It.IsAny<AuditModel<AuditDataType>>()), Times.Once);
        }

        /// <summary>
        /// Test method that verifies expected method calls from ProcessRfqEvent() 
        /// when EventType = 'Edition'
        /// </summary>
        [Fact]
        public async void ProcessRfqEvent_Edition_VerifyExpectedMethodCalls()
        {
            var now = DateTime.UtcNow;
            var operationId = "op-id-1";
            var id = "id-1";

            var rfqEvent = new RfqEvent
            {
                BrokerId = "Spain",
                EventType = RfqEventTypeContract.Update,
                RfqSnapshot = new RfqContract 
                {
                    LastModified = now,
                    CausationOperationId = operationId,
                    State = RfqOperationState.Initiated,
                    Id = id
                }
            };
            
            var mockAuditEventRepository = new Mock<IAuditEventRepository>();
            var mockAuditObjectStateFactory = new Mock<IAuditObjectStateFactory>();
            var mockAuditObjectStateRepository = new Mock<IAuditObjectStateRepository>();
            var mockObjectDiffService = new Mock<IObjectDiffService>();

            var existingObjectState = "existing-object-state-json";

            var auditObjectState = new AuditObjectState(AuditDataType.Rfq, 
                rfqEvent.RfqSnapshot.Id,
                existingObjectState, 
                rfqEvent.RfqSnapshot.LastModified);

            var newJsonState = rfqEvent.RfqSnapshot.ToJson();
            
            mockAuditObjectStateRepository.Setup(x => x.GetByDataReferenceAsync(It.IsAny<AuditDataType>(),
                It.IsAny<string>())).ReturnsAsync(auditObjectState);
            
            var sut = CreateSut(mockAuditEventRepository.Object, 
                mockAuditObjectStateRepository.Object,
                mockObjectDiffService.Object, 
                mockAuditObjectStateFactory.Object);
            
            await sut.ProcessRfqEvent(rfqEvent);
            
            // Verify that existing object state is attempted to be fetched 
            mockAuditObjectStateRepository.Verify(x => x.GetByDataReferenceAsync(It.Is<AuditDataType>(dataType => dataType == AuditDataType.Rfq), 
                It.Is<string>(dataReference => dataReference == rfqEvent.RfqSnapshot.Id)), Times.Once);
            
            // Verify that ObjectDiffService.GetJsonDiff() is called - not GenerateNewJsonDiff()
            mockObjectDiffService.Verify(x => x.GetJsonDiff(It.Is<string>(old => old == existingObjectState), It.Is<string>(n => n == newJsonState)), Times.Once);
            mockObjectDiffService.Verify(x => x.GenerateNewJsonDiff(It.IsAny<RfqContract>()), Times.Never);
            
            
            // Verify that AuditObjectStateFactory has been called with expected arguments
            mockAuditObjectStateFactory.Verify(x => x.Create(It.Is<AuditDataType>(type => type == AuditDataType.Rfq),
                It.Is<string>(dataReference => dataReference == id), 
                It.Is<string>(state => state == newJsonState), 
                It.Is<DateTime>(lastModified => lastModified == rfqEvent.RfqSnapshot.LastModified)), Times.Once);

            // Verify that new mockObjectState is saved - AddOrUpdate() is called.
            mockAuditObjectStateRepository.Verify(x => x.AddOrUpdate(It.IsAny<AuditObjectState>()), Times.Once);

            // Verify that auditEventRepository.AddAsync() is called
            mockAuditEventRepository.Verify(x => x.AddAsync(It.IsAny<AuditModel<AuditDataType>>()), Times.Once);
        }
        
        [Fact]
        public async void ProcessRfqEvent_ShouldThrowAuditObjectNotFoundException_WhenExistingObjectCouldntBeFound()
        {
            var rfqEvent = new RfqEvent
            {
                BrokerId = "Spain",
                EventType = RfqEventTypeContract.Update,
                RfqSnapshot = new RfqContract 
                {
                    LastModified = DateTime.UtcNow,
                    CausationOperationId = Guid.NewGuid().ToString(),
                    State = RfqOperationState.Initiated,
                    Id = Guid.NewGuid().ToString()
                }
            };

            var mockAuditObjectStateRepository = new Mock<IAuditObjectStateRepository>();
            mockAuditObjectStateRepository.Setup(x => x.GetByDataReferenceAsync(It.IsAny<AuditDataType>(), It.IsAny<string>())).ReturnsAsync((AuditObjectState)null!);
            
            var sut = CreateSut(auditObjectStateRepositoryArg: mockAuditObjectStateRepository.Object);
            
            await Assert.ThrowsAsync<AuditObjectNotFoundException>(async () => await sut.ProcessRfqEvent(rfqEvent));
        }
        
        [Fact]
        public void GetRfqJsonDiff_Creation_VerifyExpectedMethodCalls()
        {
            var rfqEvent = new RfqEvent
            {
                EventType = RfqEventTypeContract.New,
                RfqSnapshot = new RfqContract()
            };
            
            var mockObjectDiffService = new Mock<IObjectDiffService>();
            
            var sut = CreateSut(objectDiffServiceArg: mockObjectDiffService.Object);
            
            sut.GetRfqJsonDiff(rfqEvent, existingObject: null);

            mockObjectDiffService.Verify(x => x.GenerateNewJsonDiff(It.IsAny<object>()), Times.Once);
            mockObjectDiffService.Verify(x => x.GetJsonDiff(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void GetRfqJsonDiff_Edition_VerifyExpectedMethodCalls()
        {
            var rfqEvent = new RfqEvent
            {
                EventType = RfqEventTypeContract.Update,
                RfqSnapshot = new RfqContract()
            };

            var auditObjectState = new AuditObjectState(AuditDataType.Rfq, 
                rfqEvent.RfqSnapshot.Id,
                "state-json", 
                rfqEvent.RfqSnapshot.LastModified);
            
            var mockObjectDiffService = new Mock<IObjectDiffService>();
            
            var sut = CreateSut(objectDiffServiceArg: mockObjectDiffService.Object);
            
            sut.GetRfqJsonDiff(rfqEvent, existingObject: auditObjectState);

            mockObjectDiffService.Verify(x => x.GetJsonDiff(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
            mockObjectDiffService.Verify(x => x.GenerateNewJsonDiff(It.IsAny<object>()), Times.Never);
        }

        private RfqAuditTrailService CreateSut(IAuditEventRepository? auditEventRepositoryArg = null,
            IAuditObjectStateRepository? auditObjectStateRepositoryArg = null,
            IObjectDiffService? objectDiffServiceArg = null,
            IAuditObjectStateFactory? auditObjectStateFactoryArg = null)
        {
            var auditEventRepository = new Mock<IAuditEventRepository>().Object;
            var auditObjectStateRepository = new Mock<IAuditObjectStateRepository>().Object;
            var objectDiffService = new Mock<IObjectDiffService>().Object;
            var auditObjectStateFactory = new Mock<IAuditObjectStateFactory>().Object;

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
            if(auditObjectStateFactoryArg != null)
            {
                auditObjectStateFactory = auditObjectStateFactoryArg;
            }
            
            return new RfqAuditTrailService(auditEventRepository, auditObjectStateRepository, objectDiffService, auditObjectStateFactory);
        }
    }
}
