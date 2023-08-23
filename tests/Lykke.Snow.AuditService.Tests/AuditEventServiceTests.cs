// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.Domain.Repositories;
using Lykke.Snow.AuditService.Domain.Services;
using Lykke.Snow.AuditService.DomainServices.Services;
using Moq;
using Xunit;

namespace Lykke.Snow.AuditService.Tests
{
    public class AuditEventServiceTests
    {
        private readonly IList<IAuditModel<AuditDataType>> auditEventsTestData = new List<IAuditModel<AuditDataType>>()
        {
            new AuditModel<AuditDataType>() { Id = 1, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-1", UserName = "username-1", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 2, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-2", UserName = "username-2", Type = AuditEventType.Edition },
            new AuditModel<AuditDataType>() { Id = 3, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-3", UserName = "username-3", Type = AuditEventType.Deletion },
            new AuditModel<AuditDataType>() { Id = 4, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 5, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 6, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 7, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 8, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 9, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 10, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
        };

        //[Fact]
        //public async void GetAll_ShouldPassFilterParameter_ToGetAllAsyncCall()
        //{
        //    var filter = new AuditTrailFilter<AuditDataType>()
        //    {
        //        DataTypes = new AuditDataType[] { AuditDataType.Rfq },
        //        CorrelationId = "correlation-id-1",
        //        ActionType = AuditEventType.Deletion
        //    };

        //    var mockAuditEventRepository = new Mock<IAuditEventRepository>();
        //    mockAuditEventRepository.Setup(x => x.GetAllAsync(It.IsAny<AuditTrailFilter<AuditDataType>>()))
        //        .ReturnsAsync(auditEventsTestData);

        //    var sut = CreateSut(auditEventRepositoryArg: mockAuditEventRepository.Object);
        //    
        //    await sut.GetAll(filter, new Dictionary<AuditDataType, List<JsonDiffFilter>>());
        //    
        //    mockAuditEventRepository.Verify(x => x.GetAllAsync(It.Is<AuditTrailFilter<AuditDataType>>(
        //        f => f.DataTypes == filter.DataTypes &&
        //        f.CorrelationId == filter.CorrelationId &&
        //        f.ActionType == filter.ActionType
        //    )), Times.Once);
        //}
        
        // TODO: revise tests
        //[Fact]
        //public async void GetAll_ShouldPerformJsonDiffFilter_IfProvided()
        //{
        //    var filter = new AuditTrailFilter<AuditDataType>()
        //    {
        //        DataTypes = new AuditDataType[] { AuditDataType.Rfq },
        //        CorrelationId = "correlation-id-1",
        //        ActionType = AuditEventType.Deletion
        //    };
        //    
        //    var mockAuditEventRepository = new Mock<IAuditEventRepository>();
        //    mockAuditEventRepository.Setup(x => x.GetAllAsync(It.IsAny<AuditTrailFilter<AuditDataType>>()))
        //        .ReturnsAsync(auditEventsTestData);

        //    var mockObjectDiffService = new Mock<IObjectDiffService>();
        //    
        //    var sut = CreateSut(mockAuditEventRepository.Object, mockObjectDiffService.Object);
        //    
        //    var jsonDiffFilter = new JsonDiffFilter(propertyName: "SamplePropertyName");
        //    var jsonDiffFilters = new List<JsonDiffFilter> { jsonDiffFilter };
        //    
        //    await sut.GetAll(filter, jsonDiffFilters);
        //    
        //    mockObjectDiffService.Verify(x => x.FilterBasedOnJsonDiff(It.IsAny<IList<IAuditModel<AuditDataType>>>(), 
        //        It.Is<IEnumerable<JsonDiffFilter>>(x => x == jsonDiffFilters)), 
        //        Times.Once);
        //}

        //[Theory]
        //[InlineData(0, 10, 10, 0, 10, 10)]
        //[InlineData(1, 5, 5, 1, 5, 10)]
        //[InlineData(0, 7, 7, 0, 7, 10)]
        //[InlineData(4, 10, 6, 4, 6, 10)]
        //public async void GetAll_ShouldPaginateResults_BasedOnGivenSkipAndTake(int skip, int take, 
        //    int expectedArrayLength,
        //    int expectedStart, int expectedSize, int expectedTotalSize)
        //{
        //    var filter = new AuditTrailFilter<AuditDataType>()
        //    {
        //        DataTypes = new AuditDataType[] { AuditDataType.Rfq },
        //        CorrelationId = "correlation-id-1",
        //        ActionType = AuditEventType.Deletion
        //    };
        //    
        //    var mockAuditEventRepository = new Mock<IAuditEventRepository>();
        //    mockAuditEventRepository.Setup(x => x.GetAllAsync(It.IsAny<AuditTrailFilter<AuditDataType>>()))
        //        .ReturnsAsync(auditEventsTestData);

        //    var mockObjectDiffService = new Mock<IObjectDiffService>();
        //    
        //    var sut = CreateSut(mockAuditEventRepository.Object, mockObjectDiffService.Object);
        //    
        //    var result = await sut.GetAll(filter, new List<JsonDiffFilter>(), skip, take);
        //    
        //    Assert.Equal(expectedArrayLength, result.Contents.Count);
        //    Assert.Equal(expectedStart, result.Start);
        //    Assert.Equal(expectedSize, result.Size);
        //    Assert.Equal(expectedTotalSize, result.TotalSize);
        //}
        
        private AuditEventService CreateSut(IAuditEventRepository? auditEventRepositoryArg = null, IObjectDiffService? objectDiffServiceArg = null)
        {
            IAuditEventRepository auditEventRepository = new Mock<IAuditEventRepository>().Object;
            IObjectDiffService objectDiffService = new Mock<IObjectDiffService>().Object;
            
            if(auditEventRepositoryArg != null)
            {
                auditEventRepository = auditEventRepositoryArg;
            }
            
            if(objectDiffServiceArg != null)
            {
                objectDiffService = objectDiffServiceArg;
            }
            
            return new AuditEventService(auditEventRepository, objectDiffService);
        }
    }
}