/**
 * WorkoutCompletionHandler - Wrapper component that handles workout completion logic
 */

import * as React from 'react';
import { Workout, WorkoutStatus } from '@/types/workout';
import { WorkoutCompletionButton } from './WorkoutCompletionButton';
import { CompletionAnimation } from './CompletionAnimation';
import { UndoCompletionToast } from './UndoCompletionToast';
import { MilestoneAnimation } from './MilestoneAnimation';
import { MotivationalMessage } from './MotivationalMessage';
import { useWorkoutCompletion } from '@/hooks/progress/useWorkoutCompletion';
import { useStreakInfo } from '@/hooks/progress/useProgressStats';

export interface WorkoutCompletionHandlerProps {
  workout: Workout;
  onWorkoutUpdated?: (workout: Workout) => void;
  className?: string;
}

export const WorkoutCompletionHandler: React.FC<WorkoutCompletionHandlerProps> = ({
  workout,
  onWorkoutUpdated,
  className = '',
}) => {
  const [showCompletionAnimation, setShowCompletionAnimation] = React.useState(false);
  const [showUndoToast, setShowUndoToast] = React.useState(false);
  const [showMilestoneAnimation, setShowMilestoneAnimation] = React.useState(false);
  const [milestoneValue, setMilestoneValue] = React.useState(0);
  const [motivationalType, setMotivationalType] = React.useState<
    'completion' | 'streak' | 'milestone' | null
  >(null);

  const { streakInfo, refetch: refetchStreak } = useStreakInfo();
  
  // Check for reduced motion preference
  const prefersReducedMotion = React.useMemo(
    () =>
      typeof window !== 'undefined' &&
      window.matchMedia('(prefers-reduced-motion: reduce)').matches,
    []
  );

  const { isCompleting, isUndoing, completeWorkout, undoCompletion } = useWorkoutCompletion({
    onSuccess: async (updatedWorkout) => {
      onWorkoutUpdated?.(updatedWorkout);
      
      // Refetch streak info to get updated data
      await refetchStreak();
      
      if (updatedWorkout.status === WorkoutStatus.COMPLETED) {
        // Show completion animation
        setShowCompletionAnimation(true);
        setMotivationalType('completion');
        
        // Check for milestone after a delay
        setTimeout(() => {
          if (streakInfo) {
            const milestones = [7, 14, 30, 60, 90, 180, 365];
            const currentStreak = streakInfo.currentStreak;
            
            if (milestones.includes(currentStreak)) {
              setMilestoneValue(currentStreak);
              setShowMilestoneAnimation(true);
              setMotivationalType('milestone');
            } else if (currentStreak > 0) {
              setMotivationalType('streak');
            }
          }
        }, 2500);
      }
    },
    onError: (error) => {
      console.error('Failed to complete workout:', error);
      // TODO: Show error toast
    },
  });

  const handleComplete = async () => {
    await completeWorkout(workout.id);
    setShowUndoToast(true);
  };

  const handleUndo = async () => {
    setShowUndoToast(false);
    await undoCompletion(workout.id);
    setMotivationalType(null);
  };

  const handleCompletionAnimationComplete = () => {
    setShowCompletionAnimation(false);
  };

  const handleMilestoneAnimationComplete = () => {
    setShowMilestoneAnimation(false);
  };

  const handleToastDismiss = () => {
    setShowUndoToast(false);
  };

  const isCompleted = workout.status === WorkoutStatus.COMPLETED;
  const isFutureWorkout = new Date(workout.scheduledDate) > new Date();

  return (
    <div className={className}>
      {/* Completion button */}
      <WorkoutCompletionButton
        isCompleted={isCompleted}
        isLoading={isCompleting || isUndoing}
        onComplete={handleComplete}
        onUndo={handleUndo}
        showUndo={showUndoToast}
        disabled={isFutureWorkout}
      />

      {/* Motivational message */}
      {motivationalType && streakInfo && (
        <div className="mt-4">
          <MotivationalMessage
            type={motivationalType}
            data={{
              streakDays: streakInfo.currentStreak,
              milestone: milestoneValue,
            }}
          />
        </div>
      )}

      {/* Completion animation */}
      <CompletionAnimation
        show={showCompletionAnimation}
        onComplete={handleCompletionAnimationComplete}
        reducedMotion={prefersReducedMotion}
      />

      {/* Undo toast */}
      <UndoCompletionToast
        show={showUndoToast}
        onUndo={handleUndo}
        onDismiss={handleToastDismiss}
      />

      {/* Milestone animation */}
      <MilestoneAnimation
        show={showMilestoneAnimation}
        milestone={milestoneValue}
        onComplete={handleMilestoneAnimationComplete}
        reducedMotion={prefersReducedMotion}
      />
    </div>
  );
};
