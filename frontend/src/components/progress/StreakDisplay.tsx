/**
 * StreakDisplay - Display current streak with fire emoji and milestone tracking
 */

import * as React from 'react';
import { Card } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useStreakInfo } from '@/hooks/progress/useProgressStats';
import { cn } from '@/lib/utils';
import { Flame, Award, Target, Trophy } from 'lucide-react';

export interface StreakDisplayProps {
  compact?: boolean;
  showMilestoneProgress?: boolean;
}

export const StreakDisplay: React.FC<StreakDisplayProps> = ({
  compact = false,
  showMilestoneProgress = true,
}) => {
  const { streakInfo, isLoading } = useStreakInfo();
  const [showCelebration, setShowCelebration] = React.useState(false);

  React.useEffect(() => {
    // Check if we just hit a milestone
    if (streakInfo) {
      const milestones = [7, 14, 30, 60, 90, 180, 365];
      const justHitMilestone = milestones.includes(streakInfo.currentStreak);
      
      if (justHitMilestone) {
        setShowCelebration(true);
        const timer = setTimeout(() => setShowCelebration(false), 3000);
        return () => clearTimeout(timer);
      }
    }
  }, [streakInfo]);

  if (isLoading) {
    return (
      <Card className={cn('p-6', compact && 'p-4')}>
        <Skeleton className="h-6 w-32 mb-2" />
        <Skeleton className="h-12 w-full" />
      </Card>
    );
  }

  if (!streakInfo) {
    return (
      <Card className={cn('p-6', compact && 'p-4')}>
        <div className="text-center">
          <p className="text-sm text-gray-500">Start your streak today!</p>
        </div>
      </Card>
    );
  }

  const {
    currentStreak,
    longestStreak,
    daysUntilNextMilestone,
    nextMilestone,
  } = streakInfo;

  const isNewRecord = currentStreak === longestStreak && currentStreak > 0;
  const milestoneProgress = nextMilestone > 0
    ? ((currentStreak % nextMilestone) / nextMilestone) * 100
    : 0;

  return (
    <Card className={cn('p-6', compact && 'p-4', showCelebration && 'animate-pulse-glow')}>
      {/* Header */}
      <div className="flex items-center justify-between mb-4">
        <div className="flex items-center gap-2">
          <Flame className="h-5 w-5 text-orange-600" aria-hidden="true" />
          <h3 className="text-lg font-semibold text-gray-900">
            {compact ? 'Streak' : 'Current Streak'}
          </h3>
        </div>
        {isNewRecord && (
          <Award className="h-5 w-5 text-yellow-600" aria-label="New record!" />
        )}
      </div>

      {/* Streak count */}
      <div className="text-center mb-4">
        <div className="flex items-center justify-center gap-2 mb-2">
          <span
            className={cn(
              'text-5xl',
              currentStreak >= 7 && 'animate-pulse'
            )}
            role="img"
            aria-label="Fire emoji"
          >
            🔥
          </span>
          <span className="text-4xl font-bold text-gray-900">{currentStreak}</span>
        </div>
        <p className="text-sm text-gray-600">
          {currentStreak === 1 ? 'day streak' : 'days streak'}
        </p>
      </div>

      {/* Personal best */}
      {longestStreak > 0 && (
        <div className="flex items-center justify-center gap-2 mb-4 text-sm text-gray-600">
          <Trophy className="h-4 w-4" aria-hidden="true" />
          <span>
            Personal best: <span className="font-semibold">{longestStreak}</span> days
          </span>
        </div>
      )}

      {/* Milestone progress */}
      {showMilestoneProgress && nextMilestone > 0 && (
        <div className="pt-4 border-t">
          <div className="flex items-center justify-between mb-2">
            <div className="flex items-center gap-2">
              <Target className="h-4 w-4 text-blue-600" aria-hidden="true" />
              <p className="text-sm font-medium text-gray-700">
                Next Milestone: {nextMilestone} days
              </p>
            </div>
            <span className="text-xs text-gray-500">
              {daysUntilNextMilestone} {daysUntilNextMilestone === 1 ? 'day' : 'days'} to go
            </span>
          </div>

          {/* Progress bar */}
          <div
            className="relative h-2 w-full overflow-hidden rounded-full bg-gray-200"
            role="progressbar"
            aria-valuenow={milestoneProgress}
            aria-valuemin={0}
            aria-valuemax={100}
            aria-label={`${milestoneProgress.toFixed(0)}% to next milestone`}
          >
            <div
              className="h-full bg-gradient-to-r from-orange-500 to-red-500 transition-all duration-500"
              style={{ width: `${milestoneProgress}%` }}
            />
          </div>
        </div>
      )}

      {/* Motivational message */}
      {!compact && (
        <div className="mt-4 pt-4 border-t">
          <p className="text-sm text-center font-medium text-gray-700">
            {currentStreak === 0
              ? "Let's start your streak today!"
              : currentStreak >= 30
                ? 'Incredible consistency! 🎉'
                : currentStreak >= 14
                  ? 'Two weeks strong! 💪'
                  : currentStreak >= 7
                    ? 'One week down! Keep it up! 🌟'
                    : "You're building momentum! 🚀"}
          </p>
        </div>
      )}
    </Card>
  );
};
