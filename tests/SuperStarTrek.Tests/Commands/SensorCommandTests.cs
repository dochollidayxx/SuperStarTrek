using Xunit;
using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for the ShortRangeSensorsCommand
    /// </summary>
    public class ShortRangeSensorsCommandTests
    {
        [Fact]
        public void Execute_WithOperationalSensors_ReturnsSuccessWithDisplay()
        {
            // Arrange
            var gameState = new GameState(12345); // Use seed for consistency
            var command = new ShortRangeSensorsCommand();

            // Act
            var result = command.Execute(gameState, new string[0]);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Message);
            Assert.Contains("SHORT RANGE SENSORS", result.Message);
            Assert.Contains("STARDATE", result.Message);
            Assert.Contains("CONDITION", result.Message);
            Assert.Contains("QUADRANT", result.Message);
            Assert.Contains("SECTOR", result.Message);
        }

        [Fact]
        public void Execute_WithDamagedSensors_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShortRangeSensors, -1.0); // Damage sensors
            var command = new ShortRangeSensorsCommand();

            // Act
            var result = command.Execute(gameState, new string[0]);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("SHORT RANGE SENSORS ARE DAMAGED", result.Message);
        }

        [Fact]
        public void GetHelpText_ReturnsCorrectText()
        {
            // Arrange
            var command = new ShortRangeSensorsCommand();

            // Act
            var helpText = command.GetHelpText();

            // Assert
            Assert.Equal("SRS - SHORT RANGE SENSORS: Display current quadrant", helpText);
        }
    }

    /// <summary>
    /// Tests for the LongRangeSensorsCommand
    /// </summary>
    public class LongRangeSensorsCommandTests
    {
        [Fact]
        public void Execute_WithOperationalSensors_ReturnsSuccessWithDisplay()
        {
            // Arrange
            var gameState = new GameState(12345); // Use seed for consistency
            var command = new LongRangeSensorsCommand();

            // Act
            var result = command.Execute(gameState, new string[0]);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.NotNull(result.Message);
            Assert.Contains("LONG RANGE SENSORS", result.Message);
            Assert.Contains("FOR QUADRANT", result.Message);
            Assert.Contains("LEGEND", result.Message);
        }

        [Fact]
        public void Execute_WithDamagedSensors_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.SetSystemDamage(ShipSystem.LongRangeSensors, -1.0); // Damage sensors
            var command = new LongRangeSensorsCommand();

            // Act
            var result = command.Execute(gameState, new string[0]);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("LONG RANGE SENSORS ARE DAMAGED", result.Message);
        }

        [Fact]
        public void GetHelpText_ReturnsCorrectText()
        {
            // Arrange
            var command = new LongRangeSensorsCommand();

            // Act
            var helpText = command.GetHelpText();

            // Assert
            Assert.Equal("LRS - LONG RANGE SENSORS: Display surrounding quadrants", helpText);
        }
    }
}
