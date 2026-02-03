# Workout Calendar Component - Implementation Guide

This guide provides detailed implementation examples and patterns for building the Workout Calendar component based on [ADR-0001: Workout Calendar Component Architecture](../adr/0001-workout-calendar-component-architecture.md).

---

## Table of Contents

1. [Getting Started](#getting-started)
2. [Type Definitions](#type-definitions)
3. [Utilities & Helpers](#utilities--helpers)
4. [Component Examples](#component-examples)
5. [Hooks](#hooks)
6. [Service Layer](#service-layer)
7. [Testing Examples](#testing-examples)
8. [Common Patterns](#common-patterns)

---

## Getting Started

### File Structure Setup

Create the following directory structure:

```bash
# From frontend/ directory
mkdir -p src/components/calendar/{views,workout,utils,hooks}
mkdir -p src/types
mkdir -p src/stores
mkdir -p src/services
mkdir -p src/__tests__/components/calendar/{views,hooks,utils}
mkdir -p src/__tests__/mocks
```

### Installation (if needed)

All required dependencies are already in `package.json`. Verify with:

```bash
npm list zustand lucide-react
```

---

## Type Definitions

### 1. Discipline Types (`src/types/discipline.ts`)

```typescript
/**
 * Training discipline enumeration
 * Represents the different types of workouts in the training plan
 */
export enum Discipline {
  HYROX = 'HYROX',
  Running = 'Running',
  Strength = 'Strength',
  Hybrid = 'Hybrid',
  Rest = 'Rest',
}

/**
 * Tailwind color classes for each discipline
 * Ensures WCAG AA contrast compliance
 */
export const DISCIPLINE_COLORS = {
  [Discipline.HYROX]: {
    bg: 'bg-orange-100',
    bgHover: 'hover:bg-orange-200',
    border: 'border-orange-500',
    text: 'text-orange-900',
    icon: 'text-orange-600',
    badge: 'bg-orange-500 text-white',
  },
  [Discipline.Running]: {
    bg: 'bg-blue-100',
    bgHover: 'hover:bg-blue-200',
    border: 'border-blue-500',
    text: 'text-blue-900',
    icon: 'text-blue-600',
    badge: 'bg-blue-500 text-white',
  },
  [Discipline.Strength]: {
    bg: 'bg-purple-100',
    bgHover: 'hover:bg-purple-200',
    border: 'border-purple-500',
    text: 'text-purple-900',
    icon: 'text-purple-600',
    badge: 'bg-purple-500 text-white',
  },
  [Discipline.Hybrid]: {
    bg: 'bg-gradient-to-r from-orange-100 via-blue-100 to-purple-100',
    bgHover: 'hover:opacity-80',
    border: 'border-purple-500',
    text: 'text-gray-900',
    icon: 'text-purple-600',
    badge: 'bg-gradient-to-r from-orange-500 to-purple-500 text-white',
  },
  [Discipline.Rest]: {
    bg: 'bg-gray-100',
    bgHover: 'hover:bg-gray-200',
    border: 'border-gray-400',
    text: 'text-gray-700',
    icon: 'text-gray-500',
    badge: 'bg-gray-500 text-white',
  },
} as const;

/**
 * Lucide icon names for each discipline
 * Provides visual differentiation beyond color (colorblind-friendly)
 */
export const DISCIPLINE_ICONS = {
  [Discipline.HYROX]: 'Zap',
  [Discipline.Running]: 'Footprints',
  [Discipline.Strength]: 'Dumbbell',
  [Discipline.Hybrid]: 'Sparkles',
  [Discipline.Rest]: 'Coffee',
} as const;

export type DisciplineIconName = typeof DISCIPLINE_ICONS[Discipline];
```

### 2. Workout Types (`src/types/workout.ts`)

```typescript
import { Discipline } from './discipline';

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
  LongRun = 'LongRun',
  Recovery = 'Recovery',
  RaceSimulation = 'RaceSimulation',
  FullBody = 'FullBody',
  UpperBody = 'UpperBody',
  LowerBody = 'LowerBody',
  Core = 'Core',
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
  scheduledDate: string; // ISO 8601 format: YYYY-MM-DD
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

// Status badge configuration
export const WORKOUT_STATUS_CONFIG = {
  [WorkoutStatus.NotStarted]: {
    label: 'Scheduled',
    color: 'bg-gray-200 text-gray-700',
    icon: 'Calendar',
  },
  [WorkoutStatus.InProgress]: {
    label: 'In Progress',
    color: 'bg-yellow-200 text-yellow-800',
    icon: 'PlayCircle',
  },
  [WorkoutStatus.Completed]: {
    label: 'Completed',
    color: 'bg-green-200 text-green-800',
    icon: 'CheckCircle2',
  },
  [WorkoutStatus.Missed]: {
    label: 'Missed',
    color: 'bg-red-200 text-red-800',
    icon: 'XCircle',
  },
  [WorkoutStatus.Skipped]: {
    label: 'Skipped',
    color: 'bg-gray-300 text-gray-600',
    icon: 'SkipForward',
  },
} as const;
```

### 3. Training Plan Types (`src/types/training-plan.ts`)

```typescript
import { Workout, IntensityLevel } from './workout';

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
  weeklyVolume: number; // total minutes
  intensityLevel: IntensityLevel;
  focusArea: string;
  startDate: string; // ISO 8601
  endDate: string; // ISO 8601
  workouts: Workout[];
}

export interface TrainingPlan {
  id: string;
  userId: string;
  name: string;
  startDate: string; // ISO 8601
  endDate: string; // ISO 8601
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

### 4. Calendar Types (`src/types/calendar.ts`)

```typescript
import { TrainingPlan, Workout } from './';

export enum CalendarView {
  Daily = 'Daily',
  Weekly = 'Weekly',
  Monthly = 'Monthly',
}

export interface CalendarDate {
  year: number;
  month: number; // 0-11 (JavaScript Date convention)
  day: number;
}

export interface CalendarState {
  view: CalendarView;
  selectedDate: CalendarDate;
}

export interface WorkoutsByDate {
  [dateKey: string]: Workout[]; // dateKey format: 'YYYY-MM-DD'
}

export interface CalendarWeek {
  weekNumber: number;
  dates: Date[];
  workouts: WorkoutsByDate;
}
```

### 5. Index Export (`src/types/index.ts`)

```typescript
// Re-export all types for easier imports
export * from './discipline';
export * from './workout';
export * from './training-plan';
export * from './calendar';
export * from './onboarding'; // existing
```

---

## Utilities & Helpers

### 1. Calendar Helpers (`src/components/calendar/utils/calendar-helpers.ts`)

```typescript
import { CalendarDate, CalendarView } from '@/types/calendar';

/**
 * Get the start of the week (Sunday) for a given date
 */
export function getWeekStart(date: Date): Date {
  const result = new Date(date);
  const day = result.getDay();
  const diff = result.getDate() - day;
  result.setDate(diff);
  result.setHours(0, 0, 0, 0);
  return result;
}

/**
 * Get all 7 dates in the week containing the given date
 * Returns Sunday through Saturday
 */
export function getWeekDates(date: Date): Date[] {
  const weekStart = getWeekStart(date);
  const dates: Date[] = [];
  
  for (let i = 0; i < 7; i++) {
    const day = new Date(weekStart);
    day.setDate(weekStart.getDate() + i);
    dates.push(day);
  }
  
  return dates;
}

/**
 * Format a date as YYYY-MM-DD for use as object keys
 */
export function formatDateKey(date: Date): string {
  const year = date.getFullYear();
  const month = String(date.getMonth() + 1).padStart(2, '0');
  const day = String(date.getDate()).padStart(2, '0');
  return `${year}-${month}-${day}`;
}

/**
 * Parse a date key (YYYY-MM-DD) into a Date object
 */
export function parseDateKey(dateKey: string): Date {
  return new Date(dateKey);
}

/**
 * Check if a date is today
 */
export function isToday(date: Date): boolean {
  const today = new Date();
  return isSameDay(date, today);
}

/**
 * Check if two dates are the same day
 */
export function isSameDay(date1: Date, date2: Date): boolean {
  return (
    date1.getFullYear() === date2.getFullYear() &&
    date1.getMonth() === date2.getMonth() &&
    date1.getDate() === date2.getDate()
  );
}

/**
 * Check if a date is in the past
 */
export function isPast(date: Date): boolean {
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  return date < today;
}

/**
 * Check if a date is in the future
 */
export function isFuture(date: Date): boolean {
  const today = new Date();
  today.setHours(23, 59, 59, 999);
  return date > today;
}

/**
 * Get the first day of the month for a given date
 */
export function getMonthStart(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth(), 1);
}

/**
 * Get the last day of the month for a given date
 */
export function getMonthEnd(date: Date): Date {
  return new Date(date.getFullYear(), date.getMonth() + 1, 0);
}

/**
 * Get all dates in the month (including leading/trailing days to fill grid)
 */
export function getMonthDates(date: Date): Date[] {
  const monthStart = getMonthStart(date);
  const monthEnd = getMonthEnd(date);
  const weekStart = getWeekStart(monthStart);
  
  const dates: Date[] = [];
  let current = new Date(weekStart);
  
  // Generate dates until we've covered the entire month
  while (current <= monthEnd || dates.length % 7 !== 0) {
    dates.push(new Date(current));
    current.setDate(current.getDate() + 1);
  }
  
  return dates;
}

/**
 * Format a date for display
 */
export function formatDisplayDate(date: Date, format: 'short' | 'long' = 'short'): string {
  if (format === 'short') {
    return date.toLocaleDateString('en-US', { 
      month: 'short', 
      day: 'numeric' 
    });
  }
  
  return date.toLocaleDateString('en-US', { 
    weekday: 'long',
    month: 'long', 
    day: 'numeric',
    year: 'numeric'
  });
}

/**
 * Get the name of the month
 */
export function getMonthName(date: Date): string {
  return date.toLocaleDateString('en-US', { month: 'long' });
}

/**
 * Get the abbreviated day name
 */
export function getDayAbbreviation(date: Date): string {
  return date.toLocaleDateString('en-US', { weekday: 'short' });
}

/**
 * Convert CalendarDate to JavaScript Date
 */
export function calendarDateToDate(calendarDate: CalendarDate): Date {
  return new Date(calendarDate.year, calendarDate.month, calendarDate.day);
}

/**
 * Convert JavaScript Date to CalendarDate
 */
export function dateToCalendarDate(date: Date): CalendarDate {
  return {
    year: date.getFullYear(),
    month: date.getMonth(),
    day: date.getDate(),
  };
}

/**
 * Calculate the next period based on current view
 */
export function getNextPeriod(view: CalendarView, date: CalendarDate): CalendarDate {
  const jsDate = calendarDateToDate(date);
  
  switch (view) {
    case CalendarView.Daily:
      jsDate.setDate(jsDate.getDate() + 1);
      break;
    case CalendarView.Weekly:
      jsDate.setDate(jsDate.getDate() + 7);
      break;
    case CalendarView.Monthly:
      jsDate.setMonth(jsDate.getMonth() + 1);
      break;
  }
  
  return dateToCalendarDate(jsDate);
}

/**
 * Calculate the previous period based on current view
 */
export function getPreviousPeriod(view: CalendarView, date: CalendarDate): CalendarDate {
  const jsDate = calendarDateToDate(date);
  
  switch (view) {
    case CalendarView.Daily:
      jsDate.setDate(jsDate.getDate() - 1);
      break;
    case CalendarView.Weekly:
      jsDate.setDate(jsDate.getDate() - 7);
      break;
    case CalendarView.Monthly:
      jsDate.setMonth(jsDate.getMonth() - 1);
      break;
  }
  
  return dateToCalendarDate(jsDate);
}
```

### 2. Discipline Colors Utility (`src/components/calendar/utils/discipline-colors.ts`)

```typescript
import { Discipline, DISCIPLINE_COLORS } from '@/types/discipline';

export function getDisciplineColors(discipline: Discipline) {
  return DISCIPLINE_COLORS[discipline];
}

export function getDisciplineColorClasses(discipline: Discipline): string {
  const colors = DISCIPLINE_COLORS[discipline];
  return `${colors.bg} ${colors.border} ${colors.text}`;
}

export function getDisciplineBadgeClasses(discipline: Discipline): string {
  return DISCIPLINE_COLORS[discipline].badge;
}
```

### 3. Workout Filters (`src/components/calendar/utils/workout-filters.ts`)

```typescript
import { Workout, WorkoutStatus, Discipline } from '@/types';
import { formatDateKey } from './calendar-helpers';

export function filterWorkoutsByDate(
  workouts: Workout[],
  date: Date
): Workout[] {
  const dateKey = formatDateKey(date);
  return workouts.filter((w) => w.scheduledDate === dateKey);
}

export function filterWorkoutsByStatus(
  workouts: Workout[],
  status: WorkoutStatus
): Workout[] {
  return workouts.filter((w) => w.status === status);
}

export function filterWorkoutsByDiscipline(
  workouts: Workout[],
  discipline: Discipline
): Workout[] {
  return workouts.filter((w) => w.discipline === discipline);
}

export function getCompletedWorkoutsCount(workouts: Workout[]): number {
  return workouts.filter((w) => w.status === WorkoutStatus.Completed).length;
}

export function getTotalWorkoutsCount(workouts: Workout[]): number {
  return workouts.filter((w) => w.status !== WorkoutStatus.Skipped).length;
}

export function getCompletionPercentage(workouts: Workout[]): number {
  const total = getTotalWorkoutsCount(workouts);
  if (total === 0) return 0;
  
  const completed = getCompletedWorkoutsCount(workouts);
  return Math.round((completed / total) * 100);
}

export function groupWorkoutsByDate(workouts: Workout[]): Record<string, Workout[]> {
  return workouts.reduce((acc, workout) => {
    const dateKey = workout.scheduledDate;
    if (!acc[dateKey]) {
      acc[dateKey] = [];
    }
    acc[dateKey].push(workout);
    return acc;
  }, {} as Record<string, Workout[]>);
}
```

---

## Component Examples

### 1. Discipline Icon (`src/components/calendar/workout/DisciplineIcon.tsx`)

```typescript
'use client';

import { Discipline, DISCIPLINE_ICONS, DISCIPLINE_COLORS } from '@/types/discipline';
import { 
  Zap, 
  Footprints, 
  Dumbbell, 
  Sparkles, 
  Coffee,
  LucideIcon 
} from 'lucide-react';

const ICON_COMPONENTS: Record<Discipline, LucideIcon> = {
  [Discipline.HYROX]: Zap,
  [Discipline.Running]: Footprints,
  [Discipline.Strength]: Dumbbell,
  [Discipline.Hybrid]: Sparkles,
  [Discipline.Rest]: Coffee,
};

interface DisciplineIconProps {
  discipline: Discipline;
  size?: number;
  className?: string;
}

export function DisciplineIcon({ 
  discipline, 
  size = 20, 
  className = '' 
}: DisciplineIconProps) {
  const Icon = ICON_COMPONENTS[discipline];
  const colorClass = DISCIPLINE_COLORS[discipline].icon;
  
  return (
    <Icon 
      size={size} 
      className={`${colorClass} ${className}`}
      aria-label={`${discipline} discipline`}
    />
  );
}
```

### 2. Workout Status Badge (`src/components/calendar/workout/WorkoutStatusBadge.tsx`)

```typescript
'use client';

import { WorkoutStatus, WORKOUT_STATUS_CONFIG } from '@/types/workout';
import {
  Calendar,
  PlayCircle,
  CheckCircle2,
  XCircle,
  SkipForward,
  LucideIcon
} from 'lucide-react';

const STATUS_ICONS: Record<WorkoutStatus, LucideIcon> = {
  [WorkoutStatus.NotStarted]: Calendar,
  [WorkoutStatus.InProgress]: PlayCircle,
  [WorkoutStatus.Completed]: CheckCircle2,
  [WorkoutStatus.Missed]: XCircle,
  [WorkoutStatus.Skipped]: SkipForward,
};

interface WorkoutStatusBadgeProps {
  status: WorkoutStatus;
  showIcon?: boolean;
  showLabel?: boolean;
  className?: string;
}

export function WorkoutStatusBadge({
  status,
  showIcon = true,
  showLabel = true,
  className = '',
}: WorkoutStatusBadgeProps) {
  const config = WORKOUT_STATUS_CONFIG[status];
  const Icon = STATUS_ICONS[status];

  return (
    <span
      className={`inline-flex items-center gap-1 px-2 py-1 rounded-full text-xs font-medium ${config.color} ${className}`}
      aria-label={`Status: ${config.label}`}
    >
      {showIcon && <Icon size={14} aria-hidden="true" />}
      {showLabel && <span>{config.label}</span>}
    </span>
  );
}
```

### 3. Workout Card (`src/components/calendar/workout/WorkoutCard.tsx`)

```typescript
'use client';

import { memo } from 'react';
import { Workout, WorkoutStatus } from '@/types/workout';
import { Card } from '@/components/ui/card';
import { DisciplineIcon } from './DisciplineIcon';
import { WorkoutStatusBadge } from './WorkoutStatusBadge';
import { getDisciplineColors } from '../utils/discipline-colors';
import { Clock } from 'lucide-react';

interface WorkoutCardProps {
  workout: Workout;
  onClick?: (workout: Workout) => void;
  onStatusChange?: (workoutId: string, status: WorkoutStatus) => void;
  compact?: boolean;
}

function WorkoutCardComponent({
  workout,
  onClick,
  compact = false,
}: WorkoutCardProps) {
  const disciplineColors = getDisciplineColors(workout.discipline);

  const handleClick = () => {
    onClick?.(workout);
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      handleClick();
    }
  };

  if (compact) {
    return (
      <div
        role="button"
        tabIndex={0}
        onClick={handleClick}
        onKeyDown={handleKeyDown}
        className={`
          p-2 rounded-lg border-l-4 cursor-pointer
          ${disciplineColors.bg} ${disciplineColors.border}
          ${disciplineColors.bgHover}
          focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-1
          transition-colors
        `}
        aria-label={`${workout.name} - ${workout.discipline} workout`}
      >
        <div className="flex items-start justify-between gap-2">
          <div className="flex-1 min-w-0">
            <p className={`text-sm font-medium truncate ${disciplineColors.text}`}>
              {workout.name}
            </p>
            <div className="flex items-center gap-2 mt-1">
              <DisciplineIcon discipline={workout.discipline} size={14} />
              <span className="text-xs text-gray-600 flex items-center gap-1">
                <Clock size={12} />
                {workout.estimatedDuration}min
              </span>
            </div>
          </div>
          <WorkoutStatusBadge status={workout.status} showLabel={false} />
        </div>
      </div>
    );
  }

  return (
    <Card
      role="article"
      tabIndex={0}
      onClick={handleClick}
      onKeyDown={handleKeyDown}
      className={`
        cursor-pointer border-l-4
        ${disciplineColors.border}
        hover:shadow-md
        focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-2
        transition-all
      `}
      aria-label={`${workout.name} - ${workout.discipline} workout`}
      aria-describedby={`workout-${workout.id}-description`}
    >
      <div className="p-4">
        <div className="flex items-start justify-between mb-3">
          <div className="flex items-center gap-2">
            <DisciplineIcon discipline={workout.discipline} size={24} />
            <div>
              <h3 className={`font-semibold ${disciplineColors.text}`}>
                {workout.name}
              </h3>
              <p className="text-xs text-gray-500">
                {workout.sessionType}
              </p>
            </div>
          </div>
          <WorkoutStatusBadge status={workout.status} />
        </div>

        <p
          id={`workout-${workout.id}-description`}
          className="text-sm text-gray-600 mb-3 line-clamp-2"
        >
          {workout.description}
        </p>

        <div className="flex items-center justify-between text-sm">
          <div className="flex items-center gap-1 text-gray-600">
            <Clock size={16} />
            <span>{workout.estimatedDuration} min</span>
          </div>

          {workout.isKeyWorkout && (
            <span className="px-2 py-1 bg-yellow-100 text-yellow-800 text-xs font-medium rounded">
              Key Workout
            </span>
          )}
        </div>
      </div>
    </Card>
  );
}

export const WorkoutCard = memo(WorkoutCardComponent, (prev, next) => {
  return (
    prev.workout.id === next.workout.id &&
    prev.workout.status === next.workout.status &&
    prev.compact === next.compact
  );
});

WorkoutCard.displayName = 'WorkoutCard';
```

---

## Hooks

### 1. Calendar Navigation Hook (`src/components/calendar/hooks/useCalendarNavigation.ts`)

```typescript
import { useCallback } from 'react';
import { useCalendarStore } from '@/stores/calendar-store';
import { CalendarView } from '@/types/calendar';

export function useCalendarNavigation() {
  const {
    view,
    selectedDate,
    setView,
    setSelectedDate,
    goToToday,
    nextPeriod,
    previousPeriod,
  } = useCalendarStore();

  const navigateNext = useCallback(() => {
    nextPeriod();
  }, [nextPeriod]);

  const navigatePrevious = useCallback(() => {
    previousPeriod();
  }, [previousPeriod]);

  const navigateToToday = useCallback(() => {
    goToToday();
  }, [goToToday]);

  const switchView = useCallback(
    (newView: CalendarView) => {
      setView(newView);
    },
    [setView]
  );

  return {
    view,
    selectedDate,
    navigateNext,
    navigatePrevious,
    navigateToToday,
    switchView,
  };
}
```

### 2. Workout Data Hook (`src/components/calendar/hooks/useWorkoutData.ts`)

```typescript
import { useState, useEffect, useCallback } from 'react';
import { TrainingPlanService } from '@/services/training-plan-service';
import { Workout, WorkoutStatus } from '@/types/workout';

interface UseWorkoutDataOptions {
  userId: string;
  startDate: string;
  endDate: string;
  enabled?: boolean;
}

interface UseWorkoutDataResult {
  workouts: Workout[];
  isLoading: boolean;
  error: string | null;
  refetch: () => Promise<void>;
  updateStatus: (workoutId: string, status: WorkoutStatus) => Promise<void>;
}

export function useWorkoutData({
  userId,
  startDate,
  endDate,
  enabled = true,
}: UseWorkoutDataOptions): UseWorkoutDataResult {
  const [workouts, setWorkouts] = useState<Workout[]>([]);
  const [isLoading, setIsLoading] = useState(false);
  const [error, setError] = useState<string | null>(null);

  const fetchWorkouts = useCallback(async () => {
    if (!enabled) return;

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
      const message = err instanceof Error ? err.message : 'Failed to load workouts';
      setError(message);
      console.error('Error fetching workouts:', err);
    } finally {
      setIsLoading(false);
    }
  }, [userId, startDate, endDate, enabled]);

  useEffect(() => {
    fetchWorkouts();
  }, [fetchWorkouts]);

  const updateStatus = useCallback(
    async (workoutId: string, status: WorkoutStatus) => {
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
    },
    [fetchWorkouts]
  );

  return {
    workouts,
    isLoading,
    error,
    refetch: fetchWorkouts,
    updateStatus,
  };
}
```

### 3. Keyboard Navigation Hook (`src/components/calendar/hooks/useCalendarKeyboard.ts`)

```typescript
import { useEffect } from 'react';
import { useCalendarStore } from '@/stores/calendar-store';
import { CalendarView } from '@/types/calendar';

export function useCalendarKeyboard(enabled: boolean = true) {
  const { nextPeriod, previousPeriod, goToToday, setView } = useCalendarStore();

  useEffect(() => {
    if (!enabled) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      // Ignore if user is typing in an input/textarea
      const target = e.target as HTMLElement;
      if (
        target.tagName === 'INPUT' ||
        target.tagName === 'TEXTAREA' ||
        target.isContentEditable
      ) {
        return;
      }

      // Arrow key navigation
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

      // View switching
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

    return () => {
      window.removeEventListener('keydown', handleKeyDown);
    };
  }, [enabled, nextPeriod, previousPeriod, goToToday, setView]);
}
```

---

## Service Layer

### Training Plan Service (`src/services/training-plan-service.ts`)

```typescript
import axios, { AxiosError } from 'axios';
import { TrainingPlan, Workout, WorkoutStatus } from '@/types';

const API_BASE = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000/api';

export class TrainingPlanService {
  /**
   * Get the active training plan for a user
   */
  static async getActivePlan(userId: string): Promise<TrainingPlan> {
    try {
      const response = await axios.get<TrainingPlan>(
        `${API_BASE}/training-plans/active`,
        { params: { userId } }
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  /**
   * Get workouts within a date range
   */
  static async getWorkoutsByDateRange(
    userId: string,
    startDate: string,
    endDate: string
  ): Promise<Workout[]> {
    try {
      const response = await axios.get<Workout[]>(
        `${API_BASE}/workouts/range`,
        { params: { userId, startDate, endDate } }
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  /**
   * Get today's workout for a user
   */
  static async getTodaysWorkout(userId: string): Promise<Workout | null> {
    try {
      const response = await axios.get<Workout | null>(
        `${API_BASE}/workouts/today`,
        { params: { userId } }
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  /**
   * Update workout status
   */
  static async updateWorkoutStatus(
    workoutId: string,
    status: WorkoutStatus
  ): Promise<Workout> {
    try {
      const response = await axios.patch<Workout>(
        `${API_BASE}/workouts/${workoutId}/status`,
        { status }
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  /**
   * Get workout details by ID
   */
  static async getWorkoutById(workoutId: string): Promise<Workout> {
    try {
      const response = await axios.get<Workout>(
        `${API_BASE}/workouts/${workoutId}`
      );
      return response.data;
    } catch (error) {
      throw this.handleError(error);
    }
  }

  /**
   * Centralized error handling
   */
  private static handleError(error: unknown): Error {
    if (axios.isAxiosError(error)) {
      const axiosError = error as AxiosError<{ message?: string }>;
      const message =
        axiosError.response?.data?.message ||
        axiosError.message ||
        'An unexpected error occurred';
      return new Error(message);
    }
    
    if (error instanceof Error) {
      return error;
    }
    
    return new Error('An unexpected error occurred');
  }
}
```

---

## Testing Examples

See the ADR for comprehensive testing examples. Key files to create:

1. `src/__tests__/utils/test-utils.tsx` - Test rendering utilities
2. `src/__tests__/mocks/workout-factory.ts` - Mock data generators
3. Component tests for each major component
4. Integration tests for full workflows

---

## Common Patterns

### Pattern 1: Handling Loading States

```typescript
function WeeklyView() {
  const { workouts, isLoading, error } = useWorkoutData({...});

  if (isLoading) {
    return <CalendarSkeleton />;
  }

  if (error) {
    return (
      <div className="text-center py-8">
        <p className="text-red-600">{error}</p>
        <Button onClick={refetch}>Try Again</Button>
      </div>
    );
  }

  if (workouts.length === 0) {
    return <EmptyState message="No workouts scheduled this week" />;
  }

  return (
    // Render workouts
  );
}
```

### Pattern 2: Optimistic Updates

```typescript
const handleCompleteWorkout = async (workoutId: string) => {
  try {
    // Update UI immediately
    await updateStatus(workoutId, WorkoutStatus.Completed);
    
    // Show success feedback
    toast.success('Workout marked as complete!');
  } catch (error) {
    // Automatically rolled back by useWorkoutData hook
    toast.error('Failed to update workout status');
  }
};
```

### Pattern 3: Accessibility-First Components

```typescript
<button
  onClick={handleClick}
  aria-label="Navigate to next week"
  className="focus:outline-none focus:ring-2 focus:ring-blue-500"
>
  <ChevronRight size={20} aria-hidden="true" />
</button>
```

---

## Next Steps

1. **Start with Types:** Create all type definitions first
2. **Build Utilities:** Implement helper functions with tests
3. **Create Atoms:** Build smallest components (Icon, Badge)
4. **Compose Molecules:** Combine into WorkoutCard
5. **Build Views:** Implement Daily, Weekly, Monthly views
6. **Integration:** Wire up with API and state management
7. **Testing:** Achieve ≥85% coverage
8. **Polish:** Accessibility audit, performance optimization

---

## Resources

- [ADR-0001: Component Architecture](../adr/0001-workout-calendar-component-architecture.md)
- [Task 011: Workout Calendar UI](../tasks/011-task-workout-calendar-ui.md)
- [Tailwind CSS Documentation](https://tailwindcss.com/docs)
- [Lucide React Icons](https://lucide.dev/guide/packages/lucide-react)
- [Zustand Documentation](https://zustand-demo.pmnd.rs/)
