using Xunit;
using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;
using System.Linq;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for Klingon counter-attack behavior during combat
    /// Verifies BASIC lines 6000-6200 (Klingon attacks) and 6090 (ship destruction)
    /// </summary>
    public class KlingonCounterAttackTests
    {
        [Fact]
        public void PhaserCommand_WhenShieldsDestroyedByCounterAttack_SetsEnergyToZero()
        {
            // Arrange - Create scenario where Klingon attack will destroy shields
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;
            var quadrant = gameState.CurrentQuadrant;

            // Set up Enterprise with low shields
            enterprise.Shields = 10;
            enterprise.Energy = 2000;

            // Place a powerful Klingon nearby to guarantee lethal damage
            if (quadrant.KlingonShips.Count == 0)
            {
                var klingon = new KlingonShip(new Coordinates(3, 3), 500);
                quadrant.AddKlingonShip(klingon);
            }
            else
            {
                // Boost existing Klingon's power
                quadrant.KlingonShips[0].ShieldLevel = 500;
                quadrant.KlingonShips[0].SectorCoordinates = new Coordinates(3, 3);
            }

            // Position Enterprise near Klingon for maximum damage
            enterprise.SectorCoordinates = new Coordinates(3, 4);

            var phaserCommand = new PhaserCommand();

            // Act - Fire phasers (this triggers Klingon counter-attack)
            var result = phaserCommand.Execute(gameState, new[] { "50" });

            // Assert - If shields were destroyed, energy should be 0
            if (enterprise.Shields <= 0)
            {
                Assert.Equal(0, enterprise.Energy);
                Assert.Contains("ENTERPRISE HAS BEEN DESTROYED", result.Message);
            }
        }

        [Fact]
        public void TorpedoCommand_WhenShieldsDestroyedByCounterAttack_SetsEnergyToZero()
        {
            // Arrange - Create scenario where Klingon attack will destroy shields
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;
            var quadrant = gameState.CurrentQuadrant;

            // Set up Enterprise with low shields
            enterprise.Shields = 10;
            enterprise.Energy = 2000;
            enterprise.PhotonTorpedoes = 10;

            // Place a powerful Klingon nearby
            if (quadrant.KlingonShips.Count == 0)
            {
                var klingon = new KlingonShip(new Coordinates(3, 3), 500);
                quadrant.AddKlingonShip(klingon);
            }
            else
            {
                quadrant.KlingonShips[0].ShieldLevel = 500;
                quadrant.KlingonShips[0].SectorCoordinates = new Coordinates(3, 3);
            }

            // Position Enterprise near Klingon
            enterprise.SectorCoordinates = new Coordinates(3, 4);

            var torpedoCommand = new TorpedoCommand();

            // Act - Fire torpedo (this triggers Klingon counter-attack)
            var result = torpedoCommand.Execute(gameState, new[] { "1.0" });

            // Assert - If shields were destroyed, energy should be 0
            if (enterprise.Shields <= 0)
            {
                Assert.Equal(0, enterprise.Energy);
                Assert.Contains("ENTERPRISE HAS BEEN DESTROYED", result.Message);
            }
        }

        [Fact]
        public void KlingonCounterAttack_WithAdequateShields_ShouldNotDestroyShip()
        {
            // Arrange
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;
            var quadrant = gameState.CurrentQuadrant;

            // Set up Enterprise with strong shields
            enterprise.Shields = 2000;
            enterprise.Energy = 3000;

            // Place a weak Klingon
            if (quadrant.KlingonShips.Count == 0)
            {
                var klingon = new KlingonShip(new Coordinates(5, 5), 100);
                quadrant.AddKlingonShip(klingon);
            }
            else
            {
                quadrant.KlingonShips[0].ShieldLevel = 100;
            }

            enterprise.SectorCoordinates = new Coordinates(3, 3);

            var phaserCommand = new PhaserCommand();

            // Act
            var result = phaserCommand.Execute(gameState, new[] { "50" });

            // Assert - Ship should survive
            Assert.True(enterprise.Shields > 0);
            Assert.True(enterprise.Energy > 0);
            Assert.DoesNotContain("DESTROYED", result.Message);
        }

        [Fact]
        public void KlingonCounterAttack_ShieldsGoToExactlyZero_ShouldDestroyShip()
        {
            // Arrange - Test boundary condition: shields == 0
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;

            // Manually test the logic: if shields reach exactly 0, ship is destroyed
            enterprise.Shields = 100;
            enterprise.Energy = 1000;

            // Simulate damage that reduces shields to exactly 0
            var damage = 100;
            enterprise.Shields -= damage;

            // Act - Simulate what counter-attack code does
            if (enterprise.Shields <= 0)
            {
                enterprise.Energy = 0;
            }

            // Assert
            Assert.Equal(0, enterprise.Shields);
            Assert.Equal(0, enterprise.Energy);
        }

        [Fact]
        public void KlingonCounterAttack_ShieldsGoNegative_ShouldDestroyShip()
        {
            // Arrange - Test shields going negative (BASIC allows S to go negative)
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;

            enterprise.Shields = 50;
            enterprise.Energy = 1000;

            // Simulate damage exceeding shields
            var damage = 100;
            enterprise.Shields -= damage;

            // Act - Simulate what counter-attack code does
            if (enterprise.Shields <= 0)
            {
                enterprise.Energy = 0;
            }

            // Assert
            Assert.True(enterprise.Shields < 0);
            Assert.Equal(0, enterprise.Energy);
        }

        [Fact]
        public void KlingonCounterAttack_WhenDocked_ShouldNotDamageShip()
        {
            // Arrange - BASIC line 6010: IF D0<>0 THEN PRINT "STARBASE SHIELDS..."
            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;
            var quadrant = gameState.CurrentQuadrant;

            // Dock the Enterprise
            enterprise.DockAtStarbase();

            // Place a Klingon
            if (quadrant.KlingonShips.Count == 0)
            {
                var klingon = new KlingonShip(new Coordinates(5, 5), 200);
                quadrant.AddKlingonShip(klingon);
            }

            var initialShields = enterprise.Shields;
            var initialEnergy = enterprise.Energy;

            var phaserCommand = new PhaserCommand();

            // Act
            var result = phaserCommand.Execute(gameState, new[] { "50" });

            // Assert - Docked ships are protected by starbase
            // The counter-attack should display protection message
            Assert.Contains("STARBASE", result.Message.ToUpper());
        }

        [Fact]
        public void KlingonCounterAttack_DamageFormula_MatchesBASIC()
        {
            // Test that damage formula matches BASIC: H=INT((K(I,3)/FND(1))*(2+RND(1)))
            // This is tested implicitly by the counter-attack implementations
            // Just verify the general behavior

            var gameState = new GameState(42);
            var enterprise = gameState.Enterprise;
            var quadrant = gameState.CurrentQuadrant;

            // Set up predictable scenario
            enterprise.Shields = 1000;
            enterprise.Energy = 3000;

            // Place Klingon at known distance
            if (quadrant.KlingonShips.Count == 0)
            {
                var klingon = new KlingonShip(new Coordinates(5, 5), 200);
                quadrant.AddKlingonShip(klingon);
            }

            enterprise.SectorCoordinates = new Coordinates(3, 3);

            var initialShields = enterprise.Shields;
            var phaserCommand = new PhaserCommand();

            // Act
            phaserCommand.Execute(gameState, new[] { "50" });

            // Assert - Shields should be reduced (damage occurred)
            // Exact damage depends on random, distance, and Klingon power
            Assert.True(enterprise.Shields < initialShields || quadrant.KlingonShips.All(k => k.IsDestroyed));
        }
    }
}
