using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Models
{
    /// <summary>
    /// Represents the entire galaxy (8x8 grid of quadrants) in Super Star Trek.
    /// Corresponds to the G(8,8) and Z(8,8) arrays in the original BASIC implementation.
    /// </summary>
    public class Galaxy
    {
        private readonly int[,] _galaxyData; // G(8,8) in original BASIC
        private readonly int[,] _knownData;  // Z(8,8) in original BASIC
        private readonly Dictionary<Coordinates, Quadrant> _loadedQuadrants;

        /// <summary>
        /// Total number of Klingon ships in the galaxy
        /// </summary>
        public int TotalKlingons
        {
            get
            {
                int total = 0;
                for (int x = 1; x <= 8; x++)
                {
                    for (int y = 1; y <= 8; y++)
                    {
                        var coords = new Coordinates(x, y);
                        if (_loadedQuadrants.ContainsKey(coords))
                        {
                            // Use actual count from loaded quadrant
                            total += _loadedQuadrants[coords].KlingonCount;
                        }
                        else
                        {
                            // Use stored galaxy data for unloaded quadrants
                            total += _galaxyData[x - 1, y - 1] / 100;
                        }
                    }
                }
                return total;
            }
        }

        /// <summary>
        /// Total number of starbases in the galaxy
        /// </summary>
        public int TotalStarbases { get; private set; }

        /// <summary>
        /// Random number generator for consistent behavior
        /// </summary>
        private readonly Random _random;

        /// <summary>
        /// Initializes a new galaxy with procedurally generated content
        /// </summary>
        /// <param name="seed">Random seed for reproducible galaxy generation</param>
        public Galaxy(int? seed = null)
        {
            _random = seed.HasValue ? new Random(seed.Value) : new Random();
            _galaxyData = new int[8, 8];
            _knownData = new int[8, 8];
            _loadedQuadrants = new Dictionary<Coordinates, Quadrant>();

            GenerateGalaxy();
        }

        /// <summary>
        /// Gets the galaxy data for a specific quadrant (Klingons*100 + Starbases*10 + Stars)
        /// </summary>
        public int GetQuadrantData(Coordinates quadrantCoords)
        {
            ValidateQuadrantCoordinates(quadrantCoords);
            return _galaxyData[quadrantCoords.X - 1, quadrantCoords.Y - 1];
        }

        /// <summary>
        /// Gets the known data for a specific quadrant (what the player has explored)
        /// </summary>
        public int GetKnownData(Coordinates quadrantCoords)
        {
            ValidateQuadrantCoordinates(quadrantCoords);
            return _knownData[quadrantCoords.X - 1, quadrantCoords.Y - 1];
        }

        /// <summary>
        /// Marks a quadrant as explored by the player
        /// </summary>
        public void MarkQuadrantExplored(Coordinates quadrantCoords)
        {
            ValidateQuadrantCoordinates(quadrantCoords);
            _knownData[quadrantCoords.X - 1, quadrantCoords.Y - 1] =
                _galaxyData[quadrantCoords.X - 1, quadrantCoords.Y - 1];
        }

        /// <summary>
        /// Gets or loads a specific quadrant
        /// </summary>
        public Quadrant GetQuadrant(Coordinates quadrantCoords)
        {
            ValidateQuadrantCoordinates(quadrantCoords);

            if (_loadedQuadrants.TryGetValue(quadrantCoords, out var existingQuadrant))
            {
                return existingQuadrant;
            }

            // Generate the quadrant based on galaxy data
            var quadrant = GenerateQuadrant(quadrantCoords);
            _loadedQuadrants[quadrantCoords] = quadrant;
            return quadrant;
        }

        /// <summary>
        /// Updates galaxy data when Klingons are destroyed
        /// </summary>
        public void RemoveKlingon(Coordinates quadrantCoords)
        {
            ValidateQuadrantCoordinates(quadrantCoords);
            var currentData = _galaxyData[quadrantCoords.X - 1, quadrantCoords.Y - 1];
            var klingons = currentData / 100;
            var starbases = (currentData % 100) / 10;
            var stars = currentData % 10;

            if (klingons > 0)
            {
                klingons--;
                _galaxyData[quadrantCoords.X - 1, quadrantCoords.Y - 1] = klingons * 100 + starbases * 10 + stars;
                _knownData[quadrantCoords.X - 1, quadrantCoords.Y - 1] = _galaxyData[quadrantCoords.X - 1, quadrantCoords.Y - 1];
            }
        }

        /// <summary>
        /// Updates galaxy data when a starbase is destroyed
        /// </summary>
        public void RemoveStarbase(Coordinates quadrantCoords)
        {
            ValidateQuadrantCoordinates(quadrantCoords);
            var currentData = _galaxyData[quadrantCoords.X - 1, quadrantCoords.Y - 1];
            var klingons = currentData / 100;
            var starbases = (currentData % 100) / 10;
            var stars = currentData % 10;

            if (starbases > 0)
            {
                starbases = 0;
                TotalStarbases--;
                _galaxyData[quadrantCoords.X - 1, quadrantCoords.Y - 1] = klingons * 100 + starbases * 10 + stars;
                _knownData[quadrantCoords.X - 1, quadrantCoords.Y - 1] = _galaxyData[quadrantCoords.X - 1, quadrantCoords.Y - 1];
            }
        }

        /// <summary>
        /// Generates the galaxy content using the original BASIC algorithm
        /// </summary>
        private void GenerateGalaxy()
        {
            TotalStarbases = 0;

            // Generate each quadrant following the original BASIC logic
            for (int i = 1; i <= 8; i++)
            {
                for (int j = 1; j <= 8; j++)
                {
                    var klingons = 0;
                    var r1 = _random.NextDouble();

                    // Original BASIC logic for Klingon placement
                    if (r1 > 0.98)
                    {
                        klingons = 3;
                    }
                    else if (r1 > 0.95)
                    {
                        klingons = 2;
                    }
                    else if (r1 > 0.80)
                    {
                        klingons = 1;
                    }

                    // Starbase placement
                    var starbases = 0;
                    if (_random.NextDouble() > 0.96)
                    {
                        starbases = 1;
                        TotalStarbases++;
                    }

                    // Stars (1-8 as in original)
                    var stars = (int)(_random.NextDouble() * 7.98 + 1.01); // FNR(1) function

                    _galaxyData[i - 1, j - 1] = klingons * 100 + starbases * 10 + stars;
                }
            }

            // Ensure at least one starbase exists (original BASIC logic)
            if (TotalStarbases == 0)
            {
                var quadX = (int)(_random.NextDouble() * 7.98 + 1.01);
                var quadY = (int)(_random.NextDouble() * 7.98 + 1.01);

                var currentData = _galaxyData[quadX - 1, quadY - 1];
                var klingons = currentData / 100;
                var stars = currentData % 10;

                _galaxyData[quadX - 1, quadY - 1] = klingons * 100 + 10 + stars; // Add starbase
                TotalStarbases = 1;
            }
        }

        /// <summary>
        /// Generates a quadrant with the appropriate objects based on galaxy data
        /// </summary>
        private Quadrant GenerateQuadrant(Coordinates quadrantCoords)
        {
            var quadrant = new Quadrant();
            var quadrantData = GetQuadrantData(quadrantCoords);

            var klingons = quadrantData / 100;
            var starbases = (quadrantData % 100) / 10;
            var stars = quadrantData % 10;

            // Place Klingons
            for (int i = 0; i < klingons; i++)
            {
                var sector = quadrant.FindEmptySector(_random);
                if (sector.HasValue)
                {
                    var klingon = KlingonShip.CreateWithRandomShields(sector.Value, _random);
                    quadrant.PlaceKlingon(klingon);
                }
            }

            // Place starbase
            if (starbases > 0)
            {
                var sector = quadrant.FindEmptySector(_random);
                if (sector.HasValue)
                {
                    quadrant.PlaceStarbase(sector.Value);
                }
            }

            // Place stars
            for (int i = 0; i < stars; i++)
            {
                var sector = quadrant.FindEmptySector(_random);
                if (sector.HasValue)
                {
                    quadrant.PlaceStar(sector.Value);
                }
            }

            return quadrant;
        }

        private static void ValidateQuadrantCoordinates(Coordinates coords)
        {
            if (!Coordinates.IsValid(coords.X, coords.Y))
            {
                throw new ArgumentOutOfRangeException(nameof(coords), "Quadrant coordinates must be between 1 and 8");
            }
        }
    }
}
