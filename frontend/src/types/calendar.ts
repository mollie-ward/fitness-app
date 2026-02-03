/**
 * Calendar-specific types
 */

export enum CalendarView {
  DAILY = 'DAILY',
  WEEKLY = 'WEEKLY',
  MONTHLY = 'MONTHLY',
}

export interface CalendarDate {
  year: number;
  month: number; // 1-12
  day: number; // 1-31
}

export interface DateRange {
  startDate: string; // ISO 8601 date string
  endDate: string; // ISO 8601 date string
}

export interface CalendarState {
  view: CalendarView;
  selectedDate: CalendarDate;
  currentDate: CalendarDate;
}

export interface CalendarNavigationActions {
  setView: (view: CalendarView) => void;
  nextPeriod: () => void;
  previousPeriod: () => void;
  goToToday: () => void;
  selectDate: (date: CalendarDate) => void;
}
