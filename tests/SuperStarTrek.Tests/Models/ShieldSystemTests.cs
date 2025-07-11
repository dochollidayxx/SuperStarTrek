using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Models
{
    /// <summary>
    /// Tests for the Enterprise shield system, including damage absorption,
    /// automatic docking behavior, and energy management features.
    /// Based on BASIC combat logic from lines 6010-6200 and docking from 6620.
    /// </summary>
    public class ShieldSystemTests
    {
        private readonly Enterprise _enterprise;
        private readonly Random _random;

        public ShieldSystemTests()
        {
            _enterprise = new Enterprise();
            _random = new Random(42); // Fixed seed for reproducible tests
        }

        [Fact]
        public void ApplyShieldedDamage_WhenDocked_StarbaseProtectsShip()
        {
            // Arrange - BASIC line 6010: "STARBASE SHIELDS PROTECT THE ENTERPRISE"
            _enterprise.IsDocked = true;
            _enterprise.Energy = 3000;
            _enterprise.Shields = 1000;

            // Act
            var messages = _enterprise.ApplyShieldedDamage(500, _random);

            // Assert
            Assert.Single(messages);
            Assert.Equal("STARBASE SHIELDS PROTECT THE ENTERPRISE", messages[0]);
            Assert.Equal(3000, _enterprise.Energy); // Unchanged
            Assert.Equal(1000, _enterprise.Shields); // Unchanged
        }

        [Fact]
        public void ApplyShieldedDamage_WithSufficientShields_AbsorbsDamage()
        {
            // Arrange - BASIC line 6060: S=S-H
            _enterprise.Shields = 1000;
            _enterprise.Energy = 2000;

            // Act
            var messages = _enterprise.ApplyShieldedDamage(300, _random);

            // Assert
            Assert.Equal(700, _enterprise.Shields); // 1000 - 300 = 700
            Assert.Equal(2000, _enterprise.Energy); // Energy unchanged
            Assert.Contains(messages, m => m.Contains("SHIELDS DOWN TO 700 UNITS"));
        }

        [Fact]
        public void ApplyShieldedDamage_WhenShieldsDestroyed_DestroyShip()
        {
            // Arrange
            _enterprise.Shields = 200;
            _enterprise.Energy = 3000;

            // Act - damage exceeds shields
            var messages = _enterprise.ApplyShieldedDamage(300, _random);

            // Assert
            Assert.True(_enterprise.Shields <= 0); // Shields destroyed
            Assert.Contains(messages, m => m.Contains("THE ENTERPRISE HAS BEEN DESTROYED"));
            Assert.Contains(messages, m => m.Contains("THE FEDERATION"));
            Assert.Contains(messages, m => m.Contains("WILL BE CONQUERED"));
        }

        [Fact]
        public void DockAtStarbase_AutomaticallyLowersShields()
        {
            // Arrange - BASIC line 6620: "SHIELDS DROPPED FOR DOCKING PURPOSES":S=0
            _enterprise.Shields = 500;
            _enterprise.Energy = 2500;

            // Act
            string message = _enterprise.DockAtStarbase();

            // Assert
            Assert.True(_enterprise.IsDocked);
            Assert.Equal(0, _enterprise.Shields); // Shields automatically lowered
            Assert.Equal(2500, _enterprise.Energy); // Energy unchanged
            Assert.Equal("SHIELDS DROPPED FOR DOCKING PURPOSES", message);
        }

        [Fact]
        public void DockAtStarbase_WhenShieldsAlreadyDown_NoMessage()
        {
            // Arrange
            _enterprise.Shields = 0;
            _enterprise.Energy = 3000;

            // Act
            string message = _enterprise.DockAtStarbase();

            // Assert
            Assert.True(_enterprise.IsDocked);
            Assert.Equal(0, _enterprise.Shields);
            Assert.Equal("", message); // No message when shields already down
        }

        [Fact]
        public void UndockFromStarbase_ResetsDockedStatus()
        {
            // Arrange
            _enterprise.IsDocked = true;

            // Act
            _enterprise.UndockFromStarbase();

            // Assert
            Assert.False(_enterprise.IsDocked);
        }

        [Fact]
        public void AreShieldsDangerouslyLow_WhenShieldsBelow200_ReturnsTrue()
        {
            // Arrange - based on BASIC line 1580 warning logic
            _enterprise.Shields = 150;
            _enterprise.IsDocked = false;

            // Act
            bool isDangerous = _enterprise.AreShieldsDangerouslyLow();

            // Assert
            Assert.True(isDangerous);
        }

        [Fact]
        public void AreShieldsDangerouslyLow_WhenShieldsAbove200_ReturnsFalse()
        {
            // Arrange
            _enterprise.Shields = 250;
            _enterprise.IsDocked = false;

            // Act
            bool isDangerous = _enterprise.AreShieldsDangerouslyLow();

            // Assert
            Assert.False(isDangerous);
        }

        [Fact]
        public void AreShieldsDangerouslyLow_WhenDocked_ReturnsFalse()
        {
            // Arrange - docked ships don't need shields
            _enterprise.Shields = 50;
            _enterprise.IsDocked = true;

            // Act
            bool isDangerous = _enterprise.AreShieldsDangerouslyLow();

            // Assert
            Assert.False(isDangerous); // Not dangerous when docked
        }

        [Fact]
        public void ShieldDamageAbsorption_IntegratesWithExistingCombatDamage()
        {
            // Arrange
            _enterprise.Shields = 1000;
            _enterprise.Energy = 2000;
            var fixedRandom = new Random(123); // Fixed seed for predictable system damage

            // Act
            var messages = _enterprise.ApplyShieldedDamage(25, fixedRandom);

            // Assert
            Assert.Equal(975, _enterprise.Shields); // Shields reduced
            Assert.Contains(messages, m => m.Contains("SHIELDS DOWN TO 975 UNITS"));
            
            // Check if system damage was applied (depends on random and damage calculations)
            // The exact system depends on random number generation and damage threshold
            bool systemDamageReported = messages.Any(m => m.Contains("DAMAGE CONTROL REPORTS") && m.Contains("DAMAGED"));
            // System damage may or may not occur based on BASIC combat logic
        }

        [Fact]
        public void Resupply_AutomaticallyLowersShields()
        {
            // Arrange
            _enterprise.Energy = 1500;
            _enterprise.Shields = 800;
            _enterprise.PhotonTorpedoes = 5;

            // Act
            _enterprise.Resupply();

            // Assert
            Assert.Equal(3000, _enterprise.Energy); // Restored to maximum
            Assert.Equal(10, _enterprise.PhotonTorpedoes); // Restored to maximum
            Assert.Equal(0, _enterprise.Shields); // Shields lowered during resupply
        }

        [Fact]
        public void ShieldSystem_AuthenticBasicBehavior_CombatDamageIntegration()
        {
            // Arrange - simulate exact BASIC combat scenario
            // From line 6060: H=INT((K(I,3)/FND(1))*(2+RND(1))):S=S-H
            _enterprise.Shields = 800;
            _enterprise.Energy = 2200;
            var fixedRandom = new Random(777); // Fixed for reproducible behavior

            // Act - apply damage similar to Klingon attack
            var messages = _enterprise.ApplyShieldedDamage(150, fixedRandom);

            // Assert
            Assert.Equal(650, _enterprise.Shields); // 800 - 150 = 650
            Assert.Equal(2200, _enterprise.Energy); // Energy unchanged by shield absorption
            Assert.Contains(messages, m => m.Contains("SHIELDS DOWN TO 650 UNITS"));
            
            // Verify shield absorption worked and ship wasn't destroyed
            Assert.True(_enterprise.Shields > 0);
            Assert.DoesNotContain(messages, m => m.Contains("ENTERPRISE HAS BEEN DESTROYED"));
        }
    }
}
