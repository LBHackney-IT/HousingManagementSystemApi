namespace HousingManagementSystemApi.Tests.GatewaysTests
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net.Http;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Gateways;
    using HACT.Dtos;
    using Moq;
    using Repositories;
    using Xunit;

    public class AddressesHttpGatewayTests
    {
        private Mock<IHttpClientFactory> httpClientFactory;
        private AddressesHttpGateway systemUnderTest;

        public AddressesHttpGatewayTests()
        {
            httpClientFactory = new Mock<IHttpClientFactory>();
            systemUnderTest = new AddressesHttpGateway(httpClientFactory.Object);
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
#pragma warning disable CA1707
        public async void GivenInvalidPostcodeArgument_WhenSearchingForPostcode_ThenAnExceptionIsThrown<T>(T exception, string postcode) where T : Exception
#pragma warning restore CA1707
#pragma warning restore xUnit1026
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.SearchByPostcode(postcode);

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
#pragma warning disable CA1707
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenNoExceptionIsThrown()
#pragma warning restore CA1707
        {
            // Arrange

            // Act
            Func<Task> act = async () => await systemUnderTest.SearchByPostcode("M3 OW");

            // Assert
            await act.Should().NotThrowAsync();
        }

        [Fact]
#pragma warning disable CA1707
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenAddressesAreRetrievedFromApi()
#pragma warning restore CA1707
        {
            // Arrange
            const string postcode = "M3 OW";
            // addressesRepositoryMock.Setup(repository => repository.GetAddressesByPostcode(postcode))
                // .ReturnsAsync(new[] { new PropertyAddress() });

            // Act
            var results = await this.systemUnderTest.SearchByPostcode(postcode);

            // Assert
            Assert.True(results.Any());
        }
    }
}
