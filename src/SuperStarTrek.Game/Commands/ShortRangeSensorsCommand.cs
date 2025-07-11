using System.Linq;
using System.Text;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Short Range Sensors command - displays the current quadrant
    /// </summary>
    public class ShortRangeSensorsCommand : IGameCommand
    {
        public CommandResult Execute(GameState gameState, string[] parameters)
        {
            if (!gameState.Enterprise.IsSystemOperational(ShipSystem.ShortRangeSensors))
            {
                return CommandResult.Failure("SHORT RANGE SENSORS ARE DAMAGED");
            }

            var display = GenerateShortRangeDisplay(gameState);
            return CommandResult.Success(display);
        }

        public string GetHelpText()
        {
            return "SRS - SHORT RANGE SENSORS: Display current quadrant";
        }

        /// <summary>
        /// Generates the short range sensor display matching the original BASIC format
        /// </summary>
        private string GenerateShortRangeDisplay(GameState gameState)
        {
            var sb = new StringBuilder();
            var quadrant = gameState.CurrentQuadrant;
            var enterprise = gameState.Enterprise;

            // Header
            sb.AppendLine("SHORT RANGE SENSORS");
            sb.AppendLine();

            // Generate the 8x8 grid display
            for (int y = 1; y <= 8; y++)
            {
                // Sector row
                for (int x = 1; x <= 8; x++)
                {
                    var coords = new Coordinates(x, y);
                    var sectorDisplay = quadrant.GetSectorDisplay(coords);
                    sb.Append($" {sectorDisplay}");
                }

                // Add status information on the right side for certain rows
                switch (y)
                {
                    case 1:
                        sb.Append($"    STARDATE      {gameState.CurrentStardate:F1}");
                        break;
                    case 2:
                        sb.Append($"    CONDITION     {GetConditionText(gameState)}");
                        break;
                    case 3:
                        sb.Append($"    QUADRANT      {enterprise.QuadrantCoordinates.X},{enterprise.QuadrantCoordinates.Y}");
                        break;
                    case 4:
                        sb.Append($"    SECTOR        {enterprise.SectorCoordinates.X},{enterprise.SectorCoordinates.Y}");
                        break;
                    case 5:
                        sb.Append($"    PHOTON TORPEDOES {enterprise.PhotonTorpedoes}");
                        break;
                    case 6:
                        sb.Append($"    TOTAL ENERGY  {enterprise.Energy + enterprise.Shields}");
                        break;
                    case 7:
                        sb.Append($"    SHIELDS       {enterprise.Shields}");
                        break;
                    case 8:
                        sb.Append($"    KLINGONS REMAINING {gameState.KlingonsRemaining}");
                        break;
                }

                sb.AppendLine();
            }

            return sb.ToString();
        }

        /// <summary>
        /// Gets the ship's condition text based on current status
        /// </summary>
        private string GetConditionText(GameState gameState)
        {
            var quadrant = gameState.CurrentQuadrant;
            var enterprise = gameState.Enterprise;

            if (enterprise.IsDocked)
            {
                return "DOCKED";
            }

            if (quadrant.KlingonShips.Any(k => !k.IsDestroyed))
            {
                return "RED";
            }

            if (enterprise.Energy < 1000)
            {
                return "YELLOW";
            }

            return "GREEN";
        }
    }
}
