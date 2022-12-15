namespace HousingManagementSystemApi.Gateways;

using System.Threading.Tasks;
using Hackney.Shared.Asset.Boundary.Response;

public interface IAssetGateway
{
    public Task<AssetResponseObject> RetrieveAsset(string assetId);
}
