namespace HousingManagementSystemApi.Models;

public class HousingSearchApiResponse
{
    public long Total { get; set; }

    public HousingSearchApiResponseResults Results { get; set; }
}
