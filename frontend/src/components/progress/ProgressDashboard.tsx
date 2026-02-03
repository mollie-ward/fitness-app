/**
 * ProgressDashboard - Main dashboard component for progress tracking
 */

import * as React from 'react';
import { WeeklySummary } from './WeeklySummary';
import { OverallPlanProgress } from './OverallPlanProgress';
import { AllTimeStats } from './AllTimeStats';
import { StreakDisplay } from './StreakDisplay';
import { cn } from '@/lib/utils';

export interface ProgressDashboardProps {
  className?: string;
}

export const ProgressDashboard: React.FC<ProgressDashboardProps> = ({ className = '' }) => {
  return (
    <div className={cn('space-y-6', className)}>
      {/* Top section - Streak and Weekly Summary */}
      <div className="grid grid-cols-1 lg:grid-cols-2 gap-6">
        <StreakDisplay />
        <WeeklySummary />
      </div>

      {/* Middle section - Overall Plan Progress */}
      <OverallPlanProgress />

      {/* Bottom section - All-Time Stats */}
      <AllTimeStats />
    </div>
  );
};
