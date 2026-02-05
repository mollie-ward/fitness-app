# Task: Progress Tracking UI Components

**Task ID:** 013  
**GitHub Issue:** [#25](https://github.com/mollie-ward/fitness-app/issues/25)  
**Feature:** Progress Tracking & Completion (FRD-005)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Large  

---

## Description

Implement frontend UI components for marking workouts complete, displaying progress statistics, visualizing streaks, and showing historical completion data.

---

## Dependencies

- **Task 002:** Frontend scaffolding must be complete
- **Task 011:** Workout calendar UI must be complete
- **Task 012:** Progress tracking backend must be complete

---

## Technical Requirements

### Workout Completion Component
- "Mark Complete" button on workout detail view
- Confirmation animation on completion (checkmark, confetti, celebration)
- Immediate visual feedback (button state change)
- Undo option displayed briefly after completion
- Optimistic UI update (update UI before API response)

### Progress Statistics Dashboard
- Create progress/stats view with sections:
  
  **Weekly Summary:**
  - "X of Y workouts completed this week"
  - Visual progress bar or pie chart
  - Days remaining in current week
  - Comparison to target completion rate
  
  **Overall Plan Progress:**
  - "X of Y total workouts completed"
  - Percentage complete toward plan end
  - Estimated completion date based on current pace
  - Visual progress indicator
  
  **All-Time Stats:**
  - Total training days
  - Total workouts completed
  - Average weekly completion rate
  - Member since date

### Streak Display Component
- Prominent streak indicator with fire emoji or icon
- Display current active streak (e.g., "🔥 14-day streak")
- Show personal best streak record
- Visual indicator for approaching milestones
- Celebration animation when milestone reached
- Motivational message based on streak status

### Historical Progress View
- Calendar heatmap showing completion density
- Color intensity indicates frequency (GitHub-style)
- Tap day to see workouts completed
- Month-by-month navigation
- Toggle between:
  - All disciplines combined
  - Individual discipline view (HYROX, Running, Strength)

### Discipline Breakdown (Multi-Discipline Users)
- Completion percentage by discipline
- Visual comparison (bar chart or pie chart)
- Identify most/least consistent discipline
- Drill down to see specific workouts

### Completion Action Flow
1. User finishes workout
2. Taps "Mark Complete" button
3. Completion animation plays
4. Workout status updates in calendar
5. Progress stats update in real-time
6. Streak counter increments
7. Milestone celebration (if applicable)
8. Brief "Undo" option displayed (3-5 seconds)

### API Integration
- Call PUT /api/progress/workouts/{id}/complete
- Fetch GET /api/progress/stats/* for statistics
- Fetch GET /api/progress/streak for streak info
- Fetch GET /api/progress/history for heatmap data
- Handle loading states gracefully
- Implement retry logic for failed requests

### State Management
- Track completion status locally (optimistic updates)
- Sync with backend on success
- Rollback on error
- Refresh statistics after completion
- Cache statistics with appropriate TTL

### Motivational Messaging
- Display Coach Tom avatar with progress messages
- Context-aware encouragement:
  - After completion: "Great work!"
  - On streak milestone: "14 days in a row - amazing!"
  - On week completion: "All 4 workouts done this week!"
  - On low completion: "Let's get back on track!"

### Empty States
- No active plan: "Generate your first plan to start tracking"
- New user: "Complete your first workout to see progress"
- No workouts this week: "Rest week - recovery is important too"

### Accessibility
- Completion button is clearly labeled
- Screen reader announces completion status
- Keyboard accessible controls
- Progress charts have text alternatives
- Color is not sole indicator of status

---

## Acceptance Criteria

- ✅ "Mark Complete" button marks workout as completed
- ✅ Completion triggers visual celebration/confirmation
- ✅ Calendar updates immediately to show completion
- ✅ Progress statistics update in real-time
- ✅ Streak counter increments correctly
- ✅ Undo option allows reverting completion
- ✅ Progress dashboard displays accurate statistics
- ✅ Historical heatmap shows completion patterns
- ✅ Discipline breakdown shows per-discipline completion rates
- ✅ Milestone celebrations appear at appropriate times
- ✅ Optimistic UI updates provide immediate feedback
- ✅ Error states handle failed API calls gracefully

---

## Testing Requirements

### Unit Tests
- Test completion action updates local state
- Test undo functionality reverts completion
- Test statistics calculation display logic
- Test streak calculation display logic
- **Minimum coverage:** ≥85% for component logic

### Component Tests
- Test "Mark Complete" button renders and functions
- Test completion animation plays
- Test progress stats display correctly
- Test streak indicator shows current streak
- Test heatmap renders with historical data
- Test milestone celebration appears
- Test undo button appears and works
- **Minimum coverage:** ≥85% for UI components

### Integration Tests
- Test marking complete calls API and updates UI
- Test statistics fetch and display
- Test streak info fetch and display
- Test historical data fetches for heatmap
- Test optimistic update rolls back on error
- Test concurrent completions are handled
- **Minimum coverage:** ≥85% for data integration

### Accessibility Tests
- Test keyboard navigation for completion action
- Test screen reader announces completion
- Test focus management after completion
- Test charts have text alternatives

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Code follows React and TypeScript best practices
- Components are accessible
- Animations are smooth and not overwhelming
- Optimistic updates provide instant feedback
- Error handling is user-friendly
- Progress visualizations are clear and motivating
- Works responsively on all devices
