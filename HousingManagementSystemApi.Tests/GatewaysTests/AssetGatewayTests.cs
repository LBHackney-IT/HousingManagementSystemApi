namespace HousingManagementSystemApi.Tests.GatewaysTests
{
    using System;
    using System.Collections.Generic;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Gateways;
    using Hackney.Shared.Asset.Boundary.Response;
    using Moq;
    using RichardSzalay.MockHttp;
    using Xunit;

    public class AssetGatewayTests
    {
        public const string AssetId = "AssetId";
        private readonly AssetGateway systemUnderTest;
        private MockHttpMessageHandler mockHttp;

        private string assetResponse =
            "{\"id\":\"c7f39282-2505-f0fb-fbe3-d4ae3c8b7097\",\"assetId\":\"AssetId\",\"assetType\":\"Dwelling\",\"rootAsset\":\"ROOT\",\"parentAssetIds\":\"ROOT\",\"assetLocation\":{\"floorNo\":\"\",\"totalBlockFloors\":0,\"parentAssets\":[]},\"assetAddress\":{\"uprn\":\"100022574652\",\"addressLine1\":\"97A Old Church Road\",\"addressLine2\":\"Chingford\",\"addressLine3\":\"LONDON\",\"addressLine4\":\"\",\"postCode\":\"E4 6ST\",\"postPreamble\":\"\"},\"assetManagement\":{\"agent\":\"Homefinders\",\"areaOfficeName\":null,\"isCouncilProperty\":false,\"managingOrganisation\":\"London Borough of Hackney\",\"managingOrganisationId\":\"c01e3146-e630-c2cd-e709-18ef57bf3724\",\"owner\":\"\",\"isTMOManaged\":false,\"propertyOccupiedStatus\":\"VR\",\"isNoRepairsMaintenance\":false},\"assetCharacteristics\":{\"numberOfBedrooms\":3,\"numberOfLifts\":0,\"numberOfLivingRooms\":0,\"windowType\":\"\",\"yearConstructed\":\"0\"},\"tenure\":{\"id\":\"6af3ff4e-09de-f27d-514e-20de013b207a\",\"paymentReference\":\"9174652206\",\"type\":\"Temp Private Lt\",\"startOfTenureDate\":\"2013-06-24T00:00:00\",\"endOfTenureDate\":\"2015-02-01T00:00:00\",\"isActive\":false}}";


        public AssetGatewayTests()
        {
            mockHttp = new MockHttpMessageHandler();

            mockHttp.When($"*assets/assetId/{AssetId}")
                .Respond("application/json", assetResponse);

            var httpClient = mockHttp.ToHttpClient();
            httpClient.BaseAddress = new Uri("http://localhost/");

            var httpClientFactory = new Mock<IHttpClientFactory>();
            httpClientFactory.Setup(x => x.CreateClient(It.IsAny<string>())).Returns(httpClient);
            systemUnderTest = new AssetGateway(httpClientFactory.Object);
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
#pragma warning disable CA1707
        public async void GivenInvalidAssetIdArgument_WhenRetrievingAnAsset_ThenAnExceptionIsThrown<T>(T exception, string assetId) where T : Exception
#pragma warning restore CA1707
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.RetrieveAsset(assetId);

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
        public async void GivenAnAssetIdArgument_WhenRetrievingAnAsset_ThenNoExceptionIsThrown()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.RetrieveAsset(AssetId);

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenAnAssetId_WhenRetrievingAnAsset_ThenAddressesAreRetrievedFromApi()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            var results = await systemUnderTest.RetrieveAsset(AssetId);

            // Assert
            results.AssetId.Should().Be(AssetId);
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenNoAssetInApiResponse_WhenRetrievingAnAsset_ThenNoAssetIsReturned()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            var results = await systemUnderTest.RetrieveAsset("random");

            // Assert
            results.AssetId.Should().BeNull();
            results.Should().BeOfType(typeof(AssetResponseObject));
        }
    }

}
