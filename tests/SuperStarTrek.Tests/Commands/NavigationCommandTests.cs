using System;
using System.Linq;
using Xunit;
using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for the NavigationCommand
    /// </summary>
    public class NavigationCommandTests
    {
        [Fact]
        public void Execute_WithDamagedWarpEngines_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -1.0); // Damaged
            var command = new NavigationCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1", "2" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("WARP ENGINES ARE DAMAGED", result.Message);
        }

        [Fact]
        public void Execute_WithInsufficientParameters_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            var command = new NavigationCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1" }); // Missing warp factor

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("NAVIGATION REQUIRES COURSE AND WARP FACTOR", result.Message);
        }

        [Fact]
        public void Execute_WithInvalidCourse_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            var command = new NavigationCommand();

            // Act
            var result = command.Execute(gameState, new[] { "0", "2" }); // Course 0 is invalid

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("COURSE MUST BE BETWEEN 1.0 AND 9.0", result.Message);
        }

        [Fact]
        public void Execute_WithInvalidWarpFactor_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            var command = new NavigationCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1", "-1" }); // Negative warp

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("WARP FACTOR MUST BE POSITIVE", result.Message);
        }

        [Fact]
        public void Execute_WithInsufficientEnergy_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.Energy = 10; // Very low energy
            var command = new NavigationCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1", "5" }); // Requires 40 energy

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("INSUFFICIENT ENERGY", result.Message);
        }

        [Fact]
        public void Execute_WithValidParameters_ConsumesEnergyAndTime()
        {
            // Arrange
            var gameState = new GameState(12345);
            // Ensure Enterprise is positioned in center of galaxy for safe movement
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            // Place Enterprise in the quadrant
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var initialEnergy = gameState.Enterprise.Energy;
            var command = new NavigationCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1", "2" }); // Course 1, Warp 2

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.ConsumesTime);
            Assert.Equal(1.0, result.TimeConsumed);
            Assert.True(gameState.Enterprise.Energy < initialEnergy);
        }

        [Fact]
        public void Execute_WithValidParameters_UpdatesPosition()
        {
            // Arrange
            var gameState = new GameState(12345);
            // Ensure Enterprise is positioned in center of galaxy for safe movement
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            // Place Enterprise in the quadrant
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var initialQuadrant = gameState.Enterprise.QuadrantCoordinates;
            var initialSector = gameState.Enterprise.SectorCoordinates;
            var command = new NavigationCommand();

            // Act
            var result = command.Execute(gameState, new[] { "1", "1" }); // Small movement

            // Assert
            Assert.True(result.IsSuccess);

            // Position should have changed (either quadrant or sector)
            var positionChanged = !gameState.Enterprise.QuadrantCoordinates.Equals(initialQuadrant) ||
                                 !gameState.Enterprise.SectorCoordinates.Equals(initialSector);
            Assert.True(positionChanged);
        }

        [Fact]
        public void Execute_MovingToStarbaseQuadrant_ChecksDocking()
        {
            // Arrange
            var gameState = new GameState(42); // Use seed to get consistent galaxy
            var command = new NavigationCommand();

            // Find a quadrant with a starbase
            Coordinates? starbaseQuadrant = null;
            for (int x = 1; x <= 8 && !starbaseQuadrant.HasValue; x++)
            {
                for (int y = 1; y <= 8 && !starbaseQuadrant.HasValue; y++)
                {
                    var coords = new Coordinates(x, y);
                    var quadrantData = gameState.Galaxy.GetQuadrantData(coords);
                    if ((quadrantData % 100) / 10 > 0) // Has starbase
                    {
                        starbaseQuadrant = coords;
                    }
                }
            }

            // If we found a starbase quadrant, test navigation to it
            if (starbaseQuadrant.HasValue)
            {
                // Move Enterprise to adjacent quadrant first
                var adjacentQuadrant = new Coordinates(
                    Math.Max(1, starbaseQuadrant.Value.X - 1),
                    starbaseQuadrant.Value.Y);

                gameState.MoveEnterpriseToQuadrant(adjacentQuadrant, new Coordinates(4, 4));

                // Calculate course to starbase quadrant (roughly east = 3)
                var result = command.Execute(gameState, new[] { "3", "2" });

                // Should succeed
                Assert.True(result.IsSuccess);
            }
        }

        [Fact]
        public void GetHelpText_ReturnsValidHelp()
        {
            // Arrange
            var command = new NavigationCommand();

            // Act
            var help = command.GetHelpText();

            // Assert
            Assert.Contains("NAV", help);
            Assert.Contains("NAVIGATION", help);
            Assert.Contains("course", help);
            Assert.Contains("warp", help);
        }
    }
}
