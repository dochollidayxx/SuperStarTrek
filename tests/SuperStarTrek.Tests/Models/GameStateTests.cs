using Xunit;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Tests.Models
{
    /// <summary>
    /// Tests for the GameState class
    /// </summary>
    public class GameStateTests
    {
        [Fact]
        public void Constructor_WithSeed_CreatesReproducibleGame()
        {
            // Arrange
            const int seed = 12345;

            // Act
            var game1 = new GameState(seed);
            var game2 = new GameState(seed);

            // Assert
            Assert.Equal(game1.InitialKlingonCount, game2.InitialKlingonCount);
            Assert.Equal(game1.StartingStardate, game2.StartingStardate);
            Assert.Equal(game1.MissionTimeLimit, game2.MissionTimeLimit);
            Assert.Equal(game1.Enterprise.QuadrantCoordinates, game2.Enterprise.QuadrantCoordinates);
            Assert.Equal(game1.Enterprise.SectorCoordinates, game2.Enterprise.SectorCoordinates);
        }

        [Fact]
        public void Constructor_WithoutSeed_CreatesValidGame()
        {
            // Act
            var gameState = new GameState();

            // Assert
            Assert.NotNull(gameState.Galaxy);
            Assert.NotNull(gameState.Enterprise);
            Assert.True(gameState.InitialKlingonCount > 0);
            Assert.True(gameState.CurrentStardate >= 2000 && gameState.CurrentStardate <= 3900);
            Assert.True(gameState.MissionTimeLimit >= 25 && gameState.MissionTimeLimit <= 34);
            Assert.InRange(gameState.Enterprise.QuadrantCoordinates.X, 1, 8);
            Assert.InRange(gameState.Enterprise.QuadrantCoordinates.Y, 1, 8);
            Assert.InRange(gameState.Enterprise.SectorCoordinates.X, 1, 8);
            Assert.InRange(gameState.Enterprise.SectorCoordinates.Y, 1, 8);
        }

        [Fact]
        public void CurrentStardate_InitiallyEqualsStartingStardate()
        {
            // Act
            var gameState = new GameState();

            // Assert
            Assert.Equal(gameState.StartingStardate, gameState.CurrentStardate);
        }

        [Fact]
        public void AdvanceTime_UpdatesCurrentStardate()
        {
            // Arrange
            var gameState = new GameState();
            var initialStardate = gameState.CurrentStardate;
            const double timeToAdvance = 5.5;

            // Act
            gameState.AdvanceTime(timeToAdvance);

            // Assert
            Assert.Equal(initialStardate + timeToAdvance, gameState.CurrentStardate);
        }

        [Fact]
        public void RemainingTime_CalculatedCorrectly()
        {
            // Arrange
            var gameState = new GameState();
            var expectedRemaining = gameState.MissionEndStardate - gameState.CurrentStardate;

            // Assert
            Assert.Equal(expectedRemaining, gameState.RemainingTime);

            // Advance time and check again
            gameState.AdvanceTime(10);
            expectedRemaining = gameState.MissionEndStardate - gameState.CurrentStardate;
            Assert.Equal(expectedRemaining, gameState.RemainingTime);
        }

        [Fact]
        public void IsMissionComplete_TrueWhenNoKlingonsRemaining()
        {
            // Arrange
            var gameState = new GameState(42); // Use seed for consistency

            // Act - Destroy all Klingons by clearing them from the galaxy
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrant = gameState.Galaxy.GetQuadrant(new Coordinates(x, y));
                    quadrant.KlingonShips.Clear();
                }
            }

            // Assert
            Assert.True(gameState.IsMissionComplete);
            Assert.Equal(0, gameState.KlingonsRemaining);
        }

        [Fact]
        public void IsMissionFailed_TrueWhenTimeExpired()
        {
            // Arrange
            var gameState = new GameState();

            // Act - Advance time beyond mission limit
            gameState.AdvanceTime(gameState.MissionTimeLimit + 1);

            // Assert
            Assert.True(gameState.IsMissionFailed);
            Assert.True(gameState.RemainingTime <= 0);
        }

        [Fact]
        public void IsGameOver_TrueWhenMissionCompleteOrFailed()
        {
            // Arrange
            var gameState = new GameState(42);

            // Initially game should not be over
            Assert.False(gameState.IsGameOver);

            // Complete mission
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrant = gameState.Galaxy.GetQuadrant(new Coordinates(x, y));
                    quadrant.KlingonShips.Clear();
                }
            }

            // Assert game is over due to completion
            Assert.True(gameState.IsGameOver);
            Assert.True(gameState.IsMissionComplete);
        }

        [Fact]
        public void MoveEnterpriseToQuadrant_UpdatesPositionCorrectly()
        {
            // Arrange
            var gameState = new GameState();
            var newQuadrant = new Coordinates(3, 4);
            var newSector = new Coordinates(5, 6);
            var oldQuadrant = gameState.Enterprise.QuadrantCoordinates;
            var oldSector = gameState.Enterprise.SectorCoordinates;

            // Act
            gameState.MoveEnterpriseToQuadrant(newQuadrant, newSector);

            // Assert
            Assert.Equal(newQuadrant, gameState.Enterprise.QuadrantCoordinates);
            Assert.Equal(newSector, gameState.Enterprise.SectorCoordinates);

            // Check that Enterprise is placed in new quadrant
            var currentQuadrant = gameState.CurrentQuadrant;
            Assert.True(currentQuadrant.IsEnterprisePresent);
            Assert.Equal(newSector, currentQuadrant.EnterprisePosition);
        }

        [Fact]
        public void CalculateEfficiencyRating_ReturnsZeroWhenMissionNotComplete()
        {
            // Arrange
            var gameState = new GameState();

            // Act & Assert
            Assert.Equal(0, gameState.CalculateEfficiencyRating());
        }

        [Fact]
        public void GetMissionStatus_ReturnsCorrectStatus()
        {
            // Arrange
            var gameState = new GameState(42);

            // Initially mission in progress
            Assert.Contains("MISSION IN PROGRESS", gameState.GetMissionStatus());

            // Complete mission
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrant = gameState.Galaxy.GetQuadrant(new Coordinates(x, y));
                    quadrant.KlingonShips.Clear();
                }
            }

            Assert.Equal("MISSION ACCOMPLISHED!", gameState.GetMissionStatus());
        }
    }
}
