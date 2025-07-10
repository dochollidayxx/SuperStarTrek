# Super Star Trek (C# Port)

A C# console application port of the classic Super Star Trek game, originally written in BASIC.

## About

This is a faithful recreation of the Super Star Trek game that was originally created by Mike Mayfield and later modified by various contributors. The original BASIC source code is preserved in the `src/SuperStarTrek.Basic` folder for reference.

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

## Game Rules

Super Star Trek is a strategic simulation game where you command the USS Enterprise on a mission to eliminate Klingon warships that have invaded Federation space. You have limited time, energy, and resources to complete your mission.

### Commands

- **NAV** - Navigate the Enterprise
- **SRS** - Short Range Sensor scan
- **LRS** - Long Range Sensor scan
- **PHA** - Fire phasers
- **TOR** - Fire photon torpedoes
- **SHE** - Shield control
- **DAM** - Damage control report
- **COM** - Library computer
- **XXX** - Resign command

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
