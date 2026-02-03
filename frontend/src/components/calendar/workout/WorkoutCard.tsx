/**
 * WorkoutCard - Display individual workout information
 */

import * as React from 'react';
import { Workout } from '@/types/workout';
import { Card } from '@/components/ui/card';
import { DisciplineIcon } from './DisciplineIcon';
import { WorkoutStatusBadge } from './WorkoutStatusBadge';
import { getDisciplineClasses } from '../utils/discipline-colors';
import { cn } from '@/lib/utils';
import { Clock, Star } from 'lucide-react';

export interface WorkoutCardProps {
  workout: Workout;
  onClick?: () => void;
  compact?: boolean;
  showDate?: boolean;
  className?: string;
}

export const WorkoutCard: React.FC<WorkoutCardProps> = ({
  workout,
  onClick,
  compact = false,
  showDate = false,
  className = '',
}) => {
  const disciplineClasses = getDisciplineClasses(workout.discipline);

  return (
    <Card
      className={cn(
        'cursor-pointer transition-all hover:shadow-md border-l-4',
        disciplineClasses.border,
        onClick && 'hover:scale-[1.02]',
        className
      )}
      onClick={onClick}
      role="button"
      tabIndex={0}
      onKeyDown={(e) => {
        if (e.key === 'Enter' || e.key === ' ') {
          e.preventDefault();
          onClick?.();
        }
      }}
      aria-label={`${workout.name} workout on ${workout.scheduledDate}`}
    >
      <div className={cn('p-4', compact && 'p-3')}>
        <div className="flex items-start justify-between gap-2">
          <div className="flex-1 min-w-0">
            <div className="flex items-center gap-2 mb-1">
              <DisciplineIcon discipline={workout.discipline} size={compact ? 16 : 18} />
              <h3
                className={cn(
                  'font-semibold truncate',
                  compact ? 'text-sm' : 'text-base',
                  disciplineClasses.text
                )}
              >
                {workout.name}
                {workout.isKeyWorkout && (
                  <Star
                    className="inline-block ml-1 text-yellow-500 fill-yellow-500"
                    size={14}
                    aria-label="Key workout"
                  />
                )}
              </h3>
            </div>

            {!compact && (
              <p className="text-sm text-gray-600 line-clamp-2 mb-2">{workout.description}</p>
            )}

            <div className="flex items-center gap-3 text-sm text-gray-500">
              <span className="flex items-center gap-1">
                <Clock size={14} />
                {workout.duration} min
              </span>
              {workout.focusArea && !compact && (
                <span className="text-xs text-gray-500 italic">• {workout.focusArea}</span>
              )}
            </div>
          </div>

          <WorkoutStatusBadge status={workout.status} size={compact ? 'sm' : 'md'} />
        </div>

        {showDate && (
          <div className="mt-2 pt-2 border-t text-xs text-gray-500">
            {new Date(workout.scheduledDate).toLocaleDateString('en-US', {
              weekday: 'long',
              month: 'short',
              day: 'numeric',
            })}
          </div>
        )}
      </div>
    </Card>
  );
};
