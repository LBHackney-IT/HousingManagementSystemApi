namespace HousingManagementSystemApi.Models;

using System.Linq;
using Hackney.Shared.Asset.Domain;
using HACT.Dtos;

public static class AssetsExtensions
{
    public static PropertyAddress ToHactPropertyAddress(this Asset asset)
    {
        var propertyReference = asset.AssetId == null
            ? null
            : new Reference
            {
                ID = asset.AssetId,
                AllocatedBy = "HousingSearchApi",
            };

        var addressLines =
            new[]
            {
                asset.AssetAddress.AddressLine1,
                asset.AssetAddress.AddressLine2,
                asset.AssetAddress.AddressLine3,
                asset.AssetAddress.AddressLine4
            }.Where(x => !string.IsNullOrWhiteSpace(x));

        var result = new PropertyAddress
        {
            AddressLine = addressLines.ToArray(),
            PostalCode = asset.AssetAddress.PostCode,
            BuildingNumber = "PLACEHOLDER",
            Reference = propertyReference
        };

        return result;
    }
}
