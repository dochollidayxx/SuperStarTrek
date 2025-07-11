using System.Text;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Long Range Sensors command - displays surrounding quadrants
    /// </summary>
    public class LongRangeSensorsCommand : IGameCommand
    {
        public CommandResult Execute(GameState gameState, string[] parameters)
        {
            if (!gameState.Enterprise.IsSystemOperational(ShipSystem.LongRangeSensors))
            {
                return CommandResult.Failure("LONG RANGE SENSORS ARE DAMAGED");
            }

            var display = GenerateLongRangeDisplay(gameState);
            return CommandResult.Success(display);
        }

        public string GetHelpText()
        {
            return "LRS - LONG RANGE SENSORS: Display surrounding quadrants";
        }

        /// <summary>
        /// Generates the long range sensor display showing a 3x3 grid of quadrants
        /// </summary>
        private string GenerateLongRangeDisplay(GameState gameState)
        {
            var sb = new StringBuilder();
            var enterprise = gameState.Enterprise;
            var galaxy = gameState.Galaxy;

            sb.AppendLine("LONG RANGE SENSORS");
            sb.AppendLine($"FOR QUADRANT {enterprise.QuadrantCoordinates.X},{enterprise.QuadrantCoordinates.Y}");
            sb.AppendLine();

            // Display 3x3 grid centered on current quadrant
            var centerX = enterprise.QuadrantCoordinates.X;
            var centerY = enterprise.QuadrantCoordinates.Y;

            sb.AppendLine("    -------------------");

            for (int deltaY = -1; deltaY <= 1; deltaY++)
            {
                sb.Append("    ");
                for (int deltaX = -1; deltaX <= 1; deltaX++)
                {
                    var quadX = centerX + deltaX;
                    var quadY = centerY + deltaY;

                    sb.Append(": ");

                    // Check if coordinates are within galaxy bounds
                    if (quadX >= 1 && quadX <= 8 && quadY >= 1 && quadY <= 8)
                    {
                        var coords = new Coordinates(quadX, quadY);

                        // Mark current quadrant as explored
                        if (deltaX == 0 && deltaY == 0)
                        {
                            galaxy.MarkQuadrantExplored(coords);
                        }

                        var knownData = galaxy.GetKnownData(coords);
                        if (knownData > 0)
                        {
                            // Display known quadrant data
                            sb.Append($"{knownData:D3}");
                        }
                        else
                        {
                            // Unknown quadrant
                            sb.Append("***");
                        }
                    }
                    else
                    {
                        // Outside galaxy bounds
                        sb.Append("   ");
                    }

                    sb.Append(" ");
                }
                sb.AppendLine(":");
            }

            sb.AppendLine("    -------------------");
            sb.AppendLine();
            sb.AppendLine("LEGEND:");
            sb.AppendLine("  First digit: Klingons");
            sb.AppendLine("  Second digit: Starbases");
            sb.AppendLine("  Third digit: Stars");

            return sb.ToString();
        }
    }
}
