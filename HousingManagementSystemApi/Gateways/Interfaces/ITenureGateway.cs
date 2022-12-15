using System;
using System.Threading.Tasks;

namespace HousingManagementSystemApi.Gateways.Interfaces
{
    using Hackney.Shared.Tenure.Domain;

    public interface ITenureGateway
    {
        Task<TenureInformation> RetrieveTenureType(string id);
    }
}
