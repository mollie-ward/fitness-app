# Feature Requirements Document (FRD)
## User Onboarding & Fitness Profiling

**Feature ID:** FRD-001  
**Priority:** High (P0)  
**Status:** Draft  
**Last Updated:** January 28, 2026  
**Related PRD:** [PRD Section 4 - REQ-1](../prd.md)

---

## 1. Feature Overview

### Purpose
Enable new users to provide comprehensive fitness information during their first app experience, creating a personalized profile that drives all subsequent training plan generation and coaching interactions.

### Problem Statement
Generic fitness apps fail because they don't understand individual starting points, goals, schedules, or limitations. Without accurate user profiling, the app cannot deliver on its promise of personalized, adaptive coaching that feels like working with a real trainer.

### User Value
- Users receive training plans appropriate for their actual fitness level (not too easy, not overwhelming)
- Plans fit realistically into their actual weekly schedule
- Goals are tracked and plans are tailored to specific target dates or milestones
- Training accounts for existing injuries or limitations from day one
- Users feel understood and confident that the app "knows them"

---

## 2. Success Criteria

### Acceptance Criteria
- [ ] All new users complete onboarding before accessing training plans
- [ ] Users can specify their fitness level across three disciplines independently (beginner/intermediate/advanced for HYROX, running, and strength)
- [ ] Users can set at least one specific, measurable goal (e.g., "Complete HYROX race on March 15, 2026")
- [ ] Users can indicate exact days of the week they are available to train
- [ ] Users can specify existing injuries or physical limitations
- [ ] Users can indicate their training background and history
- [ ] The onboarding flow takes no longer than 5 minutes to complete
- [ ] Users can modify their profile information after initial onboarding
- [ ] All collected information is stored and accessible for training plan generation

### Key Performance Indicators (KPIs)
- **Onboarding Completion Rate:** 90%+ of users who start onboarding complete it
- **Time to Complete:** Average completion time ≤ 4 minutes
- **Profile Accuracy:** <5% of users modify core profile data (fitness level, goals) within first week
- **Return Rate:** 80%+ of users who complete onboarding log back in within 48 hours

### Out of Scope (for this feature)
- Fitness assessment tests (e.g., timed runs, max rep tests)
- Body composition measurements or photos
- Integration with previous workout history from other apps
- Social profile information (profile photos, bio, friends)
- Payment or subscription management
- Nutrition or dietary preferences

---

## 3. User Stories & Scenarios

### Core User Stories

**Story 1: First-Time Beginner**
```gherkin
As a new gym member with no structured training experience,
I want to indicate that I'm a beginner across all disciplines,
So that my first workouts don't overwhelm or injure me.
```

**Story 2: Goal-Oriented Athlete**
```gherkin
As someone training for a specific HYROX competition on June 1st,
I want to set that date as my target goal during onboarding,
So that my training plan builds appropriately toward peak performance.
```

**Story 3: Limited Availability**
```gherkin
As a busy professional who can only train Monday, Wednesday, and Saturday,
I want to specify exactly those three days during onboarding,
So that I never receive workouts on days I can't commit to.
```

**Story 4: Injured User**
```gherkin
As someone currently recovering from a knee injury,
I want to report this limitation during onboarding,
So that my initial plan avoids exercises that could aggravate it.
```

**Story 5: Mixed Experience Levels**
```gherkin
As an experienced runner but novice weightlifter,
I want to set different fitness levels for running (advanced) vs. strength (beginner),
So that each discipline's workouts match my actual capabilities.
```

### User Journey Example

**Scenario:** Sarah, 32, gym member for 6 months, wants to prepare for her first HYROX race in 4 months.

1. Sarah opens the app for the first time
2. She's greeted by Coach Tom and prompted to create her fitness profile
3. She selects:
   - **Primary Goal:** "Complete HYROX race" with target date "May 25, 2026"
   - **Fitness Levels:** Running (Intermediate), HYROX (Beginner), Strength (Intermediate)
   - **Training Background:** "I've been running 5Ks for a year but never done HYROX"
   - **Available Days:** Monday, Wednesday, Friday, Sunday (4 days/week)
   - **Injuries/Limitations:** "Previous ankle sprain—avoid box jumps"
4. Profile is saved and Coach Tom confirms understanding
5. Sarah proceeds to receive her personalized 16-week HYROX preparation plan

---

## 4. Functional Requirements

### WHAT the feature must do:

**FR-1: Fitness Level Assessment**
- Must allow users to self-assess their fitness level for each discipline separately
- Must provide clear definitions or examples for beginner/intermediate/advanced levels
- Must support users with zero experience in one or more disciplines

**FR-2: Goal Setting**
- Must enable users to define at least one specific goal (e.g., race date, weight target, endurance milestone)
- Must support time-bound goals with specific target dates
- Must support open-ended goals (e.g., "general fitness improvement")
- Must allow multiple concurrent goals across different disciplines

**FR-3: Schedule Configuration**
- Must capture exact days of the week the user is available to train
- Must support variable availability (e.g., 2 days/week, 7 days/week)
- Must allow users to specify minimum and maximum weekly training frequency

**FR-4: Training Background Collection**
- Must collect information about previous structured training experience
- Must understand user's familiarity with gym equipment and exercises
- Must identify users who are complete beginners vs. those returning after a break

**FR-5: Injury & Limitation Reporting**
- Must allow users to report current injuries, chronic conditions, or physical limitations
- Must capture enough detail to inform workout modifications (e.g., "lower back pain," "shoulder impingement")
- Must distinguish between temporary injuries and permanent limitations

**FR-6: Profile Persistence**
- Must save all onboarding data before the user can access training plans
- Must make profile data accessible for plan generation and coach interactions
- Must allow users to view and edit their profile at any time after onboarding

**FR-7: Onboarding Flow Control**
- Must require completion of mandatory fields before proceeding
- Must allow users to skip or defer optional information
- Must provide progress indication throughout the onboarding process
- Must allow users to go back and modify previous answers

**FR-8: Data Validation**
- Must validate that selected training days don't conflict (e.g., ensure at least 1 day is selected)
- Must validate that goal dates are in the future
- Must ensure fitness level selections are made for disciplines the user intends to train

---

## 5. Non-Functional Requirements

### Usability
- Onboarding questions must be clear and unambiguous
- The interface must feel conversational and welcoming, not like a clinical questionnaire
- Users must be able to complete onboarding on mobile devices with minimal scrolling
- Error messages must guide users toward correction without frustration

### Performance
- Each step of onboarding must load within 1 second
- Profile data must be saved within 2 seconds of submission
- Users must be able to navigate back/forward between steps without delay

### Accessibility
- Onboarding must be completable by users with visual, motor, or cognitive accessibility needs
- All form fields must have clear labels and support screen readers
- Color must not be the only indicator of required fields or errors

### Data Privacy
- Users must be informed how their profile data will be used
- Sensitive information (injuries, health conditions) must be stored securely
- Users must be able to delete their profile data at any time

---

## 6. Dependencies & Constraints

### Dependencies
- **Profile Data Storage:** Requires backend system capable of storing structured user profile data
- **Coach Tom Integration:** Onboarding must introduce Coach Tom as the user's guide
- **Training Plan Generator:** All collected data must feed into the plan generation system
- **UI Framework:** Requires mobile-friendly UI supporting multi-step forms

### Constraints
- Must work offline (allow users to complete onboarding without constant internet)
- Must support iOS and Android (assuming mobile app)
- Must comply with data privacy regulations (GDPR, CCPA)
- Cannot require users to provide medical records or doctor's approval
- Cannot make users feel judged or inadequate based on fitness level selections

### Integration Points
- **FRD-002 (AI Training Plan Generation):** Profile data is the primary input for plan creation
- **FRD-003 (Conversational AI Coach):** Coach Tom references profile data in all interactions
- **FRD-006 (Adaptive Plan Adjustment):** Profile changes trigger plan recalculation

---

## 7. Open Questions

1. **Fitness Level Granularity:** Should we offer more than three levels (e.g., beginner, early-intermediate, late-intermediate, advanced, elite)?
2. **Re-onboarding:** If a user hasn't used the app for 6+ months, should they be prompted to re-validate their profile?
3. **Goal Flexibility:** Can users have multiple simultaneous goals with different target dates (e.g., 5K race in March, HYROX race in June)?
4. **Injury Updates:** Should users be prompted to update injury status periodically, or only when they proactively report changes?
5. **Training History Detail:** How much detail about previous training should we collect (just "yes/no" or specific programs/durations)?
6. **Minimum Commitment:** Should we set a minimum weekly training frequency (e.g., at least 2 days) to ensure meaningful progress?
7. **Profile Editing Guardrails:** If a user changes fitness level mid-program, should we warn them about plan disruption?

---

## 8. Acceptance Testing Scenarios

### Test Scenario 1: Complete Beginner Onboarding
- **Given:** A brand new user with no fitness experience
- **When:** They complete onboarding selecting "beginner" for all disciplines, 3 days/week, no specific goal
- **Then:** They should receive a general fitness improvement plan appropriate for beginners, scheduled on their selected days

### Test Scenario 2: Goal-Driven Onboarding
- **Given:** An intermediate user preparing for a HYROX race in 90 days
- **When:** They specify the race date, intermediate fitness level, and 4 training days/week
- **Then:** They should receive a 12-week HYROX-focused plan that peaks at the target date

### Test Scenario 3: Injured User Onboarding
- **Given:** A user with a current shoulder injury
- **When:** They report the injury during onboarding
- **Then:** Their initial plan must exclude or modify upper-body intensive exercises

### Test Scenario 4: Profile Modification
- **Given:** An existing user who completed onboarding 2 weeks ago
- **When:** They edit their profile to add an additional training day
- **Then:** Their plan should adapt to include the new day without losing progress

### Test Scenario 5: Mixed Experience Onboarding
- **Given:** A user who is an advanced runner but beginner in strength training
- **When:** They set different fitness levels for each discipline
- **Then:** Running workouts should be challenging while strength workouts remain foundational

---

## 9. Metrics & Success Tracking

### Onboarding Funnel Metrics
- **Started Onboarding:** Number of users who begin the first step
- **Step Completion Rate:** Percentage completing each individual step
- **Drop-off Points:** Specific steps where users abandon onboarding
- **Completion Time Distribution:** Histogram of time-to-complete

### Profile Quality Metrics
- **Profile Completeness:** Percentage of users who fill all optional fields
- **Early Modifications:** Users who change profile within first 7 days (indicates poor initial guidance)
- **Goal Clarity:** Percentage of users who set specific, measurable goals vs. vague goals

### Downstream Impact Metrics
- **Plan Generation Success:** Percentage of completed profiles that successfully generate a training plan
- **Day-1 Engagement:** Users who start their first workout within 24 hours of onboarding
- **Profile-Driven Personalization:** Variance in generated plans based on profile differences (confirms personalization is working)

---

**Document Status:** Draft v1.0  
**Next Steps:** Development team technical design, UX/UI mockups, backend data schema design
