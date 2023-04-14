namespace HousingManagementSystemApi.Gateways;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Hackney.Shared.Asset.Boundary.Response;
using Microsoft.Extensions.Logging;

public class AssetGateway : IAssetGateway
{
    private readonly IHttpClientFactory httpClientFactory;
    private readonly ILogger<AssetGateway> logger;


    public AssetGateway(IHttpClientFactory httpClientFactory, ILogger<AssetGateway> logger)
    {
        this.httpClientFactory = httpClientFactory;
        this.logger = logger;
    }

    public async Task<AssetResponseObject> RetrieveAsset(string assetId)
    {
        this.logger.LogInformation("Calling Asset API to retrieve asset for assetId {AssetId}", assetId);

        Guard.Against.NullOrWhiteSpace(assetId, nameof(assetId));

        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.Asset);
        var request = new HttpRequestMessage(HttpMethod.Get,
            $"assets/assetId/{assetId}");
        var response = await httpClient.SendAsync(request);

        var data = new AssetResponseObject();
        if (response.StatusCode == HttpStatusCode.OK)
        {
            this.logger.LogInformation("Received status code 200 from Asset API, when attempting to retrieve asset for assetId {AssetId}", assetId);
            data = await response.Content.ReadFromJsonAsync<AssetResponseObject>();
        }

        return data;
    }
}
