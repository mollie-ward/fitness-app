# Workout Calendar Component - Architecture Summary

**Document Type:** Architecture Guidance  
**Component:** Workout Calendar UI  
**Status:** Ready for Implementation  
**Last Updated:** 2025-02-03  

---

## Overview

This document provides a comprehensive architecture for implementing the Workout Calendar UI Component (Task 011). The calendar is a critical feature that displays training plans in daily, weekly, and monthly views with full accessibility support and ≥85% test coverage.

---

## Quick Links

- **[ADR-0001: Component Architecture](../specs/adr/0001-workout-calendar-component-architecture.md)** - Architectural decisions and rationale
- **[Implementation Guide](./workout-calendar-implementation-guide.md)** - Code examples and patterns
- **[Task Specification](../specs/tasks/011-task-workout-calendar-ui.md)** - Requirements and acceptance criteria

---

## Key Architectural Decisions

### 1. Component Structure: Feature-Based Modular Architecture

**Decision:** Decompose into specialized sub-components with clear responsibilities

**Folder Structure:**
```
src/components/calendar/
├── index.ts                    # Public exports
├── WorkoutCalendar.tsx         # Main orchestrator
├── CalendarHeader.tsx          # Navigation & view switcher
├── views/                      # View components (Daily, Weekly, Monthly)
├── workout/                    # Workout display components
├── utils/                      # Calendar helpers
└── hooks/                      # Custom hooks
```

**Benefits:**
- Clear separation of concerns
- Highly testable
- Reusable components
- Better code organization

### 2. State Management: Zustand + Local State

**Decision:** Use Zustand for global calendar state (view preference, navigation) with local component state for transient UI

**Why:**
- Lightweight with minimal boilerplate
- Excellent TypeScript support
- Built-in persistence for user preferences
- Simple API: `useCalendarStore()`

**What Goes Where:**
- **Zustand Store:** View mode, selected date, navigation actions
- **Component State:** Modal open/closed, hover states, loading indicators
- **Props/Context:** Workout data fetched from API

### 3. API Integration: Service Layer Pattern

**Decision:** Service layer encapsulates API calls with async/await

**Pattern:**
```typescript
// Service layer
TrainingPlanService.getWorkoutsByDateRange(userId, startDate, endDate)

// Custom hook wraps service
useWorkoutData({ userId, startDate, endDate })

// Component consumes hook
const { workouts, isLoading, error } = useWorkoutData({...})
```

**Benefits:**
- Clean separation between data fetching and UI
- Testable in isolation
- Consistent error handling
- Easy to add caching layer later (React Query)

### 4. Type System: Strict TypeScript with Domain Types

**Decision:** Comprehensive type definitions for all domain concepts

**Core Types:**
- `Discipline` - Workout categories (HYROX, Running, Strength, Hybrid, Rest)
- `Workout` - Individual workout sessions
- `TrainingPlan` - Multi-week training programs
- `CalendarView` - View modes (Daily, Weekly, Monthly)
- `WorkoutStatus` - Completion states

**Benefits:**
- Compile-time type safety
- Better IDE autocomplete
- Self-documenting code
- Prevents runtime errors

### 5. Accessibility: WCAG AA Compliance

**Decision:** Build accessibility in from the start, not as an afterthought

**Implementation:**
- Semantic HTML (`<article>`, `<nav>`, `<time>`)
- ARIA labels and descriptions
- Keyboard navigation (arrows, 't' for today, 'd/w/m' for views)
- Focus management with visible indicators
- Color contrast ≥4.5:1 for text
- Icons + color for discipline differentiation (colorblind-friendly)

**Testing:**
- Screen reader compatibility tests
- Keyboard navigation tests
- Color contrast validation

### 6. Performance: Optimized for Large Plans

**Decision:** Multiple optimization strategies for smooth UX

**Techniques:**
- Lazy loading view components with `React.lazy()`
- Memoization with `React.memo()` for expensive components
- Custom equality checks to prevent unnecessary re-renders
- Efficient date calculations (no moment.js, use native Date)
- Suspense boundaries with loading skeletons

### 7. Testing: Comprehensive Coverage Strategy

**Decision:** Test pyramid with ≥85% coverage

**Breakdown:**
- **Unit Tests (60%):** Utilities, helpers, pure functions
- **Component Tests (30%):** Individual component behavior
- **Integration Tests (10%):** Full user workflows

**Tools:**
- Jest for test runner
- React Testing Library for component testing
- `@testing-library/user-event` for user interactions
- Mock Service Worker (MSW) for API mocking (optional)

---

## Implementation Roadmap

### Phase 1: Foundation (Week 1)
- [ ] Create all TypeScript type definitions
- [ ] Set up Zustand calendar store
- [ ] Implement calendar utility functions (date helpers)
- [ ] Create base UI components (Modal, Skeleton)
- [ ] Set up test utilities and mock factories

**Deliverables:**
- `src/types/*.ts` - All type definitions
- `src/stores/calendar-store.ts` - State management
- `src/components/calendar/utils/*.ts` - Helpers
- `src/__tests__/utils/test-utils.tsx` - Test setup

### Phase 2: Atomic Components (Week 2)
- [ ] Build `DisciplineIcon` component
- [ ] Build `WorkoutStatusBadge` component
- [ ] Build `WorkoutCard` component (compact & full)
- [ ] Create `CalendarSkeleton` loading state
- [ ] Write unit tests for each component

**Deliverables:**
- `src/components/calendar/workout/*.tsx` - Workout components
- Component tests with ≥85% coverage

### Phase 3: Views & Navigation (Week 3)
- [ ] Implement `WeeklyView` component
- [ ] Implement `DailyView` component
- [ ] Implement `MonthlyView` component
- [ ] Build `CalendarHeader` with view switcher
- [ ] Build `CalendarControls` for navigation
- [ ] Implement `useCalendarKeyboard` hook

**Deliverables:**
- `src/components/calendar/views/*.tsx` - View components
- `src/components/calendar/hooks/*.ts` - Custom hooks
- View component tests

### Phase 4: Integration & API (Week 4)
- [ ] Create `TrainingPlanService` for API calls
- [ ] Build `useWorkoutData` hook
- [ ] Implement `WorkoutDetail` modal component
- [ ] Wire up main `WorkoutCalendar` orchestrator
- [ ] Add loading and error states
- [ ] Implement optimistic updates

**Deliverables:**
- `src/services/training-plan-service.ts` - API integration
- `src/components/calendar/WorkoutCalendar.tsx` - Main component
- `src/components/calendar/workout/WorkoutDetail.tsx` - Detail modal

### Phase 5: Polish & Testing (Week 5)
- [ ] Performance optimization (memoization, lazy loading)
- [ ] Accessibility audit with screen reader testing
- [ ] Write integration tests for full workflows
- [ ] Visual regression tests (optional)
- [ ] Documentation and inline code comments
- [ ] Code review and refinement

**Deliverables:**
- Integration test suite
- Accessibility compliance report
- Performance benchmarks
- Updated documentation

---

## File Structure Reference

```
frontend/src/
├── components/
│   ├── calendar/                         # Workout Calendar feature
│   │   ├── index.ts                      # Public API exports
│   │   ├── WorkoutCalendar.tsx           # Main component
│   │   ├── CalendarHeader.tsx            # Header with view switcher
│   │   ├── CalendarControls.tsx          # Date navigation
│   │   │
│   │   ├── views/                        # View-specific components
│   │   │   ├── DailyView.tsx
│   │   │   ├── WeeklyView.tsx
│   │   │   └── MonthlyView.tsx
│   │   │
│   │   ├── workout/                      # Workout display components
│   │   │   ├── WorkoutCard.tsx
│   │   │   ├── WorkoutDetail.tsx
│   │   │   ├── WorkoutStatusBadge.tsx
│   │   │   └── DisciplineIcon.tsx
│   │   │
│   │   ├── utils/                        # Calendar utilities
│   │   │   ├── calendar-helpers.ts       # Date calculations
│   │   │   ├── discipline-colors.ts      # Color mapping
│   │   │   └── workout-filters.ts        # Filtering logic
│   │   │
│   │   └── hooks/                        # Calendar hooks
│   │       ├── useCalendarNavigation.ts
│   │       ├── useWorkoutData.ts
│   │       └── useCalendarKeyboard.ts
│   │
│   └── ui/                               # Shared UI components
│       ├── button.tsx                    # Existing
│       ├── card.tsx                      # Existing
│       ├── modal.tsx                     # To create
│       └── skeleton.tsx                  # To create
│
├── types/                                # TypeScript types
│   ├── index.ts                          # Re-exports
│   ├── discipline.ts                     # Discipline enums & colors
│   ├── workout.ts                        # Workout types
│   ├── training-plan.ts                  # Training plan types
│   └── calendar.ts                       # Calendar types
│
├── stores/                               # Zustand stores
│   └── calendar-store.ts                 # Calendar state management
│
├── services/                             # API services
│   └── training-plan-service.ts          # Training plan API calls
│
└── __tests__/                            # Tests
    ├── utils/
    │   └── test-utils.tsx                # Test helpers
    │
    ├── mocks/
    │   └── workout-factory.ts            # Mock data generators
    │
    └── components/
        └── calendar/                     # Mirror component structure
            ├── WorkoutCalendar.test.tsx
            ├── views/
            │   ├── DailyView.test.tsx
            │   ├── WeeklyView.test.tsx
            │   └── MonthlyView.test.tsx
            ├── workout/
            │   ├── WorkoutCard.test.tsx
            │   └── WorkoutStatusBadge.test.tsx
            ├── hooks/
            │   └── useCalendarNavigation.test.ts
            └── utils/
                └── calendar-helpers.test.ts
```

---

## Code Examples

### Basic Usage

```typescript
// pages/calendar/index.tsx
import { WorkoutCalendar } from '@/components/calendar';

export default function CalendarPage() {
  const userId = 'current-user-id'; // From auth context

  return (
    <main className="container mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">Training Calendar</h1>
      <WorkoutCalendar userId={userId} />
    </main>
  );
}
```

### Zustand Store Usage

```typescript
import { useCalendarStore } from '@/stores/calendar-store';

function ViewSwitcher() {
  const { view, setView } = useCalendarStore();

  return (
    <div className="flex gap-2">
      <button onClick={() => setView(CalendarView.Daily)}>
        Daily
      </button>
      <button onClick={() => setView(CalendarView.Weekly)}>
        Weekly
      </button>
      <button onClick={() => setView(CalendarView.Monthly)}>
        Monthly
      </button>
    </div>
  );
}
```

### Custom Hook Usage

```typescript
import { useWorkoutData } from '@/components/calendar/hooks/useWorkoutData';

function WeeklyView() {
  const { workouts, isLoading, error, updateStatus } = useWorkoutData({
    userId: 'user-123',
    startDate: '2025-02-01',
    endDate: '2025-02-07',
  });

  if (isLoading) return <Skeleton />;
  if (error) return <Error message={error} />;

  return (
    <div>
      {workouts.map((workout) => (
        <WorkoutCard
          key={workout.id}
          workout={workout}
          onStatusChange={updateStatus}
        />
      ))}
    </div>
  );
}
```

---

## API Contract

The calendar expects these endpoints to be available:

### GET /api/workouts/range
**Query Parameters:**
- `userId` (string): User ID
- `startDate` (string): ISO 8601 date (YYYY-MM-DD)
- `endDate` (string): ISO 8601 date (YYYY-MM-DD)

**Response:** `Workout[]`

### GET /api/workouts/today
**Query Parameters:**
- `userId` (string): User ID

**Response:** `Workout | null`

### PATCH /api/workouts/:workoutId/status
**Body:**
```json
{
  "status": "Completed" | "Skipped" | "InProgress"
}
```

**Response:** `Workout`

---

## Accessibility Checklist

- [ ] All interactive elements are keyboard accessible
- [ ] Focus indicators visible with ≥3:1 contrast
- [ ] ARIA labels on all status indicators
- [ ] Screen reader announcements for navigation
- [ ] Color is not the only way to convey information
- [ ] Text contrast ≥4.5:1 for normal text
- [ ] Semantic HTML used throughout
- [ ] Skip links for keyboard users
- [ ] Reduced motion support for animations

---

## Testing Checklist

### Unit Tests
- [ ] Date calculation utilities
- [ ] Color mapping functions
- [ ] Workout filtering logic
- [ ] Status badge rendering
- [ ] Discipline icon mapping

### Component Tests
- [ ] WorkoutCard renders correctly
- [ ] Status badges show correct state
- [ ] View switching works
- [ ] Navigation buttons work
- [ ] Keyboard shortcuts trigger actions
- [ ] Loading states display
- [ ] Error states display

### Integration Tests
- [ ] Full workflow: load → view → select → complete workout
- [ ] View switching persists preference
- [ ] Optimistic updates work
- [ ] Error recovery works
- [ ] Keyboard navigation end-to-end

### Accessibility Tests
- [ ] Screen reader compatibility
- [ ] Keyboard-only navigation
- [ ] Focus management
- [ ] ARIA labels present

---

## Performance Targets

- **Initial Load:** < 1 second for calendar skeleton
- **Data Load:** < 500ms for workout data
- **View Switch:** < 100ms transition
- **Status Update:** Optimistic UI update immediate, API call in background
- **Bundle Size:** < 50KB for calendar feature (gzipped)

---

## Common Pitfalls to Avoid

1. **Don't** use moment.js or date-fns - native Date is sufficient
2. **Don't** fetch all workouts upfront - fetch by date range
3. **Don't** ignore keyboard users - test with Tab navigation
4. **Don't** use color alone - always include icons/text
5. **Don't** skip loading states - they improve perceived performance
6. **Don't** mutate state directly - use immutable updates
7. **Don't** forget to cleanup event listeners in useEffect
8. **Don't** skip error boundaries - graceful degradation is key

---

## Next Steps

1. **Review** the ADR and Implementation Guide thoroughly
2. **Set up** the project structure (folders and files)
3. **Start** with Phase 1: Foundation
4. **Follow** the implementation roadmap sequentially
5. **Test** each component as you build it
6. **Document** any deviations or new patterns
7. **Get feedback** early and often

---

## Support & Resources

- **Questions?** Refer to the Implementation Guide for code examples
- **Architectural decisions?** See the ADR for rationale
- **Requirements?** Check Task 011 specification
- **Blocked?** Document blockers and alternatives

---

## Success Criteria

The Workout Calendar component is considered complete when:

- ✅ All three views (Daily, Weekly, Monthly) are functional
- ✅ Users can navigate between dates and views
- ✅ Workouts display with correct discipline colors and icons
- ✅ Status updates work with optimistic UI
- ✅ Keyboard navigation is fully functional
- ✅ Screen readers can navigate and understand the calendar
- ✅ Test coverage is ≥85%
- ✅ WCAG AA accessibility compliance verified
- ✅ Performance targets met
- ✅ Code review approved

---

**Ready to build!** Start with the Foundation phase and work through systematically. 🚀
