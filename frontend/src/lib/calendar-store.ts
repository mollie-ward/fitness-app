/**
 * Calendar state management with Zustand
 */

import { create } from 'zustand';
import { persist } from 'zustand/middleware';
import {
  CalendarView,
  CalendarDate,
  CalendarState,
  CalendarNavigationActions,
} from '@/types/calendar';
import {
  getCurrentDate,
  addDays,
  addMonths,
  getWeekStart,
  getWeekEnd,
  getMonthStart,
  getMonthEnd,
} from '@/components/calendar/utils/calendar-helpers';

interface CalendarStore extends CalendarState, CalendarNavigationActions {
  // Additional computed properties
  getDateRange: () => { startDate: CalendarDate; endDate: CalendarDate };
}

const useCalendarStore = create<CalendarStore>()(
  persist(
    (set, get) => ({
      // Initial state
      view: CalendarView.WEEKLY,
      selectedDate: getCurrentDate(),
      currentDate: getCurrentDate(),

      // Actions
      setView: (view: CalendarView) => {
        set({ view });
      },

      nextPeriod: () => {
        const { view, selectedDate } = get();
        let newDate: CalendarDate;

        switch (view) {
          case CalendarView.DAILY:
            newDate = addDays(selectedDate, 1);
            break;
          case CalendarView.WEEKLY:
            newDate = addDays(selectedDate, 7);
            break;
          case CalendarView.MONTHLY:
            newDate = addMonths(selectedDate, 1);
            break;
          default:
            newDate = selectedDate;
        }

        set({ selectedDate: newDate });
      },

      previousPeriod: () => {
        const { view, selectedDate } = get();
        let newDate: CalendarDate;

        switch (view) {
          case CalendarView.DAILY:
            newDate = addDays(selectedDate, -1);
            break;
          case CalendarView.WEEKLY:
            newDate = addDays(selectedDate, -7);
            break;
          case CalendarView.MONTHLY:
            newDate = addMonths(selectedDate, -1);
            break;
          default:
            newDate = selectedDate;
        }

        set({ selectedDate: newDate });
      },

      goToToday: () => {
        set({ selectedDate: getCurrentDate() });
      },

      selectDate: (date: CalendarDate) => {
        set({ selectedDate: date });
      },

      // Computed property - get date range for current view
      getDateRange: () => {
        const { view, selectedDate } = get();

        switch (view) {
          case CalendarView.DAILY:
            return {
              startDate: selectedDate,
              endDate: selectedDate,
            };
          case CalendarView.WEEKLY:
            return {
              startDate: getWeekStart(selectedDate),
              endDate: getWeekEnd(selectedDate),
            };
          case CalendarView.MONTHLY:
            return {
              startDate: getMonthStart(selectedDate),
              endDate: getMonthEnd(selectedDate),
            };
          default:
            return {
              startDate: selectedDate,
              endDate: selectedDate,
            };
        }
      },
    }),
    {
      name: 'calendar-storage',
      partialize: (state) => ({
        view: state.view,
        // Don't persist selectedDate - always start with current date
      }),
    }
  )
);

export default useCalendarStore;
