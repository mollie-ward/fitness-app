/**
 * HeatmapDayDetail - Modal showing workouts completed on a specific day
 */

import * as React from 'react';
import { Modal } from '@/components/ui/modal';
import type { CompletionHistoryDto } from '@/types/progress';
import { formatDateForDisplay } from './utils/progress-helpers';
import { getDisciplineClasses } from '@/components/calendar/utils/discipline-colors';
import { Clock, FileText } from 'lucide-react';

export interface HeatmapDayDetailProps {
  isOpen: boolean;
  onClose: () => void;
  date: string;
  workouts: CompletionHistoryDto[];
}

export const HeatmapDayDetail: React.FC<HeatmapDayDetailProps> = ({
  isOpen,
  onClose,
  date,
  workouts,
}) => {
  if (!isOpen) return null;

  return (
    <Modal
      open={isOpen}
      onClose={onClose}
      title={formatDateForDisplay(date)}
      size="md"
    >
      <div className="space-y-4">
        {workouts.length === 0 ? (
          <p className="text-center text-gray-500 py-8">
            No workouts completed on this day
          </p>
        ) : (
          <>
            <p className="text-sm text-gray-600">
              {workouts.length} workout{workouts.length !== 1 ? 's' : ''} completed
            </p>

            <div className="space-y-3">
              {workouts.map((workout) => {
                const disciplineClasses = getDisciplineClasses(workout.discipline);

                return (
                  <div
                    key={workout.workoutId}
                    className={`p-4 rounded-lg border-l-4 bg-gray-50 ${disciplineClasses.border}`}
                  >
                    <h4 className={`font-semibold mb-2 ${disciplineClasses.text}`}>
                      {workout.workoutName}
                    </h4>

                    <div className="space-y-1 text-sm text-gray-600">
                      {workout.duration && (
                        <div className="flex items-center gap-2">
                          <Clock className="h-3 w-3" aria-hidden="true" />
                          <span>{workout.duration} minutes</span>
                        </div>
                      )}

                      {workout.notes && (
                        <div className="flex items-start gap-2">
                          <FileText className="h-3 w-3 mt-0.5" aria-hidden="true" />
                          <span className="flex-1">{workout.notes}</span>
                        </div>
                      )}

                      <div className="text-xs text-gray-500 mt-2">
                        Completed at{' '}
                        {new Date(workout.completedAt).toLocaleTimeString('en-US', {
                          hour: 'numeric',
                          minute: '2-digit',
                        })}
                      </div>
                    </div>
                  </div>
                );
              })}
            </div>
          </>
        )}
      </div>
    </Modal>
  );
};
