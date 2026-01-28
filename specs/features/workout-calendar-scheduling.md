# Feature Requirements Document (FRD)
## Workout Calendar & Scheduling

**Feature ID:** FRD-004  
**Priority:** High (P0)  
**Status:** Draft  
**Last Updated:** January 28, 2026  
**Related PRD:** [PRD Section 4 - REQ-7, REQ-11](../prd.md)

---

## 1. Feature Overview

### Purpose
Provide users with a visual calendar interface that clearly displays today's workout, the current week's training plan, and upcoming key sessions across all training disciplines.

### Problem Statement
Without a clear visual representation of their training schedule, users struggle to plan their week, prepare mentally for upcoming workouts, and maintain consistency. A well-designed calendar transforms an abstract plan into an actionable, day-by-day roadmap.

### User Value
- Users know exactly what to do today without searching or confusion
- Weekly view helps users plan their schedule and prepare equipment in advance
- Visual differentiation between disciplines helps users mentally prepare (running shoes vs. gym bag)
- Seeing upcoming workouts creates accountability and reduces decision fatigue
- Progress visualization (completed vs. upcoming) provides motivation and sense of accomplishment

---

## 2. Success Criteria

### Acceptance Criteria
- [ ] Today's workout is prominently displayed with clear call-to-action
- [ ] Users can view their training plan in daily, weekly, and monthly views
- [ ] Each workout clearly indicates discipline type (HYROX, running, strength)
- [ ] Workouts visually distinguish between completed, scheduled, and missed sessions
- [ ] Calendar displays workout focus/type (e.g., "Interval Run", "Full HYROX Simulation", "Upper Body Strength")
- [ ] Users can tap/click workouts to see detailed information
- [ ] Calendar updates in real-time when plans are modified via Coach Tom or other means
- [ ] Users can navigate forward/backward through weeks and months
- [ ] Key workouts or milestone sessions are visually highlighted

### Key Performance Indicators (KPIs)
- **Daily View Engagement:** 80%+ of active users check today's workout daily
- **Weekly Planning:** 60%+ of users view their full week's plan at least once per week
- **Calendar Clarity:** <5% of users contact support asking "what workout should I do today?"
- **Workout Initiation:** 70%+ of completed workouts are started from the calendar interface

### Out of Scope (for this feature)
- Integration with external calendars (Google Calendar, Outlook)
- Social features (sharing calendars with friends or coaches)
- Workout reminders or push notifications (separate feature)
- Detailed workout execution interface (rep counters, timers, exercise demos)
- Calendar export or printing functionality

---

## 3. User Stories & Scenarios

### Core User Stories

**Story 1: Daily Check-In**
```gherkin
As a user starting my day,
I want to immediately see what today's workout is,
So that I can plan my schedule and prepare mentally.
```

**Story 2: Weekly Planning**
```gherkin
As a user planning my week on Sunday evening,
I want to see all of this week's scheduled workouts,
So that I can organize my time, work commitments, and social plans accordingly.
```

**Story 3: Multi-Discipline Clarity**
```gherkin
As a user training in multiple disciplines,
I want to visually distinguish between running, HYROX, and strength days at a glance,
So that I prepare the right gear and mindset for each session.
```

**Story 4: Progress Visualization**
```gherkin
As a consistent user,
I want to see which workouts I've completed over the past weeks,
So that I feel proud of my progress and motivated to continue.
```

**Story 5: Future Planning**
```gherkin
As a user with an upcoming business trip,
I want to look ahead at next week's plan,
So that I can request modifications from Coach Tom before conflicts arise.
```

**Story 6: Workout Details**
```gherkin
As a user curious about tomorrow's workout,
I want to tap on it to see more details about what it entails,
So that I can mentally prepare and understand the session's purpose.
```

### User Journey Example

**Scenario:** Sarah opens the app on Wednesday morning.

1. App opens to calendar view
2. **Today (Wednesday)** is prominently displayed: "HYROX Simulation - 45 min" with HYROX discipline color
3. Sarah taps on today's workout to see details: "Full HYROX simulation focusing on transitions and pacing. Practice moving quickly between stations."
4. Sarah scrolls to see the rest of this week:
   - Monday (completed): ✓ "Easy Run - 30 min" (green checkmark)
   - Tuesday (missed): ⚠ "Strength - Upper Body" (grayed out)
   - Thursday (scheduled): "Tempo Run - 40 min"
   - Saturday (scheduled): "Long Run - 90 min" (highlighted as key workout)
5. Sarah swipes forward to next week to see what's coming
6. She returns to today's workout and taps "Start Workout"

---

## 4. Functional Requirements

### WHAT the feature must do:

**FR-1: Today's Workout Display**
- Must prominently display today's scheduled workout at the top of calendar view
- Must show workout title, discipline type, and approximate duration
- Must include clear call-to-action button (e.g., "Start Workout", "View Details")
- If no workout scheduled today, must display rest day indicator
- Must update automatically at midnight to show new day's workout

**FR-2: Multi-View Calendar**
- Must support daily view (today's focus)
- Must support weekly view (7-day overview)
- Must support monthly view (high-level planning)
- Users must be able to switch between views seamlessly

**FR-3: Discipline Visual Differentiation**
- Must use distinct visual indicators for each discipline (color coding, icons, or both)
- HYROX workouts must be visually distinct from running workouts
- Running workouts must be visually distinct from strength workouts
- Mixed or hybrid workouts must indicate multiple disciplines

**FR-4: Workout Status Indicators**
- Completed workouts must show clear completion indicator (checkmark, green highlight)
- Scheduled future workouts must be visually distinct from past workouts
- Missed workouts (past scheduled date but not completed) must be indicated
- Today's workout must stand out from all other days

**FR-5: Workout Information**
- Each calendar entry must display: workout title/type, discipline, approximate duration
- Users must be able to tap/click for expanded details
- Expanded view should include: full description, workout focus, rationale (if available)

**FR-6: Navigation**
- Must allow users to navigate backward to view past weeks/months
- Must allow users to navigate forward to view upcoming weeks/months
- Must include "Today" button to quickly return to current date
- Must preserve user's selected view (daily/weekly/monthly) across navigation

**FR-7: Real-Time Updates**
- Calendar must refresh immediately when user completes a workout
- Calendar must update when plan is modified (via Coach Tom or other triggers)
- Must handle concurrent updates without requiring manual refresh

**FR-8: Key Workout Highlighting**
- Must visually highlight milestone or particularly important workouts
- Examples: race simulation days, test weeks, long runs, peak training sessions
- Highlighting must be distinct from regular workout display

**FR-9: Empty State Handling**
- If no plan exists (new user before onboarding completion), must prompt user to create profile
- If user is on rest week or taper, must clearly indicate intentional rest
- Must never show blank/empty calendar without explanation

---

## 5. Non-Functional Requirements

### Performance
- Calendar must load within 1 second on app open
- Switching between daily/weekly/monthly views must be instant (<200ms)
- Scrolling/swiping between weeks must be smooth (60fps)

### Usability
- Calendar must be intuitive for users unfamiliar with fitness apps
- Color differentiation must be accessible to colorblind users (icons/patterns in addition to color)
- Text must be readable on all device sizes (mobile, tablet)
- Touch targets must be appropriately sized for mobile interaction (minimum 44x44pt)

### Visual Design
- Calendar must feel motivating and energizing, not clinical or overwhelming
- Completed workout indicators should create sense of achievement
- Design must align with Coach Tom's personality (supportive, professional, approachable)
- Must maintain visual consistency with rest of app

### Accessibility
- Must support screen readers for visually impaired users
- Must provide text alternatives for color-based information
- Must support dynamic text sizing for readability
- Must be navigable via keyboard (for tablet/desktop use)

---

## 6. Dependencies & Constraints

### Dependencies
- **FRD-002 (AI Training Plan Generation):** Calendar displays plan data from generated training plans
- **FRD-003 (Conversational AI Coach):** Plan modifications via Coach Tom update calendar in real-time
- **FRD-005 (Progress Tracking):** Completion data determines workout status indicators
- **Coach Tom Avatar:** May be displayed in calendar for encouragement or guidance

### Constraints
- Must work on various screen sizes (small mobile to large tablet)
- Must handle plans of varying lengths (4 weeks to 52+ weeks)
- Must support users training 1-7 days per week without cluttered interface
- Cannot assume users will use external calendar apps (must be self-contained)

### Integration Points
- **FRD-005 (Progress Tracking):** Tapping "Start Workout" from calendar initiates tracking
- **FRD-003 (Coach Tom):** Users can access Coach Tom from calendar to ask questions or request modifications
- **FRD-006 (Adaptive Adjustment):** Calendar reflects adaptations triggered by missed workouts or feedback

---

## 7. Visual Design Requirements

### WHAT the calendar must show:

**Daily View:**
- Large, clear display of today's workout
- Quick summary of tomorrow's workout (preview)
- Current week's completion progress (e.g., "2 of 4 workouts completed")

**Weekly View:**
- 7-day grid or list showing all workouts for the current week
- Clear visual separation between past, present, and future days
- Completion indicators for past days
- Discipline-based color coding or icons

**Monthly View:**
- High-level overview of all scheduled training days
- Visual indication of workout density (which weeks are harder/easier)
- Key milestone workouts highlighted
- Current week emphasized

**Discipline Color Scheme (example):**
- HYROX: Orange/Red
- Running: Blue
- Strength: Purple/Green
- Rest: Gray
- Mixed/Hybrid: Multi-color indicator

---

## 8. Open Questions

1. **Default View:** Should app open to daily, weekly, or monthly view by default (or user preference)?
2. **Past History:** How far back should users be able to scroll (all-time history or limited to current plan)?
3. **Future Visibility:** Should users see beyond their current plan duration (e.g., what happens after 12-week plan ends)?
4. **Workout Swap:** Should calendar allow direct drag-and-drop to reschedule workouts, or only via Coach Tom?
5. **Rest Day Labels:** Should rest days show motivational messages ("Recovery Day - You've earned it!") or remain minimal?
6. **Weekly Summary:** Should calendar show weekly volume statistics (total training time, total workouts)?
7. **Integration with Device Calendar:** Should we allow one-way export to Google Calendar/iCal for visibility?

---

## 9. Acceptance Testing Scenarios

### Test Scenario 1: New User Daily View
- **Given:** User has just completed onboarding and received their first plan
- **When:** User opens calendar for the first time
- **Then:** Today's workout should be prominently displayed with clear "Start Workout" button

### Test Scenario 2: Weekly Planning
- **Given:** User is viewing weekly calendar on Sunday
- **When:** User looks at upcoming week
- **Then:** All scheduled workouts should be visible with discipline indicators, durations, and rest days clearly marked

### Test Scenario 3: Completion Tracking
- **Given:** User completes Wednesday's workout
- **When:** User returns to calendar view
- **Then:** Wednesday's workout should show completion checkmark, and today (Thursday) should now be highlighted

### Test Scenario 4: Multi-Discipline Clarity
- **Given:** User has a plan with HYROX, running, and strength workouts
- **When:** User views weekly calendar
- **Then:** Each discipline should be visually distinct through color, icon, or both

### Test Scenario 5: Plan Modification Update
- **Given:** User tells Coach Tom to skip tomorrow's workout
- **When:** Calendar refreshes
- **Then:** Tomorrow's workout should be removed/marked as rest day without manual page refresh

---

## 10. Metrics & Success Tracking

### Engagement Metrics
- **Daily Calendar Views:** Number of times users check calendar per day
- **Weekly Planning Sessions:** Users who view full weekly calendar at start of each week
- **Workout Initiations:** Percentage of workouts started directly from calendar vs. other entry points

### Usability Metrics
- **View Preference:** Distribution of daily vs. weekly vs. monthly view usage
- **Navigation Patterns:** How often users look ahead vs. look back in calendar
- **Time to Workout:** Average time from calendar view to starting workout

### Business Impact Metrics
- **Completion Correlation:** Correlation between calendar engagement and workout completion rates
- **Planning Behavior:** Do users who review full week ahead complete more workouts?
- **Dropout Prediction:** Does decreased calendar viewing predict plan abandonment?

---

**Document Status:** Draft v1.0  
**Next Steps:** UI/UX mockups, calendar component selection, discipline color scheme finalization, mobile responsiveness testing
