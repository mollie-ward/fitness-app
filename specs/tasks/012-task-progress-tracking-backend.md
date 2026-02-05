# Task: Progress Tracking Backend Service

**Task ID:** 012  
**GitHub Issue:** [#23](https://github.com/mollie-ward/fitness-app/issues/23)  
**Feature:** Progress Tracking & Completion (FRD-005)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Medium  

---

## Description

Implement backend service and API endpoints for tracking workout completions, calculating statistics, managing streaks, and providing progress metrics to users.

---

## Dependencies

- **Task 008:** Training plan data model must be complete
- **Task 010:** Training plan API endpoints must be complete

---

## Technical Requirements

### Progress Tracking Service

#### Completion Tracking
- Create `IProgressTrackingService` interface
- Implement methods:
  - MarkWorkoutCompleteAsync(workoutId, userId, completionTime)
  - UndoWorkoutCompletionAsync(workoutId, userId)
  - GetCompletionStatsAsync(userId, dateRange)
  - GetStreakInfoAsync(userId)

#### Statistics Calculation
- Calculate completion metrics:
  - Workouts completed this week
  - Workouts completed this month
  - Overall plan completion percentage
  - Total training days (all-time)
  - Average weekly completion rate
  
#### Streak Tracking
- Track consecutive training days
- Track consecutive training weeks (minimum threshold)
- Calculate current active streak
- Record personal best streak
- Identify streak milestones (7 days, 14 days, 30 days, etc.)

#### Historical Data
- Store completion history for analytics
- Track completion patterns over time
- Identify consistency trends
- Calculate adherence rate by discipline

### Data Model Extensions
- Add `CompletionHistory` entity:
  - UserId
  - WorkoutId
  - CompletedAt (timestamp)
  - Duration (actual time taken)
  - Notes (optional user feedback)
  
- Add `UserStreak` entity:
  - UserId
  - CurrentStreak (days)
  - LongestStreak (personal best)
  - LastWorkoutDate
  - StreakType (daily vs weekly)

### API Endpoints

#### PUT /api/progress/workouts/{workoutId}/complete
- Mark workout complete
- Request: Completion timestamp, optional notes
- Response: Updated workout with completion status
- Updates streak information

#### PUT /api/progress/workouts/{workoutId}/undo
- Undo accidental completion
- Response: Updated workout status
- Recalculates streak if necessary

#### GET /api/progress/stats/weekly
- Get current week's completion statistics
- Response: Completed count, total scheduled, percentage

#### GET /api/progress/stats/monthly
- Get current month's statistics
- Response: Monthly completion data

#### GET /api/progress/stats/overall
- Get overall plan and all-time statistics
- Response: Comprehensive stats DTO

#### GET /api/progress/streak
- Get current streak information
- Response: Current streak, longest streak, milestone progress

#### GET /api/progress/history
- Get historical completion data
- Query params: startDate, endDate
- Response: Completion history for calendar heatmap

### Validation Logic
- Prevent completing future workouts
- Prevent marking same workout complete multiple times
- Validate user owns the workout being completed
- Ensure completion timestamp is reasonable

### Side Effects & Triggers
- Update workout completion status in database
- Recalculate plan progress percentage
- Update streak counters
- Trigger notifications for milestones
- Feed data to adaptive adjustment service

---

## Acceptance Criteria

- ✅ Marking workout complete updates database status
- ✅ Completion statistics are calculated accurately
- ✅ Streak tracking counts consecutive training days
- ✅ Streak tracking counts consecutive training weeks
- ✅ Personal best streak is recorded and maintained
- ✅ Completion can be undone within reasonable timeframe
- ✅ Historical completion data is retrievable
- ✅ Statistics API returns accurate metrics
- ✅ Cannot complete workouts user doesn't own
- ✅ Cannot complete future workouts

---

## Testing Requirements

### Unit Tests
- Test completion status update logic
- Test statistics calculation algorithms
- Test streak calculation for various scenarios
- Test undo completion logic
- Test validation rules
- **Minimum coverage:** ≥85% for service logic

### Integration Tests
- Test marking workout complete persists to database
- Test completion updates workout status correctly
- Test streak increments on consecutive completions
- Test streak breaks on missed day
- Test statistics API returns correct data
- Test historical data query for date range
- Test concurrent completion updates
- **Minimum coverage:** ≥85% for API and database operations

### Edge Case Tests
- Test completing last workout of week triggers weekly stats
- Test streak calculation with gaps (missed days)
- Test undo completion after several days
- Test multiple workouts on same day (valid scenario)

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- API endpoints are documented in OpenAPI spec
- Service follows established patterns
- Statistics are calculated efficiently
- Streak logic is accurate and tested
- Progress data feeds into other services (adaptive adjustment)
