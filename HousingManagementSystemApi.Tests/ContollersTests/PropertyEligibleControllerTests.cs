using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Castle.Core.Logging;
using FluentAssertions;
using HACT.Dtos;
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
        private readonly PropertyEligibleController systemUnderTest;
        private readonly Mock<IVerifyPropertyEligibilityUseCase> verifyPropertyEligibilityUseCase;
        private readonly string propertyId;
        public PropertyEligibleControllerTests()
        {
            propertyId = "01234567";

            verifyPropertyEligibilityUseCase = new Mock<IVerifyPropertyEligibilityUseCase>();
            systemUnderTest = new PropertyEligibleController(verifyPropertyEligibilityUseCase.Object, new NullLogger<AddressesController>());
        }

        private void SetupValidDummyPropertyEligibilityResult()
        {
            var validDummyPropertyEligibilityResult = new PropertyEligibilityResult(true, "The property is valid");

            verifyPropertyEligibilityUseCase
                .Setup(x => x.Execute(this.propertyId))
                .ReturnsAsync(validDummyPropertyEligibilityResult);
        }

        [Fact]
        public async Task GivenAPropertyId_WhenAValidRequestIsMade_ItReturnsASuccessfullResponse()
        {
            SetupValidDummyPropertyEligibilityResult();

            var result = await systemUnderTest.VerifyPropertyEligibility(this.propertyId);
            verifyPropertyEligibilityUseCase.Verify(x => x.Execute(this.propertyId), Times.Once);
            GetStatusCode(result).Should().Be(200);
        }

        [Fact]
        public async Task GivenAnExceptionIsThrown_WhenRequestMadeToValidatePropertyEligibility_ResponseHttpStatusCodeIs500()
        {
            // Arrange
            verifyPropertyEligibilityUseCase.Setup(x => x.Execute(It.IsAny<string>()))
                .Throws<Exception>();

            // Act
            var result = await systemUnderTest.VerifyPropertyEligibility(this.propertyId);
            // Assert
            GetStatusCode(result).Should().Be(500);
        }
    }

}
