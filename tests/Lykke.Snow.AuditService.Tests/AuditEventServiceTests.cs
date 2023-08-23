// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using System.Collections.Generic;
using System.Linq;
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
            new AuditModel<AuditDataType>() { Id = 1, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-1", UserName = "username-1", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 2, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-2", UserName = "username-2", Type = AuditEventType.Edition },
            new AuditModel<AuditDataType>() { Id = 3, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-3", UserName = "username-3", Type = AuditEventType.Deletion },
            new AuditModel<AuditDataType>() { Id = 4, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 5, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 6, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 7, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 8, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 9, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 10, DataType = AuditDataType.Rfq, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 11, DataType = AuditDataType.Axle, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 12, DataType = AuditDataType.CorporateActions, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
            new AuditModel<AuditDataType>() { Id = 13, DataType = AuditDataType.CorporateActions, Timestamp = DateTime.UtcNow, CorrelationId = "correlationid-4", UserName = "username-4", Type = AuditEventType.Creation },
        };
        
        [Fact]
        public async void GetAll_ShouldPassCollectiveDataTypesBasedOnDomainFilters_ToTheAuditEventRepository()
        {
            var filter = new AuditTrailFilter<AuditDataType>()
            {
            };
            
            var domainFilters = new Dictionary<AuditDataType, List<JsonDiffFilter>>()
            {
                { 
                    AuditDataType.Rfq, new List<JsonDiffFilter> { new JsonDiffFilter(propertyName: "State" )}
                },
                {
                    AuditDataType.Axle, new List<JsonDiffFilter>()
                },
                {
                    AuditDataType.CorporateActions, new List<JsonDiffFilter>()
                }
            };
   
            var mockAuditEventRepository = new Mock<IAuditEventRepository>();
            mockAuditEventRepository.Setup(x => x.GetAllAsync(It.IsAny<AuditTrailFilter<AuditDataType>>()))
                .ReturnsAsync(auditEventsTestData);

            var sut = CreateSut(auditEventRepositoryArg: mockAuditEventRepository.Object);
            
            await sut.GetAll(filter, domainFilters);
            
            mockAuditEventRepository
                .Verify(x => x.GetAllAsync(It.Is<AuditTrailFilter<AuditDataType>>(filter => 
                    filter.DataTypes.Length == 3 &&
                    filter.DataTypes.Contains(AuditDataType.Rfq) && filter.DataTypes.Contains(AuditDataType.CorporateActions) && filter.DataTypes.Contains(AuditDataType.Axle)) ));
        }

        [Fact]
        public async void GetAll_ShouldApplyJsonDiffFilters_OnTypeFilteredResults()
        {
            var filter = new AuditTrailFilter<AuditDataType>()
            {
            };
            
            var domainFilters = new Dictionary<AuditDataType, List<JsonDiffFilter>>()
            {
                { 
                    AuditDataType.Rfq, new List<JsonDiffFilter> { new JsonDiffFilter(propertyName: "State" )}
                },
                {
                    AuditDataType.Axle, new List<JsonDiffFilter>()
                },
                {
                    AuditDataType.CorporateActions, new List<JsonDiffFilter>
                    {
                        new JsonDiffFilter(propertyName: "InstrumentId", value: "BAYER_AG")
                    }
                }
            };
   
            var mockAuditEventRepository = new Mock<IAuditEventRepository>();
            mockAuditEventRepository.Setup(x => x.GetAllAsync(It.IsAny<AuditTrailFilter<AuditDataType>>()))
                .ReturnsAsync(auditEventsTestData);
            
            var mockObjectDiffService = new Mock<IObjectDiffService>();
            mockObjectDiffService.Setup(x => x.FilterBasedOnJsonDiff(It.IsAny<List<IAuditModel<AuditDataType>>>(), 
                It.IsAny<IEnumerable<JsonDiffFilter>>())).Returns(new List<IAuditModel<AuditDataType>>());

            var sut = CreateSut(auditEventRepositoryArg: mockAuditEventRepository.Object, objectDiffServiceArg: mockObjectDiffService.Object);
            
            await sut.GetAll(filter, domainFilters);
            

            mockObjectDiffService.Verify(x => 
                x.FilterBasedOnJsonDiff(It.Is<List<IAuditModel<AuditDataType>>>(x => x.All(elm => elm.DataType == AuditDataType.Rfq)), 
                It.Is<List<JsonDiffFilter>>(jsonDiffFilters => jsonDiffFilters.Count() == 1 && jsonDiffFilters.First().PropertyName == "State" && jsonDiffFilters.First().Value == null)), Times.Once);

            mockObjectDiffService.Verify(x => 
                x.FilterBasedOnJsonDiff(It.Is<List<IAuditModel<AuditDataType>>>(x => x.All(elm => elm.DataType == AuditDataType.Axle)), 
                It.IsAny<List<JsonDiffFilter>>()), Times.Never);

            mockObjectDiffService.Verify(x => 
                x.FilterBasedOnJsonDiff(It.Is<List<IAuditModel<AuditDataType>>>(x => x.All(elm => elm.DataType == AuditDataType.CorporateActions)), 
                It.Is<List<JsonDiffFilter>>(jsonDiffFilters => jsonDiffFilters.Count == 1 
                && jsonDiffFilters.First().PropertyName == "InstrumentId" 
                && jsonDiffFilters.First().Value != null 
                && (string?)jsonDiffFilters.First().Value == "BAYER_AG")), Times.Once);
        }

        [Theory]
        [InlineData(0, 10, 10, 0, 10, 13)]
        [InlineData(1, 5, 5, 1, 5, 13)]
        [InlineData(0, 7, 7, 0, 7, 13)]
        [InlineData(6, 10, 7, 6, 7, 13)]
        public async void GetAll_ShouldPaginateResults_BasedOnGivenSkipAndTake(int skip, int take, 
            int expectedArrayLength,
            int expectedStart, int expectedSize, int expectedTotalSize)
        {
            var filter = new AuditTrailFilter<AuditDataType>()
            {
            };

            var domainFilters = new Dictionary<AuditDataType, List<JsonDiffFilter>>()
            {
                { 
                    AuditDataType.Rfq, new List<JsonDiffFilter>() 
                },
                {
                    AuditDataType.Axle, new List<JsonDiffFilter>()
                },
                {
                    AuditDataType.CorporateActions, new List<JsonDiffFilter>()
                }
            };
            
            var mockAuditEventRepository = new Mock<IAuditEventRepository>();
            mockAuditEventRepository.Setup(x => x.GetAllAsync(It.IsAny<AuditTrailFilter<AuditDataType>>()))
                .ReturnsAsync(auditEventsTestData);

            var mockObjectDiffService = new Mock<IObjectDiffService>();
            mockObjectDiffService.Setup(x => x.FilterBasedOnJsonDiff(It.IsAny<List<IAuditModel<AuditDataType>>>(), 
                It.IsAny<IEnumerable<JsonDiffFilter>>())).Returns(auditEventsTestData);
            
            var sut = CreateSut(mockAuditEventRepository.Object, mockObjectDiffService.Object);
            
            var result = await sut.GetAll(filter, domainFilters, skip, take);
            
            Assert.Equal(expectedArrayLength, result.Contents.Count);
            Assert.Equal(expectedStart, result.Start);
            Assert.Equal(expectedSize, result.Size);
            Assert.Equal(expectedTotalSize, result.TotalSize);
        }
        
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