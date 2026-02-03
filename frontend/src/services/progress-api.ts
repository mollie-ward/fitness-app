/**
 * API service for progress tracking
 */
import apiClient from '@/lib/api/api-client';
import type {
  WorkoutCompletionDto,
  CompletionStatsDto,
  OverallStatsDto,
  StreakInfoDto,
  CompletionHistoryDto,
} from '@/types/progress';
import type { Workout } from '@/types/workout';

/**
 * Mark a workout as complete
 */
export async function completeWorkout(
  workoutId: string,
  completionData: WorkoutCompletionDto
): Promise<Workout> {
  const response = await apiClient.put<Workout>(
    `/progress/workouts/${workoutId}/complete`,
    completionData
  );
  return response.data;
}

/**
 * Undo workout completion
 */
export async function undoWorkoutCompletion(workoutId: string): Promise<Workout> {
  const response = await apiClient.put<Workout>(`/progress/workouts/${workoutId}/undo`);
  return response.data;
}

/**
 * Get weekly completion statistics
 */
export async function getWeeklyStats(): Promise<CompletionStatsDto> {
  const response = await apiClient.get<CompletionStatsDto>('/progress/stats/weekly');
  return response.data;
}

/**
 * Get monthly completion statistics
 */
export async function getMonthlyStats(): Promise<CompletionStatsDto> {
  const response = await apiClient.get<CompletionStatsDto>('/progress/stats/monthly');
  return response.data;
}

/**
 * Get overall all-time statistics
 */
export async function getOverallStats(): Promise<OverallStatsDto> {
  const response = await apiClient.get<OverallStatsDto>('/progress/stats/overall');
  return response.data;
}

/**
 * Get current streak information
 */
export async function getStreakInfo(): Promise<StreakInfoDto> {
  const response = await apiClient.get<StreakInfoDto>('/progress/streak');
  return response.data;
}

/**
 * Get historical completion data
 */
export async function getCompletionHistory(
  startDate?: Date,
  endDate?: Date
): Promise<CompletionHistoryDto[]> {
  const params = new URLSearchParams();
  if (startDate) {
    params.append('startDate', startDate.toISOString());
  }
  if (endDate) {
    params.append('endDate', endDate.toISOString());
  }

  const response = await apiClient.get<CompletionHistoryDto[]>(
    `/progress/history${params.toString() ? `?${params.toString()}` : ''}`
  );
  return response.data;
}
