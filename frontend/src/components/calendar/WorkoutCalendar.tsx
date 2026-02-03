/**
 * WorkoutCalendar - Main calendar component orchestrating all views
 */

import * as React from 'react';
import { useState } from 'react';
import { Workout } from '@/types/workout';
import { CalendarView, CalendarDate } from '@/types/calendar';
import { CalendarHeader } from './CalendarHeader';
import { CalendarControls } from './CalendarControls';
import { DailyView } from './views/DailyView';
import { WeeklyView } from './views/WeeklyView';
import { MonthlyView } from './views/MonthlyView';
import { WorkoutDetail } from './workout/WorkoutDetail';
import { useCalendarNavigation } from './hooks/useCalendarNavigation';
import { useWorkoutData } from './hooks/useWorkoutData';
import { calendarDateToISO } from './utils/calendar-helpers';

export interface WorkoutCalendarProps {
  userId: string;
  onStartWorkout?: (workoutId: string) => void;
  enableKeyboardNav?: boolean;
  className?: string;
}

export const WorkoutCalendar: React.FC<WorkoutCalendarProps> = ({
  userId,
  onStartWorkout,
  enableKeyboardNav = true,
  className = '',
}) => {
  const {
    view,
    selectedDate,
    setView,
    nextPeriod,
    previousPeriod,
    goToToday,
    selectDate,
    getDateRange,
  } = useCalendarNavigation({ enableKeyboardNav });

  const dateRange = getDateRange();
  const { workouts, isLoading, updateWorkoutStatus } = useWorkoutData({
    userId,
    startDate: calendarDateToISO(dateRange.startDate),
    endDate: calendarDateToISO(dateRange.endDate),
  });

  const [selectedWorkout, setSelectedWorkout] = useState<Workout | null>(null);
  const [isDetailOpen, setIsDetailOpen] = useState(false);

  const handleWorkoutClick = (workout: Workout) => {
    setSelectedWorkout(workout);
    setIsDetailOpen(true);
  };

  const handleCloseDetail = () => {
    setIsDetailOpen(false);
    setSelectedWorkout(null);
  };

  const handleStartWorkout = (workoutId: string) => {
    handleCloseDetail();
    onStartWorkout?.(workoutId);
  };

  const handleDateClick = (date: CalendarDate) => {
    selectDate(date);
    if (view !== CalendarView.DAILY) {
      setView(CalendarView.DAILY);
    }
  };

  return (
    <div className={className}>
      {/* Header */}
      <CalendarHeader view={view} selectedDate={selectedDate} onViewChange={setView} />

      {/* Navigation Controls */}
      <div className="my-6">
        <CalendarControls
          view={view}
          onPrevious={previousPeriod}
          onNext={nextPeriod}
          onToday={goToToday}
        />
      </div>

      {/* View Content */}
      <div className="mt-6">
        {view === CalendarView.DAILY && (
          <DailyView
            workouts={workouts}
            isLoading={isLoading}
            onWorkoutClick={handleWorkoutClick}
            onStartWorkout={handleStartWorkout}
          />
        )}

        {view === CalendarView.WEEKLY && (
          <WeeklyView
            workouts={workouts}
            selectedDate={selectedDate}
            isLoading={isLoading}
            onWorkoutClick={handleWorkoutClick}
            onDateClick={handleDateClick}
          />
        )}

        {view === CalendarView.MONTHLY && (
          <MonthlyView
            workouts={workouts}
            selectedDate={selectedDate}
            isLoading={isLoading}
            onWorkoutClick={handleWorkoutClick}
            onDateClick={handleDateClick}
          />
        )}
      </div>

      {/* Workout Detail Modal */}
      <WorkoutDetail
        workout={selectedWorkout}
        open={isDetailOpen}
        onClose={handleCloseDetail}
        onStartWorkout={handleStartWorkout}
      />

      {/* Keyboard shortcuts hint */}
      {enableKeyboardNav && (
        <div className="mt-8 p-4 bg-gray-50 rounded-lg text-sm text-gray-600">
          <p className="font-semibold mb-2">Keyboard Shortcuts:</p>
          <ul className="space-y-1">
            <li>
              <kbd className="px-2 py-1 bg-white border border-gray-300 rounded">←</kbd> /{' '}
              <kbd className="px-2 py-1 bg-white border border-gray-300 rounded">→</kbd> -
              Navigate periods
            </li>
            <li>
              <kbd className="px-2 py-1 bg-white border border-gray-300 rounded">H</kbd> - Go to
              today
            </li>
            <li>
              <kbd className="px-2 py-1 bg-white border border-gray-300 rounded">1</kbd> /{' '}
              <kbd className="px-2 py-1 bg-white border border-gray-300 rounded">2</kbd> /{' '}
              <kbd className="px-2 py-1 bg-white border border-gray-300 rounded">3</kbd> - Switch
              views
            </li>
          </ul>
        </div>
      )}
    </div>
  );
};
