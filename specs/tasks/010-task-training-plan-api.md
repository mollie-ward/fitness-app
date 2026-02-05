# Task: Training Plan API Endpoints

**Task ID:** 010  
**GitHub Issue:** [#19](https://github.com/mollie-ward/fitness-app/issues/19)  
**Feature:** AI Training Plan Generation (FRD-002)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Medium  

---

## Description

Implement REST API endpoints for training plan operations, allowing clients to generate plans, retrieve current plans, get workouts, and mark workouts complete.

---

## Dependencies

- **Task 008:** Training plan data model must be complete
- **Task 009:** Training plan generation service must be complete

---

## Technical Requirements

### API Endpoints

#### POST /api/training/plans/generate
- Generate new training plan for authenticated user
- Triggers plan generation service
- Request body: Optional override parameters
- Response: Complete plan summary (201 Created)
- Returns 409 if user already has active plan

#### GET /api/training/plans/current
- Get user's currently active training plan
- Include plan metadata, current week, total progress
- Response: Plan summary DTO (200 OK)
- Returns 404 if no active plan exists

#### GET /api/training/plans/{planId}
- Get specific plan by ID
- Include all weeks and workouts
- Response: Detailed plan DTO (200 OK)
- Verify user owns this plan

#### GET /api/training/plans/{planId}/weeks/{weekNumber}
- Get specific week from plan
- Include all workouts for that week
- Response: Week DTO with workouts (200 OK)

#### GET /api/training/workouts/today
- Get today's scheduled workout for user
- Response: Workout DTO with exercises (200 OK)
- Returns 404 if no workout scheduled today

#### GET /api/training/workouts/upcoming
- Get upcoming workouts (next 7 days)
- Response: List of workout DTOs (200 OK)

#### GET /api/training/workouts/{workoutId}
- Get detailed workout information
- Include all exercises with sets, reps, instructions
- Response: Detailed workout DTO (200 OK)

#### PUT /api/training/workouts/{workoutId}/complete
- Mark workout as completed
- Request body: Completion timestamp
- Response: Updated workout DTO (200 OK)
- Triggers progress tracking updates

#### PUT /api/training/workouts/{workoutId}/skip
- Mark workout as skipped
- Request body: Optional reason
- Response: Updated workout DTO (200 OK)

#### GET /api/training/workouts/calendar
- Get workouts for date range (calendar view)
- Query params: startDate, endDate
- Response: List of workouts with summary info (200 OK)

#### DELETE /api/training/plans/{planId}
- Archive or delete plan
- Response: 204 No Content
- Mark as inactive rather than hard delete

---

### DTOs (Data Transfer Objects)
- `TrainingPlanSummaryDto`: Plan overview with metadata
- `TrainingPlanDetailDto`: Complete plan with all weeks
- `TrainingWeekDto`: Week with workouts
- `WorkoutSummaryDto`: Basic workout info for calendar
- `WorkoutDetailDto`: Complete workout with exercises
- `WorkoutExerciseDto`: Exercise within workout with parameters

### Response Formatting
- Consistent response structure across endpoints
- Include navigation links (HATEOAS) where appropriate
- Return appropriate HTTP status codes
- Include pagination for large result sets

### Caching Strategy
- Cache plan data with appropriate TTL
- Invalidate cache when plan is modified
- Use ETag for conditional requests

### Error Handling
- Return 404 when plan or workout not found
- Return 409 when attempting duplicate plan generation
- Return 403 when user tries to access another user's plan
- Return 400 for invalid date ranges or parameters

### Authentication & Authorization
- Secure all endpoints with JWT authentication
- Verify user owns the plan/workout being accessed
- Prevent access to other users' data

### OpenAPI Documentation
- Document all endpoints with examples
- Include response schemas
- Document error responses
- Provide sample requests

---

## Acceptance Criteria

- ✅ POST /api/training/plans/generate creates new plan
- ✅ GET /api/training/plans/current returns active plan
- ✅ GET /api/training/workouts/today returns today's workout
- ✅ GET /api/training/workouts/upcoming returns next 7 days
- ✅ PUT /api/training/workouts/{id}/complete marks workout complete
- ✅ GET /api/training/workouts/calendar returns workouts for date range
- ✅ All endpoints require authentication
- ✅ Users cannot access other users' plans or workouts
- ✅ OpenAPI spec documents all endpoints accurately
- ✅ Response DTOs are properly structured and typed

---

## Testing Requirements

### Unit Tests
- Test controller actions with mocked services
- Test DTO mapping from entities
- Test authorization logic
- **Minimum coverage:** ≥85% for controller logic

### Integration Tests
- Test plan generation creates plan in database
- Test retrieving current plan returns correct data
- Test today's workout returns scheduled workout
- Test marking workout complete updates status
- Test calendar endpoint returns correct date range
- Test unauthorized access returns 403
- Test not-found scenarios return 404
- **Minimum coverage:** ≥85% for API endpoints

### Contract Tests
- Validate OpenAPI spec matches implementation
- Test request/response schemas are accurate
- Verify all status codes are documented

### Performance Tests
- Test plan generation completes within 5 seconds
- Test calendar queries are efficient for large date ranges
- Test concurrent workout completion updates

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- OpenAPI documentation is complete
- Error responses are consistent and helpful
- Code follows RESTful API best practices
- Authorization prevents unauthorized access
- Performance meets requirements (<5s plan generation)
- API is ready for frontend integration
