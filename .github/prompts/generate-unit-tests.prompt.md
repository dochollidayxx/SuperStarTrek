---
description: "Generate comprehensive unit tests for game functionality"
mode: "edit"
---

# Generate Unit Tests for Game System

Create comprehensive unit tests for a Super Star Trek game system that verify behavior matches the original BASIC implementation.

## Test Requirements

### Test Coverage
- All public methods and properties
- Edge cases and boundary conditions
- Error handling and validation
- Game state transitions
- Original BASIC behavior verification

### Test Structure
- Use Arrange-Act-Assert pattern
- Descriptive test method names
- Proper test data setup
- Mock external dependencies

## Target Code

```csharp
${selection}
```

## Test Guidelines

### Game-Specific Testing
- Test coordinate systems (1-based indexing)
- Verify random number generation patterns
- Test string formatting matches original
- Validate game rules and mechanics

### C# Testing Best Practices
- Use xUnit framework
- Create test data builders/factories
- Use parameterized tests for multiple scenarios
- Test both success and failure paths

### Original Behavior Verification
- Compare outputs with BASIC implementation
- Test with known random seeds
- Verify exact string formats
- Test command parsing accuracy

## Expected Output

Generate a complete test class with:
1. **Setup Methods**: Test data initialization
2. **Unit Tests**: Individual method testing
3. **Integration Tests**: System interaction testing
4. **Edge Case Tests**: Boundary condition validation
5. **Behavior Tests**: Original game behavior verification

Include test helper methods and mock setups as needed.

Reference the original BASIC code for expected behavior patterns and edge cases.
