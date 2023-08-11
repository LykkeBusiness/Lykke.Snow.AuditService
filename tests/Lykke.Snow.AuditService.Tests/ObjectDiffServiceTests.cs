using System.Linq;
using Lykke.Snow.AuditService.DomainServices.Services;
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

        private ObjectDiffService CreateSut()
        {
            return new ObjectDiffService();
        }
    }
}