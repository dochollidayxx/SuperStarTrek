using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Implements the DAM (Damage Control) command from the original BASIC game.
    /// Handles damage reports, manual repairs, and starbase full repairs.
    /// Based on BASIC lines 5680-5980 and repair logic from 5810-5890.
    /// </summary>
    public class DamageControlCommand : IGameCommand
    {
        private readonly Random _random;

        public DamageControlCommand(Random random)
        {
            _random = random;
        }

        /// <summary>
        /// Executes the damage control command
        /// </summary>
        public CommandResult Execute(GameState gameState, string[] parameters)
        {
            var enterprise = gameState.Enterprise;
            double timeConsumed = 0;

            // Check if damage control system is operational (BASIC line 5690)
            if (!enterprise.IsSystemOperational(ShipSystem.DamageControl))
            {
                Console.WriteLine("DAMAGE CONTROL REPORT NOT AVAILABLE");

                // If docked, we can still show reports even with damaged damage control
                if (!enterprise.IsDocked)
                {
                    DisplayDamageReport(enterprise);
                    return CommandResult.Success();
                }
            }

            // Calculate total damage and repair time if any repairs are needed
            var damagedSystems = enterprise.GetDamagedSystems().ToList();

            if (damagedSystems.Any() && enterprise.IsDocked)
            {
                // Calculate repair time (BASIC lines 5720-5820)
                double totalDamage = 0;
                foreach (var system in damagedSystems)
                {
                    if (enterprise.GetSystemDamage(system) < 0)
                    {
                        totalDamage += 0.1; // Each damaged system adds 0.1 to repair time
                    }
                }

                // Add current damage factor (D4 in BASIC, representing accumulated damage)
                // In the original, D4 is set during quadrant entry with D4=.5*RND(1)
                double currentDamageFactor = 0.5 * _random.NextDouble();
                totalDamage += currentDamageFactor;

                // Cap at 0.9 stardates (BASIC line 5780)
                if (totalDamage >= 1.0)
                {
                    totalDamage = 0.9;
                }

                // Offer repair service
                Console.WriteLine();
                Console.WriteLine("TECHNICIANS STANDING BY TO EFFECT REPAIRS TO YOUR SHIP;");
                Console.WriteLine($"ESTIMATED TIME TO REPAIR: {totalDamage:F2} STARDATES");
                Console.Write("WILL YOU AUTHORIZE THE REPAIR ORDER (Y/N)? ");

                string response = Console.ReadLine()?.Trim().ToUpper() ?? "";

                if (response == "Y")
                {
                    // Repair all systems (BASIC lines 5870-5890)
                    foreach (var system in ShipSystemExtensions.GetAllSystems())
                    {
                        if (enterprise.GetSystemDamage(system) < 0)
                        {
                            enterprise.SetSystemDamage(system, 0.0);
                        }
                    }

                    // Set time consumed (will be applied by game engine)
                    timeConsumed = totalDamage + 0.1;

                    Console.WriteLine("REPAIRS COMPLETED.");
                }
            }

            // Display damage report (BASIC lines 5910-5950)
            DisplayDamageReport(enterprise);

            return CommandResult.Success(timeConsumed: timeConsumed);
        }

        /// <summary>
        /// Gets help text for the damage control command
        /// </summary>
        public string GetHelpText()
        {
            return "DAM  (FOR DAMAGE CONTROL REPORTS)";
        }

        /// <summary>
        /// Displays the current damage report for all ship systems
        /// </summary>
        private void DisplayDamageReport(Enterprise enterprise)
        {
            Console.WriteLine();
            Console.WriteLine("DEVICE             STATE OF REPAIR");

            foreach (var system in ShipSystemExtensions.GetAllSystems())
            {
                string deviceName = system.GetDisplayName();
                double damageLevel = enterprise.GetSystemDamage(system);

                // Format damage level to 2 decimal places (BASIC: INT(D(R1)*100)*.01)
                double formattedDamage = Math.Floor(damageLevel * 100) * 0.01;

                // Calculate spacing to align the damage values (original uses LEFT$(Z$,25-LEN(G2$)))
                int spacing = Math.Max(1, 25 - deviceName.Length);
                string spacer = new string(' ', spacing);

                Console.WriteLine($"{deviceName}{spacer}{formattedDamage:F2}");
            }

            Console.WriteLine();
        }
    }
}
