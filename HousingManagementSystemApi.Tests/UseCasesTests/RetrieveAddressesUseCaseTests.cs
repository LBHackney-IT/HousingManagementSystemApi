using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HACT.Dtos;
using HousingManagementSystemApi.UseCases;
using Moq;
using Xunit;
using HousingManagementSystemApi.Gateways.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;

namespace HousingManagementSystemApi.Tests
{
    public class RetrieveAddressesUseCaseTests
    {
        private readonly Mock<IAddressesGateway> _retrieveAddressesGateway;
        private readonly RetrieveAddressesUseCase _retrieveAddressesUseCase;

        private const string PostCode = "postCode";

        public RetrieveAddressesUseCaseTests()
        {
            _retrieveAddressesGateway = new Mock<IAddressesGateway>();
            _retrieveAddressesUseCase = new RetrieveAddressesUseCase(
                _retrieveAddressesGateway.Object,
                new NullLogger<RetrieveAddressesUseCase>());
        }

        [Fact]
        public async Task GivenAPostcode_WhenExecute_GatewayReceivesCorrectInput()
        {
            // Arrange
            _retrieveAddressesGateway.Setup(x => x.SearchByPostcode(PostCode));

            // Act
            await _retrieveAddressesUseCase.Execute(PostCode);

            // Assert
            _retrieveAddressesGateway.Verify(x => x.SearchByPostcode(PostCode), Times.Once);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAnAddressExistsWithAnEligibleAssetAndTenureType_ThenThePropertyIsReturned()
        {
            // Arrange
            _retrieveAddressesGateway.Setup(x => x.SearchByPostcode(PostCode))
                .ReturnsAsync(new PropertyAddress[] { new() { PostalCode = PostCode, Reference = new Reference { ID = "assetId" } } });

            // Act
            var result = await _retrieveAddressesUseCase.Execute(PostCode);

            // Assert
            result.First().PostalCode.Should().Be(PostCode);
        }

        [Fact]
        public async void GivenNullPostcode_WhenExecute_ThrowsNullException()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _retrieveAddressesUseCase.Execute(null);

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }

        [Fact]
        public async void GivenEmptyPostcode_WhenExecute_ThrowsNullException()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _retrieveAddressesUseCase.Execute(postCode: "");

            // Assert
            await act.Should().ThrowAsync<ArgumentNullException>();
        }
    }
}
