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

        public async Task<IEnumerable<PropertyAddress>> Execute(string postcode)
        {
            _logger.LogInformation("Calling RetrieveAddressesUseCase for {PostCode}", postcode);

            if (string.IsNullOrWhiteSpace(postcode))
            {
                _logger.LogInformation("The value of {PostCode} was null or whitespace. Throwing Exception", postcode);
                throw new ArgumentNullException(nameof(postcode));
            }

            var result = await _addressesGateway.SearchByPostcode(postcode);

            _logger.LogInformation("Retrieved {Count} results from addressGateway for {PostCode}", result.Count(), postcode);

            var filteredAssets = new List<PropertyAddress>();

            await Parallel.ForEachAsync(result, async (property, _) =>
            {
                var filteredAsset = await FilterAsset(property);

                if (filteredAsset != null)
                {
                    filteredAssets.Add(filteredAsset);
                }
            });

            _logger.LogInformation("Returning {Count} filteredAssets from RetrieveAddressesUseCase for {PostCode}", filteredAssets.Count(), postcode);

            return filteredAssets;
        }

        private async Task<PropertyAddress> FilterAsset(PropertyAddress property)
        {
            var asset = await _assetGateway.RetrieveAsset(property.Reference.ID);

            if (asset == null)
            {
                _logger.LogInformation("The asset with {Id} returned null for postCode {PostCode}. Skipping iteration", property.Reference.ID, postcode);
                return null;
            }

            if (!_assetTypes.Contains(asset.AssetType))
            {
                _logger.LogInformation("The asset with {Id} was skipped because the assetType was {AssetType} for postCode {PostCode}. Skipping iteration", property.Reference.ID, asset.AssetType, postcode);
                return null;
            }

            if (asset?.Tenure?.Id == null)
            {
                _logger.LogInformation("TenureId was null for postCode {PostCode}. Skipping iteration", postcode);
                return null;
            }

            var tenureInformation = await _tenureGateway.RetrieveTenureType(asset?.Tenure?.Id);
            var tenureTypeCode = tenureInformation?.TenureType?.Code;

            if (tenureTypeCode != null && EligibleTenureCodes.Contains(tenureTypeCode))
            {
                return property;
            }

            return null;
        }
    }
}
