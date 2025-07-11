namespace SuperStarTrek.Game.Models
{
    /// <summary>
    /// Represents a single quadrant in the galaxy (8x8 sectors).
    /// Corresponds to the quadrant state management in the original BASIC implementation.
    /// </summary>
    public class Quadrant
    {
        /// <summary>
        /// The 8x8 sector grid. Each string represents what's in that sector.
        /// Uses 3-character strings to match original BASIC display format.
        /// </summary>
        private readonly string[,] _sectors;

        /// <summary>
        /// Klingon ships present in this quadrant (up to 3 as in original)
        /// </summary>
        public List<KlingonShip> KlingonShips { get; }

        /// <summary>
        /// Starbase coordinates in this quadrant (null if no starbase)
        /// </summary>
        public Coordinates? StarbaseCoordinates { get; set; }

        /// <summary>
        /// Coordinates of stars in this quadrant
        /// </summary>
        public List<Coordinates> StarCoordinates { get; }

        /// <summary>
        /// Enterprise coordinates in this quadrant (null if Enterprise not present)
        /// </summary>
        public Coordinates? EnterpriseCoordinates { get; private set; }

        /// <summary>
        /// Number of Klingon ships in this quadrant
        /// </summary>
        public int KlingonCount => KlingonShips.Count(k => !k.IsDestroyed);

        /// <summary>
        /// Whether this quadrant has a starbase
        /// </summary>
        public bool HasStarbase => StarbaseCoordinates.HasValue;

        /// <summary>
        /// Whether the Enterprise is present in this quadrant
        /// </summary>
        public bool IsEnterprisePresent => EnterpriseCoordinates.HasValue;

        /// <summary>
        /// Enterprise position (throws if Enterprise not present)
        /// </summary>
        public Coordinates EnterprisePosition => EnterpriseCoordinates ?? throw new InvalidOperationException("Enterprise not present in this quadrant");

        /// <summary>
        /// Number of stars in this quadrant
        /// </summary>
        public int StarCount => StarCoordinates.Count;

        /// <summary>
        /// Initializes an empty quadrant
        /// </summary>
        public Quadrant()
        {
            _sectors = new string[8, 8];
            KlingonShips = new List<KlingonShip>();
            StarCoordinates = new List<Coordinates>();

            // Initialize all sectors as empty space
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    _sectors[i, j] = "   "; // 3 spaces for empty sector
                }
            }
        }

        /// <summary>
        /// Gets the display string for a specific sector
        /// </summary>
        public string GetSectorDisplay(Coordinates sectorCoords)
        {
            ValidateCoordinates(sectorCoords);
            return _sectors[sectorCoords.X - 1, sectorCoords.Y - 1]; // Convert to 0-based
        }

        /// <summary>
        /// Sets the display string for a specific sector
        /// </summary>
        public void SetSectorDisplay(Coordinates sectorCoords, string display)
        {
            ValidateCoordinates(sectorCoords);
            if (display.Length != 3)
            {
                throw new ArgumentException("Sector display must be exactly 3 characters", nameof(display));
            }

            _sectors[sectorCoords.X - 1, sectorCoords.Y - 1] = display; // Convert to 0-based
        }

        /// <summary>
        /// Checks if a sector is empty (contains only spaces)
        /// </summary>
        public bool IsSectorEmpty(Coordinates sectorCoords)
        {
            return GetSectorDisplay(sectorCoords) == "   ";
        }

        /// <summary>
        /// Finds an empty sector for placing objects
        /// </summary>
        public Coordinates? FindEmptySector(Random random)
        {
            var emptySectors = new List<Coordinates>();

            for (int x = 1; x <= 8; x++)
            {
                for (int y = 1; y <= 8; y++)
                {
                    var coords = new Coordinates(x, y);
                    if (IsSectorEmpty(coords))
                    {
                        emptySectors.Add(coords);
                    }
                }
            }

            return emptySectors.Count > 0 ? emptySectors[random.Next(emptySectors.Count)] : null;
        }

        /// <summary>
        /// Places the Enterprise in the specified sector
        /// </summary>
        public void PlaceEnterprise(Coordinates sectorCoords)
        {
            ValidateCoordinates(sectorCoords);
            EnterpriseCoordinates = sectorCoords;
            SetSectorDisplay(sectorCoords, "<*>");
        }

        /// <summary>
        /// Removes the Enterprise from its current position
        /// </summary>
        public void RemoveEnterprise(Coordinates sectorCoords)
        {
            ValidateCoordinates(sectorCoords);
            EnterpriseCoordinates = null;
            SetSectorDisplay(sectorCoords, "   ");
        }

        /// <summary>
        /// Places a Klingon ship in the specified sector
        /// </summary>
        public void PlaceKlingon(KlingonShip klingon)
        {
            SetSectorDisplay(klingon.SectorCoordinates, "+K+");
            if (!KlingonShips.Contains(klingon))
            {
                KlingonShips.Add(klingon);
            }
        }

        /// <summary>
        /// Moves a Klingon ship to a new position within the quadrant
        /// </summary>
        public void MoveKlingon(KlingonShip klingon, Coordinates newPosition)
        {
            // Clear old position
            SetSectorDisplay(klingon.SectorCoordinates, "   ");

            // Update Klingon position
            klingon.SectorCoordinates = newPosition;

            // Set new position
            SetSectorDisplay(newPosition, "+K+");
        }

        /// <summary>
        /// Removes a Klingon ship from the quadrant
        /// </summary>
        public void RemoveKlingon(KlingonShip klingon)
        {
            SetSectorDisplay(klingon.SectorCoordinates, "   ");
            KlingonShips.Remove(klingon);
        }

        /// <summary>
        /// Places a starbase in the specified sector
        /// </summary>
        public void PlaceStarbase(Coordinates sectorCoords)
        {
            SetSectorDisplay(sectorCoords, ">!<");
            StarbaseCoordinates = sectorCoords;
        }

        /// <summary>
        /// Removes the starbase from this quadrant
        /// </summary>
        public void RemoveStarbase(Coordinates sectorCoords)
        {
            if (StarbaseCoordinates.HasValue && StarbaseCoordinates.Value.Equals(sectorCoords))
            {
                SetSectorDisplay(sectorCoords, "   ");
                StarbaseCoordinates = null;
            }
        }

        /// <summary>
        /// Places a star in the specified sector
        /// </summary>
        public void PlaceStar(Coordinates sectorCoords)
        {
            SetSectorDisplay(sectorCoords, " * ");
            StarCoordinates.Add(sectorCoords);
        }

        /// <summary>
        /// Generates the galaxy code for this quadrant as used in the original BASIC.
        /// Format: hundreds=Klingons, tens=starbases, units=stars
        /// </summary>
        public int GetGalaxyCode()
        {
            var klingons = Math.Min(KlingonCount, 9); // Max 9 for single digit
            var starbases = HasStarbase ? 1 : 0;
            var stars = Math.Min(StarCount, 9); // Max 9 for single digit

            return klingons * 100 + starbases * 10 + stars;
        }

        /// <summary>
        /// Gets the full quadrant display as a formatted string for the short range sensors
        /// </summary>
        public string GetDisplayString()
        {
            var lines = new List<string>();

            for (int y = 1; y <= 8; y++)
            {
                var line = "";
                for (int x = 1; x <= 8; x++)
                {
                    line += " " + GetSectorDisplay(new Coordinates(x, y));
                }
                lines.Add(line);
            }

            return string.Join(Environment.NewLine, lines);
        }

        private static void ValidateCoordinates(Coordinates coords)
        {
            if (!Coordinates.IsValid(coords.X, coords.Y))
            {
                throw new ArgumentOutOfRangeException(nameof(coords), "Coordinates must be between 1 and 8");
            }
        }
    }
}
