using System;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Ardalis.GuardClauses;
using Hackney.Shared.Tenure.Domain;
using HousingManagementSystemApi.Gateways.Interfaces;

namespace HousingManagementSystemApi.Gateways;

public class TenureGateway : ITenureGateway
{
    private readonly IHttpClientFactory _httpClientFactory;

    public TenureGateway(IHttpClientFactory httpClientFactory)
    {
        _httpClientFactory = httpClientFactory;
    }

    public async Task<TenureInformation> RetrieveTenureType(string id)
    {
        Guard.Against.NullOrWhiteSpace(id, nameof(id));

        var httpClient = _httpClientFactory.CreateClient(HttpClientNames.TenureInformation);
        var request = new HttpRequestMessage(HttpMethod.Get, $"tenures/{id}");
        var response = await httpClient.SendAsync(request);

        var data = new TenureInformation();
        if (response.StatusCode == HttpStatusCode.OK)
        {
            data = await response.Content.ReadFromJsonAsync<TenureInformation>();
        }

        return data;
    }
}
