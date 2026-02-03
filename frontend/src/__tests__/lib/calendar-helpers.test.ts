/**
 * Unit tests for calendar-helpers utilities
 */

import { describe, it, expect } from '@jest/globals';
import {
  getCurrentDate,
  calendarDateToDate,
  dateToCalendarDate,
  calendarDateToISO,
  isoToCalendarDate,
  isSameDate,
  isToday,
  isPast,
  isFuture,
  addDays,
  addMonths,
  getWeekStart,
  getWeekEnd,
  getMonthStart,
  getMonthEnd,
  getWeekDays,
  formatDate,
  formatDayOfWeek,
} from '@/components/calendar/utils/calendar-helpers';
import { CalendarDate } from '@/types/calendar';

describe('calendar-helpers', () => {
  describe('getCurrentDate', () => {
    it('should return current date as CalendarDate', () => {
      const result = getCurrentDate();
      const now = new Date();
      expect(result.year).toBe(now.getFullYear());
      expect(result.month).toBe(now.getMonth() + 1);
      expect(result.day).toBe(now.getDate());
    });
  });

  describe('calendarDateToDate', () => {
    it('should convert CalendarDate to Date object', () => {
      const calDate: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = calendarDateToDate(calDate);
      expect(result.getFullYear()).toBe(2026);
      expect(result.getMonth()).toBe(1); // 0-indexed
      expect(result.getDate()).toBe(15);
    });
  });

  describe('dateToCalendarDate', () => {
    it('should convert Date to CalendarDate', () => {
      const date = new Date(2026, 1, 15); // Feb 15, 2026
      const result = dateToCalendarDate(date);
      expect(result).toEqual({ year: 2026, month: 2, day: 15 });
    });
  });

  describe('calendarDateToISO', () => {
    it('should convert CalendarDate to ISO string', () => {
      const calDate: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = calendarDateToISO(calDate);
      expect(result).toBe('2026-02-15');
    });

    it('should pad single-digit months and days', () => {
      const calDate: CalendarDate = { year: 2026, month: 3, day: 5 };
      const result = calendarDateToISO(calDate);
      expect(result).toBe('2026-03-05');
    });
  });

  describe('isoToCalendarDate', () => {
    it('should parse ISO string to CalendarDate', () => {
      const result = isoToCalendarDate('2026-02-15');
      expect(result.year).toBe(2026);
      expect(result.month).toBe(2);
      expect(result.day).toBe(15);
    });
  });

  describe('isSameDate', () => {
    it('should return true for same dates', () => {
      const date1: CalendarDate = { year: 2026, month: 2, day: 15 };
      const date2: CalendarDate = { year: 2026, month: 2, day: 15 };
      expect(isSameDate(date1, date2)).toBe(true);
    });

    it('should return false for different dates', () => {
      const date1: CalendarDate = { year: 2026, month: 2, day: 15 };
      const date2: CalendarDate = { year: 2026, month: 2, day: 16 };
      expect(isSameDate(date1, date2)).toBe(false);
    });
  });

  describe('addDays', () => {
    it('should add days to a date', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = addDays(date, 5);
      expect(result).toEqual({ year: 2026, month: 2, day: 20 });
    });

    it('should handle month boundaries', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 28 };
      const result = addDays(date, 1);
      expect(result).toEqual({ year: 2026, month: 3, day: 1 });
    });

    it('should subtract days with negative value', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = addDays(date, -5);
      expect(result).toEqual({ year: 2026, month: 2, day: 10 });
    });
  });

  describe('addMonths', () => {
    it('should add months to a date', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = addMonths(date, 3);
      expect(result).toEqual({ year: 2026, month: 5, day: 15 });
    });

    it('should handle year boundaries', () => {
      const date: CalendarDate = { year: 2026, month: 11, day: 15 };
      const result = addMonths(date, 2);
      expect(result).toEqual({ year: 2027, month: 1, day: 15 });
    });
  });

  describe('getWeekStart', () => {
    it('should return Monday of the week', () => {
      const wednesday: CalendarDate = { year: 2026, month: 2, day: 4 }; // Wed Feb 4, 2026
      const result = getWeekStart(wednesday);
      expect(result).toEqual({ year: 2026, month: 2, day: 2 }); // Mon Feb 2
    });

    it('should handle Sunday correctly', () => {
      const sunday: CalendarDate = { year: 2026, month: 2, day: 1 }; // Sun Feb 1, 2026
      const result = getWeekStart(sunday);
      expect(result).toEqual({ year: 2026, month: 1, day: 26 }); // Previous Monday
    });
  });

  describe('getWeekEnd', () => {
    it('should return Sunday of the week', () => {
      const wednesday: CalendarDate = { year: 2026, month: 2, day: 4 }; // Wed Feb 4, 2026
      const result = getWeekEnd(wednesday);
      expect(result).toEqual({ year: 2026, month: 2, day: 8 }); // Sun Feb 8
    });
  });

  describe('getMonthStart', () => {
    it('should return first day of month', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = getMonthStart(date);
      expect(result).toEqual({ year: 2026, month: 2, day: 1 });
    });
  });

  describe('getMonthEnd', () => {
    it('should return last day of month', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = getMonthEnd(date);
      expect(result.year).toBe(2026);
      expect(result.month).toBe(2);
      expect(result.day).toBe(28); // Feb 2026 has 28 days
    });
  });

  describe('getWeekDays', () => {
    it('should return 7 consecutive days', () => {
      const monday: CalendarDate = { year: 2026, month: 2, day: 2 };
      const result = getWeekDays(monday);
      expect(result).toHaveLength(7);
      expect(result[0]).toEqual(monday);
      expect(result[6]).toEqual({ year: 2026, month: 2, day: 8 });
    });
  });

  describe('formatDate', () => {
    it('should format date in long format', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = formatDate(date, 'long');
      expect(result).toContain('February');
      expect(result).toContain('15');
      expect(result).toContain('2026');
    });

    it('should format date in short format', () => {
      const date: CalendarDate = { year: 2026, month: 2, day: 15 };
      const result = formatDate(date, 'short');
      expect(result).toContain('Feb');
    });
  });

  describe('formatDayOfWeek', () => {
    it('should format day of week in long format', () => {
      const monday: CalendarDate = { year: 2026, month: 2, day: 2 };
      const result = formatDayOfWeek(monday, 'long');
      expect(result).toBe('Monday');
    });

    it('should format day of week in short format', () => {
      const monday: CalendarDate = { year: 2026, month: 2, day: 2 };
      const result = formatDayOfWeek(monday, 'short');
      expect(result).toBe('Mon');
    });
  });
});
