/**
 * Unit tests for workout-filters utilities
 */

import { describe, it, expect } from '@jest/globals';
import {
  filterWorkoutsByDate,
  filterWorkoutsByStatus,
  filterWorkoutsByDiscipline,
  getWorkoutForDate,
  countWorkoutsByStatus,
  calculateCompletionPercentage,
} from '@/components/calendar/utils/workout-filters';
import { Workout, WorkoutStatus } from '@/types/workout';
import { Discipline } from '@/types/discipline';
import { CalendarDate } from '@/types/calendar';

const mockWorkouts: Workout[] = [
  {
    id: '1',
    name: 'HYROX Sim',
    discipline: Discipline.HYROX,
    scheduledDate: '2026-02-15',
    duration: 60,
    description: 'Test',
    status: WorkoutStatus.COMPLETED,
    exercises: [],
  },
  {
    id: '2',
    name: 'Easy Run',
    discipline: Discipline.RUNNING,
    scheduledDate: '2026-02-16',
    duration: 30,
    description: 'Test',
    status: WorkoutStatus.NOT_STARTED,
    exercises: [],
  },
  {
    id: '3',
    name: 'Strength',
    discipline: Discipline.STRENGTH,
    scheduledDate: '2026-02-17',
    duration: 45,
    description: 'Test',
    status: WorkoutStatus.MISSED,
    exercises: [],
  },
];

describe('workout-filters', () => {
  describe('filterWorkoutsByDate', () => {
    it('should filter workouts by specific date', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = filterWorkoutsByDate(mockWorkouts, date);
      expect(result).toHaveLength(1);
      expect(result[0].id).toBe('1');
    });

    it('should return empty array for date with no workouts', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 20 };
      const result = filterWorkoutsByDate(mockWorkouts, date);
      expect(result).toHaveLength(0);
    });
  });

  describe('filterWorkoutsByStatus', () => {
    it('should filter completed workouts', () => {
      const result = filterWorkoutsByStatus(mockWorkouts, WorkoutStatus.COMPLETED);
      expect(result).toHaveLength(1);
      expect(result[0].status).toBe(WorkoutStatus.COMPLETED);
    });

    it('should filter not started workouts', () => {
      const result = filterWorkoutsByStatus(mockWorkouts, WorkoutStatus.NOT_STARTED);
      expect(result).toHaveLength(1);
      expect(result[0].status).toBe(WorkoutStatus.NOT_STARTED);
    });

    it('should filter missed workouts', () => {
      const result = filterWorkoutsByStatus(mockWorkouts, WorkoutStatus.MISSED);
      expect(result).toHaveLength(1);
      expect(result[0].status).toBe(WorkoutStatus.MISSED);
    });
  });

  describe('filterWorkoutsByDiscipline', () => {
    it('should filter HYROX workouts', () => {
      const result = filterWorkoutsByDiscipline(mockWorkouts, Discipline.HYROX);
      expect(result).toHaveLength(1);
      expect(result[0].discipline).toBe(Discipline.HYROX);
    });

    it('should filter RUNNING workouts', () => {
      const result = filterWorkoutsByDiscipline(mockWorkouts, Discipline.RUNNING);
      expect(result).toHaveLength(1);
      expect(result[0].discipline).toBe(Discipline.RUNNING);
    });
  });

  describe('getWorkoutForDate', () => {
    it('should return workout for specific date', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = getWorkoutForDate(mockWorkouts, date);
      expect(result).not.toBeNull();
      expect(result?.id).toBe('1');
    });

    it('should return null for date with no workout', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 20 };
      const result = getWorkoutForDate(mockWorkouts, date);
      expect(result).toBeNull();
    });
  });

  describe('countWorkoutsByStatus', () => {
    it('should count workouts by status', () => {
      const counts = countWorkoutsByStatus(mockWorkouts);
      expect(counts[WorkoutStatus.COMPLETED]).toBe(1);
      expect(counts[WorkoutStatus.NOT_STARTED]).toBe(1);
      expect(counts[WorkoutStatus.MISSED]).toBe(1);
      expect(counts[WorkoutStatus.IN_PROGRESS]).toBe(0);
      expect(counts[WorkoutStatus.SKIPPED]).toBe(0);
    });

    it('should return zero counts for empty array', () => {
      const counts = countWorkoutsByStatus([]);
      expect(counts[WorkoutStatus.COMPLETED]).toBe(0);
      expect(counts[WorkoutStatus.NOT_STARTED]).toBe(0);
    });
  });

  describe('calculateCompletionPercentage', () => {
    it('should calculate completion percentage', () => {
      const percentage = calculateCompletionPercentage(mockWorkouts);
      expect(percentage).toBe(33); // 1 out of 3 = 33%
    });

    it('should return 0 for empty array', () => {
      const percentage = calculateCompletionPercentage([]);
      expect(percentage).toBe(0);
    });

    it('should return 100 for all completed', () => {
      const allCompleted: Workout[] = [
        { ...mockWorkouts[0], status: WorkoutStatus.COMPLETED },
        { ...mockWorkouts[1], status: WorkoutStatus.COMPLETED },
      ];
      const percentage = calculateCompletionPercentage(allCompleted);
      expect(percentage).toBe(100);
    });
  });
});
