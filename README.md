# Super Star Trek (C# Port)

A C# console application port of the classic Super Star Trek game, originally written in BASIC by Mike Mayfield in 1971.

## About the Game

Super Star Trek is a strategic space simulation game where you command the USS Enterprise (NCC-1701) on a critical mission to eliminate Klingon warships that have invaded Federation space. Originally created by Mike Mayfield and later enhanced by various contributors including Dave Ahl, Bob Leedom, and John Gorders, this game became one of the most famous early computer games.

### The Mission

You are the captain of the USS Enterprise with a time-critical mission:
- **Objective**: Destroy all Klingon battle cruisers in the galaxy
- **Time Limit**: Limited stardates before they attack Federation Headquarters
- **Resources**: Limited energy, photon torpedoes, and shield power
- **Support**: Scattered starbases for resupply and repairs

### Game Features

- **8x8 Galaxy Grid**: Navigate through 64 quadrants, each containing 8x8 sectors
- **Strategic Combat**: Use phasers and photon torpedoes against Klingon ships
- **Resource Management**: Manage energy, shields, and torpedo supplies
- **Ship Systems**: Deal with random damage to various ship components
- **Tactical Decisions**: Choose when to fight, flee, or seek starbase support

## Historical Context

This C# port preserves the gameplay mechanics and feel of the original 1978 Microsoft BASIC version while modernizing the codebase. The original BASIC source code is preserved in the `src/SuperStarTrek.Basic` folder for reference and historical accuracy.

## Getting Started

### Prerequisites

- .NET 9.0 SDK or later
- Visual Studio Code (recommended)
- C# Dev Kit extension for VS Code

### Building and Running

1. **Clone the repository** (if not already done):
   ```bash
   git clone <repository-url>
   cd SuperStarTrek
   ```

2. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

3. **Build the solution**:
   ```bash
   dotnet build
   ```

4. **Run the game**:
   ```bash
   dotnet run --project src/SuperStarTrek.Game
   ```

5. **Run tests**:
   ```bash
   dotnet test
   ```

### Development with VS Code

This project includes VS Code configuration files for an optimal development experience:

- **Build**: Press `Ctrl+Shift+P` → "Tasks: Run Build Task" or `Ctrl+Shift+B`
- **Run**: Press `F5` to debug or `Ctrl+F5` to run without debugging
- **Test**: Press `Ctrl+Shift+P` → "Tasks: Run Test Task"

## Project Structure

```
SuperStarTrek/
├── src/
│   ├── SuperStarTrek.Basic/     # Original BASIC source code
│   └── SuperStarTrek.Game/      # C# game implementation
├── tests/
│   └── SuperStarTrek.Tests/     # Unit tests
├── .vscode/                     # VS Code configuration
├── SuperStarTrek.sln           # Solution file
└── README.md                   # This file
```

## Game Commands

The game uses a command-driven interface with the following commands:

- **NAV** - Navigate the Enterprise through space
- **SRS** - Short Range Sensor scan (shows current quadrant)
- **LRS** - Long Range Sensor scan (shows surrounding quadrants)
- **PHA** - Fire phasers at enemy ships
- **TOR** - Fire photon torpedoes
- **SHE** - Raise or lower shield power
- **DAM** - Damage control reports and repairs
- **COM** - Access library computer functions
- **XXX** - Resign command (quit game)

### Library Computer Functions

When using the **COM** command, you can access:
- **0** - Cumulative galactic record
- **1** - Status report
- **2** - Photon torpedo data
- **3** - Starbase navigation data
- **4** - Direction/distance calculator
- **5** - Galaxy region name map

## Game Elements

### Space Objects
- **`<*>`** - USS Enterprise (your ship)
- **`+K+`** - Klingon battle cruiser
- **`>!<`** - Starbase
- **` * `** - Star

### Ship Status
- **Condition Green** - All systems normal
- **Condition Yellow** - Low energy
- **Condition Red** - Enemy ships present
- **Docked** - Adjacent to starbase (automatic resupply)

### Damage System
The Enterprise has 8 main systems that can be damaged:
1. Warp Engines
2. Short Range Sensors
3. Long Range Sensors
4. Phaser Control
5. Photon Tubes
6. Damage Control
7. Shield Control
8. Library Computer

## Development Notes

This C# port aims to maintain the gameplay mechanics and feel of the original while leveraging modern C# features and best practices:

- Object-oriented design
- Separation of concerns
- Unit testing
- Clean code principles

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests for new functionality
5. Ensure all tests pass
6. Submit a pull request

## License

This project is inspired by the original Super Star Trek game which was in the public domain. The C# implementation is provided as-is for educational and entertainment purposes.

## Acknowledgments

- Mike Mayfield - Original creator of Super Star Trek
- Dave Ahl - Published the game in "101 BASIC Games"
- Bob Leedom - Various modifications and debugging
- John Gorders - Converted to Microsoft 8K BASIC
