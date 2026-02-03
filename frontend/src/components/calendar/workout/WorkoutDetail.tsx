/**
 * WorkoutDetail - Modal displaying detailed workout information
 */

import * as React from 'react';
import { Workout } from '@/types/workout';
import { Modal } from '@/components/ui/modal';
import { Button } from '@/components/ui/button';
import { DisciplineIcon } from './DisciplineIcon';
import { WorkoutStatusBadge } from './WorkoutStatusBadge';
import { getDisciplineClasses } from '../utils/discipline-colors';
import { cn } from '@/lib/utils';
import { Clock, Calendar, Target, Info, Play } from 'lucide-react';

export interface WorkoutDetailProps {
  workout: Workout | null;
  open: boolean;
  onClose: () => void;
  onStartWorkout?: (workoutId: string) => void;
}

export const WorkoutDetail: React.FC<WorkoutDetailProps> = ({
  workout,
  open,
  onClose,
  onStartWorkout,
}) => {
  if (!workout) return null;

  const disciplineClasses = getDisciplineClasses(workout.discipline);

  return (
    <Modal open={open} onClose={onClose} size="lg" title="Workout Details">
      <div className="space-y-4">
        {/* Header */}
        <div className={cn('p-4 rounded-lg border-l-4', disciplineClasses.bg, disciplineClasses.border)}>
          <div className="flex items-start justify-between gap-4">
            <div className="flex-1">
              <div className="flex items-center gap-2 mb-2">
                <DisciplineIcon discipline={workout.discipline} size={24} />
                <h2 className={cn('text-2xl font-bold', disciplineClasses.text)}>
                  {workout.name}
                </h2>
              </div>
              <p className="text-gray-700">{workout.description}</p>
            </div>
            <WorkoutStatusBadge status={workout.status} size="lg" showText />
          </div>
        </div>

        {/* Meta information */}
        <div className="grid grid-cols-2 gap-4">
          <div className="flex items-center gap-2 text-gray-700">
            <Calendar className="text-gray-500" size={18} />
            <div>
              <div className="text-xs text-gray-500">Scheduled Date</div>
              <div className="font-medium">
                {new Date(workout.scheduledDate).toLocaleDateString('en-US', {
                  weekday: 'long',
                  month: 'long',
                  day: 'numeric',
                  year: 'numeric',
                })}
              </div>
            </div>
          </div>

          <div className="flex items-center gap-2 text-gray-700">
            <Clock className="text-gray-500" size={18} />
            <div>
              <div className="text-xs text-gray-500">Duration</div>
              <div className="font-medium">{workout.duration} minutes</div>
            </div>
          </div>
        </div>

        {/* Focus Area */}
        {workout.focusArea && (
          <div className="flex items-start gap-2">
            <Target className="text-blue-500 mt-1" size={18} />
            <div className="flex-1">
              <div className="text-sm font-semibold text-gray-700">Focus Area</div>
              <div className="text-sm text-gray-600">{workout.focusArea}</div>
            </div>
          </div>
        )}

        {/* Rationale */}
        {workout.rationale && (
          <div className="flex items-start gap-2">
            <Info className="text-blue-500 mt-1" size={18} />
            <div className="flex-1">
              <div className="text-sm font-semibold text-gray-700">Rationale</div>
              <div className="text-sm text-gray-600">{workout.rationale}</div>
            </div>
          </div>
        )}

        {/* Exercises */}
        {workout.exercises.length > 0 && (
          <div>
            <h3 className="text-lg font-semibold mb-3 text-gray-900">Exercises</h3>
            <div className="space-y-2">
              {workout.exercises.map((exercise, index) => (
                <div
                  key={exercise.id}
                  className="p-3 bg-gray-50 rounded-lg border border-gray-200"
                >
                  <div className="flex items-start gap-2">
                    <span className="flex-shrink-0 w-6 h-6 bg-blue-500 text-white rounded-full flex items-center justify-center text-xs font-bold">
                      {index + 1}
                    </span>
                    <div className="flex-1">
                      <div className="font-medium text-gray-900">{exercise.name}</div>
                      <div className="text-sm text-gray-600 mt-1 space-x-3">
                        {exercise.sets && exercise.reps && (
                          <span>
                            {exercise.sets} × {exercise.reps} reps
                          </span>
                        )}
                        {exercise.duration && (
                          <span>{Math.floor(exercise.duration / 60)} min</span>
                        )}
                        {exercise.distance && <span>{exercise.distance}m</span>}
                      </div>
                      {exercise.notes && (
                        <div className="text-xs text-gray-500 mt-1 italic">{exercise.notes}</div>
                      )}
                    </div>
                  </div>
                </div>
              ))}
            </div>
          </div>
        )}

        {/* Action Buttons */}
        <div className="flex gap-3 pt-4 border-t">
          {onStartWorkout && workout.status !== 'COMPLETED' && (
            <Button
              onClick={() => onStartWorkout(workout.id)}
              className="flex-1"
              size="lg"
            >
              <Play className="mr-2" size={18} />
              Start Workout
            </Button>
          )}
          <Button onClick={onClose} variant="outline" className="flex-1" size="lg">
            Close
          </Button>
        </div>
      </div>
    </Modal>
  );
};
