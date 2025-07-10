# Super Star Trek Project Configuration

## Project Overview
- **Type**: Classic game port (BASIC to C#)
- **Framework**: .NET 9.0 Console Application
- **Architecture**: Object-oriented, clean architecture principles
- **Testing**: xUnit with comprehensive coverage
- **Domain**: Space strategy simulation game

## Key Files and Directories
- `src/SuperStarTrek.Game/`: Main game implementation
- `src/SuperStarTrek.Basic/superstartrek.bas`: Original BASIC source (reference)
- `tests/SuperStarTrek.Tests/`: Unit and integration tests
- `DESIGN.md`: Complete game architecture documentation
- `MIGRATION.md`: 14-week implementation plan
- `.github/copilot-instructions.md`: Main development guidelines
- `.github/instructions/`: Task-specific instruction files
- `.github/prompts/`: Reusable prompt files for common tasks

## Development Context
This project is actively migrating a 1978 BASIC game to modern C# while preserving exact gameplay behavior. The focus is on clean, testable code that maintains the classic gaming experience.

## Common Tasks
- Converting BASIC procedures to C# classes
- Implementing game systems (combat, navigation, sensors)
- Writing comprehensive unit tests
- Preserving original game behavior and display formatting
- Following incremental development approach per migration plan
