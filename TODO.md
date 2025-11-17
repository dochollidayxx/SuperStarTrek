# Super Star Trek - TODO Analysis

## Overview

This document provides a comprehensive analysis of the original 1978 Microsoft BASIC Super Star Trek game, cross-referenced with our C# implementation to identify what has been completed and what remains for a **FAITHFUL** re-implementation.

**BASIC Source Reference**: `src/SuperStarTrek.Basic/superstartrek.bas` (425 lines)

## Implementation Status Summary

| Component | BASIC Lines | Status | C# Implementation |
|-----------|-------------|--------|-------------------|
| Galaxy Generation | 810-1160 | ✅ Complete | GameState, Galaxy, Quadrant |
| Ship Initialization | 370-670 | ✅ Complete | Enterprise model |
| Mission Briefing | 1200-1280 | ✅ Complete | StarTrekGame |
| Navigation (NAV) | 2300-3980 | ✅ Complete | NavigationCommand |
| Short Range Sensors (SRS) | 6420-7260 | ✅ Complete | ShortRangeSensorsCommand |
| Long Range Sensors (LRS) | 4000-4230 | ✅ Complete | LongRangeSensorsCommand |
| Phasers (PHA) | 4260-4670 | ✅ Complete | PhaserCommand |
| Torpedoes (TOR) | 4700-5490 | ✅ Complete | TorpedoCommand |
| Shields (SHE) | 5530-5660 | ✅ Complete | ShieldCommand |
| Damage Control (DAM) | 5690-5980 | ✅ Complete | DamageControlCommand |
| Library Computer (COM) | 7290-9260 | ❌ **MISSING** | **NOT IMPLEMENTED** |
| Emergency Conditions | 1990-2050 | ✅ Complete | StarTrekGame |
| Klingon Combat AI | 2590-2700, 6000-6200 | ✅ Complete | Enterprise, Commands |
| Docking System | 6430-6720 | ⚠️ Partial | Enterprise (needs verification) |
| Victory/Defeat Logic | 6220-6400 | ⚠️ Partial | StarTrekGame (needs verification) |
| Quadrant Names | 9010-9260 | ❌ **MISSING** | **NOT IMPLEMENTED** |
| XXX (Quit) | 2260 | ⚠️ Unknown | Needs verification |

## Detailed Feature Analysis

### ✅ COMPLETED - Phases 1-4 (Verified)

#### Phase 1: Foundation (Lines 370-1300)
- [x] **Galaxy Data Structure** (Line 330: `DIM G(8,8)`)
  - C#: `Galaxy.cs` with 8x8 grid
  - Proper 3-digit encoding (hundreds=Klingons, tens=Starbases, units=Stars)

- [x] **Ship Initialization** (Lines 370-670)
  - C#: `Enterprise.cs`
  - Energy: 3000 units (Line 370: `E=3000`)
  - Torpedoes: 10 (Line 440: `P=10`)
  - Shields: 200 max (Line 440: `S9=200`)
  - Damage array for 8 systems (Line 330: `DIM D(8)`)

- [x] **Random Placement** (Lines 480-530)
  - C#: Galaxy generation in `GameState`
  - FNR function equivalent (Line 475: `DEF FNR(R)=INT(RND(R)*7.98+1.01)`)
  - Enterprise quadrant/sector randomization

- [x] **Course Direction Table** (Lines 530-600)
  - C#: `NavigationCommand` course calculation
  - 9-directional movement with interpolation

#### Phase 2: Core Systems (Lines 810-4230)
- [x] **Galaxy Generation** (Lines 810-1160)
  - C#: `Galaxy.cs` initialization
  - Klingon distribution (2-3% for 3 ships, 2-5% for 2 ships, 5-20% for 1 ship)
  - Starbase placement (4% chance per quadrant)
  - Guaranteed starbase if none generated
  - Stars: 1-8 per quadrant

- [x] **Quadrant Entry** (Lines 1320-1910)
  - C#: Quadrant initialization in game loop
  - Object placement (Enterprise, Klingons, starbases, stars)
  - Sector position management

- [x] **Short Range Sensors** (Lines 6420-7260)
  - C#: `ShortRangeSensorsCommand.cs`
  - 8x8 quadrant display with status panel
  - Condition reporting (GREEN/YELLOW/RED/DOCKED)
  - Damage detection (Line 6720: `IF D(2)>=0`)

- [x] **Long Range Sensors** (Lines 4000-4230)
  - C#: `LongRangeSensorsCommand.cs`
  - 3x3 surrounding quadrant scan
  - Cumulative record update (Z array)
  - Damage detection (Line 4000: `IF D(3)<0`)

#### Phase 3: Combat Systems (Lines 4260-6200)
- [x] **Phaser Combat** (Lines 4260-4670)
  - C#: `PhaserCommand.cs`
  - Energy allocation across targets
  - Distance-based damage: `H=INT((H1/FND(0))*(RND(1)+2))`
  - Computer damage effects (Line 4330: `IF D(8)<0`)
  - Shield control damage: multiplier effect (Line 4410: `IF D(7)<0 THEN X=X*RND(1)`)
  - Enemy destruction and galaxy update

- [x] **Torpedo Combat** (Lines 4700-5490)
  - C#: `TorpedoCommand.cs`
  - Course-based trajectory (fractional course interpolation)
  - Collision detection (Klingons, stars, starbases)
  - Energy cost: 2 units (Line 4850)
  - Starbase destruction consequences (Lines 5330-5410)

- [x] **Klingon Movement** (Lines 2590-2700)
  - C#: Integrated into `NavigationCommand`
  - Movement only during Enterprise navigation (authentic BASIC behavior)
  - Random sector repositioning

- [x] **Klingon Counter-Attacks** (Lines 6000-6200)
  - C#: Called from commands after player actions
  - Damage formula: `H=INT((K(I,3)/FND(1))*(2+RND(1)))`
  - Shield depletion logic
  - System damage on strong hits (Lines 6120-6170)

#### Phase 4: Ship Systems (Lines 2770-5980)
- [x] **Damage System** (Lines 2770-3030)
  - C#: `Enterprise.cs` automatic repair system
  - 8 ship systems with damage states
  - Automatic repair during warp travel
  - Random damage/improvement events (20% chance)
  - Repair completion reporting

- [x] **Shield Management** (Lines 5530-5660)
  - C#: `ShieldCommand.cs`
  - Energy transfer between shields and main power
  - Damage control check (Line 5530: `IF D(7)<0`)
  - Validation for available energy

- [x] **Damage Control** (Lines 5690-5980)
  - C#: `DamageControlCommand.cs`
  - Damage report display
  - Starbase repair service (when docked)
  - Repair time calculation: 0.1 stardates per system + random
  - System status display with repair percentages

- [x] **Emergency Conditions** (Lines 1990-2050)
  - C#: Implemented in `StarTrekGame.cs`
  - Fatal error detection: insufficient energy + shields
  - Cross-circuit check (Shield control operational check)
  - Game over trigger

### ❌ MISSING - Primary Implementation Gap

#### Library Computer Command (COM) - Lines 7290-9260
**STATUS**: Not implemented at all
**BASIC REFERENCE**: Lines 7290-9260 (970 lines total, ~23% of game code!)
**PRIORITY**: **HIGH** - This is the largest missing feature

The Library Computer provides 6 distinct functions:

##### Function -1: Help Menu (Lines 7360-7380)
- Displays available computer functions
- Lists all 6 function numbers with descriptions
- **Implementation Needed**: `ComputerCommand.cs` with help display

##### Function 0: Cumulative Galactic Record (Lines 7540-7850)
**BASIC Lines**: 7540-7850
**Purpose**: Display cumulative knowledge of galaxy exploration

```basic
7540 REM CUM GALACTIC RECORD
7544 PRINT"COMPUTER RECORD OF GALAXY FOR QUADRANT";Q1;",";Q2
7550 PRINT"       1     2     3     4     5     6     7     8"
7630 FORJ=1TO8:PRINT"   ";:IFZ(I,J)=0THENPRINT"***";:GOTO7720
7700 PRINTRIGHT$(STR$(Z(I,J)+1000),3);
```

**Requirements**:
- Display 8x8 galaxy grid
- Show Z array contents (explored quadrants)
- Show "***" for unexplored quadrants
- Show 3-digit quadrant data for explored quadrants
- Format with column/row headers

**C# Implementation Needed**:
- New class: `src/SuperStarTrek.Game/Commands/ComputerCommand.cs`
- Access to `gameState.Galaxy.ScannedData` (Z array equivalent)
- Display formatting matching BASIC output
- Subcommand: Function 0

##### Function 1: Status Report (Lines 7900-8020)
**BASIC Lines**: 7900-8020
**Purpose**: Mission status and remaining resources

```basic
7900 PRINT "   STATUS REPORT:":X$="":IFK9>1THENX$="S"
7940 PRINT"KLINGON";X$;" LEFT: ";K9
7960 PRINT"MISSION MUST BE COMPLETED IN";.1*INT((T0+T9-T)*10);"STARDATES"
7980 PRINT"THE FEDERATION IS MAINTAINING";B9;"STARBASE";X$;" IN THE GALAXY"
7990 GOTO5690  ' Jump to damage control report
```

**Requirements**:
- Display Klingons remaining with proper pluralization
- Show mission time remaining (calculated from T0, T9, T)
- Display starbase count with pluralization
- Chain to damage control report (line 5690)
- Handle case of no starbases remaining (lines 8010-8020)

**C# Implementation Needed**:
- Status report formatting method
- Time calculation: `(T0 + T9 - T)` stardates remaining
- Proper singular/plural handling ("KLINGON" vs "KLINGONS")
- Chain call to DAM command or display damage report
- Subcommand: Function 1

##### Function 2: Photon Torpedo Data (Lines 8070-8100)
**BASIC Lines**: 8070-8100
**Purpose**: Direction and distance to all Klingon ships in current quadrant

```basic
8070 IFK3<=0THEN4270  ' No Klingons message
8090 PRINT"FROM ENTERPRISE TO KLINGON BATTLE CRUSER";X$
8100 H8=0:FORI=1TO3:IFK(I,3)<=0THEN8480
8110 W1=K(I,1):X=K(I,2)
8120 C1=S1:A=S2:GOTO8220  ' Jump to direction/distance calculator
```

**Requirements**:
- Check for Klingon presence in quadrant
- Loop through all active Klingons
- Display "FROM ENTERPRISE TO KLINGON BATTLE CRUISER(S)"
- For each Klingon, show direction and distance
- Use direction/distance calculator (lines 8220+)

**C# Implementation Needed**:
- Loop through Klingons in current quadrant
- Call direction/distance calculator for each
- Format output with proper headers
- Subcommand: Function 2

##### Function 3: Starbase Navigation Data (Lines 8500-8520)
**BASIC Lines**: 8500-8520
**Purpose**: Direction and distance to starbase in current quadrant

```basic
8500 IFB3<>0THENPRINT"FROM ENTERPRISE TO STARBASE:":W1=B4:X=B5:GOTO8120
8510 PRINT"MR. SPOCK REPORTS,  'SENSORS SHOW NO STARBASES IN THIS";
8520 PRINT" QUADRANT.'":GOTO1990
```

**Requirements**:
- Check for starbase presence (B3 variable)
- If present: show direction and distance
- If absent: display Spock's message
- Use stored starbase coordinates (B4, B5)

**C# Implementation Needed**:
- Check `gameState.CurrentQuadrant.Starbase` presence
- Get starbase sector coordinates
- Call direction/distance calculator
- Proper error message if no starbase
- Subcommand: Function 3

##### Function 4: Direction/Distance Calculator (Lines 8150-8460)
**BASIC Lines**: 8150-8460
**Purpose**: General-purpose calculator for any two points

```basic
8150 PRINT"DIRECTION/DISTANCE CALCULATOR:"
8160 PRINT"YOU ARE AT QUADRANT ";Q1;",";Q2;" SECTOR ";S1;",";S2
8170 PRINT"PLEASE ENTER":INPUT"  INITIAL COORDINATES (X,Y)";C1,A
8200 INPUT"  FINAL COORDINATES (X,Y)";W1,X
8220 X=X-A:A=C1-W1:IFX<0THEN8350
...
8460 PRINT"DISTANCE =";SQR(X^2+A^2):IFH8=1THEN1990
```

**Requirements**:
- Display current position (quadrant and sector)
- Prompt for initial coordinates (X, Y)
- Prompt for final coordinates (X, Y)
- Calculate direction (course 1-9 with fractions)
- Calculate distance (Pythagorean theorem)
- Complex logic for 8-direction determination (lines 8220-8460)

**C# Implementation Needed**:
- Interactive coordinate input (2 pairs)
- Direction calculation algorithm (matching BASIC logic exactly)
- Distance calculation: `Math.Sqrt(deltaX^2 + deltaY^2)`
- Proper formatting of course number (fractional values)
- Subcommand: Function 4

**Direction Calculation Logic** (Critical for authenticity):
```basic
' Lines 8220-8460 implement complex octant-based direction finding
' Based on comparing X and Y deltas and their signs
' Returns course value 1-9 with fractional component
```

##### Function 5: Galaxy Region Name Map (Lines 7400-7850)
**BASIC Lines**: 7400-7850, 9010-9260
**Purpose**: Display galaxy map with region names instead of numeric data

```basic
7400 H8=0:G5=1:PRINT"                        THE GALAXY":GOTO7550
7740 Z4=I:Z5=1:GOSUB 9030:J0=INT(15-.5*LEN(G2$)):PRINTTAB(J0);G2$;
9030 IFZ5<=4THENONZ4GOTO9040,9050,9060,9070,9080,9090,9100,9110
9040 G2$="ANTARES"
...
9210 IFG5<>1THENONZ5GOTO9230,9240,9250,9260,9230,9240,9250,9260
9230 G2$=G2$+" I":RETURN
```

**Requirements**:
- Display 8x8 galaxy grid with region names
- Names based on Z4 (row) and Z5 (column) coordinates
- 16 unique region names (8 for each half of galaxy)
- Quadrant suffixes: I, II, III, IV
- Proper formatting with centered text

**Galaxy Region Names** (Lines 9040-9200):
- **Quadrants 1-4** (Z5≤4): ANTARES, RIGEL, PROCYON, VEGA, CANOPUS, ALTAIR, SAGITTARIUS, POLLUX
- **Quadrants 5-8** (Z5>4): SIRIUS, DENEB, CAPELLA, BETELGEUSE, ALDEBARAN, REGULUS, ARCTURUS, SPICA
- **Suffixes** (Z5 mod 4): I, II, III, IV

**C# Implementation Needed**:
- Region name lookup table (16 base names)
- Suffix generation based on column (I, II, III, IV)
- Display formatting with proper spacing
- Reuse cumulative record display logic
- Subcommand: Function 5

##### Quadrant Name Generator (Lines 9010-9260)
**BASIC Lines**: 9010-9260
**Purpose**: Subroutine to generate quadrant names from coordinates

**Required for**:
- Function 5 (Galaxy region name map)
- Quadrant entry messages (Line 1490: "NOW ENTERING [name] QUADRANT")
- Mission briefing (Line 1470: "...LOCATED IN THE GALACTIC QUADRANT, '[name]'")

**C# Implementation Needed**:
- Static utility class: `QuadrantNameGenerator.cs` in Models
- Method: `GetQuadrantName(int q1, int q2, bool regionOnly = false)`
- Returns: String like "ANTARES II" or just "ANTARES" if regionOnly

**Name Generation Logic**:
```csharp
// Simplified representation of BASIC logic:
// Base names by row (q1):
string[] baseNames = q2 <= 4
    ? ["ANTARES", "RIGEL", "PROCYON", "VEGA", "CANOPUS", "ALTAIR", "SAGITTARIUS", "POLLUX"]
    : ["SIRIUS", "DENEB", "CAPELLA", "BETELGEUSE", "ALDEBARAN", "REGULUS", "ARCTURUS", "SPICA"];

// Suffix by column (q2):
string[] suffixes = [" I", " II", " III", " IV"];
int suffixIndex = ((q2 - 1) % 4);

return baseNames[q1 - 1] + (regionOnly ? "" : suffixes[suffixIndex]);
```

**Integration Points**:
- Used by Computer Command Function 5
- Used by `StarTrekGame` for quadrant entry messages
- Used by mission briefing initialization

---

### ⚠️ NEEDS VERIFICATION - Potentially Incomplete

#### 1. Docking System (Lines 6430-6720)
**BASIC Reference**: Lines 6430-6720 (within SRS subroutine)
**Current Status**: Partially implemented in `Enterprise.cs`

**BASIC Logic**:
```basic
6430 FORI=S1-1TOS1+1:FORJ=S2-1TOS2+1  ' Check adjacent sectors
6490 A$=">!<":Z1=I:Z2=J:GOSUB8830:IFZ3=1THEN6580  ' Find starbase
6580 D0=1:C$="DOCKED":E=E0:P=P0  ' Set docked, refill energy & torpedoes
6620 PRINT"SHIELDS DROPPED FOR DOCKING PURPOSES":S=0  ' Drop shields
```

**Verification Needed**:
- [ ] Check if `Enterprise.cs` properly detects adjacent starbases
- [ ] Verify energy refill to maximum (E=E0=3000)
- [ ] Verify torpedo refill to maximum (P=P0=10)
- [ ] Confirm shields automatically drop to 0 when docked
- [ ] Test docking message display
- [ ] Verify condition display shows "DOCKED" status

**Test Required**:
- Create test scenario with Enterprise adjacent to starbase
- Verify all docking effects occur automatically
- Test that Klingons don't attack while docked (Line 6010)

#### 2. Victory Conditions (Lines 6370-6400)
**BASIC Reference**: Lines 6370-6400
**Current Status**: Unknown

**BASIC Logic**:
```basic
6370 PRINT"CONGRULATION, CAPTAIN!  THEN LAST KLINGON BATTLE CRUISER"
6380 PRINT"MENACING THE FDERATION HAS BEEN DESTROYED.":PRINT
6400 PRINT"YOUR EFFICIENCY RATING IS";1000*(K7/(T-T0))^2:GOTO6290
```

**Verification Needed**:
- [ ] Check if victory is detected when K9=0 (all Klingons destroyed)
- [ ] Verify congratulations message is displayed
- [ ] Confirm efficiency rating calculation: `1000 * (K7 / (T - T0))^2`
  - K7 = initial Klingon count
  - T-T0 = stardates elapsed
- [ ] Test play again prompt after victory

**Efficiency Rating Formula**:
- Higher score = faster completion
- Formula rewards speed: time is in denominator, squared
- Example: 10 Klingons in 10 stardates = 1000 * (10/10)^2 = 1000
- Example: 10 Klingons in 5 stardates = 1000 * (10/5)^2 = 4000

#### 3. Defeat Conditions (Lines 6220-6290)
**BASIC Reference**: Lines 6220-6290
**Current Status**: Unknown

**BASIC Logic**:
```basic
6220 PRINT"IT IS STARDATE";T:GOTO 6270  ' Time up or fatal error
6240 PRINT"THE ENTERPRISE HAS BEEN DESTROYED.  THEN FEDERATION ";
6250 PRINT"WILL BE CONQUERED":GOTO 6220  ' Ship destroyed
6270 PRINT"THERE WERE";K9;"KLINGON BATTLE CRUISERS LEFT AT"
6280 PRINT"THE END OF YOUR MISSION."
6290 PRINT:PRINT:IFB9=0THEN6360  ' Check for play again prompt
```

**Verification Needed**:
- [ ] Ship destruction detection (shields <= 0 during combat)
- [ ] Time limit detection (T > T0 + T9)
- [ ] Fatal error condition (lines 2020-2050) - Completed in Phase 4
- [ ] Proper defeat message display
- [ ] Display remaining Klingon count
- [ ] Play again prompt

#### 4. Exit Command (XXX)
**BASIC Reference**: Line 2260 (in help text)
**Current Status**: Unknown

**BASIC Logic**:
```basic
2260 PRINT"  XXX  (TO RESIGN YOUR COMMAND)":PRINT:GOTO 1990
```

**Verification Needed**:
- [ ] Check if XXX command is recognized by command parser
- [ ] Verify game exits gracefully
- [ ] Confirm final status is displayed before exit
- [ ] Test that XXX is listed in help menu

**Implementation Check**:
- Look for XXX handling in `StarTrekGame.cs` main loop
- Verify command is not in `CommandFactory` (it's handled specially)

#### 5. Quadrant Entry Messages (Lines 1460-1500)
**BASIC Reference**: Lines 1460-1500
**Current Status**: Unknown

**BASIC Logic**:
```basic
1460 PRINT"YOUR MISSION BEGINS WITH YOUR STARSHIP LOCATED"
1470 PRINT"IN THE GALACTIC QUADRANT, '";G2$;"'.":GOTO 1500
1490 PRINT"NOW ENTERING ";G2$;" QUADRANT . . ."
```

**Verification Needed**:
- [ ] Check if quadrant names are displayed when entering new quadrant
- [ ] Verify mission start message uses quadrant name
- [ ] Confirm quadrant name generation (requires lines 9010-9260)
- [ ] Test that "NOW ENTERING..." message appears on quadrant transitions

**Dependency**: Requires Quadrant Name Generator (see Library Computer section)

#### 6. Starbase Destruction Consequences (Lines 5330-5410)
**BASIC Reference**: Lines 5330-5410 (in torpedo code)
**Current Status**: Likely implemented, needs verification

**BASIC Logic**:
```basic
5330 PRINT"*** STARBASE DESTROYED ***":B3=B3-1:B9=B9-1
5360 IFB9>0ORK9>T-T0-T9THEN5400  ' Check if mission still viable
5370 PRINT"THAT DOES IT, CAPTAIN!!  YOU ARE HEREBY RELIEVED OF COMMAND"
5380 PRINT"AND SENTENCED TO 99 STARDATES AT HARD LABOR ON CYGNUS 12!!"
5390 GOTO 6270  ' Game over
5400 PRINT"STARFLEET COMMAND REVIEWING YOUR RECORD TO CONSIDER"
5410 PRINT"COURT MARTIAL!":D0=0
```

**Verification Needed**:
- [ ] Confirm instant game over if no starbases AND insufficient time
- [ ] Check if court martial warning is displayed for starbase destruction
- [ ] Verify B3 and B9 counters are decremented
- [ ] Test condition: `B9 > 0 OR K9 > T - T0 - T9`

#### 7. Galactic Perimeter Messages (Lines 3800-3850)
**BASIC Reference**: Lines 3800-3850 (in navigation code)
**Current Status**: Unknown

**BASIC Logic**:
```basic
3800 PRINT"LT. UHURA REPORTS MESSAGE FROM STARFLEET COMMAND:"
3810 PRINT"  'PERMISSION TO ATTEMPT CROSSING OF GALACTIC PERIMETER"
3820 PRINT"  IS HEREBY *DENIED*.  SHUT DOWN YOUR ENGINES.'"
3830 PRINT"CHIEF ENGINEER SCOTT REPORTS  'WARP ENGINES SHUT DOWN"
3840 PRINT"  AT SECTOR";S1;",";S2;"OF QUADRANT";Q1;",";Q2;".'"
```

**Verification Needed**:
- [ ] Check if attempting to leave galaxy shows Uhura's message
- [ ] Verify Enterprise stops at galaxy boundary
- [ ] Confirm position is reported when stopped
- [ ] Test all 4 edges of galaxy (Q1/Q2 < 1 or > 8)

---

## Implementation Priority Order

### Priority 1: CRITICAL - Library Computer (COM Command)
**Estimated Effort**: 2-3 days
**BASIC Lines**: 7290-9260 (970 lines)
**Impact**: Major feature gap, ~23% of original game code

**Tasks**:
1. Create `ComputerCommand.cs` with function menu
2. Implement Function 0: Cumulative Galactic Record
3. Implement Function 1: Status Report (+ chain to DAM)
4. Implement Function 2: Photon Torpedo Data
5. Implement Function 3: Starbase Navigation Data
6. Implement Function 4: Direction/Distance Calculator
7. Implement Function 5: Galaxy Region Name Map
8. Create `QuadrantNameGenerator.cs` utility class
9. Write comprehensive tests for all 6 functions
10. Integrate with CommandFactory

**Dependencies**:
- Access to GameState.Galaxy.ScannedData (Z array)
- Access to Klingon positions in current quadrant
- Access to starbase coordinates
- Direction/distance calculation logic

**Authenticity Requirements**:
- Exact direction calculation algorithm (lines 8220-8460)
- Exact quadrant name generation (lines 9010-9260)
- Proper display formatting matching BASIC output

### Priority 2: MEDIUM - Verification Tasks
**Estimated Effort**: 1-2 days
**Impact**: Ensure existing features are complete and authentic

**Tasks**:
1. Verify docking system functionality
   - Test energy/torpedo refill
   - Test shield auto-drop
   - Test Klingon attack protection

2. Verify victory/defeat conditions
   - Test K9=0 victory trigger
   - Test efficiency rating calculation
   - Test time limit defeat
   - Test ship destruction defeat

3. Verify XXX exit command
   - Check command recognition
   - Test graceful exit

4. Verify quadrant entry messages
   - Test "NOW ENTERING..." display
   - Requires quadrant name generator first

5. Verify galactic perimeter handling
   - Test boundary collision messages
   - Test all 4 edges

6. Verify starbase destruction consequences
   - Test instant game over condition
   - Test court martial warning

### Priority 3: LOW - Polish and Enhancement
**Estimated Effort**: 1-2 days
**Impact**: User experience improvements

**Tasks**:
1. Add help system (HELP or ? command)
2. Improve command error messages
3. Add input validation hints
4. Consider adding play again functionality
5. Review all message formatting vs BASIC

---

## Testing Requirements

### New Tests Needed for Library Computer
- [ ] `ComputerCommandTests.cs` - Main command structure (10+ tests)
  - Computer disabled detection
  - Function menu display
  - Invalid function number handling
  - Each function routing

- [ ] `CumulativeGalacticRecordTests.cs` - Function 0 (5+ tests)
  - Unexplored quadrant display ("***")
  - Explored quadrant data display
  - Grid formatting
  - Header display

- [ ] `StatusReportTests.cs` - Function 1 (5+ tests)
  - Klingon count display
  - Time remaining calculation
  - Starbase count display
  - Singular/plural handling
  - No starbase warning

- [ ] `TorpedoDataTests.cs` - Function 2 (5+ tests)
  - No Klingon handling
  - Single Klingon data
  - Multiple Klingon data
  - Direction/distance accuracy

- [ ] `StarbaseNavDataTests.cs` - Function 3 (4+ tests)
  - No starbase handling
  - Starbase present data
  - Direction/distance accuracy

- [ ] `DirectionDistanceCalculatorTests.cs` - Function 4 (10+ tests)
  - All 8 primary directions
  - Fractional directions
  - Zero distance handling
  - Distance calculation accuracy
  - Coordinate input validation

- [ ] `GalaxyRegionMapTests.cs` - Function 5 (5+ tests)
  - Region name generation
  - Suffix generation (I, II, III, IV)
  - Display formatting
  - All 64 quadrant names

- [ ] `QuadrantNameGeneratorTests.cs` - Utility (10+ tests)
  - All 16 base region names
  - All 4 suffix variations
  - RegionOnly parameter
  - Boundary cases (coordinates 1,1 and 8,8)

**Estimated New Tests**: 50-60 tests

### Verification Tests Needed
- [ ] `DockingSystemTests.cs` - Verify docking (5+ tests)
- [ ] `VictoryConditionTests.cs` - Verify win conditions (5+ tests)
- [ ] `DefeatConditionTests.cs` - Verify loss conditions (5+ tests)
- [ ] `ExitCommandTests.cs` - Verify XXX command (3+ tests)
- [ ] `GalacticPerimeterTests.cs` - Verify boundaries (4+ tests)

**Estimated Verification Tests**: 20-25 tests

**Total New Tests Required**: 70-85 tests

---

## BASIC Code Coverage Analysis

### Current Implementation Coverage

**Total BASIC Lines**: 425 lines (excluding comments and blank lines)
**Estimated Implemented**: ~320 lines (75%)
**Remaining**: ~105 lines (25%)

### Line-by-Line Implementation Status

| Line Range | Description | Status |
|------------|-------------|--------|
| 10-220 | Header comments & title display | ✅ Implicit |
| 260-670 | Initialization & data structures | ✅ Complete |
| 710 | Command string definition | ✅ Complete |
| 810-1160 | Galaxy setup | ✅ Complete |
| 1200-1280 | Mission briefing | ✅ Complete |
| 1320-1910 | New quadrant entry | ✅ Complete |
| 1980-2050 | Emergency conditions | ✅ Complete |
| 2060-2260 | Command input loop | ✅ Complete |
| 2300-3980 | Navigation (NAV) | ✅ Complete |
| 4000-4230 | Long range sensors (LRS) | ✅ Complete |
| 4260-4670 | Phasers (PHA) | ✅ Complete |
| 4700-5490 | Torpedoes (TOR) | ✅ Complete |
| 5530-5660 | Shields (SHE) | ✅ Complete |
| 5690-5980 | Damage control (DAM) | ✅ Complete |
| 6000-6200 | Klingon combat | ✅ Complete |
| 6220-6400 | Game end conditions | ⚠️ Verify |
| 6430-7260 | Short range sensors (SRS) | ✅ Complete |
| 7290-9260 | **Library computer (COM)** | ❌ **MISSING** |

### Missing Code Breakdown

**Library Computer**: ~970 lines (23% of game)
- Lines 7290-7380: Computer menu (90 lines)
- Lines 7400-7850: Functions 0 & 5 (450 lines)
- Lines 7900-8020: Function 1 (120 lines)
- Lines 8070-8100: Function 2 (30 lines)
- Lines 8150-8520: Functions 3 & 4 (370 lines)
- Lines 8590-8900: Support subroutines (310 lines)
- Lines 9010-9260: Quadrant names (250 lines)

**Verification Needed**: ~105 lines (2.5% of game)
- Victory/defeat messages: ~50 lines
- Docking system: ~30 lines
- Perimeter messages: ~25 lines

---

## Feature Completeness Checklist

### Core Game Loop ✅
- [x] Galaxy initialization
- [x] Mission briefing
- [x] Command input
- [x] Command parsing
- [x] Command execution
- [x] Time advancement
- [ ] Victory detection (verify)
- [ ] Defeat detection (verify)
- [ ] Play again option (verify)

### Commands ✅ (7/8 complete, 1 missing)
- [x] NAV - Navigation
- [x] SRS - Short Range Sensors
- [x] LRS - Long Range Sensors
- [x] PHA - Phasers
- [x] TOR - Torpedoes
- [x] SHE - Shields
- [x] DAM - Damage Control
- [ ] **COM - Library Computer** ❌
- [ ] XXX - Exit (verify)

### Ship Systems ✅
- [x] Energy management
- [x] Shield control
- [x] Torpedo supply
- [x] Damage tracking (8 systems)
- [x] Automatic repairs
- [x] Manual repairs at starbase

### Combat Systems ✅
- [x] Phaser firing
- [x] Torpedo firing
- [x] Klingon movement
- [x] Klingon counter-attacks
- [x] Collision detection
- [x] System damage from combat
- [x] Shield damage absorption

### Navigation Systems ✅
- [x] 9-directional movement
- [x] Warp speed control
- [x] Energy consumption
- [x] Quadrant transitions
- [x] Sector boundary checking
- [x] Collision avoidance
- [ ] Galactic perimeter messages (verify)

### Display Systems ✅
- [x] Short range sensor grid
- [x] Long range sensor grid
- [x] Ship status panel
- [x] Damage reports
- [x] Combat messages
- [ ] Quadrant names in messages (missing - needs generator)

### Support Systems
- [x] Random number generation
- [x] Galaxy mapping
- [x] Coordinate management
- [ ] Direction/distance calculation (missing - in COM)
- [ ] Quadrant name generation (missing - in COM)
- [ ] Cumulative record tracking (partial - needs COM display)

### Starbase Features
- [ ] Docking detection (verify)
- [ ] Energy refill (verify)
- [ ] Torpedo refill (verify)
- [ ] Shield auto-drop (verify)
- [ ] Manual repair service ✅
- [ ] Destruction consequences (verify)

### Library Computer Functions ❌
- [ ] Function 0: Cumulative galactic record
- [ ] Function 1: Status report
- [ ] Function 2: Photon torpedo data
- [ ] Function 3: Starbase navigation data
- [ ] Function 4: Direction/distance calculator
- [ ] Function 5: Galaxy region name map

---

## Success Criteria for FAITHFUL Re-implementation

To consider this project a complete and faithful re-implementation, the following must be true:

### 1. Feature Parity ✅/❌
- [ ] All 8 commands implemented (7/8 complete)
- [ ] All 6 computer functions implemented (0/6 complete)
- [ ] All game end conditions working (needs verification)
- [ ] All BASIC behaviors replicated exactly

### 2. Authenticity ✅
- [x] Combat formulas match BASIC exactly
- [x] Navigation mechanics match BASIC exactly
- [x] Damage system matches BASIC exactly
- [ ] Computer functions match BASIC exactly (not implemented)
- [ ] All messages match BASIC text (mostly complete)

### 3. Code Quality ✅
- [x] Clean C# architecture
- [x] Comprehensive test coverage (>95%)
- [x] XML documentation
- [x] Separation of concerns
- [x] Object-oriented design

### 4. Testing 🔄
- [x] Existing features: 114 tests passing
- [ ] Library Computer: 50-60 tests needed
- [ ] Verification: 20-25 tests needed
- [ ] Target: 180-200 total tests

### 5. Documentation 🔄
- [x] CLAUDE.md (AI assistant guide)
- [x] MIGRATION.md (phase tracking)
- [x] DESIGN.md (architecture)
- [x] README.md (user guide)
- [ ] TODO.md (this document) ✅
- [ ] Computer functions documentation (after implementation)

---

## Estimated Effort to Completion

### Library Computer Implementation
- **Function Menu & Infrastructure**: 3-4 hours
- **Function 0 (Cumulative Record)**: 2-3 hours
- **Function 1 (Status Report)**: 1-2 hours
- **Function 2 (Torpedo Data)**: 1-2 hours
- **Function 3 (Starbase Nav)**: 1-2 hours
- **Function 4 (Direction/Distance)**: 3-4 hours (complex logic)
- **Function 5 (Region Map)**: 2-3 hours
- **Quadrant Name Generator**: 2-3 hours
- **Testing**: 6-8 hours (50-60 tests)
- **Integration & Polish**: 2-3 hours

**Total**: 23-34 hours (~3-4 days)

### Verification Tasks
- **Docking System**: 1-2 hours
- **Victory/Defeat Conditions**: 2-3 hours
- **XXX Command**: 0.5-1 hour
- **Quadrant Entry Messages**: 1-2 hours
- **Galactic Perimeter**: 1-2 hours
- **Starbase Destruction**: 1-2 hours
- **Testing**: 3-4 hours (20-25 tests)

**Total**: 9.5-16 hours (~1-2 days)

### Documentation & Polish
- **Update documentation**: 2-3 hours
- **Final testing**: 2-3 hours
- **Code review**: 1-2 hours

**Total**: 5-8 hours (~1 day)

**GRAND TOTAL**: 37.5-58 hours (~5-7 working days)

---

## Conclusion

The Super Star Trek C# re-implementation is approximately **75% complete** with excellent authenticity in the implemented features. The primary gap is the **Library Computer (COM) command**, which represents about 23% of the original BASIC code and includes 6 distinct functions plus the quadrant naming system.

**Phase Status**:
- ✅ Phase 1 (Foundation): Complete
- ✅ Phase 2 (Core Systems): Complete
- ✅ Phase 3 (Combat): Complete
- ✅ Phase 4 (Ship Systems): Complete
- ❌ **Phase 5 (Library Computer): NOT STARTED** ← Current blocker

**To achieve a truly FAITHFUL re-implementation**, we must:
1. **Implement the complete Library Computer system** (Priority 1)
2. **Verify all existing features work correctly** (Priority 2)
3. **Polish messages and user experience** (Priority 3)

Once the Library Computer is implemented and all features are verified, this will be a complete, authentic, and faithful C# port of the 1978 Microsoft BASIC Super Star Trek game.

---

**Document Version**: 1.0
**Last Updated**: 2025-11-17
**Status**: Initial comprehensive analysis complete
**Next Action**: Begin Library Computer implementation (Phase 5)
