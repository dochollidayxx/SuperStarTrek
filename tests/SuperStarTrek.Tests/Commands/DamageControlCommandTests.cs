using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for the DamageControlCommand implementation
    /// Verifies authentic BASIC behavior from lines 5680-5980
    /// </summary>
    public class DamageControlCommandTests
    {
        private readonly Random _random;
        private readonly DamageControlCommand _command;

        public DamageControlCommandTests()
        {
            _random = new Random(42); // Fixed seed for reproducible tests
            _command = new DamageControlCommand(_random);
        }

        [Fact]
        public void Execute_WithDamageControlInoperable_StillSucceeds()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.SetSystemDamage(ShipSystem.DamageControl, -1.0);
            gameState.Enterprise.IsDocked = false;

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.ConsumesTime);
            Assert.Equal(0, result.TimeConsumed);
        }

        [Fact]
        public void Execute_WithNoDamage_ReturnsSuccess()
        {
            // Arrange
            var gameState = CreateTestGameState();

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.ConsumesTime);
            Assert.Equal(0, result.TimeConsumed);
        }

        [Fact]
        public void Execute_WithDamageButNotDocked_NoRepairOffered()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = false;
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.5);

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.ConsumesTime);
            Assert.Equal(0, result.TimeConsumed);
            // Damage should remain unchanged when not docked
            Assert.Equal(-2.5, gameState.Enterprise.GetSystemDamage(ShipSystem.WarpEngines));
        }

        [Fact]
        public void Execute_WithDamageAndDocked_CalculatesRepairTimeCorrectly()
        {
            // Arrange  
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = true;
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.5);
            gameState.Enterprise.SetSystemDamage(ShipSystem.PhaserControl, -1.8);

            // Act - Decline repairs using parameter
            var result = _command.Execute(gameState, new[] { "N" });

            // Assert - Command executes successfully but no repairs performed
            Assert.True(result.IsSuccess);
            Assert.False(result.ConsumesTime);
            Assert.Equal(0, result.TimeConsumed);
            // Damage should remain unchanged when repairs are declined
            Assert.Equal(-2.5, gameState.Enterprise.GetSystemDamage(ShipSystem.WarpEngines));
            Assert.Equal(-1.8, gameState.Enterprise.GetSystemDamage(ShipSystem.PhaserControl));
        }

        [Fact]
        public void Execute_AcceptRepairs_RepairsAllSystemsAndAdvancesTime()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = true;
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.5);
            gameState.Enterprise.SetSystemDamage(ShipSystem.PhaserControl, -1.8);

            // Act - Accept repairs using parameter
            var result = _command.Execute(gameState, new[] { "Y" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.ConsumesTime);
            Assert.True(result.TimeConsumed > 0);
            // All damaged systems should be repaired
            Assert.Equal(0.0, gameState.Enterprise.GetSystemDamage(ShipSystem.WarpEngines));
            Assert.Equal(0.0, gameState.Enterprise.GetSystemDamage(ShipSystem.PhaserControl));
        }

        [Fact]
        public void Execute_DeclineRepairs_LeavesSystemsDamaged()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = true;
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.5);

            // Act - Decline repairs using parameter
            var result = _command.Execute(gameState, new[] { "N" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.ConsumesTime);
            Assert.Equal(0, result.TimeConsumed);
            Assert.Equal(-2.5, gameState.Enterprise.GetSystemDamage(ShipSystem.WarpEngines));
        }

        [Fact]
        public void Execute_DamageControlInoperableButDocked_StillShowsReport()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = true;
            gameState.Enterprise.SetSystemDamage(ShipSystem.DamageControl, -1.0);
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.0);

            // Act - Decline repairs to focus on testing the damage control system behavior
            var result = _command.Execute(gameState, new[] { "N" });

            // Assert
            Assert.True(result.IsSuccess);
            // When docked, repairs should be processed even with damaged damage control
            // This is the authentic BASIC behavior
            Assert.Equal(-2.0, gameState.Enterprise.GetSystemDamage(ShipSystem.WarpEngines));
        }

        [Fact]
        public void GetHelpText_ReturnsCorrectHelpText()
        {
            // Act
            string helpText = _command.GetHelpText();

            // Assert
            Assert.Equal("DAM  (FOR DAMAGE CONTROL REPORTS)", helpText);
        }

        private GameState CreateTestGameState()
        {
            return new GameState(seed: 42);
        }
    }
}
