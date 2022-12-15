namespace HousingManagementSystemApi.Tests.RespositoriesTests
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Diagnostics.CodeAnalysis;
    using System.Linq;
    using System.Threading.Tasks;
    using Dapper;
    using FluentAssertions;
    using HACT.Dtos;
    using Models;
    using Moq;
    using Moq.Dapper;
    using Repositories;
    using Xunit;

    public class UniversalHousingAddressesRepositoryTests
    {
        private readonly Mock<IDbConnection> _dbConnectionMock;

        private const string PostCode = "M3 OW";

        public UniversalHousingAddressesRepositoryTests()
        {
            _dbConnectionMock = new Mock<IDbConnection>();

        }


        [Fact]
        [SuppressMessage("ReSharper", "ObjectCreationAsStatement")]
        public void GivenNullArgument_WhenConstructing_ThenArgumentExceptionIsThrown()
        {
            // Arrange

            // Act

            Action act = () => new UniversalHousingAddressesRepository(null!);

            // Assert
            act.Should().Throw<ArgumentNullException>();
        }

        [Theory]
        [MemberData(nameof(InvalidArgumentTestData))]
#pragma warning disable xUnit1026
        public async void GivenInvalidPostcodeArgument_WhenSearchingForPostcode_ThenAnExceptionIsThrown<T>(T exception, string postcode) where T : Exception
#pragma warning restore xUnit1026
        {
            // Arrange
            var connectionFactory = new Func<IDbConnection>(() => new Mock<IDbConnection>().Object);
            var systemUnderTest = new UniversalHousingAddressesRepository(connectionFactory);

            // Act
            Func<Task> act = async () => await systemUnderTest.GetAddressesByPostcode(postcode);

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
            var universalHousingAddresses = new[] { new UniversalHousingAddress { PostCode = PostCode } };

            _dbConnectionMock.SetupDapperAsync(c =>
                c.QueryAsync<UniversalHousingAddress>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(universalHousingAddresses);

            var connectionFactory = new Func<IDbConnection>(() => _dbConnectionMock.Object);
            var systemUnderTest = new UniversalHousingAddressesRepository(connectionFactory);

            // Act
            Func<Task> act = async () => await systemUnderTest.GetAddressesByPostcode(PostCode);

            // Assert
            await act.Should().NotThrowAsync();
        }


        [Fact]
        public async void GivenValidPostcodeArgument_WhenSearchingForPostcode_ThenExpectedDataIsReturned()
        {
            // Arrange
            var universalHousingAddress = new UniversalHousingAddress { PostCode = PostCode };
            var universalHousingAddresses = new[] { universalHousingAddress };

            _dbConnectionMock.SetupDapperAsync(c =>
            c.QueryAsync<UniversalHousingAddress>(It.IsAny<string>(), null, null, null, null))
                .ReturnsAsync(universalHousingAddresses);

            var connectionFactory = new Func<IDbConnection>(() => _dbConnectionMock.Object);
            var systemUnderTest = new UniversalHousingAddressesRepository(connectionFactory);

            // Act
            var actual = await systemUnderTest.GetAddressesByPostcode(PostCode);

            // Assert
            actual.Should().HaveCount(1);
            actual.Single().Should().BeEquivalentTo(new PropertyAddress { PostalCode = PostCode });
        }
    }
}
