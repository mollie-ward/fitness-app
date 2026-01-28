# Task: Injury Management Service

**Task ID:** 017  
**Feature:** Injury Management & Workout Adaptation (FRD-007)  
**Priority:** High (P1)  
**Estimated Effort:** Large  

---

## Description

Implement service for managing user injuries and limitations, including exercise contraindication logic, substitute exercise recommendations, and integration with plan adaptation to modify workouts based on injury constraints.

---

## Dependencies

- **Task 004:** User profile data model must be complete (includes injury entities)
- **Task 007:** Exercise database must be complete (includes contraindications)
- **Task 016:** Adaptive plan adjustment service must be complete

---

## Technical Requirements

### Injury Management Service Interface
- Create `IInjuryManagementService` interface:
  - ReportInjuryAsync(userId, injuryDetails)
  - UpdateInjuryStatusAsync(userId, injuryId, newStatus)
  - MarkInjuryResolvedAsync(userId, injuryId)
  - GetActiveInjuriesAsync(userId)
  - GetContraindicatedExercisesAsync(userId)
  - GetSubstituteExerciseAsync(exerciseId, injuryConstraints)

### Injury Reporting & Storage
- Capture injury details:
  - Body part/region (shoulder, knee, back, ankle, etc.)
  - Injury type (acute, chronic, overuse)
  - Pain description (sharp, dull, aching)
  - Movement restrictions (overhead, impact, heavy load, rotation)
  - Reported date
  - Severity (mild, moderate, severe)
  
- Store in InjuryLimitation entity (from Task 004)
- Support multiple concurrent injuries
- Track injury history for pattern detection

### Contraindication Engine

#### Exercise Filtering
- Query exercise database for contraindications
- Match injury type/body part to restricted exercises
- Build list of exercises to avoid
- Consider severity (mild injury = modify, severe = exclude)

#### Movement Pattern Analysis
- Map injuries to movement patterns:
  - Shoulder injury → overhead, push, pull restrictions
  - Knee injury → squat, lunge, impact restrictions
  - Lower back → hinge, heavy load, rotation restrictions
  - Ankle → running, jumping, plyometric restrictions
- Filter exercises by movement pattern

#### Equipment Considerations
- Some injuries allow equipment substitutions:
  - Wrist pain: barbell → dumbbell or machine
  - Shoulder: overhead press → landmine press
- Maintain training effect with equipment swaps

### Substitute Exercise Selection

#### Matching Criteria
- Preserve training stimulus (same muscle groups)
- Match difficulty level
- Respect injury constraints
- Available equipment
- User fitness level

#### Substitution Rules
- Create substitution matrix for common scenarios:

| Injured Area | Restricted Movement | Original Exercise | Substitute |
|--------------|-------------------|------------------|-----------|
| Shoulder | Overhead | Overhead Press | Landmine Press |
| Shoulder | Pull | Pull-ups | Inverted Rows |
| Knee | Squat | Back Squat | Leg Press (reduced ROM) |
| Lower Back | Hinge | Deadlift | Trap Bar Deadlift |
| Ankle | Impact | Running | Rowing, Cycling |

#### Fallback Strategy
- If no perfect substitute exists:
  - Option 1: Modify exercise (reduced ROM, lighter load)
  - Option 2: Alternative exercise (different stimulus)
  - Option 3: Remove and redistribute volume

### Injury Status Updates

#### Status Progression
- Support status transitions:
  - Active (newly reported)
  - Improving (healing progress)
  - Resolved (fully healed)
  - Chronic (ongoing limitation)

#### Progressive Re-Introduction
- When status = "Improving":
  - Gradually reintroduce restricted exercises
  - Start at 50% intensity
  - Increase by 10-15% per week
  - Monitor for pain recurrence

### Integration with Plan Adaptation
- When injury reported:
  1. Update user profile with injury details
  2. Get contraindicated exercises
  3. Trigger plan adaptation service
  4. Scan upcoming workouts
  5. Replace/remove affected exercises
  6. Generate explanation of changes
  7. Notify user via Coach Tom

- When injury status updated:
  1. Reassess contraindications
  2. If improving: reintroduce exercises gradually
  3. If resolved: restore original programming
  4. Update affected workouts

### Safety Disclaimers
- Include medical disclaimer on all injury features
- Recommend professional medical evaluation for:
  - Sharp pain
  - Pain persisting >2 weeks
  - Worsening symptoms
  - Multiple concurrent injuries
- Never provide diagnosis or treatment advice

### API Endpoints (Extension of User Profile API)

#### POST /api/users/profile/injuries
- Report new injury (from Task 005, ensure integration)
- Triggers workout adaptation

#### PUT /api/users/profile/injuries/{injuryId}/status
- Update injury healing status
- Triggers re-evaluation of plan

#### GET /api/injuries/contraindications
- Get list of contraindicated exercises for user
- Response: Exercise IDs and reasons

#### GET /api/injuries/substitutes/{exerciseId}
- Get substitute exercise for contraindicated exercise
- Response: Substitute exercise details

---

## Acceptance Criteria

- ✅ Users can report injuries with body part and severity
- ✅ Service identifies contraindicated exercises based on injury
- ✅ Substitute exercises maintain training stimulus
- ✅ Multiple concurrent injuries are handled correctly
- ✅ Injury status can be updated (improving, resolved)
- ✅ Plan adaptation is triggered when injury reported
- ✅ Gradual reintroduction occurs when status = improving
- ✅ Full programming restored when injury resolved
- ✅ Medical disclaimers are displayed appropriately

---

## Testing Requirements

### Unit Tests
- Test injury contraindication matching
- Test substitute exercise selection logic
- Test multiple injury handling
- Test status transition logic
- Test gradual reintroduction calculations
- **Minimum coverage:** ≥85% for service logic

### Integration Tests
- Test injury reporting updates profile and triggers adaptation
- Test contraindication filtering excludes correct exercises
- Test substitute selection provides valid alternatives
- Test status update triggers plan modification
- Test concurrent injuries are processed correctly
- **Minimum coverage:** ≥85% for service operations

### Exercise Substitution Tests
- Test shoulder injury excludes overhead exercises
- Test knee injury excludes squatting movements
- Test back injury excludes heavy hinge patterns
- Test ankle injury excludes high-impact activities
- Test substitutes maintain training stimulus

### Safety Tests
- Verify medical disclaimers are present
- Test severe injuries trigger conservative modifications
- Test dangerous exercise combinations are prevented

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Contraindication logic is comprehensive
- Substitute selection provides quality alternatives
- Integration with plan adaptation works correctly
- Medical disclaimers are clear and prominent
- API endpoints are documented
