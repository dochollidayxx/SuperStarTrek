---
description: "Review code for game behavior accuracy"
mode: "ask"
---

# Game Behavior Accuracy Review

Perform a comprehensive review of the code to ensure it matches the original Super Star Trek BASIC implementation behavior.

## Review Focus Areas

### Game Mechanics Accuracy
- Are coordinate systems properly 1-based?
- Do random number patterns match original behavior?
- Are display formats identical to the original?
- Do command responses match expected text?

### BASIC Conversion Accuracy
- Are calculations performed identically?
- Are string operations producing same results?
- Are control flow patterns preserved?
- Are edge cases handled like the original?

## Code Under Review

```csharp
${selection}
```

## Review Process

### Behavioral Analysis
1. **Compare Against Original**: Reference the BASIC code in `src/SuperStarTrek.Basic/superstartrek.bas`
2. **Test Scenarios**: Identify test cases that verify original behavior
3. **Edge Cases**: Check boundary conditions and error handling
4. **Output Verification**: Ensure exact string formatting

### Game Domain Review
1. **Terminology**: Are game terms used correctly?
2. **Rules**: Are game rules properly implemented?
3. **State Management**: Is game state handled correctly?
4. **User Experience**: Does it feel like the original game?

## Expected Review Output

Provide:
1. **Accuracy Assessment**: How well does the code match the original?
2. **Behavioral Issues**: Any deviations from expected behavior
3. **Improvement Suggestions**: How to make it more accurate
4. **Test Recommendations**: Additional tests needed for verification
5. **BASIC Reference**: Specific line numbers in original code

Reference [DESIGN.md](../DESIGN.md) for game system architecture and [MIGRATION.md](../MIGRATION.md) for conversion patterns.

Focus on preserving the authentic Super Star Trek gaming experience while maintaining modern C# code quality.
