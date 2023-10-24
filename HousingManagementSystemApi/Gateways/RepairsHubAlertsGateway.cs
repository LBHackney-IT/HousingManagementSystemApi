using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text.Json;
using System.Threading.Tasks;
using Amazon.Lambda.Core;
using HousingManagementSystemApi.Models.RepairsHub;

namespace HousingManagementSystemApi.Gateways
{
    public class RepairsHubAlertsGateway : IRepairsHubAlertsGateway
    {
        private readonly HttpClient _client;

        public RepairsHubAlertsGateway(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient(HttpClientNames.RepairsHubAlerts);
        }

        public async Task<AlertsViewModel> GetLocationAlerts(string propertyReference)
        {
            if (string.IsNullOrWhiteSpace(propertyReference))
                throw new ArgumentException(propertyReference, nameof(propertyReference));

            LambdaLogger.Log("Starting person alert retrieval for location " + propertyReference);

            string uri = $"properties/{propertyReference}/location-alerts";

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            LambdaLogger.Log("About to get location alerts at " + uri);

            var response = await _client.SendAsync(request);

            if (response.StatusCode != HttpStatusCode.OK)
            {
                LambdaLogger.Log($"Call to {uri} was unsuccessful with error {response.StatusCode}, reason: {response.ReasonPhrase}");
                return null;
            }

            var alertResult = await response.Content.ReadFromJsonAsync<AlertsViewModel>();

            LambdaLogger.Log($"Successfully got {alertResult.Alerts.Count} location alerts for tenancy " + propertyReference);

            return alertResult;
        }
    }
}
