# Task: Training Plan Data Model & Storage

**Task ID:** 008  
**GitHub Issue:** [#15](https://github.com/mollie-ward/fitness-app/issues/15)  
**Feature:** AI Training Plan Generation (FRD-002)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Large  

---

## Description

Implement the data model and database schema for storing multi-week training plans, including weekly structures, individual workouts, periodization phases, and plan metadata.

---

## Dependencies

- **Task 004:** User profile data model must be complete
- **Task 007:** Exercise database must be complete

---

## Technical Requirements

### Domain Entities

#### TrainingPlan Entity
- PlanId (unique identifier)
- UserId (foreign key to user profile)
- PlanName (e.g., "12-Week HYROX Preparation")
- StartDate, EndDate
- TotalWeeks
- TrainingDaysPerWeek
- PrimaryGoal (reference to user's goal)
- Status (Active, Completed, Abandoned, Paused)
- CreatedAt, UpdatedAt
- CurrentWeek (tracking progress)

#### TrainingWeek Entity
- WeekId (unique identifier)
- PlanId (foreign key)
- WeekNumber (1, 2, 3... within plan)
- Phase (Foundation, Build, Intensity, Peak, Taper, Recovery)
- WeeklyVolume (total estimated minutes)
- IntensityLevel (Low, Moderate, High)
- FocusArea (what this week emphasizes)
- StartDate, EndDate

#### Workout Entity
- WorkoutId (unique identifier)
- WeekId (foreign key)
- DayOfWeek (Monday through Sunday)
- ScheduledDate
- Discipline (HYROX, Running, Strength, Hybrid)
- SessionType (Intervals, Tempo, Race Simulation, Full Body, etc.)
- WorkoutName (e.g., "Interval Training - 8x400m")
- Description (detailed workout explanation)
- EstimatedDuration (in minutes)
- IntensityLevel
- IsKeyWorkout (highlight milestone sessions)
- CompletionStatus (NotStarted, InProgress, Completed, Skipped, Missed)
- CompletedAt (nullable datetime)

#### WorkoutExercise Entity (Join table)
- WorkoutExerciseId
- WorkoutId (foreign key)
- ExerciseId (foreign key to Exercise database)
- OrderInWorkout (sequencing)
- Sets, Reps, Duration (workout-specific parameters)
- RestPeriod (seconds between sets)
- IntensityGuidance (e.g., "70% max", "RPE 7", "Easy pace")
- Notes (additional instructions)

#### PlanMetadata Entity
- Metadata about how plan was generated
- Algorithm version
- User profile snapshot at generation time
- Modifications history

### Database Schema Design
- Create tables with proper relationships
- Implement cascade delete rules (deleting plan deletes weeks and workouts)
- Index on UserId, ScheduledDate for efficient queries
- Support for soft deletes (mark as deleted without removing)

### Repository Interfaces
- `ITrainingPlanRepository`:
  - GetActivePlanByUserIdAsync(userId)
  - CreatePlanAsync(plan)
  - UpdatePlanAsync(plan)
  - GetPlanWithWeeksAsync(planId)
  - GetCurrentWeekWorkoutsAsync(userId)
  
- `IWorkoutRepository`:
  - GetWorkoutByIdAsync(workoutId)
  - GetWorkoutsByDateRangeAsync(userId, startDate, endDate)
  - UpdateWorkoutStatusAsync(workoutId, status)
  - GetTodaysWorkoutAsync(userId)
  - GetUpcomingWorkoutsAsync(userId, days)

### Querying & Performance
- Support efficient queries for:
  - Get today's workout
  - Get current week's workouts
  - Get workouts by month (for calendar view)
  - Get plan progress statistics
- Use eager loading to prevent N+1 queries
- Implement pagination for long-term plan history

---

## Acceptance Criteria

- ✅ Training plan entity model is created with all required properties
- ✅ Database schema supports multi-week plans with nested workouts
- ✅ Workouts can be linked to exercises from exercise database
- ✅ Plan phases (Foundation, Build, etc.) are tracked at week level
- ✅ Workout completion status can be updated
- ✅ Repository can retrieve active plan for a user
- ✅ Repository can get workouts by date range efficiently
- ✅ Cascade delete removes related weeks and workouts when plan is deleted
- ✅ Current week and today's workout can be queried efficiently
- ✅ Plan metadata captures generation parameters

---

## Testing Requirements

### Unit Tests
- Test entity creation and validation
- Test entity relationships are properly defined
- Test workout status transitions (NotStarted → Completed)
- **Minimum coverage:** ≥85% for domain logic

### Integration Tests
- Test creating complete plan with weeks and workouts
- Test retrieving plan with all nested entities (weeks, workouts, exercises)
- Test updating workout completion status
- Test querying workouts by date range
- Test cascade delete removes dependent entities
- Test concurrent plan updates are handled correctly
- **Minimum coverage:** ≥85% for repository operations

### Performance Tests
- Test query performance for retrieving month of workouts
- Verify no N+1 query issues when loading plan with weeks
- Test index usage for date-based queries

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Database migrations are versioned and tested
- Repositories follow established patterns
- Entity relationships are properly configured
- Query performance is optimized
- Documentation describes plan structure and relationships
