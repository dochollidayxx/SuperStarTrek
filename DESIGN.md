# Super Star Trek - Design Document

## Game Architecture Overview

This document outlines the design and architecture for the C# port of Super Star Trek, based on analysis of the original BASIC implementation.

## Core Game Systems

### 1. Galaxy System
- **8x8 Galaxy Grid**: The game world consists of 64 quadrants arranged in an 8x8 grid
- **Quadrant Coordinates**: Each quadrant is identified by coordinates (1-8, 1-8)
- **Sector System**: Each quadrant contains 8x8 sectors (64 sectors per quadrant)
- **Galaxy Generation**: Procedurally generated at game start with random distribution of Klingons, starbases, and stars

### 2. Game State Management
- **Time Management**: Stardate system with limited time to complete mission
- **Resource Management**: Energy, photon torpedoes, shield power
- **Ship Status**: Position, condition, damage states
- **Mission Progress**: Klingon ships remaining, starbases available

### 3. Combat System
- **Phaser Combat**: Energy-based weapons with range-dependent effectiveness
- **Torpedo Combat**: Limited-ammunition projectiles with straight-line travel
- **Enemy AI**: Klingon ships move and attack during player movement
- **Damage System**: Random damage to ship systems during combat

### 4. Navigation System
- **Course System**: 9-directional movement (1-9, with 9 = 1)
- **Warp Speed**: Variable speed affecting energy consumption and time passage
- **Quadrant Transitions**: Movement between quadrants with coordinate validation
- **Collision Detection**: Prevention of movement into occupied sectors

## Data Structures

### Galaxy Data (`G(8,8)`)
Each quadrant stores a 3-digit value encoding:
- **Hundreds digit**: Number of Klingon ships (0-3)
- **Tens digit**: Number of starbases (0-1)
- **Units digit**: Number of stars (1-8)

### Klingon Data (`K(3,3)`)
For each Klingon ship in current quadrant:
- **K(I,1)**: Sector X coordinate
- **K(I,2)**: Sector Y coordinate
- **K(I,3)**: Shield/health value

### Course Data (`C(9,2)`)
Direction vectors for 9-directional movement:
- **C(I,1)**: X component of direction I
- **C(I,2)**: Y component of direction I

### Damage Data (`D(8)`)
Damage state for each ship system:
- **Positive values**: System operational
- **Negative values**: System damaged (magnitude indicates repair time)

### Quadrant Display (`Q$`)
String representation of current quadrant (192 characters):
- **8x8 grid**: Each sector represented by 3 characters
- **Real-time updates**: Modified as objects move or are destroyed

## Key Game Variables

### Ship Status
- **Q1, Q2**: Current quadrant coordinates
- **S1, S2**: Current sector coordinates within quadrant
- **E**: Current energy level
- **P**: Photon torpedoes remaining
- **S**: Shield power level

### Mission Parameters
- **T**: Current stardate
- **T0**: Mission start stardate
- **T9**: Mission time limit (days)
- **K9**: Total Klingon ships remaining in galaxy
- **B9**: Total starbases in galaxy

### Current Quadrant State
- **K3**: Klingon ships in current quadrant
- **B3**: Starbases in current quadrant
- **S3**: Stars in current quadrant
- **D0**: Docked status (1 if docked, 0 if not)

## Command System

### Input Processing
1. **Command Parsing**: 3-character command recognition
2. **Parameter Validation**: Input validation for courses, amounts, etc.
3. **State Checking**: Verify system availability before execution
4. **Error Handling**: Appropriate error messages for invalid inputs

### Command Flow
```
Main Game Loop:
1. Display current status (SRS if sensors working)
2. Check for game end conditions
3. Accept player command
4. Process command
5. Execute Klingon actions (if applicable)
6. Update game state
7. Repeat
```

## Game Flow

### Initialization Sequence
1. **Galaxy Generation**: Populate quadrants with random objects
2. **Ship Placement**: Position Enterprise in random location
3. **Mission Briefing**: Display objectives and time limit
4. **Initial Status**: Show starting quadrant information

### Turn Structure
1. **Player Input Phase**: Accept and validate command
2. **Action Resolution**: Execute player command
3. **Enemy Response**: Klingon movement and attacks
4. **System Updates**: Damage repair, energy consumption
5. **Status Display**: Update sensors and ship status
6. **End Condition Check**: Victory, defeat, or time limit

### Victory Conditions
- **Success**: All Klingon ships destroyed within time limit
- **Failure**: Enterprise destroyed, time limit exceeded, or critical mission failure

## Technical Implementation Notes

### Random Number Generation
- **Klingon Placement**: Random distribution across galaxy
- **Combat Outcomes**: Variable damage amounts
- **System Failures**: Random component damage during combat

### String Manipulation
- **Quadrant Display**: Complex string operations for real-time display updates
- **Position Calculations**: Converting between sector positions and string indices
- **Command Parsing**: Substring matching for command recognition

### Mathematical Systems
- **Distance Calculations**: Pythagorean theorem for range calculations
- **Course Calculations**: Trigonometric functions for direction/distance
- **Energy Management**: Complex formulas for movement and combat costs

## User Interface Design

### Display Elements
- **Short Range Sensors**: 8x8 grid showing current quadrant
- **Long Range Sensors**: 3x3 grid showing surrounding quadrants
- **Status Panel**: Ship condition, resources, mission progress
- **Command Interface**: Text-based command input and validation

### Information Hierarchy
1. **Critical Information**: Ship status, immediate threats
2. **Tactical Information**: Sensor data, enemy positions
3. **Strategic Information**: Galaxy map, mission progress
4. **Historical Information**: Cumulative records, previous actions

This design preserves the classic gameplay while enabling modern software engineering practices in the C# implementation.
