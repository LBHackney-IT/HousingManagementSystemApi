using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HACT.Dtos;
namespace HousingManagementSystemApi.Gateways
{
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

            // var data = new List<PropertyAddress>
            // {
            //    new PropertyAddress()
            //    {
            //        PostalCode = "E8 3JD",
            //        Reference = new Reference()
            //        {
            //            ID = "00075109",
            //            AllocatedBy = "X",
            //            Description = "Y"
            //        },
            //        AddressLine = new List<string>
            //        {
            //            "1-19 355 Queensbridge Road"
            //        },
            //        CityName = "London"
            //
            //    }
            // };
            var data = Enumerable.Empty<PropertyAddress>();
            if (response.StatusCode == HttpStatusCode.OK)
            {
                data = await response.Content.ReadFromJsonAsync<List<PropertyAddress>>();
            }

            return data;
        }
    }
}
