// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.DomainServices.Services;
using Xunit;

namespace Lykke.Snow.AuditService.Tests
{
    public class AuditObjectStateFactoryTests
    {
        [Fact]
        public void Create_ShouldCreateANewAuditObjectState_WithCorrectProperties()
        {
            var dataType = AuditDataType.Rfq;
            var dataReference = "data-reference-1";
            var jsonState = "{}";
            var lastModified = DateTime.UtcNow;
            
            var sut = CreateSut();
            
            var actual = sut.Create(dataType, dataReference, jsonState, lastModified);
            
            Assert.Equal(dataType, actual.DataType);
            Assert.Equal(dataReference, actual.DataReference);
            Assert.Equal(jsonState, actual.StateInJson);
            Assert.Equal(lastModified, actual.LastModified);
        }
        
        private AuditObjectStateFactory CreateSut()
        {
            return new AuditObjectStateFactory();
        }
    }
}
