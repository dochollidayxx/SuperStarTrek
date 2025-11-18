using System.Linq;
using Xunit;
using SuperStarTrek.Game.Commands;

namespace SuperStarTrek.Tests.Commands
{
    /// <summary>
    /// Tests for the CommandFactory
    /// </summary>
    public class CommandFactoryTests
    {
        [Fact]
        public void CreateCommand_WithValidSRSCommand_ReturnsShortRangeSensorsCommand()
        {
            // Arrange
            var factory = new CommandFactory();

            // Act
            var command = factory.CreateCommand("SRS");

            // Assert
            Assert.NotNull(command);
            Assert.IsType<ShortRangeSensorsCommand>(command);
        }

        [Fact]
        public void CreateCommand_WithValidLRSCommand_ReturnsLongRangeSensorsCommand()
        {
            // Arrange
            var factory = new CommandFactory();

            // Act
            var command = factory.CreateCommand("LRS");

            // Assert
            Assert.NotNull(command);
            Assert.IsType<LongRangeSensorsCommand>(command);
        }

        [Fact]
        public void CreateCommand_WithInvalidCommand_ReturnsNull()
        {
            // Arrange
            var factory = new CommandFactory();

            // Act
            var command = factory.CreateCommand("INVALID");

            // Assert
            Assert.Null(command);
        }

        [Fact]
        public void CreateCommand_CaseInsensitive_ReturnsCorrectCommand()
        {
            // Arrange
            var factory = new CommandFactory();

            // Act
            var command1 = factory.CreateCommand("srs");
            var command2 = factory.CreateCommand("SRS");
            var command3 = factory.CreateCommand("Srs");

            // Assert
            Assert.NotNull(command1);
            Assert.NotNull(command2);
            Assert.NotNull(command3);
            Assert.IsType<ShortRangeSensorsCommand>(command1);
            Assert.IsType<ShortRangeSensorsCommand>(command2);
            Assert.IsType<ShortRangeSensorsCommand>(command3);
        }

        [Fact]
        public void GetAvailableCommands_ReturnsImplementedCommands()
        {
            // Arrange
            var factory = new CommandFactory();

            // Act
            var commands = factory.GetAvailableCommands().ToList();

            // Assert
            Assert.Contains("SRS", commands);
            Assert.Contains("LRS", commands);
        }

        [Fact]
        public void GetCommandHelp_WithValidCommand_ReturnsHelpText()
        {
            // Arrange
            var factory = new CommandFactory();

            // Act
            var help = factory.GetCommandHelp("SRS");

            // Assert
            Assert.NotNull(help);
            Assert.Contains("SHORT RANGE SENSORS", help);
        }

        [Fact]
        public void GetAllCommandsHelp_ReturnsFormattedHelpText()
        {
            // Arrange
            var factory = new CommandFactory();

            // Act
            var help = factory.GetAllCommandsHelp();

            // Assert
            Assert.Contains("AVAILABLE COMMANDS:", help);
            Assert.Contains("SRS", help);
            Assert.Contains("LRS", help);
            Assert.Contains("XXX - TO RESIGN YOUR COMMAND", help);
            Assert.Contains("HELP or ? - THIS COMMAND LIST", help);
        }
    }
}
