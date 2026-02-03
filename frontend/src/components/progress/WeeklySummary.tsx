/**
 * WeeklySummary - Display weekly completion statistics
 */

import * as React from 'react';
import { Card } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useWeeklyStats } from '@/hooks/progress/useProgressStats';
import { cn } from '@/lib/utils';
import { Calendar, TrendingUp } from 'lucide-react';

export const WeeklySummary: React.FC = () => {
  const { weeklyStats, isLoading } = useWeeklyStats();

  if (isLoading) {
    return (
      <Card className="p-6">
        <Skeleton className="h-6 w-48 mb-4" />
        <Skeleton className="h-32 w-full mb-4" />
        <Skeleton className="h-4 w-64" />
      </Card>
    );
  }

  if (!weeklyStats) {
    return (
      <Card className="p-6">
        <p className="text-center text-gray-500">No weekly data available</p>
      </Card>
    );
  }

  const { completedCount, totalScheduled, completionPercentage } = weeklyStats;
  const daysRemaining = 7 - new Date().getDay();
  const isOnTrack = completionPercentage >= 75;

  return (
    <Card className="p-6">
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <Calendar className="h-5 w-5 text-blue-600" aria-hidden="true" />
          <h3 className="text-lg font-semibold text-gray-900">This Week</h3>
        </div>
        {isOnTrack && (
          <TrendingUp className="h-5 w-5 text-green-600" aria-label="On track" />
        )}
      </div>

      {/* Progress text */}
      <p className="text-2xl font-bold text-gray-900 mb-4">
        {completedCount} of {totalScheduled} workouts completed
      </p>

      {/* Progress bar */}
      <div
        className="relative h-3 w-full overflow-hidden rounded-full bg-gray-200 mb-4"
        role="progressbar"
        aria-valuenow={completionPercentage}
        aria-valuemin={0}
        aria-valuemax={100}
        aria-label={`${completionPercentage.toFixed(0)}% of weekly workouts completed`}
      >
        <div
          className={cn(
            'h-full transition-all duration-500 rounded-full',
            completionPercentage >= 100
              ? 'bg-green-600'
              : completionPercentage >= 75
                ? 'bg-blue-600'
                : completionPercentage >= 50
                  ? 'bg-yellow-600'
                  : 'bg-red-600'
          )}
          style={{ width: `${Math.min(completionPercentage, 100)}%` }}
        />
      </div>

      {/* Additional info */}
      <div className="flex items-center justify-between text-sm text-gray-600">
        <span>{completionPercentage.toFixed(0)}% complete</span>
        <span>{daysRemaining} {daysRemaining === 1 ? 'day' : 'days'} remaining</span>
      </div>

      {/* Status message */}
      <div className="mt-4 pt-4 border-t">
        <p
          className={cn(
            'text-sm font-medium',
            isOnTrack ? 'text-green-700' : 'text-orange-700'
          )}
        >
          {isOnTrack
            ? "You're on track! Keep up the great work."
            : `Complete ${totalScheduled - completedCount} more workout${totalScheduled - completedCount !== 1 ? 's' : ''} to reach your target.`}
        </p>
      </div>
    </Card>
  );
};
