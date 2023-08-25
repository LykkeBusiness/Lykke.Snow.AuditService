// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.AuditEventMappers;
using Lykke.Snow.AuditService.DomainServices.Services;
using MarginTrading.Backend.Contracts.Events;
using MarginTrading.Backend.Contracts.Rfq;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Lykke.Snow.AuditService.Tests
{
    public class AuditEventProcessorTests
    {
       [Fact]
       public async void ProcessEvent_Verify_ExpectedCalls()
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
           var mockObjectDiffService = new Mock<IObjectDiffService>();
           var mockAuditObjectStateRepository = new Mock<IAuditObjectStateRepository>();
           
           var sut = CreateSut(mockAuditEventRepository.Object, mockAuditObjectStateRepository.Object, mockObjectDiffService.Object, mockAuditObjectStateFactory.Object);
           
           await sut.ProcessEvent(rfqEvent, new RfqAuditEventMapper(mockObjectDiffService.Object));
           
           mockAuditObjectStateRepository.Verify(x => x.GetByDataReferenceAsync(It.IsAny<AuditDataType>(), It.IsAny<string>()), Times.Once);
           mockAuditObjectStateFactory.Verify(x => x.Create(It.IsAny<AuditDataType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()), Times.Once);
           mockAuditObjectStateRepository.Verify(x => x.AddOrUpdate(It.IsAny<AuditObjectState>()), Times.Once);
           mockAuditEventRepository.Verify(x => x.AddAsync(It.IsAny<AuditModel<AuditDataType>>()), Times.Once);
       }

       [Fact]
       public async void ProcessEvent_ObjectStateShouldNotBeUpdated_IfEventTimestampIsOlder()
       {
           var newTimestamp = DateTime.UtcNow;
           var oldTimestamp = DateTime.UtcNow.AddSeconds(-30);
           
           var rfqEvent = new RfqEvent
           {
               BrokerId = "Spain",
               EventType = RfqEventTypeContract.Update,
               RfqSnapshot = new RfqContract 
               {
                   LastModified = oldTimestamp,
                   CausationOperationId = "operation-id-1",
                   State = RfqOperationState.Initiated,
                   Id = "id-1"
               }
           };

           // Setup the AuditObjectStateRepository so that it will return existing object with newer timestamp
           var mockAuditObjectStateRepository = new Mock<IAuditObjectStateRepository>();
           mockAuditObjectStateRepository.Setup(x => x.GetByDataReferenceAsync(It.IsAny<AuditDataType>(), It.IsAny<string>()))
            .ReturnsAsync(new AuditObjectState(AuditDataType.Rfq, "data-reference-1", "{}", newTimestamp));
           
           // Setup the AuditObjectStateFactory so that it will return new object with older timestamp
           var mockAuditObjectStateFactory = new Mock<IAuditObjectStateFactory>();
           mockAuditObjectStateFactory.Setup(x => x.Create(It.IsAny<AuditDataType>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<DateTime>()))
                .Returns(new AuditObjectState(AuditDataType.Rfq, "data-reference-1", "{}", oldTimestamp));
           
           var sut = CreateSut(auditObjectStateRepositoryArg: mockAuditObjectStateRepository.Object, auditObjectStateFactoryArg: mockAuditObjectStateFactory.Object);
           
           var mockObjectDiffService = new Mock<IObjectDiffService>();
           
           await sut.ProcessEvent(rfqEvent, new RfqAuditEventMapper(mockObjectDiffService.Object));

           // Verify that the AddOrUpdate() method has not been called
           mockAuditObjectStateRepository.Verify(x => x.AddOrUpdate(It.IsAny<AuditObjectState>()), Times.Never);
       }

       [Fact]
       public void GetRfqJsonDiff_Creation_VerifyExpectedMethodCalls()
       {
           var mockObjectDiffService = new Mock<IObjectDiffService>();
           
           var sut = CreateSut(objectDiffServiceArg: mockObjectDiffService.Object);
           
           var oldState = string.Empty;
           var newState = @"{""CreatedBy"": ""SYSTEM"", ""CorrelationId"": ""correlation-id-1"", ""State"": ""Initiated""}";
           
           var actual = sut.GetJsonDiff(oldState, newState);
           
           mockObjectDiffService.Verify(x => x.GetJsonDiff(It.IsAny<string>(), It.IsAny<string>()), Times.Never);

           Assert.Equal(newState, actual);
       }

       [Fact]
       public void GetRfqJsonDiff_Edition_VerifyExpectedMethodCalls()
       {
           var mockObjectDiffService = new Mock<IObjectDiffService>();
           
           var sut = CreateSut(objectDiffServiceArg: mockObjectDiffService.Object);
           
           var oldState = @"{""CreatedBy"": ""SYSTEM"", ""CorrelationId"": ""correlation-id-1"", ""State"": ""Initiated""}";
           var newState = @"{""CreatedBy"": ""SYSTEM"", ""CorrelationId"": ""correlation-id-1"", ""State"": ""Finished""}";
           
           var actual = sut.GetJsonDiff(oldState, newState);
           
           mockObjectDiffService.Verify(x => x.GetJsonDiff(It.Is<string>(a => a == oldState), It.Is<string>(a => a == newState)), Times.Once);
       }

        private AuditEventProcessor CreateSut(IAuditEventRepository? auditEventRepositoryArg = null,
            IAuditObjectStateRepository? auditObjectStateRepositoryArg = null,
            IObjectDiffService? objectDiffServiceArg = null,
            IAuditObjectStateFactory? auditObjectStateFactoryArg = null)
        {
            var auditEventRepository = new Mock<IAuditEventRepository>().Object;
            var auditObjectStateRepository = new Mock<IAuditObjectStateRepository>().Object;
            var objectDiffService = new Mock<IObjectDiffService>().Object;
            var auditObjectStateFactory = new Mock<IAuditObjectStateFactory>().Object;
            var mockLogger = new Mock<ILogger<AuditEventProcessor>>();

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
            
            return new AuditEventProcessor(auditEventRepository, auditObjectStateRepository, objectDiffService, auditObjectStateFactory, mockLogger.Object);
        }
    }
}