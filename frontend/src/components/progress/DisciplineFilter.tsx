/**
 * DisciplineFilter - Toggle filter for heatmap by discipline
 */

import * as React from 'react';
import { Discipline } from '@/types/discipline';
import { Button } from '@/components/ui/button';
import { cn } from '@/lib/utils';

export interface DisciplineFilterProps {
  selectedDiscipline: Discipline | 'all';
  onFilterChange: (discipline: Discipline | 'all') => void;
}

export const DisciplineFilter: React.FC<DisciplineFilterProps> = ({
  selectedDiscipline,
  onFilterChange,
}) => {
  const filters: Array<{ value: Discipline | 'all'; label: string }> = [
    { value: 'all', label: 'All' },
    { value: Discipline.HYROX, label: 'HYROX' },
    { value: Discipline.RUNNING, label: 'Running' },
    { value: Discipline.STRENGTH, label: 'Strength' },
  ];

  return (
    <div
      className="flex flex-wrap gap-2"
      role="group"
      aria-label="Filter workouts by discipline"
    >
      {filters.map(({ value, label }) => (
        <Button
          key={value}
          onClick={() => onFilterChange(value)}
          variant={selectedDiscipline === value ? 'default' : 'outline'}
          size="sm"
          className={cn(
            'transition-all',
            selectedDiscipline === value && 'ring-2 ring-blue-500 ring-offset-1'
          )}
          aria-pressed={selectedDiscipline === value}
        >
          {label}
        </Button>
      ))}
    </div>
  );
};
