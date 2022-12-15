using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FluentAssertions;
using HACT.Dtos;
using HousingManagementSystemApi.Controllers;
using HousingManagementSystemApi.Models;
using HousingManagementSystemApi.UseCases.Interfaces;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HousingManagementSystemApi.Tests.ContollersTests
{
    public class PropertyEligibleControllerTests : ControllerTests
    {
        private readonly PropertyEligibleController _systemUnderTest;
        private readonly Mock<IVerifyPropertyEligibilityUseCase> _verifyPropertyEligibilityUseCase;

        private const string PropertyId = "01234567";

        public PropertyEligibleControllerTests()
        {
            _verifyPropertyEligibilityUseCase = new Mock<IVerifyPropertyEligibilityUseCase>();
            _systemUnderTest = new PropertyEligibleController(_verifyPropertyEligibilityUseCase.Object, new NullLogger<AddressesController>());
        }

        private void SetupValidDummyPropertyEligibilityResult()
        {
            var validDummyPropertyEligibilityResult = new PropertyEligibilityResult(true, "The property is valid");

            _verifyPropertyEligibilityUseCase
                .Setup(x => x.Execute(PropertyId))
                .ReturnsAsync(validDummyPropertyEligibilityResult);
        }

        [Fact]
        public async Task GivenAPropertyId_WhenAValidRequestIsMade_ItReturnsASuccessfullResponse()
        {
            // Arrange
            SetupValidDummyPropertyEligibilityResult();

            // Act
            var result = await _systemUnderTest.VerifyPropertyEligibility(PropertyId);

            // Assert
            _verifyPropertyEligibilityUseCase.Verify(x => x.Execute(PropertyId), Times.Once);
            GetStatusCode(result).Should().Be(200);
        }

        [Fact]
        public async Task GivenAnExceptionIsThrown_WhenRequestMadeToValidatePropertyEligibility_ResponseHttpStatusCodeIs500()
        {
            // Arrange
            _verifyPropertyEligibilityUseCase.Setup(x => x.Execute(It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.VerifyPropertyEligibility(PropertyId);

            // Assert
            GetStatusCode(result).Should().Be(500);
        }
    }

}
