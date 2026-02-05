# Task: Exercise Database Schema & Seed Data

**Task ID:** 007  
**GitHub Issue:** [#13](https://github.com/mollie-ward/fitness-app/issues/13)  
**Feature:** AI Training Plan Generation (FRD-002)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Large  

---

## Description

Design and implement the exercise database schema with comprehensive tagging for disciplines, muscle groups, movement patterns, equipment needs, and difficulty levels. Create seed data with foundational exercises for HYROX, running, and strength training.

---

## Dependencies

- **Task 001:** Backend scaffolding must be complete

---

## Technical Requirements

### Exercise Entity Model
- Create `Exercise` entity with properties:
  - ExerciseId (unique identifier)
  - Name (e.g., "Back Squat", "Tempo Run", "Ski Erg Intervals")
  - Description (detailed explanation of the exercise)
  - Instructions (how to perform it)
  - Difficulty level (Beginner, Intermediate, Advanced)
  - ApproximateDuration (in minutes)
  - IntensityLevel (Low, Moderate, High, Maximum)
  
### Exercise Categorization
- Create `Discipline` enum or entity:
  - HYROX, Running, Strength, Hybrid
- Create `MovementPattern` entity/enum:
  - Push, Pull, Squat, Hinge, Carry, Core, Cardio
- Create `MuscleGroup` entity:
  - Upper Body, Lower Body, Core, Full Body
  - Specific muscles (quads, hamstrings, chest, back, shoulders, etc.)
- Create `EquipmentRequired` entity:
  - Bodyweight, Dumbbells, Barbell, Kettlebell, Ski Erg, Rowing Machine, Sled, etc.
- Create many-to-many relationships for exercises to categories

### Injury Contraindications
- Create `Contraindication` entity:
  - InjuryType (shoulder, knee, back, etc.)
  - Movement restriction (overhead, impact, heavy load, etc.)
- Link exercises to contraindications (what injuries prevent this exercise)
- Support substitute exercise recommendations

### Progression & Scaling
- Create `ExerciseProgression` entity:
  - Easier variation (regression)
  - Harder variation (progression)
  - Alternative with similar stimulus
- Link exercises in progression chains

### Workout Session Types
- Create `SessionType` entity or enum:
  - For Running: Easy Run, Intervals, Tempo, Long Run, Recovery
  - For Strength: Full Body, Upper/Lower, Push/Pull/Legs
  - For HYROX: Race Simulation, Station Practice, Transition Drills, Hybrid Conditioning

### Database Schema
- Design normalized schema with appropriate relationships
- Create indexes for common queries (by discipline, difficulty, equipment)
- Set up foreign keys and constraints
- Create Entity Framework Core migrations

### Seed Data Creation
- HYROX exercises (minimum 30):
  - Ski Erg workouts
  - Sled push/pull variations
  - Burpee broad jump drills
  - Rowing intervals
  - Farmers carry variations
  - Sandbag lunges
  - Wall balls
  - Transition practice
  
- Running workouts (minimum 25):
  - Easy runs (various durations)
  - Interval sessions (400m, 800m, 1600m, etc.)
  - Tempo runs
  - Long runs
  - Fartlek training
  - Hill repeats
  - Recovery runs

- Strength exercises (minimum 50):
  - Compound lifts (squat, deadlift, bench, overhead press, rows)
  - Accessory movements (lunges, RDLs, pull-ups, dips)
  - Core work (planks, dead bugs, pallof press)
  - Unilateral exercises
  - Olympic lift variations

- Tag all exercises with:
  - Discipline
  - Movement patterns
  - Muscle groups
  - Equipment
  - Difficulty level
  - Contraindications (where applicable)

---

## Acceptance Criteria

- ✅ Exercise database schema is created with all required entities
- ✅ Relationships between exercises and categories are properly defined
- ✅ Database migrations create tables successfully
- ✅ Seed data includes minimum required exercises for each discipline
- ✅ All exercises are tagged with discipline, difficulty, and muscle groups
- ✅ Equipment requirements are specified for each exercise
- ✅ Contraindications are defined for injury-prone exercises
- ✅ Exercise progressions link easier and harder variations
- ✅ Session types are defined for all three disciplines
- ✅ Queries can filter exercises by multiple criteria efficiently

---

## Testing Requirements

### Unit Tests
- Test exercise entity creation and validation
- Test relationship mappings (exercise to categories)
- Test contraindication logic
- **Minimum coverage:** ≥85% for domain logic

### Integration Tests
- Test database migrations apply successfully
- Test seed data inserts all exercises without errors
- Test queries filter exercises by discipline
- Test queries filter by difficulty level
- Test queries exclude contraindicated exercises for given injury
- Test progression chains are traversable
- **Minimum coverage:** ≥85% for database operations

### Data Quality Tests
- Verify all exercises have required fields populated
- Verify no duplicate exercise names within same discipline
- Verify all exercises are tagged with at least one category
- Verify contraindication relationships are bidirectional

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Database schema is normalized and optimized
- Seed data script runs successfully
- Exercise library contains minimum required exercises
- All exercises are properly categorized and tagged
- Documentation created describing exercise structure
- Query performance is acceptable for filtering operations
