namespace SuperStarTrek.Game.Models
{
    /// <summary>
    /// Represents coordinates in the Super Star Trek universe.
    /// Uses 1-based indexing to match the original BASIC implementation.
    /// </summary>
    public readonly struct Coordinates
    {
        /// <summary>
        /// X coordinate (1-8 for quadrants, 1-8 for sectors)
        /// </summary>
        public int X { get; }

        /// <summary>
        /// Y coordinate (1-8 for quadrants, 1-8 for sectors)
        /// </summary>
        public int Y { get; }

        /// <summary>
        /// Initializes new coordinates with validation
        /// </summary>
        /// <param name="x">X coordinate (1-8)</param>
        /// <param name="y">Y coordinate (1-8)</param>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when coordinates are out of valid range</exception>
        public Coordinates(int x, int y)
        {
            if (x < 1 || x > 8)
                throw new ArgumentOutOfRangeException(nameof(x), "X coordinate must be between 1 and 8");
            if (y < 1 || y > 8)
                throw new ArgumentOutOfRangeException(nameof(y), "Y coordinate must be between 1 and 8");

            X = x;
            Y = y;
        }

        /// <summary>
        /// Checks if coordinates are within valid range without throwing exceptions
        /// </summary>
        public static bool IsValid(int x, int y) => x >= 1 && x <= 8 && y >= 1 && y <= 8;

        /// <summary>
        /// Calculates distance between two coordinates using Pythagorean theorem
        /// </summary>
        public double DistanceTo(Coordinates other)
        {
            return Math.Sqrt(Math.Pow(other.X - X, 2) + Math.Pow(other.Y - Y, 2));
        }

        public override string ToString() => $"({X},{Y})";

        public override bool Equals(object? obj) => obj is Coordinates other && X == other.X && Y == other.Y;

        public override int GetHashCode() => HashCode.Combine(X, Y);

        public static bool operator ==(Coordinates left, Coordinates right) => left.Equals(right);

        public static bool operator !=(Coordinates left, Coordinates right) => !left.Equals(right);
    }
}
