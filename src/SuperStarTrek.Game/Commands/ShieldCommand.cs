using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Implements the SHE (Shield Control) command from the original BASIC game.
    /// Handles shield energy allocation, transfer between shields and main power,
    /// and prevents shield operation while docked at starbase.
    /// Based on BASIC lines 5520-5660 and docking logic from 6620.
    /// </summary>
    public class ShieldCommand : IGameCommand
    {
        /// <summary>
        /// Executes the shield control command
        /// </summary>
        public CommandResult Execute(GameState gameState, string[] parameters)
        {
            var enterprise = gameState.Enterprise;

            // Check if shield control system is operational (BASIC line 5530)
            if (!enterprise.IsSystemOperational(ShipSystem.ShieldControl))
            {
                return CommandResult.Failure("SHIELD CONTROL INOPERABLE");
            }

            // Check if docked - shields cannot be raised while docked
            if (enterprise.IsDocked)
            {
                return CommandResult.Failure("SHIELDS CANNOT BE RAISED WHILE DOCKED AT STARBASE\n" +
                    "SHIELDS REMAIN DOWN FOR DOCKING PURPOSES");
            }

            // If no parameters provided, just show current status
            if (parameters.Length == 0)
            {
                Console.WriteLine($"ENERGY AVAILABLE = {enterprise.Energy + enterprise.Shields}");
                Console.WriteLine($"CURRENT SHIELD LEVEL = {enterprise.Shields} UNITS");
                Console.Write("NUMBER OF UNITS TO SHIELDS");

                string? input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input))
                {
                    return CommandResult.Success("<SHIELDS UNCHANGED>");
                }

                parameters = new[] { input.Trim() };
            }

            // Parse energy amount to transfer to shields
            if (!int.TryParse(parameters[0], out int requestedShieldLevel))
            {
                return CommandResult.Failure("INVALID ENERGY AMOUNT. MUST BE A WHOLE NUMBER");
            }

            // Check for unchanged shields (BASIC line 5580)
            if (requestedShieldLevel < 0 || requestedShieldLevel == enterprise.Shields)
            {
                return CommandResult.Success("<SHIELDS UNCHANGED>");
            }

            // Check if we have enough total energy (BASIC line 5590)
            int totalAvailableEnergy = enterprise.Energy + enterprise.Shields;
            if (requestedShieldLevel > totalAvailableEnergy)
            {
                return CommandResult.Failure("SHIELD CONTROL REPORTS  'THIS IS NOT THE FEDERATION TREASURY.'\n" +
                    "<SHIELDS UNCHANGED>");
            }

            // Execute energy transfer (BASIC line 5630)
            int newEnergy = totalAvailableEnergy - requestedShieldLevel;
            enterprise.Energy = newEnergy;
            enterprise.Shields = requestedShieldLevel;

            // Report shield status (BASIC lines 5630-5660)
            return CommandResult.Success($"DEFLECTOR CONTROL ROOM REPORT:\n" +
                $"  'SHIELDS NOW AT {enterprise.Shields} UNITS PER YOUR COMMAND.'");
        }

        /// <summary>
        /// Gets help text for this command
        /// </summary>
        public string GetHelpText()
        {
            return "SHE - SHIELD CONTROL (TO RAISE OR LOWER SHIELDS)";
        }
    }
}
