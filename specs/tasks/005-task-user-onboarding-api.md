# Task: User Onboarding API Endpoints

**Task ID:** 005  
**GitHub Issue:** [#9](https://github.com/mollie-ward/fitness-app/issues/9)  
**Feature:** User Onboarding & Fitness Profiling (FRD-001)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Medium  

---

## Description

Implement REST API endpoints for user onboarding flow, allowing clients to create and update user profiles with fitness information, goals, schedule, and limitations.

---

## Dependencies

- **Task 004:** User profile data model and repository must be complete

---

## Technical Requirements

### API Endpoints
Create the following RESTful endpoints:

#### POST /api/users/profile
- Create new user profile during onboarding
- Request body: Complete profile DTO with fitness levels, goals, schedule, injuries
- Response: Created profile with unique ID (201 Created)
- Validates all required fields are present

#### GET /api/users/profile
- Get authenticated user's complete profile
- Include fitness levels, goals, schedule, injuries, training background
- Response: Profile DTO with all related data (200 OK)
- Returns 404 if profile doesn't exist

#### PUT /api/users/profile
- Update existing user profile
- Request body: Updated profile DTO
- Response: Updated profile (200 OK)
- Validates changes and maintains data integrity

#### GET /api/users/profile/goals
- Get all training goals for authenticated user
- Response: List of goals with details (200 OK)

#### POST /api/users/profile/goals
- Add new training goal to profile
- Request body: Goal DTO
- Response: Created goal (201 Created)

#### PUT /api/users/profile/goals/{goalId}
- Update specific goal
- Request body: Updated goal DTO
- Response: Updated goal (200 OK)

#### DELETE /api/users/profile/goals/{goalId}
- Remove goal from profile
- Response: 204 No Content

#### POST /api/users/profile/injuries
- Add injury or limitation to profile
- Request body: Injury DTO
- Response: Created injury record (201 Created)

#### PUT /api/users/profile/injuries/{injuryId}
- Update injury status (e.g., mark as healing, resolved)
- Request body: Updated injury DTO
- Response: Updated injury (200 OK)

### DTOs (Data Transfer Objects)
- Create `UserProfileDto` with all profile fields
- Create `TrainingGoalDto` for goal data
- Create `InjuryLimitationDto` for injury data
- Create `ScheduleAvailabilityDto` for schedule data
- Use AutoMapper or manual mapping from entities to DTOs

### Validation
- Implement FluentValidation or data annotations for DTOs
- Validate fitness level values
- Validate goal dates are in the future
- Ensure at least one training day is selected
- Validate injury severity and type values

### Error Handling
- Return 400 Bad Request for validation errors
- Return 404 Not Found when profile doesn't exist
- Return 401 Unauthorized for unauthenticated requests
- Return 409 Conflict for duplicate profile creation
- Include detailed error messages in response

### Authentication
- Secure all endpoints with JWT authentication
- Extract user ID from JWT claims
- Ensure users can only access/modify their own profile

### OpenAPI Documentation
- Add XML documentation comments to controllers
- Annotate endpoints with proper HTTP status codes
- Define request/response schemas in Swagger
- Provide example requests and responses

---

## Acceptance Criteria

- ✅ POST /api/users/profile creates user profile successfully
- ✅ GET /api/users/profile returns complete profile for authenticated user
- ✅ PUT /api/users/profile updates profile with new data
- ✅ Goals can be created, retrieved, updated, and deleted via API
- ✅ Injuries can be added and updated via API
- ✅ All endpoints require authentication (401 for anonymous requests)
- ✅ Validation errors return 400 with clear error messages
- ✅ OpenAPI spec accurately documents all endpoints
- ✅ DTOs are properly mapped to/from entities
- ✅ Cannot create duplicate profiles for same user

---

## Testing Requirements

### Unit Tests
- Test controller actions with mocked services
- Test DTO validation rules
- Test mapping between entities and DTOs
- Test authentication attribute is applied
- **Minimum coverage:** ≥85% for controller logic

### Integration Tests
- Test POST creates profile in database
- Test GET retrieves profile correctly
- Test PUT updates profile fields
- Test goal CRUD operations end-to-end
- Test injury CRUD operations end-to-end
- Test validation errors return 400
- Test unauthorized requests return 401
- Test not-found scenarios return 404
- **Minimum coverage:** ≥85% for API endpoints

### Contract Tests
- Validate OpenAPI specification matches actual endpoints
- Test request/response schemas are accurate
- Verify all required fields are documented

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- OpenAPI documentation is complete and accurate
- Error responses follow consistent format
- Code follows RESTful API best practices
- Authentication is enforced on all endpoints
- DTOs prevent over-posting vulnerabilities
- API versioning strategy is implemented (if applicable)
