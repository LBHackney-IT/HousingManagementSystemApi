using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;

namespace HousingManagementSystemApi.Gateways.Interfaces
{
    public interface IAddressesGateway
    {
        public Task<IEnumerable<PropertyAddress>> SearchByPostcode(string postCode);
    }
}
