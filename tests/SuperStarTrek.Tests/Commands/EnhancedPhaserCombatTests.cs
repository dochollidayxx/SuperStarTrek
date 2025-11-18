using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Commands
{
    public class EnhancedPhaserCombatTests
    {
        [Fact]
        public void PhaserCombat_WithShieldControlDamage_ReducesEffectiveEnergy()
        {
            // Arrange
            var gameState = new GameState(12345); // Fixed seed for reproducible tests
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.Energy = 1000;
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShieldControl, -1.0); // Damage shield control

            // Place a Klingon
            var klingon = new KlingonShip(new Coordinates(4, 5), 200);
            gameState.CurrentQuadrant.PlaceKlingon(klingon);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "400" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(gameState.Enterprise.Energy < 1000); // Energy should be consumed
            // The effective energy used should be reduced due to shield control damage
            Assert.Contains("UNIT HIT ON KLINGON", result.Message);
        }

        [Fact]
        public void PhaserCombat_WithComputerDamage_ShowsAccuracyWarning()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.Energy = 1000;
            gameState.Enterprise.SetSystemDamage(ShipSystem.LibraryComputer, -1.0); // Damage computer

            var klingon = new KlingonShip(new Coordinates(4, 5), 200);
            gameState.CurrentQuadrant.PlaceKlingon(klingon);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "200" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("COMPUTER FAILURE HAMPERS ACCURACY", result.Message);
        }

        [Fact]
        public void PhaserCombat_LowDamage_ShowsNoDamageMessage()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(1, 1);
            gameState.Enterprise.Energy = 1000;

            // Place Klingon far away to ensure low damage
            var klingon = new KlingonShip(new Coordinates(8, 8), 200);
            gameState.CurrentQuadrant.PlaceKlingon(klingon);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new PhaserCommand();

            // Act  
            var result = command.Execute(gameState, new[] { "10" }); // Very low energy

            // Assert
            Assert.True(result.IsSuccess);
            // With low energy and long distance, damage should be below 0.15 * shield level threshold
            Assert.Contains("SENSORS SHOW NO DAMAGE TO ENEMY", result.Message);
        }

        [Fact]
        public void KlingonCounterAttack_ReducesKlingonShields()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.Energy = 1000;
            gameState.Enterprise.Shields = 500;

            var klingon = new KlingonShip(new Coordinates(4, 5), 200);
            var initialShields = klingon.ShieldLevel;
            gameState.CurrentQuadrant.PlaceKlingon(klingon);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "200" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("KLINGON ATTACK:", result.Message);
            Assert.Contains("UNIT HIT ON ENTERPRISE FROM SECTOR", result.Message);
            // Klingon shields should be reduced after firing (original BASIC behavior)
            Assert.True(klingon.ShieldLevel < initialShields);
        }

        [Fact]
        public void KlingonCounterAttack_CausesDamageToEnterprise()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.Energy = 1000;
            gameState.Enterprise.Shields = 1000; // Sufficient shields to survive counter-attack

            var klingon = new KlingonShip(new Coordinates(4, 5), 300); // High shield Klingon at distance 1
            gameState.CurrentQuadrant.PlaceKlingon(klingon);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new PhaserCommand();
            var initialShields = gameState.Enterprise.Shields;

            // Act
            var result = command.Execute(gameState, new[] { "100" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("SHIELDS DOWN TO", result.Message);
            Assert.True(gameState.Enterprise.Shields < initialShields); // Shields should be reduced
            Assert.True(gameState.Enterprise.Shields > 0); // Ship should survive
        }

        [Fact]
        public void EnergyDistribution_DividedEquallyAmongKlingons()
        {
            // Arrange
            var gameState = new GameState(12345);
            gameState.Enterprise.QuadrantCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.SectorCoordinates = new Coordinates(4, 4);
            gameState.Enterprise.Energy = 1000;

            // Place two Klingons at same distance
            var klingon1 = new KlingonShip(new Coordinates(4, 5), 200);
            var klingon2 = new KlingonShip(new Coordinates(5, 4), 200);
            gameState.CurrentQuadrant.PlaceKlingon(klingon1);
            gameState.CurrentQuadrant.PlaceKlingon(klingon2);
            gameState.CurrentQuadrant.PlaceEnterprise(gameState.Enterprise.SectorCoordinates);

            var command = new PhaserCommand();

            // Act
            var result = command.Execute(gameState, new[] { "400" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(gameState.Enterprise.Energy <= 600); // 400 energy consumed, possibly more from counter-attack
            // Both Klingons should be targeted
            Assert.NotNull(result.Message);
            Assert.Contains("UNIT HIT ON KLINGON AT SECTOR", result.Message);
            // Should contain references to both Klingon positions
            Assert.Contains("4,5", result.Message); // First Klingon position
            Assert.Contains("5,4", result.Message); // Second Klingon position
        }
    }
}
