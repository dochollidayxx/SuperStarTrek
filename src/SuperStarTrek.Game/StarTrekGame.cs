using System;
using System.Linq;
using SuperStarTrek.Game.Commands;
using SuperStarTrek.Game.Models;

namespace SuperStarTrek.Game
{
    /// <summary>
    /// Main game class for Super Star Trek
    /// Manages the overall game flow and state
    /// </summary>
    public class StarTrekGame
    {
        private GameState? _gameState;
        private bool _gameInProgress = false;
        private readonly CommandFactory _commandFactory;

        /// <summary>
        /// Gets the current game state (null if no game is active)
        /// </summary>
        public GameState? CurrentGameState => _gameState;

        /// <summary>
        /// Whether a game is currently in progress
        /// </summary>
        public bool IsGameInProgress => _gameInProgress && _gameState != null && !_gameState.IsGameOver;

        /// <summary>
        /// Initializes a new instance of the StarTrekGame
        /// </summary>
        public StarTrekGame()
        {
            _commandFactory = new CommandFactory();
        }

        /// <summary>
        /// Starts a new game
        /// </summary>
        /// <param name="seed">Optional random seed for reproducible games</param>
        public void StartNewGame(int? seed = null)
        {
            Console.Clear();
            DisplayIntroduction();

            _gameState = new GameState(seed);
            _gameInProgress = true;

            Console.WriteLine();
            DisplayMissionBriefing();

            // Main game loop
            RunGameLoop();
        }

        /// <summary>
        /// Main game execution loop
        /// </summary>
        private void RunGameLoop()
        {
            if (_gameState == null)
            {
                return;
            }

            while (IsGameInProgress)
            {
                try
                {
                    DisplayStatus();

                    // Check for emergency conditions before accepting commands
                    if (CheckForEmergencyConditions())
                    {
                        // Ship is stranded - game over
                        _gameState.SetShipStranded();
                        DisplayGameOver();
                        break;
                    }

                    ProcessCommand();

                    if (_gameState.IsGameOver)
                    {
                        DisplayGameOver();
                        break;
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"An error occurred: {ex.Message}");
                    Console.WriteLine("Press any key to continue...");
                    Console.ReadKey();
                }
            }
        }

        /// <summary>
        /// Displays the game introduction
        /// </summary>
        private void DisplayIntroduction()
        {
            Console.WriteLine("                                    ,------*------,");
            Console.WriteLine("                    ,-------------   '---  ------'");
            Console.WriteLine("                     '-------- --'      / /");
            Console.WriteLine("                         ,---' '-------/ /--,");
            Console.WriteLine("                          '----------------'");
            Console.WriteLine();
            Console.WriteLine("                    THE USS ENTERPRISE --- NCC-1701");
            Console.WriteLine();
            Console.WriteLine("                         SUPER STAR TREK");
            Console.WriteLine("              Original BASIC program by Mike Mayfield");
            Console.WriteLine("                    C# port preserving original gameplay");
            Console.WriteLine();
        }

        /// <summary>
        /// Displays the mission briefing
        /// </summary>
        private void DisplayMissionBriefing()
        {
            if (_gameState == null)
            {
                return;
            }

            Console.WriteLine("**************************************************");
            Console.WriteLine("*                MISSION BRIEFING               *");
            Console.WriteLine("**************************************************");
            Console.WriteLine();
            Console.WriteLine($"YOUR ORDERS ARE AS FOLLOWS:");
            Console.WriteLine($"     DESTROY THE {_gameState.InitialKlingonCount} KLINGON WARSHIPS");
            Console.WriteLine($"     WHICH HAVE INVADED THE GALAXY");
            Console.WriteLine($"     BEFORE THEY CAN ATTACK FEDERATION HEADQUARTERS");
            Console.WriteLine($"     ON STARDATE {_gameState.MissionEndStardate:F1}.");
            Console.WriteLine();
            Console.WriteLine($"THIS GIVES YOU {_gameState.MissionTimeLimit} DAYS.");
            Console.WriteLine($"THERE ARE {_gameState.StarbasesRemaining} STARBASES IN THE GALAXY FOR RESUPPLY.");
            Console.WriteLine();
            Console.WriteLine("GOOD LUCK!");
            Console.WriteLine();
        }

        /// <summary>
        /// Displays current game status
        /// </summary>
        private void DisplayStatus()
        {
            if (_gameState == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("**************************************************");
            Console.WriteLine($"STARDATE: {_gameState.CurrentStardate:F1}   TIME REMAINING: {_gameState.RemainingTime:F1}");
            Console.WriteLine($"CONDITION: {GetConditionText()}");
            Console.WriteLine($"QUADRANT: ({_gameState.Enterprise.QuadrantCoordinates.X},{_gameState.Enterprise.QuadrantCoordinates.Y})   " +
                             $"SECTOR: ({_gameState.Enterprise.SectorCoordinates.X},{_gameState.Enterprise.SectorCoordinates.Y})");
            Console.WriteLine($"ENERGY: {_gameState.Enterprise.Energy}   SHIELDS: {_gameState.Enterprise.Shields}   " +
                             $"TORPEDOES: {_gameState.Enterprise.PhotonTorpedoes}");
            Console.WriteLine($"KLINGONS REMAINING: {_gameState.KlingonsRemaining}");
            Console.WriteLine("**************************************************");
        }

        /// <summary>
        /// Gets the ship's condition text based on current status
        /// </summary>
        private string GetConditionText()
        {
            if (_gameState == null)
            {
                return "UNKNOWN";
            }

            var quadrant = _gameState.CurrentQuadrant;
            var enterprise = _gameState.Enterprise;

            if (enterprise.IsDocked)
            {
                return "DOCKED";
            }

            if (quadrant.KlingonShips.Any())
            {
                return "RED";
            }

            if (enterprise.Energy < 1000)
            {
                return "YELLOW";
            }

            return "GREEN";
        }

        /// <summary>
        /// Processes user commands
        /// </summary>
        private void ProcessCommand()
        {
            Console.WriteLine();
            Console.Write("COMMAND? ");
            var input = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("PLEASE ENTER A COMMAND");
                return;
            }

            var parts = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            var commandName = parts[0].ToUpper();
            var parameters = parts.Skip(1).ToArray();

            // Handle built-in commands
            switch (commandName)
            {
                case "XXX":
                    Console.WriteLine("EMERGENCY EXIT");
                    _gameInProgress = false;
                    return;

                case "HELP":
                case "?":
                    DisplayCommands();
                    return;
            }

            // Try to execute game command
            var command = _commandFactory.CreateCommand(commandName);
            if (command != null && _gameState != null)
            {
                try
                {
                    var result = command.Execute(_gameState, parameters);

                    if (!string.IsNullOrEmpty(result.Message))
                    {
                        Console.WriteLine();
                        Console.WriteLine(result.Message);
                    }

                    if (result.ConsumesTime)
                    {
                        _gameState.AdvanceTime(result.TimeConsumed);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"ERROR EXECUTING COMMAND: {ex.Message}");
                }
            }
            else
            {
                // Handle not-yet-implemented commands
                switch (commandName)
                {
                    case "SHE":
                        Console.WriteLine("SHIELDS - Not yet implemented");
                        break;
                    case "DAM":
                        Console.WriteLine("DAMAGE CONTROL REPORT - Not yet implemented");
                        break;
                    case "COM":
                        Console.WriteLine("LIBRARY COMPUTER - Not yet implemented");
                        break;
                    default:
                        Console.WriteLine("UNKNOWN COMMAND. TYPE 'HELP' FOR COMMAND LIST.");
                        break;
                }
            }
        }

        /// <summary>
        /// Displays available commands
        /// </summary>
        private void DisplayCommands()
        {
            Console.WriteLine();
            Console.WriteLine(_commandFactory.GetAllCommandsHelp());
        }

        /// <summary>
        /// Displays game over screen
        /// </summary>
        private void DisplayGameOver()
        {
            if (_gameState == null)
            {
                return;
            }

            Console.WriteLine();
            Console.WriteLine("**************************************************");
            Console.WriteLine("*                  GAME OVER                    *");
            Console.WriteLine("**************************************************");
            Console.WriteLine();
            Console.WriteLine(_gameState.GetMissionStatus());

            if (_gameState.IsMissionComplete)
            {
                Console.WriteLine();
                Console.WriteLine($"YOUR EFFICIENCY RATING: {_gameState.CalculateEfficiencyRating():F2}");
                Console.WriteLine();
                Console.WriteLine("CONGRATULATIONS, CAPTAIN!");
                Console.WriteLine("THE FEDERATION HAS BEEN SAVED!");
            }
            else
            {
                Console.WriteLine();
                Console.WriteLine("BETTER LUCK NEXT TIME, CAPTAIN.");
            }

            Console.WriteLine();
            Console.WriteLine("Press any key to exit...");
            Console.ReadKey();
            _gameInProgress = false;
        }

        /// <summary>
        /// Checks for emergency conditions that would strand the ship
        /// Returns true if the ship is stranded and cannot continue
        /// </summary>
        private bool CheckForEmergencyConditions()
        {
            if (_gameState == null)
            {
                return false;
            }

            var enterprise = _gameState.Enterprise;

            // Check for fatal error condition from original BASIC lines 1990-2050
            // If (shields + energy) <= 10 AND (energy <= 10 AND shield control is damaged)
            var totalAvailableEnergy = enterprise.Shields + enterprise.Energy;
            var isShieldControlDamaged = enterprise.GetSystemDamage(ShipSystem.ShieldControl) < 0;

            if (totalAvailableEnergy <= 10 && (enterprise.Energy <= 10 && isShieldControlDamaged))
            {
                DisplayFatalErrorMessage();
                return true;
            }

            return false;
        }

        /// <summary>
        /// Displays the fatal error message when ship is stranded
        /// </summary>
        private void DisplayFatalErrorMessage()
        {
            Console.WriteLine();
            Console.WriteLine("** FATAL ERROR **   YOU'VE JUST STRANDED YOUR SHIP IN");
            Console.WriteLine("SPACE");
            Console.WriteLine("YOU HAVE INSUFFICIENT MANEUVERING ENERGY,");
            Console.WriteLine(" AND SHIELD CONTROL");
            Console.WriteLine("IS PRESENTLY INCAPABLE OF CROSS");
            Console.WriteLine("-CIRCUITING TO ENGINE ROOM!!");
            Console.WriteLine();
        }
    }
}
