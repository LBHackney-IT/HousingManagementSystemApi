namespace HousingManagementSystemApi.Gateways;

using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Hackney.Shared.Asset.Boundary.Response;
using HousingManagementSystemApi.Exceptions;
using Microsoft.Extensions.Logging;

public class AssetGateway : IAssetGateway
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<AssetGateway> _logger;


    public AssetGateway(IHttpClientFactory httpClientFactory, ILogger<AssetGateway> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<AssetResponseObject> RetrieveAsset(string assetId)
    {
        _logger.LogInformation("Calling Asset API to retrieve asset for assetId {AssetId}", assetId);

        Guard.Against.NullOrWhiteSpace(assetId, nameof(assetId));

        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.Asset);
        var request = new HttpRequestMessage(HttpMethod.Get, $"assets/assetId/{assetId}");
        var response = await httpClient.SendAsync(request);

        _logger.LogInformation("Received {StatusCode} from Asset API, when attempting to retrieve asset for assetId {AssetId}", response.StatusCode, assetId);

        if (response.StatusCode == HttpStatusCode.NotFound)
        {
            return null;
        }

        if (response.StatusCode != HttpStatusCode.OK)
        {
            throw new ApiException(response.StatusCode);
        }

        return await response.Content.ReadFromJsonAsync<AssetResponseObject>();
    }
}
