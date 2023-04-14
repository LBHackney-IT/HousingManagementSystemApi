using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using HACT.Dtos;
using HousingManagementSystemApi.Controllers;
using HousingManagementSystemApi.UseCases;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HousingManagementSystemApi.Tests.ContollersTests
{

    public class AddressControllerTests : ControllerTests
    {
        private readonly AddressesController _systemUnderTest;
        private readonly Mock<IRetrieveAddressesUseCase> _retrieveAddressesUseCaseMock;
        private readonly string _postcode;
        public AddressControllerTests()
        {
            _postcode = "postcode";

            _retrieveAddressesUseCaseMock = new Mock<IRetrieveAddressesUseCase>();
            _systemUnderTest = new AddressesController(_retrieveAddressesUseCaseMock.Object, new NullLogger<AddressesController>());
        }

        private void SetupDummyAddresses()
        {
            var dummyList = new List<PropertyAddress> { new() { PostalCode = _postcode } };
            _retrieveAddressesUseCaseMock
                .Setup(x => x.Execute(_postcode))
                .ReturnsAsync(dummyList);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAValidAddressRequestIsMade_ItReturnsASuccessfullResponse()
        {
            SetupDummyAddresses();

            var result = await _systemUnderTest.Address(_postcode);
            _retrieveAddressesUseCaseMock.Verify(x => x.Execute(_postcode), Times.Once);
            GetStatusCode(result).Should().Be(200);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAValidAddressRequestIsMade_ItReturnsCorrectData()
        {
            SetupDummyAddresses();

            var result = await _systemUnderTest.Address(_postcode);
            GetResultData<List<PropertyAddress>>(result).First().PostalCode.Should().Be(_postcode);
        }

        [Fact]
        public async Task GivenAnExceptionIsThrown_WhenRequestMadeForAddresses_ResponseHttpStatusCodeIs500()
        {
            // Arrange
            _retrieveAddressesUseCaseMock.Setup(x => x.Execute(It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Address(_postcode);
            // Assert
            GetStatusCode(result).Should().Be(500);
        }

        [Fact]
        public async Task GivenAnExceptionIsThrown_WhenRequestMadeForAddresses_ResponseHttpStatusMessageIsExceptionMessage()
        {
            // Arrange
            const string errorMessage = "An error message";
            _retrieveAddressesUseCaseMock.Setup(x => x.Execute(It.IsAny<string>()))
                .Throws(new InvalidOperationException(errorMessage));

            // Act
            var result = await _systemUnderTest.Address(_postcode);
            // Assert
            GetResultData<string>(result).Should().Be(errorMessage);
        }

    }

}
