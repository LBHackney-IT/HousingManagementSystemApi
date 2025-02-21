namespace HousingManagementSystemApi.Models.RepairsHub
{
    public class CautionaryAlertViewModel
    {
        /// <summary>
        /// Gets or Sets AlertCode
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Gets or Sets Description
        /// </summary>
        public string Comments { get; set; }

        /// <summary>
        /// Gets or Sets StartDate
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// Gets or Sets EndDate
        /// </summary>
        public string EndDate { get; set; }
    }
}
