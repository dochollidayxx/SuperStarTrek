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
    }
}
