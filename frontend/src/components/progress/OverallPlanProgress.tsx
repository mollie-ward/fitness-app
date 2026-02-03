/**
 * OverallPlanProgress - Display overall plan completion statistics
 */

import * as React from 'react';
import { Card } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useOverallStats } from '@/hooks/progress/useProgressStats';
import { cn } from '@/lib/utils';
import { Target, TrendingUp } from 'lucide-react';

export const OverallPlanProgress: React.FC = () => {
  const { overallStats, isLoading } = useOverallStats();

  if (isLoading) {
    return (
      <Card className="p-6">
        <Skeleton className="h-6 w-48 mb-4" />
        <Skeleton className="h-32 w-full mb-4" />
        <Skeleton className="h-4 w-64" />
      </Card>
    );
  }

  if (!overallStats) {
    return (
      <Card className="p-6">
        <p className="text-center text-gray-500">No plan data available</p>
      </Card>
    );
  }

  const {
    totalWorkoutsCompleted,
    overallPlanCompletionPercentage,
    averageWeeklyCompletionRate,
  } = overallStats;

  // Estimate completion based on average rate (placeholder logic)
  const weeksRemaining = overallPlanCompletionPercentage > 0
    ? Math.ceil((100 - overallPlanCompletionPercentage) / (averageWeeklyCompletionRate || 1))
    : 0;
  
  const estimatedCompletionDate = weeksRemaining > 0
    ? new Date(Date.now() + weeksRemaining * 7 * 24 * 60 * 60 * 1000)
    : null;

  return (
    <Card className="p-6">
      <div className="flex items-center gap-2 mb-4">
        <Target className="h-5 w-5 text-purple-600" aria-hidden="true" />
        <h3 className="text-lg font-semibold text-gray-900">Overall Plan Progress</h3>
      </div>

      {/* Completion percentage */}
      <div className="mb-4">
        <p className="text-3xl font-bold text-gray-900 mb-2">
          {overallPlanCompletionPercentage.toFixed(1)}%
        </p>
        <p className="text-sm text-gray-600">
          {totalWorkoutsCompleted} workouts completed
        </p>
      </div>

      {/* Circular progress indicator */}
      <div className="flex items-center justify-center mb-4">
        <div className="relative h-40 w-40">
          <svg className="transform -rotate-90" width="160" height="160">
            {/* Background circle */}
            <circle
              cx="80"
              cy="80"
              r="70"
              fill="none"
              stroke="#e5e7eb"
              strokeWidth="12"
            />
            {/* Progress circle */}
            <circle
              cx="80"
              cy="80"
              r="70"
              fill="none"
              stroke="#8b5cf6"
              strokeWidth="12"
              strokeDasharray={`${2 * Math.PI * 70}`}
              strokeDashoffset={`${2 * Math.PI * 70 * (1 - overallPlanCompletionPercentage / 100)}`}
              strokeLinecap="round"
              className="transition-all duration-1000"
            />
          </svg>
          <div className="absolute inset-0 flex flex-col items-center justify-center">
            <span className="text-2xl font-bold text-gray-900">
              {overallPlanCompletionPercentage.toFixed(0)}%
            </span>
            <span className="text-xs text-gray-500">complete</span>
          </div>
        </div>
      </div>

      {/* Estimated completion */}
      {estimatedCompletionDate && (
        <div className="pt-4 border-t">
          <div className="flex items-center gap-2 mb-2">
            <TrendingUp className="h-4 w-4 text-gray-600" aria-hidden="true" />
            <p className="text-sm font-medium text-gray-700">Estimated Completion</p>
          </div>
          <p className="text-sm text-gray-600">
            {estimatedCompletionDate.toLocaleDateString('en-US', {
              month: 'long',
              day: 'numeric',
              year: 'numeric',
            })}
          </p>
          <p className="text-xs text-gray-500 mt-1">
            Based on current pace ({averageWeeklyCompletionRate.toFixed(1)}% per week)
          </p>
        </div>
      )}
    </Card>
  );
};
