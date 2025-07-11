# Super Star Trek Navigation System Guide

## Overview

The navigation system in Super Star Trek uses a **9-direction course system** with **interpolation** between cardinal and intercardinal directions. This guide explains exactly how navigation should work based on the original 1978 BASIC implementation.

## Course Direction System

### Course Numbers (1-9)
The original BASIC uses courses 1-8 with course 9 wrapping to course 1:

```
        1 (North)
    8       2 (Northeast) 
7 (West)     3 (East)
    6       4 (Southeast)
        5 (South)
```

### Direction Vectors (C Array)

The original BASIC code initializes direction vectors in the C array:

```basic
530 FORI=1TO9:C(I,1)=0:C(I,2)=0:NEXTI
540 C(3,1)=-1:C(2,1)=-1:C(4,1)=-1:C(4,2)=-1:C(5,2)=-1:C(6,2)=-1
600 C(1,2)=1:C(2,2)=1:C(6,1)=1:C(7,1)=1:C(8,1)=1:C(8,2)=1:C(9,2)=1
```

This creates the following mapping:

| Course | Description | C(I,1) X-Direction | C(I,2) Y-Direction |
|--------|-------------|--------------------|--------------------|
| 1      | North       | 0                  | 1                  |
| 2      | Northeast   | -1                 | 1                  |
| 3      | East        | -1                 | 0                  |
| 4      | Southeast   | -1                 | -1                 |
| 5      | South       | 0                  | -1                 |
| 6      | Southwest   | 1                  | -1                 |
| 7      | West        | 1                  | 0                  |
| 8      | Northwest   | 1                  | 1                  |
| 9      | → 1         | 0                  | 1                  |

## Coordinate System

### Galaxy Structure
- **8x8 Galaxy** of quadrants (1,1) to (8,8)
- Each **quadrant** contains **8x8 sectors** (1,1) to (8,8)
- **Total: 64x64 sectors** in absolute coordinates

### Coordinate Interpretation
- **X-axis**: Quadrant columns (1=leftmost, 8=rightmost)
- **Y-axis**: Quadrant rows (1=topmost, 8=bottommost)
- **Direction vectors** work in sector space within quadrants:
  - **Negative X** = Move toward smaller sector X within quadrant
  - **Positive X** = Move toward larger sector X within quadrant  
  - **Negative Y** = Move toward smaller sector Y within quadrant
  - **Positive Y** = Move toward larger sector Y within quadrant
- **Quadrant transitions** occur when sector coordinates exceed 1-8 range

## Navigation Algorithm

### Course Interpolation
The original BASIC supports **fractional courses** between whole numbers:

```basic
3110 X1=C(C1,1)+(C(C1+1,1)-C(C1,1))*(C1-INT(C1)):X=S1:Y=S2
3140 X2=C(C1,2)+(C(C1+1,2)-C(C1,2))*(C1-INT(C1)):Q4=Q1:Q5=Q2
```

**Formula**:
- `deltaX = C[course,1] + (C[course+1,1] - C[course,1]) * fractional_part`
- `deltaY = C[course,2] + (C[course+1,2] - C[course,2]) * fractional_part`

### Distance Calculation
```basic
2490 N=INT(W1*8+.5)
```
- **Distance in sectors** = `warp_factor × 8`
- Movement is applied as: `deltaX × distance`, `deltaY × distance`

### Energy Consumption
```basic
3910 E=E-N-10
```
- **Energy required** = `distance + 10`
- **Additional energy** for maneuvering and life support

## Navigation Examples

### Example 1: Basic Cardinal Movement
**Command**: `NAV 3 1` (East, Warp 1)
- **Course**: 3 (East)
- **Direction**: deltaX = -1, deltaY = 0  
- **Distance**: 1 × 8 = 8 sectors
- **Movement**: 8 sectors in sector space (may cross quadrants)
- **Energy**: 8 + 10 = 18 units

### Example 2: Diagonal Movement  
**Command**: `NAV 2 0.5` (Northeast, Warp 0.5)
- **Course**: 2 (Northeast)
- **Direction**: deltaX = -1, deltaY = 1
- **Distance**: 0.5 × 8 = 4 sectors
- **Movement**: 4 sectors northeast
- **Energy**: 4 + 10 = 14 units

### Example 3: Fractional Course
**Command**: `NAV 1.5 1` (North-Northeast, Warp 1)
- **Course**: 1.5 (halfway between North and Northeast)
- **Interpolation**: 
  - deltaX = 0 + (-1 - 0) × 0.5 = -0.5
  - deltaY = 1 + (1 - 1) × 0.5 = 1.0
- **Distance**: 8 sectors
- **Movement**: (-4, 8) sectors (northwest direction)

### Example 4: Quadrant Transition
**Command**: `NAV 5 2` (South, Warp 2) from sector (4,7)
- **Course**: 5 (South)
- **Direction**: deltaX = 0, deltaY = -1
- **Distance**: 2 × 8 = 16 sectors  
- **Movement**: 16 sectors south
- **Result**: Cross into adjacent quadrant

### Example 5: Long Range Navigation
**Command**: `NAV 7 4` (West, Warp 4)
- **Course**: 7 (West)
- **Direction**: deltaX = 1, deltaY = 0
- **Distance**: 4 × 8 = 32 sectors
- **Movement**: 32 sectors west (4 quadrants)
- **Energy**: 32 + 10 = 42 units

### Example 6: Precision Movement
**Command**: `NAV 6 0.25` (Southwest, Warp 0.25)
- **Course**: 6 (Southwest)
- **Direction**: deltaX = 1, deltaY = -1  
- **Distance**: 0.25 × 8 = 2 sectors
- **Movement**: 2 sectors southwest
- **Energy**: 2 + 10 = 12 units

### Example 7: Course Wrapping
**Command**: `NAV 9 1` (same as NAV 1 1)
- **Course**: 9 → 1 (North)
- **Direction**: deltaX = 0, deltaY = 1
- **Distance**: 8 sectors north

### Example 8: Intercardinal Precision
**Command**: `NAV 4.7 1.5` (Southeast-South, Warp 1.5)
- **Course**: 4.7 (between Southeast and South)
- **Interpolation**:
  - deltaX = -1 + (0 - (-1)) × 0.7 = -0.3
  - deltaY = -1 + (-1 - (-1)) × 0.7 = -1.0
- **Distance**: 1.5 × 8 = 12 sectors
- **Movement**: (-3.6, -12) sectors

### Example 9: Navigation from Galaxy Edge
**Command**: `NAV 3 1` from quadrant (8,4), sector (6,4)
- **Course**: 3 (East), deltaX = -1, deltaY = 0
- **Movement**: 8 sectors with deltaX = -1
- **Calculation**: Absolute position (62,28) + (-8,0) = (54,28)
- **Result**: Valid movement within galaxy boundaries - navigation succeeds

### Example 10: Emergency Low Warp
**Command**: `NAV 1 0.1` (North, minimal warp)
- **Course**: 1 (North)
- **Distance**: 0.1 × 8 = 0.8 sectors
- **Movement**: Less than 1 sector north
- **Energy**: 1 + 10 = 11 units (minimum)

### Example 11: High-Speed Transit
**Command**: `NAV 8 6` (Northwest, high warp)
- **Course**: 8 (Northwest)
- **Direction**: deltaX = 1, deltaY = 1
- **Distance**: 6 × 8 = 48 sectors
- **Movement**: Cross multiple quadrants northwest
- **Energy**: 48 + 10 = 58 units

### Example 12: Course Correction
**Command**: `NAV 3.2 0.8` (slightly south of east)
- **Course**: 3.2 (East + 20% toward Southeast)
- **Interpolation**:
  - deltaX = -1 + (-1 - (-1)) × 0.2 = -1.0
  - deltaY = 0 + (-1 - 0) × 0.2 = -0.2
- **Distance**: 6.4 sectors
- **Movement**: Mostly east, slightly south

### Example 13: Docking Approach  
**Command**: `NAV 7.5 0.3` (West-Northwest, slow approach)
- **Course**: 7.5 (between West and Northwest)
- **Distance**: 2.4 sectors
- **Purpose**: Careful approach to starbase
- **Energy**: 2 + 10 = 12 units

### Example 14: Combat Maneuver
**Command**: `NAV 6.8 0.4` (Southwest-West, tactical)
- **Course**: 6.8 (mostly southwest, toward west)
- **Distance**: 3.2 sectors  
- **Purpose**: Flanking maneuver around Klingons
- **Energy**: 3 + 10 = 13 units

### Example 15: Maximum Range
**Command**: `NAV 4 8` (Southeast, maximum warp)
- **Course**: 4 (Southeast)
- **Distance**: 8 × 8 = 64 sectors
- **Movement**: Traverse entire galaxy diagonally
- **Energy**: 64 + 10 = 74 units
- **Risk**: High energy consumption, potential boundary violation

## Navigation Constraints

### Energy Requirements
- **Minimum**: 11 units (for any movement)
- **Calculation**: `(warp_factor × 8) + 10`
- **Insufficient energy**: Navigation rejected

### Warp Factor Limits
- **Normal**: 0.1 to 8.0
- **Damaged engines**: Maximum 0.2
- **Zero warp**: Returns to command prompt

### Boundary Checking
- **Galaxy limits**: Absolute coordinates 1-64 in both X and Y
- **Violation**: "NAVIGATION WOULD TAKE SHIP OUTSIDE GALAXY"
- **Emergency stop**: Automatic boundary enforcement

### Collision Detection
- **Stars**: Emergency stop if path intersects star
- **Objects**: Cannot occupy same sector as game objects

## Technical Implementation Notes

### Original BASIC Algorithm
The movement loop processes one sector at a time, checking for collisions:

```basic
3170 FORI=1TON:S1=S1+X1:S2=S2+X2:IFS1<1ORS1>=9ORS2<1ORS2>=9THEN3500
3240 S8=INT(S1)*24+INT(S2)*3-26:IFMID$(Q$,S8,2)="  "THEN3360
3320 S1=INT(S1-X1):S2=INT(S2-X2):PRINT"WARP ENGINES SHUT DOWN AT ";
```

### Time Progression
- **Normal warp**: 1 stardate per movement
- **Fractional warp**: Proportional time consumption

This navigation system provides precise control over starship movement while maintaining the classic Star Trek feel of the original 1978 game.
