/**
 * Hook for managing workout completion with optimistic updates
 */

import * as React from 'react';
import { Workout, WorkoutStatus } from '@/types/workout';
import { WorkoutCompletionDto } from '@/types/progress';
import { completeWorkout, undoWorkoutCompletion } from '@/services/progress-api';
import useProgressStore from '@/lib/progress-store';

interface UseWorkoutCompletionOptions {
  onSuccess?: (workout: Workout) => void;
  onError?: (error: Error) => void;
}

interface UseWorkoutCompletionReturn {
  isCompleting: boolean;
  isUndoing: boolean;
  error: Error | null;
  completeWorkout: (workoutId: string, completionData?: Partial<WorkoutCompletionDto>) => Promise<void>;
  undoCompletion: (workoutId: string) => Promise<void>;
  clearError: () => void;
}

export function useWorkoutCompletion(
  options: UseWorkoutCompletionOptions = {}
): UseWorkoutCompletionReturn {
  const { onSuccess, onError } = options;
  const [isCompleting, setIsCompleting] = React.useState(false);
  const [isUndoing, setIsUndoing] = React.useState(false);
  const [error, setError] = React.useState<Error | null>(null);
  
  const invalidateAll = useProgressStore((state) => state.invalidateAll);

  const handleCompleteWorkout = React.useCallback(
    async (workoutId: string, completionData?: Partial<WorkoutCompletionDto>) => {
      setIsCompleting(true);
      setError(null);

      try {
        const completionDto: WorkoutCompletionDto = {
          completedAt: completionData?.completedAt || new Date().toISOString(),
          duration: completionData?.duration,
          notes: completionData?.notes,
        };

        const updatedWorkout = await completeWorkout(workoutId, completionDto);
        
        // Invalidate all cached progress data to trigger refetch
        invalidateAll();
        
        onSuccess?.(updatedWorkout);
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to complete workout');
        setError(error);
        onError?.(error);
      } finally {
        setIsCompleting(false);
      }
    },
    [onSuccess, onError, invalidateAll]
  );

  const handleUndoCompletion = React.useCallback(
    async (workoutId: string) => {
      setIsUndoing(true);
      setError(null);

      try {
        const updatedWorkout = await undoWorkoutCompletion(workoutId);
        
        // Invalidate all cached progress data to trigger refetch
        invalidateAll();
        
        onSuccess?.(updatedWorkout);
      } catch (err) {
        const error = err instanceof Error ? err : new Error('Failed to undo completion');
        setError(error);
        onError?.(error);
      } finally {
        setIsUndoing(false);
      }
    },
    [onSuccess, onError, invalidateAll]
  );

  const clearError = React.useCallback(() => {
    setError(null);
  }, []);

  return {
    isCompleting,
    isUndoing,
    error,
    completeWorkout: handleCompleteWorkout,
    undoCompletion: handleUndoCompletion,
    clearError,
  };
}
