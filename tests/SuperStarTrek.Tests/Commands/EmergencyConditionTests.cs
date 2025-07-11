using SuperStarTrek.Game;
using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for the emergency condition detection logic from original BASIC lines 2020-2050
    /// </summary>
    public class EmergencyConditionTests
    {
        [Fact]
        public void CheckForEmergencyConditions_WhenShipHasAdequateEnergy_ShouldNotBeStranded()
        {
            // Arrange
            var game = new StarTrekGame();
            var gameState = new GameState(42); // Create GameState directly

            // Set conditions where ship should NOT be stranded
            gameState.Enterprise.Energy = 50;
            gameState.Enterprise.Shields = 50;
            // Shield control is not damaged (default is 0.0)

            // Use reflection to set the private _gameState field
            var gameStateField = typeof(StarTrekGame).GetField("_gameState",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            gameStateField!.SetValue(game, gameState);

            // Act - Use reflection to access private method for testing
            var checkMethod = typeof(StarTrekGame).GetMethod("CheckForEmergencyConditions",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isStranded = (bool)checkMethod!.Invoke(game, null)!;

            // Assert
            Assert.False(isStranded);
            Assert.False(gameState.IsShipStranded);
        }

        [Fact]
        public void CheckForEmergencyConditions_WhenShipHasLowEnergyButShieldsWork_ShouldNotBeStranded()
        {
            // Arrange
            var game = new StarTrekGame();
            var gameState = new GameState(42); // Create GameState directly

            // Set conditions: low total energy but shield control works
            gameState.Enterprise.Energy = 5;
            gameState.Enterprise.Shields = 5;
            // Shield control is not damaged (default is 0.0)

            // Use reflection to set the private _gameState field
            var gameStateField = typeof(StarTrekGame).GetField("_gameState",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            gameStateField!.SetValue(game, gameState);

            // Act - Use reflection to access private method for testing
            var checkMethod = typeof(StarTrekGame).GetMethod("CheckForEmergencyConditions",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isStranded = (bool)checkMethod!.Invoke(game, null)!;

            // Assert
            Assert.False(isStranded);
            Assert.False(gameState.IsShipStranded);
        }

        [Fact]
        public void CheckForEmergencyConditions_WhenShipHasLowEnergyAndDamagedShields_ShouldBeStranded()
        {
            // Arrange
            var game = new StarTrekGame();
            var gameState = new GameState(42); // Create GameState directly

            // Set fatal error conditions: 
            // - Total energy (shields + energy) <= 10
            // - Energy <= 10 
            // - Shield control is damaged (negative value)
            gameState.Enterprise.Energy = 5;
            gameState.Enterprise.Shields = 5;
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShieldControl, -1.0);

            // Use reflection to set the private _gameState field
            var gameStateField = typeof(StarTrekGame).GetField("_gameState",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            gameStateField!.SetValue(game, gameState);

            // Act - Use reflection to access private method for testing
            var checkMethod = typeof(StarTrekGame).GetMethod("CheckForEmergencyConditions",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isStranded = (bool)checkMethod!.Invoke(game, null)!;

            // Assert
            Assert.True(isStranded);
        }

        [Fact]
        public void CheckForEmergencyConditions_EdgeCase_ExactlyTenEnergyWithDamagedShields_ShouldBeStranded()
        {
            // Arrange
            var game = new StarTrekGame();
            var gameState = new GameState(42); // Create GameState directly

            // Edge case: exactly at the boundary conditions
            gameState.Enterprise.Energy = 10;
            gameState.Enterprise.Shields = 0;
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShieldControl, -0.5);

            // Use reflection to set the private _gameState field
            var gameStateField = typeof(StarTrekGame).GetField("_gameState",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            gameStateField!.SetValue(game, gameState);

            // Act - Use reflection to access private method for testing
            var checkMethod = typeof(StarTrekGame).GetMethod("CheckForEmergencyConditions",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isStranded = (bool)checkMethod!.Invoke(game, null)!;

            // Assert
            Assert.True(isStranded);
        }

        [Fact]
        public void CheckForEmergencyConditions_EdgeCase_ElevenTotalEnergyWithDamagedShields_ShouldNotBeStranded()
        {
            // Arrange
            var game = new StarTrekGame();
            var gameState = new GameState(42); // Create GameState directly

            // Edge case: just above the boundary (11 > 10)
            gameState.Enterprise.Energy = 6;
            gameState.Enterprise.Shields = 5;
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShieldControl, -0.5);

            // Use reflection to set the private _gameState field
            var gameStateField = typeof(StarTrekGame).GetField("_gameState",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            gameStateField!.SetValue(game, gameState);

            // Act - Use reflection to access private method for testing
            var checkMethod = typeof(StarTrekGame).GetMethod("CheckForEmergencyConditions",
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            bool isStranded = (bool)checkMethod!.Invoke(game, null)!;

            // Assert
            Assert.False(isStranded);
            Assert.False(gameState.IsShipStranded);
        }

        [Fact]
        public void GameState_WhenShipIsStranded_ShouldMarkMissionAsFailed()
        {
            // Arrange
            var gameState = new GameState(42);

            // Act
            gameState.SetShipStranded();

            // Assert
            Assert.True(gameState.IsShipStranded);
            Assert.True(gameState.IsMissionFailed);
            Assert.True(gameState.IsGameOver);
            Assert.Equal("MISSION FAILED - SHIP STRANDED IN SPACE", gameState.GetMissionStatus());
        }
    }
}
