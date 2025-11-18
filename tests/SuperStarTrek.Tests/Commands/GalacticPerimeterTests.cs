using System;
using Xunit;
using SuperStarTrek.Game;
using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for galactic perimeter boundary behavior (BASIC lines 3800-3850)
    /// </summary>
    public class GalacticPerimeterTests
    {
        /// <summary>
        /// Creates a test game state with specified position
        /// </summary>
        private GameState CreateTestGameState(int quadX, int quadY, int sectorX, int sectorY, int energy)
        {
            var gameState = new GameState(42);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(quadX, quadY);
            gameState.Enterprise.SectorCoordinates = new Coordinates(sectorX, sectorY);
            gameState.Enterprise.Energy = energy;
            gameState.Enterprise.PhotonTorpedoes = 10;

            // Ensure warp engines are functional
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, 0.0);

            return gameState;
        }

        [Fact]
        public void NavigationBeyondNorthBoundary_ShowsPerimeterMessage()
        {
            // Arrange - Start at quadrant (4,7) near north edge
            // Absolute Y = 8*7 + 8 = 64
            var gameState = CreateTestGameState(4, 7, 4, 8, 3000);
            var command = new NavigationCommand();

            // Act - Attempt to navigate north beyond galaxy boundary
            // Course 1 = North (deltaY = +1), Warp 2 moves 16 sectors north
            // finalAbsoluteY = 64 + 16 = 80, newQuadrantY = 10 (exceeds boundary)
            var result = command.Execute(gameState, new[] { "1", "2" });

            // Assert - According to BASIC, this should:
            // 1. Be successful (ship moves to boundary)
            // 2. Show Lt. Uhura's message about permission denied
            // 3. Show Chief Engineer Scott's report with final position
            // 4. Ship should be at boundary (Q1=1, S1=1)

            // NOTE: Current implementation FAILS this test - it returns failure
            // BASIC behavior: SUCCESS with perimeter messages
            Assert.True(result.IsSuccess, "BASIC allows movement to boundary with warning messages");
            Assert.Contains("UHURA", result.Message ?? "");
            Assert.Contains("STARFLEET COMMAND", result.Message ?? "");
            Assert.Contains("PERMISSION", result.Message ?? "");
            Assert.Contains("CROSSING OF GALACTIC PERIMETER", result.Message ?? "");
            Assert.Contains("*DENIED*", result.Message ?? "");
            Assert.Contains("SCOTT", result.Message ?? "");
            Assert.Contains("WARP ENGINES SHUT DOWN", result.Message ?? "");

            // Verify ship is at boundary position (clamped to Q2=8, S2=8)
            Assert.Equal(8, gameState.Enterprise.QuadrantCoordinates.Y);
            Assert.Equal(8, gameState.Enterprise.SectorCoordinates.Y);
        }

        [Fact]
        public void NavigationBeyondSouthBoundary_ShowsPerimeterMessage()
        {
            // Arrange - Start at quadrant (4,2) near south edge
            // Absolute Y = 8*2 + 1 = 17
            var gameState = CreateTestGameState(4, 2, 4, 1, 3000);
            var command = new NavigationCommand();

            // Act - Attempt to navigate south beyond galaxy boundary
            // Course 5 = South (deltaY = -1), Warp 2 moves 16 sectors south
            // finalAbsoluteY = 17 - 16 = 1, newQuadrantY = 0 (below boundary)
            var result = command.Execute(gameState, new[] { "5", "2" });

            // Assert
            Assert.True(result.IsSuccess, "BASIC allows movement to boundary with warning messages");
            Assert.Contains("UHURA", result.Message ?? "");
            Assert.Contains("GALACTIC PERIMETER", result.Message ?? "");
            Assert.Contains("*DENIED*", result.Message ?? "");
            Assert.Contains("SCOTT", result.Message ?? "");

            // Verify ship is at boundary position (clamped to Q2=1, S2=1)
            Assert.Equal(1, gameState.Enterprise.QuadrantCoordinates.Y);
            Assert.Equal(1, gameState.Enterprise.SectorCoordinates.Y);
        }

        [Fact]
        public void NavigationBeyondEastBoundary_ShowsPerimeterMessage()
        {
            // Arrange - Start at quadrant (2,4) near east edge (Q1 approaching 1)
            // Absolute X = 8*2 + 1 = 17
            var gameState = CreateTestGameState(2, 4, 1, 4, 3000);
            var command = new NavigationCommand();

            // Act - Attempt to navigate east beyond galaxy boundary
            // Course 3 = East (deltaX = -1), Warp 2 moves 16 sectors
            // finalAbsoluteX = 17 - 16 = 1, newQuadrantX = 0 (below boundary)
            var result = command.Execute(gameState, new[] { "3", "2" });

            // Assert
            Assert.True(result.IsSuccess, "BASIC allows movement to boundary with warning messages");
            Assert.Contains("UHURA", result.Message ?? "");
            Assert.Contains("GALACTIC PERIMETER", result.Message ?? "");
            Assert.Contains("*DENIED*", result.Message ?? "");
            Assert.Contains("SCOTT", result.Message ?? "");

            // Verify ship is at boundary position (clamped to Q1=1, S1=1)
            Assert.Equal(1, gameState.Enterprise.QuadrantCoordinates.X);
            Assert.Equal(1, gameState.Enterprise.SectorCoordinates.X);
        }

        [Fact]
        public void NavigationBeyondWestBoundary_ShowsPerimeterMessage()
        {
            // Arrange - Start at quadrant (7,4) near west edge (Q1 approaching 8)
            // Absolute X = 8*7 + 8 = 64
            var gameState = CreateTestGameState(7, 4, 8, 4, 3000);
            var command = new NavigationCommand();

            // Act - Attempt to navigate west beyond galaxy boundary
            // Course 7 = West (deltaX = 1), Warp 2 moves 16 sectors
            // finalAbsoluteX = 64 + 16 = 80, newQuadrantX = 10 (exceeds boundary)
            var result = command.Execute(gameState, new[] { "7", "2" });

            // Assert
            Assert.True(result.IsSuccess, "BASIC allows movement to boundary with warning messages");
            Assert.Contains("UHURA", result.Message ?? "");
            Assert.Contains("GALACTIC PERIMETER", result.Message ?? "");
            Assert.Contains("*DENIED*", result.Message ?? "");
            Assert.Contains("SCOTT", result.Message ?? "");

            // Verify ship is at boundary position (clamped to Q1=8, S1=8)
            Assert.Equal(8, gameState.Enterprise.QuadrantCoordinates.X);
            Assert.Equal(8, gameState.Enterprise.SectorCoordinates.X);
        }

        [Fact]
        public void NavigationToBoundary_ReportsCorrectPosition()
        {
            // Arrange - Position to actually hit boundary
            var gameState = CreateTestGameState(4, 7, 4, 8, 3000);
            var command = new NavigationCommand();

            // Act - Navigate to boundary (north)
            var result = command.Execute(gameState, new[] { "1", "2" });

            // Assert - Message should include final position
            // "AT SECTOR S1, S2 OF QUADRANT Q1, Q2"
            Assert.Contains("SECTOR", result.Message ?? "");
            Assert.Contains("QUADRANT", result.Message ?? "");
            Assert.Contains("1", result.Message ?? ""); // Should show Q1=1 or Q2 value
        }

        [Fact]
        public void NavigationToBoundary_ConsumesTimeAndEnergy()
        {
            // Arrange - Position to actually hit boundary
            var gameState = CreateTestGameState(4, 7, 4, 8, 3000);
            var initialEnergy = gameState.Enterprise.Energy;
            var command = new NavigationCommand();

            // Act - Navigate to boundary (Warp 2 = 16 sectors + 10 = 26 energy)
            var result = command.Execute(gameState, new[] { "1", "2" });

            // Assert - Time and energy should be consumed
            Assert.True(result.ConsumesTime, "Navigation consumes time even when hitting boundary");
            Assert.True(result.TimeConsumed > 0, "Time consumed should be positive");
            Assert.True(gameState.Enterprise.Energy < initialEnergy, "Energy should be consumed");
        }

        [Fact]
        public void NavigationWithinGalaxy_NoPerimeterMessage()
        {
            // Arrange - Start in middle of galaxy
            var gameState = CreateTestGameState(4, 4, 4, 4, 3000);
            var command = new NavigationCommand();

            // Act - Navigate within galaxy bounds
            var result = command.Execute(gameState, new[] { "1", "1" });

            // Assert - No perimeter messages should appear
            Assert.True(result.IsSuccess);
            Assert.DoesNotContain("UHURA", result.Message ?? "");
            Assert.DoesNotContain("GALACTIC PERIMETER", result.Message ?? "");
            Assert.DoesNotContain("SCOTT", result.Message ?? "");
        }

        [Fact]
        public void NavigationToCorner_NorthWest_ShowsPerimeterMessage()
        {
            // Arrange - Start near northwest corner
            // Course 8 = NW (deltaX=1, deltaY=1), so we need X and Y to exceed
            var gameState = CreateTestGameState(7, 7, 4, 4, 3000);
            var command = new NavigationCommand();

            // Act - Navigate northwest (course 8) beyond boundary
            // Warp 2 = 16 sectors, will push both X and Y past boundary
            var result = command.Execute(gameState, new[] { "8", "2" });

            // Assert - Should hit boundary and show perimeter message
            Assert.True(result.IsSuccess, "Movement to boundary should succeed");
            Assert.Contains("GALACTIC PERIMETER", result.Message ?? "");

            // Should be at corner (8,8)
            Assert.Equal(8, gameState.Enterprise.QuadrantCoordinates.Y);
            Assert.Equal(8, gameState.Enterprise.QuadrantCoordinates.X);
        }

        [Fact]
        public void NavigationToCorner_SouthEast_ShowsPerimeterMessage()
        {
            // Arrange - Start near southeast corner
            // Course 4 = SE (deltaX=-1, deltaY=-1), so need both to go below 1
            var gameState = CreateTestGameState(2, 2, 4, 4, 3000);
            var command = new NavigationCommand();

            // Act - Navigate southeast (course 4) beyond boundary
            // Warp 2 = 16 sectors, will push both X and Y below boundary
            var result = command.Execute(gameState, new[] { "4", "2" });

            // Assert - Should hit boundary and show perimeter message
            Assert.True(result.IsSuccess, "Movement to boundary should succeed");
            Assert.Contains("GALACTIC PERIMETER", result.Message ?? "");

            // Should be at corner (1,1)
            Assert.Equal(1, gameState.Enterprise.QuadrantCoordinates.Y);
            Assert.Equal(1, gameState.Enterprise.QuadrantCoordinates.X);
        }

        [Fact]
        public void PerimeterMessage_MatchesBASICFormat()
        {
            // Arrange - Position to actually hit boundary
            var gameState = CreateTestGameState(4, 7, 4, 8, 3000);
            var command = new NavigationCommand();

            // Act - Navigate north to boundary
            var result = command.Execute(gameState, new[] { "1", "2" });

            // Assert - Verify exact BASIC message format
            var message = result.Message ?? "";

            // BASIC Line 3800-3810
            Assert.Contains("LT. UHURA REPORTS MESSAGE FROM STARFLEET COMMAND:", message);
            Assert.Contains("'PERMISSION TO ATTEMPT CROSSING OF GALACTIC PERIMETER", message);
            Assert.Contains("IS HEREBY *DENIED*.  SHUT DOWN YOUR ENGINES.'", message);

            // BASIC Line 3830-3840
            Assert.Contains("CHIEF ENGINEER SCOTT REPORTS", message);
            Assert.Contains("'WARP ENGINES SHUT DOWN", message);
        }
    }
}
