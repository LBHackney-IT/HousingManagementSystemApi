namespace HousingManagementSystemApi.Gateways
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Ardalis.GuardClauses;
    using HACT.Dtos;
    using Repositories;

    public class AddressesDatabaseGateway : IAddressesGateway
    {
        private readonly IAddressesRepository _addressesRepository;

        public AddressesDatabaseGateway(IAddressesRepository addressesRepository)
        {
            _addressesRepository = addressesRepository;
        }

        public Task<IEnumerable<PropertyAddress>> SearchByPostcode(string postcode)
        {
            Guard.Against.NullOrWhiteSpace(postcode, nameof(postcode));

            return _addressesRepository.GetAddressesByPostcode(postcode);
        }
    }
}
