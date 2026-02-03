/**
 * HistoricalProgressView - Complete historical progress view with heatmap and filters
 */

import * as React from 'react';
import { CalendarHeatmap } from './CalendarHeatmap';
import { DisciplineFilter } from './DisciplineFilter';
import { HeatmapDayDetail } from './HeatmapDayDetail';
import { Discipline } from '@/types/discipline';
import type { CompletionHistoryDto } from '@/types/progress';

export const HistoricalProgressView: React.FC = () => {
  const [selectedDiscipline, setSelectedDiscipline] = React.useState<Discipline | 'all'>('all');
  const [selectedDate, setSelectedDate] = React.useState<string | null>(null);
  const [selectedWorkouts, setSelectedWorkouts] = React.useState<CompletionHistoryDto[]>([]);

  const handleDayClick = (date: string, workouts: CompletionHistoryDto[]) => {
    setSelectedDate(date);
    setSelectedWorkouts(workouts);
  };

  const handleCloseModal = () => {
    setSelectedDate(null);
    setSelectedWorkouts([]);
  };

  return (
    <div className="space-y-6">
      {/* Discipline filter */}
      <div>
        <h3 className="text-lg font-semibold text-gray-900 mb-3">Filter by Discipline</h3>
        <DisciplineFilter
          selectedDiscipline={selectedDiscipline}
          onFilterChange={setSelectedDiscipline}
        />
      </div>

      {/* Heatmap */}
      <CalendarHeatmap
        disciplineFilter={selectedDiscipline}
        onDayClick={handleDayClick}
      />

      {/* Day detail modal */}
      {selectedDate && (
        <HeatmapDayDetail
          isOpen={!!selectedDate}
          onClose={handleCloseModal}
          date={selectedDate}
          workouts={selectedWorkouts}
        />
      )}
    </div>
  );
};
