using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HACT.Dtos;
using HousingManagementSystemApi.Gateways;
using HousingManagementSystemApi.UseCases;
using Moq;
using Xunit;
namespace HousingManagementSystemApi.Tests
{
    using System.Collections.Generic;
    using Hackney.Shared.Asset.Boundary.Response;
    using Hackney.Shared.Asset.Domain;

    public class RetrieveAddressesUseCaseTests
    {
        private readonly Mock<IAddressesGateway> retrieveAddressesGateway;
        private readonly Mock<IAssetGateway> retrieveAssetGateway;
        private readonly RetrieveAddressesUseCase retrieveAddressesUseCase;

        public static IEnumerable<AssetType> EligibleAssetTypes = new[]
        {
            AssetType.Flat, AssetType.House, AssetType.Dwelling,
        };
        public RetrieveAddressesUseCaseTests()
        {
            retrieveAddressesGateway = new Mock<IAddressesGateway>();
            retrieveAssetGateway = new Mock<IAssetGateway>();
            retrieveAddressesUseCase = new RetrieveAddressesUseCase(retrieveAddressesGateway.Object, retrieveAssetGateway.Object, EligibleAssetTypes);

        }

        [Fact]
        public async Task GivenAPostcode_WhenExecute_GatewayReceivesCorrectInput()
        {
            const string TestPostcode = "postcode";
            retrieveAddressesGateway.Setup(x => x.SearchByPostcode(TestPostcode));
            await retrieveAddressesUseCase.Execute(TestPostcode);
            retrieveAddressesGateway.Verify(x => x.SearchByPostcode(TestPostcode), Times.Once);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAnAddressExistsWithAnEligibleAssetType_ThenThePropertyIsReturned()
        {
            const string TestPostcode = "postcode";
            retrieveAddressesGateway.Setup(x => x.SearchByPostcode(TestPostcode))
                .ReturnsAsync(new PropertyAddress[] { new() { PostalCode = TestPostcode, Reference = new Reference { ID = "assetId" } } });

            retrieveAssetGateway.Setup(x => x.RetrieveAsset("assetId"))
                .ReturnsAsync(new AssetResponseObject { AssetType = AssetType.Dwelling });

            var result = await retrieveAddressesUseCase.Execute(TestPostcode);
            result.First().PostalCode.Should().Be(TestPostcode);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAnAddressExistsWithAnIneligibleAssetType_ThenAPropertyIsNotReturned()
        {
            const string TestPostcode = "postcode";
            retrieveAddressesGateway.Setup(x => x.SearchByPostcode(TestPostcode))
                .ReturnsAsync(new PropertyAddress[] { new() { PostalCode = TestPostcode, Reference = new Reference { ID = "assetId" } } });

            retrieveAssetGateway.Setup(x => x.RetrieveAsset("assetId"))
                .ReturnsAsync(new AssetResponseObject { AssetType = AssetType.Concierge });

            var result = await retrieveAddressesUseCase.Execute(TestPostcode);
            result.Should().BeEmpty();
        }

        [Fact]
        public async void GivenNullPostcode_WhenExecute_ThrowsNullException()
        {
            Func<Task> act = async () => await retrieveAddressesUseCase.Execute(null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenEmptyPostcode_WhenExecute_SearchByPostcodeIsNotCalled()
        {
            const string TestPostcode = "";
            await retrieveAddressesUseCase.Execute(postcode: TestPostcode);
            retrieveAddressesGateway.Verify(x => x.SearchByPostcode(TestPostcode), Times.Never);
        }
    }

}
