/**
 * CalendarControls - Navigation controls for calendar
 */

import * as React from 'react';
import { CalendarView } from '@/types/calendar';
import { Button } from '@/components/ui/button';
import { ChevronLeft, ChevronRight, Home } from 'lucide-react';
import { cn } from '@/lib/utils';

export interface CalendarControlsProps {
  view: CalendarView;
  onPrevious: () => void;
  onNext: () => void;
  onToday: () => void;
  className?: string;
}

export const CalendarControls: React.FC<CalendarControlsProps> = ({
  view,
  onPrevious,
  onNext,
  onToday,
  className = '',
}) => {
  const getPeriodLabel = () => {
    switch (view) {
      case CalendarView.DAILY:
        return 'day';
      case CalendarView.WEEKLY:
        return 'week';
      case CalendarView.MONTHLY:
        return 'month';
      default:
        return 'period';
    }
  };

  return (
    <div className={cn('flex items-center justify-between gap-2', className)}>
      <Button
        variant="outline"
        size="sm"
        onClick={onPrevious}
        className="flex items-center gap-1"
        aria-label={`Previous ${getPeriodLabel()}`}
      >
        <ChevronLeft size={16} />
        <span className="hidden sm:inline">Previous</span>
      </Button>

      <Button
        variant="outline"
        size="sm"
        onClick={onToday}
        className="flex items-center gap-1"
        aria-label="Go to today"
      >
        <Home size={16} />
        <span>Today</span>
      </Button>

      <Button
        variant="outline"
        size="sm"
        onClick={onNext}
        className="flex items-center gap-1"
        aria-label={`Next ${getPeriodLabel()}`}
      >
        <span className="hidden sm:inline">Next</span>
        <ChevronRight size={16} />
      </Button>
    </div>
  );
};
