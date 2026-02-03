# Workout Calendar - Quick Reference

**Component:** Workout Calendar UI  
**Status:** Ready for Implementation  
**Coverage Required:** ≥85%  

---

## 📁 Folder Structure

```
src/
├── components/calendar/        # 🎯 Main feature folder
│   ├── views/                  # Daily, Weekly, Monthly
│   ├── workout/                # WorkoutCard, Detail, Badge, Icon
│   ├── utils/                  # Helpers & filters
│   └── hooks/                  # Custom hooks
├── types/                      # TypeScript definitions
├── stores/calendar-store.ts    # Zustand state
└── services/training-plan-service.ts  # API calls
```

---

## 🔑 Key Components

| Component | Purpose | Props |
|-----------|---------|-------|
| `WorkoutCalendar` | Main orchestrator | `userId` |
| `DailyView` | Today's workout | `workouts[]` |
| `WeeklyView` | 7-day view | `workouts[]` |
| `MonthlyView` | Month grid | `workouts[]` |
| `WorkoutCard` | Individual workout | `workout`, `onClick` |
| `WorkoutDetail` | Modal with full details | `workout`, `onClose` |

---

## 📊 State Management

### Zustand Store
```typescript
const { view, selectedDate, setView, nextPeriod, goToToday } 
  = useCalendarStore();
```

**State:**
- `view: CalendarView` - Daily/Weekly/Monthly
- `selectedDate: CalendarDate` - Currently displayed date

**Actions:**
- `setView(view)` - Switch view mode
- `nextPeriod()` - Navigate forward
- `previousPeriod()` - Navigate backward
- `goToToday()` - Jump to today

---

## 🎨 Discipline Colors

| Discipline | Colors | Icon |
|------------|--------|------|
| HYROX | Orange/Red | ⚡ Zap |
| Running | Blue | 👣 Footprints |
| Strength | Purple/Green | 🏋️ Dumbbell |
| Hybrid | Gradient | ✨ Sparkles |
| Rest | Gray | ☕ Coffee |

**Usage:**
```typescript
import { getDisciplineColors } from './utils/discipline-colors';
const colors = getDisciplineColors(Discipline.HYROX);
```

---

## 🔄 Workout Status

| Status | Color | Icon | Meaning |
|--------|-------|------|---------|
| NotStarted | Gray | 📅 Calendar | Scheduled |
| InProgress | Yellow | ▶️ PlayCircle | Currently doing |
| Completed | Green | ✅ CheckCircle | Done |
| Missed | Red | ❌ XCircle | Didn't do |
| Skipped | Gray | ⏭️ SkipForward | Intentionally skipped |

---

## 🪝 Custom Hooks

### useWorkoutData
```typescript
const { workouts, isLoading, error, updateStatus } = useWorkoutData({
  userId: 'user-123',
  startDate: '2025-02-01',
  endDate: '2025-02-07',
});
```

### useCalendarNavigation
```typescript
const { view, navigateNext, navigatePrevious, navigateToToday, switchView } 
  = useCalendarNavigation();
```

### useCalendarKeyboard
```typescript
useCalendarKeyboard(); // Enables keyboard shortcuts
```

---

## ⌨️ Keyboard Shortcuts

| Key | Action |
|-----|--------|
| `←` | Previous period |
| `→` | Next period |
| `t` | Jump to today |
| `d` | Switch to Daily view |
| `w` | Switch to Weekly view |
| `m` | Switch to Monthly view |
| `Enter/Space` | Select focused workout |
| `Esc` | Close modal |
| `Tab` | Navigate between elements |

---

## 🌐 API Endpoints

### GET /api/workouts/range
```typescript
?userId=xxx&startDate=2025-02-01&endDate=2025-02-07
→ Workout[]
```

### GET /api/workouts/today
```typescript
?userId=xxx
→ Workout | null
```

### PATCH /api/workouts/:id/status
```typescript
Body: { status: 'Completed' }
→ Workout
```

---

## 🧪 Testing Patterns

### Component Test
```typescript
it('renders workout card', () => {
  const workout = createMockWorkout();
  render(<WorkoutCard workout={workout} />);
  expect(screen.getByText(workout.name)).toBeInTheDocument();
});
```

### Hook Test
```typescript
it('navigates to next period', () => {
  const { result } = renderHook(() => useCalendarNavigation());
  act(() => result.current.navigateNext());
  expect(result.current.selectedDate.day).toBe(expectedDay);
});
```

### Integration Test
```typescript
it('completes workout workflow', async () => {
  const user = userEvent.setup();
  render(<WorkoutCalendar userId="test" />);
  
  await user.click(screen.getByText('Morning Run'));
  await user.click(screen.getByRole('button', { name: /complete/i }));
  
  expect(screen.getByText(/completed/i)).toBeInTheDocument();
});
```

---

## 📦 Type Definitions

### Workout
```typescript
interface Workout {
  id: string;
  scheduledDate: string;  // 'YYYY-MM-DD'
  discipline: Discipline;
  name: string;
  description: string;
  estimatedDuration: number;  // minutes
  status: WorkoutStatus;
  exercises: WorkoutExercise[];
}
```

### CalendarView
```typescript
enum CalendarView {
  Daily = 'Daily',
  Weekly = 'Weekly',
  Monthly = 'Monthly',
}
```

---

## 🎯 Implementation Checklist

### Phase 1: Foundation
- [ ] Create type definitions
- [ ] Set up Zustand store
- [ ] Implement calendar helpers
- [ ] Create test utilities

### Phase 2: Components
- [ ] DisciplineIcon
- [ ] WorkoutStatusBadge
- [ ] WorkoutCard
- [ ] CalendarSkeleton

### Phase 3: Views
- [ ] DailyView
- [ ] WeeklyView
- [ ] MonthlyView
- [ ] CalendarHeader
- [ ] CalendarControls

### Phase 4: Integration
- [ ] TrainingPlanService
- [ ] useWorkoutData hook
- [ ] WorkoutDetail modal
- [ ] Main WorkoutCalendar

### Phase 5: Polish
- [ ] Performance optimization
- [ ] Accessibility audit
- [ ] Integration tests
- [ ] Documentation

---

## 🚨 Common Pitfalls

❌ **Don't:**
- Use moment.js (use native Date)
- Fetch all workouts at once
- Ignore keyboard navigation
- Use only color for status
- Skip loading states
- Mutate state directly

✅ **Do:**
- Fetch by date range
- Test with keyboard only
- Use icons + color
- Show loading skeletons
- Use immutable updates
- Add error boundaries

---

## 📚 Key Documents

1. **[Architecture Summary](./workout-calendar-architecture-summary.md)** - Start here
2. **[Implementation Guide](./workout-calendar-implementation-guide.md)** - Code examples
3. **[Architecture Diagram](./workout-calendar-architecture-diagram.md)** - Visual reference
4. **[ADR-0001](../specs/adr/0001-workout-calendar-component-architecture.md)** - Decisions

---

## ♿ Accessibility Checklist

- [ ] Keyboard navigation works
- [ ] Focus indicators visible
- [ ] ARIA labels present
- [ ] Screen reader tested
- [ ] Color contrast ≥4.5:1
- [ ] Semantic HTML used
- [ ] Reduced motion support

---

## 🎨 Tailwind Utility Classes

```typescript
// Focus indicator
className="focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2"

// Card hover
className="hover:shadow-md transition-shadow"

// Responsive grid
className="grid grid-cols-1 md:grid-cols-7 gap-4"

// Status badge
className="inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs"
```

---

## 🔍 Debugging Tips

**State not updating?**
- Check Zustand DevTools
- Verify immutable updates

**Workouts not loading?**
- Check Network tab for API calls
- Verify date format (YYYY-MM-DD)
- Check console for errors

**Keyboard shortcuts not working?**
- Ensure useCalendarKeyboard is called
- Check if input has focus
- Verify event.preventDefault()

**Tests failing?**
- Use `renderWithProviders` not `render`
- Mock TrainingPlanService
- Use `waitFor` for async updates

---

## 📞 Support

- Architecture questions → See ADR-0001
- Code examples → Implementation Guide
- Visual reference → Architecture Diagram
- Quick overview → Architecture Summary

---

**Version:** 1.0  
**Last Updated:** 2025-02-03
