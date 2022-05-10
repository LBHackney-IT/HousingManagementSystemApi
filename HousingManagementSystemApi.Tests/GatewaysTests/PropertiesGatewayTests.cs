namespace HousingManagementSystemApi.Tests.GatewaysTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Gateways;
    using Moq;
    using RichardSzalay.MockHttp;
    using Xunit;

    public class PropertiesGatewayTests
    {
        private readonly PropertiesGateway systemUnderTest;

        private const string PostcodeSingleAddress = "E100 1QZ";
        private string propertiesSearchResponseSingleAddress =
            @"[
  {
    ""propRef"": ""100000000001"",
    ""postCode"": ""E100 1QZ"",
    ""address1"": ""1A Old Hackney Road""
  }
]";

        private const string PostcodeMultipleAddresses = "E100 1QQ";
        private string PropertiesSearchResponseMultipleAddresses =
            @"[
  {
    ""propRef"": ""100000000001"",
    ""postCode"": ""E100 1QQ"",
    ""address1"": ""1A Old Hackney Road""
  },
    {
    ""propRef"": ""100000000002"",
    ""postCode"": ""E100 1QQ"",
    ""address1"": ""2A Old Hackney Road""
    }
]";

        private MockHttpMessageHandler mockHttp;

        public PropertiesGatewayTests()
        {
            mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"*properties").WithQueryString("postcode", PostcodeSingleAddress)
                .Respond("application/json", propertiesSearchResponseSingleAddress);

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            systemUnderTest = new PropertiesGateway(httpClientFactory.Object);
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
#pragma warning disable CA1707
        public async void GivenInvalidPostcodeArgument_WhenSearchingForPostcode_ThenAnExceptionIsThrown<T>(T exception, string postcode) where T : Exception
#pragma warning restore CA1707
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.SearchByPostcode(postcode);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        public static IEnumerable<object[]> InvalidArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), " " };
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenNoExceptionIsThrown()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.SearchByPostcode(PostcodeSingleAddress);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenAddressesAreRetrievedFromApi()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            var results = await systemUnderTest.SearchByPostcode(PostcodeSingleAddress);

            // Assert
            Assert.True(results.Any());
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenNoPropertyInApiResponse_WhenSearchingForPostcode_ThenNoAddressesAreReturned()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            var results = await systemUnderTest.SearchByPostcode("E4 0PR");

            // Assert
            Assert.Empty(results);
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenASinglePropertyInApiResponse_WhenSearchingForPostcode_ThenASingleAddressIsReturned()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            var results = await systemUnderTest.SearchByPostcode(PostcodeSingleAddress);

            // Assert
            Assert.Single(results);
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenMultiplePropertiesInApiResponse_WhenSearchingForPostcode_ThenMultipleAddressesAreReturned()
#pragma warning restore CA1707
        {
            // Arrange
            mockHttp.When($"*properties").WithQueryString("postcode", PostcodeMultipleAddresses)
                .Respond("application/json", PropertiesSearchResponseMultipleAddresses);

            // Act
            var results = await systemUnderTest.SearchByPostcode(PostcodeMultipleAddresses);

            // Assert
            Assert.Equal(2, results.Count());
        }
    }
}
