using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Json;
using System;
using System.Text.Json;
using System.Threading.Tasks;
using HousingManagementSystemApi.Models.RepairsHub;
using Amazon.Lambda.Core;

namespace HousingManagementSystemApi.Gateways
{
    public class RepairsHubAlertsGateway : IRepairsHubAlertsGateway
    {
        private readonly HttpClient _client;

        public RepairsHubAlertsGateway(IHttpClientFactory httpClientFactory)
        {
            _client = httpClientFactory.CreateClient(HttpClientNames.RepairsHubAlerts);
        }

        public async Task<AlertsViewModel> GetPersonAlerts(string tenancyAgreementReference)
        {
            if (string.IsNullOrWhiteSpace(tenancyAgreementReference)) throw new ArgumentException(tenancyAgreementReference, nameof(tenancyAgreementReference));

            LambdaLogger.Log("Starting person alert retrieval for tenancy " + tenancyAgreementReference);

            string uri = $"properties/{tenancyAgreementReference}/person-alerts";

            var request = new HttpRequestMessage(HttpMethod.Get, uri);

            LambdaLogger.Log("About to get person alerts at " + uri);

            var response = await _client.SendAsync(request);

            AlertsViewModel alertResult = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                alertResult = await response.Content.ReadFromJsonAsync<AlertsViewModel>();

                LambdaLogger.Log($"Successfully got {alertResult.Alerts.Count} person alerts for tenancy " + tenancyAgreementReference);
            }
            else
            {
                LambdaLogger.Log($"Call to {uri} was unsuccessful with error {response.StatusCode}, reason: {response.ReasonPhrase}");
            }

            return alertResult;
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

            AlertsViewModel alertResult = null;

            if (response.StatusCode == HttpStatusCode.OK)
            {
                alertResult = await response.Content.ReadFromJsonAsync<AlertsViewModel>();

                LambdaLogger.Log($"Successfully got {alertResult.Alerts.Count} location alerts for tenancy " + propertyReference);
            }
            else
            {
                LambdaLogger.Log($"Call to {uri} was unsuccessful with error {response.StatusCode}, reason: {response.ReasonPhrase}");
            }

            return alertResult;
        }
    }
}
