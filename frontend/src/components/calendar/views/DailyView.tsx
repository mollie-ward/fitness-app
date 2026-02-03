/**
 * DailyView - Display today's workout and tomorrow's preview
 */

import * as React from 'react';
import { Workout, WorkoutStatus } from '@/types/workout';
import { Card, CardContent, CardHeader, CardTitle } from '@/components/ui/card';
import { Button } from '@/components/ui/button';
import { WorkoutCard } from '../workout/WorkoutCard';
import { Skeleton } from '@/components/ui/skeleton';
import { getTodaysWorkout, getTomorrowsWorkout, countWorkoutsByStatus } from '../utils/workout-filters';
import { getWeekStart, getWeekEnd } from '../utils/calendar-helpers';
import { getCurrentDate } from '../utils/calendar-helpers';
import { CalendarOff, TrendingUp } from 'lucide-react';

export interface DailyViewProps {
  workouts: Workout[];
  isLoading?: boolean;
  onWorkoutClick?: (workout: Workout) => void;
  onStartWorkout?: (workoutId: string) => void;
}

export const DailyView: React.FC<DailyViewProps> = ({
  workouts,
  isLoading = false,
  onWorkoutClick,
  onStartWorkout,
}) => {
  const today = getCurrentDate();
  const weekStart = getWeekStart(today);
  const weekEnd = getWeekEnd(today);

  const todaysWorkout = getTodaysWorkout(workouts);
  const tomorrowsWorkout = getTomorrowsWorkout(workouts);

  // Get workouts for current week
  const weekWorkouts = workouts.filter((w) => {
    const workoutDate = new Date(w.scheduledDate);
    const weekStartDate = new Date(weekStart.year, weekStart.month - 1, weekStart.day);
    const weekEndDate = new Date(weekEnd.year, weekEnd.month - 1, weekEnd.day);
    return workoutDate >= weekStartDate && workoutDate <= weekEndDate;
  });

  const weekStats = countWorkoutsByStatus(weekWorkouts);
  const completedCount = weekStats[WorkoutStatus.COMPLETED];
  const totalCount = weekWorkouts.length;

  if (isLoading) {
    return (
      <div className="space-y-6">
        <Skeleton className="h-64 w-full" />
        <Skeleton className="h-32 w-full" />
        <Skeleton className="h-24 w-full" />
      </div>
    );
  }

  return (
    <div className="space-y-6">
      {/* Today's Workout */}
      <Card className="shadow-lg">
        <CardHeader>
          <CardTitle className="text-2xl">Today's Workout</CardTitle>
          <p className="text-sm text-gray-500">
            {today.month}/{today.day}/{today.year}
          </p>
        </CardHeader>
        <CardContent>
          {todaysWorkout ? (
            <div className="space-y-4">
              <WorkoutCard
                workout={todaysWorkout}
                onClick={() => onWorkoutClick?.(todaysWorkout)}
              />
              {todaysWorkout.status !== WorkoutStatus.COMPLETED && onStartWorkout && (
                <Button
                  className="w-full"
                  size="lg"
                  onClick={() => onStartWorkout(todaysWorkout.id)}
                >
                  Start Workout
                </Button>
              )}
            </div>
          ) : (
            <div className="flex flex-col items-center justify-center py-8 text-gray-500">
              <CalendarOff size={48} className="mb-3 text-gray-400" />
              <p className="text-lg font-medium">No workout scheduled for today</p>
              <p className="text-sm">Enjoy your rest day!</p>
            </div>
          )}
        </CardContent>
      </Card>

      {/* Week Progress */}
      <Card>
        <CardHeader>
          <CardTitle className="flex items-center gap-2">
            <TrendingUp size={20} />
            This Week's Progress
          </CardTitle>
        </CardHeader>
        <CardContent>
          <div className="space-y-3">
            <div className="flex justify-between items-center">
              <span className="text-lg font-semibold text-gray-700">
                {completedCount} of {totalCount} workouts completed
              </span>
              <span className="text-2xl font-bold text-blue-600">
                {totalCount > 0 ? Math.round((completedCount / totalCount) * 100) : 0}%
              </span>
            </div>
            <div className="w-full bg-gray-200 rounded-full h-3">
              <div
                className="bg-blue-600 h-3 rounded-full transition-all duration-300"
                style={{
                  width: `${totalCount > 0 ? (completedCount / totalCount) * 100 : 0}%`,
                }}
                role="progressbar"
                aria-valuenow={completedCount}
                aria-valuemin={0}
                aria-valuemax={totalCount}
                aria-label={`${completedCount} of ${totalCount} workouts completed`}
              />
            </div>
          </div>
        </CardContent>
      </Card>

      {/* Tomorrow's Preview */}
      {tomorrowsWorkout && (
        <Card>
          <CardHeader>
            <CardTitle>Tomorrow's Workout</CardTitle>
          </CardHeader>
          <CardContent>
            <WorkoutCard
              workout={tomorrowsWorkout}
              onClick={() => onWorkoutClick?.(tomorrowsWorkout)}
              compact
            />
          </CardContent>
        </Card>
      )}
    </div>
  );
};
