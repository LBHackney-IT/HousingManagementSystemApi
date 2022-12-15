using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FluentAssertions;
using HACT.Dtos;
using HousingManagementSystemApi.Controllers;
using HousingManagementSystemApi.UseCases.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HousingManagementSystemApi.Tests.ContollersTests
{
    public class AddressControllerTests : ControllerTests
    {
        private readonly AddressesController _systemUnderTest;
        private readonly Mock<IRetrieveAddressesUseCase> _retrieveAddressesUseCaseMock;

        private const string PostCode = "postcode";

        public AddressControllerTests()
        {
            _retrieveAddressesUseCaseMock = new Mock<IRetrieveAddressesUseCase>();
            _systemUnderTest = new AddressesController(_retrieveAddressesUseCaseMock.Object, new NullLogger<AddressesController>());
        }

        private void SetupDummyAddresses()
        {
            var dummyList = new List<PropertyAddress> { new() { PostalCode = PostCode } };
            _retrieveAddressesUseCaseMock
                .Setup(x => x.Execute(PostCode))
                .ReturnsAsync(dummyList);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAValidAddressRequestIsMade_ItReturnsASuccessfullResponse()
        {
            // Arrange
            SetupDummyAddresses();

            // Act
            var result = await _systemUnderTest.Address(PostCode);

            // Assert
            _retrieveAddressesUseCaseMock.Verify(x => x.Execute(PostCode), Times.Once);
            GetStatusCode(result).Should().Be(200);
        }

        [Fact]
        public async Task GivenAPostcode_WhenAValidAddressRequestIsMade_ItReturnsCorrectData()
        {
            // Arrange
            SetupDummyAddresses();

            // Act
            var result = await _systemUnderTest.Address(PostCode);

            // Assert
            GetResultData<List<PropertyAddress>>(result).First().PostalCode.Should().Be(PostCode);
        }

        [Fact]
        public async Task GivenAnExceptionIsThrown_WhenRequestMadeForAddresses_ResponseHttpStatusCodeIs500()
        {
            // Arrange
            _retrieveAddressesUseCaseMock.Setup(x => x.Execute(It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.Address(PostCode);

            // Assert
            GetStatusCode(result).Should().Be(500);
        }

        [Fact]
        public async Task GivenAnExceptionIsThrown_WhenRequestMadeForAddresses_ResponseHttpStatusMessageIsExceptionMessage()
        {
            // Arrange
            const string errorMessage = "An error message";
            _retrieveAddressesUseCaseMock.Setup(x => x.Execute(It.IsAny<string>()))
                .Throws(new Exception(errorMessage));

            // Act
            var result = await _systemUnderTest.Address(PostCode);

            // Assert
            GetResultData<string>(result).Should().Be(errorMessage);
        }

    }

}
