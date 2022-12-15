namespace HousingManagementSystemApi.Tests.GatewaysTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Gateways;
    using HACT.Dtos;
    using HousingManagementSystemApi.Repositories.Interfaces;
    using Moq;
    using Repositories;
    using Xunit;

    public class AddressesDatabaseGatewayTests
    {
        private readonly AddressesDatabaseGateway _systemUnderTest;
        private readonly Mock<IAddressesRepository> _addressesRepositoryMock;

        public AddressesDatabaseGatewayTests()
        {
            _addressesRepositoryMock = new Mock<IAddressesRepository>();
            _systemUnderTest = new AddressesDatabaseGateway(_addressesRepositoryMock.Object);
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenInvalidPostcodeArgument_WhenSearchingForPostcode_ThenAnExceptionIsThrown<T>(T exception, string postcode) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _systemUnderTest.SearchByPostcode(postcode);

            // Assert
            await act.Should().ThrowExactlyAsync<T>();
        }

        public static IEnumerable<object[]> InvalidArgumentTestData()
        {
            yield return new object[] { new ArgumentNullException(), null };
            yield return new object[] { new ArgumentException(), "" };
            yield return new object[] { new ArgumentException(), " " };
        }

        [Fact]
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenNoExceptionIsThrown()
        {
            // Arrange

            // Act
            Func<Task> act = async () => await _systemUnderTest.SearchByPostcode("M3 OW");

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenAddressesAreRetrievedFromTheDatabase()
        {
            // Arrange
            const string postcode = "M3 OW";
            _addressesRepositoryMock.Setup(repository => repository.GetAddressesByPostcode(postcode))
                .ReturnsAsync(new[] { new PropertyAddress() });

            // Act
            var results = await this._systemUnderTest.SearchByPostcode(postcode);

            // Assert
            Assert.True(results.Any());
        }
    }
}
