namespace SuperStarTrek.Game.Models
{
    /// <summary>
    /// Represents a Klingon warship in the game.
    /// Corresponds to the K(3,3) array in the original BASIC implementation.
    /// </summary>
    public class KlingonShip
    {
        /// <summary>
        /// Sector coordinates of the Klingon ship
        /// </summary>
        public Coordinates SectorCoordinates { get; set; }

        /// <summary>
        /// Shield/health level of the Klingon ship
        /// </summary>
        public int ShieldLevel { get; set; }

        /// <summary>
        /// Whether this Klingon ship is destroyed
        /// </summary>
        public bool IsDestroyed => ShieldLevel <= 0;

        /// <summary>
        /// Initializes a new Klingon ship at the specified coordinates
        /// </summary>
        /// <param name="sectorCoordinates">Sector coordinates where the ship is located</param>
        /// <param name="shieldLevel">Initial shield/health level</param>
        public KlingonShip(Coordinates sectorCoordinates, int shieldLevel)
        {
            SectorCoordinates = sectorCoordinates;
            ShieldLevel = shieldLevel;
        }

        /// <summary>
        /// Applies damage to the Klingon ship
        /// </summary>
        /// <param name="damage">Amount of damage to apply</param>
        /// <returns>True if the ship was destroyed by this damage</returns>
        public bool TakeDamage(int damage)
        {
            var wasAlive = !IsDestroyed;
            ShieldLevel -= damage;
            return wasAlive && IsDestroyed;
        }

        /// <summary>
        /// Calculates distance to another position
        /// </summary>
        public double DistanceTo(Coordinates target)
        {
            return SectorCoordinates.DistanceTo(target);
        }

        /// <summary>
        /// Creates a Klingon ship with random shield level as in the original game
        /// </summary>
        /// <param name="sectorCoordinates">Sector coordinates</param>
        /// <param name="random">Random number generator</param>
        /// <returns>New Klingon ship with random shield level</returns>
        public static KlingonShip CreateWithRandomShields(Coordinates sectorCoordinates, Random random)
        {
            // Original BASIC: K(I,3)=S9*(0.5+RND(1)) where S9=200
            var shieldLevel = (int)(200 * (0.5 + random.NextDouble()));
            return new KlingonShip(sectorCoordinates, shieldLevel);
        }
    }
}
