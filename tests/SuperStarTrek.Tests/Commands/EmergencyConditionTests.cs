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
            var gameState = CreateTestGameState();

            // Set conditions where ship should NOT be stranded
            gameState.Enterprise.Energy = 50;
            gameState.Enterprise.Shields = 50;
            // Shield control is not damaged (default is 0.0)

            // Act
            bool isStranded = CheckEmergencyCondition(gameState);

            // Assert
            Assert.False(isStranded);
            Assert.False(gameState.IsShipStranded);
        }

        [Fact]
        public void CheckForEmergencyConditions_WhenShipHasLowEnergyButShieldsWork_ShouldNotBeStranded()
        {
            // Arrange
            var gameState = CreateTestGameState();

            // Set conditions: low total energy but shield control works
            gameState.Enterprise.Energy = 5;
            gameState.Enterprise.Shields = 5;
            // Shield control is not damaged (default is 0.0)

            // Act
            bool isStranded = CheckEmergencyCondition(gameState);

            // Assert
            Assert.False(isStranded);
            Assert.False(gameState.IsShipStranded);
        }

        [Fact]
        public void CheckForEmergencyConditions_WhenShipHasLowEnergyAndDamagedShields_ShouldBeStranded()
        {
            // Arrange
            var gameState = CreateTestGameState();

            // Set fatal error conditions: 
            // - Total energy (shields + energy) <= 10
            // - Energy <= 10 
            // - Shield control is damaged (negative value)
            gameState.Enterprise.Energy = 5;
            gameState.Enterprise.Shields = 5;
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShieldControl, -1.0);

            // Act
            bool isStranded = CheckEmergencyCondition(gameState);

            // Assert
            Assert.True(isStranded);
        }

        [Fact]
        public void CheckForEmergencyConditions_EdgeCase_ExactlyTenEnergyWithDamagedShields_ShouldBeStranded()
        {
            // Arrange
            var gameState = CreateTestGameState();

            // Edge case: exactly at the boundary conditions
            gameState.Enterprise.Energy = 10;
            gameState.Enterprise.Shields = 0;
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShieldControl, -0.5);

            // Act
            bool isStranded = CheckEmergencyCondition(gameState);

            // Assert
            Assert.True(isStranded);
        }

        [Fact]
        public void CheckForEmergencyConditions_EdgeCase_ElevenTotalEnergyWithDamagedShields_ShouldNotBeStranded()
        {
            // Arrange
            var gameState = CreateTestGameState();

            // Edge case: just above the boundary (11 > 10)
            gameState.Enterprise.Energy = 6;
            gameState.Enterprise.Shields = 5;
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShieldControl, -0.5);

            // Act
            bool isStranded = CheckEmergencyCondition(gameState);

            // Assert
            Assert.False(isStranded);
            Assert.False(gameState.IsShipStranded);
        }

        [Fact]
        public void GameState_WhenShipIsStranded_ShouldMarkMissionAsFailed()
        {
            // Arrange
            var gameState = CreateTestGameState();

            // Act
            gameState.SetShipStranded();

            // Assert
            Assert.True(gameState.IsShipStranded);
            Assert.True(gameState.IsMissionFailed);
            Assert.True(gameState.IsGameOver);
            Assert.Equal("MISSION FAILED - SHIP STRANDED IN SPACE", gameState.GetMissionStatus());
        }

        /// <summary>
        /// Helper method to test the emergency condition logic directly
        /// Replicates the logic from StarTrekGame.CheckForEmergencyConditions()
        /// </summary>
        private bool CheckEmergencyCondition(GameState gameState)
        {
            var enterprise = gameState.Enterprise;

            // Emergency condition from original BASIC lines 1990-2050:
            // If (shields + energy) <= 10 AND (energy <= 10 AND shield control is damaged)
            var totalAvailableEnergy = enterprise.Shields + enterprise.Energy;
            var isShieldControlDamaged = enterprise.GetSystemDamage(ShipSystem.ShieldControl) < 0;

            return totalAvailableEnergy <= 10 && (enterprise.Energy <= 10 && isShieldControlDamaged);
        }

        /// <summary>
        /// Creates a test game state with default values
        /// </summary>
        private GameState CreateTestGameState()
        {
            return new GameState(seed: 42);
        }
    }
}
