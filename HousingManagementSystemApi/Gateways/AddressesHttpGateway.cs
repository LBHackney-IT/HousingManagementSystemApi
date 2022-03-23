using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;
using HACT.Dtos;
namespace HousingManagementSystemApi.Gateways
{
    public class AddressesHttpGateway : IAddressesGateway
    {
        private readonly HttpClient httpClient;
        private readonly string addressesApiUrl;
        private readonly string addressesApiKey;

        public AddressesHttpGateway(HttpClient httpClient, string addressesApiUrl, string addressesApiKey)
        {
            this.httpClient = httpClient;
            this.addressesApiUrl = addressesApiUrl;
            this.addressesApiKey = addressesApiKey;
        }

        public async Task<IEnumerable<PropertyAddress>> SearchByPostcode(string postcode)
        {
            // var request = new HttpRequestMessage(HttpMethod.Get,
            //     $"{addressesApiUrl}/address?postcode={postcode}");
            // request.Headers.Add("X-API-Key", addressesApiKey);
            // var response = await httpClient.SendAsync(request);
            //
            var data = new List<PropertyAddress>
            {
               new PropertyAddress()
               {
                   PostalCode = "LN1 3AT",
                   StreetName = "Diagon Alley",
                   Country = CountryCode.AD,
                   Reference = new Reference()
                   {
                       AllocatedBy = "X",
                       Description = "Y"
                   },
                   AddressLine = new List<string>
                   {
                       "AddressLine"
                   },
                   CityName = "London"

               }
            };

            // if (response.StatusCode == HttpStatusCode.OK)
            // {
                // data = await response.Content.ReadFromJsonAsync<List<PropertyAddress>>();
            // }

            return data;
        }
    }
}
