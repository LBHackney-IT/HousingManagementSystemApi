using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HACT.Dtos;
using HousingManagementSystemApi.Gateways;
using HousingManagementSystemApi.UseCases;
using Moq;
using Xunit;

namespace HousingManagementSystemApi.Tests
{
    using System.Collections.Generic;
    using Castle.Core.Logging;
    using Hackney.Shared.Asset.Boundary.Response;
    using Hackney.Shared.Asset.Domain;
    using Hackney.Shared.Tenure.Domain;
    using Microsoft.Extensions.Logging.Abstractions;

    public class RetrieveAddressesUseCaseTests
    {
        private readonly Mock<IAddressesGateway> retrieveAddressesGateway;

        private readonly RetrieveAddressesUseCase retrieveAddressesUseCase;

        public RetrieveAddressesUseCaseTests()
        {
            retrieveAddressesGateway = new Mock<IAddressesGateway>();
            retrieveAddressesUseCase = new RetrieveAddressesUseCase(retrieveAddressesGateway.Object,
                new NullLogger<RetrieveAddressesUseCase>());
        }

        [Fact]
        public async Task GivenAPostcode_WhenExecute_GatewayReceivesCorrectInput()
        {
            const string TestPostcode = "postcode";
            retrieveAddressesGateway.Setup(x => x.SearchByPostcode(TestPostcode));
            await retrieveAddressesUseCase.Execute(TestPostcode);
            retrieveAddressesGateway.Verify(x => x.SearchByPostcode(TestPostcode), Times.Once);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAnAddressExistsWithAnEligibleAssetAndTenureType_ThenThePropertyIsReturned()
        {
            const string TestPostcode = "postcode";
            retrieveAddressesGateway.Setup(x => x.SearchByPostcode(TestPostcode))
                .ReturnsAsync(new PropertyAddress[] { new() { PostalCode = TestPostcode, Reference = new Reference { ID = "assetId" } } });

            var result = await retrieveAddressesUseCase.Execute(TestPostcode);
            result.First().PostalCode.Should().Be(TestPostcode);
        }

        [Fact]
        public async void GivenNullPostcode_WhenExecute_ThrowsNullException()
        {
            Func<Task> act = async () => await retrieveAddressesUseCase.Execute(null);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenEmptyPostcode_WhenExecute_ThrowsNullException()
        {
            const string TestPostcode = "";

            Func<Task> act = async () => await retrieveAddressesUseCase.Execute(postCode: TestPostcode);
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
