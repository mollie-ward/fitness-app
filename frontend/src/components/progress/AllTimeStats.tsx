/**
 * AllTimeStats - Display all-time statistics
 */

import * as React from 'react';
import { Card } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useOverallStats } from '@/hooks/progress/useProgressStats';
import { Trophy, Calendar, TrendingUp, Clock } from 'lucide-react';

export const AllTimeStats: React.FC = () => {
  const { overallStats, isLoading } = useOverallStats();

  if (isLoading) {
    return (
      <Card className="p-6">
        <Skeleton className="h-6 w-48 mb-6" />
        <div className="grid grid-cols-2 gap-4">
          {[1, 2, 3, 4].map((i) => (
            <Skeleton key={i} className="h-24 w-full" />
          ))}
        </div>
      </Card>
    );
  }

  if (!overallStats) {
    return (
      <Card className="p-6">
        <p className="text-center text-gray-500">No statistics available</p>
      </Card>
    );
  }

  const {
    totalTrainingDays,
    totalWorkoutsCompleted,
    averageWeeklyCompletionRate,
    firstWorkoutDate,
  } = overallStats;

  const memberSince = firstWorkoutDate
    ? new Date(firstWorkoutDate).toLocaleDateString('en-US', {
        month: 'long',
        year: 'numeric',
      })
    : 'N/A';

  const stats = [
    {
      icon: Calendar,
      label: 'Total Training Days',
      value: totalTrainingDays.toString(),
      color: 'text-blue-600',
      bgColor: 'bg-blue-100',
    },
    {
      icon: Trophy,
      label: 'Total Workouts',
      value: totalWorkoutsCompleted.toString(),
      color: 'text-purple-600',
      bgColor: 'bg-purple-100',
    },
    {
      icon: TrendingUp,
      label: 'Avg Weekly Rate',
      value: `${averageWeeklyCompletionRate.toFixed(1)}%`,
      color: 'text-green-600',
      bgColor: 'bg-green-100',
    },
    {
      icon: Clock,
      label: 'Member Since',
      value: memberSince,
      color: 'text-orange-600',
      bgColor: 'bg-orange-100',
    },
  ];

  return (
    <Card className="p-6">
      <h3 className="text-lg font-semibold text-gray-900 mb-6">All-Time Stats</h3>

      <div className="grid grid-cols-1 sm:grid-cols-2 gap-4">
        {stats.map(({ icon: Icon, label, value, color, bgColor }) => (
          <div
            key={label}
            className="flex items-start gap-3 p-4 rounded-lg bg-gray-50 border border-gray-200"
          >
            <div className={`rounded-lg p-2 ${bgColor}`}>
              <Icon className={`h-5 w-5 ${color}`} aria-hidden="true" />
            </div>
            <div className="flex-1 min-w-0">
              <p className="text-sm text-gray-600 mb-1">{label}</p>
              <p className="text-xl font-bold text-gray-900 truncate" title={value}>
                {value}
              </p>
            </div>
          </div>
        ))}
      </div>

      {/* Additional insights */}
      {totalWorkoutsCompleted > 0 && (
        <div className="mt-6 pt-6 border-t">
          <div className="flex items-center gap-2 mb-2">
            <Trophy className="h-4 w-4 text-yellow-600" aria-hidden="true" />
            <p className="text-sm font-medium text-gray-700">Achievement Unlocked</p>
          </div>
          <p className="text-sm text-gray-600">
            {totalWorkoutsCompleted >= 100
              ? '🏆 Century Club - 100+ workouts completed!'
              : totalWorkoutsCompleted >= 50
                ? '🥇 Half Century - 50+ workouts completed!'
                : totalWorkoutsCompleted >= 25
                  ? '🥈 Quarter Century - 25+ workouts completed!'
                  : totalWorkoutsCompleted >= 10
                    ? '🥉 Double Digits - 10+ workouts completed!'
                    : '🌟 Getting Started - Keep building your streak!'}
          </p>
        </div>
      )}
    </Card>
  );
};
