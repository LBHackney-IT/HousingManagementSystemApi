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

    public class HousingSearchGateway : IAddressesGateway
    {
        private readonly IHttpClientFactory httpClientFactory;

        public HousingSearchGateway(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<PropertyAddress>> SearchByPostcode(string postcode)
        {
            Guard.Against.NullOrWhiteSpace(postcode, nameof(postcode));

            var httpClient = httpClientFactory.CreateClient(HttpClientNames.HousingSearch);
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"search/assets?searchText={postcode}&useCustomSorting=true");
            var response = await httpClient.SendAsync(request);

            var data = new HousingSearchApiResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                data = await response.Content.ReadFromJsonAsync<HousingSearchApiResponse>();
            }

            var result = data.Results.Assets.Select(asset => asset.ToHactPropertyAddress());

            return result;
        }
    }
}
