// Copyright (c) 2023 Lykke Corp.
// See the LICENSE file in the project root for more information.

using System.Collections.Generic;
using System.Linq;
using Lykke.Snow.Audit;
using Lykke.Snow.Audit.Abstractions;
using Lykke.Snow.AuditService.Domain.Enum;
using Lykke.Snow.AuditService.Domain.Model;
using Lykke.Snow.AuditService.DomainServices.Services;
using Microsoft.Extensions.Logging;
using Moq;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Xunit;

namespace Lykke.Snow.AuditService.Tests
{
    class Customer
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public int Age { get; set; }
        public string Language { get; set; } = string.Empty;
    }

    public class ObjectDiffServiceTests
    {
        /// <summary>
        /// Test method that tests generating new json diff without an old object
        /// </summary>
        [Fact]
        public void GenerateNewJsonDiff_ShouldCreate_NewJsonData()
        {
            var customer1 = new Customer() 
            { 
                FirstName = "jane", 
                LastName = "doe", 
                Email = "jane.doe@example.com",
                Age = 18,
                Language = "en"
            };
            
            var sut = CreateSut();
            
            var actual = sut.GenerateNewJsonDiff(customer1);

            JObject jobject = JObject.Parse(actual);
            
            Assert.Equal(5, jobject.Children().Count());
            
            var firstNameToken = jobject[nameof(Customer.FirstName)];
            var lastNameToken = jobject[nameof(Customer.LastName)];
            var emailToken = jobject[nameof(Customer.Email)];
            var ageToken = jobject[nameof(Customer.Age)];
            var languageToken = jobject[nameof(Customer.Language)];
            
            Assert.Equal(1, firstNameToken?.Children().Count());
            Assert.Equal(customer1.FirstName, firstNameToken?.First?.ToString());

            Assert.Equal(1, lastNameToken?.Children().Count());
            Assert.Equal(customer1.LastName, lastNameToken?.First?.ToString());

            Assert.Equal(1, emailToken?.Children().Count());
            Assert.Equal(customer1.Email, emailToken?.First?.ToString());

            Assert.Equal(1, ageToken?.Children().Count());
            Assert.Equal(customer1.Age.ToString(), ageToken?.First?.ToString());

            Assert.Equal(1, languageToken?.Children().Count());
            Assert.Equal(customer1.Language, languageToken?.First?.ToString());
        }
        
        /// <summary>
        /// Test method that checks if GetJsonDiff() with two object paramter works as expected
        /// </summary>
        [Fact]
        public void GetJsonDiff_ShouldGenerateJsonDiff_BasedOnTwoObjects()
        {
            var customer1 = new Customer() 
            { 
                FirstName = "jane", 
                LastName = "doe", 
                Email = "jane.doe@example.com",
                Age = 18,
                Language = "en"
            };

            var customer2 = new Customer() 
            { 
                FirstName = "jane", 
                LastName = "doe", 
                Email = "jane.doe@example.com",
                Age = 21,
                Language = "es"
            };
            
            var sut = CreateSut();
            
            var actual = sut.GetJsonDiff(customer1, customer2);
            
            var jobject = JObject.Parse(actual);
            
            Assert.Equal(2, jobject.Children().Count());
            
            var ageToken = jobject[nameof(Customer.Age)];
            var languageToken = jobject[nameof(Customer.Language)];
            
            Assert.Equal(2, ageToken?.Children().Count());
            Assert.Equal(customer1.Age.ToString(), ageToken?.First?.ToString());
            Assert.Equal(customer2.Age.ToString(), ageToken?.Last?.ToString());
            
            Assert.Equal(2, languageToken?.Children().Count());
            Assert.Equal(customer1.Language, languageToken?.First?.ToString());
            Assert.Equal(customer2.Language, languageToken?.Last?.ToString());
        }

        /// <summary>
        /// Test method that checks if GetJsonDiff() with two json strings works as expected
        /// </summary>
        [Fact]
        public void GetJsonDiff_ShouldGenerateJsonDiff_BasedOnTwoJsonStrings()
        {
            var state1 = @"{""FirstName"":""jane"", ""LastName"": ""doe"", ""Email"":""jane.doe@example.com"", ""Age"":18, ""Language"":""en""}";
            var state2 = @"{""FirstName"":""jane"", ""LastName"": ""doe smith"", ""Email"":""jane.doe.smith@example.com"", ""Age"":18, ""Language"":""en""}";

            var sut = CreateSut();
            
            var actual = sut.GetJsonDiff(state1, state2);
            
            var jobject = JObject.Parse(actual);
            
            Assert.Equal(2, jobject.Children().Count());
            
            var lastNameToken = jobject[nameof(Customer.LastName)];
            var emailToken = jobject[nameof(Customer.Email)];

            Assert.Equal(2, lastNameToken?.Children().Count());
            Assert.Equal("doe", lastNameToken?.First?.ToString());
            Assert.Equal("doe smith", lastNameToken?.Last?.ToString());

            Assert.Equal(2, emailToken?.Children().Count());
            Assert.Equal("jane.doe@example.com", emailToken?.First?.ToString());
            Assert.Equal("jane.doe.smith@example.com", emailToken?.Last?.ToString());
        }
        
        /// <summary>
        /// Test method that tests if json diff filter work when the diff the client is interested in is the only diff.
        //  In this test there's two auditEvents in the collection 
        //  The aimed auditEvent has only one property - which is the one that the client is interested in
        /// </summary>
        [Fact]
        public void FilterBasedOnJsonDiff_WithoutAnyExtraDiff_ShouldWorkAsExpected()
        {
            var auditEvent1 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""State"": [
                            ""Initiated"",
                            ""Started""
                        ]
                    }
                " 
            };

            var auditEvent2 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""AccountId"": [
                            ""account-1"",
                            ""account-2""
                        ]
                    }
                " 
            };
            
            var list = new List<IAuditModel<AuditDataType>>
            {
                auditEvent1, 
                auditEvent2
            };

            var sut = CreateSut();
            
            var jsonDiffFilter = new JsonDiffFilter(propertyName: "State");

            var actual = sut.FilterBasedOnJsonDiff(list, new List<JsonDiffFilter>() { jsonDiffFilter });
            var item = Assert.Single(actual);

            Assert.Equal(auditEvent1, item);
        }

        /// <summary>
        /// Test method that tests if json diff filter works when there's more properties than the one client is interested in.
        /// </summary>
        [Fact]
        public void FilterBasedOnJsonDiff_WithMorePropertyDiffs_ShouldWorkAsExpected()
        {
            var auditEvent1 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""State"": [
                            ""Initiated"",
                            ""Started""
                        ],
                        ""RequestNumber"": [
                            4,
                            5
                        ],
                        ""InstrumentId"": [
                            ""old-instrument"",
                            ""new-instrument""
                        ]
                    }
                " 
            };

            var auditEvent2 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""AccountId"": [
                            ""account-1"",
                            ""account-2""
                        ]
                    }
                " 
            };
            
            var list = new List<IAuditModel<AuditDataType>>()
            {
                auditEvent1,
                auditEvent2
            };

            var sut = CreateSut();
            
            var jsonDiffFilter = new JsonDiffFilter(propertyName: "State");

            var actual = sut.FilterBasedOnJsonDiff(list, new List<JsonDiffFilter> { jsonDiffFilter });
            var item = Assert.Single(actual);

            Assert.Equal(auditEvent1, item);
        }

        /// <summary>
        /// Test method that tests if json diff filter works when there's more than one match in given collection.
        /// </summary>
        [Fact]
        public void FilterBasedOnJsonDiff_WithMoreThanOneMatch_ShouldReturnAllItems()
        {
            var auditEvent1 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""State"": [
                            ""Initiated"",
                            ""Started""
                        ],
                        ""RequestNumber"": [
                            4,
                            5
                        ],
                        ""InstrumentId"": [
                            ""old-instrument"",
                            ""new-instrument""
                        ]
                    }
                " 
            };

            var auditEvent2 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""AccountId"": [
                            ""account-1"",
                            ""account-2""
                        ],
                        ""RequestNumber"": [
                            3,
                            4
                        ]
                    }
                " 
            };
            
            var list = new List<IAuditModel<AuditDataType>>();
            list.Add(auditEvent1);
            list.Add(auditEvent2);

            var sut = CreateSut();
            
            var jsonDiffFilter = new JsonDiffFilter(propertyName: "RequestNumber");

            var actual = sut.FilterBasedOnJsonDiff(list, new List<JsonDiffFilter> { jsonDiffFilter });
            var count = actual.Count();
            
            Assert.Equal(2, count);
            
            Assert.Collection(actual, 
                (item) => Assert.Equal(auditEvent1, item),
                (item) => Assert.Equal(auditEvent2, item));
        }

        /// <summary>
        /// Test method that tests if json diff filter works when there's multiple Properties which are targeted by the JsonDiffFilter.
        /// </summary>
        [Fact]
        public void FilterBasedOnJsonDiff_WithMultipleJsonDiffFilter_ShouldReturnCorrectItems_1()
        {
            var auditEvent1 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""State"": [
                            ""Initiated"",
                            ""Started""
                        ],
                        ""RequestNumber"": [
                            4,
                            5
                        ],
                        ""InstrumentId"": [
                            ""old-instrument"",
                            ""new-instrument""
                        ]
                    }
                " 
            };

            var auditEvent2 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""RequestNumber"": [
                            4,
                            5
                        ],
                        ""InstrumentId"": [
                            ""old-instrument"",
                            ""new-instrument""
                        ]
                    }
                " 
            };

            var auditEvent3 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""InstrumentId"": [
                            ""old-instrument"",
                            ""new-instrument""
                        ]
                    }
                " 
            };

            var auditEvent4 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""RequestNumber"": [
                            7,
                            8
                        ]
                    }
                " 
            };
            
            var list = new List<IAuditModel<AuditDataType>>
            {
                auditEvent1,
                auditEvent2,
                auditEvent3,
                auditEvent4
            };

            var sut = CreateSut();
            
            var jsonDiffFilter1 = new JsonDiffFilter(propertyName: "State");
            var jsonDiffFilter2 = new JsonDiffFilter(propertyName: "RequestNumber");

            var actual = sut.FilterBasedOnJsonDiff(list, new List<JsonDiffFilter> { jsonDiffFilter1, jsonDiffFilter2 });
            var count = actual.Count();
            
            Assert.Equal(3, count);
            
            Assert.Collection(actual, 
                (item) => Assert.Equal(auditEvent1, item),
                (item) => Assert.Equal(auditEvent2, item),
                (item) => Assert.Equal(auditEvent4, item));
        }

        /// <summary>
        /// Test method that tests if json diff filter works when there's multiple Property which is targeted by the JsonDiffFilter.
        /// In this test method the collection contains three element. And json filter targets three properties.
        /// One of them has one of the targeted properties.
        /// The rest of the auditevents does not contain any of targeted properties.
        /// </summary>
        [Fact]
        public void FilterBasedOnJsonDiff_WithMultipleJsonDiffFilter_ShouldReturnCorrectItems_2()
        {
            var auditEvent1 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""State"": [
                            ""Initiated"",
                            ""Started""
                        ]
                    }
                " 
            };

            var auditEvent2 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""InstrumentId"": [
                            ""old-instrument"",
                            ""new-instrument""
                        ]
                    }
                " 
            };

            var auditEvent3 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""LastUpdatedBy"": [
                            ""user1"",
                            ""user2""
                        ]
                    }
                " 
            };

            var auditEvent4 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""Provider"": [
                            ""provider1"",
                            ""provider2""
                        ]
                    }
                " 
            };
            
            var list = new List<IAuditModel<AuditDataType>>
            {
                auditEvent1,
                auditEvent2,
                auditEvent3,
                auditEvent4,
            };

            var sut = CreateSut();
            
            var jsonDiffFilter1 = new JsonDiffFilter(propertyName: "CorrelationId");
            var jsonDiffFilter2 = new JsonDiffFilter(propertyName: "RequestNumber");
            var jsonDiffFilter3 = new JsonDiffFilter(propertyName: "Timestamp");
            var jsonDiffFilter4 = new JsonDiffFilter(propertyName: "Provider");


            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                jsonDiffFilter1,
                jsonDiffFilter2,
                jsonDiffFilter3,
                jsonDiffFilter4,
            };

            var actual = sut.FilterBasedOnJsonDiff(list, jsonDiffFilters);
            var count = actual.Count();
            
            Assert.Equal(1, count);
            
            Assert.Collection(actual, 
                (item) => Assert.Equal(item, auditEvent4));
        }

        /// <summary>
        /// This test method tests FilterBasedOnJsonDiff function against test cases
        /// Where JsonDiff filter contains a STRING value along a with a property, 
        /// (e.g. JsonDiffFilter(propertyName: "State", value: "Started"))
        /// This method checks if the sut returns the audit events in which the state has been transitioned to "Started"
        /// </summary>
        [Fact]
        public void FilterBasedOnJsonDiff_ShouldWorkAsExpected_WhenFilterValueIsAString()
        {
            var auditEvent1 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""State"": [
                            ""Initiated"",
                            ""Started""
                        ]
                    }
                " 
            };

            var auditEvent2 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""InstrumentId"": [
                            ""old-instrument"",
                            ""new-instrument""
                        ]
                    }
                " 
            };

            var auditEvent3 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""LastUpdatedBy"": [
                            ""user1"",
                            ""user2""
                        ]
                    }
                " 
            };

            var auditEvent4 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""Provider"": [
                            ""provider1"",
                            ""provider2""
                        ]
                    }
                " 
            };

            var list = new List<IAuditModel<AuditDataType>>
            {
                auditEvent1,
                auditEvent2,
                auditEvent3,
                auditEvent4
            };
            
            var sut = CreateSut();
            
            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                new JsonDiffFilter("State", "Started"),
                new JsonDiffFilter("Provider", "provider2")
            };
            
            var actual = sut.FilterBasedOnJsonDiff(list, jsonDiffFilters);
            
            Assert.Equal(2, actual.Count());
            
            Assert.Collection(actual, 
                (item) => Assert.Equal(auditEvent1, item),
                (item) => Assert.Equal(auditEvent4, item));
        }
        
        /// <summary>
        /// This test method tests FilterBasedOnJsonDiff function against test cases
        /// Where JsonDiff filter contains an INTEGER value along a with a property, 
        /// (e.g. JsonDiffFilter(propertyName: "State", value: "Started"))
        /// This method checks if the sut returns the audit events in which the state has been transitioned to "Started"
        /// </summary>
        [Fact]
        public void FilterBasedOnJsonDiff_ShouldWorkAsExpected_WhenFilterValueIsAnInteger()
        {
            var auditEvent1 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""State"": [
                            ""Initiated"",
                            ""Started""
                        ]
                    }
                " 
            };

            var auditEvent2 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""InstrumentId"": [
                            ""old-instrument"",
                            ""new-instrument""
                        ],
                        ""RequestNumber"": [
                            6,
                            7
                        ]
                    }
                " 
            };

            var auditEvent3 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""LastUpdatedBy"": [
                            ""user1"",
                            ""user2""
                        ]
                    }
                " 
            };

            var auditEvent4 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = @"
                    {
                        ""Provider"": [
                            ""provider1"",
                            ""provider2""
                        ]
                    }
                " 
            };

            var list = new List<IAuditModel<AuditDataType>>
            {
                auditEvent1,
                auditEvent2,
                auditEvent3,
                auditEvent4
            };
            
            var sut = CreateSut();
            
            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                new JsonDiffFilter("RequestNumber", 7),
            };
            
            var actual = sut.FilterBasedOnJsonDiff(list, jsonDiffFilters);
            
            Assert.Single(actual);
            
            Assert.Collection(actual, 
                (item) => Assert.Equal(auditEvent2, item));
        }
        
        /// <summary>
        /// This test method tests CheckJsonProperties function against test cases
        /// Where there's only "PropertyName" (e.g. JsonDiffFilter(propertyName: "State"))
        /// This method checks if sut finds the match with given JsonDiffFilter
        /// </summary>
        [Fact]
        public void CheckJsonProperties_ShouldReturnTrue_OnlyWithPropertyName()
        {
            var jsonProperties = new List<JProperty>
            {
                new JProperty("State", new JArray(new string[] { "Initiated", "Started"})),
                new JProperty("RequestNumber", new JArray(new int[] { 1, 2})),
                new JProperty("UserName", new JArray(new string[] { "investor-1", "investor-2"})),
                new JProperty("AccountId", new JArray(new string[] { "account-1", "account-2"}))
            };
            
            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                new JsonDiffFilter("UserName")
            };
            
            var sut = CreateSut();
            
            var actual = sut.CheckJsonProperties(jsonProperties, jsonDiffFilters);
            
            Assert.True(actual);
        }

        /// <summary>
        /// This test method tests CheckJsonProperties function against test cases
        /// Where there's only "PropertyName" (e.g. JsonDiffFilter(propertyName: "State"))
        /// This method checks if sut returns false when DataDiff does not contain any of given JsonDiffFilter
        /// </summary>
        [Fact]
        public void CheckJsonProperties_ShouldReturnFalse_WhenPropertyDoesNotExist()
        {
            var jsonProperties = new List<JProperty>
            {
                new JProperty("State", new JArray(new string[] { "Initiated", "Started"})),
                new JProperty("RequestNumber", new JArray(new int[] { 1, 2})),
                new JProperty("UserName", new JArray(new string[] { "investor-1", "investor-2"})),
                new JProperty("AccountId", new JArray(new string[] { "account-1", "account-2"}))
            };
            
            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                new JsonDiffFilter("LastModified")
            };
            
            var sut = CreateSut();
            
            var actual = sut.CheckJsonProperties(jsonProperties, jsonDiffFilters);
            
            Assert.False(actual);
        }
        
        /// <summary>
        /// This test method tests CheckJsonProperties function against test cases
        /// Where there's value along with "PropertyName" (e.g. JsonDiffFilter(propertyName: "State", value: "Started"))
        /// This method checks if sut finds the value match and returns true
        /// </summary>
        [Fact]
        public void CheckJsonProperties_ShouldReturnTrue_WhenValueIsPassed()
        {
            var jsonProperties = new List<JProperty>
            {
                new JProperty("State", new JArray(new string[] { "Initiated", "Started"})),
                new JProperty("RequestNumber", new JArray(new int[] { 1, 2})),
                new JProperty("UserName", new JArray(new string[] { "investor-1", "investor-2"})),
                new JProperty("AccountId", new JArray(new string[] { "account-1", "account-2"}))
            };
            
            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                new JsonDiffFilter("UserName", "investor-2")
            };
            
            var sut = CreateSut();
            
            var actual = sut.CheckJsonProperties(jsonProperties, jsonDiffFilters);
            
            Assert.True(actual);
        }

        /// <summary>
        /// This test method tests CheckJsonProperties function against test cases
        /// Where there's value along with "PropertyName" (e.g. JsonDiffFilter(propertyName: "State", value: "Started"))
        /// This method checks if sut returns false when a different value is presented
        /// </summary>
        [Fact]
        public void CheckJsonProperties_ShouldReturnFalse_WhenInvalidValueIsPassed()
        {
            var jsonProperties = new List<JProperty>
            {
                new JProperty("State", @"[""Initiated"", ""Started""]"),
                new JProperty("RequestNumber", @"[1, 2]"),
                new JProperty("UserName",  @"[""investor-1"", ""investor-2""]"),
                new JProperty("AccountId", @"[""account-1"", ""account-2""]")
            };
            
            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                new JsonDiffFilter("UserName", "some-other-investor")
            };
            
            var sut = CreateSut();
            
            var actual = sut.CheckJsonProperties(jsonProperties, jsonDiffFilters);
            
            Assert.False(actual);
        }

        /// <summary>
        /// This test method tests CheckJsonProperties function against test cases
        /// Where there's value along with "PropertyName" (e.g. JsonDiffFilter(propertyName: "State", value: "Started"))
        /// And there's only one element in DataDiff (it's data Creation - there's no old value)
        /// </summary>
        [Fact]
        public void CheckJsonProperties_ShouldReturnTrue_ForDiffsWithOneValue()
        {
            var jsonProperties = new List<JProperty>
            {
                new JProperty("State", new JArray(new string[] { "Initiated", "Started"})),
                new JProperty("RequestNumber", new JArray(new int[] { 1 })),
                new JProperty("UserName", new JArray(new string[] { "investor-1", "investor-2"})),
                new JProperty("AccountId", new JArray(new string[] { "account-1", "account-2"}))
            };
            
            var jsonDiffFilters = new List<JsonDiffFilter>
            {
                new JsonDiffFilter("RequestNumber", 1)
            };
            
            var sut = CreateSut();
            
            var actual = sut.CheckJsonProperties(jsonProperties, jsonDiffFilters);
            
            Assert.True(actual);
        }

        [Fact]
        public void FilterBasedOnJsonDiff_ShouldThrowJsonReaderException_WhenInvalidJsonPassed()
        {
            var auditEvent1 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = "invalid-json" 
            };

            var auditEvent2 = new AuditModel<AuditDataType>() 
            { 
                DataDiff = "invalid-json"
            };
            
            var list = new List<IAuditModel<AuditDataType>>();
            list.Add(auditEvent1);
            list.Add(auditEvent2);

            var sut = CreateSut();

            var jsonDiffFilter = new JsonDiffFilter(propertyName: "RequestNumber");
            
            Assert.Throws<JsonReaderException>(() => sut.FilterBasedOnJsonDiff(list, new List<JsonDiffFilter> { jsonDiffFilter }).ToList());
        }

        private ObjectDiffService CreateSut()
        {
            var mockLogger = new Mock<ILogger<ObjectDiffService>>();

            return new ObjectDiffService(mockLogger.Object);
        }
    }
}