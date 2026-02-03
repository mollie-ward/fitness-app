/**
 * Calendar date manipulation utilities
 */

import { CalendarDate } from '@/types/calendar';

/**
 * Get current date as CalendarDate
 * Note: JavaScript Date.getMonth() is 0-indexed, so we add 1 to convert to 1-12
 */
export function getCurrentDate(): CalendarDate {
  const now = new Date();
  return {
    year: now.getFullYear(),
    month: now.getMonth() + 1, // Convert from 0-11 to 1-12
    day: now.getDate(),
  };
}

/**
 * Convert CalendarDate to Date object
 * Note: CalendarDate uses 1-indexed months, JavaScript Date uses 0-indexed
 */
export function calendarDateToDate(calendarDate: CalendarDate): Date {
  return new Date(calendarDate.year, calendarDate.month - 1, calendarDate.day);
}

/**
 * Convert Date to CalendarDate
 * Note: Converts JavaScript's 0-indexed months to CalendarDate's 1-indexed months
 */
export function dateToCalendarDate(date: Date): CalendarDate {
  return {
    year: date.getFullYear(),
    month: date.getMonth() + 1, // Convert from 0-11 to 1-12
    day: date.getDate(),
  };
}

/**
 * Convert CalendarDate to ISO 8601 date string (YYYY-MM-DD)
 */
export function calendarDateToISO(calendarDate: CalendarDate): string {
  const month = calendarDate.month.toString().padStart(2, '0');
  const day = calendarDate.day.toString().padStart(2, '0');
  return `${calendarDate.year}-${month}-${day}`;
}

/**
 * Parse ISO 8601 date string to CalendarDate
 */
export function isoToCalendarDate(isoDate: string): CalendarDate {
  const date = new Date(isoDate);
  return dateToCalendarDate(date);
}

/**
 * Check if two calendar dates are equal
 */
export function isSameDate(date1: CalendarDate, date2: CalendarDate): boolean {
  return date1.year === date2.year && date1.month === date2.month && date1.day === date2.day;
}

/**
 * Check if a date is today
 */
export function isToday(date: CalendarDate): boolean {
  return isSameDate(date, getCurrentDate());
}

/**
 * Check if a date is in the past
 */
export function isPast(date: CalendarDate): boolean {
  const dateObj = calendarDateToDate(date);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  dateObj.setHours(0, 0, 0, 0);
  return dateObj < today;
}

/**
 * Check if a date is in the future
 */
export function isFuture(date: CalendarDate): boolean {
  const dateObj = calendarDateToDate(date);
  const today = new Date();
  today.setHours(0, 0, 0, 0);
  dateObj.setHours(0, 0, 0, 0);
  return dateObj > today;
}

/**
 * Add days to a calendar date
 */
export function addDays(date: CalendarDate, days: number): CalendarDate {
  const dateObj = calendarDateToDate(date);
  dateObj.setDate(dateObj.getDate() + days);
  return dateToCalendarDate(dateObj);
}

/**
 * Add months to a calendar date
 */
export function addMonths(date: CalendarDate, months: number): CalendarDate {
  const dateObj = calendarDateToDate(date);
  dateObj.setMonth(dateObj.getMonth() + months);
  return dateToCalendarDate(dateObj);
}

/**
 * Get start of week (Monday) for a given date
 */
export function getWeekStart(date: CalendarDate): CalendarDate {
  const dateObj = calendarDateToDate(date);
  const day = dateObj.getDay();
  const diff = day === 0 ? -6 : 1 - day; // Adjust when day is Sunday
  dateObj.setDate(dateObj.getDate() + diff);
  return dateToCalendarDate(dateObj);
}

/**
 * Get end of week (Sunday) for a given date
 */
export function getWeekEnd(date: CalendarDate): CalendarDate {
  const weekStart = getWeekStart(date);
  return addDays(weekStart, 6);
}

/**
 * Get start of month for a given date
 */
export function getMonthStart(date: CalendarDate): CalendarDate {
  return {
    year: date.year,
    month: date.month,
    day: 1,
  };
}

/**
 * Get end of month for a given date
 */
export function getMonthEnd(date: CalendarDate): CalendarDate {
  const dateObj = new Date(date.year, date.month, 0);
  return dateToCalendarDate(dateObj);
}

/**
 * Get all days in a week
 */
export function getWeekDays(weekStart: CalendarDate): CalendarDate[] {
  const days: CalendarDate[] = [];
  for (let i = 0; i < 7; i++) {
    days.push(addDays(weekStart, i));
  }
  return days;
}

/**
 * Get all days in a month (including padding days from previous/next month for grid)
 */
export function getMonthDays(date: CalendarDate): CalendarDate[] {
  const monthStart = getMonthStart(date);
  const gridStart = getWeekStart(monthStart);

  const days: CalendarDate[] = [];
  let current = gridStart;

  // Generate 6 weeks (42 days) for consistent grid
  for (let i = 0; i < 42; i++) {
    days.push(current);
    current = addDays(current, 1);
  }

  return days;
}

/**
 * Format date for display (e.g., "Feb 3, 2026")
 */
export function formatDate(date: CalendarDate, format: 'short' | 'long' = 'long'): string {
  const dateObj = calendarDateToDate(date);
  return dateObj.toLocaleDateString('en-US', {
    month: format === 'short' ? 'short' : 'long',
    day: 'numeric',
    year: 'numeric',
  });
}

/**
 * Format date as day of week (e.g., "Monday", "Mon")
 */
export function formatDayOfWeek(date: CalendarDate, format: 'short' | 'long' = 'long'): string {
  const dateObj = calendarDateToDate(date);
  return dateObj.toLocaleDateString('en-US', {
    weekday: format === 'short' ? 'short' : 'long',
  });
}

/**
 * Get week number in year
 */
export function getWeekNumber(date: CalendarDate): number {
  const dateObj = calendarDateToDate(date);
  const firstDayOfYear = new Date(dateObj.getFullYear(), 0, 1);
  const pastDaysOfYear = (dateObj.getTime() - firstDayOfYear.getTime()) / 86400000;
  return Math.ceil((pastDaysOfYear + firstDayOfYear.getDay() + 1) / 7);
}
