namespace HousingManagementSystemApi.Tests.GatewaysTests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Gateways;
    using Hackney.Shared.Asset.Boundary.Response;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using RichardSzalay.MockHttp;
    using Xunit;

    public class AssetGatewayTests
    {
        public const string AssetId = "AssetId";
        private readonly AssetGateway _systemUnderTest;
        private readonly MockHttpMessageHandler _mockHttp;

        private readonly string _assetResponse =
            "{\"id\":\"c7f39282-2505-f0fb-fbe3-d4ae3c8b7097\"," +
            "\"assetId\":\"AssetId\"," +
            "\"assetType\":\"Dwelling\"," +
            "\"assetAddress\":{\"uprn\":\"100022574692\",\"addressLine1\":\"9 Old Church Road\",\"addressLine2\":\"Chingford\",\"addressLine3\":\"LONDON\",\"addressLine4\":\"\",\"postCode\":\"E4 6ST\",\"postPreamble\":\"\"}," +
            "\"tenure\":{\"id\":\"6af3ff4e-09de-f27d-514e-20de013b207a\"}}";


        public AssetGatewayTests()
        {
            _mockHttp = new MockHttpMessageHandler();

            _mockHttp.When($"*assets/assetId/{AssetId}")
                .Respond("application/json", _assetResponse);

            var httpClient = _mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            _systemUnderTest = new AssetGateway(httpClientFactory.Object, new NullLogger<AssetGateway>());
        }

        [Fact]
        public async void GivenAnAssetIdArgument_WhenRetrievingAnAsset_ThenNoExceptionIsThrown()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _systemUnderTest.RetrieveAsset(AssetId);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async void GivenAnAssetId_WhenRetrievingAnAsset_ThenAddressesAreRetrievedFromApi()
        {
            // Arrange

            // Act
            var results = await _systemUnderTest.RetrieveAsset(AssetId);

            // Assert
            results.AssetId.Should().Be(AssetId);
        }

        [Fact]
        public async void GivenNoAssetInApiResponse_WhenRetrievingAnAsset_ThenNoAssetIsReturned()
        {
            // Arrange

            // Act
            var results = await _systemUnderTest.RetrieveAsset("random");

            // Assert
            results.AssetId.Should().BeNull();
            results.Should().BeOfType(typeof(AssetResponseObject));
        }
    }

}
