# Getting Started with Workout Calendar Implementation

**Welcome!** This guide will help you get started implementing the Workout Calendar component.

---

## 📚 Before You Start

### Required Reading (30 minutes)

Read these documents in order:

1. **[Architecture Summary](./workout-calendar-architecture-summary.md)** (15 min)
   - Understand the big picture
   - Review key decisions
   - See the roadmap

2. **[Quick Reference](./workout-calendar-quick-reference.md)** (10 min)
   - Familiarize yourself with key concepts
   - Bookmark for easy access during coding

3. **Skim [Implementation Guide](./workout-calendar-implementation-guide.md)** (5 min)
   - Know what's available
   - Come back when coding

---

## 🛠️ Setup (15 minutes)

### 1. Create Directory Structure

```bash
cd /home/runner/work/fitness-app/fitness-app/frontend/src

# Create component directories
mkdir -p components/calendar/views
mkdir -p components/calendar/workout
mkdir -p components/calendar/utils
mkdir -p components/calendar/hooks

# Create type directories
mkdir -p types

# Create stores directory
mkdir -p stores

# Create test directories
mkdir -p __tests__/components/calendar/views
mkdir -p __tests__/components/calendar/workout
mkdir -p __tests__/components/calendar/hooks
mkdir -p __tests__/components/calendar/utils
mkdir -p __tests__/mocks
mkdir -p __tests__/utils
```

### 2. Install Dependencies (if needed)

All dependencies should already be in `package.json`. Verify:

```bash
cd /home/runner/work/fitness-app/fitness-app/frontend
npm list zustand lucide-react
```

If missing, install:

```bash
npm install zustand lucide-react
```

### 3. Verify Existing Components

Check that you have the base UI components:

```bash
ls src/components/ui/
# Should see: button.tsx, card.tsx, input.tsx, label.tsx, select.tsx, textarea.tsx
```

---

## 🚀 Day 1: Types and Store (2-3 hours)

### Step 1: Create Discipline Types

**File:** `src/types/discipline.ts`

Copy the complete implementation from the **[Implementation Guide - Section: Type Definitions - Discipline Types](./workout-calendar-implementation-guide.md#1-discipline-types-srctypesdisciplinets)**

Test it:
```bash
npm run type-check
```

### Step 2: Create Workout Types

**File:** `src/types/workout.ts`

Copy from **[Implementation Guide - Section: Type Definitions - Workout Types](./workout-calendar-implementation-guide.md#2-workout-types-srctypesworkoutts)**

Test it:
```bash
npm run type-check
```

### Step 3: Create Training Plan Types

**File:** `src/types/training-plan.ts`

Copy from **[Implementation Guide - Section: Type Definitions - Training Plan Types](./workout-calendar-implementation-guide.md#3-training-plan-types-srctypestraining-plants)**

### Step 4: Create Calendar Types

**File:** `src/types/calendar.ts`

Copy from **[Implementation Guide - Section: Type Definitions - Calendar Types](./workout-calendar-implementation-guide.md#4-calendar-types-srctypescalendarts)**

### Step 5: Create Index Export

**File:** `src/types/index.ts`

Copy from **[Implementation Guide - Section: Type Definitions - Index Export](./workout-calendar-implementation-guide.md#5-index-export-srctypesindexts)**

### Step 6: Set Up Zustand Store

**File:** `src/stores/calendar-store.ts`

Refer to **[ADR-0001 - Section: State Management Strategy](../specs/adr/0001-workout-calendar-component-architecture.md#3-state-management-strategy)**

**Verify:**
```bash
npm run type-check
npm run lint
```

---

## 📅 Day 2: Utilities (2-3 hours)

### Step 1: Calendar Helpers

**File:** `src/components/calendar/utils/calendar-helpers.ts`

Copy from **[Implementation Guide - Section: Utilities & Helpers - Calendar Helpers](./workout-calendar-implementation-guide.md#1-calendar-helpers-srccomponentscalendarutilscalendar-helpersts)**

**Write tests immediately:**
**File:** `src/__tests__/components/calendar/utils/calendar-helpers.test.ts`

Copy test examples from **[ADR-0001 - Section: Testing Strategy - Example Unit Test](../specs/adr/0001-workout-calendar-component-architecture.md#example-unit-test)**

Run tests:
```bash
npm test -- calendar-helpers.test.ts
```

### Step 2: Discipline Colors

**File:** `src/components/calendar/utils/discipline-colors.ts`

Copy from **[Implementation Guide - Section: Utilities & Helpers - Discipline Colors](./workout-calendar-implementation-guide.md#2-discipline-colors-utility-srccomponentscalendarutilsdiscipline-colorsts)**

### Step 3: Workout Filters

**File:** `src/components/calendar/utils/workout-filters.ts`

Copy from **[Implementation Guide - Section: Utilities & Helpers - Workout Filters](./workout-calendar-implementation-guide.md#3-workout-filters-srccomponentscalendarutilsworkout-filtersts)**

**Write tests for filters** following the same pattern.

**Verify:**
```bash
npm test -- --coverage
# Should see high coverage for utils
```

---

## 🎨 Day 3: Atomic Components (3-4 hours)

### Step 1: Discipline Icon

**File:** `src/components/calendar/workout/DisciplineIcon.tsx`

Copy from **[Implementation Guide - Section: Component Examples - Discipline Icon](./workout-calendar-implementation-guide.md#1-discipline-icon-srccomponentscalendarworkoutdisciplineicontsx)**

### Step 2: Workout Status Badge

**File:** `src/components/calendar/workout/WorkoutStatusBadge.tsx`

Copy from **[Implementation Guide - Section: Component Examples - Workout Status Badge](./workout-calendar-implementation-guide.md#2-workout-status-badge-srccomponentscalendarworkoutworkoutstatusbadgetsx)**

### Step 3: Workout Card

**File:** `src/components/calendar/workout/WorkoutCard.tsx`

Copy from **[Implementation Guide - Section: Component Examples - Workout Card](./workout-calendar-implementation-guide.md#3-workout-card-srccomponentscalendarworkoutworkoutcardtsx)**

### Step 4: Test Components

**File:** `src/__tests__/components/calendar/workout/WorkoutCard.test.tsx`

Write component tests following patterns in the **[ADR-0001 - Example Component Test](../specs/adr/0001-workout-calendar-component-architecture.md#example-component-test)**

**Verify:**
```bash
npm test -- WorkoutCard.test.tsx
npm test -- --coverage --collectCoverageFrom='src/components/calendar/**/*.tsx'
```

---

## 🔗 Day 4: Hooks and Service (3-4 hours)

### Step 1: Service Layer

**File:** `src/services/training-plan-service.ts`

Copy from **[Implementation Guide - Section: Service Layer](./workout-calendar-implementation-guide.md#training-plan-service-srcservicestraining-plan-servicets)**

### Step 2: useWorkoutData Hook

**File:** `src/components/calendar/hooks/useWorkoutData.ts`

Copy from **[Implementation Guide - Section: Hooks - useWorkoutData](./workout-calendar-implementation-guide.md#2-workout-data-hook-srccomponentscalendarhooksuseworkoutdatats)**

### Step 3: useCalendarNavigation Hook

**File:** `src/components/calendar/hooks/useCalendarNavigation.ts`

Copy from **[Implementation Guide - Section: Hooks - useCalendarNavigation](./workout-calendar-implementation-guide.md#1-calendar-navigation-hook-srccomponentscalendarhooksusecalendarnavigationts)**

### Step 4: useCalendarKeyboard Hook

**File:** `src/components/calendar/hooks/useCalendarKeyboard.ts`

Copy from **[Implementation Guide - Section: Hooks - useCalendarKeyboard](./workout-calendar-implementation-guide.md#3-keyboard-navigation-hook-srccomponentscalendarhooksusecalendarkeyboardts)**

**Test hooks** using `@testing-library/react-hooks` patterns.

---

## 📊 Week 2-3: Views

Follow the **[Architecture Summary - Implementation Roadmap](./workout-calendar-architecture-summary.md#phase-3-views--navigation-week-3)** for building:

- WeeklyView
- DailyView
- MonthlyView
- CalendarHeader
- CalendarControls

---

## 🎯 Tips for Success

### ✅ Do This

- **Copy code examples exactly first**, then customize
- **Write tests immediately** after each component
- **Run tests frequently** (`npm test -- --watch`)
- **Check types often** (`npm run type-check`)
- **Commit after each working component**
- **Keep the Quick Reference open** while coding

### ❌ Avoid This

- Skipping tests ("I'll add them later")
- Changing the structure before understanding it
- Implementing everything before testing anything
- Ignoring TypeScript errors
- Copying code without understanding it

---

## 📖 Reference Documents

Keep these open in tabs:

1. **[Quick Reference](./workout-calendar-quick-reference.md)** - For daily coding
2. **[Implementation Guide](./workout-calendar-implementation-guide.md)** - For code examples
3. **[Architecture Diagram](./workout-calendar-architecture-diagram.md)** - For understanding relationships

---

## 🆘 When You Get Stuck

### TypeScript Errors?
- Check the type definitions in `src/types/`
- Ensure all imports are correct
- Run `npm run type-check`

### Tests Failing?
- Read the error message carefully
- Check if you're using `renderWithProviders`
- Verify mock data structure
- Look at test examples in Implementation Guide

### Component Not Rendering?
- Check browser console for errors
- Verify all imports
- Ensure props are passed correctly
- Use React DevTools

### Can't Find Example Code?
- Search in Implementation Guide
- Check ADR-0001 for patterns
- Look at Quick Reference for syntax

---

## ✅ Daily Checklist

At the end of each day:

- [ ] All new code has passing tests
- [ ] TypeScript compilation succeeds
- [ ] ESLint shows no errors
- [ ] Git commit with clear message
- [ ] Coverage remains ≥85%

---

## 🎉 You're Ready!

You have:
- ✅ Project structure created
- ✅ Documentation at hand
- ✅ Clear day-by-day plan
- ✅ Code examples to follow
- ✅ Testing patterns

**Start with Day 1 and work systematically through the plan.**

Good luck! 🚀

---

**Questions?** Refer to the comprehensive documentation:
- Quick answers → Quick Reference
- How-to → Implementation Guide  
- Why → ADR-0001
- Overview → Architecture Summary
