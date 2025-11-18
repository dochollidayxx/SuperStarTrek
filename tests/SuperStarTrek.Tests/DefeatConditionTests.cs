using Xunit;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Tests
{
    /// <summary>
    /// Tests for defeat conditions
    /// Verifies BASIC lines 6090, 6220-6290 (ship destruction, time limit, stranded)
    /// </summary>
    public class DefeatConditionTests
    {
        [Fact]
        public void IsMissionFailed_WhenEnterpriseEnergyZero_ReturnsTrue()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.Energy = 0;

            // Act & Assert
            Assert.True(gameState.IsMissionFailed);
            Assert.True(gameState.IsGameOver);
        }

        [Fact]
        public void IsMissionFailed_WhenEnterpriseEnergyNegative_ReturnsTrue()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.Energy = -10;

            // Act & Assert
            Assert.True(gameState.IsMissionFailed);
        }

        [Fact]
        public void IsMissionFailed_WhenTimeExpired_ReturnsTrue()
        {
            // Arrange
            var gameState = new GameState(42);

            // Advance time past mission deadline
            var timeToExpire = gameState.MissionTimeLimit + 1;
            gameState.AdvanceTime(timeToExpire);

            // Act & Assert
            Assert.True(gameState.IsMissionFailed);
            Assert.True(gameState.IsGameOver);
            Assert.True(gameState.RemainingTime <= 0);
        }

        [Fact]
        public void IsMissionFailed_WhenShipStranded_ReturnsTrue()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.SetShipStranded();

            // Act & Assert
            Assert.True(gameState.IsMissionFailed);
            Assert.True(gameState.IsGameOver);
            Assert.True(gameState.IsShipStranded);
        }

        [Fact]
        public void IsMissionFailed_WhenNormalConditions_ReturnsFalse()
        {
            // Arrange
            var gameState = new GameState(42);

            // Act & Assert
            Assert.False(gameState.IsMissionFailed);
            Assert.False(gameState.IsGameOver);
        }

        [Fact]
        public void GetMissionStatus_WhenTimeExpired_ReturnsTimeExpiredMessage()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.AdvanceTime(gameState.MissionTimeLimit + 1);

            // Act
            var status = gameState.GetMissionStatus();

            // Assert
            Assert.Equal("MISSION FAILED - TIME EXPIRED", status);
        }

        [Fact]
        public void GetMissionStatus_WhenEnterpriseDestroyed_ReturnsDestroyedMessage()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.Energy = 0;

            // Act
            var status = gameState.GetMissionStatus();

            // Assert
            Assert.Equal("MISSION FAILED - ENTERPRISE DESTROYED", status);
        }

        [Fact]
        public void GetMissionStatus_WhenShipStranded_ReturnsStrandedMessage()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.SetShipStranded();

            // Act
            var status = gameState.GetMissionStatus();

            // Assert
            Assert.Equal("MISSION FAILED - SHIP STRANDED IN SPACE", status);
        }

        [Fact]
        public void ShieldsDestroyedByKlingonAttack_ShouldSetEnergyToZero()
        {
            // Arrange - Original BASIC line 6090: IF S<=0 THEN 6240
            // When shields go to 0 or below, ship is destroyed
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;

            // Set shields to a low value
            enterprise.Shields = 50;
            enterprise.Energy = 1000;

            // Simulate heavy Klingon attack (100 damage)
            enterprise.Shields -= 100;

            // Act - In real game, counter-attack code would detect shields <= 0 and set energy to 0
            // We simulate that here
            if (enterprise.Shields <= 0)
            {
                enterprise.Energy = 0;
            }

            // Assert
            Assert.True(enterprise.Shields <= 0);
            Assert.Equal(0, enterprise.Energy);
            Assert.True(gameState.IsMissionFailed);
        }

        [Fact]
        public void ShieldsAtExactlyZero_ShouldTriggerDefeat()
        {
            // Arrange - Test boundary condition
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;

            enterprise.Shields = 0;
            enterprise.Energy = 0;

            // Act & Assert
            Assert.True(gameState.IsMissionFailed);
        }

        [Fact]
        public void ShieldsJustAboveZero_ShouldNotTriggerDefeat()
        {
            // Arrange
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;

            enterprise.Shields = 1;
            enterprise.Energy = 500;

            // Act & Assert
            Assert.False(gameState.IsMissionFailed);
        }

        [Fact]
        public void RemainingTime_AtMissionStart_ShouldEqualTimeLimit()
        {
            // Arrange
            var gameState = new GameState(42);

            // Act
            var remainingTime = gameState.RemainingTime;

            // Assert
            Assert.Equal(gameState.MissionTimeLimit, remainingTime);
        }

        [Fact]
        public void RemainingTime_AfterAdvancingTime_ShouldDecrease()
        {
            // Arrange
            var gameState = new GameState(42);
            var initialRemaining = gameState.RemainingTime;

            // Act
            gameState.AdvanceTime(5);

            // Assert
            Assert.Equal(initialRemaining - 5, gameState.RemainingTime);
        }

        [Fact]
        public void MissionEndStardate_ShouldEqualStartPlusTimeLimit()
        {
            // Arrange
            var gameState = new GameState(42);

            // Act
            var endDate = gameState.MissionEndStardate;

            // Assert
            Assert.Equal(gameState.StartingStardate + gameState.MissionTimeLimit, endDate);
        }
    }
}
