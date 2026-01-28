# Task: Workout Calendar UI Component

**Task ID:** 011  
**Feature:** Workout Calendar & Scheduling (FRD-004)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Large  

---

## Description

Implement the frontend workout calendar component that displays training plans in daily, weekly, and monthly views. Show today's workout prominently, visualize completion status, and differentiate between disciplines with color coding.

---

## Dependencies

- **Task 002:** Frontend scaffolding must be complete
- **Task 010:** Training plan API endpoints must be available

---

## Technical Requirements

### Calendar Component Architecture
- Create main calendar component with view switching
- Implement responsive design for mobile, tablet, desktop
- Support multiple view modes: Daily, Weekly, Monthly
- Persist user's preferred view in localStorage

### Daily View
- Display today's workout prominently
  - Workout name and discipline
  - Estimated duration
  - Brief description
  - "Start Workout" call-to-action button
- Show tomorrow's workout as preview
- Display current week progress (e.g., "2 of 4 workouts completed")
- Show Coach Tom avatar with motivational message

### Weekly View
- Display 7-day grid or list for current week
- Show each day with:
  - Day of week and date
  - Workout name (if scheduled)
  - Discipline indicator (icon and/or color)
  - Estimated duration
  - Completion status (checkmark, pending, missed)
- Highlight today's workout
- Distinguish past, present, and future days visually
- Show rest days clearly

### Monthly View
- Display calendar grid for current month
- Show workout density (which days have workouts)
- Use color coding for disciplines
- Highlight key/milestone workouts
- Indicate current week
- Show completion percentage for month

### Discipline Visual Differentiation
- Implement color scheme:
  - HYROX: Orange/Red
  - Running: Blue
  - Strength: Purple/Green
  - Hybrid: Multi-color or gradient
  - Rest: Gray/Neutral
- Use icons for each discipline
- Ensure color scheme is accessible (WCAG AA contrast)
- Support colorblind-friendly patterns/icons

### Workout Status Indicators
- Visual states for workouts:
  - Completed: Checkmark, green highlight
  - Scheduled (future): Default state
  - In Progress: Partial/loading indicator
  - Missed: Red indicator, distinct styling
  - Skipped: Gray/neutral indicator
- Animate status changes smoothly

### Navigation Controls
- Date navigation (previous/next week, month)
- "Today" button to jump to current date
- Month/year picker for quick navigation
- Smooth transitions between dates
- Keyboard shortcuts for navigation (arrow keys)

### Workout Detail Modal/Panel
- Tap workout to see details:
  - Full workout description
  - List of exercises with sets/reps
  - Estimated time breakdown
  - Rationale/focus area
  - "Start Workout" button
  - Option to access Coach Tom for questions

### API Integration
- Fetch current plan from backend
- Fetch workouts for current week/month
- Implement efficient data fetching (avoid over-fetching)
- Cache calendar data with appropriate invalidation
- Real-time updates when plan changes (via Coach Tom)
- Handle loading states gracefully

### State Management
- Manage calendar state (selected date, view mode)
- Track workout completion status locally before sync
- Handle optimistic updates for better UX
- Sync with backend on network reconnection

### Performance Optimization
- Lazy load workouts outside visible date range
- Virtualize monthly view for large plans
- Optimize re-renders with React.memo
- Debounce navigation actions

### Accessibility
- Keyboard navigation through calendar
- Screen reader announcements for date changes
- Proper ARIA labels for workout states
- Focus management for modal/detail views
- Support for reduced motion preferences

---

## Acceptance Criteria

- ✅ Calendar displays in daily, weekly, and monthly views
- ✅ Today's workout is prominently displayed
- ✅ Users can switch between views seamlessly
- ✅ Disciplines are visually differentiated by color/icon
- ✅ Completion status is clearly indicated on workouts
- ✅ Calendar navigation works (previous/next, today button)
- ✅ Tapping workout shows detailed modal/panel
- ✅ Calendar fetches data from backend API
- ✅ Calendar updates in real-time when plan changes
- ✅ Works responsively on mobile, tablet, and desktop
- ✅ Loading states are displayed during data fetch
- ✅ Empty states handled (no plan, no workout today)

---

## Testing Requirements

### Unit Tests
- Test calendar state management
- Test date navigation logic
- Test workout status display logic
- Test discipline color mapping
- **Minimum coverage:** ≥85% for calendar logic

### Component Tests
- Test calendar renders in all three views
- Test switching between views updates display
- Test workout detail modal opens/closes
- Test completion status indicators display correctly
- Test navigation updates calendar
- Test "Today" button returns to current date
- **Minimum coverage:** ≥85% for UI components

### Integration Tests
- Test calendar fetches workouts from API
- Test calendar displays correct workouts for date range
- Test tapping workout opens detail with correct data
- Test status changes reflect in calendar
- Test calendar updates when plan modified
- **Minimum coverage:** ≥85% for data integration

### Accessibility Tests
- Test keyboard navigation through calendar
- Test screen reader compatibility
- Test ARIA labels are present
- Test focus management in modal

### Visual Regression Tests
- Test calendar renders correctly across screen sizes
- Test color scheme is consistent
- Test transitions are smooth

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Code follows React and TypeScript best practices
- Calendar is accessible (keyboard, screen reader)
- Responsive design works on all target devices
- Performance is acceptable (smooth scrolling, quick navigation)
- Color scheme is colorblind-friendly
- API integration is efficient (no over-fetching)
- Empty and error states are user-friendly
