---
description: "Create a C# class for a game system"
mode: "edit"
---

# Create Game System Class

Create a C# class for a Super Star Trek game system following the project's architecture patterns.

## Requirements

- Follow the coding standards in [csharp-standards.instructions.md](../instructions/csharp-standards.instructions.md)
- Implement proper error handling and validation
- Include comprehensive XML documentation
- Add unit tests for all public methods
- Follow the game domain patterns from [DESIGN.md](../../DESIGN.md)

## Class Details

**System Name**: ${input:systemName:Enter the game system name (e.g., Navigation, Combat, Sensors)}
**Primary Responsibility**: ${input:responsibility:What is the main purpose of this class?}

## Implementation Guidelines

### Class Structure
- Use dependency injection for external dependencies
- Implement appropriate interfaces for testability
- Follow single responsibility principle
- Use readonly fields for immutable data

### Game Integration
- Maintain 1-based coordinate systems
- Preserve original game behavior
- Use game-specific terminology
- Handle edge cases like the original BASIC code

### Testing
- Create comprehensive unit tests
- Test boundary conditions
- Verify behavior matches original game
- Mock external dependencies

## Expected Output

Generate:
1. The main class file with full implementation
2. Interface definition if needed
3. Unit test class with comprehensive test coverage
4. Any supporting types or enums required

Reference the original BASIC code in `src/SuperStarTrek.Basic/superstartrek.bas` for exact behavior requirements.
