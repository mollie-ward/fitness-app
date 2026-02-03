/**
 * WeeklyView - Display 7-day week view of workouts
 */

import * as React from 'react';
import { Workout } from '@/types/workout';
import { CalendarDate } from '@/types/calendar';
import { Card } from '@/components/ui/card';
import { WorkoutCard } from '../workout/WorkoutCard';
import { Skeleton } from '@/components/ui/skeleton';
import { getWorkoutForDate } from '../utils/workout-filters';
import { formatDayOfWeek, isToday, isPast, getWeekDays } from '../utils/calendar-helpers';
import { cn } from '@/lib/utils';
import { CalendarOff } from 'lucide-react';

export interface WeeklyViewProps {
  workouts: Workout[];
  selectedDate: CalendarDate;
  isLoading?: boolean;
  onWorkoutClick?: (workout: Workout) => void;
  onDateClick?: (date: CalendarDate) => void;
}

export const WeeklyView: React.FC<WeeklyViewProps> = ({
  workouts,
  selectedDate,
  isLoading = false,
  onWorkoutClick,
  onDateClick,
}) => {
  const weekDays = getWeekDays(selectedDate);

  if (isLoading) {
    return (
      <div className="space-y-4">
        {[...Array(7)].map((_, i) => (
          <Skeleton key={i} className="h-32 w-full" />
        ))}
      </div>
    );
  }

  return (
    <div className="space-y-4">
      {weekDays.map((date) => {
        const workout = getWorkoutForDate(workouts, date);
        const isTodayDate = isToday(date);
        const isPastDate = isPast(date);

        return (
          <Card
            key={`${date.year}-${date.month}-${date.day}`}
            className={cn(
              'transition-all',
              isTodayDate && 'ring-2 ring-blue-500 shadow-md',
              isPastDate && !isTodayDate && 'opacity-75'
            )}
          >
            <div className="p-4">
              {/* Date Header */}
              <div className="flex items-center justify-between mb-3">
                <button
                  onClick={() => onDateClick?.(date)}
                  className={cn(
                    'flex items-center gap-3 hover:bg-gray-50 rounded-lg p-2 -m-2 transition-colors',
                    isTodayDate && 'font-bold'
                  )}
                >
                  <div
                    className={cn(
                      'w-12 h-12 rounded-lg flex flex-col items-center justify-center',
                      isTodayDate ? 'bg-blue-600 text-white' : 'bg-gray-100 text-gray-700'
                    )}
                  >
                    <span className="text-xs uppercase">
                      {formatDayOfWeek(date, 'short')}
                    </span>
                    <span className="text-lg font-bold">{date.day}</span>
                  </div>
                  <div className="text-left">
                    <div className={cn('text-sm', isTodayDate ? 'font-semibold' : 'text-gray-600')}>
                      {formatDayOfWeek(date, 'long')}
                    </div>
                    <div className="text-xs text-gray-500">
                      {new Date(date.year, date.month - 1, date.day).toLocaleDateString('en-US', {
                        month: 'long',
                        day: 'numeric',
                      })}
                    </div>
                  </div>
                </button>
                {isTodayDate && (
                  <span className="px-3 py-1 bg-blue-100 text-blue-700 text-xs font-semibold rounded-full">
                    Today
                  </span>
                )}
              </div>

              {/* Workout Content */}
              {workout ? (
                <WorkoutCard
                  workout={workout}
                  onClick={() => onWorkoutClick?.(workout)}
                  compact
                />
              ) : (
                <div className="flex items-center justify-center py-6 text-gray-400">
                  <CalendarOff size={20} className="mr-2" />
                  <span className="text-sm">Rest Day</span>
                </div>
              )}
            </div>
          </Card>
        );
      })}
    </div>
  );
};
