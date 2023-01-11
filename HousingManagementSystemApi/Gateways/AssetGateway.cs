namespace HousingManagementSystemApi.Gateways;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Hackney.Shared.Asset.Boundary.Response;

public class AssetGateway : IAssetGateway
{
    private readonly IHttpClientFactory _httpClientFactory;

    public AssetGateway(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<AssetResponseObject> RetrieveAsset(string assetId)
    {
        Guard.Against.NullOrWhiteSpace(assetId, nameof(assetId));

        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.Asset);
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"assets/assetId/{assetId}");
        var response = await httpClient.SendAsync(request);

        var data = new AssetResponseObject();
        if (response.StatusCode == HttpStatusCode.OK)
        {
            data = await response.Content.ReadFromJsonAsync<AssetResponseObject>();
        }

        return data;
    }
}
