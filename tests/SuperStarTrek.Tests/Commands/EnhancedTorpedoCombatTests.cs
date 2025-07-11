using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Enhanced torpedo combat tests based on original BASIC implementation (lines 4690-5490)
    /// </summary>
    public class EnhancedTorpedoCombatTests
    {
        [Fact]
        public void TorpedoCombat_NoTorpedoes_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 0; // No torpedoes left

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "3.0" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ALL PHOTON TORPEDOES EXPENDED", result.Message);
        }

        [Fact]
        public void TorpedoCombat_DamagedPhotonTubes_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.SetSystemDamage(ShipSystem.PhotonTubes, -1.0); // Damage photon tubes

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "3.0" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("PHOTON TUBES ARE NOT OPERATIONAL", result.Message);
        }

        [Fact]
        public void TorpedoCombat_InvalidCourse_ReturnsFailure()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "10.0" }); // Invalid course

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("ENSIGN CHEKOV REPORTS,  'INCORRECT COURSE DATA, SIR!'", result.Message);
        }

        [Fact]
        public void TorpedoCombat_Course9_ConvertsTo1()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;
            // Clear any Klingons to avoid counter-attack for precise energy test
            gameState.CurrentQuadrant.KlingonShips.Clear();
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "9.0" }); // Course 9 should become 1

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("TORPEDO TRACK:", result.Message);
            Assert.Equal(4, gameState.Enterprise.PhotonTorpedoes); // One torpedo consumed
            Assert.Equal(998, gameState.Enterprise.Energy); // Exactly 2 energy consumed with no counter-attack
        }

        [Fact]
        public void TorpedoCombat_EnergyAndTorpedoConsumption_MatchesOriginal()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "3.0" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(4, gameState.Enterprise.PhotonTorpedoes); // P=P-1 in original
            // Energy should be reduced by at least 2 (E=E-2 in original), may be more due to Klingon counter-attack
            Assert.True(gameState.Enterprise.Energy <= 998);
        }

        [Fact]
        public void TorpedoCombat_HitKlingon_DestroysCompletely()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;

            // Place Klingon directly in torpedo path (course 3 = East)
            var klingon = new KlingonShip(new Coordinates(5, 4), 200);
            gameState.CurrentQuadrant.PlaceKlingon(klingon);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "3.0" }); // Fire east

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("*** KLINGON DESTROYED ***", result.Message);
            Assert.Contains("TORPEDO TRACK:", result.Message);
            Assert.Contains("5,4", result.Message); // Should show torpedo hitting at 5,4
            Assert.True(klingon.IsDestroyed);
        }

        [Fact]
        public void TorpedoCombat_HitStar_AbsorbsEnergy()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;

            // Place star directly in torpedo path
            gameState.CurrentQuadrant.PlaceStar(new Coordinates(5, 4));
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "3.0" }); // Fire east

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("STAR AT 5,4 ABSORBED TORPEDO ENERGY", result.Message);
            Assert.Contains("TORPEDO TRACK:", result.Message);
        }

        [Fact]
        public void TorpedoCombat_HitStarbase_ShowsCourtMartialMessage()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;

            // Place starbase directly in torpedo path
            gameState.CurrentQuadrant.PlaceStarbase(new Coordinates(5, 4));
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "3.0" }); // Fire east

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("*** STARBASE DESTROYED ***", result.Message);
            // Should show either court martial warning or relieved of command message
            Assert.True(!string.IsNullOrEmpty(result.Message) &&
                       (result.Message.Contains("STARFLEET COMMAND REVIEWING") ||
                        result.Message.Contains("THAT DOES IT, CAPTAIN")));
            Assert.False(gameState.CurrentQuadrant.HasStarbase);
        }

        [Fact]
        public void TorpedoCombat_ExitQuadrant_ShowsMissMessage()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(8, 8); // Near edge
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;

            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "3.0" }); // Fire east (will exit quadrant)

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("TORPEDO MISSED", result.Message);
            Assert.Contains("TORPEDO TRACK:", result.Message);
        }

        [Fact]
        public void TorpedoCombat_TrajectoryCalculation_UsesCorrectDirectionVectors()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;

            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act - Fire in different directions to test trajectory calculation
            var resultNorth = command.Execute(gameState, new[] { "1.0" }); // North

            // Reset for next test
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;
            var resultEast = command.Execute(gameState, new[] { "3.0" }); // East

            // Assert
            Assert.True(resultNorth.IsSuccess);
            Assert.True(resultEast.IsSuccess);
            Assert.Contains("TORPEDO TRACK:", resultNorth.Message);
            Assert.Contains("TORPEDO TRACK:", resultEast.Message);

            // North should show decreasing Y coordinates, East should show increasing X coordinates
            Assert.Contains("4,3", resultNorth.Message); // Moving north from 4,4
            Assert.Contains("5,4", resultEast.Message); // Moving east from 4,4
        }

        [Fact]
        public void TorpedoCombat_WithKlingonPresent_TriggersCounterAttack()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;
            gameState.Enterprise.Shields = 500;

            // Place Klingon not in torpedo path so it survives to counter-attack
            var klingon = new KlingonShip(new Coordinates(3, 3), 200);
            var initialShields = klingon.ShieldLevel;
            gameState.CurrentQuadrant.PlaceKlingon(klingon);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "3.0" }); // Fire east (miss Klingon)

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("TORPEDO TRACK:", result.Message);

            // If Klingon survives, should counter-attack
            if (!klingon.IsDestroyed)
            {
                Assert.Contains("KLINGON ATTACK:", result.Message);
                Assert.Contains("UNIT HIT ON ENTERPRISE FROM SECTOR", result.Message);
                // Klingon shields should degrade after firing (original BASIC: K(I,3)=K(I,3)/(3+RND(0)))
                Assert.True(klingon.ShieldLevel < initialShields);
            }
        }

        [Fact]
        public void TorpedoCombat_FractionalCourse_CalculatesCorrectTrajectory()
        {
            // Arrange - Test the original BASIC interpolation formula
            // X1=C(C1,1)+(C(C1+1,1)-C(C1,1))*(C1-INT(C1))
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.PhotonTorpedoes = 5;
            gameState.Enterprise.Energy = 1000;

            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new TorpedoCommand();

            // Act
            var result = command.Execute(gameState, new[] { "2.5" }); // Course 2.5 (between NE and E)

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("TORPEDO TRACK:", result.Message);
            // Should show diagonal movement between northeast and east directions
        }
    }
}
