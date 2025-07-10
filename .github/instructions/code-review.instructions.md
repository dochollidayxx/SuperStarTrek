---
description: "Code review guidelines for Super Star Trek C# port"
applyTo: "**/*.cs"
---

# Code Review Guidelines for Super Star Trek

## Review Checklist

### Game Mechanics Preservation
- [ ] Does the implementation match the original BASIC behavior?
- [ ] Are coordinate systems using 1-based indexing like the original?
- [ ] Do random number patterns match expected game behavior?
- [ ] Are display formats identical to the original game?

### Code Quality
- [ ] Are public APIs documented with XML comments?
- [ ] Do class and method names reflect the game domain?
- [ ] Is error handling appropriate and consistent?
- [ ] Are magic numbers replaced with named constants?

### Architecture
- [ ] Does the code follow SOLID principles?
- [ ] Is separation of concerns maintained?
- [ ] Are dependencies properly injected?
- [ ] Is the code testable?

### Testing
- [ ] Are unit tests included for new functionality?
- [ ] Do tests cover edge cases and boundary conditions?
- [ ] Are tests verifying behavior against original game?
- [ ] Is test coverage adequate (>90%)?

### Migration Plan Compliance
- [ ] Does the implementation follow the phase structure in MIGRATION.md?
- [ ] Are BASIC-to-C# conversion patterns followed?
- [ ] Is the incremental development approach maintained?

## Common Issues to Look For

### Game Logic
- Off-by-one errors in coordinate systems
- Incorrect random number generation patterns
- String formatting inconsistencies
- Command parsing errors

### C# Specific
- Nullable reference type handling
- Proper async/await usage if applicable
- Resource disposal (IDisposable)
- Exception handling patterns

### Performance
- Inefficient string operations
- Unnecessary object allocations
- Slow game loop performance
- Memory leaks

## Review Process

1. **Functional Review**: Verify game behavior matches original
2. **Code Quality Review**: Check coding standards and patterns
3. **Architecture Review**: Ensure design principles are followed
4. **Test Review**: Validate test coverage and quality
5. **Documentation Review**: Ensure adequate documentation
