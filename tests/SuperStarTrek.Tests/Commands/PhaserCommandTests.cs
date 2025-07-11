using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Commands
{
    public class PhaserCommandTests
    {
        [Fact]
        public void Execute_WithDamagedPhasers_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.SetSystemDamage(ShipSystem.PhaserControl, -1.0);
            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "100" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("PHASERS INOPERATIVE", result.Message);
        }

        [Fact]
        public void Execute_WithNoKlingons_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            // Ensure Enterprise is positioned safely
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            // Ensure no Klingons in the quadrant
            gameState.CurrentQuadrant.KlingonShips.Clear();

            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "100" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("SENSORS SHOW NO ENEMY SHIPS", result.Message);
        }

        [Fact]
        public void Execute_WithNoParameters_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new string[0]);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("PHASERS LOCKED ON TARGET", result.Message);
        }

        [Fact]
        public void Execute_WithInvalidEnergyAmount_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "invalid" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("INVALID ENERGY AMOUNT", result.Message);
        }

        [Fact]
        public void Execute_WithInsufficientEnergy_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.Energy = 50;
            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "100" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("INSUFFICIENT ENERGY", result.Message);
        }

        [Fact]
        public void Execute_WithValidParameters_ConsumesEnergyAndTime()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            // Add a Klingon to fight
            var klingon = new KlingonShip(new Coordinates(5, 5), 100);
            gameState.CurrentQuadrant.PlaceKlingon(klingon);

            var initialEnergy = gameState.Enterprise.Energy;
            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "50" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.ConsumesTime);
            Assert.Equal(1.0, result.TimeConsumed);
            // Energy should be less than initial (exact amount depends on Klingon counter-attack)
            Assert.True(gameState.Enterprise.Energy < initialEnergy);
        }

        [Fact]
        public void Execute_HitsKlingon_DamagesOrDestroysKlingon()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            // Add a weak Klingon that should be destroyed
            var klingon = new KlingonShip(new Coordinates(5, 5), 10);
            gameState.CurrentQuadrant.PlaceKlingon(klingon);

            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "200" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("UNIT HIT ON KLINGON", result.Message);
        }

        [Fact]
        public void GetHelpText_ReturnsValidHelpText()
        {
            // Arrange
            var command = new PhaserCommand();

            // Act
            var helpText = command.GetHelpText();

            // Assert
            Assert.Contains("PHA", helpText);
            Assert.Contains("PHASERS", helpText);
            Assert.Contains("energy", helpText);
        }
    }
}
