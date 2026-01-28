# Task: Adaptive Plan Adjustment Service

**Task ID:** 016  
**Feature:** Adaptive Plan Adjustment (FRD-006)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Extra Large  

---

## Description

Implement the intelligent plan adaptation service that dynamically adjusts training plans based on missed workouts, user feedback, schedule changes, injuries, and goal timeline modifications. This service ensures plans remain realistic and effective despite life's unpredictability.

---

## Dependencies

- **Task 008:** Training plan data model must be complete
- **Task 009:** Training plan generation service must be complete
- **Task 012:** Progress tracking backend must be complete
- **Task 014:** AI coach backend must be complete

---

## Technical Requirements

### Adaptation Service Interface
- Create `IPlanAdaptationService` interface with methods:
  - AdaptForMissedWorkoutsAsync(userId, missedWorkoutIds)
  - AdaptForIntensityChangeAsync(userId, direction)  // harder/easier
  - AdaptForScheduleChangeAsync(userId, newSchedule)
  - AdaptForInjuryAsync(userId, injuryId)
  - AdaptForGoalTimelineChangeAsync(userId, goalId, newDate)
  - AdaptForPerceivedDifficultyAsync(userId, feedbackPattern)

### Adaptation Triggers

#### Automatic Triggers
- **Missed Workouts Detection:**
  - Monitor for 2+ consecutive missed workouts
  - Trigger proactive adaptation offer via Coach Tom
  - Reduce intensity for re-entry week
  
- **Perceived Difficulty Pattern:**
  - Track "too easy" or "too hard" feedback
  - Trigger adaptation after 3 consistent signals
  - Adjust intensity or volume accordingly

#### User-Requested Triggers
- **Intensity Adjustment:**
  - "Make it harder" increases volume/intensity by 10-15%
  - "Make it easier" reduces volume/intensity by 15-20%
  - Apply to future workouts only (preserve past)
  
- **Schedule Changes:**
  - User changes available training days
  - Redistribute workouts across new schedule
  - Maintain key sessions, drop accessory work if needed
  
- **Injury Reports:**
  - Triggered from injury management service
  - Scan and modify upcoming workouts
  - Substitute contraindicated exercises
  
- **Goal Timeline Changes:**
  - User extends or compresses goal date
  - Recalculate periodization and intensity curve
  - Prevent unsafe compression (maintain minimum build time)

### Adaptation Algorithms

#### Missed Workout Recovery
1. Count consecutive missed days
2. Calculate re-entry intensity reduction (20-30%)
3. Extend plan by missed weeks (if time-bound goal)
4. Reduce next week's volume
5. Gradually ramp back up over 2-3 weeks
6. Preserve overall goal timeline if possible

#### Intensity Adjustment
1. Identify affected workouts (future only)
2. Adjust parameters:
   - Volume: ±10-15% sets/reps
   - Intensity: ±5-10% weight/pace
   - Complexity: swap exercise variations
3. Maintain progressive overload trajectory
4. Preserve recovery periods

#### Schedule Redistribution
1. Get new available days from user
2. Prioritize key/hard workouts
3. Redistribute remaining workouts
4. Balance hard/easy days
5. Adjust weekly volume if days reduced
6. Warn if significant goal impact

#### Injury Accommodation
1. Receive injury contraindications from injury service
2. Scan upcoming workouts for affected exercises
3. Remove or substitute exercises
4. Maintain training stimulus for unaffected areas
5. Update workout descriptions with explanations

#### Goal Timeline Recalculation
1. Calculate new plan duration
2. Adjust periodization phases:
   - Extension: Add build weeks, slower progression
   - Compression: Steeper intensity curve (validate safety)
3. Recalculate weekly intensity distribution
4. Preserve peak and taper timing

### Adaptation Guardrails

#### Safety Validation
- Prevent intensity spikes >20% week-over-week
- Ensure minimum recovery between hard sessions
- Limit adaptation frequency (max 1 per week)
- Validate total weekly volume is safe
- Prevent overtraining scenarios

#### Coherence Checks
- Ensure logical progression maintained
- Prevent contradictory adaptations
- Validate periodization structure intact
- Check for illogical exercise sequences

#### Conflict Resolution
- Handle conflicting requests:
  - "Make harder" + "Injury" → Prioritize safety (injury wins)
  - "More days" + "Make easier" → Resolve via user confirmation
- Ask user to clarify when conflicts detected

### Adaptation Tracking & History
- Create `PlanAdaptation` entity:
  - AdaptationId
  - PlanId
  - Trigger (MissedWorkouts, UserRequest, Injury, etc.)
  - Type (Intensity, Schedule, Timeline, Injury)
  - Description
  - ChangesApplied (JSON or structured data)
  - AppliedAt
  
- Maintain audit trail of all adaptations
- Support reverting recent adaptation if needed

### Integration Points
- Call from AI Coach service when plan modification requested
- Trigger from Progress Tracking when missed workout threshold met
- Trigger from Injury Management when injury reported
- Update calendar and notify user of changes

---

## Acceptance Criteria

- ✅ Service adapts plan for 2+ consecutive missed workouts
- ✅ User can request intensity increase/decrease
- ✅ Schedule changes redistribute workouts correctly
- ✅ Injury reports trigger appropriate modifications
- ✅ Goal timeline changes recalculate periodization
- ✅ Adaptations preserve training progression principles
- ✅ Safety guardrails prevent dangerous modifications
- ✅ Plan coherence is maintained after adaptations
- ✅ Conflicting requests are detected and resolved
- ✅ Users receive explanations for what changed

---

## Testing Requirements

### Unit Tests
- Test missed workout detection logic
- Test intensity adjustment calculations
- Test schedule redistribution algorithm
- Test injury accommodation logic
- Test timeline recalculation
- Test safety validation rules
- Test conflict detection
- **Minimum coverage:** ≥85% for adaptation logic

### Integration Tests
- Test complete adaptation flow for missed workouts
- Test intensity change updates future workouts
- Test schedule change redistributes correctly
- Test injury triggers exercise substitution
- Test timeline change recalculates plan
- Test multiple concurrent adaptations
- Test adaptation history is recorded
- **Minimum coverage:** ≥85% for service operations

### Safety Tests
- Test adaptation doesn't create intensity spikes
- Test compressed timeline rejects unsafe compression
- Test overtraining scenarios are prevented
- Test minimum recovery is preserved

### Coherence Tests
- Test adaptations maintain logical progression
- Test periodization structure remains valid
- Test no illogical exercise sequences created

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Code follows service layer patterns
- Adaptation algorithms are documented
- Safety guardrails are comprehensive
- Integration with AI Coach works end-to-end
- Adaptation history is tracked
- Performance is acceptable (<3 seconds)
