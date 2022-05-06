using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HACT.Dtos;
namespace HousingManagementSystemApi.Gateways
{
    using Models;

    public class AddressesHttpGateway : IAddressesGateway
    {
        private readonly IHttpClientFactory httpClientFactory;

        public AddressesHttpGateway(IHttpClientFactory httpClientFactory)
        {
            this.httpClientFactory = httpClientFactory;
        }

        public async Task<IEnumerable<PropertyAddress>> SearchByPostcode(string postcode)
        {
            var httpClient = httpClientFactory.CreateClient(HttpClientNames.HousingSearch);
            var request = new HttpRequestMessage(HttpMethod.Get,
                $"search/assets?searchText={postcode}&pageSize=100");
            var response = await httpClient.SendAsync(request);

            var data = new HousingSearchApiResponse();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                data = await response.Content.ReadFromJsonAsync<HousingSearchApiResponse>();
            }

            return Enumerable.Empty<PropertyAddress>();
        }
    }
}
