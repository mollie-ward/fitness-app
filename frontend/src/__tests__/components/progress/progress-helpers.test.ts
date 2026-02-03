/**
 * Unit tests for progress helper functions
 */

import { describe, it, expect } from '@jest/globals';
import {
  transformToHeatmapData,
  filterHistoryByDiscipline,
  getHeatmapIntensityColor,
  generateDateRange,
  getMonthDates,
  formatDateForDisplay,
} from '@/components/progress/utils/progress-helpers';
import { Discipline } from '@/types/discipline';
import type { CompletionHistoryDto } from '@/types/progress';

describe('progress-helpers', () => {
  describe('transformToHeatmapData', () => {
    it('should group workouts by date', () => {
      const history: CompletionHistoryDto[] = [
        {
          completedAt: '2026-02-03T10:00:00Z',
          workoutId: '1',
          workoutName: 'Workout 1',
          discipline: Discipline.HYROX,
        },
        {
          completedAt: '2026-02-03T14:00:00Z',
          workoutId: '2',
          workoutName: 'Workout 2',
          discipline: Discipline.RUNNING,
        },
        {
          completedAt: '2026-02-04T10:00:00Z',
          workoutId: '3',
          workoutName: 'Workout 3',
          discipline: Discipline.STRENGTH,
        },
      ];

      const result = transformToHeatmapData(history);

      expect(result).toHaveLength(2);
      expect(result.find((d) => d.date === '2026-02-03')?.count).toBe(2);
      expect(result.find((d) => d.date === '2026-02-04')?.count).toBe(1);
    });

    it('should return empty array for empty history', () => {
      const result = transformToHeatmapData([]);
      expect(result).toEqual([]);
    });
  });

  describe('filterHistoryByDiscipline', () => {
    const history: CompletionHistoryDto[] = [
      {
        completedAt: '2026-02-03T10:00:00Z',
        workoutId: '1',
        workoutName: 'HYROX Workout',
        discipline: Discipline.HYROX,
      },
      {
        completedAt: '2026-02-03T14:00:00Z',
        workoutId: '2',
        workoutName: 'Running Workout',
        discipline: Discipline.RUNNING,
      },
      {
        completedAt: '2026-02-04T10:00:00Z',
        workoutId: '3',
        workoutName: 'Strength Workout',
        discipline: Discipline.STRENGTH,
      },
    ];

    it('should return all workouts for "all" filter', () => {
      const result = filterHistoryByDiscipline(history, 'all');
      expect(result).toHaveLength(3);
    });

    it('should filter by HYROX discipline', () => {
      const result = filterHistoryByDiscipline(history, Discipline.HYROX);
      expect(result).toHaveLength(1);
      expect(result[0].discipline).toBe(Discipline.HYROX);
    });

    it('should filter by RUNNING discipline', () => {
      const result = filterHistoryByDiscipline(history, Discipline.RUNNING);
      expect(result).toHaveLength(1);
      expect(result[0].discipline).toBe(Discipline.RUNNING);
    });

    it('should return empty array if no matches', () => {
      const result = filterHistoryByDiscipline(
        [
          {
            completedAt: '2026-02-03T10:00:00Z',
            workoutId: '1',
            workoutName: 'HYROX Workout',
            discipline: Discipline.HYROX,
          },
        ],
        Discipline.RUNNING
      );
      expect(result).toHaveLength(0);
    });
  });

  describe('getHeatmapIntensityColor', () => {
    it('should return gray for 0 count', () => {
      expect(getHeatmapIntensityColor(0)).toBe('bg-gray-100');
    });

    it('should return light green for 1 count', () => {
      expect(getHeatmapIntensityColor(1)).toBe('bg-green-200');
    });

    it('should return medium green for 2 count', () => {
      expect(getHeatmapIntensityColor(2)).toBe('bg-green-400');
    });

    it('should return dark green for 3 count', () => {
      expect(getHeatmapIntensityColor(3)).toBe('bg-green-600');
    });

    it('should return darkest green for 4+ count', () => {
      expect(getHeatmapIntensityColor(4)).toBe('bg-green-800');
      expect(getHeatmapIntensityColor(10)).toBe('bg-green-800');
    });
  });

  describe('generateDateRange', () => {
    it('should generate correct date range', () => {
      const startDate = new Date('2026-02-01');
      const endDate = new Date('2026-02-03');
      const result = generateDateRange(startDate, endDate);

      expect(result).toHaveLength(3);
      expect(result).toEqual(['2026-02-01', '2026-02-02', '2026-02-03']);
    });

    it('should handle single day range', () => {
      const date = new Date('2026-02-01');
      const result = generateDateRange(date, date);

      expect(result).toHaveLength(1);
      expect(result[0]).toBe('2026-02-01');
    });
  });

  describe('getMonthDates', () => {
    it('should return all dates for January 2026', () => {
      const result = getMonthDates(2026, 0); // January is month 0
      expect(result).toHaveLength(31);
      expect(result[0]).toBe('2026-01-01');
      expect(result[30]).toBe('2026-01-31');
    });

    it('should return all dates for February 2026', () => {
      const result = getMonthDates(2026, 1); // February is month 1
      expect(result).toHaveLength(28); // 2026 is not a leap year
    });
  });

  describe('formatDateForDisplay', () => {
    it('should format date correctly', () => {
      const result = formatDateForDisplay('2026-02-03');
      expect(result).toMatch(/Feb/);
      expect(result).toMatch(/3/);
    });
  });
});
