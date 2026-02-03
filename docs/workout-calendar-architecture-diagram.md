# Workout Calendar Component - Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│                          WorkoutCalendar.tsx                            │
│                       (Main Orchestrator Component)                     │
│                                                                         │
│  • Manages overall layout                                              │
│  • Coordinates between header, controls, and views                     │
│  • Applies keyboard navigation hook                                    │
│  • Handles workout detail modal state                                  │
└────────────────────────┬────────────────────────────────────────────────┘
                         │
         ┌───────────────┼───────────────┐
         │               │               │
         ▼               ▼               ▼
┌──────────────┐  ┌──────────────┐  ┌─────────────────────┐
│CalendarHeader│  │CalendarControls│ │   View Components   │
│              │  │                │  │                     │
│• View Toggle │  │• Prev/Next     │  │  ┌───────────────┐ │
│• Date Display│  │• Today Button  │  │  │  DailyView    │ │
└──────┬───────┘  └──────┬─────────┘  │  ├───────────────┤ │
       │                 │             │  │  WeeklyView   │ │
       │                 │             │  ├───────────────┤ │
       │                 │             │  │  MonthlyView  │ │
       │                 │             │  └───────┬───────┘ │
       │                 │             └──────────┼─────────┘
       │                 │                        │
       └─────────────────┴────────────────────────┘
                         │
                         │ All use
                         ▼
         ┌───────────────────────────────┐
         │   useCalendarStore (Zustand)  │
         │                               │
         │  State:                       │
         │   • view: CalendarView        │
         │   • selectedDate: Date        │
         │                               │
         │  Actions:                     │
         │   • setView()                 │
         │   • nextPeriod()              │
         │   • previousPeriod()          │
         │   • goToToday()               │
         └───────────────────────────────┘


┌─────────────────────────────────────────────────────────────────────────┐
│                          View Components                                │
└─────────────────────────────────────────────────────────────────────────┘

         DailyView              WeeklyView              MonthlyView
             │                      │                       │
             └──────────────────────┴───────────────────────┘
                                    │
                    All use useWorkoutData hook
                                    │
                                    ▼
                    ┌───────────────────────────────┐
                    │   useWorkoutData Hook         │
                    │                               │
                    │  • Fetches workout data       │
                    │  • Manages loading state      │
                    │  • Handles errors             │
                    │  • Optimistic updates         │
                    │                               │
                    │  Uses TrainingPlanService ────┼───┐
                    └───────────────────────────────┘   │
                                    │                   │
                    Returns workouts to views           │
                                    │                   │
                                    ▼                   │
            ┌───────────────────────────────┐           │
            │   Workout Display Components  │           │
            │                               │           │
            │  ┌─────────────────────────┐  │           │
            │  │   WorkoutCard           │  │           │
            │  │  • Compact variant      │  │           │
            │  │  • Full variant         │  │           │
            │  │  Uses:                  │  │           │
            │  │   - DisciplineIcon ─────┼──┼───┐       │
            │  │   - WorkoutStatusBadge ─┼──┼───┼───┐   │
            │  └─────────────────────────┘  │   │   │   │
            │                               │   │   │   │
            │  ┌─────────────────────────┐  │   │   │   │
            │  │   WorkoutDetail         │  │   │   │   │
            │  │  (Modal)                │  │   │   │   │
            │  │  • Full workout info    │  │   │   │   │
            │  │  • Exercise list        │  │   │   │   │
            │  │  • Status actions       │  │   │   │   │
            │  └─────────────────────────┘  │   │   │   │
            └───────────────────────────────┘   │   │   │
                                                │   │   │
        ┌───────────────────────────────────────┘   │   │
        │                                           │   │
        ▼                                           │   │
┌──────────────────────┐                            │   │
│  DisciplineIcon      │                            │   │
│                      │                            │   │
│  • HYROX → Zap       │◄───────────────────────────┘   │
│  • Running → Foot    │                                │
│  • Strength → Dumb   │                                │
│  • Hybrid → Sparkles │                                │
│  • Rest → Coffee     │                                │
│                      │                                │
│  Uses:               │                                │
│   DISCIPLINE_COLORS  │                                │
│   DISCIPLINE_ICONS   │                                │
└──────────────────────┘                                │
                                                        │
        ┌───────────────────────────────────────────────┘
        │
        ▼
┌──────────────────────┐
│ WorkoutStatusBadge   │
│                      │
│  • NotStarted        │
│  • InProgress        │
│  • Completed         │
│  • Missed            │
│  • Skipped           │
│                      │
│  Uses:               │
│   STATUS_COLORS      │
│   STATUS_ICONS       │
└──────────────────────┘


┌─────────────────────────────────────────────────────────────────────────┐
│                          Data Flow                                      │
└─────────────────────────────────────────────────────────────────────────┘

Component            Hook                Service              Backend API
   │                  │                     │                      │
   │  useWorkoutData  │                     │                      │
   ├─────────────────►│                     │                      │
   │                  │                     │                      │
   │                  │ getWorkoutsByRange  │                      │
   │                  ├────────────────────►│                      │
   │                  │                     │  GET /workouts/range │
   │                  │                     ├─────────────────────►│
   │                  │                     │                      │
   │                  │                     │  ◄─── Workout[] ─────┤
   │                  │  ◄── Workout[] ─────┤                      │
   │  ◄── workouts ───┤                     │                      │
   │                  │                     │                      │
   │                  │                     │                      │
   │ updateStatus()   │                     │                      │
   ├─────────────────►│                     │                      │
   │                  │                     │                      │
   │ (optimistic UI)  │                     │                      │
   ◄──────────────────┤                     │                      │
   │                  │                     │                      │
   │                  │  updateWorkoutStatus│                      │
   │                  ├────────────────────►│ PATCH /workouts/:id  │
   │                  │                     ├─────────────────────►│
   │                  │                     │                      │
   │                  │                     │  ◄─── Workout ───────┤
   │                  │  ◄── Workout ───────┤                      │
   │                  │                     │                      │
   │ (confirmed)      │                     │                      │
   ◄──────────────────┤                     │                      │


┌─────────────────────────────────────────────────────────────────────────┐
│                          Type System                                    │
└─────────────────────────────────────────────────────────────────────────┘

                        types/index.ts
                              │
              ┌───────────────┼───────────────┬──────────────┐
              │               │               │              │
              ▼               ▼               ▼              ▼
        discipline.ts    workout.ts    training-plan.ts  calendar.ts
              │               │               │              │
       ┌──────┴──────┐        │               │              │
       │             │        │               │              │
       ▼             ▼        ▼               ▼              ▼
  Discipline    COLORS    Workout      TrainingPlan    CalendarView
  (enum)       (const)   (interface)   (interface)      (enum)
                         WorkoutStatus  TrainingWeek   CalendarDate
                          (enum)        (interface)    (interface)
                         SessionType
                          (enum)


┌─────────────────────────────────────────────────────────────────────────┐
│                       Utility Functions                                 │
└─────────────────────────────────────────────────────────────────────────┘

calendar-helpers.ts         discipline-colors.ts      workout-filters.ts
      │                            │                         │
      │                            │                         │
  ┌───┴─────┐              ┌───────┴────────┐        ┌──────┴──────┐
  │         │              │                │        │             │
  ▼         ▼              ▼                ▼        ▼             ▼
getWeekDates    getDisciplineColors    filterByDate    filterByStatus
formatDateKey   getDisciplineBadge     filterByDiscipline
isToday         (uses DISCIPLINE_COLORS) groupByDate
isSameDay                                getCompletion%
isPast/Future
getMonthDates


┌─────────────────────────────────────────────────────────────────────────┐
│                       Custom Hooks                                      │
└─────────────────────────────────────────────────────────────────────────┘

useCalendarNavigation          useWorkoutData         useCalendarKeyboard
        │                            │                         │
        │                            │                         │
Uses calendar store          Uses service layer      Listens to window events
Returns navigation           Returns workouts +      Triggers store actions
  functions                    update functions         on key press


┌─────────────────────────────────────────────────────────────────────────┐
│                    Testing Structure                                    │
└─────────────────────────────────────────────────────────────────────────┘

__tests__/
    │
    ├── utils/
    │   └── test-utils.tsx ──────► Render helpers for all tests
    │
    ├── mocks/
    │   └── workout-factory.ts ──► Mock data generators
    │
    └── components/calendar/
            │
            ├── WorkoutCalendar.test.tsx ──► Integration tests
            │
            ├── views/
            │   ├── DailyView.test.tsx
            │   ├── WeeklyView.test.tsx
            │   └── MonthlyView.test.tsx
            │
            ├── workout/
            │   ├── WorkoutCard.test.tsx
            │   └── WorkoutStatusBadge.test.tsx
            │
            ├── hooks/
            │   └── useCalendarNavigation.test.ts
            │
            └── utils/
                └── calendar-helpers.test.ts


┌─────────────────────────────────────────────────────────────────────────┐
│                    Build & Bundle Strategy                              │
└─────────────────────────────────────────────────────────────────────────┘

                      WorkoutCalendar.tsx
                              │
                    (uses dynamic imports)
                              │
              ┌───────────────┼───────────────┐
              │               │               │
              ▼               ▼               ▼
    lazy(() => DailyView)  WeeklyView    MonthlyView
              │               │               │
              │               │               │
              └───────────────┴───────────────┘
                              │
              Only loaded when user switches to that view
                   (Code splitting optimization)


┌─────────────────────────────────────────────────────────────────────────┐
│                    Accessibility Flow                                   │
└─────────────────────────────────────────────────────────────────────────┘

User Action                 System Response              Screen Reader
     │                            │                            │
     │  Press Tab                 │                            │
     ├──────────────────────────► │                            │
     │                            │  Focus workout card        │
     │                            ├───────────────────────────►│
     │                            │                 "Interval Training,
     │                            │                  HYROX workout,
     │                            │                  Status: Scheduled"
     │                            │                            │
     │  Press Enter/Space         │                            │
     ├──────────────────────────► │                            │
     │                            │  Open workout detail       │
     │                            ├───────────────────────────►│
     │                            │               "Dialog opened,
     │                            │                Workout details"
     │                            │                            │
     │  Press Escape              │                            │
     ├──────────────────────────► │                            │
     │                            │  Close modal, restore focus│
     │                            ├───────────────────────────►│
     │                            │               "Dialog closed"


┌─────────────────────────────────────────────────────────────────────────┐
│                    Legend                                               │
└─────────────────────────────────────────────────────────────────────────┘

  ─────►  Data flow / Function call
  ◄─────  Return value
  ───┐    Dependency / Uses
     └──►
  │       Contains / Parent-child relationship
  ▼
```
