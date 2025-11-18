using Xunit;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Tests
{
    /// <summary>
    /// Tests for victory conditions and efficiency rating calculation
    /// Verifies BASIC lines 6370-6400
    /// </summary>
    public class VictoryConditionTests
    {
        [Fact]
        public void IsMissionComplete_WhenAllKlingonsDestroyed_ReturnsTrue()
        {
            // Arrange
            var gameState = new GameState(42);

            // Destroy all Klingons in galaxy by setting their shields to 0
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrantCoord = new Coordinates(x, y);
                    var quadrant = gameState.Galaxy.GetQuadrant(quadrantCoord);
                    foreach (var klingon in quadrant.KlingonShips)
                    {
                        klingon.ShieldLevel = 0; // This makes IsDestroyed return true
                    }
                }
            }

            // Act & Assert
            Assert.True(gameState.IsMissionComplete);
        }

        [Fact]
        public void IsMissionComplete_WhenKlingonsRemaining_ReturnsFalse()
        {
            // Arrange
            var gameState = new GameState(42);

            // Act & Assert
            Assert.False(gameState.IsMissionComplete);
            Assert.True(gameState.KlingonsRemaining > 0);
        }

        [Fact]
        public void IsGameOver_WhenMissionComplete_ReturnsTrue()
        {
            // Arrange
            var gameState = new GameState(42);

            // Destroy all Klingons by setting their shields to 0
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrantCoord = new Coordinates(x, y);
                    var quadrant = gameState.Galaxy.GetQuadrant(quadrantCoord);
                    foreach (var klingon in quadrant.KlingonShips)
                    {
                        klingon.ShieldLevel = 0;
                    }
                }
            }

            // Act & Assert
            Assert.True(gameState.IsGameOver);
        }

        [Fact]
        public void CalculateEfficiencyRating_WhenMissionComplete_UsesCorrectFormula()
        {
            // Arrange
            var gameState = new GameState(42);
            var initialKlingonCount = gameState.InitialKlingonCount;
            var startStardate = gameState.StartingStardate;

            // Destroy all Klingons by setting their shields to 0
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrantCoord = new Coordinates(x, y);
                    var quadrant = gameState.Galaxy.GetQuadrant(quadrantCoord);
                    foreach (var klingon in quadrant.KlingonShips)
                    {
                        klingon.ShieldLevel = 0;
                    }
                }
            }

            // Advance time by 10 stardates
            gameState.AdvanceTime(10);
            var timeUsed = gameState.CurrentStardate - startStardate;

            // Act
            var rating = gameState.CalculateEfficiencyRating();

            // Assert - Match BASIC formula: 1000*(K7/(T-T0))^2
            var expectedRating = 1000 * Math.Pow(initialKlingonCount / timeUsed, 2);
            Assert.Equal(expectedRating, rating, 2); // Allow small floating point difference
        }

        [Fact]
        public void CalculateEfficiencyRating_WithFastCompletion_GivesHigherScore()
        {
            // Scenario 1: Complete in 5 stardates
            var gameState1 = new GameState(42);
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrantCoord = new Coordinates(x, y);
                    var quadrant = gameState1.Galaxy.GetQuadrant(quadrantCoord);
                    foreach (var klingon in quadrant.KlingonShips)
                    {
                        klingon.ShieldLevel = 0;
                    }
                }
            }
            gameState1.AdvanceTime(5);
            var rating1 = gameState1.CalculateEfficiencyRating();

            // Scenario 2: Complete in 10 stardates (slower)
            var gameState2 = new GameState(42);
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrantCoord = new Coordinates(x, y);
                    var quadrant = gameState2.Galaxy.GetQuadrant(quadrantCoord);
                    foreach (var klingon in quadrant.KlingonShips)
                    {
                        klingon.ShieldLevel = 0;
                    }
                }
            }
            gameState2.AdvanceTime(10);
            var rating2 = gameState2.CalculateEfficiencyRating();

            // Assert - Faster completion should have higher rating
            Assert.True(rating1 > rating2);
        }

        [Fact]
        public void CalculateEfficiencyRating_WhenMissionNotComplete_ReturnsZero()
        {
            // Arrange
            var gameState = new GameState(42);

            // Don't destroy Klingons, advance time
            gameState.AdvanceTime(10);

            // Act
            var rating = gameState.CalculateEfficiencyRating();

            // Assert
            Assert.Equal(0, rating);
        }

        [Fact]
        public void GetMissionStatus_WhenMissionComplete_ReturnsAccomplished()
        {
            // Arrange
            var gameState = new GameState(42);

            // Destroy all Klingons by setting their shields to 0
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrantCoord = new Coordinates(x, y);
                    var quadrant = gameState.Galaxy.GetQuadrant(quadrantCoord);
                    foreach (var klingon in quadrant.KlingonShips)
                    {
                        klingon.ShieldLevel = 0;
                    }
                }
            }

            // Act
            var status = gameState.GetMissionStatus();

            // Assert
            Assert.Equal("MISSION ACCOMPLISHED!", status);
        }

        [Fact]
        public void EfficiencyRating_ExampleCalculation_10KlingonsIn10Days()
        {
            // Arrange
            var gameState = new GameState(42);

            // Use the actual initial count for this seed
            var initialKlingons = gameState.InitialKlingonCount;

            // Destroy all Klingons by setting their shields to 0
            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var quadrantCoord = new Coordinates(x, y);
                    var quadrant = gameState.Galaxy.GetQuadrant(quadrantCoord);
                    foreach (var klingon in quadrant.KlingonShips)
                    {
                        klingon.ShieldLevel = 0;
                    }
                }
            }

            // Advance time by exactly 10 stardates
            gameState.AdvanceTime(10);

            // Act
            var rating = gameState.CalculateEfficiencyRating();

            // Assert - With 10 days: rating = 1000 * (K7/10)^2
            var expectedRating = 1000 * Math.Pow(initialKlingons / 10.0, 2);
            Assert.Equal(expectedRating, rating, 2);
        }
    }
}
