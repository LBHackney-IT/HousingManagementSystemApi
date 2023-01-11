namespace HousingManagementSystemApi.Tests.ModelsTests
{
    using FluentAssertions;
    using HACT.Dtos;
    using Models;
    using Xunit;

    public class UniversalHousingAddressExtensionsTests
    {
        [Fact]
        public void GivenEmptyUniversalHousingAddress_WhenConvertingToHactPropertyAddress_ThenPropertyAddressShouldBeEmpty()
        {
            // Arrange

            // Act
            var actual = new UniversalHousingAddress().ToHactPropertyAddress();

            // Assert
            actual.Should().BeEquivalentTo(new PropertyAddress());
        }

        [Fact]
        public void GivenUniversalHousingAddressWithPostCodeOnly_WhenConvertingToHactPropertyAddress_ThenPropertyAddressShouldOnlyContainPostalCode()
        {
            // Arrange
            var postCode = "a postcode";

            // Act
            var actual = new UniversalHousingAddress { PostCode = postCode }.ToHactPropertyAddress();

            // Assert
            actual.Should().BeEquivalentTo(new PropertyAddress { PostalCode = postCode });
        }

        [Fact]
        public void GivenUniversalHousingAddressWithAddressOnly_WhenConvertingToHactPropertyAddress_ThenPropertyAddressShouldOnlyContainAddress()
        {
            // Arrange
            var address = "an address";

            // Act
            var actual = new UniversalHousingAddress { ShortAddress = address }.ToHactPropertyAddress();

            // Assert
            actual.Should().BeEquivalentTo(new PropertyAddress { AddressLine = new[] { address } });
        }

        [Fact]
        public void GivenUniversalHousingAddressWithPostDesigOnly_WhenConvertingToHactPropertyAddress_ThenPropertyAddressShouldOnlyContainBuildingNumber()
        {
            // Arrange
            var doorNumber = "a door number";

            // Act
            var actual = new UniversalHousingAddress { PostDesig = doorNumber }.ToHactPropertyAddress();

            // Assert
            actual.Should().BeEquivalentTo(new PropertyAddress { BuildingNumber = doorNumber });
        }
    }
}
