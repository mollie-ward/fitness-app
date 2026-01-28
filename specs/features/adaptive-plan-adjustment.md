# Feature Requirements Document (FRD)
## Adaptive Plan Adjustment

**Feature ID:** FRD-006  
**Priority:** High (P0)  
**Status:** Draft  
**Last Updated:** January 28, 2026  
**Related PRD:** [PRD Section 4 - REQ-4, REQ-6](../prd.md)

---

## 1. Feature Overview

### Purpose
Enable training plans to dynamically adapt and adjust in real-time based on user feedback, missed workouts, schedule changes, perceived difficulty, injuries, and other life circumstances.

### Problem Statement
Life is unpredictable. Static training plans fail because they can't account for unexpected travel, illness, changing energy levels, or shifting priorities. Users need plans that flex with their reality while maintaining progression toward goals—just like a human coach would adjust based on ongoing feedback.

### User Value
- Plans remain realistic and achievable despite life's unpredictability
- Users don't abandon plans when circumstances change—plans adapt instead
- Training intensity matches actual capacity (not too easy, not overwhelming)
- Missed workouts don't derail entire plans—system recovers intelligently
- Users feel supported rather than judged when they need flexibility

---

## 2. Success Criteria

### Acceptance Criteria
- [ ] Plans automatically adjust when users miss multiple consecutive workouts
- [ ] Users can request intensity changes (harder/easier) via Coach Tom
- [ ] Users can modify weekly training frequency mid-plan (add/remove days)
- [ ] Plans recalculate when users report injuries or new limitations
- [ ] System maintains goal alignment while adapting to changes
- [ ] Adaptations preserve training progression principles (don't reset progress unnecessarily)
- [ ] Users receive explanations for why adaptations were made
- [ ] Plans don't adapt so frequently that they lose coherence

### Key Performance Indicators (KPIs)
- **Adaptation Rate:** 40%+ of users request at least one plan modification during their program
- **Plan Abandonment Reduction:** Users who adapt plans show 30%+ lower dropout rates than those who don't
- **Missed Workout Recovery:** 70%+ of users who miss 2+ workouts continue training after system adapts
- **Satisfaction with Adaptations:** 85%+ of users find adapted plans appropriate (measured via feedback)
- **Coherence Maintenance:** <5% of adapted plans produce illogical workout sequences

### Out of Scope (for this feature)
- Automatic adaptation based on wearable device data (heart rate variability, fatigue scores)
- AI-driven adaptation without user input or awareness
- Complete plan regeneration from scratch (prefer targeted adjustments)
- Social recommendations (adapting based on what other similar users do)

---

## 3. User Stories & Scenarios

### Core User Stories

**Story 1: Missed Workout Recovery**
```gherkin
As a user who missed 3 workouts due to illness,
I want the system to adjust my plan to get back on track,
So that I don't feel overwhelmed trying to catch up or completely abandon my progress.
```

**Story 2: Intensity Adjustment**
```gherkin
As a user who found this week's workouts too easy,
I want to tell Coach Tom to increase difficulty,
So that future weeks challenge me appropriately and I continue progressing.
```

**Story 3: Schedule Change**
```gherkin
As a user whose work schedule changed and I can now only train 3 days instead of 5,
I want to update my availability and have my plan rebalanced,
So that my plan remains achievable without sacrificing too much progress.
```

**Story 4: Goal Timeline Extension**
```gherkin
As a user whose race got postponed by 4 weeks,
I want to extend my training plan to align with the new date,
So that I don't peak too early and can maintain optimal progression.
```

**Story 5: Perceived Difficulty Feedback**
```gherkin
As a user who is struggling to complete workouts at prescribed intensity,
I want the system to recognize I'm overreaching and dial back,
So that I can sustain training without burning out or getting injured.
```

### User Journey Example

**Scenario:** Emma has a 4-day-per-week plan but catches the flu and misses an entire week.

1. **Week 1 (Missed):** Emma's plan shows 4 workouts; she completes 0 due to illness
2. **Monday (Week 2):** Emma opens app feeling recovered
3. Coach Tom proactively messages: "Hey Emma, I noticed you missed last week's workouts. Are you feeling better? Let's adjust your plan so you can ease back in without overdoing it."
4. Emma responds: "Yes, I'm better. What should I do?"
5. Coach Tom: "Great! I've adjusted this week to be a recovery/rebuild week. We're reducing intensity by 20% and focusing on movement quality. Your timeline to your goal is still on track—we'll make up the missed volume gradually over the next 3 weeks."
6. **Updated Plan (Week 2):**
   - Original: Hard interval run, HYROX simulation, heavy strength, long run
   - Adapted: Easy run, light HYROX drills, moderate strength, moderate run
7. Emma's calendar updates immediately
8. Emma completes Week 2 successfully and plan gradually ramps back up in Weeks 3-4

---

## 4. Functional Requirements

### WHAT the feature must do:

**FR-1: Automatic Missed Workout Adaptation**
- Must detect when user misses 2+ consecutive scheduled workouts
- Must trigger proactive Coach Tom check-in to understand reason
- Must adjust upcoming workouts to facilitate re-entry (reduced intensity or volume)
- Must prevent over-ambitious catch-up plans that risk injury or burnout

**FR-2: User-Requested Intensity Changes**
- Must allow users to request "make it harder" or "make it easier" via Coach Tom
- Must apply intensity adjustments to future workouts (not retroactively)
- Must preserve exercise selection and structure while modulating difficulty
- Must explain what specific changes were made (e.g., "increased weights by 10%", "added 1 extra interval")

**FR-3: Schedule Frequency Modification**
- Must allow users to change weekly training day count mid-plan
- Must redistribute workouts across new schedule without losing key sessions
- Must warn users if reduction in days will impact goal timeline
- Must rebalance recovery and intensity across new availability

**FR-4: Injury-Triggered Adaptation**
- Must update user profile when new injury is reported
- Must scan upcoming workouts for contraindicated exercises
- Must substitute or remove problematic movements
- Must maintain training effect through alternative exercises

**FR-5: Goal Timeline Adjustment**
- Must allow users to extend or compress goal target dates
- Must recalculate periodization and progression to align with new timeline
- Must prevent unsafe compression (e.g., cramming 16 weeks into 4 weeks)
- Must warn users of implications (faster/slower progression required)

**FR-6: Perceived Effort Adaptation**
- Must accept user feedback on workout difficulty ("too easy", "just right", "too hard")
- Must trend feedback over time to identify systematic over/under-reaching
- Must adjust future intensity when patterns emerge (e.g., 3 "too hard" responses in a row)

**FR-7: Adaptation Explanation**
- Must notify users when plan is automatically adapted
- Must explain why adaptation was made and what changed
- Must maintain transparency (no "black box" changes)

**FR-8: Adaptation Guardrails**
- Must prevent excessive adaptation frequency (avoid daily plan changes)
- Must validate that adaptations maintain training coherence
- Must ensure adaptations don't contradict recent user requests
- Must prevent users from requesting conflicting changes simultaneously

**FR-9: Plan Coherence Preservation**
- Must maintain logical workout sequencing after adaptations
- Must preserve periodization structure (foundation → build → peak → taper)
- Must avoid sudden jumps or drops in difficulty
- Must ensure adaptations don't create overtraining scenarios

---

## 5. Non-Functional Requirements

### Performance
- Plan adaptations must complete within 3 seconds
- Adapted plans must update calendar and workout views immediately
- System must handle multiple concurrent adaptation requests without conflicts

### Accuracy
- Adapted plans must remain aligned with user goals
- Intensity adjustments must follow evidence-based progression principles
- Workout substitutions must maintain equivalent training stimulus

### Usability
- Users must understand what changed and why
- Adaptation process should feel supportive, not punitive
- Users should feel empowered to request changes without guilt

### Reliability
- Adaptations must succeed 99%+ of the time
- Edge cases (extreme requests, conflicting inputs) must be handled gracefully
- System must never produce unsafe or illogical training recommendations

---

## 6. Dependencies & Constraints

### Dependencies
- **FRD-002 (AI Training Plan Generation):** Adaptation engine builds on plan generation logic
- **FRD-003 (Conversational AI Coach):** Coach Tom is primary interface for requesting/explaining adaptations
- **FRD-005 (Progress Tracking):** Completion data triggers automatic adaptations
- **FRD-007 (Injury Management):** Injury updates are a key adaptation trigger

### Constraints
- Must adapt plans without losing weeks of user progress
- Cannot make adaptations that compromise safety (e.g., sudden intensity spikes after layoff)
- Must handle users who frequently change their minds (prevent whiplash)
- Must work for users at all experience levels (beginners to advanced)

### Integration Points
- **FRD-003 (Coach Tom):** All user-requested adaptations flow through conversational interface
- **FRD-004 (Calendar):** Adapted plans immediately reflect in calendar view
- **FRD-005 (Progress Tracking):** Missed workout patterns trigger adaptive responses
- **FRD-007 (Injury Management):** Injury reports trigger targeted adaptations

---

## 7. Adaptation Scenarios

### WHAT adaptations look like:

**Scenario 1: Missed Week (Illness)**
- **Trigger:** User misses 5+ consecutive scheduled days
- **Action:** Reduce next week's intensity by 20-30%, extend plan by 1 week if time-bound goal
- **Rationale:** Prevent injury from returning too aggressively after layoff

**Scenario 2: Too Easy Feedback**
- **Trigger:** User reports "too easy" for 3+ consecutive workouts
- **Action:** Increase volume by 10%, increase intensity by 5-10%, introduce more complex exercises
- **Rationale:** User is under-challenged and not progressing optimally

**Scenario 3: Too Hard Feedback**
- **Trigger:** User reports "too hard" for 3+ consecutive workouts or fails to complete workouts
- **Action:** Reduce volume by 15%, reduce intensity by 10%, simplify exercise selection
- **Rationale:** User is overreaching and at risk of burnout or injury

**Scenario 4: Reduced Availability**
- **Trigger:** User changes from 5 days/week to 3 days/week
- **Action:** Consolidate to 3 full-body or hybrid workouts, remove accessory sessions, prioritize key workouts
- **Rationale:** Maintain training effect with limited time

**Scenario 5: Goal Date Change**
- **Trigger:** User's race postponed from 12 weeks to 16 weeks
- **Action:** Extend foundation and build phases, reduce weekly intensity slightly, add extra recovery weeks
- **Rationale:** Prevent premature peaking with extended timeline

**Scenario 6: New Injury**
- **Trigger:** User reports knee pain
- **Action:** Remove running workouts, substitute with low-impact cardio (rowing, biking), modify squat patterns
- **Rationale:** Allow continued training while avoiding injury aggravation

---

## 8. Open Questions

1. **Adaptation Frequency Limits:** Should there be a maximum number of adaptations per month to prevent plan incoherence?
2. **Automatic vs. Manual:** Which adaptations should be automatic vs. requiring user confirmation?
3. **Undo Adaptations:** Should users be able to revert to the original plan if they don't like the adaptation?
4. **Severity Thresholds:** At what point does adaptation become "start over with new plan" vs. "adjust current plan"?
5. **Conflicting Feedback:** How should system handle "make it harder" followed by "I'm injured" within same week?
6. **Learning from Patterns:** Should system learn user preferences over time (e.g., always prefers shorter, harder workouts)?
7. **Notification Timing:** Should adaptations happen silently, or always notify users immediately?

---

## 9. Acceptance Testing Scenarios

### Test Scenario 1: Missed Week Recovery
- **Given:** User has missed 7 consecutive days due to travel
- **When:** User returns and opens app
- **Then:** Coach Tom should proactively offer adjusted plan with reduced intensity for re-entry week

### Test Scenario 2: Intensity Increase Request
- **Given:** User tells Coach Tom "this week felt too easy"
- **When:** Adaptation is applied
- **Then:** Next week's workouts should show increased volume, intensity, or complexity with explanation

### Test Scenario 3: Schedule Reduction
- **Given:** User changes availability from 4 days/week to 2 days/week
- **When:** Plan is adapted
- **Then:** Calendar should show 2 comprehensive workouts per week, key sessions preserved, timeline adjusted

### Test Scenario 4: Goal Extension
- **Given:** User extends goal date from 8 weeks to 12 weeks
- **When:** Plan recalculates
- **Then:** Periodization should stretch to 12 weeks with more gradual progression

### Test Scenario 5: Conflicting Request Handling
- **Given:** User requests "make it harder" but also reports shoulder injury same day
- **When:** System processes requests
- **Then:** Coach Tom should identify conflict and ask user to clarify priorities

---

## 10. Metrics & Success Tracking

### Adaptation Usage Metrics
- **Adaptation Request Rate:** Percentage of users who request at least one adaptation
- **Adaptation Type Distribution:** Frequency of intensity, schedule, injury, goal adaptations
- **Automatic vs. Manual:** Ratio of system-triggered vs. user-requested adaptations

### Adaptation Effectiveness Metrics
- **Post-Adaptation Retention:** Retention rate of users who receive adaptations vs. those who don't
- **Completion Rate Change:** Workout completion before vs. after adaptation
- **User Satisfaction:** Explicit feedback on whether adaptation felt appropriate

### Plan Coherence Metrics
- **Logical Progression:** Percentage of adapted plans that maintain progressive overload
- **Safety Validation:** Percentage of adaptations flagged as potentially unsafe (should be 0%)
- **Adaptation Stability:** Average time between adaptations (too frequent suggests instability)

---

**Document Status:** Draft v1.0  
**Next Steps:** Adaptation algorithm design, guardrail rule development, Coach Tom adaptation conversation flows, testing framework for plan coherence
