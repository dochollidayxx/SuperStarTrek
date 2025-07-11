using System.Text;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Implements the Photon Torpedo (TOR) command for projectile-based combat.
    /// Corresponds to line 4690-5490 in the original BASIC implementation.
    /// </summary>
    public class TorpedoCommand : IGameCommand
    {
        public CommandResult Execute(GameState gameState, string[] parameters)
        {
            var enterprise = gameState.Enterprise;

            // Check if torpedoes are available (P <= 0 in BASIC)
            if (enterprise.PhotonTorpedoes <= 0)
            {
                return CommandResult.Failure("ALL PHOTON TORPEDOES EXPENDED");
            }

            // Check if photon tubes are operational (D(5) in BASIC)
            if (enterprise.GetSystemDamage(ShipSystem.PhotonTubes) < 0)
            {
                return CommandResult.Failure("PHOTON TUBES ARE NOT OPERATIONAL");
            }

            // Parse course parameter
            if (parameters.Length == 0)
            {
                return CommandResult.Failure("PHOTON TORPEDO COURSE REQUIRED (1-9)\nExample: TOR 1.5");
            }

            if (!double.TryParse(parameters[0], out double course))
            {
                return CommandResult.Failure("ENSIGN CHEKOV REPORTS,  'INCORRECT COURSE DATA, SIR!'");
            }

            // Validate course range (1-9, with 9 = 1)
            if (course == 9.0)
            {
                course = 1.0;
            }

            if (course < 1.0 || course >= 9.0)
            {
                return CommandResult.Failure("ENSIGN CHEKOV REPORTS,  'INCORRECT COURSE DATA, SIR!'");
            }

            // Execute torpedo firing
            return ExecuteTorpedoFire(gameState, course);
        }

        /// <summary>
        /// Executes the torpedo firing sequence with trajectory calculation
        /// </summary>
        private CommandResult ExecuteTorpedoFire(GameState gameState, double course)
        {
            var enterprise = gameState.Enterprise;
            var currentQuadrant = gameState.CurrentQuadrant;
            var result = new StringBuilder();

            // Consume torpedo and energy (original: E=E-2:P=P-1)
            enterprise.PhotonTorpedoes--;
            enterprise.Energy -= 2;

            // Calculate trajectory using original BASIC formula
            // X1=C(C1,1)+(C(C1+1,1)-C(C1,1))*(C1-INT(C1))
            // X2=C(C1,2)+(C(C1+1,2)-C(C1,2))*(C1-INT(C1))
            var courseInt = (int)course;
            var courseFrac = course - courseInt;

            var deltaX = GetCourseX(courseInt) + (GetCourseX(courseInt + 1) - GetCourseX(courseInt)) * courseFrac;
            var deltaY = GetCourseY(courseInt) + (GetCourseY(courseInt + 1) - GetCourseY(courseInt)) * courseFrac;

            // Starting position
            var currentX = (double)enterprise.SectorCoordinates.X;
            var currentY = (double)enterprise.SectorCoordinates.Y;

            result.AppendLine("TORPEDO TRACK:");

            // Track torpedo path
            while (true)
            {
                // Move torpedo one step
                currentX += deltaX;
                currentY += deltaY;

                // Convert to integer coordinates for checking
                var sectorX = (int)(currentX + 0.5);
                var sectorY = (int)(currentY + 0.5);

                // Check if torpedo left the quadrant
                if (sectorX < 1 || sectorX > 8 || sectorY < 1 || sectorY > 8)
                {
                    result.AppendLine("TORPEDO MISSED");
                    break;
                }

                result.AppendLine($"               {sectorX},{sectorY}");

                // Check what's at this position
                var hitTarget = CheckTorpedoHit(gameState, sectorX, sectorY, result);
                if (hitTarget)
                {
                    break;
                }
            }

            // Klingons return fire if any remain
            var remainingKlingons = currentQuadrant.KlingonShips.Where(k => !k.IsDestroyed).ToList();
            if (remainingKlingons.Any())
            {
                var counterAttackResult = ExecuteKlingonCounterAttack(gameState);
                result.Append(counterAttackResult);
            }

            return CommandResult.Success(result.ToString().TrimEnd(), 1.0);
        }

        /// <summary>
        /// Checks if torpedo hits something at the given coordinates
        /// </summary>
        private bool CheckTorpedoHit(GameState gameState, int sectorX, int sectorY, StringBuilder result)
        {
            var coordinates = new Coordinates(sectorX, sectorY);
            var currentQuadrant = gameState.CurrentQuadrant;
            var enterprise = gameState.Enterprise;

            // Check for empty space first
            var cellContents = currentQuadrant.GetSectorDisplay(coordinates);
            if (cellContents == "   ")
            {
                return false; // Continue tracking
            }

            // Check for Klingon hit
            var klingon = currentQuadrant.KlingonShips.FirstOrDefault(k =>
                k.SectorCoordinates.X == sectorX && k.SectorCoordinates.Y == sectorY && !k.IsDestroyed);

            if (klingon != null)
            {
                result.AppendLine("*** KLINGON DESTROYED ***");
                klingon.TakeDamage(klingon.ShieldLevel); // Destroy completely
                currentQuadrant.RemoveKlingon(klingon);
                gameState.Galaxy.RemoveKlingon(enterprise.QuadrantCoordinates);

                // Check for mission completion
                if (gameState.KlingonsRemaining <= 0)
                {
                    result.AppendLine("\n*** MISSION ACCOMPLISHED ***");
                }

                return true;
            }

            // Check for star hit
            if (cellContents == " * ")
            {
                result.AppendLine($"STAR AT {sectorX},{sectorY} ABSORBED TORPEDO ENERGY.");
                return true;
            }

            // Check for starbase hit
            if (cellContents == ">!<")
            {
                result.AppendLine("*** STARBASE DESTROYED ***");
                currentQuadrant.RemoveStarbase(coordinates);
                gameState.Galaxy.RemoveStarbase(enterprise.QuadrantCoordinates);

                // Check if this was the last starbase and mission is still ongoing
                if (gameState.Galaxy.TotalStarbases == 0 && gameState.KlingonsRemaining > 0)
                {
                    result.AppendLine("THAT DOES IT, CAPTAIN!!  YOU ARE HEREBY RELIEVED OF COMMAND");
                    result.AppendLine("AND SENTENCED TO 99 STARDATES AT HARD LABOR ON CYGNUS 12!!");
                }
                else if (gameState.Galaxy.TotalStarbases > 0)
                {
                    result.AppendLine("STARFLEET COMMAND REVIEWING YOUR RECORD TO CONSIDER");
                    result.AppendLine("COURT MARTIAL!");
                    enterprise.IsDocked = false; // No longer docked if starbase destroyed
                }

                return true;
            }

            return false;
        }

        /// <summary>
        /// Gets the X direction component for a given course (1-8)
        /// Based on the original BASIC C(course,1) array
        /// </summary>
        private double GetCourseX(int course)
        {
            return course switch
            {
                1 => 0,     // North
                2 => 1,     // Northeast  
                3 => 1,     // East
                4 => 1,     // Southeast
                5 => 0,     // South
                6 => -1,    // Southwest
                7 => -1,    // West
                8 => -1,    // Northwest
                9 => 0,     // North (wraps to 1)
                _ => 0
            };
        }

        /// <summary>
        /// Gets the Y direction component for a given course (1-8)
        /// Based on the original BASIC C(course,2) array
        /// </summary>
        private double GetCourseY(int course)
        {
            return course switch
            {
                1 => -1,    // North
                2 => -1,    // Northeast
                3 => 0,     // East
                4 => 1,     // Southeast
                5 => 1,     // South
                6 => 1,     // Southwest
                7 => 0,     // West
                8 => -1,    // Northwest
                9 => -1,    // North (wraps to 1)
                _ => 0
            };
        }

        /// <summary>
        /// Executes Klingon counter-attack after torpedo fire
        /// Matches original BASIC subroutine 6000 exactly
        /// </summary>
        private string ExecuteKlingonCounterAttack(GameState gameState)
        {
            var result = new StringBuilder();
            var enterprise = gameState.Enterprise;
            var currentQuadrant = gameState.CurrentQuadrant;
            var activeKlingons = currentQuadrant.KlingonShips.Where(k => !k.IsDestroyed).ToList();

            if (activeKlingons.Count == 0)
            {
                return string.Empty;
            }

            result.AppendLine("\nKLINGON ATTACK:");

            var totalDamage = 0;
            foreach (var klingon in activeKlingons)
            {
                var deltaX = klingon.SectorCoordinates.X - enterprise.SectorCoordinates.X;
                var deltaY = klingon.SectorCoordinates.Y - enterprise.SectorCoordinates.Y;
                var distance = Math.Sqrt(deltaX * deltaX + deltaY * deltaY);

                // Original BASIC formula: H=INT((K(I,3)/FND(1))*(2+RND(1)))
                var attackPower = (int)((klingon.ShieldLevel / distance) * (2 + gameState.Random.NextDouble()));
                totalDamage += attackPower;

                result.AppendLine($"{attackPower} UNIT HIT ON ENTERPRISE FROM SECTOR {klingon.SectorCoordinates.X},{klingon.SectorCoordinates.Y}");

                // Original BASIC: K(I,3)=K(I,3)/(3+RND(0)) - Klingon shields degrade after firing
                klingon.ShieldLevel = (int)(klingon.ShieldLevel / (3 + gameState.Random.NextDouble()));
            }

            // Apply damage to shields first, then hull
            if (totalDamage > 0)
            {
                if (enterprise.Shields >= totalDamage)
                {
                    enterprise.Shields -= totalDamage;
                    result.AppendLine($"      <SHIELDS DOWN TO {enterprise.Shields} UNITS>");

                    // Check for system damage on heavy hits (original BASIC logic)
                    if (totalDamage >= 20)
                    {
                        // Original BASIC: IF RND(1)>.6 OR H/S<=.02 THEN skip damage
                        if (gameState.Random.NextDouble() <= 0.6 && totalDamage / (double)enterprise.Shields > 0.02)
                        {
                            // Damage a random system: R1=FNR(1):D(R1)=D(R1)-H/S-.5*RND(1)
                            var systemIndex = gameState.Random.Next(1, 9); // 1-8 systems
                            var system = (ShipSystem)systemIndex;
                            var damageAmount = totalDamage / (double)enterprise.Shields + 0.5 * gameState.Random.NextDouble();

                            enterprise.SetSystemDamage(system, enterprise.GetSystemDamage(system) - damageAmount);
                            result.AppendLine($"DAMAGE CONTROL REPORTS '{GetSystemName(system)}' DAMAGED BY THE HIT'");
                        }
                    }
                }
                else
                {
                    var remainingDamage = totalDamage - enterprise.Shields;
                    enterprise.Shields = 0;
                    enterprise.Energy -= remainingDamage;

                    result.AppendLine("      <SHIELDS DOWN TO 0 UNITS>");
                    result.AppendLine($"HULL DAMAGE: {remainingDamage} UNITS");

                    if (enterprise.Energy <= 0)
                    {
                        result.AppendLine("\nTHE ENTERPRISE HAS BEEN DESTROYED. THE FEDERATION WILL BE CONQUERED");
                        // Game over logic would go here
                    }
                }
            }

            return result.ToString();
        }

        /// <summary>
        /// Gets the display name for a ship system for damage reports
        /// </summary>
        private string GetSystemName(ShipSystem system)
        {
            return system switch
            {
                ShipSystem.WarpEngines => "WARP ENGINES",
                ShipSystem.ShortRangeSensors => "SHORT RANGE SENSORS",
                ShipSystem.LongRangeSensors => "LONG RANGE SENSORS",
                ShipSystem.PhaserControl => "PHASER CONTROL",
                ShipSystem.PhotonTubes => "PHOTON TUBES",
                ShipSystem.DamageControl => "DAMAGE CONTROL",
                ShipSystem.ShieldControl => "SHIELD CONTROL",
                ShipSystem.LibraryComputer => "LIBRARY COMPUTER",
                _ => "UNKNOWN SYSTEM"
            };
        }

        public string GetHelpText()
        {
            return "TOR <course> - PHOTON TORPEDOES: Fire projectile weapons\n" +
                   "  Course: 1-8.999 (1=up, 3=right, 5=down, 7=left)\n" +
                   "  Torpedoes travel in straight line until hitting target";
        }
    }
}
