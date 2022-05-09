using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HACT.Dtos;

namespace HousingManagementSystemApi.Gateways
{
    using Ardalis.GuardClauses;
    using Models;

    public class PropertiesGateway : IAddressesGateway
    {
        private readonly IHttpClientFactory httpClientFactory;

        public PropertiesGateway(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<PropertyAddress>> SearchByPostcode(string postcode)
        {
            Guard.Against.NullOrWhiteSpace(postcode, nameof(postcode));

            var httpClient = httpClientFactory.CreateClient(HttpClientNames.Properties);
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"properties?postcode={postcode}&pageSize=100");
            var response = await httpClient.SendAsync(request);

            var data = Enumerable.Empty<PropertyApiResponse>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                data = await response.Content.ReadFromJsonAsync<IEnumerable<PropertyApiResponse>>();
            }

            var result = data.Select(asset => asset.ToHactPropertyAddress());

            return result;
        }
    }
}
