# Feature Requirements Document (FRD)
## AI Training Plan Generation

**Feature ID:** FRD-002  
**Priority:** High (P0)  
**Status:** Draft  
**Last Updated:** January 28, 2026  
**Related PRD:** [PRD Section 4 - REQ-1, REQ-2, REQ-3, REQ-10](../prd.md)

---

## 1. Feature Overview

### Purpose
Generate personalized, multi-week training plans across three disciplines (HYROX, running, strength training) that are tailored to each user's fitness level, goals, available training days, and constraints.

### Problem Statement
Static, one-size-fits-all training plans don't account for individual differences in fitness level, schedule constraints, goals, or life circumstances. Users need intelligent, dynamic plan generation that creates a roadmap specifically for them—just like a human coach would.

### User Value
- Users receive training plans designed specifically for their current fitness level (not too easy, not impossible)
- Plans fit exactly into their available training days (no wasted recommendations)
- Workouts progress logically toward specific goals with appropriate periodization
- Multi-discipline plans are balanced to avoid conflicts and overtraining
- Users trust that every workout serves a purpose in their overall progression

---

## 2. Success Criteria

### Acceptance Criteria
- [ ] Plans are generated based on all user profile data (fitness level, goals, schedule, injuries, background)
- [ ] Plans accommodate exactly the number of training days specified by the user
- [ ] Plans support all three disciplines: HYROX, running, and strength training
- [ ] Plans include multi-week progression with increasing intensity/complexity
- [ ] For time-bound goals, plans scale duration and intensity to align with target dates
- [ ] Each workout specifies discipline type, session focus, and approximate duration
- [ ] Plans respect injury constraints by avoiding or modifying contraindicated exercises
- [ ] Users can train in one, two, or all three disciplines simultaneously
- [ ] Generated plans are coherent and follow established training principles (progressive overload, recovery, specificity)

### Key Performance Indicators (KPIs)
- **Plan Generation Success Rate:** 99%+ of completed profiles generate valid training plans
- **Plan Relevance:** <10% of users immediately request major plan changes after generation
- **Workout Completion Rate:** 50%+ of prescribed workouts are marked complete
- **Plan Abandonment:** <20% of users abandon their plan within first 4 weeks
- **Multi-Discipline Balance:** Users training multiple disciplines show balanced completion rates across all disciplines

### Out of Scope (for this feature)
- Real-time plan execution (exercise demonstrations, timers, rep counting)
- Integration with wearable device data (heart rate, GPS, power metrics)
- Social plan sharing or community templates
- Nutritional recommendations or meal plans
- Exercise video demonstrations
- Equipment-specific plan variations

---

## 3. User Stories & Scenarios

### Core User Stories

**Story 1: Single-Discipline Goal**
```gherkin
As a runner preparing for a half-marathon in 12 weeks,
I want a running-specific training plan that builds my endurance progressively,
So that I can complete the race comfortably and avoid injury.
```

**Story 2: Multi-Discipline Training**
```gherkin
As a HYROX competitor who also values strength,
I want a balanced plan that includes both HYROX-specific work and strength sessions,
So that I improve race performance without neglecting foundational strength.
```

**Story 3: Schedule Constraint**
```gherkin
As someone who can only train 3 days per week,
I want workouts that maximize efficiency on those days,
So that I still make meaningful progress despite limited availability.
```

**Story 4: Time-Bound Goal**
```gherkin
As a user with a competition in exactly 8 weeks,
I want a plan that peaks my performance at the right time,
So that I'm not under-trained or over-trained on race day.
```

**Story 5: Beginner Progression**
```gherkin
As a complete beginner to strength training,
I want a plan that starts with foundational movements and proper form,
So that I build a solid base before advancing to complex exercises.
```

### User Journey Example

**Scenario:** Marcus, 28, intermediate runner, beginner HYROX athlete, training 4 days/week for a HYROX race in 16 weeks.

1. Marcus completes onboarding with his profile
2. System analyzes: 16-week timeline, intermediate running base, beginner HYROX experience, 4 days/week
3. Plan generation creates:
   - **Week 1-4:** Foundation phase (2 running sessions, 1 HYROX simulation, 1 strength/conditioning)
   - **Week 5-8:** Build phase (increased volume, HYROX-specific exercises introduced)
   - **Week 9-12:** Intensity phase (harder intervals, full HYROX simulations)
   - **Week 13-15:** Peak phase (race-pace work, specificity)
   - **Week 16:** Taper (reduced volume, maintain intensity)
4. Each week fits exactly into Monday, Wednesday, Friday, Sunday
5. Marcus sees his full 16-week plan and can start immediately

---

## 4. Functional Requirements

### WHAT the feature must do:

**FR-1: Profile-Driven Plan Generation**
- Must use all user profile data as inputs (fitness level, goals, schedule, injuries, training background)
- Must generate plans within 5 seconds of profile completion or update
- Must create coherent plans regardless of profile combination

**FR-2: Multi-Week Progression**
- Must create plans spanning multiple weeks (minimum 4 weeks, up to 24+ weeks for long-term goals)
- Must implement progressive overload principles (increasing volume, intensity, or complexity over time)
- Must include periodization (foundation, build, intensity, peak, recovery phases)
- Must prevent excessive week-to-week jumps in difficulty

**FR-3: Discipline-Specific Plans**
- Must generate HYROX-specific workouts (race simulations, station practice, transition drills)
- Must generate running workouts (easy runs, intervals, tempo, long runs)
- Must generate strength workouts (compound movements, accessory work, progressive resistance)
- Must provide clear workout descriptions for each session type

**FR-4: Schedule Fitting**
- Must allocate workouts to exactly the days specified by the user
- Must respect rest days (no workouts assigned to unavailable days)
- Must intelligently sequence workouts (e.g., hard/easy alternation, proper recovery)
- Must handle variable weekly availability (e.g., 2-7 days/week)

**FR-5: Goal Alignment**
- For time-bound goals: Must calculate appropriate training phases to peak at target date
- For distance/performance goals: Must structure progression toward that specific capability
- For general fitness: Must create balanced, sustainable long-term programming
- Must adjust plan duration based on goal urgency (compressed vs. extended timelines)

**FR-6: Multi-Discipline Integration**
- Must balance workload when user trains multiple disciplines simultaneously
- Must prevent scheduling conflicts (e.g., hard running and hard leg strength on consecutive days)
- Must allocate appropriate volume to each discipline based on user priorities
- Must ensure recovery between similar muscle groups and energy systems

**FR-7: Injury Accommodation**
- Must exclude or modify exercises that contradict reported injuries
- Must provide alternative exercises when primary movements are contraindicated
- Must maintain training effect despite exercise substitutions

**FR-8: Workout Detail**
- Each workout must specify: discipline type, session focus, approximate duration
- Must provide enough detail for users to understand workout intent
- Must differentiate between session types (e.g., "Easy Run" vs. "Interval Training" vs. "Long Run")

**FR-9: Plan Metadata**
- Must include plan overview (total weeks, training days per week, primary goal)
- Must show key milestone workouts or test weeks
- Must indicate current training phase (foundation, build, peak, etc.)

---

## 5. Non-Functional Requirements

### Performance
- Plan generation must complete within 5 seconds
- Plans must be retrievable instantly after initial generation
- System must support concurrent plan generation for thousands of users

### Accuracy
- Plans must follow evidence-based training principles (progressive overload, specificity, recovery)
- Workout sequencing must prevent overtraining and injury risk
- Plans for similar profiles should show consistency while maintaining personalization

### Scalability
- System must support unlimited plan variations based on profile combinations
- Must handle plans ranging from 4 weeks to 52+ weeks
- Must support users training 1-7 days per week

### Reliability
- Plan generation must succeed 99%+ of the time for valid profiles
- Edge cases (extreme constraints, conflicting goals) must fail gracefully with user guidance
- Generated plans must remain valid even if user profile changes slightly

---

## 6. Dependencies & Constraints

### Dependencies
- **FRD-001 (User Onboarding):** Requires completed user profile as input
- **Exercise Database:** Requires library of exercises categorized by discipline, difficulty, muscle groups, and equipment
- **Training Methodology:** Requires codified training principles (periodization models, progression rules)
- **Coach Tom Integration:** Plans must be explainable and modifiable through conversational interface

### Constraints
- Plans must work with equipment available in typical gym settings (no specialized equipment assumptions)
- Must accommodate users with minimal training time (as low as 2 days/week)
- Cannot require fitness testing or baseline measurements beyond self-reported fitness level
- Must avoid medical advice or prescriptive training for injured users (disclaimer needed)
- Plans must be understandable by users without coaching or exercise science background

### Integration Points
- **FRD-003 (Conversational AI Coach):** Coach Tom explains plan rationale and makes real-time modifications
- **FRD-004 (Workout Calendar):** Generated plan populates the visual calendar
- **FRD-005 (Progress Tracking):** Plan structure enables completion tracking
- **FRD-006 (Adaptive Adjustment):** Plan regenerates when user provides feedback or circumstances change
- **FRD-007 (Injury Management):** Injury updates trigger plan re-generation with accommodations

---

## 7. Training Plan Structure

### Plan Components (WHAT the plan includes)

**Weekly Structure:**
- Each week contains workouts fitting user's available days
- Workouts distributed to balance intensity and recovery
- Clear indication of which day is "hard" vs. "easy" vs. "rest"

**Workout Types per Discipline:**

*HYROX:*
- Race simulations (full or partial)
- Station-specific practice (ski erg, sled, burpees, wall balls, etc.)
- Transition drills
- Hybrid conditioning (cardio + strength circuits)

*Running:*
- Easy/recovery runs
- Interval training (short, medium, long intervals)
- Tempo/threshold runs
- Long runs (endurance building)

*Strength Training:*
- Full-body sessions
- Upper/lower splits
- Push/pull/legs splits
- Compound lift focus (squat, deadlift, bench, overhead press)
- Accessory work for muscle balance

**Progression Mechanisms:**
- Volume progression (more sets, reps, distance, time)
- Intensity progression (heavier weights, faster paces, shorter rest)
- Complexity progression (more advanced exercises or movement patterns)
- Density progression (more work in same timeframe)

---

## 8. Open Questions

1. **Plan Length Limits:** What's the maximum reasonable plan duration (24 weeks? 52 weeks? Indefinite?)?
2. **Auto-Regeneration:** Should plans auto-regenerate periodically, or only when user explicitly requests changes?
3. **Exercise Library Size:** How many unique exercises/workouts are needed to create sufficient variety?
4. **Deload Weeks:** Should recovery/deload weeks be built in automatically every 3-4 weeks?
5. **Testing Weeks:** Should plans include periodic fitness tests to validate progression?
6. **Equipment Variability:** Should users specify available equipment during onboarding to tailor exercise selection?
7. **Volume Limits:** What are safe maximum weekly training volumes for beginners vs. advanced users?
8. **Goal Conflicts:** How should the system handle competing goals (e.g., "get stronger" + "run faster" with only 3 days/week)?

---

## 9. Acceptance Testing Scenarios

### Test Scenario 1: Single-Discipline Beginner Plan
- **Given:** Beginner runner, 3 days/week, 8-week goal of running 5K continuously
- **When:** Plan is generated
- **Then:** Plan should show 8 weeks of progressive running workouts (walk/run intervals → continuous running), scheduled on selected days

### Test Scenario 2: Multi-Discipline Advanced Plan
- **Given:** Advanced strength athlete + intermediate runner, 5 days/week, 12-week general fitness goal
- **When:** Plan is generated
- **Then:** Plan should balance 3 strength sessions and 2 running sessions per week, avoiding hard sessions on consecutive days

### Test Scenario 3: Time-Bound HYROX Goal
- **Given:** Intermediate user, 4 days/week, HYROX race in exactly 16 weeks
- **When:** Plan is generated
- **Then:** Plan should show clear periodization peaking at week 16, with taper in final week

### Test Scenario 4: Minimal Time Availability
- **Given:** Beginner, 2 days/week, general fitness goal
- **When:** Plan is generated
- **Then:** Plan should provide efficient full-body workouts on those 2 days without overwhelming the user

### Test Scenario 5: Injury-Constrained Plan
- **Given:** Intermediate user with knee injury (no running), 4 days/week, strength goal
- **When:** Plan is generated
- **Then:** Plan should exclude running, provide upper body and low-impact conditioning alternatives

---

## 10. Metrics & Success Tracking

### Generation Quality Metrics
- **Generation Success Rate:** Percentage of valid profiles producing valid plans
- **Generation Time:** Average and P95 time to generate plans
- **Plan Variability:** Measure of plan uniqueness across similar user profiles

### User Engagement Metrics
- **First Workout Completion:** Percentage of users who complete Week 1, Day 1 workout
- **Weekly Adherence:** Average percentage of prescribed workouts completed per week
- **Plan Duration:** Average time users stay on their original plan before requesting major changes

### Plan Effectiveness Metrics
- **Goal Achievement Rate:** Percentage of users with time-bound goals who report success
- **Perceived Difficulty:** User feedback on whether workouts are too easy/hard/just right
- **Injury Rate:** Percentage of users reporting injuries potentially caused by plan (should be <2%)

---

**Document Status:** Draft v1.0  
**Next Steps:** Exercise database design, training algorithm development, periodization model implementation
