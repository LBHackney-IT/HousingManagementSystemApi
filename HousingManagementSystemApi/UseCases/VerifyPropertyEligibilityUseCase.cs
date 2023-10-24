using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;
using HousingManagementSystemApi.Gateways;

namespace HousingManagementSystemApi.UseCases
{
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using Amazon.Runtime.Internal.Util;
    using Hackney.Shared.Asset.Domain;
    using Hackney.Shared.Tenure.Domain;
    using HousingManagementSystemApi.Models;
    using Microsoft.Extensions.Logging;

    public class VerifyPropertyEligibilityUseCase : IVerifyPropertyEligibilityUseCase
    {
        private readonly IAssetGateway _assetGateway;
        private readonly IEnumerable<AssetType> _assetTypes;
        private readonly ITenureGateway _tenureGateway;
        private readonly ILogger<VerifyPropertyEligibilityUseCase> _logger;
        private readonly IRepairsHubAlertsGateway _repairsHubAlertGateway;

        private IEnumerable<string> EligibleTenureCodes { get; }

        public VerifyPropertyEligibilityUseCase(
            IAssetGateway assetGateway,
            IEnumerable<AssetType> assetTypes,
            ITenureGateway tenureGateway,
            IEnumerable<TenureType> eligibleTenureTypes,
            ILogger<VerifyPropertyEligibilityUseCase> logger,
            IRepairsHubAlertsGateway repairsHubAlertGateway)
        {
            _assetGateway = assetGateway;
            _assetTypes = assetTypes;
            _tenureGateway = tenureGateway;
            EligibleTenureCodes = eligibleTenureTypes.Select(x => x.Code);
            _logger = logger;
            _repairsHubAlertGateway = repairsHubAlertGateway;
        }

        public async Task<PropertyEligibilityResult> Execute(string propertyId)
        {
            _logger.LogInformation("Calling VerifyPropertyEligibilityUseCase for {PropertyId}", propertyId);

            var asset = await _assetGateway.RetrieveAsset(propertyId);

            if (asset == null)
            {
                _logger.LogInformation("The asset with property ID {PropertyId} cannot be found", propertyId);
                return new PropertyEligibilityResult(false, $"The asset with property ID {propertyId} cannot be found");
            }

            if (!_assetTypes.Contains(asset.AssetType))
            {
                _logger.LogInformation("The asset with property ID {PropertyId} is not eligible because of the asset type", propertyId);
                return new PropertyEligibilityResult(false, $"The asset with {propertyId} is not eligible because of the asset type");
            }

            if (asset.Tenure == null || asset.Tenure.Id == null)
            {
                _logger.LogInformation("Tenure or TenureId was null for property ID {PropertyId}", propertyId);
                return new PropertyEligibilityResult(false, $"The asset with property ID {propertyId} has no valid tenure");
            }

            if (asset.AssetManagement == null)
            {
                _logger.LogInformation("Can't find TMO status for property {PropertyId}", propertyId);
                return new PropertyEligibilityResult(false, $"Can't find TMO status for {propertyId}");
            }

            if (asset.AssetManagement.IsTMOManaged)
            {
                _logger.LogInformation("Property {PropertyId} is ineligible due to being managed by a TMO", propertyId);
                return new PropertyEligibilityResult(false, $"Asset {propertyId} is managed by a TMO");

            }

            var tenureInformation = await _tenureGateway.RetrieveTenureType(asset?.Tenure?.Id);
            var tenureTypeCode = tenureInformation?.TenureType?.Code;

            if (tenureTypeCode == null)
            {
                _logger.LogInformation("Tenure type code was null for asset with property ID {PropertyId}", propertyId);
                return new PropertyEligibilityResult(false, $"Tenure type code was null for asset with property ID {propertyId}");
            }

            if (!EligibleTenureCodes.Contains(tenureTypeCode))
            {
                _logger.LogInformation("TenureTypeCode for asset with property ID {PropertyId} is not suitable for Online Repairs", propertyId);
                return new PropertyEligibilityResult(false, $"Tenure type for property ID {propertyId} is not suitable for Online Repairs");
            }

            _logger.LogInformation("About to get location alerts for: ", propertyId);

            var locationAlerts = await _repairsHubAlertGateway.GetLocationAlerts(propertyId);

            if (locationAlerts.Alerts.Any())
            {
                var locationFailureText = $"Property {propertyId} is not eligable for RHOL due to having {locationAlerts.Alerts.Count} active Location Alert(s)";
                _logger.LogInformation(locationFailureText);
                return new PropertyEligibilityResult(false, locationFailureText);
            }

            return new PropertyEligibilityResult(true, "The property is valid");
        }
    }
}
