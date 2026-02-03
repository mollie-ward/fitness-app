/**
 * CalendarHeatmap - GitHub-style calendar heatmap for completion history
 */

import * as React from 'react';
import { Card } from '@/components/ui/card';
import { Skeleton } from '@/components/ui/skeleton';
import { useCompletionHistory } from '@/hooks/progress/useProgressStats';
import { Discipline } from '@/types/discipline';
import type { CompletionHistoryDto } from '@/types/progress';
import {
  transformToHeatmapData,
  filterHistoryByDiscipline,
  getHeatmapIntensityColor,
  generateDateRange,
  formatDateForDisplay,
} from './utils/progress-helpers';
import { cn } from '@/lib/utils';
import { ChevronLeft, ChevronRight } from 'lucide-react';
import { Button } from '@/components/ui/button';

export interface CalendarHeatmapProps {
  onDayClick?: (date: string, workouts: CompletionHistoryDto[]) => void;
  disciplineFilter?: Discipline | 'all';
}

export const CalendarHeatmap: React.FC<CalendarHeatmapProps> = ({
  onDayClick,
  disciplineFilter = 'all',
}) => {
  const [currentMonth, setCurrentMonth] = React.useState(new Date());
  
  // Fetch last 90 days of data
  const startDate = React.useMemo(
    () => new Date(Date.now() - 90 * 24 * 60 * 60 * 1000),
    []
  );
  const endDate = React.useMemo(() => new Date(), []);
  
  const { completionHistory, isLoading } = useCompletionHistory(startDate, endDate);

  // Filter by discipline
  const filteredHistory = React.useMemo(
    () => filterHistoryByDiscipline(completionHistory, disciplineFilter),
    [completionHistory, disciplineFilter]
  );

  // Transform to heatmap data
  const heatmapData = React.useMemo(
    () => {
      const data = transformToHeatmapData(filteredHistory);
      const dataMap = new Map(data.map((d) => [d.date, d]));
      return dataMap;
    },
    [filteredHistory]
  );

  // Generate all dates for the last 90 days
  const allDates = React.useMemo(
    () => generateDateRange(startDate, endDate),
    [startDate, endDate]
  );

  // Group dates by weeks
  const weeks = React.useMemo(() => {
    const weekArray: string[][] = [];
    let currentWeek: string[] = [];

    allDates.forEach((date, index) => {
      const dayOfWeek = new Date(date).getDay();
      
      if (index === 0 && dayOfWeek !== 0) {
        // Fill empty days at start
        currentWeek = new Array(dayOfWeek).fill('');
      }

      currentWeek.push(date);

      if (dayOfWeek === 6 || index === allDates.length - 1) {
        weekArray.push(currentWeek);
        currentWeek = [];
      }
    });

    return weekArray;
  }, [allDates]);

  const goToPreviousMonth = () => {
    setCurrentMonth((prev) => new Date(prev.getFullYear(), prev.getMonth() - 1));
  };

  const goToNextMonth = () => {
    const now = new Date();
    const next = new Date(currentMonth.getFullYear(), currentMonth.getMonth() + 1);
    if (next <= now) {
      setCurrentMonth(next);
    }
  };

  const handleDayClick = (date: string) => {
    if (!date) return;
    const data = heatmapData.get(date);
    onDayClick?.(date, data?.workouts || []);
  };

  if (isLoading) {
    return (
      <Card className="p-6">
        <Skeleton className="h-6 w-48 mb-4" />
        <Skeleton className="h-64 w-full" />
      </Card>
    );
  }

  const monthName = currentMonth.toLocaleDateString('en-US', {
    month: 'long',
    year: 'numeric',
  });

  return (
    <Card className="p-6">
      {/* Header with month navigation */}
      <div className="flex items-center justify-between mb-6">
        <h3 className="text-lg font-semibold text-gray-900">Activity Heatmap</h3>
        <div className="flex items-center gap-2">
          <Button
            onClick={goToPreviousMonth}
            variant="ghost"
            size="sm"
            aria-label="Previous month"
          >
            <ChevronLeft className="h-4 w-4" />
          </Button>
          <span className="text-sm font-medium text-gray-700 min-w-[120px] text-center">
            {monthName}
          </span>
          <Button
            onClick={goToNextMonth}
            variant="ghost"
            size="sm"
            aria-label="Next month"
            disabled={currentMonth.getMonth() === new Date().getMonth()}
          >
            <ChevronRight className="h-4 w-4" />
          </Button>
        </div>
      </div>

      {/* Day labels */}
      <div className="flex gap-1 mb-2 ml-8">
        {['Sun', 'Mon', 'Tue', 'Wed', 'Thu', 'Fri', 'Sat'].map((day) => (
          <div
            key={day}
            className="w-3 text-xs text-gray-500 text-center"
            aria-hidden="true"
          >
            {day[0]}
          </div>
        ))}
      </div>

      {/* Heatmap grid */}
      <div className="overflow-x-auto">
        <div className="inline-flex gap-1">
          {weeks.map((week, weekIndex) => (
            <div key={weekIndex} className="flex flex-col gap-1">
              {week.map((date, dayIndex) => {
                const data = date ? heatmapData.get(date) : null;
                const count = data?.count || 0;
                const colorClass = getHeatmapIntensityColor(count);

                return (
                  <button
                    key={`${weekIndex}-${dayIndex}`}
                    onClick={() => handleDayClick(date)}
                    disabled={!date}
                    className={cn(
                      'w-3 h-3 rounded-sm border border-gray-300 transition-all',
                      'hover:ring-2 hover:ring-blue-500 hover:ring-offset-1',
                      'focus:outline-none focus:ring-2 focus:ring-blue-500 focus:ring-offset-1',
                      !date && 'invisible',
                      colorClass
                    )}
                    aria-label={
                      date
                        ? `${formatDateForDisplay(date)}: ${count} workout${count !== 1 ? 's' : ''}`
                        : undefined
                    }
                    title={
                      date
                        ? `${formatDateForDisplay(date)}: ${count} workout${count !== 1 ? 's' : ''}`
                        : undefined
                    }
                  />
                );
              })}
            </div>
          ))}
        </div>
      </div>

      {/* Legend */}
      <div className="flex items-center justify-end gap-2 mt-4 text-xs text-gray-600">
        <span>Less</span>
        {[0, 1, 2, 3, 4].map((level) => (
          <div
            key={level}
            className={cn(
              'w-3 h-3 rounded-sm border border-gray-300',
              getHeatmapIntensityColor(level)
            )}
            aria-hidden="true"
          />
        ))}
        <span>More</span>
      </div>
    </Card>
  );
};
