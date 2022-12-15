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

    public class HousingSearchGatewayTests
    {
        private readonly Mock<IHttpClientFactory> _httpClientFactory;
        private readonly HousingSearchGateway _systemUnderTest;
        private readonly MockHttpMessageHandler _mockHttp;

        private const string Postcode = "M3 OW";
        private const string AddressSearchResponse = @"{
          ""results"": {
            ""assets"": [
              {
                ""id"": ""c7f39282-2505-f0fb-fbe3-d4ae3c8b7097"",
                ""assetId"": ""100022574652"",
                ""assetType"": ""Dwelling"",
                ""isAssetCautionaryAlerted"": false,
                ""assetAddress"": {
                  ""uprn"": ""100022574652"",
                  ""addressLine1"": ""97A Old Church Road"",
                  ""addressLine2"": ""Chingford"",
                  ""addressLine3"": ""LONDON"",
                  ""addressLine4"": """",
                  ""postCode"": ""E4 6ST"",
                  ""postPreamble"": """"
                },
                ""tenure"": {
                  ""id"": ""6af3ff4e-09de-f27d-514e-20de013b207a"",
                  ""paymentReference"": ""9174652206"",
                  ""startOfTenureDate"": ""2013-06-24"",
                  ""endOfTenureDate"": ""2015-02-01"",
                  ""type"": ""Temp Private Lt"",
                  ""isActive"": false
                }
              }
            ]
          },
          ""total"": 1
        }";

        public HousingSearchGatewayTests()
        {
            _mockHttp = new MockHttpMessageHandler();

            _mockHttp.When($"*search/assets").WithQueryString("searchText", Postcode)
                .Respond("application/json", AddressSearchResponse);

            var httpClient = _mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");

            _httpClientFactory = new Mock<IHttpClientFactory>();
            _httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            _systemUnderTest = new HousingSearchGateway(_httpClientFactory.Object);
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenInvalidPostcodeArgument_WhenSearchingForPostcode_ThenAnExceptionIsThrown<T>(T exception, string postcode) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _systemUnderTest.SearchByPostcode(postcode);

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
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenNoExceptionIsThrown()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _systemUnderTest.SearchByPostcode(Postcode);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenAddressesAreRetrievedFromApi()
        {
            // Arrange

            // Act
            var results = await this._systemUnderTest.SearchByPostcode(Postcode);

            // Assert
            Assert.True(results.Any());
        }
    }
}
