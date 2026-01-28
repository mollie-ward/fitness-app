# Task: User Profile Data Model & Repository

**Task ID:** 004  
**Feature:** User Onboarding & Fitness Profiling (FRD-001)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Medium  

---

## Description

Implement the backend data model, database schema, and repository layer for user profiles. This includes storing fitness levels, goals, schedule availability, injuries, and training background information collected during onboarding.

---

## Dependencies

- **Task 001:** Backend scaffolding must be complete

---

## Technical Requirements

### Domain Entities
- Create `UserProfile` entity with properties:
  - UserId (unique identifier)
  - Personal information (name, email - from auth system)
  - CreatedAt, UpdatedAt timestamps
- Create `FitnessLevel` value object or enum:
  - Separate levels for HYROX, Running, Strength
  - Values: Beginner, Intermediate, Advanced
- Create `TrainingGoal` entity:
  - Goal type (HYROX race, running distance, strength milestone, general fitness)
  - Target date (nullable for open-ended goals)
  - Goal description
  - Priority/status
- Create `ScheduleAvailability` value object:
  - Days of week (Monday through Sunday)
  - Minimum/maximum weekly training frequency
- Create `InjuryLimitation` entity:
  - Body part affected
  - Type (acute vs chronic)
  - Reported date
  - Status (active, improving, resolved)
  - Movement restrictions
- Create `TrainingBackground` entity:
  - Previous structured training experience
  - Equipment familiarity
  - Training history details

### Database Schema
- Design and create database tables for:
  - UserProfiles (main profile table)
  - FitnessLevels (embedded or separate table)
  - TrainingGoals (one-to-many relationship)
  - ScheduleAvailability (embedded or JSON column)
  - InjuryLimitations (one-to-many relationship)
  - TrainingBackground (one-to-one relationship)
- Create appropriate indexes for query performance
- Set up foreign key constraints and cascade rules
- Create Entity Framework Core migrations

### Repository Pattern
- Create `IUserProfileRepository` interface with methods:
  - GetByIdAsync(userId)
  - CreateAsync(profile)
  - UpdateAsync(profile)
  - DeleteAsync(userId)
  - GetProfileWithGoalsAsync(userId)
  - GetProfileWithInjuriesAsync(userId)
- Implement repository using Entity Framework Core
- Include proper error handling and validation
- Implement unit of work pattern if needed

### Data Validation
- Validate fitness level selections are valid enums
- Ensure at least one training goal is specified
- Validate training availability (at least 1 day selected)
- Validate goal dates are in the future
- Ensure data integrity constraints are enforced

---

## Acceptance Criteria

- ✅ User profile entity model is created with all required properties
- ✅ Database migrations create tables successfully
- ✅ Repository can create, read, update, and delete user profiles
- ✅ Fitness levels for all three disciplines are stored correctly
- ✅ Multiple training goals can be associated with one user
- ✅ Schedule availability is persisted and retrieved correctly
- ✅ Injuries can be added, updated, and marked as resolved
- ✅ Training background information is stored and retrieved
- ✅ Database constraints prevent invalid data (e.g., duplicate users)
- ✅ Repository methods handle not-found scenarios gracefully

---

## Testing Requirements

### Unit Tests
- Test entity creation with valid data
- Test validation logic for invalid data
- Test value object behavior (equality, immutability if applicable)
- Test repository interface methods with mocked DbContext
- **Minimum coverage:** ≥85% for domain logic and validation

### Integration Tests
- Test database migrations apply successfully
- Test repository creates user profile in database
- Test repository retrieves profile with related entities (goals, injuries)
- Test repository updates profile and related entities
- Test repository deletes profile and cascades correctly
- Test concurrent updates are handled correctly
- **Minimum coverage:** ≥85% for repository operations

### Data Validation Tests
- Test invalid fitness level throws exception
- Test missing required fields fails validation
- Test goal date in the past is rejected
- Test empty schedule availability is rejected

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Database migrations are versioned and tested
- Code follows repository and domain-driven design patterns
- Entity relationships are properly configured in EF Core
- API documentation is prepared (no API endpoints yet, but DTOs ready)
- No N+1 query issues in repository methods
