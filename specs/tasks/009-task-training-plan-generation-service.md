# Task: Training Plan Generation Service

**Task ID:** 009  
**GitHub Issue:** [#17](https://github.com/mollie-ward/fitness-app/issues/17)  
**Feature:** AI Training Plan Generation (FRD-002)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Extra Large  

---

## Description

Implement the core training plan generation service that creates personalized, multi-week training plans based on user profiles. This service applies training science principles, periodization, and progressive overload to generate coherent workout schedules.

---

## Dependencies

- **Task 004:** User profile data model must be complete
- **Task 007:** Exercise database must be complete
- **Task 008:** Training plan data model must be complete

---

## Technical Requirements

### Plan Generation Service Interface
- Create `ITrainingPlanGenerationService` with methods:
  - GeneratePlanAsync(userId)
  - RegeneratePlanAsync(userId, modifications)
  - ValidatePlanParametersAsync(userProfile)

### Plan Generation Algorithm

#### Phase 1: Profile Analysis
- Read user profile (fitness levels, goals, schedule, injuries)
- Determine plan duration based on goal timeline
- Calculate weekly training frequency from schedule availability
- Identify injury constraints that affect exercise selection

#### Phase 2: Periodization Design
- Divide plan into phases based on duration:
  - Short plans (4-8 weeks): Foundation → Build → Peak
  - Medium plans (8-16 weeks): Foundation → Build → Intensity → Peak → Taper
  - Long plans (16+ weeks): Multiple build cycles with recovery weeks
- Assign week ranges to each phase
- Define intensity progression curve across weeks

#### Phase 3: Weekly Structure Creation
- For each week in plan:
  - Determine phase and intensity level
  - Allocate training days to user's available schedule days
  - Balance hard/easy days within week
  - Calculate weekly volume (total training time)
  - Assign focus areas based on goals and phase

#### Phase 4: Workout Assignment
- For each training day:
  - Determine discipline (HYROX, Running, Strength) based on goals
  - Select session type appropriate for phase and day
  - Choose exercises from database matching:
    - User's fitness level
    - Current phase and intensity
    - Equipment availability
    - Injury constraints (exclude contraindicated exercises)
  - Sequence exercises logically (warm-up → main work → cool-down)
  - Set parameters (sets, reps, duration, intensity)

#### Phase 5: Progressive Overload
- Implement progression mechanisms:
  - Volume progression (more sets, reps, distance, time)
  - Intensity progression (heavier weights, faster paces)
  - Complexity progression (more advanced exercises)
- Ensure week-to-week changes are manageable (10-15% increases)
- Include deload weeks every 3-4 weeks for recovery

#### Phase 6: Multi-Discipline Balancing
- For users training multiple disciplines:
  - Allocate training days proportionally to goals
  - Prevent scheduling conflicts (e.g., hard leg day before long run)
  - Balance training stimulus across disciplines
  - Manage total weekly volume to prevent overtraining

#### Phase 7: Validation & Quality Checks
- Verify plan coherence:
  - No illogical exercise sequencing
  - Progressive overload is consistent
  - Recovery is adequate
  - Goal alignment is maintained
- Check for contraindicated exercises
- Validate plan completeness (all days scheduled)

### Training Science Rules Engine
- Implement evidence-based training principles:
  - Progressive overload guidelines
  - Recovery requirements (rest days, deload weeks)
  - Specificity for goal-based training
  - Variation to prevent adaptation
- Codify rules for:
  - Beginner vs. intermediate vs. advanced progression rates
  - HYROX-specific periodization
  - Running periodization (base building, speed work, race prep)
  - Strength training splits and recovery

### Exercise Selection Logic
- Create algorithm for selecting exercises:
  - Match discipline and session type
  - Filter by fitness level
  - Exclude contraindicated exercises
  - Prioritize compound movements
  - Include variety across weeks
  - Substitute exercises when injury constraints exist

### Integration with External Services
- Prepare for future AI/LLM integration for:
  - Exercise variation suggestions
  - Workout descriptions
  - Progression recommendations
- Initially use rule-based logic, design for future enhancement

---

## Acceptance Criteria

- ✅ Service generates valid training plan from user profile
- ✅ Plans respect user's available training days exactly
- ✅ Plans include appropriate phases (Foundation, Build, Peak, etc.)
- ✅ Progressive overload is applied across weeks
- ✅ Exercises are selected based on fitness level and discipline
- ✅ Injury constraints exclude contraindicated exercises
- ✅ Multi-discipline plans balance workload appropriately
- ✅ Time-bound goals align plan duration and intensity
- ✅ Plan validation prevents illogical or unsafe workouts
- ✅ Generated plans are persisted to database

---

## Testing Requirements

### Unit Tests
- Test periodization calculation for various plan durations
- Test exercise selection filters by discipline and level
- Test injury constraint filtering excludes correct exercises
- Test progressive overload calculation
- Test weekly structure generation
- Test multi-discipline allocation logic
- **Minimum coverage:** ≥85% for generation logic

### Integration Tests
- Test complete plan generation for beginner single-discipline user
- Test complete plan generation for advanced multi-discipline user
- Test plan generation with injury constraints
- Test plan generation for time-bound goal (12-week HYROX race)
- Test plan generation for open-ended goal (general fitness)
- Test plan regeneration modifies existing plan
- **Minimum coverage:** ≥85% for service operations

### Validation Tests
- Test generated plans contain no contraindicated exercises
- Test plans respect user's schedule availability
- Test plans maintain logical progression
- Test plans don't exceed safe volume limits
- Test deload weeks are scheduled appropriately

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Code follows service layer patterns
- Generation algorithm is well-documented
- Training science principles are documented in code comments
- Generated plans pass validation checks
- Performance is acceptable (<5 seconds for plan generation)
- Service is ready for API integration
