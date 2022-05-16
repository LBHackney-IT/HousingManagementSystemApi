using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;
using HousingManagementSystemApi.Gateways;

namespace HousingManagementSystemApi.UseCases
{
    using System.Linq;
    using Hackney.Shared.Asset.Domain;
    using Hackney.Shared.Tenure.Domain;

    public class RetrieveAddressesUseCase : IRetrieveAddressesUseCase
    {
        private readonly IAddressesGateway addressesGateway;
        private readonly IAssetGateway assetGateway;
        private readonly IEnumerable<AssetType> assetTypes;
        private readonly ITenureGateway tenureGateway;

        private IEnumerable<string> EligibleTenureCodes { get; }
        public RetrieveAddressesUseCase(IAddressesGateway addressesGateway, IAssetGateway assetGateway,
            IEnumerable<AssetType> assetTypes, ITenureGateway tenureGateway, IEnumerable<TenureType> eligibleTenureTypes)
        {
            this.addressesGateway = addressesGateway;
            this.assetGateway = assetGateway;
            this.assetTypes = assetTypes;
            this.tenureGateway = tenureGateway;
            this.EligibleTenureCodes = eligibleTenureTypes.Select(x => x.Code);
        }

        public async Task<IEnumerable<PropertyAddress>> Execute(string postcode)
        {
            if (postcode == null)
                throw new ArgumentNullException(nameof(postcode));

            if (postcode == "")
                return new List<PropertyAddress>();
            var result = await addressesGateway.SearchByPostcode(postcode);
            var filteredAssets = new List<PropertyAddress>();
            foreach (var property in result)
            {
                var asset = await assetGateway.RetrieveAsset(property.Reference.ID);
                if (assetTypes.Contains(asset.AssetType))
                {
                    var tenureInformation = await tenureGateway.RetrieveTenureType(asset?.Tenure?.Id);
                    var tenureTypeCode = tenureInformation.TenureType.Code;

                    if (EligibleTenureCodes.Contains(tenureTypeCode))
                    {
                        filteredAssets.Add(property);
                    }
                }
            }

            return filteredAssets;
        }
    }
}
