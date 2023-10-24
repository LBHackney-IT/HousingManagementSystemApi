using System.Threading.Tasks;
using HousingManagementSystemApi.Models.RepairsHub;

namespace HousingManagementSystemApi.Gateways
{
    public interface IRepairsHubAlertsGateway
    {
        public Task<AlertsViewModel> GetLocationAlerts(string propertyReference);
    }
}
