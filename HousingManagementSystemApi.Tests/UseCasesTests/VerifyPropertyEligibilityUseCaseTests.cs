namespace HousingManagementSystemApi.Tests.UseCasesTests
{
    using System;
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Hackney.Shared.Asset.Boundary.Response;
    using Hackney.Shared.Asset.Domain;
    using Hackney.Shared.Tenure.Domain;
    using HousingManagementSystemApi.Gateways;
    using HousingManagementSystemApi.UseCases;
    using HousingManagementSystemApi.Models.RepairsHub;
    using Microsoft.Extensions.Logging.Abstractions;
    using Moq;
    using Xunit;

    public class VerifyPropertyEligibilityUseCaseTests
    {
        private readonly Mock<IAssetGateway> _retrieveAssetGateway;
        private readonly Mock<ITenureGateway> _tenureGateway;
        private readonly VerifyPropertyEligibilityUseCase _sut;
        private readonly Mock<IRepairsHubAlertsGateway> _alertsGatewayMock;

        public static readonly IEnumerable<AssetType> EligibleAssetTypes = new[]
{
            AssetType.Flat, AssetType.House, AssetType.Dwelling,
        };

        private readonly List<TenureType> eligibleTenureTypes = new()
        {
            TenureTypes.Introductory,
            TenureTypes.Secure,
            TenureTypes.Freehold
        };

        private readonly AlertsViewModel noAlerts = new()
        {
            Alerts = new List<CautionaryAlertViewModel>()
        };

        public VerifyPropertyEligibilityUseCaseTests()
        {
            _retrieveAssetGateway = new Mock<IAssetGateway>();
            _tenureGateway = new Mock<ITenureGateway>();
            _alertsGatewayMock = new Mock<IRepairsHubAlertsGateway>();

            _alertsGatewayMock.Setup(s => s.GetPersonAlerts(It.IsAny<string>()))
                .ReturnsAsync(noAlerts);

            _alertsGatewayMock.Setup(s => s.GetLocationAlerts(It.IsAny<string>()))
                .ReturnsAsync(noAlerts);

            _sut = new VerifyPropertyEligibilityUseCase(_retrieveAssetGateway.Object, EligibleAssetTypes, _tenureGateway.Object, eligibleTenureTypes, new NullLogger<VerifyPropertyEligibilityUseCase>(), _alertsGatewayMock.Object);
        }

        [Fact]
        public async Task GivenAssetCannotBeFound_WhenUseCaseIsExecuted_ThenThereWillBeAFailureResult()
        {
            // Arrange
            const string HouseThatDoesntExist = "Missing";

            _retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatDoesntExist))
                .ReturnsAsync((AssetResponseObject) null);

            _tenureGateway.Setup(x => x.RetrieveTenureType(It.IsAny<string>()))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await _sut.Execute(HouseThatDoesntExist);

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

            _retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.TravellerSite,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    }
                });

            _tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await _sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("is not eligible because of the asset type", result.Reason);
        }

        [Fact]
        public async Task GivenAssetWithANullTenure_WhenUseCaseIsExecuted_ThenThereWillBeAFailureResult()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            _retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    },
                    AssetManagement = new AssetManagement
                    {
                        IsTMOManaged = false,
                    }
                });

            _tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync((TenureInformation) null);


            // Act
            var result = await _sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("Tenure type code was null for asset", result.Reason);
        }

        [Fact]
        public async Task GivenAssetWithAnInvalidTenureType_WhenUseCaseIsExecuted_ThenThereWillBeAFailureResult()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            _retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    },
                    AssetManagement = new AssetManagement
                    {
                        IsTMOManaged = false,
                    }
                });

            _tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.AsylumSeeker });

            // Act
            var result = await _sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("is not suitable for Online Repairs", result.Reason);
        }

        [Fact]
        public async Task GivenAllValidData_WhenUseCaseIsExecuted_ThenShouldBeASuccessfulValidation()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            _retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    },
                    AssetManagement = new AssetManagement
                    {
                        IsTMOManaged = false,
                    }
                });

            _tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await _sut.Execute(HouseThatExists);

            // Assert
            Assert.True(result.PropertyEligible);
        }

        [Fact]
        public async Task GivenAMissingAssetManagementRecord_WhenUseCaseIsExecuted_ThenShouldBeAFailureResult()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            _retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    }
                });

            _tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await _sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("Can't find TMO status for", result.Reason);
        }

        [Fact]
        public async Task GivenATmoManagedProperty_WhenUseCaseIsExecuted_ThenShouldBeAFailureResult()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            _retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    },
                    AssetManagement = new AssetManagement
                    {
                        IsTMOManaged = true,
                    }
                });

            _tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            // Act
            var result = await _sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("is managed by a TMO", result.Reason);
        }

        [Fact]
        public async Task GivenAPropertyWithLocationAlerts_WhenUseCaseIsExecuted_ThenShouldBeAFailureResult()
        {
            // Arrange
            const string HouseThatExists = "01234567";

            _retrieveAssetGateway.Setup(x => x.RetrieveAsset(HouseThatExists))
                .ReturnsAsync(new AssetResponseObject
                {
                    AssetType = AssetType.Dwelling,
                    Tenure = new AssetTenureResponseObject
                    {
                        Id = "TEN001"
                    },
                    AssetManagement = new AssetManagement
                    {
                        IsTMOManaged = false,
                    }
                });

            _tenureGateway.Setup(x => x.RetrieveTenureType("TEN001"))
                .ReturnsAsync(new TenureInformation { TenureType = TenureTypes.Secure });

            _alertsGatewayMock.Setup(s => s.GetLocationAlerts(It.IsAny<string>()))
                .ReturnsAsync(new AlertsViewModel
                {
                    Alerts = new List<CautionaryAlertViewModel>
                    {
                        new CautionaryAlertViewModel
                        {
                            Type = "VA",
                            Comments = "Violent resident"
                        }
                    },
                    Reference = "TEST REFERENCE"
                });
            ;

            // Act
            var result = await _sut.Execute(HouseThatExists);

            // Assert
            Assert.False(result.PropertyEligible);
            Assert.Contains("not eligable for RHOL due to having 1 active Location Alert", result.Reason);
        }
    }
}
