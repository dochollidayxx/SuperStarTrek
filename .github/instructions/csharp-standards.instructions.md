---
description: "C# coding standards for Super Star Trek project"
applyTo: "**/*.cs"
---

# C# Coding Standards for Super Star Trek

## Naming Conventions

### Game Domain Names
- Use game-specific terminology: `Enterprise`, `Klingon`, `Quadrant`, `Sector`
- Preserve original command names: `NavigateCommand`, `SensorScanCommand`, `PhaserFireCommand`
- Use descriptive names for game states: `CombatCondition`, `DockingStatus`, `ShieldLevel`

### C# Conventions
- Classes: PascalCase (`GameEngine`, `KlingonShip`)
- Methods: PascalCase (`FirePhasers`, `ScanQuadrant`)
- Properties: PascalCase (`CurrentQuadrant`, `EnergyLevel`)
- Fields: camelCase with underscore prefix (`_currentEnergy`, `_klingonShips`)
- Constants: PascalCase (`MaxTorpedoes`, `GalaxySize`)

## Code Organization

### Class Structure
```csharp
public class ExampleClass
{
    // Constants
    private const int MaxValue = 100;
    
    // Fields
    private readonly IService _service;
    private int _currentValue;
    
    // Constructor
    public ExampleClass(IService service)
    {
        _service = service;
    }
    
    // Properties
    public int CurrentValue => _currentValue;
    
    // Public methods
    public void PublicMethod() { }
    
    // Private methods
    private void PrivateMethod() { }
}
```

### File Organization
- One class per file
- Filename matches class name
- Organize files by game system (Combat/, Navigation/, Sensors/)

## Game-Specific Patterns

### Coordinate Systems
```csharp
// Always use 1-based indexing like original BASIC
public struct Coordinates
{
    public int X { get; } // 1-8
    public int Y { get; } // 1-8
}
```

### Game Commands
```csharp
// Use command pattern for user actions
public interface IGameCommand
{
    CommandResult Execute(GameState state);
}
```

### Random Number Generation
```csharp
// Use seeded random for reproducible behavior
public class GameRandom
{
    private readonly Random _random;
    
    public GameRandom(int? seed = null)
    {
        _random = seed.HasValue ? new Random(seed.Value) : new Random();
    }
}
```

## Error Handling

### Game-Specific Exceptions
```csharp
public class InvalidCoordinatesException : Exception
{
    public InvalidCoordinatesException(int x, int y) 
        : base($"Invalid coordinates: ({x}, {y})")
    {
    }
}
```

### Input Validation
```csharp
public void ValidateInput(string input)
{
    if (string.IsNullOrWhiteSpace(input))
        throw new ArgumentException("Input cannot be empty");
        
    // Validate against game rules
}
```

## Documentation Standards

### XML Documentation
```csharp
/// <summary>
/// Fires phasers at Klingon ships in the current quadrant.
/// </summary>
/// <param name="energyUnits">Amount of energy to allocate to phasers</param>
/// <returns>Result of the phaser attack</returns>
/// <exception cref="InsufficientEnergyException">Thrown when not enough energy available</exception>
public PhaserResult FirePhasers(int energyUnits)
{
    // Implementation
}
```

### Code Comments
```csharp
// Match original BASIC behavior: line 4480
var damage = CalculateDamage(distance, energy);

// TODO: Implement damage to multiple systems like original
```

## Performance Guidelines

### String Operations
- Use StringBuilder for complex string building
- Cache formatted strings when possible
- Avoid string concatenation in loops

### Memory Management
- Dispose of resources properly
- Use object pooling for frequently created objects
- Minimize allocations in game loop

### Game Loop Performance
- Keep frame rate consistent
- Avoid blocking operations
- Use async/await for I/O operations

## Testing Guidelines

### Unit Test Structure
```csharp
[Fact]
public void FirePhasers_WithSufficientEnergy_DestroyKlingon()
{
    // Arrange
    var game = new GameEngine();
    var klingon = new KlingonShip(100);
    
    // Act
    var result = game.FirePhasers(200);
    
    // Assert
    Assert.True(result.KlingonDestroyed);
}
```

### Test Naming
- Use descriptive test names: `Method_Condition_ExpectedResult`
- Test edge cases and boundary conditions
- Verify behavior matches original BASIC implementation
