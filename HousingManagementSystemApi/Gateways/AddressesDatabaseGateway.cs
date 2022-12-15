namespace HousingManagementSystemApi.Gateways
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using HACT.Dtos;
    using HousingManagementSystemApi.Gateways.Interfaces;
    using HousingManagementSystemApi.Repositories.Interfaces;
    using Repositories;

    public class AddressesDatabaseGateway : IAddressesGateway
    {
        private readonly IAddressesRepository _addressesRepository;

        public AddressesDatabaseGateway(IAddressesRepository addressesRepository)
        {
            _addressesRepository = addressesRepository;
        }

        public Task<IEnumerable<PropertyAddress>> SearchByPostcode(string postCode)
        {
            Guard.Against.NullOrWhiteSpace(postCode, nameof(postCode));

            return _addressesRepository.GetAddressesByPostcode(postCode);
        }
    }
}
