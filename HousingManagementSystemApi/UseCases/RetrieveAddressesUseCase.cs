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
    using Microsoft.Extensions.Logging;

    public class RetrieveAddressesUseCase : IRetrieveAddressesUseCase
    {
        private readonly IAddressesGateway _addressesGateway;
        private readonly IAssetGateway _assetGateway;
        private readonly IEnumerable<AssetType> _assetTypes;
        private readonly ITenureGateway _tenureGateway;
        private readonly ILogger<RetrieveAddressesUseCase> _logger;

        private IEnumerable<string> EligibleTenureCodes { get; }

        public RetrieveAddressesUseCase(
            IAddressesGateway addressesGateway,
            IAssetGateway assetGateway,
            IEnumerable<AssetType> assetTypes,
            ITenureGateway tenureGateway,
            IEnumerable<TenureType> eligibleTenureTypes,
            ILogger<RetrieveAddressesUseCase> logger)
        {
            _addressesGateway = addressesGateway;
            _assetGateway = assetGateway;
            _assetTypes = assetTypes;
            _tenureGateway = tenureGateway;
            EligibleTenureCodes = eligibleTenureTypes.Select(x => x.Code);
            _logger = logger;
        }

        public async Task<IEnumerable<PropertyAddress>> Execute(string postCode)
        {
            _logger.LogInformation("Calling RetrieveAddressesUseCase for {PostCode}", postCode);

            if (string.IsNullOrWhiteSpace(postCode))
            {
                _logger.LogInformation("The value of {PostCode} was null or whitespace. Throwing Exception", postCode);
                throw new ArgumentNullException(nameof(postCode));
            }

            var result = await _addressesGateway.SearchByPostcode(postCode);

            _logger.LogInformation("Retrieved {Count} results from addressGateway for {PostCode}", result.Count(), postCode);

            var filteredAssets = new List<PropertyAddress>();

            await Parallel.ForEachAsync(result, async (property, _) =>
            {
                var filteredAsset = await FilterAsset(property, postCode);

                if (filteredAsset != null)
                {
                    filteredAssets.Add(filteredAsset);
                }
            });

            _logger.LogInformation("Returning {Count} filteredAssets from RetrieveAddressesUseCase for {PostCode}", filteredAssets.Count(), postCode);

            return filteredAssets;
        }

        private async Task<PropertyAddress> FilterAsset(PropertyAddress property, string postCode)
        {
            var asset = await _assetGateway.RetrieveAsset(property.Reference.ID);

            if (asset == null)
            {
                _logger.LogInformation("The asset with {Id} returned null for postCode {PostCode}. Skipping iteration", property.Reference.ID, postCode);
                return null;
            }

            if (!_assetTypes.Contains(asset.AssetType))
            {
                _logger.LogInformation("The asset with {Id} was skipped because the assetType was {AssetType} for postCode {PostCode}. Skipping iteration", property.Reference.ID, asset.AssetType, postCode);
                return null;
            }

            if (asset.Tenure == null || asset.Tenure.Id == null)
            {
                _logger.LogInformation("Tenure or TenureId was null for postCode {PostCode}. Skipping iteration", postCode);
                return null;
            }

            var tenureInformation = await _tenureGateway.RetrieveTenureType(asset?.Tenure?.Id);
            var tenureTypeCode = tenureInformation?.TenureType?.Code;

            if (tenureTypeCode == null)
            {
                _logger.LogInformation("TenureTypeCode was null for postCode {PostCode}. Skipping iteration", postCode);
                return null;
            }

            if (!EligibleTenureCodes.Contains(tenureTypeCode))
            {
                _logger.LogInformation("The asset was skipped because tenureTypeCode was {TenureTypeCode} for postCode {PostCode}. Skipping iteration", tenureTypeCode, postCode);
                return null;
            }

            return property;
        }
    }
}
