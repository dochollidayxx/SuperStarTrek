using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Factory for creating and managing game commands
    /// </summary>
    public class CommandFactory
    {
        private readonly Dictionary<string, Func<IGameCommand>> _commands;

        public CommandFactory()
        {
            _commands = new Dictionary<string, Func<IGameCommand>>(StringComparer.OrdinalIgnoreCase)
            {
                { "SRS", () => new ShortRangeSensorsCommand() },
                { "LRS", () => new LongRangeSensorsCommand() },
                { "NAV", () => new NavigationCommand() },
                { "PHA", () => new PhaserCommand() },
                { "TOR", () => new TorpedoCommand() },
                { "DAM", () => new DamageControlCommand(new Random()) },
                { "SHE", () => new ShieldCommand() },
                // Additional commands will be added in future weeks
                // { "COM", () => new ComputerCommand() }
            };
        }

        /// <summary>
        /// Creates a command instance for the given command name
        /// </summary>
        public IGameCommand? CreateCommand(string commandName)
        {
            if (_commands.TryGetValue(commandName, out var factory))
            {
                return factory();
            }
            return null;
        }

        /// <summary>
        /// Gets all available command names
        /// </summary>
        public IEnumerable<string> GetAvailableCommands()
        {
            return _commands.Keys;
        }

        /// <summary>
        /// Gets help text for a specific command
        /// </summary>
        public string? GetCommandHelp(string commandName)
        {
            var command = CreateCommand(commandName);
            return command?.GetHelpText();
        }

        /// <summary>
        /// Gets help text for all commands
        /// </summary>
        public string GetAllCommandsHelp()
        {
            var helpText = "AVAILABLE COMMANDS:\n";
            foreach (var commandName in _commands.Keys.OrderBy(x => x))
            {
                var command = CreateCommand(commandName);
                if (command != null)
                {
                    helpText += $"{command.GetHelpText()}\n";
                }
            }
            helpText += "XXX - TO RESIGN YOUR COMMAND\n";
            helpText += "HELP or ? - THIS COMMAND LIST";
            return helpText;
        }
    }
}
