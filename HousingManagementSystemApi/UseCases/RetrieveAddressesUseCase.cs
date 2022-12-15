using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HACT.Dtos;

namespace HousingManagementSystemApi.UseCases
{
    using System.Linq;
    using System.Reflection.Metadata.Ecma335;
    using Amazon.Runtime.Internal.Util;
    using Hackney.Shared.Asset.Domain;
    using Hackney.Shared.Tenure.Domain;
    using HousingManagementSystemApi.Gateways.Interfaces;
    using HousingManagementSystemApi.UseCases.Interfaces;
    using Microsoft.Extensions.Logging;

    public class RetrieveAddressesUseCase : IRetrieveAddressesUseCase
    {
        private readonly IAddressesGateway _addressesGateway;
        private readonly ILogger<RetrieveAddressesUseCase> _logger;

        public RetrieveAddressesUseCase(
            IAddressesGateway addressesGateway,
            ILogger<RetrieveAddressesUseCase> logger)
        {
            _addressesGateway = addressesGateway;
            _logger = logger;
        }

        public async Task<IEnumerable<PropertyAddress>> Execute(string postCode)
        {
            _logger.LogInformation("Calling RetrieveAddressesUseCase for {PostCode}", postCode);

            if (string.IsNullOrWhiteSpace(postCode))
            {
                _logger.LogInformation("The value of {PostCode} was null or whitespace. Throwing Exception", postCode);
                throw new ArgumentNullException(nameof(postCode));
            }
            var result = await _addressesGateway.SearchByPostcode(postCode);

            _logger.LogInformation("Retrieved {Count} results from addressGateway for {PostCode}", result.Count(), postCode);

            return result;
        }
    }
}
