using System;
using System.Threading.Tasks;

namespace HousingManagementSystemApi.Gateways
{
    using Hackney.Shared.Tenure.Domain;

    public interface ITenureGateway
    {
        Task<TenureInformation> RetrieveTenureType(string id);
    }
}
