namespace HousingManagementSystemApi.Tests.UseCasesTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Moq;
    using Xunit;
    using HousingManagementSystemApi.Gateways;
    using Hackney.Shared.Asset.Domain;
    using Hackney.Shared.Tenure.Domain;
    using Hackney.Shared.Asset.Boundary.Response;
    using HousingManagementSystemApi.UseCases;
    using Microsoft.Extensions.Logging.Abstractions;

    public class VerifyPropertyEligibilityUseCaseTests
    {
        private readonly Mock<IAssetGateway> retrieveAssetGateway;
        private readonly Mock<ITenureGateway> tenureGateway;
        private readonly VerifyPropertyEligibilityUseCase sut;

        public static IEnumerable<AssetType> EligibleAssetTypes = new[]
{
            AssetType.Flat, AssetType.House, AssetType.Dwelling,
        };

        private readonly List<TenureType> eligibleTenureTypes = new()
        {
            TenureTypes.Introductory,
            TenureTypes.Secure,
            TenureTypes.Freehold
        };

        public VerifyPropertyEligibilityUseCaseTests()
        {
            this.retrieveAssetGateway = new Mock<IAssetGateway>();
            this.tenureGateway = new Mock<ITenureGateway>();
            this.sut = new VerifyPropertyEligibilityUseCase(retrieveAssetGateway.Object, EligibleAssetTypes, tenureGateway.Object, eligibleTenureTypes, new NullLogger<VerifyPropertyEligibilityUseCase>());
        }

        [Fact]
        public async Task GivenANullPropertyId_WhenUseCaseIsExecuted_ThenAnExceptionShouldBeThrown()
        {
            // Act
            var action = async () => await this.sut.Execute(null);

            // Assert
            await Assert.ThrowsAsync<ArgumentNullException>(action);
        }

        [Fact]
        public async Task GivenAssetCannotBeFound_WhenUseCaseIsExecuted_ThenThereWillBeAFailureResult()
        {
            // Arrange
            const string HouseThatDoesntExist = "Missing";

            retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatDoesntExist))
                .ReturnsAsync((AssetResponseObject)null);

            tenureGateway.Setup(x => x.RetrieveTenureType(It.IsAny<string>()))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await this.sut.Execute(HouseThatDoesntExist);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("The asset with property ID", result.Reason);
            Assert.Contains("cannot be found", result.Reason);
        }

        [Fact]
        public async Task GivenAssetWithAnInvalidAssetType_WhenUseCaseIsExecuted_ThenThereWillBeAFailureResult()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.TravellerSite,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    }
                });

            tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await this.sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("is not eligible because of the asset type", result.Reason);
        }

        [Fact]
        public async Task GivenAssetWithANullTenure_WhenUseCaseIsExecuted_ThenThereWillBeAFailureResult()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    }
                });

            tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync((TenureInformation)null);


            // Act
            var result = await this.sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("Tenure type code was null for asset", result.Reason);
        }

        [Fact]
        public async Task GivenAssetWithAnInvalidTenureType_WhenUseCaseIsExecuted_ThenThereWillBeAFailureResult()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    }
                });

            tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.AsylumSeeker });

            // Act
            var result = await this.sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("is not suitable for Online Repairs", result.Reason);
        }

        [Fact]
        public async Task GivenAllValidData_WhenUseCaseIsExecuted_ThenShouldBeASuccessfulValidation()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    }
                });

            tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await this.sut.Execute(HouseThatExists);

            // Assert
            Assert.True(result.PropertyEligible);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAnAddressExistsWithANullTenure_ThenAPropertyIsNotReturned()
        {
            const string TestPostcode = "postcode";
            var eligibleTenureType = TenureTypes.Secure;

            retrieveAssetGateway.Setup(x => x.RetrieveAsset("assetId"))
                .ReturnsAsync(new AssetResponseObject { AssetType = AssetType.Dwelling });


            tenureGateway.Setup(x => x.RetrieveTenureType("Id"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await this.sut.Execute(TestPostcode);
        }
    }
}
