using Xunit;
using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for starbase destruction consequences (BASIC lines 5330-5410)
    /// Verifies authentic BASIC behavior for court martial and instant game over conditions
    /// </summary>
    public class StarbaseDestructionTests
    {
        /// <summary>
        /// Test: Starbase destruction message displayed
        /// BASIC: Line 5330 - PRINT"*** STARBASE DESTROYED ***"
        /// </summary>
        [Fact]
        public void DestroyStarbase_ShowsDestructionMessage()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.SectorCoordinates = new Coordinates(3, 4);

            // Place a starbase at sector 4,4 in current quadrant
            gameState.CurrentQuadrant.PlaceStarbase(new Coordinates(4, 4));

            var torpedoCommand = new TorpedoCommand();

            // Act - Fire torpedo at starbase (course 3 = east)
            var result = torpedoCommand.Execute(gameState, new[] { "3.0" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("*** STARBASE DESTROYED ***", result.Message);
        }

        /// <summary>
        /// Test: Court martial warning when starbases remain
        /// BASIC: Line 5360 - IF B9>0 THEN 5400 (court martial)
        /// </summary>
        [Fact]
        public void DestroyStarbase_WhenOthersRemain_ShowsCourtMartialWarning()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.SectorCoordinates = new Coordinates(3, 4);

            // Place a starbase in current quadrant
            gameState.CurrentQuadrant.PlaceStarbase(new Coordinates(4, 4));

            // The galaxy should have at least one other starbase from random generation
            var initialStarbases = gameState.Galaxy.TotalStarbases;
            Assert.True(initialStarbases > 0, "Test requires at least one starbase in galaxy");

            var torpedoCommand = new TorpedoCommand();

            // Act
            var result = torpedoCommand.Execute(gameState, new[] { "3.0" });

            // Assert - Should show court martial if other starbases remain OR mission time allows
            // BASIC: IF B9>0 OR K9>T-T0-T9 THEN 5400
            Assert.True(result.IsSuccess);
            Assert.Contains("*** STARBASE DESTROYED ***", result.Message);

            // Will show court martial warning if B9>0 (other starbases) OR K9+RemainingTime>0
            // In early game with seed 42, should have starbases remaining
            if (gameState.Galaxy.TotalStarbases > 0)
            {
                Assert.Contains("COURT MARTIAL!", result.Message);
                Assert.DoesNotContain("99 STARDATES", result.Message);
            }
        }

        /// <summary>
        /// Test: Verifies the BASIC logic condition is implemented
        /// BASIC: Line 5360 - IF B9>0 OR K9>T-T0-T9 THEN 5400
        /// </summary>
        [Fact]
        public void DestroyStarbase_ImplementsCorrectBasicCondition()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.SectorCoordinates = new Coordinates(3, 4);

            // Place a starbase
            gameState.CurrentQuadrant.PlaceStarbase(new Coordinates(4, 4));

            var klingonsRemaining = gameState.KlingonsRemaining;
            var remainingTime = gameState.RemainingTime;
            var totalStarbases = gameState.Galaxy.TotalStarbases;

            var torpedoCommand = new TorpedoCommand();

            // Act
            var result = torpedoCommand.Execute(gameState, new[] { "3.0" });

            // Assert - Verify the condition after destruction
            var starbasesAfter = gameState.Galaxy.TotalStarbases;

            // The BASIC condition: IF B9>0 OR K9>T-T0-T9 THEN 5400
            // Translates to: IF starbases>0 OR (klingons + remainingTime > 0) THEN court_martial
            bool shouldBeCourtMartial = (starbasesAfter > 0) || (klingonsRemaining + remainingTime > 0);

            if (shouldBeCourtMartial)
            {
                Assert.Contains("COURT MARTIAL!", result.Message);
                Assert.DoesNotContain("99 STARDATES AT HARD LABOR", result.Message);
            }
            else
            {
                Assert.Contains("99 STARDATES AT HARD LABOR ON CYGNUS 12!!", result.Message);
                Assert.DoesNotContain("COURT MARTIAL!", result.Message);
            }
        }

        /// <summary>
        /// Test: Undocking when docked starbase is destroyed
        /// BASIC: Line 5410 - D0=0 (undock)
        /// </summary>
        [Fact]
        public void DestroyStarbase_WhileDocked_UndocksEnterprise()
        {
            // Arrange
            var gameState = new GameState(42);

            // Place starbase adjacent to Enterprise
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.CurrentQuadrant.PlaceStarbase(new Coordinates(4, 5));

            // Dock the Enterprise
            gameState.Enterprise.DockAtStarbase();
            Assert.True(gameState.Enterprise.IsDocked);

            var torpedoCommand = new TorpedoCommand();

            // Act - Fire torpedo at starbase (course 3 = east, sector 4,5)
            var result = torpedoCommand.Execute(gameState, new[] { "3.0" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("*** STARBASE DESTROYED ***", result.Message);

            // Should undock (D0=0 in BASIC line 5410) regardless of court martial or game over
            Assert.False(gameState.Enterprise.IsDocked);
        }

        /// <summary>
        /// Test: Starbase counter decremented correctly
        /// BASIC: Line 5330 - B3=B3-1:B9=B9-1
        /// </summary>
        [Fact]
        public void DestroyStarbase_DecrementsStarbaseCount()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.SectorCoordinates = new Coordinates(3, 4);

            // Place a starbase
            gameState.CurrentQuadrant.PlaceStarbase(new Coordinates(4, 4));

            var initialCount = gameState.Galaxy.TotalStarbases;

            var torpedoCommand = new TorpedoCommand();

            // Act
            var result = torpedoCommand.Execute(gameState, new[] { "3.0" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(initialCount - 1, gameState.Galaxy.TotalStarbases);
        }

        /// <summary>
        /// Test: Both messages (destruction + court martial) appear together
        /// BASIC: Lines 5330 and 5400-5410
        /// </summary>
        [Fact]
        public void DestroyStarbase_ShowsBothDestructionAndCourtMartialMessages()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.SectorCoordinates = new Coordinates(3, 4);

            // Place a starbase
            gameState.CurrentQuadrant.PlaceStarbase(new Coordinates(4, 4));

            var torpedoCommand = new TorpedoCommand();

            // Act
            var result = torpedoCommand.Execute(gameState, new[] { "3.0" });

            // Assert
            Assert.True(result.IsSuccess);

            // Should have destruction message
            Assert.Contains("*** STARBASE DESTROYED ***", result.Message);

            // And either court martial OR instant game over (not both)
            bool hasCourtMartial = result.Message.Contains("COURT MARTIAL!");
            bool hasGameOver = result.Message.Contains("99 STARDATES AT HARD LABOR");

            // Exactly one should be true (XOR)
            Assert.True(hasCourtMartial ^ hasGameOver, "Should have either court martial or game over, but not both");
        }

        /// <summary>
        /// Test: Verify messages match BASIC exactly
        /// </summary>
        [Fact]
        public void DestroyStarbase_MessagesMatchBasicExactly()
        {
            // Arrange
            var gameState = new GameState(42);
            gameState.Enterprise.SectorCoordinates = new Coordinates(3, 4);
            gameState.CurrentQuadrant.PlaceStarbase(new Coordinates(4, 4));

            var torpedoCommand = new TorpedoCommand();

            // Act
            var result = torpedoCommand.Execute(gameState, new[] { "3.0" });

            // Assert - Check exact BASIC messages
            Assert.Contains("*** STARBASE DESTROYED ***", result.Message); // Line 5330

            // Either court martial (lines 5400-5410) or game over (lines 5370-5380)
            if (result.Message.Contains("COURT MARTIAL!"))
            {
                Assert.Contains("STARFLEET COMMAND REVIEWING YOUR RECORD TO CONSIDER", result.Message);
                Assert.Contains("COURT MARTIAL!", result.Message);
            }
            else
            {
                Assert.Contains("THAT DOES IT, CAPTAIN!!  YOU ARE HEREBY RELIEVED OF COMMAND", result.Message);
                Assert.Contains("AND SENTENCED TO 99 STARDATES AT HARD LABOR ON CYGNUS 12!!", result.Message);
            }
        }
    }
}
