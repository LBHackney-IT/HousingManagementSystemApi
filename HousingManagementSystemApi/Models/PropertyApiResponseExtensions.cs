namespace HousingManagementSystemApi.Models;

using HACT.Dtos;

public static class PropertyApiResponseExtensions
{
    public static PropertyAddress ToHactPropertyAddress(this PropertyApiResponse asset)
    {
        var propertyReference = asset.PropRef == null
            ? null
            : new Reference
            {
                ID = asset.PropRef,
                AllocatedBy = "PropertiesApi",
            };

        var result = new PropertyAddress
        {
            AddressLine = string.IsNullOrWhiteSpace(asset.Address1) ? null : new[] { asset.Address1 },
            PostalCode = asset.PostCode,
            Reference = propertyReference
        };

        return result;
    }
}
