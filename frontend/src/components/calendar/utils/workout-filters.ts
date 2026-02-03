/**
 * Workout filtering and grouping utilities
 */

import { Workout, WorkoutStatus } from '@/types/workout';
import { Discipline } from '@/types/discipline';
import { CalendarDate } from '@/types/calendar';
import { calendarDateToISO } from './calendar-helpers';

/**
 * Filter workouts by date
 */
export function filterWorkoutsByDate(workouts: Workout[], date: CalendarDate): Workout[] {
  const targetISO = calendarDateToISO(date);
  return workouts.filter((workout) => workout.scheduledDate === targetISO);
}

/**
 * Filter workouts by date range (inclusive)
 */
export function filterWorkoutsByDateRange(
  workouts: Workout[],
  startDate: CalendarDate,
  endDate: CalendarDate
): Workout[] {
  const startISO = calendarDateToISO(startDate);
  const endISO = calendarDateToISO(endDate);

  return workouts.filter((workout) => {
    return workout.scheduledDate >= startISO && workout.scheduledDate <= endISO;
  });
}

/**
 * Filter workouts by status
 */
export function filterWorkoutsByStatus(
  workouts: Workout[],
  status: WorkoutStatus
): Workout[] {
  return workouts.filter((workout) => workout.status === status);
}

/**
 * Filter workouts by discipline
 */
export function filterWorkoutsByDiscipline(
  workouts: Workout[],
  discipline: Discipline
): Workout[] {
  return workouts.filter((workout) => workout.discipline === discipline);
}

/**
 * Group workouts by date
 */
export function groupWorkoutsByDate(
  workouts: Workout[]
): Map<string, Workout[]> {
  const grouped = new Map<string, Workout[]>();

  workouts.forEach((workout) => {
    const dateKey = workout.scheduledDate;
    const existing = grouped.get(dateKey) || [];
    grouped.set(dateKey, [...existing, workout]);
  });

  return grouped;
}

/**
 * Get workout for a specific date (returns first if multiple)
 */
export function getWorkoutForDate(workouts: Workout[], date: CalendarDate): Workout | null {
  const filtered = filterWorkoutsByDate(workouts, date);
  return filtered.length > 0 ? filtered[0] : null;
}

/**
 * Get today's workout
 */
export function getTodaysWorkout(workouts: Workout[]): Workout | null {
  const today = new Date();
  const todayDate: CalendarDate = {
    year: today.getFullYear(),
    month: today.getMonth() + 1,
    day: today.getDate(),
  };
  return getWorkoutForDate(workouts, todayDate);
}

/**
 * Get tomorrow's workout
 */
export function getTomorrowsWorkout(workouts: Workout[]): Workout | null {
  const tomorrow = new Date();
  tomorrow.setDate(tomorrow.getDate() + 1);
  const tomorrowDate: CalendarDate = {
    year: tomorrow.getFullYear(),
    month: tomorrow.getMonth() + 1,
    day: tomorrow.getDate(),
  };
  return getWorkoutForDate(workouts, tomorrowDate);
}

/**
 * Count workouts by status
 */
export function countWorkoutsByStatus(workouts: Workout[]): Record<WorkoutStatus, number> {
  const counts = {
    [WorkoutStatus.NOT_STARTED]: 0,
    [WorkoutStatus.IN_PROGRESS]: 0,
    [WorkoutStatus.COMPLETED]: 0,
    [WorkoutStatus.MISSED]: 0,
    [WorkoutStatus.SKIPPED]: 0,
  };

  workouts.forEach((workout) => {
    counts[workout.status]++;
  });

  return counts;
}

/**
 * Calculate completion percentage
 */
export function calculateCompletionPercentage(workouts: Workout[]): number {
  if (workouts.length === 0) return 0;
  const completed = filterWorkoutsByStatus(workouts, WorkoutStatus.COMPLETED).length;
  return Math.round((completed / workouts.length) * 100);
}

/**
 * Check if a workout is overdue (past date and not completed)
 */
export function isWorkoutOverdue(workout: Workout): boolean {
  const workoutDateObj = new Date(workout.scheduledDate);
  const today = new Date();
  const todayDateObj = new Date(today.getFullYear(), today.getMonth(), today.getDate());

  return (
    workoutDateObj < todayDateObj &&
    workout.status !== WorkoutStatus.COMPLETED &&
    workout.status !== WorkoutStatus.SKIPPED
  );
}
