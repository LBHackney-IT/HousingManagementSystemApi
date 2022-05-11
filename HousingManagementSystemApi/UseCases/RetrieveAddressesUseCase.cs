using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;
using HousingManagementSystemApi.Gateways;

namespace HousingManagementSystemApi.UseCases
{
    using System.Linq;
    using Dapper;
    using Hackney.Shared.Asset.Boundary.Response;
    using Hackney.Shared.Asset.Domain;
    using Microsoft.AspNetCore.Mvc;

    public class RetrieveAddressesUseCase : IRetrieveAddressesUseCase
    {
        private readonly IAddressesGateway addressesGateway;
        private readonly IAssetGateway assetGateway;
        private readonly IEnumerable<AssetType> assetTypes;

        public RetrieveAddressesUseCase(IAddressesGateway addressesGateway, IAssetGateway assetGateway, IEnumerable<AssetType> assetTypes)
        {
            this.addressesGateway = addressesGateway;
            this.assetGateway = assetGateway;
            this.assetTypes = assetTypes;
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
                    filteredAssets.Add(property);
                }
            }
            return filteredAssets;
        }
    }
}
