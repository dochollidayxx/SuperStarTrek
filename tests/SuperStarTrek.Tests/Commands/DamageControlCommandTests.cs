using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;
using Xunit;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for the DamageControlCommand implementation
    /// Verifies authentic BASIC behavior from lines 5680-5980
    /// </summary>
    public class DamageControlCommandTests
    {
        private readonly Random _random;
        private readonly DamageControlCommand _command;

        public DamageControlCommandTests()
        {
            _random = new Random(42); // Fixed seed for reproducible tests
            _command = new DamageControlCommand(_random);
        }

        [Fact]
        public void Execute_WithDamageControlInoperable_ShowsUnavailableMessage()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.SetSystemDamage(ShipSystem.DamageControl, -1.0);
            gameState.Enterprise.IsDocked = false;

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            var output = consoleOutput.ToString();
            Assert.Contains("DAMAGE CONTROL REPORT NOT AVAILABLE", output);
        }

        [Fact]
        public void Execute_WithNoDamage_ShowsAllSystemsOperational()
        {
            // Arrange
            var gameState = CreateTestGameState();

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            var output = consoleOutput.ToString();
            Assert.Contains("DEVICE             STATE OF REPAIR", output);
            Assert.Contains("WARP ENGINES", output);
            Assert.Contains("0.00", output); // All systems should show 0.00 damage
        }

        [Fact]
        public void Execute_WithDamageAndDocked_OffersRepairService()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = true;
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.5);
            gameState.Enterprise.SetSystemDamage(ShipSystem.PhaserControl, -1.8);

            using var consoleOutput = new StringWriter();
            using var consoleInput = new StringReader("N\n");
            Console.SetOut(consoleOutput);
            Console.SetIn(consoleInput);

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            var output = consoleOutput.ToString();
            Assert.Contains("TECHNICIANS STANDING BY TO EFFECT REPAIRS TO YOUR SHIP;", output);
            Assert.Contains("ESTIMATED TIME TO REPAIR:", output);
            Assert.Contains("WILL YOU AUTHORIZE THE REPAIR ORDER (Y/N)?", output);
        }

        [Fact]
        public void Execute_AcceptRepairs_RepairsAllSystemsAndAdvancesTime()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = true;
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.5);
            gameState.Enterprise.SetSystemDamage(ShipSystem.PhaserControl, -1.8);

            using var consoleOutput = new StringWriter();
            using var consoleInput = new StringReader("Y\n");
            Console.SetOut(consoleOutput);
            Console.SetIn(consoleInput);

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.True(result.ConsumesTime);
            Assert.True(result.TimeConsumed > 0);
            Assert.Equal(0.0, gameState.Enterprise.GetSystemDamage(ShipSystem.WarpEngines));
            Assert.Equal(0.0, gameState.Enterprise.GetSystemDamage(ShipSystem.PhaserControl));

            var output = consoleOutput.ToString();
            Assert.Contains("REPAIRS COMPLETED.", output);
        }

        [Fact]
        public void Execute_DeclineRepairs_LeavesSystemsDamaged()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = true;
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.5);

            using var consoleOutput = new StringWriter();
            using var consoleInput = new StringReader("N\n");
            Console.SetOut(consoleOutput);
            Console.SetIn(consoleInput);

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            Assert.False(result.ConsumesTime);
            Assert.Equal(0, result.TimeConsumed);
            Assert.Equal(-2.5, gameState.Enterprise.GetSystemDamage(ShipSystem.WarpEngines));
        }

        [Fact]
        public void Execute_DamageControlInoperableButDocked_StillShowsReport()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.IsDocked = true;
            gameState.Enterprise.SetSystemDamage(ShipSystem.DamageControl, -1.0);
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.0);

            using var consoleOutput = new StringWriter();
            using var consoleInput = new StringReader("Y\n");
            Console.SetOut(consoleOutput);
            Console.SetIn(consoleInput);

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            var output = consoleOutput.ToString();
            Assert.Contains("DAMAGE CONTROL REPORT NOT AVAILABLE", output);
            Assert.Contains("TECHNICIANS STANDING BY", output); // Should still offer repairs when docked
        }

        [Fact]
        public void DisplayDamageReport_FormatsSystemNamesAndDamageCorrectly()
        {
            // Arrange
            var gameState = CreateTestGameState();
            gameState.Enterprise.SetSystemDamage(ShipSystem.WarpEngines, -2.55);
            gameState.Enterprise.SetSystemDamage(ShipSystem.ShortRangeSensors, 1.33);
            gameState.Enterprise.SetSystemDamage(ShipSystem.LibraryComputer, -0.05);

            using var consoleOutput = new StringWriter();
            Console.SetOut(consoleOutput);

            // Act
            var result = _command.Execute(gameState, Array.Empty<string>());

            // Assert
            Assert.True(result.IsSuccess);
            var output = consoleOutput.ToString();
            Assert.Contains("DEVICE             STATE OF REPAIR", output);
            Assert.Contains("WARP ENGINES", output);
            Assert.Contains("-2.55", output);
            Assert.Contains("SHORT RANGE SENSORS", output);
            Assert.Contains("1.33", output);
            Assert.Contains("LIBRARY-COMPUTER", output);
            Assert.Contains("-0.05", output);
        }

        [Fact]
        public void GetHelpText_ReturnsCorrectHelpText()
        {
            // Act
            string helpText = _command.GetHelpText();

            // Assert
            Assert.Equal("DAM  (FOR DAMAGE CONTROL REPORTS)", helpText);
        }

        private GameState CreateTestGameState()
        {
            return new GameState(seed: 42);
        }
    }
}
