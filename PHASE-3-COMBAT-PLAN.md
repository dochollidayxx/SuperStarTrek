# Phase 3: Combat Systems Implementation Plan

## Overview

Phase 3 focuses on implementing the complete combat system for Super Star Trek, including phaser combat, torpedo combat, and enemy AI. This phase builds on the foundation and navigation systems completed in Phases 1-2.

**Duration**: Weeks 5-6 (2 weeks)  
**Priority**: Critical path for playable game  
**Dependencies**: Galaxy generation, navigation system, display system  

## Current Status Assessment

### ✅ Already Implemented
- **Basic Command Structure**: `PhaserCommand` and `TorpedoCommand` classes exist
- **Klingon Model**: `KlingonShip` class with basic damage tracking
- **Command Validation**: Input parsing and error handling
- **System Damage Checks**: Phaser control and photon tubes validation

### 🚧 Partially Implemented
- **Phaser Combat**: Basic structure exists, needs complete damage calculation
- **Torpedo Combat**: Basic structure exists, needs trajectory and collision detection
- **Klingon Data**: Model exists but lacks full combat behavior

### ❌ Missing Implementation
- **Enemy AI**: Klingon movement and attack patterns
- **Damage Calculation**: Authentic BASIC damage formulas
- **Combat Resolution**: Complete hit detection and destruction logic
- **Tactical Feedback**: Combat result reporting and status updates

## Week 1 Tasks (Days 1-5)

### Day 1: Phaser Combat System
**Goal**: Complete phaser combat implementation with authentic damage calculations

#### Morning (2-3 hours)
- [ ] **Study Original BASIC**: Analyze lines 4250-4690 for exact phaser mechanics
- [ ] **Energy Distribution**: Implement automatic energy distribution among targets
- [ ] **Range Calculation**: Port distance-based damage reduction formula
- [ ] **Computer Damage Effects**: Implement accuracy penalties when computer is damaged

#### Afternoon (2-3 hours)
- [ ] **Damage Application**: Complete shield reduction and destruction logic
- [ ] **Combat Messages**: Implement authentic BASIC combat text output
- [ ] **Unit Tests**: Write comprehensive tests for all phaser scenarios
- [ ] **Integration Testing**: Test phaser combat in game context

### Day 2: Torpedo Combat System
**Goal**: Complete torpedo trajectory and collision detection

#### Morning (2-3 hours)
- [ ] **Study Original BASIC**: Analyze lines 4690-5490 for torpedo mechanics
- [ ] **Trajectory Calculation**: Implement straight-line movement with original C array
- [ ] **Collision Detection**: Port step-by-step collision checking algorithm
- [ ] **Hit Resolution**: Implement damage application for various targets

#### Afternoon (2-3 hours)
- [ ] **Obstacle Handling**: Implement star collision and destruction
- [ ] **Miss Handling**: Implement torpedo missing target scenarios
- [ ] **Ammunition Tracking**: Ensure proper torpedo count management
- [ ] **Unit Tests**: Write comprehensive torpedo combat tests

### Day 3: Combat Integration & Polish
**Goal**: Integrate combat systems and refine behavior

#### Morning (2-3 hours)
- [ ] **Combat Validation**: Test all combat scenarios against original BASIC
- [ ] **Edge Cases**: Handle boundary conditions and error scenarios
- [ ] **Message Formatting**: Ensure authentic combat text output
- [ ] **Energy Management**: Verify proper energy consumption and transfer

#### Afternoon (2-3 hours)
- [ ] **Performance Optimization**: Optimize combat calculations
- [ ] **Code Cleanup**: Refactor and document combat classes
- [ ] **Integration Tests**: Full combat scenario testing
- [ ] **Bug Fixes**: Address any issues discovered during testing

### Day 4: Enemy AI Foundation
**Goal**: Implement basic Klingon movement and AI behavior

#### Morning (2-3 hours)
- [ ] **Study Original BASIC**: Analyze Klingon movement patterns (lines vary)
- [ ] **Movement System**: Implement Klingon movement during player actions
- [ ] **Position Validation**: Ensure Klingons don't move into occupied sectors
- [ ] **Movement Triggers**: Implement movement during combat and navigation

#### Afternoon (2-3 hours)
- [ ] **Basic AI Logic**: Implement simple Klingon decision making
- [ ] **Attack Patterns**: Basic Klingon firing behavior
- [ ] **Unit Tests**: Test Klingon movement and basic AI
- [ ] **Integration Testing**: Test enemy behavior in combat scenarios

### Day 5: Combat Feedback & Status
**Goal**: Complete combat result reporting and game state updates

#### Morning (2-3 hours)
- [ ] **Combat Results**: Implement detailed combat outcome reporting
- [ ] **Status Updates**: Update game state after combat actions
- [ ] **Victory Conditions**: Check for quadrant clearing and mission progress
- [ ] **Damage Reports**: Integrate combat damage with ship systems

#### Afternoon (2-3 hours)
- [ ] **Testing & Validation**: Comprehensive combat system testing
- [ ] **Documentation**: Update NAVIGATION-GUIDE.md with combat examples
- [ ] **Code Review**: Review all combat-related code for quality
- [ ] **Week 1 Wrap-up**: Prepare for Week 2 enemy AI focus

## Week 2 Tasks (Days 6-10)

### Day 6: Advanced Enemy AI
**Goal**: Implement sophisticated Klingon attack patterns

#### Morning (2-3 hours)
- [ ] **Attack Calculation**: Implement Klingon damage calculation against Enterprise
- [ ] **Targeting Logic**: Klingon target selection and firing decisions
- [ ] **Range Factors**: Distance-based accuracy for Klingon attacks
- [ ] **Damage Application**: Apply Klingon damage to Enterprise shields

#### Afternoon (2-3 hours)
- [ ] **Tactical Responses**: Klingon behavior based on combat situation
- [ ] **Evasion Patterns**: Klingon movement to avoid Enterprise attacks
- [ ] **Unit Tests**: Test advanced AI behaviors
- [ ] **Balance Testing**: Ensure fair and challenging combat

### Day 7: Shield System Integration
**Goal**: Complete shield system interaction with combat

#### Morning (2-3 hours)
- [ ] **Shield Damage**: Implement shield absorption of incoming damage
- [ ] **Energy Transfer**: Complete shield/energy management during combat
- [ ] **Shield Status**: Display shield levels during and after combat
- [ ] **Critical Damage**: Handle hull damage when shields are down

#### Afternoon (2-3 hours)
- [ ] **Automatic Systems**: Implement automatic shield behavior
- [ ] **Emergency Procedures**: Handle shield failure scenarios
- [ ] **Integration Testing**: Test shield system with all combat scenarios
- [ ] **Performance Validation**: Ensure smooth shield operations

### Day 8: Combat Balance & Tuning
**Goal**: Fine-tune combat for authentic gameplay experience

#### Morning (2-3 hours)
- [ ] **Damage Formulas**: Validate all damage calculations against original
- [ ] **Probability Tuning**: Ensure hit/miss rates match original behavior
- [ ] **Energy Balance**: Verify energy costs and consumption rates
- [ ] **Difficulty Curve**: Test progression from easy to hard encounters

#### Afternoon (2-3 hours)
- [ ] **Edge Case Testing**: Test extreme scenarios and boundary conditions
- [ ] **Random Seed Testing**: Verify consistent behavior with same seeds
- [ ] **Performance Benchmarking**: Ensure acceptable combat performance
- [ ] **User Experience**: Test combat flow and feedback quality

### Day 9: Integration & Testing
**Goal**: Complete integration with existing systems

#### Morning (2-3 hours)
- [ ] **Navigation Integration**: Test combat during and after movement
- [ ] **Sensor Integration**: Ensure sensors work correctly during combat
- [ ] **Command Integration**: Test all combat commands in game context
- [ ] **State Persistence**: Verify game state consistency after combat

#### Afternoon (2-3 hours)
- [ ] **Full Game Testing**: Test complete gameplay scenarios with combat
- [ ] **Regression Testing**: Ensure no breaking changes to existing features
- [ ] **Error Handling**: Test error scenarios and edge cases
- [ ] **Documentation Updates**: Update all relevant documentation

### Day 10: Polish & Completion
**Goal**: Finalize Phase 3 and prepare for Phase 4

#### Morning (2-3 hours)
- [ ] **Code Review**: Final review of all combat system code
- [ ] **Performance Final**: Final performance optimization pass
- [ ] **Documentation**: Complete combat system documentation
- [ ] **Test Coverage**: Ensure 100% test coverage for combat systems

#### Afternoon (2-3 hours)
- [ ] **Phase 3 Validation**: Verify all Phase 3 objectives completed
- [ ] **Migration Plan Update**: Update MIGRATION.md with Phase 3 completion
- [ ] **Phase 4 Preparation**: Prepare foundation for ship systems phase
- [ ] **Deliverable Review**: Final quality check and sign-off

## Technical Implementation Details

### Phaser Combat Algorithm (Original BASIC Lines 4250-4690)

```csharp
// Energy distribution among targets
foreach (var klingon in klingons)
{
    var distance = CalculateDistance(enterprise.Position, klingon.Position);
    var energyPerTarget = totalEnergy / klingons.Count;
    
    // Apply computer damage penalty if applicable
    if (enterprise.GetSystemDamage(ShipSystem.LibraryComputer) < 0)
    {
        energyPerTarget *= RandomFactor(0.5, 1.0); // Reduced accuracy
    }
    
    // Distance-based damage reduction
    var damage = (int)(energyPerTarget * (1.0 - distance / 10.0));
    var destroyed = klingon.TakeDamage(damage);
    
    if (destroyed)
    {
        // Handle Klingon destruction
        UpdateQuadrantDisplay();
        CheckVictoryConditions();
    }
}
```

### Torpedo Combat Algorithm (Original BASIC Lines 4690-5490)

```csharp
// Calculate trajectory using original C array direction vectors
var direction = GetDirectionVector(course);
var currentX = enterprise.SectorCoordinates.X;
var currentY = enterprise.SectorCoordinates.Y;

// Step-by-step movement along trajectory
for (int step = 1; step <= 8; step++)
{
    currentX += direction.X;
    currentY += direction.Y;
    
    // Check boundaries
    if (currentX < 1 || currentX > 8 || currentY < 1 || currentY > 8)
    {
        return "TORPEDO MISSED";
    }
    
    // Check for collisions
    var target = GetSectorContent(currentX, currentY);
    if (target != SectorContent.Empty)
    {
        return HandleTorpedoHit(target, currentX, currentY);
    }
}
```

### Enemy AI Decision Tree

```csharp
public class KlingonAI
{
    public KlingonAction DecideAction(KlingonShip klingon, GameState gameState)
    {
        var distanceToEnterprise = klingon.DistanceTo(gameState.Enterprise.SectorCoordinates);
        
        // Close range: Attack
        if (distanceToEnterprise <= 2.0)
        {
            return new AttackAction(CalculateAttackPower(klingon, distanceToEnterprise));
        }
        
        // Medium range: Move closer or attack
        if (distanceToEnterprise <= 5.0)
        {
            return Random.NextDouble() < 0.7 
                ? new MoveAction(GetCloserPosition(klingon, gameState.Enterprise))
                : new AttackAction(CalculateAttackPower(klingon, distanceToEnterprise));
        }
        
        // Long range: Move closer
        return new MoveAction(GetCloserPosition(klingon, gameState.Enterprise));
    }
}
```

## Testing Strategy

### Unit Test Categories
1. **Phaser Combat Tests**
   - Energy distribution accuracy
   - Range-based damage calculation
   - Computer damage effects
   - Target destruction logic

2. **Torpedo Combat Tests**
   - Trajectory calculation
   - Collision detection
   - Hit/miss scenarios
   - Multi-target interactions

3. **Enemy AI Tests**
   - Movement validation
   - Attack decision logic
   - Response to player actions
   - Edge case behavior

4. **Integration Tests**
   - Full combat scenarios
   - Multi-system interaction
   - State consistency
   - Performance validation

### Test Data Requirements
- Sample game states with various Klingon configurations
- Edge case scenarios (single Klingon, multiple Klingons, damaged systems)
- Performance test data for large-scale combat
- Reference data from original BASIC for validation

## Success Criteria

### Functional Requirements
- [ ] Phaser combat works identically to original BASIC
- [ ] Torpedo combat with accurate trajectory and collision detection
- [ ] Klingon AI provides challenging and authentic gameplay
- [ ] All combat commands integrated with game systems
- [ ] Victory/defeat conditions correctly triggered

### Technical Requirements
- [ ] All combat code covered by unit tests (>95%)
- [ ] Performance acceptable for real-time combat
- [ ] Clean, maintainable code following project standards
- [ ] Complete integration with existing navigation and display systems

### Quality Requirements
- [ ] Authentic gameplay experience matching original
- [ ] Robust error handling for all edge cases
- [ ] Comprehensive documentation and code comments
- [ ] No regression in existing functionality

## Risk Management

### Identified Risks
1. **Complexity Risk**: Combat system interactions are complex
   - *Mitigation*: Incremental development with continuous testing

2. **Authenticity Risk**: Behavior might not match original exactly
   - *Mitigation*: Constant validation against original BASIC code

3. **Performance Risk**: Combat calculations might be slow
   - *Mitigation*: Performance testing and optimization throughout

4. **Integration Risk**: Combat might break existing systems
   - *Mitigation*: Comprehensive regression testing

### Contingency Plans
- If phaser system is complex: Focus on core functionality first, polish later
- If torpedo trajectory is problematic: Implement simplified version, then enhance
- If AI is too complex: Start with basic behavior, add sophistication incrementally
- If integration issues arise: Use feature flags to isolate problematic components

## Next Phase Preparation

Phase 3 completion prepares for Phase 4 (Ship Systems) by:
- Establishing damage system integration points
- Providing combat validation for shield systems
- Creating foundation for repair mechanics
- Validating energy management across all systems

**Phase 4 Dependencies Met**:
- ✅ Combat damage application system
- ✅ Shield interaction framework
- ✅ Energy consumption validation
- ✅ System damage effects testing

This plan ensures systematic development of authentic, robust combat systems that maintain the classic Super Star Trek gameplay experience while leveraging modern development practices.
