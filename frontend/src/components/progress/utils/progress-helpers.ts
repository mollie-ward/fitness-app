/**
 * Helper functions for progress tracking
 */

import type { CompletionHistoryDto, CompletionHeatmapData } from '@/types/progress';
import { Discipline } from '@/types/discipline';

/**
 * Transform completion history into heatmap data grouped by date
 */
export function transformToHeatmapData(
  history: CompletionHistoryDto[]
): CompletionHeatmapData[] {
  const dateMap = new Map<string, CompletionHistoryDto[]>();

  // Group workouts by date
  history.forEach((workout) => {
    const date = new Date(workout.completedAt).toISOString().split('T')[0];
    const existing = dateMap.get(date) || [];
    dateMap.set(date, [...existing, workout]);
  });

  // Convert to array of heatmap data
  return Array.from(dateMap.entries()).map(([date, workouts]) => ({
    date,
    count: workouts.length,
    workouts,
  }));
}

/**
 * Filter history by discipline
 */
export function filterHistoryByDiscipline(
  history: CompletionHistoryDto[],
  discipline: Discipline | 'all'
): CompletionHistoryDto[] {
  if (discipline === 'all') {
    return history;
  }
  return history.filter((workout) => workout.discipline === discipline);
}

/**
 * Get intensity color based on workout count
 */
export function getHeatmapIntensityColor(count: number): string {
  if (count === 0) return 'bg-gray-100';
  if (count === 1) return 'bg-green-200';
  if (count === 2) return 'bg-green-400';
  if (count === 3) return 'bg-green-600';
  return 'bg-green-800';
}

/**
 * Generate date range for heatmap
 */
export function generateDateRange(startDate: Date, endDate: Date): string[] {
  const dates: string[] = [];
  const current = new Date(startDate);

  while (current <= endDate) {
    dates.push(current.toISOString().split('T')[0]);
    current.setDate(current.getDate() + 1);
  }

  return dates;
}

/**
 * Get dates for a specific month
 */
export function getMonthDates(year: number, month: number): string[] {
  const startDate = new Date(year, month, 1);
  const endDate = new Date(year, month + 1, 0);
  return generateDateRange(startDate, endDate);
}

/**
 * Format date for display
 */
export function formatDateForDisplay(dateString: string): string {
  const date = new Date(dateString);
  return date.toLocaleDateString('en-US', {
    weekday: 'short',
    month: 'short',
    day: 'numeric',
  });
}
