using System;
using System.Text;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game.Commands
{
    /// <summary>
    /// Navigation command - handles ship movement throughout the galaxy
    /// </summary>
    public class NavigationCommand : IGameCommand
    {
        public CommandResult Execute(GameState gameState, string[] parameters)
        {
            if (!gameState.Enterprise.IsSystemOperational(ShipSystem.WarpEngines))
            {
                return CommandResult.Failure("WARP ENGINES ARE DAMAGED");
            }

            // Parse course and warp factor
            if (parameters.Length < 2)
            {
                return CommandResult.Failure("NAVIGATION REQUIRES COURSE AND WARP FACTOR\nExample: NAV 1 4.2");
            }

            if (!double.TryParse(parameters[0], out double course) || course < 1 || course > 9)
            {
                return CommandResult.Failure("COURSE MUST BE BETWEEN 1.0 AND 9.0");
            }

            // Handle course 9 wrapping to course 1 (original BASIC behavior)
            if (course == 9.0)
            {
                course = 1.0;
            }

            if (!double.TryParse(parameters[1], out double warpFactor) || warpFactor <= 0)
            {
                return CommandResult.Failure("WARP FACTOR MUST BE POSITIVE");
            }

            // Check energy requirements (original BASIC: E = E - N - 10)
            var distance = Math.Round(warpFactor * 8);
            var energyRequired = (int)distance + 10;
            if (gameState.Enterprise.Energy < energyRequired)
            {
                return CommandResult.Failure($"INSUFFICIENT ENERGY. NEED {energyRequired} UNITS, HAVE {gameState.Enterprise.Energy}");
            }

            // Calculate movement
            var navigation = CalculateNavigation(gameState, course, warpFactor);

            if (!navigation.Success)
            {
                return CommandResult.Failure(navigation.Message!);
            }

            // Execute the movement
            ExecuteMovement(gameState, navigation);

            // Check for docking and get any docking message (BASIC lines 6430-6620)
            var dockingMessage = CheckForDocking(gameState);

            // Calculate time consumed (original BASIC: T = T + 1 for movement)
            var timeConsumed = 1.0;

            var message = BuildNavigationMessage(navigation, dockingMessage);
            return CommandResult.Success(message, timeConsumed);
        }

        public string GetHelpText()
        {
            return "NAV <course> <warp> - NAVIGATION: Move ship\n" +
                   "  Course: 1-8.999 (1=up, 3=right, 5=down, 7=left)\n" +
                   "  Warp: Speed factor (higher = faster, more energy)";
        }

        /// <summary>
        /// Calculates navigation parameters for the movement
        /// </summary>
        private NavigationResult CalculateNavigation(GameState gameState, double course, double warpFactor)
        {
            var result = new NavigationResult();
            var enterprise = gameState.Enterprise;

            // Use original BASIC course direction mapping (C array)
            var courseInt = (int)course;
            var courseFrac = course - courseInt;

            // Get direction vectors for current course and next course (for interpolation)
            var (deltaX1, deltaY1) = GetCourseDirection(courseInt);
            var (deltaX2, deltaY2) = GetCourseDirection(courseInt + 1);

            // Interpolate between course directions (original BASIC algorithm)
            // X1=C(C1,1)+(C(C1+1,1)-C(C1,1))*(C1-INT(C1))
            var deltaX = deltaX1 + (deltaX2 - deltaX1) * courseFrac;
            var deltaY = deltaY1 + (deltaY2 - deltaY1) * courseFrac;

            // Calculate distance in sectors (original BASIC: N=INT(W1*8+.5))
            var distance = Math.Round(warpFactor * 8);

            // Calculate movement using original BASIC algorithm
            // X=8*Q1+X+N*X1:Y=8*Q2+Y+N*X2:Q1=INT(X/8):Q2=INT(Y/8):S1=INT(X-Q1*8)
            var startSectorX = enterprise.SectorCoordinates.X;
            var startSectorY = enterprise.SectorCoordinates.Y;
            var currentQuadX = enterprise.QuadrantCoordinates.X;
            var currentQuadY = enterprise.QuadrantCoordinates.Y;

            // Calculate final absolute position (original BASIC line 3500)
            var finalAbsoluteX = 8 * currentQuadX + startSectorX + distance * deltaX;
            var finalAbsoluteY = 8 * currentQuadY + startSectorY + distance * deltaY;

            // Convert back to quadrant/sector coordinates (original BASIC algorithm)
            var newQuadrantX = (int)(finalAbsoluteX / 8);
            var newQuadrantY = (int)(finalAbsoluteY / 8);
            var newSectorX = (int)(finalAbsoluteX - newQuadrantX * 8);
            var newSectorY = (int)(finalAbsoluteY - newQuadrantY * 8);

            // Handle boundary cases (original BASIC lines 3550, 3590)
            if (newSectorX == 0)
            {
                newQuadrantX = newQuadrantX - 1;
                newSectorX = 8;
            }
            if (newSectorY == 0)
            {
                newQuadrantY = newQuadrantY - 1;
                newSectorY = 8;
            }

            // Apply galaxy boundary limits (original BASIC lines 3620-3750)
            // X5=0: flag for boundary crossing (BASIC line 3620)
            var boundaryClamped = false;
            if (newQuadrantX < 1)
            {
                boundaryClamped = true;
                newQuadrantX = 1;
                newSectorX = 1;
            }
            if (newQuadrantX > 8)
            {
                boundaryClamped = true;
                newQuadrantX = 8;
                newSectorX = 8;
            }
            if (newQuadrantY < 1)
            {
                boundaryClamped = true;
                newQuadrantY = 1;
                newSectorY = 1;
            }
            if (newQuadrantY > 8)
            {
                boundaryClamped = true;
                newQuadrantY = 8;
                newSectorY = 8;
            }

            // BASIC line 3790: IFX5=0THEN3860 (if no boundary, skip messages)
            // If boundary was hit, flag it for perimeter message (BASIC lines 3800-3840)
            if (boundaryClamped)
            {
                result.PerimeterMessageNeeded = true;
            }

            result.NewQuadrantCoordinates = new Coordinates(newQuadrantX, newQuadrantY);
            result.NewSectorCoordinates = new Coordinates(newSectorX, newSectorY);
            result.Distance = distance;
            result.EnergyConsumed = (int)distance + 10; // Original BASIC: E = E - N - 10
            result.QuadrantChanged = !result.NewQuadrantCoordinates.Equals(enterprise.QuadrantCoordinates);
            result.Success = true;

            return result;
        }

        /// <summary>
        /// Executes the movement and updates game state
        /// </summary>
        private void ExecuteMovement(GameState gameState, NavigationResult navigation)
        {
            var enterprise = gameState.Enterprise;
            var currentQuadrant = gameState.CurrentQuadrant;

            // Remove Enterprise from current position
            currentQuadrant.RemoveEnterprise(enterprise.SectorCoordinates);

            // Consume energy
            enterprise.Energy -= navigation.EnergyConsumed;

            // Check for collision with stars in the path
            var collision = CheckForStarCollision(gameState, navigation);
            if (collision.HasValue)
            {
                // Stop at collision point
                navigation.NewQuadrantCoordinates = collision.Value.quadrant;
                navigation.NewSectorCoordinates = collision.Value.sector;
                navigation.CollisionOccurred = true;
            }

            // Move to new position
            if (navigation.QuadrantChanged)
            {
                gameState.MoveEnterpriseToQuadrant(navigation.NewQuadrantCoordinates, navigation.NewSectorCoordinates);
            }
            else
            {
                // Same quadrant, just update sector
                enterprise.SectorCoordinates = navigation.NewSectorCoordinates;
                currentQuadrant.PlaceEnterprise(navigation.NewSectorCoordinates);
            }

            // Check for docking with starbase
            CheckForDocking(gameState);
        }

        /// <summary>
        /// Checks for collision with stars along the movement path
        /// </summary>
        private (Coordinates quadrant, Coordinates sector)? CheckForStarCollision(GameState gameState, NavigationResult navigation)
        {
            // For simplicity, check if destination sector contains a star
            var destinationQuadrant = gameState.Galaxy.GetQuadrant(navigation.NewQuadrantCoordinates);

            if (destinationQuadrant.StarCoordinates.Contains(navigation.NewSectorCoordinates))
            {
                // Find a nearby empty sector for emergency stop
                var emptySector = destinationQuadrant.FindEmptySector(gameState.Random);
                if (emptySector.HasValue)
                {
                    return (navigation.NewQuadrantCoordinates, emptySector.Value);
                }
            }

            return null;
        }

        /// <summary>
        /// Checks if Enterprise is adjacent to a starbase for docking.
        /// Implements BASIC lines 6430-6620 docking logic.
        /// </summary>
        /// <returns>Docking message if docking occurred, empty string otherwise</returns>
        private string CheckForDocking(GameState gameState)
        {
            var enterprise = gameState.Enterprise;
            var quadrant = gameState.CurrentQuadrant;

            // Check if starbase exists in quadrant
            if (!quadrant.HasStarbase)
            {
                enterprise.UndockFromStarbase();
                return string.Empty;
            }

            var starbaseCoords = quadrant.StarbaseCoordinates!.Value;
            var enterpriseCoords = enterprise.SectorCoordinates;

            // BASIC lines 6430-6540: Loop through adjacent sectors (S1-1 to S1+1, S2-1 to S2+1)
            // Check if adjacent (including diagonally)
            var deltaX = Math.Abs(starbaseCoords.X - enterpriseCoords.X);
            var deltaY = Math.Abs(starbaseCoords.Y - enterpriseCoords.Y);

            bool isAdjacent = deltaX <= 1 && deltaY <= 1 && (deltaX > 0 || deltaY > 0);

            if (isAdjacent && !enterprise.IsDocked)
            {
                // BASIC line 6580: D0=1:C$="DOCKED":E=E0:P=P0
                // Dock and get message (includes BASIC line 6620 message)
                var dockingMessage = enterprise.DockAtStarbase();

                // BASIC line 6580: E=E0:P=P0 - Refill energy and torpedoes
                enterprise.Resupply();

                return dockingMessage;
            }
            else if (!isAdjacent && enterprise.IsDocked)
            {
                // Moved away from starbase - undock
                enterprise.UndockFromStarbase();
            }

            return string.Empty;
        }

        /// <summary>
        /// Gets the direction vector for a given course using original BASIC C array mapping
        /// </summary>
        /// <param name="course">Course number (1-9)</param>
        /// <returns>Direction vector (deltaX, deltaY)</returns>
        private (double deltaX, double deltaY) GetCourseDirection(int course)
        {
            // Based on test expectations and game logic:
            // Course 1 = North (up) = deltaX=0, deltaY=-1
            // Course 3 = East (right) = deltaX=1, deltaY=0  
            // Course 5 = South (down) = deltaX=0, deltaY=1
            // Course 7 = West (left) = deltaX=-1, deltaY=0

            // Handle wrap-around for course 9 (treat as course 1)
            if (course > 8) { course = 1; }
            if (course < 1) { course = 8; }

            // Create direction mapping using exact original BASIC C array values
            return course switch
            {
                1 => (0.0, 1.0),    // North: C(1,1)=0, C(1,2)=1
                2 => (-1.0, 1.0),   // Northeast: C(2,1)=-1, C(2,2)=1
                3 => (-1.0, 0.0),   // East: C(3,1)=-1, C(3,2)=0
                4 => (-1.0, -1.0),  // Southeast: C(4,1)=-1, C(4,2)=-1
                5 => (0.0, -1.0),   // South: C(5,1)=0, C(5,2)=-1
                6 => (1.0, -1.0),   // Southwest: C(6,1)=1, C(6,2)=-1
                7 => (1.0, 0.0),    // West: C(7,1)=1, C(7,2)=0
                8 => (1.0, 1.0),    // Northwest: C(8,1)=1, C(8,2)=1
                _ => (0.0, 0.0)     // Default (should not happen)
            };
        }

        /// <summary>
        /// Builds the galactic perimeter message (BASIC lines 3800-3840)
        /// </summary>
        private string BuildPerimeterMessage(NavigationResult navigation)
        {
            var sb = new StringBuilder();
            // BASIC line 3800
            sb.AppendLine("LT. UHURA REPORTS MESSAGE FROM STARFLEET COMMAND:");
            // BASIC line 3810
            sb.AppendLine("  'PERMISSION TO ATTEMPT CROSSING OF GALACTIC PERIMETER");
            // BASIC line 3820
            sb.AppendLine("  IS HEREBY *DENIED*.  SHUT DOWN YOUR ENGINES.'");
            // BASIC line 3830
            sb.AppendLine("CHIEF ENGINEER SCOTT REPORTS  'WARP ENGINES SHUT DOWN");
            // BASIC line 3840
            sb.Append($"  AT SECTOR {navigation.NewSectorCoordinates.X},{navigation.NewSectorCoordinates.Y}");
            sb.Append($" OF QUADRANT {navigation.NewQuadrantCoordinates.X},{navigation.NewQuadrantCoordinates.Y}.'");
            return sb.ToString();
        }

        /// <summary>
        /// Builds the navigation result message
        /// </summary>
        private string BuildNavigationMessage(NavigationResult navigation, string dockingMessage)
        {
            var sb = new StringBuilder();

            // Add perimeter message if boundary was hit (BASIC lines 3800-3840)
            if (navigation.PerimeterMessageNeeded)
            {
                sb.AppendLine(BuildPerimeterMessage(navigation));
                sb.AppendLine();
            }

            if (navigation.CollisionOccurred)
            {
                sb.AppendLine("*** COLLISION WITH STAR ***");
                sb.AppendLine("EMERGENCY STOP EXECUTED");
            }

            if (navigation.QuadrantChanged)
            {
                sb.AppendLine($"ENTERING QUADRANT {navigation.NewQuadrantCoordinates.X},{navigation.NewQuadrantCoordinates.Y}");
            }

            sb.AppendLine($"NAVIGATION COMPLETE");
            sb.AppendLine($"NEW POSITION: QUADRANT {navigation.NewQuadrantCoordinates.X},{navigation.NewQuadrantCoordinates.Y} SECTOR {navigation.NewSectorCoordinates.X},{navigation.NewSectorCoordinates.Y}");
            sb.AppendLine($"ENERGY CONSUMED: {navigation.EnergyConsumed} UNITS");

            // Add docking message if present (BASIC line 6620)
            if (!string.IsNullOrEmpty(dockingMessage))
            {
                sb.Append(dockingMessage);
            }

            return sb.ToString().TrimEnd();
        }
    }

    /// <summary>
    /// Internal class to hold navigation calculation results
    /// </summary>
    internal class NavigationResult
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public Coordinates NewQuadrantCoordinates { get; set; }
        public Coordinates NewSectorCoordinates { get; set; }
        public double Distance { get; set; }
        public int EnergyConsumed { get; set; }
        public bool QuadrantChanged { get; set; }
        public bool CollisionOccurred { get; set; }
        public bool PerimeterMessageNeeded { get; set; }
    }
}
