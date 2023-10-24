using System.Collections.Generic;

namespace HousingManagementSystemApi.Models.RepairsHub
{
    public class AlertsViewModel
    {
        public string Reference { get; set; }
        public List<CautionaryAlertViewModel> Alerts { get; set; }
    }
}
