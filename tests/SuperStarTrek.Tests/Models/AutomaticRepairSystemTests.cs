using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Models
{
    /// <summary>
    /// Tests for the automatic repair system in the Enterprise class
    /// Verifies authentic BASIC behavior from lines 2770-3030
    /// </summary>
    public class AutomaticRepairSystemTests
    {
        private readonly Random _random;
        private readonly Enterprise _enterprise;

        public AutomaticRepairSystemTests()
        {
            _random = new Random(42); // Fixed seed for reproducible tests
            _enterprise = new Enterprise();
        }

        [Fact]
        public void PerformAutomaticRepairs_WithHighWarpFactor_RepairsAtFullRate()
        {
            // Arrange - Damage several systems
            _enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.5);
            _enterprise.SetSystemDamage(ShipSystem.PhaserControl, -1.8);

            // Act - High warp factor (>= 1.0) gives full repair amount
            var messages = _enterprise.PerformAutomaticRepairs(8.0, _random);

            // Assert - Each system should get 1.0 repair amount
            Assert.Equal(-1.5, _enterprise.GetSystemDamage(ShipSystem.WarpEngines)); // -2.5 + 1.0
            Assert.Equal(-0.8, _enterprise.GetSystemDamage(ShipSystem.PhaserControl)); // -1.8 + 1.0
        }

        [Fact]
        public void PerformAutomaticRepairs_WithLowWarpFactor_RepairsAtPartialRate()
        {
            // Arrange - Damage a system
            _enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.0);

            // Act - Low warp factor gives proportional repair
            var messages = _enterprise.PerformAutomaticRepairs(0.5, _random);

            // Assert - Should get 0.5 repair amount
            Assert.Equal(-1.5, _enterprise.GetSystemDamage(ShipSystem.WarpEngines)); // -2.0 + 0.5
        }

        [Fact]
        public void PerformAutomaticRepairs_SystemFullyRepaired_ReportsCompletion()
        {
            // Arrange - System with minor damage that will be fully repaired
            _enterprise.SetSystemDamage(ShipSystem.WarpEngines, -0.8);

            // Act
            var messages = _enterprise.PerformAutomaticRepairs(1.0, _random);

            // Assert
            Assert.Equal(0.0, _enterprise.GetSystemDamage(ShipSystem.WarpEngines));
            Assert.Contains("DAMAGE CONTROL REPORT:", messages);
            Assert.Contains("        WARP ENGINES REPAIR COMPLETED.", messages);
        }

        [Fact]
        public void PerformAutomaticRepairs_PartialRepairNearZero_SetsToMinorDamage()
        {
            // Arrange - System with damage that would repair to between -0.1 and 0
            // If we start with -0.05 and apply 0.02 repair, we get -0.03 (between -0.1 and 0)
            _enterprise.SetSystemDamage(ShipSystem.WarpEngines, -0.05);

            // Act - Use low warp factor that results in repair between -0.1 and 0
            var messages = _enterprise.PerformAutomaticRepairs(0.02, _random);

            // Assert - Should be set to -0.1 per BASIC logic
            Assert.Equal(-0.1, _enterprise.GetSystemDamage(ShipSystem.WarpEngines));
        }

        [Fact]
        public void PerformAutomaticRepairs_UndamagedSystems_NoChangeOccurs()
        {
            // Arrange - All systems operational
            foreach (var system in ShipSystemExtensions.GetAllSystems())
            {
                _enterprise.SetSystemDamage(system, 0.0);
            }

            // Act
            var messages = _enterprise.PerformAutomaticRepairs(1.0, _random);

            // Assert - No systems should change
            foreach (var system in ShipSystemExtensions.GetAllSystems())
            {
                Assert.Equal(0.0, _enterprise.GetSystemDamage(system));
            }
        }

        [Fact]
        public void PerformAutomaticRepairs_RandomDamageEvent_OccursWithCorrectProbability()
        {
            // Arrange - Use a controlled random that will trigger the 20% chance
            var controlledRandom = new Random(123); // Seed that produces damage event
            _enterprise.SetSystemDamage(ShipSystem.WarpEngines, -1.0);

            // Act - Multiple runs to check for random events
            bool eventOccurred = false;
            for (int i = 0; i < 50; i++)
            {
                var newEnterprise = new Enterprise();
                newEnterprise.SetSystemDamage(ShipSystem.WarpEngines, -1.0);
                var messages = newEnterprise.PerformAutomaticRepairs(1.0, new Random(i));

                if (messages.Any(m => m.Contains("DAMAGED") || m.Contains("STATE OF REPAIR IMPROVED")))
                {
                    eventOccurred = true;
                    break;
                }
            }

            // Assert - Should eventually see random damage/improvement events
            Assert.True(eventOccurred, "Random damage/improvement events should occur during automatic repairs");
        }

        [Fact]
        public void ApplyCombatDamage_WithSignificantHit_DamagesRandomSystem()
        {
            // Arrange
            int hitStrength = 50;
            int shieldLevel = 100;
            // Use a random that won't trigger the skip conditions
            var controlledRandom = new Random(123);

            // Act
            string? damagedSystem = _enterprise.ApplyCombatDamage(hitStrength, shieldLevel, controlledRandom);

            // Assert - Try multiple times with different randoms if needed
            bool damageOccurred = false;
            for (int i = 0; i < 10; i++)
            {
                var testEnterprise = new Enterprise();
                var testRandom = new Random(i + 100); // Different seeds to avoid skip conditions
                var result = testEnterprise.ApplyCombatDamage(50, 100, testRandom);
                if (result != null)
                {
                    damageOccurred = true;
                    break;
                }
            }

            Assert.True(damageOccurred, "Combat damage should occur with appropriate random values");
        }

        [Fact]
        public void ApplyCombatDamage_WithWeakHit_NoDamageOccurs()
        {
            // Arrange - Hit strength below threshold
            int hitStrength = 15; // Below 20 threshold
            int shieldLevel = 100;

            // Act
            string? damagedSystem = _enterprise.ApplyCombatDamage(hitStrength, shieldLevel, _random);

            // Assert
            Assert.Null(damagedSystem); // No damage should occur

            // Verify no systems were damaged
            foreach (var system in ShipSystemExtensions.GetAllSystems())
            {
                Assert.Equal(0.0, _enterprise.GetSystemDamage(system));
            }
        }

        [Fact]
        public void ApplyCombatDamage_WithHighShieldsLowDamageRatio_NoDamageOccurs()
        {
            // Arrange - Damage ratio is very low (H/S <= 0.02)
            int hitStrength = 20;
            int shieldLevel = 2000; // Ratio = 20/2000 = 0.01 <= 0.02

            // Act
            string? damagedSystem = _enterprise.ApplyCombatDamage(hitStrength, shieldLevel, _random);

            // Assert
            Assert.Null(damagedSystem); // No damage should occur due to high shields
        }

        [Fact]
        public void ApplyCombatDamage_WithNoShields_CalculatesDamageCorrectly()
        {
            // Arrange
            int hitStrength = 100;
            int shieldLevel = 0; // No shields

            // Act - Try multiple times to get past probability checks
            bool damageOccurred = false;
            for (int i = 0; i < 10; i++)
            {
                var testEnterprise = new Enterprise();
                var testRandom = new Random(i + 200); // Different seeds
                var result = testEnterprise.ApplyCombatDamage(hitStrength, shieldLevel, testRandom);
                if (result != null)
                {
                    damageOccurred = true;

                    // With no shields, damage should be significant
                    bool significantDamageOccurred = ShipSystemExtensions.GetAllSystems()
                        .Any(s => testEnterprise.GetSystemDamage(s) < -50);
                    Assert.True(significantDamageOccurred, "Significant damage should occur when shields are down");
                    break;
                }
            }

            Assert.True(damageOccurred, "Combat damage should eventually occur with no shields");
        }

        [Fact]
        public void GetDamagedSystems_ReturnsOnlyDamagedSystems()
        {
            // Arrange
            _enterprise.SetSystemDamage(ShipSystem.WarpEngines, -1.5);
            _enterprise.SetSystemDamage(ShipSystem.PhaserControl, -2.0);
            _enterprise.SetSystemDamage(ShipSystem.ShortRangeSensors, 0.0); // Not damaged
            _enterprise.SetSystemDamage(ShipSystem.LongRangeSensors, 1.0); // Good repair

            // Act
            var damagedSystems = _enterprise.GetDamagedSystems().ToList();

            // Assert
            Assert.Equal(2, damagedSystems.Count);
            Assert.Contains(ShipSystem.WarpEngines, damagedSystems);
            Assert.Contains(ShipSystem.PhaserControl, damagedSystems);
            Assert.DoesNotContain(ShipSystem.ShortRangeSensors, damagedSystems);
            Assert.DoesNotContain(ShipSystem.LongRangeSensors, damagedSystems);
        }
    }
}
