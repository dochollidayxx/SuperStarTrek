namespace SuperStarTrek.Game.Models
{
    /// <summary>
    /// Represents the USS Enterprise starship with all its systems and state.
    /// Corresponds to the ship variables in the original BASIC implementation.
    /// </summary>
    public class Enterprise
    {
        private readonly Dictionary<ShipSystem, double> _systemDamage;

        /// <summary>
        /// Current quadrant coordinates of the Enterprise
        /// </summary>
        public Coordinates QuadrantCoordinates { get; set; }

        /// <summary>
        /// Current sector coordinates within the quadrant
        /// </summary>
        public Coordinates SectorCoordinates { get; set; }

        /// <summary>
        /// Current energy level (E in original BASIC)
        /// </summary>
        public int Energy { get; set; }

        /// <summary>
        /// Initial/maximum energy level (E0 in original BASIC)
        /// </summary>
        public int MaxEnergy { get; }

        /// <summary>
        /// Current number of photon torpedoes (P in original BASIC)
        /// </summary>
        public int PhotonTorpedoes { get; set; }

        /// <summary>
        /// Initial/maximum number of photon torpedoes (P0 in original BASIC)
        /// </summary>
        public int MaxPhotonTorpedoes { get; }

        /// <summary>
        /// Current shield energy level (S in original BASIC)
        /// </summary>
        public int Shields { get; set; }

        /// <summary>
        /// Whether the ship is currently docked at a starbase (D0 in original BASIC)
        /// </summary>
        public bool IsDocked { get; set; }

        /// <summary>
        /// Initializes a new Enterprise with default starting values
        /// </summary>
        public Enterprise()
        {
            // Initialize with original BASIC starting values
            Energy = 3000;
            MaxEnergy = 3000;
            PhotonTorpedoes = 10;
            MaxPhotonTorpedoes = 10;
            Shields = 0;
            IsDocked = false;

            // Initialize all systems as undamaged
            _systemDamage = new Dictionary<ShipSystem, double>();
            foreach (var system in ShipSystemExtensions.GetAllSystems())
            {
                _systemDamage[system] = 0.0;
            }

            // Set random starting position (will be set properly during game initialization)
            QuadrantCoordinates = new Coordinates(1, 1);
            SectorCoordinates = new Coordinates(1, 1);
        }

        /// <summary>
        /// Gets the damage level for a specific ship system.
        /// Negative values indicate damage, positive values indicate good repair.
        /// </summary>
        public double GetSystemDamage(ShipSystem system)
        {
            return _systemDamage[system];
        }

        /// <summary>
        /// Sets the damage level for a specific ship system
        /// </summary>
        public void SetSystemDamage(ShipSystem system, double damage)
        {
            _systemDamage[system] = damage;
        }

        /// <summary>
        /// Checks if a specific ship system is operational (not damaged)
        /// </summary>
        public bool IsSystemOperational(ShipSystem system)
        {
            return _systemDamage[system] >= 0;
        }

        /// <summary>
        /// Gets all damaged systems
        /// </summary>
        public IEnumerable<ShipSystem> GetDamagedSystems()
        {
            return _systemDamage.Where(kvp => kvp.Value < 0).Select(kvp => kvp.Key);
        }

        /// <summary>
        /// Repairs all systems to full operational status (used when docked)
        /// </summary>
        public void RepairAllSystems()
        {
            foreach (var system in ShipSystemExtensions.GetAllSystems())
            {
                _systemDamage[system] = 0.0;
            }
        }

        /// <summary>
        /// Resupplies the ship to maximum energy and torpedoes (used when docked)
        /// </summary>
        public void Resupply()
        {
            Energy = MaxEnergy;
            PhotonTorpedoes = MaxPhotonTorpedoes;
            Shields = 0; // Shields are lowered when docked
        }

        /// <summary>
        /// Gets the current condition of the ship based on game state
        /// </summary>
        public string GetCondition(bool enemiesPresent)
        {
            if (IsDocked)
                return "DOCKED";
            if (enemiesPresent)
                return "*RED*";
            if (Energy < MaxEnergy * 0.1)
                return "YELLOW";
            return "GREEN";
        }

        /// <summary>
        /// Performs automatic repairs during ship movement based on warp factor.
        /// Implements the repair logic from BASIC lines 2770-3030.
        /// </summary>
        /// <param name="warpFactor">The warp factor used for movement</param>
        /// <param name="random">Random number generator for repair calculations</param>
        /// <returns>List of repair messages to display to the player</returns>
        public List<string> PerformAutomaticRepairs(double warpFactor, Random random)
        {
            var repairMessages = new List<string>();
            double repairAmount = warpFactor >= 1.0 ? 1.0 : warpFactor; // D6 in BASIC
            bool firstRepair = true;

            // Check each system for automatic repair (BASIC lines 2770-2880)
            foreach (var system in ShipSystemExtensions.GetAllSystems())
            {
                double currentDamage = _systemDamage[system];

                // Skip systems that aren't damaged
                if (currentDamage >= 0)
                {
                    continue;
                }

                // Apply repair amount
                double newDamage = currentDamage + repairAmount;

                // Handle partial repairs (BASIC lines 2790-2800)
                if (newDamage > -0.1 && newDamage < 0)
                {
                    _systemDamage[system] = -0.1; // Keep slightly damaged
                }
                else if (newDamage < 0)
                {
                    // Still damaged, but improved
                    _systemDamage[system] = newDamage;
                }
                else
                {
                    // System fully repaired (newDamage >= 0)
                    _systemDamage[system] = 0.0;

                    // Report repair completion (BASIC lines 2810-2840)
                    if (firstRepair)
                    {
                        repairMessages.Add("DAMAGE CONTROL REPORT:");
                        firstRepair = false;
                    }
                    repairMessages.Add($"        {system.GetDisplayName()} REPAIR COMPLETED.");
                }
            }

            // Random system damage or improvement during movement (BASIC lines 2880-3030)
            if (random.NextDouble() <= 0.2) // 20% chance of random event
            {
                var randomSystem = (ShipSystem)random.Next(1, 9); // FNR(1) function

                if (random.NextDouble() < 0.6) // 60% chance of damage
                {
                    // Random damage (BASIC lines 2930-2960)
                    double damageAmount = random.NextDouble() * 5 + 1; // RND(1)*5+1
                    _systemDamage[randomSystem] -= damageAmount;

                    if (firstRepair)
                    {
                        repairMessages.Add("DAMAGE CONTROL REPORT:");
                    }
                    repairMessages.Add($"{randomSystem.GetDisplayName()} DAMAGED");
                }
                else
                {
                    // Random improvement (BASIC lines 3000-3030)
                    double improvementAmount = random.NextDouble() * 3 + 1; // RND(1)*3+1
                    _systemDamage[randomSystem] += improvementAmount;

                    if (firstRepair)
                    {
                        repairMessages.Add("DAMAGE CONTROL REPORT:");
                    }
                    repairMessages.Add($"{randomSystem.GetDisplayName()} STATE OF REPAIR IMPROVED");
                }
            }

            return repairMessages;
        }

        /// <summary>
        /// Applies combat damage to a random ship system.
        /// Implements the damage logic from BASIC lines 6140-6170.
        /// </summary>
        /// <param name="hitStrength">The strength of the hit</param>
        /// <param name="shieldLevel">Current shield level</param>
        /// <param name="random">Random number generator</param>
        /// <returns>Name of the damaged system, or null if no damage occurred</returns>
        public string? ApplyCombatDamage(int hitStrength, int shieldLevel, Random random)
        {
            // Only apply system damage for significant hits (BASIC: H<20THEN6200)
            if (hitStrength < 20)
            {
                return null;
            }

            // Check if damage should occur (BASIC: RND(1)>.6ORH/S<=.02THEN6200)
            // Note: The original logic has OR not AND, meaning damage occurs if either condition is false
            bool skipDamage = random.NextDouble() > 0.6 || (shieldLevel > 0 && (double)hitStrength / shieldLevel <= 0.02);
            if (skipDamage)
            {
                return null;
            }

            // Select random system and apply damage (BASIC: R1=FNR(1):D(R1)=D(R1)-H/S-.5*RND(1))
            var randomSystem = (ShipSystem)random.Next(1, 9); // FNR(1) function
            double damageAmount = (shieldLevel > 0 ? (double)hitStrength / shieldLevel : hitStrength) + 0.5 * random.NextDouble();

            _systemDamage[randomSystem] -= damageAmount;

            return randomSystem.GetDisplayName();
        }

        /// <summary>
        /// Applies damage to the ship, considering shield absorption.
        /// Shields absorb damage before it affects the hull and systems.
        /// Based on BASIC combat damage logic from lines 6060-6100.
        /// </summary>
        /// <param name="damageAmount">Amount of damage to apply</param>
        /// <param name="random">Random number generator for damage calculations</param>
        /// <returns>Damage report messages</returns>
        public List<string> ApplyShieldedDamage(int damageAmount, Random random)
        {
            var messages = new List<string>();

            // If docked, starbase shields protect the Enterprise (BASIC line 6010)
            if (IsDocked)
            {
                messages.Add("STARBASE SHIELDS PROTECT THE ENTERPRISE");
                return messages;
            }

            // Apply damage to shields first (BASIC line 6060)
            Shields -= damageAmount;

            // Check if shields are destroyed
            if (Shields <= 0)
            {
                // Shields down - ship is destroyed
                messages.Add("");
                messages.Add("THE ENTERPRISE HAS BEEN DESTROYED.  THE FEDERATION ");
                messages.Add("WILL BE CONQUERED");
                return messages;
            }

            // Shields absorbed the damage (BASIC line 6100)
            messages.Add($"      <SHIELDS DOWN TO {Shields} UNITS>");

            // Apply system damage if hit was significant enough (handled by existing ApplyCombatDamage)
            var damagedSystem = ApplyCombatDamage(damageAmount, Shields, random);
            if (damagedSystem != null)
            {
                messages.Add($"DAMAGE CONTROL REPORTS {damagedSystem} DAMAGED BY THE HIT'");
            }

            return messages;
        }

        /// <summary>
        /// Handles automatic shield lowering when docking at a starbase.
        /// Based on BASIC line 6620: "SHIELDS DROPPED FOR DOCKING PURPOSES"
        /// </summary>
        /// <returns>Message about shield status change</returns>
        public string DockAtStarbase()
        {
            IsDocked = true;

            // Automatically lower shields when docking (BASIC line 6620)
            if (Shields > 0)
            {
                Shields = 0;
                return "SHIELDS DROPPED FOR DOCKING PURPOSES";
            }

            return "";
        }

        /// <summary>
        /// Handles undocking from a starbase
        /// </summary>
        public void UndockFromStarbase()
        {
            IsDocked = false;
        }

        /// <summary>
        /// Checks if shields are dangerously low and should trigger a warning.
        /// Based on BASIC line 1580 shield warning logic.
        /// </summary>
        /// <returns>True if shields are dangerously low</returns>
        public bool AreShieldsDangerouslyLow()
        {
            // This threshold matches the original game's warning system
            return Shields < 200 && !IsDocked;
        }
    }
}
