using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Represents the result of executing a game command
    /// </summary>
    public class CommandResult
    {
        /// <summary>
        /// Whether the command executed successfully
        /// </summary>
        public bool IsSuccess { get; set; }

        /// <summary>
        /// Message to display to the player
        /// </summary>
        public string? Message { get; set; }

        /// <summary>
        /// Whether the command should consume game time
        /// </summary>
        public bool ConsumesTime { get; set; }

        /// <summary>
        /// Amount of time to advance (in stardates)
        /// </summary>
        public double TimeConsumed { get; set; }

        /// <summary>
        /// Creates a successful result
        /// </summary>
        public static CommandResult Success(string? message = null, double timeConsumed = 0)
        {
            return new CommandResult
            {
                IsSuccess = true,
                Message = message,
                ConsumesTime = timeConsumed > 0,
                TimeConsumed = timeConsumed
            };
        }

        /// <summary>
        /// Creates a failure result
        /// </summary>
        public static CommandResult Failure(string message)
        {
            return new CommandResult
            {
                IsSuccess = false,
                Message = message,
                ConsumesTime = false,
                TimeConsumed = 0
            };
        }
    }

    /// <summary>
    /// Interface for all game commands
    /// </summary>
    public interface IGameCommand
    {
        /// <summary>
        /// Executes the command with the given parameters
        /// </summary>
        /// <param name="gameState">Current game state</param>
        /// <param name="parameters">Command parameters from user input</param>
        /// <returns>Result of command execution</returns>
        CommandResult Execute(GameState gameState, string[] parameters);

        /// <summary>
        /// Gets help text for this command
        /// </summary>
        string GetHelpText();
    }
}
