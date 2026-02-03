# Progress Tracking UI Components

This directory contains all UI components for tracking workout completion, displaying progress statistics, visualizing streaks, and showing historical completion data.

## Components

### Workout Completion

#### `WorkoutCompletionButton`
Button component for marking workouts as complete.
- Loading states
- Undo functionality
- Keyboard accessible
- ARIA labels

**Usage:**
```tsx
<WorkoutCompletionButton
  isCompleted={workout.status === WorkoutStatus.COMPLETED}
  isLoading={isCompleting}
  onComplete={() => handleComplete(workout.id)}
  onUndo={() => handleUndo(workout.id)}
  showUndo={showUndoOption}
/>
```

#### `CompletionAnimation`
Celebration animation displayed when a workout is marked complete.
- Checkmark animation
- Confetti particles (optional, respects reduced motion)
- Auto-dismisses after 2 seconds

#### `UndoCompletionToast`
Toast notification with undo option shown after completion.
- Auto-hides after 5 seconds
- Dismissible
- Accessible

#### `WorkoutCompletionHandler`
Wrapper component that orchestrates the complete workflow.
- Manages completion state
- Shows animations
- Handles undo logic
- Displays motivational messages
- Checks for milestone achievements

**Usage:**
```tsx
<WorkoutCompletionHandler
  workout={workout}
  onWorkoutUpdated={(updated) => setWorkout(updated)}
/>
```

### Progress Statistics

#### `ProgressDashboard`
Main dashboard combining all progress statistics.
- Weekly summary
- Overall plan progress
- All-time stats
- Streak display

**Usage:**
```tsx
<ProgressDashboard />
```

#### `WeeklySummary`
Display weekly completion statistics.
- Progress bar
- Completion percentage
- Days remaining in week
- On-track indicator

#### `OverallPlanProgress`
Overall plan completion progress.
- Circular progress indicator
- Percentage complete
- Estimated completion date
- Based on current pace

#### `AllTimeStats`
All-time statistics display.
- Total training days
- Total workouts completed
- Average weekly completion rate
- Member since date
- Achievement badges

### Streak Visualization

#### `StreakDisplay`
Current streak display with fire emoji.
- Current streak count
- Personal best streak
- Milestone progress bar
- Motivational messages
- Celebration animations for milestones

**Props:**
- `compact?: boolean` - Compact display mode
- `showMilestoneProgress?: boolean` - Show/hide milestone progress

#### `MilestoneAnimation`
Celebration animation for streak milestones (7, 14, 30, 60, 90, 180, 365 days).
- Trophy icon
- Confetti particles
- Milestone message
- Auto-dismisses after 3 seconds

#### `MotivationalMessage`
Coach Tom avatar with context-aware motivational messages.
- Different messages for different contexts:
  - Completion
  - Streak achievement
  - Week completion
  - Low completion
  - Milestone reached

**Usage:**
```tsx
<MotivationalMessage
  type="streak"
  data={{ streakDays: 14 }}
/>
```

### Historical Progress

#### `HistoricalProgressView`
Complete historical progress view.
- Calendar heatmap
- Discipline filter
- Day detail modal

**Usage:**
```tsx
<HistoricalProgressView />
```

#### `CalendarHeatmap`
GitHub-style calendar heatmap showing completion density.
- Color-coded by workout count
- Month navigation
- Drill-down on click
- Accessible tooltips

#### `HeatmapDayDetail`
Modal showing workouts completed on a specific day.
- Workout list
- Duration and notes
- Discipline indicators

#### `DisciplineFilter`
Toggle filter for viewing by discipline.
- All disciplines
- HYROX
- Running
- Strength

## Hooks

### `useWorkoutCompletion`
Hook for managing workout completion with optimistic updates.

**Returns:**
```typescript
{
  isCompleting: boolean;
  isUndoing: boolean;
  error: Error | null;
  completeWorkout: (workoutId, data?) => Promise<void>;
  undoCompletion: (workoutId) => Promise<void>;
  clearError: () => void;
}
```

### `useProgressStats`
Collection of hooks for fetching progress statistics:
- `useWeeklyStats()` - Weekly statistics
- `useMonthlyStats()` - Monthly statistics
- `useOverallStats()` - All-time statistics
- `useStreakInfo()` - Streak information
- `useCompletionHistory(startDate?, endDate?)` - Historical completion data

All hooks return:
```typescript
{
  data: T | null;
  isLoading: boolean;
  refetch: () => Promise<void>;
}
```

## State Management

### `useProgressStore`
Zustand store for progress data.

**State:**
- Cached statistics (weekly, monthly, overall, streak, history)
- Loading states
- Cache invalidation methods

**Actions:**
- `setWeeklyStats(stats)` - Update weekly stats
- `setMonthlyStats(stats)` - Update monthly stats
- `setOverallStats(stats)` - Update overall stats
- `setStreakInfo(streak)` - Update streak info
- `setCompletionHistory(history)` - Update history
- `invalidateAll()` - Clear all cached data
- Individual invalidation methods for each stat type

## API Service

### `progress-api.ts`
API service for all progress tracking endpoints:
- `completeWorkout(workoutId, data)` - Mark workout complete
- `undoWorkoutCompletion(workoutId)` - Undo completion
- `getWeeklyStats()` - Fetch weekly statistics
- `getMonthlyStats()` - Fetch monthly statistics
- `getOverallStats()` - Fetch overall statistics
- `getStreakInfo()` - Fetch streak information
- `getCompletionHistory(startDate?, endDate?)` - Fetch historical data

## Types

### `progress.ts`
TypeScript types for progress tracking:
- `WorkoutCompletionDto` - Completion request data
- `CompletionStatsDto` - Statistics response
- `OverallStatsDto` - Overall statistics response
- `StreakInfoDto` - Streak information response
- `CompletionHistoryDto` - Historical completion data
- `CompletionHeatmapData` - Grouped heatmap data

## Utilities

### `progress-helpers.ts`
Helper functions for progress tracking:
- `transformToHeatmapData(history)` - Group history by date
- `filterHistoryByDiscipline(history, discipline)` - Filter by discipline
- `getHeatmapIntensityColor(count)` - Get color for heatmap cell
- `generateDateRange(start, end)` - Generate date range
- `getMonthDates(year, month)` - Get dates for a month
- `formatDateForDisplay(dateString)` - Format date for display

## Accessibility

All components follow WCAG AA guidelines:
- Proper ARIA labels and roles
- Keyboard navigation support
- Screen reader announcements
- Color is not the sole indicator of status
- Reduced motion support for animations
- Focus management

## Testing

### Unit Tests
- `progress-helpers.test.ts` - Helper function tests (22 tests, 100% coverage)

### Component Tests
- `workout-completion-button.test.tsx` - Button component tests (12 tests)
- `streak-display.test.tsx` - Streak display tests

### Running Tests
```bash
npm test -- --testPathPatterns=progress
```

### Coverage
```bash
npm test -- --testPathPatterns=progress --coverage
```

## Examples

### Basic Completion Flow
```tsx
import { WorkoutCompletionHandler } from '@/components/progress';

function WorkoutDetailPage({ workout }) {
  const [currentWorkout, setCurrentWorkout] = useState(workout);

  return (
    <div>
      <h1>{workout.name}</h1>
      {/* Workout details */}
      
      <WorkoutCompletionHandler
        workout={currentWorkout}
        onWorkoutUpdated={setCurrentWorkout}
      />
    </div>
  );
}
```

### Progress Dashboard
```tsx
import { ProgressDashboard } from '@/components/progress';

function ProgressPage() {
  return (
    <div className="container mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">Your Progress</h1>
      <ProgressDashboard />
    </div>
  );
}
```

### Historical View
```tsx
import { HistoricalProgressView } from '@/components/progress';

function HistoryPage() {
  return (
    <div className="container mx-auto py-8">
      <h1 className="text-3xl font-bold mb-6">Training History</h1>
      <HistoricalProgressView />
    </div>
  );
}
```

## Performance Optimization

- Components use React.memo where appropriate
- Heavy calculations are memoized (heatmap data, date ranges)
- Loading states prevent unnecessary renders
- Zustand store provides efficient state management
- Optimistic UI updates for instant feedback
- Animations respect `prefers-reduced-motion`

## Browser Support

Compatible with all modern browsers:
- Chrome/Edge 90+
- Firefox 88+
- Safari 14+
- Mobile browsers (iOS Safari 14+, Chrome Mobile)
