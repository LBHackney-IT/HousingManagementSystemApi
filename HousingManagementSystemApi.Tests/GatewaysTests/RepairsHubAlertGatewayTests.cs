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

    public class RepairsHubAlertsGatewayTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly RepairsHubAlertsGateway _systemUnderTest;

        private readonly string _repairsHubAlertResponse = @"{

    ""Alerts"": [
      {
        ""AlertCode"": ""VA"",
        ""Description"": ""Test Alert Description"",
        ""EndDate"": ""2026-01-01"",
        ""StartDate"": ""2021-01-01""
      }
    ],
  ""Reference"": ""TEST REF""
}";

        private readonly MockHttpMessageHandler _mockHttp;

        public RepairsHubAlertsGatewayTests()
        {
            _mockHttp = new MockHttpMessageHandler();

            _mockHttp.When($"location-alerts")
                .Respond("application/json", _repairsHubAlertResponse);

            var httpClient = _mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");

            _httpClientFactory = new Mock<IHttpClientFactory>();
            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            _systemUnderTest = new RepairsHubAlertsGateway(_httpClientFactory.Object);
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
        public async void GivenAnInvalidPropertyReference_WhenGettingAlerts_ThenAnExceptionIsThrown<T>(T exception, string propRef) where T : Exception
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _systemUnderTest.GetLocationAlerts(propRef);

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
        public async void GivenAValidPropertyReference_WhenGettingAlerts_ThenNoExceptionIsThrown()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _systemUnderTest.GetLocationAlerts("00023400");

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async void GivenAValidPropertyReference_WhenGettingAlerts_ThenAddressesAreRetrievedFromApi()
        {
            // Arrange

            // Act
            var results = await _systemUnderTest.GetLocationAlerts("00023400");

            // Assert
            Assert.True(results.Alerts.Any());
        }
    }
}
