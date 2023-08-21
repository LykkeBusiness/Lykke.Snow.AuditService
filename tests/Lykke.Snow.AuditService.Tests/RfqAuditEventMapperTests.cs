// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections;
using System.Collections.Generic;
using Common;
using Lykke.Snow.Audit;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.DomainServices.AuditEventMappers;
using MarginTrading.Backend.Contracts.Events;
using MarginTrading.Backend.Contracts.Rfq;
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
                  CreatedBy = username,
                  OriginatorType = RfqOriginatorType.Investor,
                  LastModified = now,
                  CausationOperationId = operationId,
                  State = RfqOperationState.Initiated,
                  Id = id
              }
          };
          
          var sut = CreateSut();
          
          var actual = sut.MapAuditEvent(rfqEvent, jsonDiff);
          
          Assert.Equal(now, actual.Timestamp);
          Assert.Equal(operationId, actual.CorrelationId);
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
      
      
      private RfqAuditEventMapper CreateSut()
      {
         return new RfqAuditEventMapper();
      }
   }
}