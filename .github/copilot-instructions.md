# Super Star Trek C# Port - Development Instructions

## Project Context

This is a C# port of the classic 1978 BASIC Super Star Trek game. The project aims to preserve the original gameplay while modernizing the codebase using object-oriented design and modern C# practices.

## Project Structure

- **`src/SuperStarTrek.Game/`**: Main C# game implementation
- **`src/SuperStarTrek.Basic/`**: Original BASIC source code (reference only)
- **`tests/SuperStarTrek.Tests/`**: Unit tests
- **`DESIGN.md`**: Complete game architecture documentation
- **`MIGRATION.md`**: 14-week implementation plan

## Key Development Principles

### Code Style and Architecture
- Use modern C# features (.NET 9.0+)
- Follow object-oriented design principles
- Implement clean architecture patterns
- Maintain separation of concerns between game logic, UI, and data
- Use dependency injection where appropriate
- Follow SOLID principles

### Migration Strategy
- Preserve original game mechanics exactly
- Convert BASIC procedural code to C# object-oriented design
- Maintain identical gameplay behavior to the original
- Use incremental development approach
- Write comprehensive unit tests for all game systems

## Game Domain Knowledge

### Core Systems
1. **Galaxy System**: 8x8 grid of quadrants, each with 8x8 sectors
2. **Combat System**: Phasers (energy-based) and torpedoes (projectile-based)
3. **Navigation System**: 9-directional movement with warp speed
4. **Damage System**: 8 ship systems that can be damaged/repaired
5. **Resource Management**: Energy, shields, photon torpedoes
6. **Time Management**: Limited stardates to complete mission

### Game Objects
- **Enterprise (`<*>`)**: Player's starship
- **Klingons (`+K+`)**: Enemy ships to destroy
- **Starbases (`>!<`)**: Resupply and repair stations
- **Stars (` * `)**: Navigation obstacles

### Ship Systems (8 total)
1. Warp Engines
2. Short Range Sensors
3. Long Range Sensors
4. Phaser Control
5. Photon Tubes
6. Damage Control
7. Shield Control
8. Library Computer

## Coding Guidelines

### C# Conventions
- Use PascalCase for public members, camelCase for private fields
- Prefer explicit types over `var` unless type is obvious
- Use meaningful names that reflect the game domain
- Add XML documentation comments for all public APIs
- Use nullable reference types where appropriate

### Game-Specific Patterns
- Use coordinate systems consistently (1-based like original BASIC)
- Implement random number generation that matches original behavior
- Preserve original command interface (NAV, SRS, LRS, PHA, TOR, SHE, DAM, COM, XXX)
- Maintain original string formatting and display layout
- Use enums for game states and system types

### Testing Strategy
- Write unit tests for all game mechanics
- Test edge cases and boundary conditions
- Verify behavior matches original BASIC implementation
- Use test-driven development for new features
- Mock external dependencies (random number generation, console I/O)

## BASIC-to-C# Conversion Patterns

### Data Structures
```csharp
// BASIC: DIM G(8,8) -> C#: int[,] galaxy = new int[8,8]
// BASIC: DIM K(3,3) -> C#: KlingonShip[] klingons = new KlingonShip[3]
// BASIC: DIM D(8) -> C#: ShipSystem[] systems = new ShipSystem[8]
```

### Control Flow
- Convert GOTO/GOSUB to structured methods and control flow
- Replace line numbers with meaningful method names
- Use proper exception handling instead of error GOTOs

### String Operations
- Replace BASIC string functions with C# equivalents
- Maintain exact output formatting for game display
- Use StringBuilder for complex string manipulations

## Common Tasks

### When Adding New Game Features
1. Reference the original BASIC code for exact behavior
2. Write unit tests first
3. Implement using modern C# patterns
4. Ensure identical gameplay experience
5. Update documentation

### When Fixing Bugs
1. Check against original BASIC behavior
2. Write a failing test that reproduces the bug
3. Fix the issue while preserving game mechanics
4. Ensure all tests pass

### When Refactoring
1. Maintain backward compatibility
2. Don't change game behavior
3. Improve code structure and readability
4. Add tests if coverage is lacking

## Error Handling

- Use exceptions for exceptional conditions
- Provide clear error messages that match original game style
- Handle invalid user input gracefully
- Maintain game state consistency

## Performance Considerations

- Optimize string operations for display updates
- Use efficient data structures for game state
- Minimize memory allocations in game loop
- Ensure responsive user input handling

## Historical Accuracy

Always reference the original BASIC code when implementing features. The goal is to create a faithful recreation that preserves the classic gaming experience while leveraging modern development practices.
