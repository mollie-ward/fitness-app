# Feature Requirements Document (FRD)
## Progress Tracking & Completion

**Feature ID:** FRD-005  
**Priority:** High (P0)  
**Status:** Draft  
**Last Updated:** January 28, 2026  
**Related PRD:** [PRD Section 4 - REQ-8](../prd.md)

---

## 1. Feature Overview

### Purpose
Enable users to mark workouts as complete and track their training consistency, adherence, and progress over time through clear visual metrics and feedback.

### Problem Statement
Without clear progress tracking, users lose motivation, can't see their consistency patterns, and struggle to understand whether they're on track to achieve their goals. Tracking completion creates accountability and provides the positive reinforcement needed for long-term adherence.

### User Value
- Users gain sense of accomplishment by marking workouts complete
- Visual progress indicators motivate continued consistency
- Users can identify patterns (streaks, drop-offs) and course-correct
- Completion data helps Coach Tom provide personalized feedback and motivation
- Users have evidence of their commitment and improvement over time

---

## 2. Success Criteria

### Acceptance Criteria
- [ ] Users can mark any scheduled workout as complete with a single action
- [ ] Completed workouts are visually distinguished from incomplete workouts
- [ ] Users can view completion statistics (workouts completed this week, this month, overall)
- [ ] System tracks completion streaks (consecutive training days or weeks)
- [ ] Users can see completion percentage for current week and current plan
- [ ] Completed workouts display completion date/time
- [ ] Users can view historical completion data for past weeks/months
- [ ] Completion data feeds into Coach Tom's adaptive recommendations
- [ ] Users cannot accidentally mark future workouts as complete

### Key Performance Indicators (KPIs)
- **Completion Rate:** 50%+ of scheduled workouts marked complete on average
- **Tracking Engagement:** 95%+ of users who complete workouts mark them as complete
- **Weekly Consistency:** 60%+ of users complete at least 3 workouts per week (when scheduled)
- **Streak Impact:** Users on 3+ week streaks show 25% higher retention rates
- **Progress View Engagement:** 40%+ of users view their progress stats at least weekly

### Out of Scope (for this feature)
- Performance tracking (weights lifted, distances run, times recorded)
- Detailed workout session data (set-by-set, exercise-by-exercise)
- Integration with wearable devices or fitness trackers
- Social progress sharing or comparisons with other users
- Advanced analytics (heart rate zones, power output, cadence)
- Video recording or form analysis

---

## 3. User Stories & Scenarios

### Core User Stories

**Story 1: Immediate Completion**
```gherkin
As a user who just finished today's workout,
I want to mark it complete with a single tap,
So that I feel accomplished and my progress is recorded immediately.
```

**Story 2: Weekly Accountability**
```gherkin
As a user trying to stay consistent,
I want to see how many workouts I've completed this week,
So that I can gauge whether I'm on track with my plan.
```

**Story 3: Streak Motivation**
```gherkin
As a user who has trained consistently for 2 weeks,
I want to see my streak highlighted,
So that I feel motivated to maintain it and not break the chain.
```

**Story 4: Historical Review**
```gherkin
As a user reflecting on my progress,
I want to look back at past months to see my completion patterns,
So that I understand my consistency trends and identify improvement opportunities.
```

**Story 5: Goal Progress**
```gherkin
As a user with a 12-week plan,
I want to see what percentage of my total workouts I've completed,
So that I can understand how far I've come and how much remains.
```

**Story 6: Missed Workout Awareness**
```gherkin
As a user who missed yesterday's session,
I want to see it clearly marked as missed,
So that I'm aware and can decide whether to make it up or move forward.
```

### User Journey Example

**Scenario:** Marcus finishes his Wednesday strength workout.

1. Marcus completes his final set and cool-down
2. App displays "Great work! Mark this workout complete?"
3. Marcus taps "Complete Workout" button
4. Confirmation animation plays (checkmark, confetti, or subtle celebration)
5. Calendar immediately updates to show Wednesday's workout with completion checkmark
6. Progress stats update: "3 of 4 workouts completed this week - You're on track!"
7. Marcus sees updated 2-week streak indicator: "🔥 14-day training streak!"
8. Coach Tom sends motivational message: "Nice work today, Marcus! That's 14 days in a row—your consistency is paying off. One more workout this week!"

---

## 4. Functional Requirements

### WHAT the feature must do:

**FR-1: Workout Completion Action**
- Must provide clear, single-action method to mark workout complete (button, swipe, tap)
- Completion action must be accessible from workout detail view and calendar
- Must confirm completion with visual feedback (animation, checkmark, color change)
- Must timestamp completion (date and time)

**FR-2: Completion Status Display**
- Completed workouts must be visually distinct on calendar (checkmark, green highlight, etc.)
- Incomplete past workouts (missed) must be visually indicated
- Future workouts must not allow completion marking
- Rest days should not show completion status (intentional non-training days)

**FR-3: Completion Statistics**
- Must calculate and display: workouts completed this week, this month, all-time
- Must calculate completion percentage for current week (e.g., "3 of 4 completed")
- Must calculate overall plan completion (e.g., "24 of 48 total workouts completed")
- Must track total training days (all-time counter)

**FR-4: Streak Tracking**
- Must identify consecutive training days (days with completed workouts)
- Must identify consecutive training weeks (weeks with at least minimum threshold of completions)
- Must display current active streak prominently
- Must record personal best streak (longest ever achieved)

**FR-5: Historical Progress View**
- Must allow users to view past weeks/months of completion data
- Must show completion patterns over time (calendar heatmap or similar visualization)
- Must indicate high-consistency periods vs. drop-off periods
- Must be accessible from dedicated progress/stats section

**FR-6: Missed Workout Handling**
- Must automatically mark scheduled workouts as "missed" if not completed by end of scheduled day
- Must differentiate between intentionally skipped vs. unintentionally missed (if user provides feedback)
- Must not penalize users for rest days or plan modifications

**FR-7: Completion Undo/Edit**
- Must allow users to undo accidental completion marks
- Must allow marking late completions (e.g., completed yesterday but forgot to log)
- Must preserve data integrity (prevent gaming/manipulation of stats)

**FR-8: Integration with Adaptive System**
- Completion data must feed into Coach Tom's awareness and feedback
- Missed workout patterns should trigger proactive coaching interventions
- High consistency should trigger encouragement and potential intensity increases

---

## 5. Non-Functional Requirements

### Performance
- Completion action must register instantly (<200ms)
- Progress statistics must update in real-time upon completion
- Historical data must load within 1 second

### Usability
- Completion action must be obvious and require minimal effort
- Progress stats must be understandable at a glance (no complex interpretations needed)
- Streak indicators must be motivating, not stressful or guilt-inducing

### Data Integrity
- Completion timestamps must be accurate and immutable
- System must handle timezone changes correctly
- Must prevent data loss if user loses connectivity during completion marking

### Motivational Design
- Completion feedback must feel rewarding (positive reinforcement)
- Progress visualizations should emphasize accomplishments, not failures
- Missed workouts should be acknowledged neutrally, not punitively

---

## 6. Dependencies & Constraints

### Dependencies
- **FRD-002 (AI Training Plan Generation):** Completion tracking requires structured plan with scheduled workouts
- **FRD-004 (Workout Calendar):** Calendar displays completion status indicators
- **FRD-003 (Conversational AI Coach):** Coach Tom references completion data in conversations
- **FRD-006 (Adaptive Adjustment):** Completion patterns trigger plan adaptations

### Constraints
- Cannot track detailed workout performance without user input (no automatic rep/weight logging)
- Cannot integrate with wearables in initial release (manual completion only)
- Must handle users who train outside scheduled days (flexibility in logging)
- Cannot gamify excessively (avoid promoting overtraining through streak pressure)

### Integration Points
- **FRD-003 (Coach Tom):** Completion data informs motivational messages and feedback
- **FRD-004 (Calendar):** Completion status displayed on calendar view
- **FRD-006 (Adaptive Adjustment):** Low completion rates trigger plan difficulty adjustments

---

## 7. Progress Visualization

### WHAT progress displays must show:

**Weekly Summary:**
- "X of Y workouts completed this week"
- Visual progress bar or pie chart
- Days remaining in current week
- Comparison to typical weekly target

**Overall Plan Progress:**
- "X of Y total workouts completed"
- Percentage complete toward plan end
- Estimated completion date based on current pace

**Streak Display:**
- "🔥 X-day training streak"
- Personal best streak record
- Streak milestone celebrations (1 week, 2 weeks, 1 month, etc.)

**Historical View:**
- Calendar heatmap showing completion density over past months
- Monthly completion percentages
- Trend line (improving consistency vs. declining)

**Discipline Breakdown (if training multiple):**
- Completion rates by discipline (HYROX: 80%, Running: 90%, Strength: 60%)
- Identifies which disciplines user is most/least consistent with

---

## 8. Open Questions

1. **Streak Definition:** Should streaks count consecutive calendar days or consecutive scheduled training days?
2. **Partial Credit:** Should users be able to mark workouts as "partially completed" if they only finish part of it?
3. **Retroactive Logging:** How far back should users be allowed to mark workouts complete (1 day? 1 week? unlimited)?
4. **Streak Freezes:** Should users get "grace days" to preserve streaks during illness/travel (like Duolingo)?
5. **Completion Nudges:** Should app send reminders if user hasn't marked obvious completion (e.g., opened app after scheduled workout time)?
6. **Performance Notes:** Should users be able to add notes to completed workouts (e.g., "felt great" vs. "struggled")?
7. **Reset Options:** Should users be able to reset stats/streaks if they want to start fresh?

---

## 9. Acceptance Testing Scenarios

### Test Scenario 1: Basic Completion
- **Given:** User has completed today's scheduled workout
- **When:** User taps "Mark Complete" button
- **Then:** Workout should show checkmark, calendar updates, and weekly progress increments by 1

### Test Scenario 2: Streak Tracking
- **Given:** User has completed workouts for 6 consecutive scheduled days
- **When:** User completes day 7
- **Then:** System should display "7-day training streak" with celebration indicator

### Test Scenario 3: Missed Workout Detection
- **Given:** User had a workout scheduled yesterday but didn't complete it
- **When:** User views calendar today
- **Then:** Yesterday's workout should be marked as "missed" with distinct visual indicator

### Test Scenario 4: Historical Review
- **Given:** User has been training for 8 weeks
- **When:** User navigates to progress/stats view
- **Then:** User should see completion heatmap or chart showing 8 weeks of data with patterns visible

### Test Scenario 5: Completion Undo
- **Given:** User accidentally marks wrong workout complete
- **When:** User immediately taps "Undo" or edits completion
- **Then:** Completion should be removed, stats should update accordingly

---

## 10. Metrics & Success Tracking

### Completion Behavior Metrics
- **Immediate Marking Rate:** Percentage of completed workouts marked within 1 hour of completion
- **Delayed Marking Rate:** Percentage marked 1+ hours after completion
- **Unmarked Workouts:** Percentage of likely-completed workouts never marked (indicates tracking friction)

### Consistency Metrics
- **Weekly Completion Rate:** Average percentage of scheduled weekly workouts completed
- **Streak Distribution:** Histogram of user streak lengths
- **Longest Streaks:** Track cohort of users with 30+ day streaks
- **Completion Decay:** Time-to-first-missed-workout after onboarding

### Engagement Correlation Metrics
- **Progress View Engagement:** Correlation between viewing stats and future completion rates
- **Streak Motivation:** Completion rate difference for users on active streaks vs. not
- **Milestone Impact:** Completion rate changes around streak milestones (7 days, 14 days, 30 days)

### Adaptive System Metrics
- **Coach Tom Trigger Rate:** How often low completion triggers proactive coaching
- **Recovery Rate:** Percentage of users who improve completion after missing 2+ workouts

---

**Document Status:** Draft v1.0  
**Next Steps:** Progress visualization mockups, streak celebration design, completion UX flow, data schema for historical tracking
