/**
 * MonthlyView - Display calendar grid for the month
 */

import * as React from 'react';
import { Workout } from '@/types/workout';
import { CalendarDate } from '@/types/calendar';
import { Skeleton } from '@/components/ui/skeleton';
import { DisciplineIcon } from '../workout/DisciplineIcon';
import { WorkoutStatusBadge } from '../workout/WorkoutStatusBadge';
import { getWorkoutForDate, calculateCompletionPercentage } from '../utils/workout-filters';
import { getMonthDays, isToday, formatDayOfWeek } from '../utils/calendar-helpers';
import { cn } from '@/lib/utils';

export interface MonthlyViewProps {
  workouts: Workout[];
  selectedDate: CalendarDate;
  isLoading?: boolean;
  onWorkoutClick?: (workout: Workout) => void;
  onDateClick?: (date: CalendarDate) => void;
}

export const MonthlyView: React.FC<MonthlyViewProps> = ({
  workouts,
  selectedDate,
  isLoading = false,
  onWorkoutClick,
  onDateClick,
}) => {
  const monthDays = getMonthDays(selectedDate);
  const monthWorkouts = workouts.filter((w) => {
    const workoutDate = new Date(w.scheduledDate);
    return (
      workoutDate.getFullYear() === selectedDate.year &&
      workoutDate.getMonth() + 1 === selectedDate.month
    );
  });
  const completionPercentage = calculateCompletionPercentage(monthWorkouts);

  const weekDayHeaders = ['Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat', 'Sun'];

  if (isLoading) {
    return <Skeleton className="h-96 w-full" />;
  }

  return (
    <div>
      {/* Month Summary */}
      <div className="mb-4 p-4 bg-blue-50 rounded-lg border border-blue-200">
        <div className="flex items-center justify-between">
          <div>
            <h3 className="text-lg font-semibold text-gray-900">
              {new Date(selectedDate.year, selectedDate.month - 1).toLocaleDateString('en-US', {
                month: 'long',
                year: 'numeric',
              })}
            </h3>
            <p className="text-sm text-gray-600">
              {monthWorkouts.length} workouts scheduled
            </p>
          </div>
          <div className="text-right">
            <div className="text-2xl font-bold text-blue-600">{completionPercentage}%</div>
            <div className="text-xs text-gray-600">Complete</div>
          </div>
        </div>
      </div>

      {/* Calendar Grid */}
      <div className="bg-white rounded-lg border border-gray-200 overflow-hidden">
        {/* Day Headers */}
        <div className="grid grid-cols-7 bg-gray-50 border-b border-gray-200">
          {weekDayHeaders.map((day) => (
            <div
              key={day}
              className="py-2 text-center text-xs font-semibold text-gray-600 uppercase"
            >
              {day}
            </div>
          ))}
        </div>

        {/* Calendar Days */}
        <div className="grid grid-cols-7">
          {monthDays.map((date, index) => {
            const workout = getWorkoutForDate(workouts, date);
            const isTodayDate = isToday(date);
            const isCurrentMonth = date.month === selectedDate.month;
            const isOtherMonth = !isCurrentMonth;

            return (
              <button
                key={`${date.year}-${date.month}-${date.day}-${index}`}
                onClick={() => {
                  if (workout) {
                    onWorkoutClick?.(workout);
                  } else {
                    onDateClick?.(date);
                  }
                }}
                className={cn(
                  'min-h-24 p-2 border-b border-r border-gray-200 hover:bg-gray-50 transition-colors focus:outline-none focus:ring-2 focus:ring-blue-500 focus:z-10',
                  isTodayDate && 'bg-blue-50 ring-1 ring-blue-500',
                  isOtherMonth && 'bg-gray-50/50 text-gray-400',
                  (index + 1) % 7 === 0 && 'border-r-0'
                )}
                aria-label={
                  workout
                    ? `${workout.name} on ${formatDayOfWeek(date, 'long')}, ${date.month}/${date.day}`
                    : `${formatDayOfWeek(date, 'long')}, ${date.month}/${date.day}`
                }
              >
                <div className="flex flex-col h-full">
                  {/* Date Number */}
                  <div className="flex items-center justify-between mb-1">
                    <span
                      className={cn(
                        'text-sm font-medium',
                        isTodayDate
                          ? 'w-6 h-6 bg-blue-600 text-white rounded-full flex items-center justify-center'
                          : isOtherMonth
                            ? 'text-gray-400'
                            : 'text-gray-700'
                      )}
                    >
                      {date.day}
                    </span>
                    {workout && (
                      <WorkoutStatusBadge status={workout.status} size="sm" />
                    )}
                  </div>

                  {/* Workout Info */}
                  {workout && (
                    <div className="flex-1 flex flex-col items-start gap-1">
                      <div className="flex items-center gap-1">
                        <DisciplineIcon discipline={workout.discipline} size={12} />
                        <span className="text-xs font-medium truncate" title={workout.name}>
                          {workout.name.length > 15
                            ? `${workout.name.substring(0, 15)}...`
                            : workout.name}
                        </span>
                      </div>
                      {workout.isKeyWorkout && (
                        <span className="text-xs px-1.5 py-0.5 bg-yellow-100 text-yellow-700 rounded">
                          Key
                        </span>
                      )}
                    </div>
                  )}
                </div>
              </button>
            );
          })}
        </div>
      </div>
    </div>
  );
};
