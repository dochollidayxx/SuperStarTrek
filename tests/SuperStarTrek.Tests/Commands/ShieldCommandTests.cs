using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for the ShieldCommand class, verifying authentic BASIC behavior
    /// from lines 5520-5660 and docking logic from 6620.
    /// </summary>
    public class ShieldCommandTests
    {
        private readonly ShieldCommand _command;
        private readonly GameState _gameState;
        private readonly Enterprise _enterprise;

        public ShieldCommandTests()
        {
            _command = new ShieldCommand();
            _gameState = new GameState(42); // Fixed seed for reproducible tests
            _enterprise = _gameState.Enterprise;
        }

        [Fact]
        public void Execute_WhenShieldControlInoperable_ReturnsFailure()
        {
            // Arrange - damage shield control system
            _enterprise.SetSystemDamage(ShipSystem.ShieldControl, -1.0);

            // Act
            var result = _command.Execute(_gameState, new[] { "100" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("SHIELD CONTROL INOPERABLE", result.Message);
        }

        [Fact]
        public void Execute_WhenDocked_ReturnsFailure()
        {
            // Arrange - dock the ship
            _enterprise.IsDocked = true;

            // Act
            var result = _command.Execute(_gameState, new[] { "100" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("SHIELDS CANNOT BE RAISED WHILE DOCKED", result.Message!);
        }

        [Fact]
        public void Execute_WithNegativeShieldLevel_ReturnsUnchanged()
        {
            // Arrange
            _enterprise.Energy = 3000;
            _enterprise.Shields = 200;

            // Act
            var result = _command.Execute(_gameState, new[] { "-50" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("<SHIELDS UNCHANGED>", result.Message);
            Assert.Equal(3000, _enterprise.Energy);
            Assert.Equal(200, _enterprise.Shields);
        }

        [Fact]
        public void Execute_WithSameShieldLevel_ReturnsUnchanged()
        {
            // Arrange
            _enterprise.Energy = 2800;
            _enterprise.Shields = 200;

            // Act
            var result = _command.Execute(_gameState, new[] { "200" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("<SHIELDS UNCHANGED>", result.Message);
            Assert.Equal(2800, _enterprise.Energy);
            Assert.Equal(200, _enterprise.Shields);
        }

        [Fact]
        public void Execute_WithInsufficientEnergy_ReturnsFailure()
        {
            // Arrange
            _enterprise.Energy = 2000;
            _enterprise.Shields = 500;

            // Act - request more energy than available
            var result = _command.Execute(_gameState, new[] { "3000" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("THIS IS NOT THE FEDERATION TREASURY", result.Message!);
            Assert.Equal(2000, _enterprise.Energy); // Energy unchanged
            Assert.Equal(500, _enterprise.Shields); // Shields unchanged
        }

        [Fact]
        public void Execute_WithValidShieldIncrease_TransfersEnergyCorrectly()
        {
            // Arrange - BASIC line 5630: E=E+S-X:S=X
            _enterprise.Energy = 2500;
            _enterprise.Shields = 300;

            // Act - transfer 200 more units to shields (total 500)
            var result = _command.Execute(_gameState, new[] { "500" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("SHIELDS NOW AT 500 UNITS", result.Message!);
            Assert.Equal(2300, _enterprise.Energy); // 2500 + 300 - 500 = 2300
            Assert.Equal(500, _enterprise.Shields);
        }

        [Fact]
        public void Execute_WithValidShieldDecrease_TransfersEnergyCorrectly()
        {
            // Arrange
            _enterprise.Energy = 2200;
            _enterprise.Shields = 800;

            // Act - reduce shields to 300
            var result = _command.Execute(_gameState, new[] { "300" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("SHIELDS NOW AT 300 UNITS", result.Message!);
            Assert.Equal(2700, _enterprise.Energy); // 2200 + 800 - 300 = 2700
            Assert.Equal(300, _enterprise.Shields);
        }

        [Fact]
        public void Execute_WithZeroShields_RemovesAllShieldEnergy()
        {
            // Arrange
            _enterprise.Energy = 2000;
            _enterprise.Shields = 1000;

            // Act - remove all shields
            var result = _command.Execute(_gameState, new[] { "0" });

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Contains("SHIELDS NOW AT 0 UNITS", result.Message!);
            Assert.Equal(3000, _enterprise.Energy); // 2000 + 1000 - 0 = 3000
            Assert.Equal(0, _enterprise.Shields);
        }

        [Fact]
        public void Execute_WithInvalidInput_ReturnsFailure()
        {
            // Arrange
            _enterprise.Energy = 3000;
            _enterprise.Shields = 0;

            // Act
            var result = _command.Execute(_gameState, new[] { "abc" });

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("INVALID ENERGY AMOUNT. MUST BE A WHOLE NUMBER", result.Message);
        }

        [Fact]
        public void Execute_PreservesTotalEnergyBalance()
        {
            // Arrange
            _enterprise.Energy = 2400;
            _enterprise.Shields = 600;
            int totalEnergyBefore = _enterprise.Energy + _enterprise.Shields;

            // Act
            var result = _command.Execute(_gameState, new[] { "1000" });

            // Assert
            Assert.True(result.IsSuccess);
            int totalEnergyAfter = _enterprise.Energy + _enterprise.Shields;
            Assert.Equal(totalEnergyBefore, totalEnergyAfter);
            Assert.Equal(2000, _enterprise.Energy);
            Assert.Equal(1000, _enterprise.Shields);
        }

        [Fact]
        public void GetHelpText_ReturnsCorrectHelp()
        {
            // Act
            var helpText = _command.GetHelpText();

            // Assert
            Assert.Equal("SHE - SHIELD CONTROL (TO RAISE OR LOWER SHIELDS)", helpText);
        }

        [Fact]
        public void Execute_AuthenticBasicBehavior_MatchesOriginalFormula()
        {
            // Arrange - simulate exact BASIC conditions
            // Original: E=E+S-X:S=X where E=2500, S=400, X=700
            _enterprise.Energy = 2500;  // E in BASIC
            _enterprise.Shields = 400;  // S in BASIC

            // Act - X=700 in BASIC
            var result = _command.Execute(_gameState, new[] { "700" });

            // Assert - verify BASIC formula: E=E+S-X:S=X
            // E = 2500 + 400 - 700 = 2200
            // S = 700
            Assert.True(result.IsSuccess);
            Assert.Equal(2200, _enterprise.Energy);
            Assert.Equal(700, _enterprise.Shields);
            Assert.Contains("DEFLECTOR CONTROL ROOM REPORT:", result.Message!);
            Assert.Contains("'SHIELDS NOW AT 700 UNITS PER YOUR COMMAND.'", result.Message!);
        }
    }
}
