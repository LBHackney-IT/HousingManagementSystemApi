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

        private IEnumerable<string> EligibleTenureCodes { get; }

        public VerifyPropertyEligibilityUseCase(
            IAssetGateway assetGateway,
            IEnumerable<AssetType> assetTypes,
            ITenureGateway tenureGateway,
            IEnumerable<TenureType> eligibleTenureTypes,
            ILogger<VerifyPropertyEligibilityUseCase> logger)
        {
            _assetGateway = assetGateway;
            _assetTypes = assetTypes;
            _tenureGateway = tenureGateway;
            EligibleTenureCodes = eligibleTenureTypes.Select(x => x.Code);
            _logger = logger;
        }

        public async Task<PropertyEligibilityResult> Execute(string propertyId)
        {
            _logger.LogInformation("Calling VerifyPropertyEligibilityUseCase for {PropertyId}", propertyId);

            if (string.IsNullOrEmpty(propertyId))
            {
                _logger.LogInformation("The property ID {PropertyId} was null or empty. Throwing Exception", propertyId);
                throw new ArgumentNullException(nameof(propertyId));
            }

            var asset = await _assetGateway.RetrieveAsset(propertyId);

            if (asset == null)
            {
                _logger.LogInformation("The asset with property ID {Id} cannot be found", propertyId);
                return new PropertyEligibilityResult(false, $"The asset with property ID {propertyId} cannot be found");
            }

            if (!_assetTypes.Contains(asset.AssetType))
            {
                _logger.LogInformation("The asset with {PropertyId} is not eligible because of the asset type", propertyId);
                return new PropertyEligibilityResult(false, $"The asset with {propertyId} is not eligible because of the asset type");
            }

            if (asset.Tenure == null || asset.Tenure.Id == null)
            {
                _logger.LogInformation("Tenure or TenureId was null for {PropertyId}", propertyId);
                return new PropertyEligibilityResult(false, $"The asset with {propertyId} has no valid tenure");
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
                return new PropertyEligibilityResult(false, $"Tenure type for property {propertyId} is not suitable for Online Repairs");
            }

            return new PropertyEligibilityResult(true, "The property is valid");
        }
    }
}
