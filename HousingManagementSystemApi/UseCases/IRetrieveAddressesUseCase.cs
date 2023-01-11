using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;
namespace HousingManagementSystemApi.UseCases
{
    public interface IRetrieveAddressesUseCase
    {
        public Task<IEnumerable<PropertyAddress>> Execute(string postCode);
    }
}
