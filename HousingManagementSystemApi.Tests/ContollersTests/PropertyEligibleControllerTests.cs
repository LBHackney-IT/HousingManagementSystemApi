using System;
using System.Threading.Tasks;
using FluentAssertions;
using HousingManagementSystemApi.Controllers;
using HousingManagementSystemApi.Models;
using HousingManagementSystemApi.UseCases;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Xunit;

namespace HousingManagementSystemApi.Tests.ContollersTests
{

    public class PropertyEligibleControllerTests : ControllerTests
    {
        private readonly PropertyEligibleController _systemUnderTest;
        private readonly Mock<IVerifyPropertyEligibilityUseCase> _verifyPropertyEligibilityUseCase;
        private readonly string _propertyId;
        public PropertyEligibleControllerTests()
        {
            _propertyId = "01234567";

            _verifyPropertyEligibilityUseCase = new Mock<IVerifyPropertyEligibilityUseCase>();
            _systemUnderTest = new PropertyEligibleController(_verifyPropertyEligibilityUseCase.Object, new NullLogger<AddressesController>());
        }

        private void SetupValidDummyPropertyEligibilityResult()
        {
            var validDummyPropertyEligibilityResult = new PropertyEligibilityResult(true, "The property is valid");

            _verifyPropertyEligibilityUseCase
                .Setup(x => x.Execute(_propertyId))
                .ReturnsAsync(validDummyPropertyEligibilityResult);
        }

        [Fact]
        public async Task GivenAPropertyId_WhenAValidRequestIsMade_ItReturnsASuccessfullResponse()
        {
            SetupValidDummyPropertyEligibilityResult();

            var result = await _systemUnderTest.VerifyPropertyEligibility(_propertyId);
            _verifyPropertyEligibilityUseCase.Verify(x => x.Execute(_propertyId), Times.Once);
            GetStatusCode(result).Should().Be(200);
        }

        [Fact]
        public async Task GivenAnExceptionIsThrown_WhenRequestMadeToValidatePropertyEligibility_ResponseHttpStatusCodeIs500()
        {
            // Arrange
            _verifyPropertyEligibilityUseCase.Setup(x => x.Execute(It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var result = await _systemUnderTest.VerifyPropertyEligibility(_propertyId);
            // Assert
            GetStatusCode(result).Should().Be(500);
        }
    }

}
