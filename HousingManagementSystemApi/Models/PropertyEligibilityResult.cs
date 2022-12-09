namespace HousingManagementSystemApi.Models
{
    public class PropertyEligibilityResult
    {
        public PropertyEligibilityResult(bool propertyEligible, string reason)
        {
            this.PropertyEligible = propertyEligible;
            this.Reason = reason;
        }

        public bool PropertyEligible { get; set; }
        public string Reason { get; set; }
    }
}
