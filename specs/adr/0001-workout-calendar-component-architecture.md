# ADR-0001: Workout Calendar Component Architecture

**Status:** Accepted  
**Date:** 2025-02-03  
**Context:** Task 011 - Workout Calendar UI Component  
**Decision Makers:** Architecture Team  

---

## Context and Problem Statement

We need to implement a workout calendar UI component for the fitness app that displays training plans in multiple views (daily, weekly, monthly). The component must support:

- Multi-view display (daily, weekly, monthly)
- Color-coded discipline differentiation (HYROX, Running, Strength, Hybrid, Rest)
- Workout status tracking (completed, scheduled, missed, in-progress)
- Responsive design across mobile, tablet, and desktop
- High accessibility (WCAG AA compliance, keyboard navigation, screen readers)
- Performance optimization for large training plans
- Integration with backend API for training plan data
- Real-time updates when plans change
- ≥85% test coverage

**Key Requirements from PRD & FRD-004:**
- REQ-7: Display today's workout, current week's plan, and upcoming sessions
- REQ-8: Users must mark workouts complete and track progress
- REQ-11: Visually distinguish between three training disciplines

**Technical Constraints:**
- Next.js 16.1.6 with React 19.2.3
- TypeScript strict mode
- Tailwind CSS for styling
- Zustand for state management
- Existing UI component library in `src/components/ui/`
- Component structure pattern: `src/components/[feature]/`

---

## Decision Drivers

1. **User Experience**: Intuitive navigation, clear visual hierarchy, smooth interactions
2. **Performance**: Fast rendering, smooth scrolling, efficient data fetching
3. **Accessibility**: WCAG AA compliance, keyboard navigation, screen reader support
4. **Maintainability**: Clear separation of concerns, testable components
5. **Scalability**: Handle multi-week plans efficiently
6. **Integration**: Seamless backend API integration with offline support
7. **Type Safety**: Leverage TypeScript for compile-time validation

---

## Considered Options

### Option 1: Monolithic Calendar Component
**Description:** Single large component handling all views and logic

**Pros:**
- Simple initial implementation
- Direct state access

**Cons:**
- Hard to test individual views
- Poor separation of concerns
- Difficult to maintain and extend
- Large bundle size

### Option 2: Feature-Based Modular Architecture (SELECTED)
**Description:** Decomposed into specialized sub-components with clear responsibilities

**Pros:**
- Clear separation of concerns
- Highly testable (unit + integration)
- Reusable components
- Better code organization
- Easier collaboration
- Optimized bundle splitting

**Cons:**
- More files to manage
- Requires careful API design between components

### Option 3: View-Based Architecture
**Description:** Separate implementations for each view type

**Pros:**
- View-specific optimizations
- Independent development

**Cons:**
- Code duplication
- Inconsistent behavior
- Hard to maintain shared logic

---

## Decision Outcome

**Chosen Option:** **Option 2 - Feature-Based Modular Architecture**

This architecture provides the best balance of maintainability, testability, and performance while adhering to React and Next.js best practices.

---

## Architecture Design

### 1. Component Structure

```
src/
├── components/
│   ├── calendar/                      # Feature folder
│   │   ├── index.ts                   # Public API exports
│   │   ├── WorkoutCalendar.tsx        # Main orchestrator component
│   │   ├── CalendarHeader.tsx         # Navigation & view switcher
│   │   ├── CalendarControls.tsx       # Date navigation controls
│   │   │
│   │   ├── views/                     # View components
│   │   │   ├── DailyView.tsx
│   │   │   ├── WeeklyView.tsx
│   │   │   └── MonthlyView.tsx
│   │   │
│   │   ├── workout/                   # Workout display components
│   │   │   ├── WorkoutCard.tsx        # Individual workout display
│   │   │   ├── WorkoutDetail.tsx      # Detailed workout modal
│   │   │   ├── WorkoutStatusBadge.tsx # Status indicators
│   │   │   └── DisciplineIcon.tsx     # Discipline visual markers
│   │   │
│   │   ├── utils/                     # Calendar utilities
│   │   │   ├── calendar-helpers.ts    # Date calculations
│   │   │   ├── discipline-colors.ts   # Color mapping
│   │   │   └── workout-filters.ts     # Filtering logic
│   │   │
│   │   └── hooks/                     # Calendar-specific hooks
│   │       ├── useCalendarNavigation.ts
│   │       ├── useWorkoutData.ts
│   │       └── useCalendarKeyboard.ts
│   │
│   └── ui/                            # Shared UI components (existing)
│       ├── button.tsx
│       ├── card.tsx
│       ├── modal.tsx                  # To be created
│       └── skeleton.tsx               # To be created
│
├── types/
│   ├── calendar.ts                    # Calendar-specific types
│   ├── workout.ts                     # Workout types
│   ├── training-plan.ts               # Training plan types
│   └── discipline.ts                  # Discipline enums
│
├── stores/
│   └── calendar-store.ts              # Zustand store for calendar state
│
├── services/
│   ├── training-plan-service.ts       # API integration
│   └── workout-service.ts             # Workout CRUD operations
│
└── __tests__/
    └── components/
        └── calendar/                  # Mirror component structure
            ├── WorkoutCalendar.test.tsx
            ├── views/
            │   ├── DailyView.test.tsx
            │   ├── WeeklyView.test.tsx
            │   └── MonthlyView.test.tsx
            └── hooks/
                └── useCalendarNavigation.test.ts
```

### 2. TypeScript Type System

#### Core Types (`src/types/discipline.ts`)
```typescript
export enum Discipline {
  HYROX = 'HYROX',
  Running = 'Running',
  Strength = 'Strength',
  Hybrid = 'Hybrid',
  Rest = 'Rest',
}

export const DISCIPLINE_COLORS = {
  [Discipline.HYROX]: { primary: 'orange-500', secondary: 'red-500' },
  [Discipline.Running]: { primary: 'blue-500', secondary: 'blue-600' },
  [Discipline.Strength]: { primary: 'purple-500', secondary: 'green-500' },
  [Discipline.Hybrid]: { primary: 'gradient', secondary: 'gradient' },
  [Discipline.Rest]: { primary: 'gray-400', secondary: 'gray-500' },
} as const;
```

#### Workout Types (`src/types/workout.ts`)
```typescript
export enum WorkoutStatus {
  NotStarted = 'NotStarted',
  InProgress = 'InProgress',
  Completed = 'Completed',
  Missed = 'Missed',
  Skipped = 'Skipped',
}

export enum SessionType {
  Intervals = 'Intervals',
  Tempo = 'Tempo',
  RaceSimulation = 'RaceSimulation',
  FullBody = 'FullBody',
  UpperBody = 'UpperBody',
  LowerBody = 'LowerBody',
  Recovery = 'Recovery',
}

export enum IntensityLevel {
  Low = 'Low',
  Moderate = 'Moderate',
  High = 'High',
}

export interface WorkoutExercise {
  exerciseId: string;
  name: string;
  orderInWorkout: number;
  sets?: number;
  reps?: number;
  duration?: number; // seconds
  restPeriod?: number; // seconds
  intensityGuidance?: string;
  notes?: string;
}

export interface Workout {
  id: string;
  weekId: string;
  dayOfWeek: number; // 0 = Sunday, 6 = Saturday
  scheduledDate: string; // ISO 8601
  discipline: Discipline;
  sessionType: SessionType;
  name: string;
  description: string;
  estimatedDuration: number; // minutes
  intensityLevel: IntensityLevel;
  isKeyWorkout: boolean;
  status: WorkoutStatus;
  completedAt?: string | null;
  exercises: WorkoutExercise[];
}
```

#### Training Plan Types (`src/types/training-plan.ts`)
```typescript
export enum PlanStatus {
  Active = 'Active',
  Completed = 'Completed',
  Abandoned = 'Abandoned',
  Paused = 'Paused',
}

export enum Phase {
  Foundation = 'Foundation',
  Build = 'Build',
  Intensity = 'Intensity',
  Peak = 'Peak',
  Taper = 'Taper',
  Recovery = 'Recovery',
}

export interface TrainingWeek {
  id: string;
  planId: string;
  weekNumber: number;
  phase: Phase;
  weeklyVolume: number; // minutes
  intensityLevel: IntensityLevel;
  focusArea: string;
  startDate: string;
  endDate: string;
  workouts: Workout[];
}

export interface TrainingPlan {
  id: string;
  userId: string;
  name: string;
  startDate: string;
  endDate: string;
  totalWeeks: number;
  trainingDaysPerWeek: number;
  primaryGoal: string;
  status: PlanStatus;
  currentWeek: number;
  weeks: TrainingWeek[];
  createdAt: string;
  updatedAt: string;
}
```

#### Calendar Types (`src/types/calendar.ts`)
```typescript
export enum CalendarView {
  Daily = 'Daily',
  Weekly = 'Weekly',
  Monthly = 'Monthly',
}

export interface CalendarDate {
  year: number;
  month: number; // 0-11
  day: number;
}

export interface CalendarState {
  view: CalendarView;
  selectedDate: CalendarDate;
  currentPlan: TrainingPlan | null;
  isLoading: boolean;
  error: string | null;
}

export interface WorkoutsByDate {
  [dateKey: string]: Workout[]; // dateKey format: 'YYYY-MM-DD'
}
```

### 3. State Management Strategy

**Decision:** Use Zustand for global calendar state with local component state

**Rationale:**
- Zustand: Lightweight, minimal boilerplate, excellent TypeScript support
- Local State: View-specific transient state (modals, hover states)
- Separation of client state (view, navigation) from server state (workouts via props/context)

#### Zustand Store (`src/stores/calendar-store.ts`)
```typescript
import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import { CalendarView, CalendarDate } from '@/types/calendar';

interface CalendarStore {
  // State
  view: CalendarView;
  selectedDate: CalendarDate;
  
  // Actions
  setView: (view: CalendarView) => void;
  setSelectedDate: (date: CalendarDate) => void;
  goToToday: () => void;
  nextPeriod: () => void;
  previousPeriod: () => void;
}

const getCurrentDate = (): CalendarDate => {
  const now = new Date();
  return {
    year: now.getFullYear(),
    month: now.getMonth(),
    day: now.getDate(),
  };
};

const calculateNextPeriod = (view: CalendarView, date: CalendarDate): CalendarDate => {
  // Implementation details...
  const current = new Date(date.year, date.month, date.day);
  
  switch (view) {
    case CalendarView.Daily:
      current.setDate(current.getDate() + 1);
      break;
    case CalendarView.Weekly:
      current.setDate(current.getDate() + 7);
      break;
    case CalendarView.Monthly:
      current.setMonth(current.getMonth() + 1);
      break;
  }
  
  return {
    year: current.getFullYear(),
    month: current.getMonth(),
    day: current.getDate(),
  };
};

const calculatePreviousPeriod = (view: CalendarView, date: CalendarDate): CalendarDate => {
  const current = new Date(date.year, date.month, date.day);
  
  switch (view) {
    case CalendarView.Daily:
      current.setDate(current.getDate() - 1);
      break;
    case CalendarView.Weekly:
      current.setDate(current.getDate() - 7);
      break;
    case CalendarView.Monthly:
      current.setMonth(current.getMonth() - 1);
      break;
  }
  
  return {
    year: current.getFullYear(),
    month: current.getMonth(),
    day: current.getDate(),
  };
};

export const useCalendarStore = create<CalendarStore>()(
  persist(
    (set, get) => ({
      // Initial state
      view: CalendarView.Weekly,
      selectedDate: getCurrentDate(),

      // Actions
      setView: (view) => set({ view }),
      
      setSelectedDate: (date) => set({ selectedDate: date }),
      
      goToToday: () => set({ selectedDate: getCurrentDate() }),
      
      nextPeriod: () => {
        const { view, selectedDate } = get();
        set({ selectedDate: calculateNextPeriod(view, selectedDate) });
      },
      
      previousPeriod: () => {
        const { view, selectedDate } = get();
        set({ selectedDate: calculatePreviousPeriod(view, selectedDate) });
      },
    }),
    {
      name: 'calendar-preferences',
      partialize: (state) => ({ view: state.view }), // Only persist view preference
    }
  )
);
```

### 4. API Integration Pattern

**Decision:** Use Service Layer with async/await pattern

**Pattern:**
1. Service layer encapsulates API calls
2. Components fetch data in useEffect or event handlers
3. Loading/error states managed locally
4. Optional: Add React Query later for caching

#### Service Layer (`src/services/training-plan-service.ts`)
```typescript
import axios from 'axios';
import { TrainingPlan, Workout, WorkoutStatus } from '@/types';

const API_BASE = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

export class TrainingPlanService {
  /**
   * Get the active training plan for a user
   */
  static async getActivePlan(userId: string): Promise<TrainingPlan> {
    const response = await axios.get(`${API_BASE}/training-plans/active`, {
      params: { userId },
    });
    return response.data;
  }

  /**
   * Get workouts within a date range
   */
  static async getWorkoutsByDateRange(
    userId: string,
    startDate: string,
    endDate: string
  ): Promise<Workout[]> {
    const response = await axios.get(`${API_BASE}/workouts/range`, {
      params: { userId, startDate, endDate },
    });
    return response.data;
  }

  /**
   * Get today's workout
   */
  static async getTodaysWorkout(userId: string): Promise<Workout | null> {
    const response = await axios.get(`${API_BASE}/workouts/today`, {
      params: { userId },
    });
    return response.data;
  }

  /**
   * Update workout status (complete, skip, etc.)
   */
  static async updateWorkoutStatus(
    workoutId: string,
    status: WorkoutStatus
  ): Promise<Workout> {
    const response = await axios.patch(
      `${API_BASE}/workouts/${workoutId}/status`,
      { status }
    );
    return response.data;
  }

  /**
   * Get workout details by ID
   */
  static async getWorkoutById(workoutId: string): Promise<Workout> {
    const response = await axios.get(`${API_BASE}/workouts/${workoutId}`);
    return response.data;
  }
}
```

#### Custom Hook for Data Fetching (`src/components/calendar/hooks/useWorkoutData.ts`)
```typescript
import { useState, useEffect } from 'react';
import { TrainingPlanService } from '@/services/training-plan-service';
import { Workout, WorkoutStatus } from '@/types/workout';

interface UseWorkoutDataResult {
  workouts: Workout[];
  isLoading: boolean;
  error: string | null;
  refetch: () => Promise<void>;
  updateStatus: (workoutId: string, status: WorkoutStatus) => Promise<void>;
}

export function useWorkoutData(
  userId: string,
  startDate: string,
  endDate: string
): UseWorkoutDataResult {
  const [workouts, setWorkouts] = useState<Workout[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<string | null>(null);

  const fetchWorkouts = async () => {
    try {
      setIsLoading(true);
      setError(null);
      const data = await TrainingPlanService.getWorkoutsByDateRange(
        userId,
        startDate,
        endDate
      );
      setWorkouts(data);
    } catch (err) {
      setError(err instanceof Error ? err.message : 'Failed to load workouts');
    } finally {
      setIsLoading(false);
    }
  };

  useEffect(() => {
    fetchWorkouts();
  }, [userId, startDate, endDate]);

  const updateStatus = async (workoutId: string, status: WorkoutStatus) => {
    // Optimistic update
    setWorkouts((prev) =>
      prev.map((w) => (w.id === workoutId ? { ...w, status } : w))
    );

    try {
      await TrainingPlanService.updateWorkoutStatus(workoutId, status);
    } catch (err) {
      // Rollback on error
      await fetchWorkouts();
      throw err;
    }
  };

  return {
    workouts,
    isLoading,
    error,
    refetch: fetchWorkouts,
    updateStatus,
  };
}
```

### 5. Accessibility Implementation

**WCAG AA Compliance Requirements:**
- Color contrast ratio ≥4.5:1 for normal text
- Color contrast ratio ≥3:1 for large text and UI components
- Keyboard navigation for all interactive elements
- Screen reader announcements for state changes
- Focus indicators for all focusable elements

#### Keyboard Navigation (`src/components/calendar/hooks/useCalendarKeyboard.ts`)
```typescript
import { useEffect } from 'react';
import { useCalendarStore } from '@/stores/calendar-store';
import { CalendarView } from '@/types/calendar';

export function useCalendarKeyboard() {
  const { nextPeriod, previousPeriod, goToToday, setView } = useCalendarStore();

  useEffect(() => {
    const handleKeyDown = (e: KeyboardEvent) => {
      // Ignore if user is typing in an input
      if (
        e.target instanceof HTMLInputElement ||
        e.target instanceof HTMLTextAreaElement
      ) {
        return;
      }

      // Arrow navigation
      if (e.key === 'ArrowLeft' && !e.metaKey && !e.ctrlKey) {
        e.preventDefault();
        previousPeriod();
      } else if (e.key === 'ArrowRight' && !e.metaKey && !e.ctrlKey) {
        e.preventDefault();
        nextPeriod();
      }
      
      // Jump to today
      else if (e.key === 't' && !e.ctrlKey && !e.metaKey) {
        e.preventDefault();
        goToToday();
      }
      
      // View switching with keyboard shortcuts
      else if (e.key === 'd' && !e.ctrlKey && !e.metaKey) {
        e.preventDefault();
        setView(CalendarView.Daily);
      } else if (e.key === 'w' && !e.ctrlKey && !e.metaKey) {
        e.preventDefault();
        setView(CalendarView.Weekly);
      } else if (e.key === 'm' && !e.ctrlKey && !e.metaKey) {
        e.preventDefault();
        setView(CalendarView.Monthly);
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [nextPeriod, previousPeriod, goToToday, setView]);
}
```

#### ARIA Labels & Semantic HTML Example
```typescript
// WorkoutCard.tsx
<article
  role="article"
  aria-label={`${workout.name} - ${workout.discipline} workout`}
  aria-describedby={`workout-${workout.id}-description`}
  tabIndex={0}
  className="focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"
  onKeyDown={(e) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      onWorkoutClick(workout);
    }
  }}
>
  <WorkoutStatusBadge 
    status={workout.status} 
    aria-label={`Status: ${workout.status}`}
  />
  <h3 className="text-lg font-semibold">{workout.name}</h3>
  <p id={`workout-${workout.id}-description`} className="text-sm text-gray-600">
    {workout.description}
  </p>
</article>
```

#### Color Contrast Utilities (`src/components/calendar/utils/discipline-colors.ts`)
```typescript
import { Discipline } from '@/types/discipline';

// WCAG AA compliant color combinations
export const DISCIPLINE_COLORS = {
  [Discipline.HYROX]: {
    bg: 'bg-orange-100',
    border: 'border-orange-500',
    text: 'text-orange-900',
    icon: 'text-orange-600',
    hover: 'hover:bg-orange-200',
  },
  [Discipline.Running]: {
    bg: 'bg-blue-100',
    border: 'border-blue-500',
    text: 'text-blue-900',
    icon: 'text-blue-600',
    hover: 'hover:bg-blue-200',
  },
  [Discipline.Strength]: {
    bg: 'bg-purple-100',
    border: 'border-purple-500',
    text: 'text-purple-900',
    icon: 'text-purple-600',
    hover: 'hover:bg-purple-200',
  },
  [Discipline.Hybrid]: {
    bg: 'bg-gradient-to-r from-orange-100 to-blue-100',
    border: 'border-purple-500',
    text: 'text-gray-900',
    icon: 'text-purple-600',
    hover: 'hover:opacity-80',
  },
  [Discipline.Rest]: {
    bg: 'bg-gray-100',
    border: 'border-gray-400',
    text: 'text-gray-700',
    icon: 'text-gray-500',
    hover: 'hover:bg-gray-200',
  },
} as const;

export function getDisciplineColors(discipline: Discipline) {
  return DISCIPLINE_COLORS[discipline];
}

// Icons for colorblind-friendly differentiation
export const DISCIPLINE_ICONS = {
  [Discipline.HYROX]: 'Zap', // lucide-react
  [Discipline.Running]: 'Footprints',
  [Discipline.Strength]: 'Dumbbell',
  [Discipline.Hybrid]: 'Sparkles',
  [Discipline.Rest]: 'Coffee',
} as const;
```

### 6. Performance Optimization

#### Strategies:
1. **Code Splitting:** Lazy load view components
2. **Memoization:** React.memo for expensive components
3. **Debouncing:** Navigation actions
4. **Efficient Rendering:** Only re-render changed parts
5. **Image Optimization:** Use Next.js Image component

#### Example: Lazy Loading Views
```typescript
// WorkoutCalendar.tsx
'use client';

import { Suspense, lazy } from 'react';
import { useCalendarStore } from '@/stores/calendar-store';
import { CalendarView } from '@/types/calendar';
import { CalendarSkeleton } from './CalendarSkeleton';

// Lazy load view components
const DailyView = lazy(() => import('./views/DailyView'));
const WeeklyView = lazy(() => import('./views/WeeklyView'));
const MonthlyView = lazy(() => import('./views/MonthlyView'));

export function WorkoutCalendar() {
  const { view } = useCalendarStore();

  const renderView = () => {
    switch (view) {
      case CalendarView.Daily:
        return <DailyView />;
      case CalendarView.Weekly:
        return <WeeklyView />;
      case CalendarView.Monthly:
        return <MonthlyView />;
    }
  };

  return (
    <div className="workout-calendar">
      <Suspense fallback={<CalendarSkeleton />}>
        {renderView()}
      </Suspense>
    </div>
  );
}
```

#### Example: Memoized Workout Card
```typescript
import { memo } from 'react';
import { Workout } from '@/types/workout';

interface WorkoutCardProps {
  workout: Workout;
  onStatusChange: (id: string, status: WorkoutStatus) => void;
  onClick: (workout: Workout) => void;
}

export const WorkoutCard = memo<WorkoutCardProps>(
  ({ workout, onStatusChange, onClick }) => {
    // Component implementation
    return (
      <article className="workout-card">
        {/* ... */}
      </article>
    );
  },
  (prevProps, nextProps) => {
    // Custom equality check - only re-render if these change
    return (
      prevProps.workout.id === nextProps.workout.id &&
      prevProps.workout.status === nextProps.workout.status &&
      prevProps.workout.scheduledDate === nextProps.workout.scheduledDate
    );
  }
);

WorkoutCard.displayName = 'WorkoutCard';
```

### 7. Testing Strategy

#### Test Pyramid:
- **Unit Tests (60%):** Utils, hooks, helpers
- **Component Tests (30%):** Individual component behavior
- **Integration Tests (10%):** Full feature workflows

#### Coverage Requirements:
- Overall: ≥85%
- Critical paths: 100%
- Edge cases: Comprehensive

#### Testing Utilities Setup (`src/__tests__/utils/test-utils.tsx`)
```typescript
import { ReactElement, ReactNode } from 'react';
import { render, RenderOptions } from '@testing-library/react';

interface AllProvidersProps {
  children: ReactNode;
}

function AllProviders({ children }: AllProvidersProps) {
  return <>{children}</>;
}

export function renderWithProviders(
  ui: ReactElement,
  options?: Omit<RenderOptions, 'wrapper'>
) {
  return render(ui, { wrapper: AllProviders, ...options });
}

export * from '@testing-library/react';
export { renderWithProviders as render };
```

#### Mock Data Factory (`src/__tests__/mocks/workout-factory.ts`)
```typescript
import { Workout, WorkoutStatus, Discipline, SessionType, IntensityLevel } from '@/types';

export function createMockWorkout(overrides?: Partial<Workout>): Workout {
  return {
    id: 'workout-1',
    weekId: 'week-1',
    dayOfWeek: 1,
    scheduledDate: '2025-02-03',
    discipline: Discipline.Running,
    sessionType: SessionType.Intervals,
    name: 'Interval Training - 8x400m',
    description: 'Speed work session',
    estimatedDuration: 45,
    intensityLevel: IntensityLevel.High,
    isKeyWorkout: false,
    status: WorkoutStatus.NotStarted,
    exercises: [],
    ...overrides,
  };
}
```

#### Example Unit Test
```typescript
// __tests__/components/calendar/utils/calendar-helpers.test.ts
import { 
  getWeekDates, 
  formatDateKey, 
  isToday,
  isSameDay 
} from '@/components/calendar/utils/calendar-helpers';

describe('calendar-helpers', () => {
  describe('getWeekDates', () => {
    it('returns 7 dates starting from Sunday', () => {
      const date = new Date('2025-02-03'); // Monday
      const dates = getWeekDates(date);
      
      expect(dates).toHaveLength(7);
      expect(dates[0].getDay()).toBe(0); // Sunday
      expect(dates[6].getDay()).toBe(6); // Saturday
    });

    it('handles week boundaries correctly', () => {
      const date = new Date('2025-02-01'); // Saturday
      const dates = getWeekDates(date);
      
      expect(dates[0]).toEqual(new Date('2025-01-26')); // Previous Sunday
      expect(dates[6]).toEqual(new Date('2025-02-01')); // This Saturday
    });
  });

  describe('formatDateKey', () => {
    it('formats date as YYYY-MM-DD', () => {
      const date = new Date('2025-02-03');
      expect(formatDateKey(date)).toBe('2025-02-03');
    });

    it('pads single-digit months and days', () => {
      const date = new Date('2025-01-05');
      expect(formatDateKey(date)).toBe('2025-01-05');
    });
  });

  describe('isToday', () => {
    it('returns true for today\'s date', () => {
      const today = new Date();
      expect(isToday(today)).toBe(true);
    });

    it('returns false for yesterday', () => {
      const yesterday = new Date();
      yesterday.setDate(yesterday.getDate() - 1);
      expect(isToday(yesterday)).toBe(false);
    });
  });
});
```

#### Example Component Test
```typescript
// __tests__/components/calendar/views/WeeklyView.test.tsx
import { screen } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '@/__tests__/utils/test-utils';
import { WeeklyView } from '@/components/calendar/views/WeeklyView';
import { createMockWorkout } from '@/__tests__/mocks/workout-factory';

describe('WeeklyView', () => {
  it('renders 7 days of the week', () => {
    render(<WeeklyView workouts={[]} />);
    
    expect(screen.getByText(/sun/i)).toBeInTheDocument();
    expect(screen.getByText(/mon/i)).toBeInTheDocument();
    expect(screen.getByText(/tue/i)).toBeInTheDocument();
    expect(screen.getByText(/wed/i)).toBeInTheDocument();
    expect(screen.getByText(/thu/i)).toBeInTheDocument();
    expect(screen.getByText(/fri/i)).toBeInTheDocument();
    expect(screen.getByText(/sat/i)).toBeInTheDocument();
  });

  it('highlights today with distinct styling', () => {
    render(<WeeklyView workouts={[]} />);
    
    const today = screen.getByTestId('today-cell');
    expect(today).toHaveClass('border-blue-500');
  });

  it('displays workouts on correct days', () => {
    const workout = createMockWorkout({
      scheduledDate: '2025-02-03',
      name: 'Morning Run',
    });

    render(<WeeklyView workouts={[workout]} />);
    
    expect(screen.getByText('Morning Run')).toBeInTheDocument();
  });

  it('shows rest days as empty', () => {
    render(<WeeklyView workouts={[]} />);
    
    const restDays = screen.getAllByText(/rest/i);
    expect(restDays.length).toBeGreaterThan(0);
  });

  it('opens workout detail on click', async () => {
    const user = userEvent.setup();
    const workout = createMockWorkout({ name: 'Speed Intervals' });
    const onWorkoutClick = jest.fn();

    render(<WeeklyView workouts={[workout]} onWorkoutClick={onWorkoutClick} />);
    
    await user.click(screen.getByText('Speed Intervals'));
    
    expect(onWorkoutClick).toHaveBeenCalledWith(workout);
  });
});
```

#### Example Integration Test
```typescript
// __tests__/components/calendar/WorkoutCalendar.integration.test.tsx
import { screen, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { render } from '@/__tests__/utils/test-utils';
import { WorkoutCalendar } from '@/components/calendar/WorkoutCalendar';
import { TrainingPlanService } from '@/services/training-plan-service';
import { createMockWorkout } from '@/__tests__/mocks/workout-factory';
import { WorkoutStatus } from '@/types/workout';

// Mock the service
jest.mock('@/services/training-plan-service');

describe('WorkoutCalendar Integration', () => {
  beforeEach(() => {
    jest.clearAllMocks();
  });

  it('loads and displays workouts from API', async () => {
    const mockWorkouts = [
      createMockWorkout({ name: 'Interval Training' }),
      createMockWorkout({ name: 'Strength Session' }),
    ];

    (TrainingPlanService.getWorkoutsByDateRange as jest.Mock).mockResolvedValue(
      mockWorkouts
    );

    render(<WorkoutCalendar userId="test-user" />);

    // Wait for workouts to load
    await waitFor(() => {
      expect(screen.getByText('Interval Training')).toBeInTheDocument();
      expect(screen.getByText('Strength Session')).toBeInTheDocument();
    });
  });

  it('completes full workout selection and status update flow', async () => {
    const user = userEvent.setup();
    const workout = createMockWorkout({ 
      name: 'Morning Run',
      status: WorkoutStatus.NotStarted,
    });

    (TrainingPlanService.getWorkoutsByDateRange as jest.Mock).mockResolvedValue([
      workout,
    ]);

    (TrainingPlanService.updateWorkoutStatus as jest.Mock).mockResolvedValue({
      ...workout,
      status: WorkoutStatus.Completed,
    });

    render(<WorkoutCalendar userId="test-user" />);

    // Wait for workout to load
    await waitFor(() => {
      expect(screen.getByText('Morning Run')).toBeInTheDocument();
    });

    // Click workout to open detail
    await user.click(screen.getByText('Morning Run'));

    // Verify modal opens
    expect(screen.getByRole('dialog')).toBeInTheDocument();

    // Mark as complete
    await user.click(screen.getByRole('button', { name: /complete workout/i }));

    // Verify status updated
    await waitFor(() => {
      expect(TrainingPlanService.updateWorkoutStatus).toHaveBeenCalledWith(
        workout.id,
        WorkoutStatus.Completed
      );
    });
  });

  it('switches between views correctly', async () => {
    const user = userEvent.setup();
    
    (TrainingPlanService.getWorkoutsByDateRange as jest.Mock).mockResolvedValue([]);

    render(<WorkoutCalendar userId="test-user" />);

    // Default is weekly view
    expect(screen.getByText(/week/i)).toBeInTheDocument();

    // Switch to monthly
    await user.click(screen.getByRole('button', { name: /monthly/i }));
    expect(screen.getByText(/month/i)).toBeInTheDocument();

    // Switch to daily
    await user.click(screen.getByRole('button', { name: /daily/i }));
    expect(screen.getByText(/today/i)).toBeInTheDocument();
  });

  it('handles navigation between periods', async () => {
    const user = userEvent.setup();
    
    (TrainingPlanService.getWorkoutsByDateRange as jest.Mock).mockResolvedValue([]);

    render(<WorkoutCalendar userId="test-user" />);

    const nextButton = screen.getByRole('button', { name: /next/i });
    const prevButton = screen.getByRole('button', { name: /previous/i });

    // Navigate forward
    await user.click(nextButton);
    
    await waitFor(() => {
      expect(TrainingPlanService.getWorkoutsByDateRange).toHaveBeenCalledTimes(2);
    });

    // Navigate backward
    await user.click(prevButton);
    
    await waitFor(() => {
      expect(TrainingPlanService.getWorkoutsByDateRange).toHaveBeenCalledTimes(3);
    });
  });
});
```

---

## Consequences

### Positive
- ✅ **Maintainability:** Clear component boundaries make code easy to understand and modify
- ✅ **Testability:** Isolated components enable comprehensive unit and integration testing
- ✅ **Performance:** Code splitting and memoization optimize bundle size and rendering
- ✅ **Accessibility:** Structured approach ensures WCAG AA compliance from the start
- ✅ **Type Safety:** Strong TypeScript types catch errors at compile time
- ✅ **Scalability:** Modular architecture supports future enhancements
- ✅ **Developer Experience:** Clear patterns make onboarding easier

### Negative
- ⚠️ **Initial Complexity:** More files and abstractions vs. monolithic approach
- ⚠️ **Learning Curve:** Team needs familiarity with Zustand patterns
- ⚠️ **Coordination Overhead:** Multiple components require careful API design

### Mitigation Strategies
- Comprehensive documentation and code examples (this ADR)
- Clear file naming conventions and folder structure
- Code review checklist focusing on architecture consistency
- Pair programming for complex integrations

---

## Implementation Checklist

### Phase 1: Foundation (Week 1)
- [ ] Create type definitions (discipline, workout, training-plan, calendar)
- [ ] Set up Zustand store for calendar state
- [ ] Implement calendar utility functions
- [ ] Create base UI components (Modal, Skeleton)
- [ ] Set up test utilities and mocks

### Phase 2: Core Components (Week 2)
- [ ] Build WorkoutCard component
- [ ] Build DisciplineIcon component
- [ ] Build WorkoutStatusBadge component
- [ ] Implement WeeklyView
- [ ] Write unit tests for components

### Phase 3: Views & Navigation (Week 3)
- [ ] Implement DailyView
- [ ] Implement MonthlyView
- [ ] Build CalendarHeader with view switcher
- [ ] Build CalendarControls for navigation
- [ ] Implement keyboard navigation hook

### Phase 4: Integration & Details (Week 4)
- [ ] Create TrainingPlanService
- [ ] Build useWorkoutData hook
- [ ] Implement WorkoutDetail modal
- [ ] Connect components to API
- [ ] Add loading and error states

### Phase 5: Polish & Testing (Week 5)
- [ ] Performance optimization (memoization, lazy loading)
- [ ] Accessibility audit (WCAG AA compliance)
- [ ] Write integration tests
- [ ] Visual regression tests
- [ ] Documentation and examples

---

## Related Decisions

- **ADR-0002:** API Integration Patterns & Error Handling (TBD)
- **ADR-0003:** Testing Strategy & Coverage Requirements (TBD)
- **ADR-0004:** Design System & Styling Conventions (TBD)

---

## References

- [Task 011: Workout Calendar UI Component](/specs/tasks/011-task-workout-calendar-ui.md)
- [Task 008: Training Plan Data Model](/specs/tasks/008-task-training-plan-data-model.md)
- [PRD: Product Requirements Document](/specs/prd.md)
- [React 19 Documentation](https://react.dev/)
- [Next.js 16 Documentation](https://nextjs.org/docs)
- [Zustand Documentation](https://zustand-demo.pmnd.rs/)
- [WCAG 2.1 Guidelines](https://www.w3.org/WAI/WCAG21/quickref/)
- [TypeScript Best Practices](https://www.typescriptlang.org/docs/handbook/declaration-files/do-s-and-don-ts.html)
