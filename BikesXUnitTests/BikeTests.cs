using BikeStore.Models;

namespace BikesXUnitTests
{
    public class BikeTests
    {
        [Fact]
        public void CheckWhereBmxRunsInHighways_ReturnFalse()
        {
            // Arrange
            Bmx bmxBike = new Bmx();

            // Assert
            Assert.Equal("Streets Only!", bmxBike.RunsAt());
        }

        [Fact]
        public void CreateAndCheckBikeDiscount_ReturnSuccess()
        {
            // Arrange
            Hardtrail hardtrailBike = new Hardtrail();
            Speed speedBike = new Speed();
            Bmx bmxBike = new Bmx();

            // Assert
            Assert.True(hardtrailBike.HasDiscount());
            Assert.True(speedBike.HasDiscount());
            Assert.False(bmxBike.HasDiscount());
        }
    }
}