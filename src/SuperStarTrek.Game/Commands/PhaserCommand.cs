using System.Text;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Implements the Phaser (PHA) command for energy-based combat.
    /// Corresponds to line 4250-4690 in the original BASIC implementation.
    /// </summary>
    public class PhaserCommand : IGameCommand
    {
        public CommandResult Execute(GameState gameState, string[] parameters)
        {
            var enterprise = gameState.Enterprise;
            var currentQuadrant = gameState.CurrentQuadrant;

            // Check if phasers are operational (D(4) in BASIC)
            if (enterprise.GetSystemDamage(ShipSystem.PhaserControl) < 0)
            {
                return CommandResult.Failure("PHASERS INOPERATIVE");
            }

            // Parse energy units to fire
            if (parameters.Length == 0)
            {
                return CommandResult.Failure($"PHASERS LOCKED ON TARGET;  ENERGY AVAILABLE = {enterprise.Energy} UNITS\n" +
                                           "Example: PHA 200");
            }

            if (!int.TryParse(parameters[0], out int energyToFire) || energyToFire <= 0)
            {
                return CommandResult.Failure("INVALID ENERGY AMOUNT. MUST BE A POSITIVE NUMBER");
            }

            // Check if sufficient energy available
            if (enterprise.Energy < energyToFire)
            {
                return CommandResult.Failure($"INSUFFICIENT ENERGY. AVAILABLE = {enterprise.Energy} UNITS");
            }

            // Check if there are Klingons in the quadrant
            var klingons = currentQuadrant.KlingonShips.Where(k => !k.IsDestroyed).ToList();
            if (klingons.Count == 0)
            {
                return CommandResult.Failure("SCIENCE OFFICER SPOCK REPORTS  'SENSORS SHOW NO ENEMY SHIPS\n" +
                                           "                                IN THIS QUADRANT'");
            }

            // Execute phaser firing
            return ExecutePhaserFire(gameState, energyToFire, klingons);
        }

        /// <summary>
        /// Executes the phaser firing sequence
        /// </summary>
        private CommandResult ExecutePhaserFire(GameState gameState, int energyToFire, List<KlingonShip> klingons)
        {
            var enterprise = gameState.Enterprise;
            var currentQuadrant = gameState.CurrentQuadrant;
            var result = new StringBuilder();

            // Consume energy first
            enterprise.Energy -= energyToFire;

            // Check for shield control damage affecting energy distribution (D(7) in BASIC: X=X*RND(1))
            var effectiveEnergy = energyToFire;
            if (enterprise.GetSystemDamage(ShipSystem.ShieldControl) < 0)
            {
                effectiveEnergy = (int)(energyToFire * gameState.Random.NextDouble());
            }

            // Check for computer damage affecting accuracy (D(8) in BASIC)
            var computerDamaged = enterprise.GetSystemDamage(ShipSystem.LibraryComputer) < 0;
            if (computerDamaged)
            {
                result.AppendLine("COMPUTER FAILURE HAMPERS ACCURACY");
            }

            // Calculate energy per Klingon (H1 in BASIC: H1=INT(X/K3))
            var energyPerKlingon = effectiveEnergy / klingons.Count;
            var klingonsDestroyed = 0;

            // Fire at each Klingon
            foreach (var klingon in klingons.ToList()) // ToList to avoid modification during enumeration
            {
                var distance = CalculateDistance(enterprise.SectorCoordinates, klingon.SectorCoordinates);

                // Calculate damage based on exact original BASIC formula
                // H = INT((H1/FND(0))*(RND(1)+2))
                var damageBase = (double)energyPerKlingon / distance;
                var damage = (int)(damageBase * (gameState.Random.NextDouble() + 2));

                // Apply computer damage penalty if applicable (affects accuracy)
                if (computerDamaged)
                {
                    damage = (int)(damage * (0.5 + gameState.Random.NextDouble() * 0.5)); // Reduced accuracy
                }

                // Check if damage is significant (original: IF H > .15 * K(I,3))
                if (damage > 0.15 * klingon.ShieldLevel)
                {
                    var wasDestroyed = klingon.TakeDamage(damage);
                    result.AppendLine($"{damage} UNIT HIT ON KLINGON AT SECTOR {klingon.SectorCoordinates.X},{klingon.SectorCoordinates.Y}");

                    if (wasDestroyed)
                    {
                        result.AppendLine("*** KLINGON DESTROYED ***");
                        klingonsDestroyed++;

                        // Remove from quadrant display
                        currentQuadrant.RemoveKlingon(klingon);

                        // Update galaxy data
                        gameState.Galaxy.RemoveKlingon(enterprise.QuadrantCoordinates);
                    }
                    else
                    {
                        result.AppendLine($"   (SENSORS SHOW {klingon.ShieldLevel} UNITS REMAINING)");
                    }
                }
                else
                {
                    result.AppendLine($"SENSORS SHOW NO DAMAGE TO ENEMY AT {klingon.SectorCoordinates.X},{klingon.SectorCoordinates.Y}");
                }
            }

            // Check for mission completion
            if (gameState.KlingonsRemaining <= 0)
            {
                result.AppendLine("\n*** MISSION ACCOMPLISHED ***");
                return CommandResult.Success(result.ToString().TrimEnd(), 1.0);
            }

            // Klingons return fire after player attack
            if (klingons.Any(k => !k.IsDestroyed))
            {
                var counterAttackResult = ExecuteKlingonCounterAttack(gameState);
                result.Append(counterAttackResult);
            }

            return CommandResult.Success(result.ToString().TrimEnd(), 1.0);
        }

        /// <summary>
        /// Calculates distance between two sector coordinates using original BASIC formula
        /// FND(0) = SQR((K(I,1)-S1)^2+(K(I,2)-S2)^2)
        /// </summary>
        private double CalculateDistance(Coordinates from, Coordinates to)
        {
            var deltaX = to.X - from.X;
            var deltaY = to.Y - from.Y;
            return Math.Sqrt(deltaX * deltaX + deltaY * deltaY);
        }

        /// <summary>
        /// Executes Klingon counter-attack after phaser fire
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
                var distance = CalculateDistance(klingon.SectorCoordinates, enterprise.SectorCoordinates);

                // Original BASIC formula: H=INT((K(I,3)/FND(1))*(2+RND(1)))
                var attackPower = (int)((klingon.ShieldLevel / distance) * (2 + gameState.Random.NextDouble()));
                totalDamage += attackPower;

                result.AppendLine($"{attackPower} UNIT HIT ON ENTERPRISE FROM SECTOR {klingon.SectorCoordinates.X},{klingon.SectorCoordinates.Y}");

                // Original BASIC: K(I,3)=K(I,3)/(3+RND(0)) - Klingon shields degrade after firing
                klingon.ShieldLevel = (int)(klingon.ShieldLevel / (3 + gameState.Random.NextDouble()));
            }

            // Apply damage to shields (original BASIC line 6060: S=S-H)
            if (totalDamage > 0)
            {
                enterprise.Shields -= totalDamage;

                // Check if shields destroyed (original BASIC line 6090: IF S<=0 THEN 6240)
                if (enterprise.Shields <= 0)
                {
                    // Ship destroyed! Set energy to 0 to trigger game over
                    enterprise.Energy = 0;
                    result.AppendLine();
                    result.AppendLine("THE ENTERPRISE HAS BEEN DESTROYED.  THE FEDERATION ");
                    result.AppendLine("WILL BE CONQUERED");
                }
                else
                {
                    // Shields absorbed damage (original BASIC line 6100)
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
            return "PHA <energy> - PHASERS: Fire energy-based weapons\n" +
                   "  Energy: Amount of energy units to fire at Klingon ships\n" +
                   "  Energy is distributed equally among all Klingon targets";
        }
    }
}
