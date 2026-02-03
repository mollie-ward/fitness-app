/**
 * useCalendarNavigation - Hook for calendar navigation with keyboard support
 */

import { useEffect } from 'react';
import useCalendarStore from '@/lib/calendar-store';
import { CalendarView } from '@/types/calendar';

export interface UseCalendarNavigationOptions {
  enableKeyboardNav?: boolean;
}

export function useCalendarNavigation(options: UseCalendarNavigationOptions = {}) {
  const { enableKeyboardNav = true } = options;

  const view = useCalendarStore((state) => state.view);
  const selectedDate = useCalendarStore((state) => state.selectedDate);
  const setView = useCalendarStore((state) => state.setView);
  const nextPeriod = useCalendarStore((state) => state.nextPeriod);
  const previousPeriod = useCalendarStore((state) => state.previousPeriod);
  const goToToday = useCalendarStore((state) => state.goToToday);
  const selectDate = useCalendarStore((state) => state.selectDate);
  const getDateRange = useCalendarStore((state) => state.getDateRange);

  useEffect(() => {
    if (!enableKeyboardNav) return;

    const handleKeyDown = (e: KeyboardEvent) => {
      // Don't handle keyboard events if user is typing in an input
      if (
        e.target instanceof HTMLInputElement ||
        e.target instanceof HTMLTextAreaElement
      ) {
        return;
      }

      switch (e.key) {
        case 'ArrowLeft':
          e.preventDefault();
          previousPeriod();
          break;
        case 'ArrowRight':
          e.preventDefault();
          nextPeriod();
          break;
        case 'h':
        case 'Home':
          e.preventDefault();
          goToToday();
          break;
        case '1':
          e.preventDefault();
          setView(CalendarView.DAILY);
          break;
        case '2':
          e.preventDefault();
          setView(CalendarView.WEEKLY);
          break;
        case '3':
          e.preventDefault();
          setView(CalendarView.MONTHLY);
          break;
        default:
          break;
      }
    };

    window.addEventListener('keydown', handleKeyDown);
    return () => window.removeEventListener('keydown', handleKeyDown);
  }, [enableKeyboardNav, setView, nextPeriod, previousPeriod, goToToday]);

  return {
    view,
    selectedDate,
    setView,
    nextPeriod,
    previousPeriod,
    goToToday,
    selectDate,
    getDateRange,
  };
}
