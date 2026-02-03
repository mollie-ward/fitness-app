/**
 * Progress tracking types
 */

import { Discipline } from './discipline';

/**
 * DTO for workout completion request
 */
export interface WorkoutCompletionDto {
  completedAt: string; // ISO 8601 datetime string
  duration?: number; // in minutes
  notes?: string;
}

/**
 * DTO for completion statistics (weekly or monthly)
 */
export interface CompletionStatsDto {
  completedCount: number;
  totalScheduled: number;
  completionPercentage: number;
  periodStart: string; // ISO 8601 datetime string
  periodEnd: string; // ISO 8601 datetime string
}

/**
 * DTO for overall all-time statistics
 */
export interface OverallStatsDto {
  totalTrainingDays: number;
  totalWorkoutsCompleted: number;
  overallPlanCompletionPercentage: number;
  averageWeeklyCompletionRate: number;
  workoutsCompletedThisWeek: number;
  workoutsCompletedThisMonth: number;
  firstWorkoutDate?: string; // ISO 8601 datetime string
  lastWorkoutDate?: string; // ISO 8601 datetime string
}

/**
 * DTO for streak information
 */
export interface StreakInfoDto {
  currentStreak: number;
  longestStreak: number;
  currentWeeklyStreak: number;
  longestWeeklyStreak: number;
  lastWorkoutDate?: string; // ISO 8601 datetime string
  daysUntilNextMilestone: number;
  nextMilestone: number;
}

/**
 * DTO for historical completion data
 */
export interface CompletionHistoryDto {
  completedAt: string; // ISO 8601 datetime string
  workoutId: string;
  workoutName: string;
  discipline: Discipline;
  duration?: number; // in minutes
  notes?: string;
}

/**
 * Grouped history data by date for heatmap visualization
 */
export interface CompletionHeatmapData {
  date: string; // YYYY-MM-DD format
  count: number;
  workouts: CompletionHistoryDto[];
}
