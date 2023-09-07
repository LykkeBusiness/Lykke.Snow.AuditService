// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.AuditEventMappers;
using MarginTrading.Backend.Contracts.Events;
using MarginTrading.Backend.Contracts.Rfq;
using Moq;
using Newtonsoft.Json.Linq;
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

   public class RfqAuditEventMapperTests
   {
      [Fact]
      public void GetAuditDataType_ShouldReturn_TheCorrectAuditDataType()
      {
         var sut = CreateSut();
         
         var actual = sut.GetAuditDataType(new RfqEvent());
         
         Assert.Equal(AuditDataType.Rfq, actual);
      }
      
      [Fact]
      public void GetDataReference_ShouldReturn_TheCorrectValue()
      { 
         var id = "rfq-snapshot-id";

         var sut = CreateSut();
         
         var rfqEvent = new RfqEvent { RfqSnapshot = new RfqContract { Id = id }};
         
         var actual = sut.GetDataReference(rfqEvent);
         
         Assert.Equal(id, actual);
      }
      
      // Make sure type is = 'Creation' without need of checking json diff when 
      // New rfqEvent is passed
      [Fact]
      public void GetAuditEventType_ShouldReturn_TheCorrectEventTypeBasedOnJsonDiff_1()
      {
          var rfqEvent = new RfqEvent 
          {
              EventType = RfqEventTypeContract.New
          };
          
          var objectDiffServiceMock = new Mock<IObjectDiffService>();
          
          var sut = CreateSut(objectDiffServiceArg: objectDiffServiceMock.Object);
          
          var actual = sut.GetAuditEventType(rfqEvent, @"{""State"": [""Initiated"", ""Started""]}");

        
          Assert.Equal(AuditEventType.Creation, actual);
          
          objectDiffServiceMock.Verify(x => x.CheckJsonProperties(It.IsAny<IEnumerable<JProperty>>(), It.IsAny<IEnumerable<JsonDiffFilter>>()), 
            Times.Never);
      }

      // Make sure type is = 'StatusChanged' when State has changed with RfqEvent
      [Fact]
      public void GetAuditEventType_ShouldReturn_TheCorrectEventTypeBasedOnJsonDiff_2()
      {
          var rfqEvent = new RfqEvent 
          {
              EventType = RfqEventTypeContract.Update
          };
          
          var objectDiffServiceMock = new Mock<IObjectDiffService>();
          objectDiffServiceMock.Setup(x => x.CheckJsonProperties(It.IsAny<IEnumerable<JProperty>>(), 
            It.Is<IEnumerable<JsonDiffFilter>>(filters => filters.Count(x => x.PropertyName == "State") > 0)))
            .Returns(true);
          
          var sut = CreateSut(objectDiffServiceArg: objectDiffServiceMock.Object);
          
          var actual = sut.GetAuditEventType(rfqEvent, @"{""State"": [""Initiated"", ""Started""], ""LastModified"": [""2023-08-25T13:00:00"", ""2023-08-25T14:00:00""]}");

          Assert.Equal(AuditEventType.StatusChanged, actual);
          
          objectDiffServiceMock.Verify(x => x.CheckJsonProperties(It.IsAny<IEnumerable<JProperty>>(), It.IsAny<IEnumerable<JsonDiffFilter>>()), 
            Times.Once);
      }

      // Make sure type is = 'Edition' when State has not changed
      [Fact]
      public void GetAuditEventType_ShouldReturn_TheCorrectEventTypeBasedOnJsonDiff_3()
      {
          var rfqEvent = new RfqEvent 
          {
              EventType = RfqEventTypeContract.Update
          };
          
          var objectDiffServiceMock = new Mock<IObjectDiffService>();
          objectDiffServiceMock.Setup(x => x.CheckJsonProperties(It.IsAny<IEnumerable<JProperty>>(), 
            It.Is<IEnumerable<JsonDiffFilter>>(filters => filters.Count(x => x.PropertyName == "State") > 0)))
            .Returns(false);
          
          var sut = CreateSut(objectDiffServiceArg: objectDiffServiceMock.Object);
          
          var actual = sut.GetAuditEventType(rfqEvent, @"{""AccountId"": [""account-1"", ""account-2""]}");

          Assert.Equal(AuditEventType.Edition, actual);
          
          objectDiffServiceMock.Verify(x => x.CheckJsonProperties(It.IsAny<IEnumerable<JProperty>>(), It.IsAny<IEnumerable<JsonDiffFilter>>()), 
            Times.Once);
      }
      [Fact]
      public void GetAuditEvent_ShouldMapFields_AndGenerateAuditModel()
      {
          var now = DateTime.UtcNow;
          var id = "id-1";
          var username = "username-1";
          var jsonDiff = "json-diff-1";

          var rfqEvent = new RfqEvent
          {
              BrokerId = "Spain",
              EventType = RfqEventTypeContract.New,
              RfqSnapshot = new RfqContract 
              {
                  CreatedBy = username,
                  OriginatorType = RfqOriginatorType.Investor,
                  LastModified = now,
                  State = RfqOperationState.Initiated,
                  Id = id
              }
          };
          
          var sut = CreateSut();
          
          var actual = sut.MapAuditEvent(rfqEvent, jsonDiff);
          
          Assert.Equal(now, actual.Timestamp);
          Assert.Equal(username, actual.UserName);
          Assert.Equal(AuditEventType.Creation, actual.Type);
          Assert.Equal(RfqOperationState.Initiated.ToString(), actual.AuditEventTypeDetails);
          Assert.Equal(AuditDataType.Rfq, actual.DataType);
          Assert.Equal(id, actual.DataReference);
          Assert.Equal(jsonDiff, actual.DataDiff);
      }
      
      [Fact]
      public void GetStateInJson_ShouldReturn_TheJsonPayloadOfRfqSnapshot()
      {
          var now = DateTime.UtcNow;
          var operationId = "op-id-1";
          var id = "id-1";
          var username = "username-1";

          var rfqEvent = new RfqEvent
          {
              BrokerId = "Spain",
              EventType = RfqEventTypeContract.New,
              RfqSnapshot = new RfqContract 
              {
                  CreatedBy = username,
                  OriginatorType = RfqOriginatorType.Investor,
                  LastModified = now,
                  CausationOperationId = operationId,
                  State = RfqOperationState.Initiated,
                  Id = id
              }
          };
          
          var sut = CreateSut();
          
          var actual = sut.GetStateInJson(rfqEvent);
          
          Assert.Equal(rfqEvent.RfqSnapshot.ToJson(), actual);
      }

      [Theory]
      [ClassData(typeof(RfqEventUsernameTestData))]
      public void GetEventUserName_ShouldReturnCorrectUsername_ForTheRfqEvent(RfqEvent rfqEvent, string expectedUsername)
      {
          var sut = CreateSut();
          
          var actual = sut.GetEventUsername(rfqEvent);
          
          Assert.Equal(expectedUsername, actual);
      }
      
      
      private RfqAuditEventMapper CreateSut(IObjectDiffService? objectDiffServiceArg = null)
      {
          IObjectDiffService objectDiffService = new Mock<IObjectDiffService>().Object;
          
          if(objectDiffServiceArg != null)
          {
              objectDiffService = objectDiffServiceArg;
          }

          return new RfqAuditEventMapper(objectDiffService, new Common.Correlation.CorrelationContextAccessor());
      }
   }
}