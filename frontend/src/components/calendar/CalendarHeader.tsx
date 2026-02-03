/**
 * CalendarHeader - Header with view switcher and current date display
 */

import * as React from 'react';
import { CalendarView, CalendarDate } from '@/types/calendar';
import { Button } from '@/components/ui/button';
import { Calendar as CalendarIcon, List, Grid3x3 } from 'lucide-react';
import { cn } from '@/lib/utils';
import { formatDate } from './utils/calendar-helpers';

export interface CalendarHeaderProps {
  view: CalendarView;
  selectedDate: CalendarDate;
  onViewChange: (view: CalendarView) => void;
  className?: string;
}

const viewConfig = {
  [CalendarView.DAILY]: { icon: List, label: 'Daily' },
  [CalendarView.WEEKLY]: { icon: CalendarIcon, label: 'Weekly' },
  [CalendarView.MONTHLY]: { icon: Grid3x3, label: 'Monthly' },
};

export const CalendarHeader: React.FC<CalendarHeaderProps> = ({
  view,
  selectedDate,
  onViewChange,
  className = '',
}) => {
  return (
    <div className={cn('space-y-4', className)}>
      {/* Title */}
      <div>
        <h1 className="text-3xl font-bold text-gray-900">Workout Calendar</h1>
        <p className="text-gray-600 mt-1">{formatDate(selectedDate, 'long')}</p>
      </div>

      {/* View Switcher */}
      <div className="flex gap-2">
        {Object.entries(viewConfig).map(([viewKey, config]) => {
          const Icon = config.icon;
          const isActive = view === viewKey;

          return (
            <Button
              key={viewKey}
              variant={isActive ? 'default' : 'outline'}
              size="sm"
              onClick={() => onViewChange(viewKey as CalendarView)}
              className={cn('flex items-center gap-2')}
              aria-pressed={isActive}
              aria-label={`Switch to ${config.label} view`}
            >
              <Icon size={16} />
              <span className="hidden sm:inline">{config.label}</span>
            </Button>
          );
        })}
      </div>
    </div>
  );
};
