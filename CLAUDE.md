# CLAUDE.md - AI Assistant Guide for Super Star Trek

## Project Overview

**Super Star Trek** is a C# port of the classic 1971 BASIC game by Mike Mayfield. This is a strategic space simulation where you command the USS Enterprise to eliminate Klingon warships within a time limit. The project preserves the original gameplay mechanics and feel while modernizing the codebase with object-oriented design and best practices.

### Key Facts
- **Language**: C# (.NET 9.0)
- **Project Type**: Console Application
- **Test Framework**: xUnit
- **Original Source**: Preserved in `src/SuperStarTrek.Basic/` for reference
- **Current Status**: Phase 4 completed (Emergency/Fatal Error Handling implemented)
- **Total Files**: 35 C# files (18 in game project, 17 in test project)

## Codebase Architecture

### High-Level Structure

```
SuperStarTrek/
├── src/
│   ├── SuperStarTrek.Basic/          # Original BASIC source code (reference only)
│   └── SuperStarTrek.Game/           # C# implementation
│       ├── Commands/                  # Command pattern implementations
│       │   ├── IGameCommand.cs       # Command interface + CommandResult
│       │   ├── CommandFactory.cs     # Creates commands from user input
│       │   ├── NavigationCommand.cs  # NAV - Ship movement
│       │   ├── ShortRangeSensorsCommand.cs  # SRS - Current quadrant
│       │   ├── LongRangeSensorsCommand.cs   # LRS - Surrounding quadrants
│       │   ├── PhaserCommand.cs      # PHA - Energy weapons
│       │   ├── TorpedoCommand.cs     # TOR - Photon torpedoes
│       │   ├── ShieldCommand.cs      # SHE - Shield management
│       │   └── DamageControlCommand.cs      # DAM - Damage reports
│       ├── Models/                    # Core game entities
│       │   ├── GameState.cs          # Central game state manager
│       │   ├── Galaxy.cs             # 8x8 galaxy grid
│       │   ├── Quadrant.cs           # Individual quadrant (8x8 sectors)
│       │   ├── Enterprise.cs         # Player's ship
│       │   ├── KlingonShip.cs        # Enemy ships
│       │   ├── Coordinates.cs        # Position management
│       │   └── ShipSystem.cs         # Ship system enumeration
│       ├── StarTrekGame.cs           # Main game loop and orchestration
│       └── Program.cs                # Entry point
├── tests/
│   └── SuperStarTrek.Tests/          # Unit tests (xUnit)
│       ├── Models/                   # Model tests
│       ├── Commands/                 # Command tests
│       └── StarTrekGameTests.cs      # Integration tests
├── .vscode/                          # VS Code configuration
│   ├── tasks.json                    # Build, test, watch tasks
│   └── launch.json                   # Debug configuration
├── .editorconfig                     # Code style settings
├── .gitignore                        # Git ignore patterns
├── SuperStarTrek.sln                # Solution file
└── [Documentation Files]             # DESIGN.md, MIGRATION.md, etc.
```

### Architecture Patterns

#### 1. Command Pattern
All game commands implement `IGameCommand` interface:
```csharp
public interface IGameCommand
{
    CommandResult Execute(GameState gameState, string[] parameters);
    string GetHelpText();
}
```

Commands return `CommandResult` with:
- `IsSuccess`: Whether command succeeded
- `Message`: Feedback to player
- `ConsumesTime`: Whether command advances game time
- `TimeConsumed`: Amount of time used (in stardates)

#### 2. Central State Management
`GameState` is the single source of truth containing:
- `Galaxy`: Complete 8x8 quadrant grid
- `Enterprise`: Player ship state
- Time management (stardate, mission limits)
- Game status (victory/defeat conditions)
- Shared `Random` instance for deterministic behavior

#### 3. Separation of Concerns
- **Models**: Data structures and business logic
- **Commands**: User actions and validation
- **StarTrekGame**: Game loop, display, and orchestration
- **Program**: Entry point only

#### 4. Authentic BASIC Behavior
The implementation strives to match the original BASIC code exactly:
- Random number generation using seeded `Random`
- Exact damage formulas from BASIC
- Original movement and combat mechanics
- Authentic counter-attack behavior (no complex AI)

## Key Design Principles

### 1. Preserve Original Gameplay
- All game mechanics match the 1978 Microsoft BASIC version
- Original BASIC source is preserved for reference
- Damage calculations use exact BASIC formulas
- No features added that weren't in original

### 2. Modern C# Practices
- Object-oriented design
- Nullable reference types enabled
- XML documentation comments
- Clean code principles
- SOLID principles where applicable

### 3. Testability
- Game logic separated from console I/O
- Commands are independently testable
- Deterministic behavior via seeded Random
- Test coverage for all major features

### 4. Readability
- Clear naming conventions
- XML documentation on public members
- Comments reference original BASIC line numbers
- Descriptive variable names (vs BASIC's single letters)

## Development Conventions

### Naming Conventions

#### Classes and Interfaces
- **PascalCase** for all types
- Interfaces prefixed with `I` (e.g., `IGameCommand`)
- Clear, descriptive names (e.g., `NavigationCommand`, not `NavCmd`)

#### Methods and Properties
- **PascalCase** for public members
- **camelCase** for private fields (with `_` prefix)
- Method names are verb-based (e.g., `Execute`, `Calculate`, `Display`)

#### Variables
- **camelCase** for local variables
- Descriptive names over brevity
- Original BASIC variable names in comments for reference

Example:
```csharp
// Original BASIC: Q1, Q2 = quadrant coords
private int _currentQuadrantX;
private int _currentQuadrantY;
```

### Code Style (.editorconfig)

- **Indentation**: 4 spaces for C#, 2 for config files
- **Line Endings**: CRLF (Windows style)
- **Braces**: Allman style (new line before open brace)
- **Using Statements**: System directives first, no grouping
- **var Usage**: Encouraged when type is apparent
- **Null Checking**: Prefer null propagation (`?.`) and coalesce (`??`)

### Documentation Standards

#### XML Documentation Required For:
- All public classes, interfaces, and enums
- All public methods, properties, and fields
- Complex private methods

Example:
```csharp
/// <summary>
/// Executes the navigation command to move the Enterprise
/// </summary>
/// <param name="gameState">Current game state</param>
/// <param name="parameters">Course and warp factor</param>
/// <returns>Result indicating success and time consumed</returns>
public CommandResult Execute(GameState gameState, string[] parameters)
```

#### Comments Should Include:
- References to original BASIC line numbers when porting logic
- Explanations of complex formulas
- Rationale for deviations from typical patterns

### Testing Practices

#### Test Organization
- **Mirror structure**: Tests in `tests/SuperStarTrek.Tests/` mirror `src/SuperStarTrek.Game/`
- **Naming**: `[ClassName]Tests.cs` (e.g., `NavigationCommandTests.cs`)
- **Method naming**: `[MethodName]_[Scenario]_[ExpectedOutcome]`

Example:
```csharp
[Fact]
public void Execute_WithValidCourse_ReturnsSuccess()
[Fact]
public void Execute_WithDamagedEngines_ReturnsFailure()
```

#### Test Patterns
1. **Arrange**: Set up game state with known values
2. **Act**: Execute the command or method
3. **Assert**: Verify expected outcomes

#### Test Coverage Goals
- All command Execute methods
- All public methods in models
- Edge cases and error conditions
- Authentic BASIC behavior verification

#### Running Tests
Tests run with `--parallel false` to avoid console interference:
```bash
dotnet test --parallel false
```

## Development Workflow

### Setting Up Development Environment

#### Prerequisites
- .NET 9.0 SDK or later
- Visual Studio Code (recommended) or Visual Studio
- C# Dev Kit extension for VS Code

#### Initial Setup
```bash
# Clone and setup
git clone <repository-url>
cd SuperStarTrek

# Restore dependencies
dotnet restore

# Build solution
dotnet build

# Run tests
dotnet test --parallel false

# Run game
dotnet run --project src/SuperStarTrek.Game
```

### VS Code Tasks

Pre-configured tasks in `.vscode/tasks.json`:

- **Build**: `Ctrl+Shift+B` or command "Tasks: Run Build Task"
- **Test**: Command "Tasks: Run Test Task" (runs with --parallel false)
- **Watch**: Auto-rebuild on file changes
- **Clean**: Remove build artifacts

### Common Development Tasks

#### Adding a New Command
1. Create class in `src/SuperStarTrek.Game/Commands/`
2. Implement `IGameCommand` interface
3. Add to `CommandFactory.CreateCommand()` switch
4. Create corresponding test file in `tests/SuperStarTrek.Tests/Commands/`
5. Add help text to `GetHelpText()` method
6. Update documentation if needed

#### Adding a New Model
1. Create class in `src/SuperStarTrek.Game/Models/`
2. Add XML documentation
3. Integrate with `GameState` if needed
4. Create corresponding test file
5. Update related commands if needed

#### Fixing a Bug
1. Create a test that reproduces the bug
2. Run tests to verify failure: `dotnet test`
3. Fix the bug in source code
4. Run tests to verify fix
5. Check if related areas need updates
6. Build and manually test if needed

#### Refactoring Code
1. Ensure existing tests pass
2. Make incremental changes
3. Run tests after each change
4. Verify game behavior hasn't changed
5. Update documentation if public API changes

### Build and Test Commands

```bash
# Build entire solution
dotnet build

# Build specific project
dotnet build src/SuperStarTrek.Game

# Run all tests
dotnet test --parallel false

# Run specific test file
dotnet test --filter "FullyQualifiedName~NavigationCommandTests"

# Run tests with verbose output
dotnet test --parallel false -v detailed

# Clean build artifacts
dotnet clean

# Rebuild from scratch
dotnet clean && dotnet build

# Watch mode (auto-rebuild on changes)
dotnet watch run --project src/SuperStarTrek.Game
```

## Git Workflow

### Branch Strategy

The project uses feature branches:
- **Main branch**: Stable, working code (no direct commits)
- **Feature branches**: Named `feature/descriptive-name` or `claude/session-id`
- **Pull Requests**: Required for merging to main

### Commit Conventions

#### Commit Message Format
```
<type>: <subject>

<optional body>
```

Types:
- `feat`: New feature
- `fix`: Bug fix
- `refactor`: Code restructuring without behavior change
- `test`: Adding or updating tests
- `docs`: Documentation changes
- `chore`: Build process, dependencies, etc.

#### Examples
```
feat: Implement emergency condition handling from BASIC lines 2020-2050

fix: ObjectDisposedException in DamageControlCommand tests

refactor: Eliminate console interference in EmergencyConditionTests

docs: Update MIGRATION.md with Phase 4 completion status
```

### Creating Commits

**IMPORTANT for AI Assistants**:
1. Check git status to see changes: `git status`
2. Review changes with: `git diff`
3. Stage relevant files: `git add <files>`
4. Create descriptive commit: `git commit -m "type: description"`
5. Verify commit: `git log -1`

**Never commit**:
- Build artifacts (`bin/`, `obj/`)
- IDE files not in `.gitignore`
- Temporary or test files
- Personal configuration

### Creating Pull Requests

Use GitHub CLI (`gh`) for PR creation:

```bash
# Ensure branch is up to date
git fetch origin main
git rebase origin/main

# Push branch
git push -u origin feature/branch-name

# Create PR (with HEREDOC for formatting)
gh pr create --title "Feature: Description" --body "$(cat <<'EOF'
## Summary
- Key change 1
- Key change 2

## Test Plan
- [ ] All tests pass
- [ ] Manual testing completed
- [ ] No regressions
EOF
)"
```

## Critical Development Notes

### 1. Console I/O Isolation
The game uses Console heavily, which interferes with parallel testing:
- Tests MUST run with `--parallel false`
- VS Code tasks.json already configures this
- Be careful when adding new Console.Write calls

### 2. Random Number Generation
All randomness uses `GameState.Random`:
- Enables reproducible testing with seeds
- Maintains BASIC behavior authenticity
- Never use `new Random()` in game code
- Always use `gameState.Random` in commands

### 3. BASIC Reference Comments
When implementing features from BASIC:
```csharp
// Original BASIC line 5520: IF D(2)<0 THEN PRINT "***SHORT RANGE SENSORS..."
if (!gameState.Enterprise.IsSystemOperational(ShipSystem.ShortRangeSensors))
{
    return CommandResult.Failure("***SHORT RANGE SENSORS ARE INOPERABLE");
}
```

### 4. Time Management
Commands that consume time must:
- Return `CommandResult` with `ConsumesTime = true`
- Set `TimeConsumed` to appropriate stardate value
- Most actions consume 1.0 stardate unit
- Navigation time varies by warp factor

### 5. Energy Management
Follow BASIC order of operations:
1. Deduct energy for action (navigation, weapons)
2. Apply damage/effects
3. Check for insufficient energy conditions
4. Update display

### 6. Klingon Behavior
**IMPORTANT**: Klingons do NOT have complex AI:
- Klingons only move during Enterprise navigation
- Counter-attacks are stationary (no movement)
- Follow exact BASIC formulas for damage
- Removed `KlingonAI.cs` in Phase 3 for authenticity

### 7. Testing Console Output
When testing methods that write to console:
- Capture console output if needed
- Use output redirection carefully
- Consider refactoring to return strings instead
- Be aware of test isolation issues

### 8. Nullable Reference Types
Project has nullable enabled:
- Use `?` for nullable reference types
- Check for null before use
- Use null-coalescing operators
- Compiler warnings are errors

## Project Phases

### Completed Phases

#### Phase 1: Foundation ✅
- Project setup, solution structure
- Core models: Galaxy, Quadrant, Enterprise, GameState
- Basic game loop

#### Phase 2: Core Systems ✅
- Galaxy generation with random placement
- Navigation system (9-directional movement)
- Display system (SRS, LRS)
- Coordinate management

#### Phase 3: Combat Systems ✅
- Phaser combat with authentic BASIC formulas
- Torpedo combat with trajectory calculation
- Enemy counter-attacks
- System damage mechanics
- Removed non-authentic KlingonAI

#### Phase 4: Ship Systems ✅
- Damage control system
- Shield energy management
- Emergency/fatal error handling (BASIC lines 2020-2050)
- Automatic repair system

### Future Development Areas
- Library computer commands (COM)
- Additional status displays
- Starbase docking and repair
- Game save/load functionality
- Enhanced error handling

## Useful File References

### Documentation Files
- `README.md`: User-facing documentation, game commands
- `DESIGN.md`: Architecture and system design
- `MIGRATION.md`: Porting progress and phase plans
- `NAVIGATION-GUIDE.md`: Navigation system details
- `PHASE[N]-*.md`: Phase completion reports

### Key Source Files
- `StarTrekGame.cs:1-400`: Main game loop, display methods
- `GameState.cs:1-200`: Central state management
- `IGameCommand.cs`: Command interface definition
- `CommandFactory.cs`: Command creation and routing
- `Enterprise.cs`: Ship state and system management

### Important Test Files
- `StarTrekGameTests.cs`: Basic game initialization
- `Commands/*Tests.cs`: Command-specific tests
- `Models/*Tests.cs`: Model behavior tests
- `EmergencyConditionTests.cs`: Critical error handling

## Tips for AI Assistants

### Before Making Changes
1. **Read relevant documentation**: Check DESIGN.md, MIGRATION.md
2. **Understand the phase**: Review phase completion documents
3. **Check BASIC reference**: Look at original BASIC code if porting
4. **Review existing tests**: Understand expected behavior
5. **Verify current state**: Run tests to ensure starting from green

### When Implementing Features
1. **Match BASIC behavior**: Preserve original game mechanics
2. **Test first**: Write failing test, then implement
3. **Small commits**: Commit logical units of work
4. **Update docs**: Keep documentation in sync with code
5. **Run full build**: Ensure no regressions

### When Debugging
1. **Run specific tests**: Isolate the failing test
2. **Check console output**: Look for error messages
3. **Verify game state**: Check if state is corrupted
4. **Compare to BASIC**: Ensure logic matches original
5. **Test manually**: Play the game to verify behavior

### When Refactoring
1. **Tests must pass first**: Start from green state
2. **Preserve behavior**: Don't change game mechanics
3. **Update comments**: Keep BASIC references accurate
4. **Run tests frequently**: After each small change
5. **Consider performance**: But accuracy comes first

### Common Pitfalls to Avoid
1. **Don't add features** not in original BASIC
2. **Don't break console output** formatting (players expect specific format)
3. **Don't use separate Random instances** (breaks determinism)
4. **Don't skip test updates** when changing behavior
5. **Don't commit generated files** (bin/, obj/)
6. **Don't run tests in parallel** (causes console interference)
7. **Don't modify BASIC reference files** (they're historical artifacts)

### Questions to Ask
- "Does this match the original BASIC behavior?"
- "Are there existing tests for this?"
- "Will this affect other commands or systems?"
- "Is this change documented?"
- "Have I tested this manually?"

## Building and Testing Checklist

Before committing changes:
- [ ] Code builds without errors: `dotnet build`
- [ ] All tests pass: `dotnet test --parallel false`
- [ ] No new warnings introduced
- [ ] Code follows style guidelines
- [ ] XML documentation added for public members
- [ ] Tests added/updated for changes
- [ ] Manual testing completed if needed
- [ ] Documentation updated if API changed
- [ ] Commit message is descriptive
- [ ] No generated files in commit

## Resources

### External Resources
- Original BASIC source: `src/SuperStarTrek.Basic/`
- .NET 9.0 Documentation: https://learn.microsoft.com/dotnet
- xUnit Documentation: https://xunit.net/docs
- C# Coding Conventions: https://learn.microsoft.com/dotnet/csharp/fundamentals/coding-style/coding-conventions

### Internal References
- Architecture decisions: DESIGN.md
- Migration progress: MIGRATION.md
- Phase completion reports: PHASE*-*.md
- Code style: .editorconfig

## Contact and Support

For questions about:
- **Game mechanics**: Reference original BASIC code or DESIGN.md
- **Architecture decisions**: Check DESIGN.md and phase completion docs
- **Build issues**: Check README.md prerequisites
- **Test failures**: Review test file and related command/model

---

**Last Updated**: 2025-11-17
**Current Phase**: Phase 4 Complete (Emergency Handling)
**Next Focus**: Library Computer Commands and Additional Features

*This guide is for AI assistants working on the Super Star Trek project. Keep it updated as the project evolves.*
