/**
 * WorkoutCompletionButton - Button to mark workout as complete
 */

import * as React from 'react';
import { Button } from '@/components/ui/button';
import { Check, Loader2, Undo2 } from 'lucide-react';
import { cn } from '@/lib/utils';

export interface WorkoutCompletionButtonProps {
  isCompleted: boolean;
  isLoading?: boolean;
  onComplete: () => void;
  onUndo?: () => void;
  showUndo?: boolean;
  disabled?: boolean;
  className?: string;
}

export const WorkoutCompletionButton: React.FC<WorkoutCompletionButtonProps> = ({
  isCompleted,
  isLoading = false,
  onComplete,
  onUndo,
  showUndo = false,
  disabled = false,
  className = '',
}) => {
  const handleClick = () => {
    if (isLoading || disabled) return;
    
    if (isCompleted && showUndo && onUndo) {
      onUndo();
    } else if (!isCompleted) {
      onComplete();
    }
  };

  const handleKeyDown = (e: React.KeyboardEvent) => {
    if (e.key === 'Enter' || e.key === ' ') {
      e.preventDefault();
      handleClick();
    }
  };

  return (
    <Button
      onClick={handleClick}
      onKeyDown={handleKeyDown}
      disabled={disabled || isLoading}
      className={cn(
        'min-w-[140px] transition-all',
        isCompleted && !showUndo
          ? 'bg-green-600 hover:bg-green-700'
          : isCompleted && showUndo
            ? 'bg-orange-600 hover:bg-orange-700'
            : 'bg-blue-600 hover:bg-blue-700',
        className
      )}
      aria-label={
        isLoading
          ? 'Processing...'
          : isCompleted && showUndo
            ? 'Undo completion'
            : isCompleted
              ? 'Workout completed'
              : 'Mark workout complete'
      }
      aria-live="polite"
      aria-busy={isLoading}
    >
      {isLoading ? (
        <>
          <Loader2 className="mr-2 h-4 w-4 animate-spin" aria-hidden="true" />
          Processing...
        </>
      ) : isCompleted && showUndo ? (
        <>
          <Undo2 className="mr-2 h-4 w-4" aria-hidden="true" />
          Undo
        </>
      ) : isCompleted ? (
        <>
          <Check className="mr-2 h-4 w-4" aria-hidden="true" />
          Completed
        </>
      ) : (
        <>
          <Check className="mr-2 h-4 w-4" aria-hidden="true" />
          Mark Complete
        </>
      )}
    </Button>
  );
};
