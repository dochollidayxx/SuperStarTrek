using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Commands
{
    public class TorpedoCommandTests
    {
        [Fact]
        public void Execute_WithNoTorpedoes_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.PhotonTorpedoes = 0;
            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ALL PHOTON TORPEDOES EXPENDED", result.Message);
        }

        [Fact]
        public void Execute_WithDamagedTubes_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.SetSystemDamage(ShipSystem.PhotonTubes, -1.0);
            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("PHOTON TUBES ARE NOT OPERATIONAL", result.Message);
        }

        [Fact]
        public void Execute_WithNoParameters_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new string[0]);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("PHOTON TORPEDO COURSE REQUIRED", result.Message);
        }

        [Fact]
        public void Execute_WithInvalidCourse_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "invalid" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("INCORRECT COURSE DATA", result.Message);
        }

        [Fact]
        public void Execute_WithCourseOutOfRange_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "10" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("INCORRECT COURSE DATA", result.Message);
        }

        [Fact]
        public void Execute_WithValidParameters_ConsumesTorpedoEnergyAndTime()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var initialTorpedoes = gameState.Enterprise.PhotonTorpedoes;
            var initialEnergy = gameState.Enterprise.Energy;
            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.ConsumesTime);
            Assert.Equal(1.0, result.TimeConsumed);
            Assert.Equal(initialTorpedoes - 1, gameState.Enterprise.PhotonTorpedoes);
            // Energy should be less than initial (exact amount depends on possible Klingon counter-attack)
            Assert.True(gameState.Enterprise.Energy < initialEnergy);
            Assert.Contains("TORPEDO TRACK", result.Message);
        }

        [Fact]
        public void Execute_CourseNine_ConvertedToOne()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "9" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("TORPEDO TRACK", result.Message);
        }

        [Fact]
        public void Execute_TorpedoMisses_ShowsMissMessage()
        {
            // Arrange
            var gameState = new GameState(12345);
            // Place Enterprise near edge so torpedo will miss
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(8, 8);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act - Fire torpedo east (course 3) from edge position
            var result = command.Execute(gameState, new[] { "3" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("TORPEDO MISSED", result.Message);
        }

        [Fact]
        public void GetHelpText_ReturnsValidHelpText()
        {
            // Arrange
            var command = new TorpedoCommand();

            // Act
            var helpText = command.GetHelpText();

            // Assert
            Assert.Contains("TOR", helpText);
            Assert.Contains("PHOTON TORPEDOES", helpText);
            Assert.Contains("course", helpText);
        }
    }
}
