using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace HousingManagementSystemApi.Gateways
{
    public class RepairsHubAlertsGateway : IRepairsHubAlertsGateway
    {
        private readonly HttpClient _client;

        public RepairsHubAlertsGateway(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient(HttpClientNames.RepairsHubAlerts);
        }

        //[Route("{tenancyAgreementReference}/person-alerts")]
        //[Route("{propertyReference}/location-alerts")]
    }
}
