namespace HousingManagementSystemApi.Models;

using System.Collections.Generic;
using Hackney.Shared.Asset.Domain;

public class HousingSearchApiResponseResults
{
    public IEnumerable<Asset> Assets { get; set; }
}
