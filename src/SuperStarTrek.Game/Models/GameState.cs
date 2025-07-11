using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Models
{
    /// <summary>
    /// Represents the complete game state for Super Star Trek.
    /// Manages all game variables and progress tracking.
    /// </summary>
    public class GameState
    {
        /// <summary>
        /// The galaxy containing all quadrants
        /// </summary>
        public Galaxy Galaxy { get; }

        /// <summary>
        /// The USS Enterprise
        /// </summary>
        public Enterprise Enterprise { get; }

        /// <summary>
        /// Current stardate (T in original BASIC)
        /// </summary>
        public double CurrentStardate { get; set; }

        /// <summary>
        /// Starting stardate (T0 in original BASIC)
        /// </summary>
        public double StartingStardate { get; }

        /// <summary>
        /// Mission time limit in stardates (T9 in original BASIC)
        /// </summary>
        public int MissionTimeLimit { get; }

        /// <summary>
        /// End stardate when mission must be completed
        /// </summary>
        public double MissionEndStardate => StartingStardate + MissionTimeLimit;

        /// <summary>
        /// Remaining time for the mission
        /// </summary>
        public double RemainingTime => MissionEndStardate - CurrentStardate;

        /// <summary>
        /// Total Klingons remaining in the galaxy
        /// </summary>
        public int KlingonsRemaining => Galaxy.TotalKlingons;

        /// <summary>
        /// Total starbases remaining in the galaxy
        /// </summary>
        public int StarbasesRemaining => Galaxy.TotalStarbases;

        /// <summary>
        /// Whether the mission is complete (all Klingons destroyed)
        /// </summary>
        public bool IsMissionComplete => KlingonsRemaining <= 0;

        /// <summary>
        /// Whether the mission has failed (time expired or ship destroyed)
        /// </summary>
        public bool IsMissionFailed => RemainingTime <= 0 || Enterprise.Energy <= 0;

        /// <summary>
        /// Whether the game is over (mission complete or failed)
        /// </summary>
        public bool IsGameOver => IsMissionComplete || IsMissionFailed;

        /// <summary>
        /// Random number generator for game events
        /// </summary>
        public Random Random { get; }

        /// <summary>
        /// Current quadrant the Enterprise is in
        /// </summary>
        public Quadrant CurrentQuadrant => Galaxy.GetQuadrant(Enterprise.QuadrantCoordinates);

        /// <summary>
        /// Initial number of Klingons for efficiency rating calculation (K7 in original BASIC)
        /// </summary>
        public int InitialKlingonCount { get; }

        /// <summary>
        /// Initializes a new game state with random galaxy generation
        /// </summary>
        /// <param name="seed">Optional random seed for reproducible games</param>
        public GameState(int? seed = null)
        {
            Random = seed.HasValue ? new Random(seed.Value) : new Random();

            // Initialize galaxy first to get Klingon count
            Galaxy = new Galaxy(seed);
            InitialKlingonCount = Galaxy.TotalKlingons;

            // Initialize Enterprise
            Enterprise = new Enterprise();

            // Set mission parameters following original BASIC logic
            // T=INT(RND(1)*20+20)*100: stardate between 2000-3900
            CurrentStardate = (int)(Random.NextDouble() * 20 + 20) * 100;
            StartingStardate = CurrentStardate;

            // T9=25+INT(RND(1)*10): mission time 25-34 days
            MissionTimeLimit = 25 + (int)(Random.NextDouble() * 10);

            // Ensure mission time is adequate if many Klingons
            if (Galaxy.TotalKlingons > MissionTimeLimit)
            {
                MissionTimeLimit = Galaxy.TotalKlingons + 1;
            }

            // Set Enterprise starting position
            SetRandomEnterprisePosition();
        }

        /// <summary>
        /// Advances the stardate by the specified amount
        /// </summary>
        public void AdvanceTime(double timeUnits)
        {
            CurrentStardate += timeUnits;
        }

        /// <summary>
        /// Places the Enterprise at a random position in the galaxy
        /// </summary>
        public void SetRandomEnterprisePosition()
        {
            // Generate random quadrant coordinates
            var quadX = (int)(Random.NextDouble() * 7.98 + 1.01); // FNR(1) function
            var quadY = (int)(Random.NextDouble() * 7.98 + 1.01);
            Enterprise.QuadrantCoordinates = new Coordinates(quadX, quadY);

            // Generate random sector coordinates
            var sectX = (int)(Random.NextDouble() * 7.98 + 1.01);
            var sectY = (int)(Random.NextDouble() * 7.98 + 1.01);
            Enterprise.SectorCoordinates = new Coordinates(sectX, sectY);

            // Place Enterprise in the current quadrant
            var currentQuadrant = CurrentQuadrant;
            currentQuadrant.PlaceEnterprise(Enterprise.SectorCoordinates);

            // Mark this quadrant as explored
            Galaxy.MarkQuadrantExplored(Enterprise.QuadrantCoordinates);
        }

        /// <summary>
        /// Moves the Enterprise to a new quadrant
        /// </summary>
        public void MoveEnterpriseToQuadrant(Coordinates newQuadrantCoords, Coordinates newSectorCoords)
        {
            // Remove Enterprise from current quadrant
            CurrentQuadrant.RemoveEnterprise(Enterprise.SectorCoordinates);

            // Update Enterprise position
            Enterprise.QuadrantCoordinates = newQuadrantCoords;
            Enterprise.SectorCoordinates = newSectorCoords;

            // Place Enterprise in new quadrant
            var newQuadrant = Galaxy.GetQuadrant(newQuadrantCoords);
            newQuadrant.PlaceEnterprise(newSectorCoords);

            // Mark new quadrant as explored
            Galaxy.MarkQuadrantExplored(newQuadrantCoords);
        }

        /// <summary>
        /// Calculates the efficiency rating as in the original game
        /// </summary>
        public double CalculateEfficiencyRating()
        {
            if (IsMissionComplete)
            {
                var timeUsed = CurrentStardate - StartingStardate;
                return 1000 * Math.Pow(InitialKlingonCount / timeUsed, 2);
            }
            return 0;
        }

        /// <summary>
        /// Gets the mission status message
        /// </summary>
        public string GetMissionStatus()
        {
            if (IsMissionComplete)
                return "MISSION ACCOMPLISHED!";
            if (RemainingTime <= 0)
                return "MISSION FAILED - TIME EXPIRED";
            if (Enterprise.Energy <= 0)
                return "MISSION FAILED - ENTERPRISE DESTROYED";

            return $"MISSION IN PROGRESS - {KlingonsRemaining} KLINGONS REMAINING";
        }
    }
}
