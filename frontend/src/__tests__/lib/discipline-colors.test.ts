/**
 * Unit tests for discipline-colors utilities
 */

import { describe, it, expect } from '@jest/globals';
import {
  getDisciplineColors,
  getDisciplineConfig,
  getDisciplineClasses,
} from '@/components/calendar/utils/discipline-colors';
import { Discipline } from '@/types/discipline';

describe('discipline-colors', () => {
  describe('getDisciplineColors', () => {
    it('should return colors for HYROX discipline', () => {
      const colors = getDisciplineColors(Discipline.HYROX);
      expect(colors).toHaveProperty('primary');
      expect(colors).toHaveProperty('secondary');
      expect(colors).toHaveProperty('bg');
      expect(colors).toHaveProperty('text');
      expect(colors).toHaveProperty('border');
      expect(colors.primary).toContain('rgb');
    });

    it('should return colors for RUNNING discipline', () => {
      const colors = getDisciplineColors(Discipline.RUNNING);
      expect(colors.primary).toContain('blue');
    });

    it('should return colors for STRENGTH discipline', () => {
      const colors = getDisciplineColors(Discipline.STRENGTH);
      expect(colors.primary).toContain('purple');
    });

    it('should return colors for REST discipline', () => {
      const colors = getDisciplineColors(Discipline.REST);
      expect(colors.primary).toContain('gray');
    });
  });

  describe('getDisciplineConfig', () => {
    it('should return full config for HYROX', () => {
      const config = getDisciplineConfig(Discipline.HYROX);
      expect(config.name).toBe('HYROX');
      expect(config.icon).toBe('Zap');
      expect(config.description).toContain('HYROX');
      expect(config.colors).toBeDefined();
    });

    it('should return full config for RUNNING', () => {
      const config = getDisciplineConfig(Discipline.RUNNING);
      expect(config.name).toBe('Running');
      expect(config.icon).toBe('Footprints');
    });

    it('should return full config for STRENGTH', () => {
      const config = getDisciplineConfig(Discipline.STRENGTH);
      expect(config.name).toBe('Strength');
      expect(config.icon).toBe('Dumbbell');
    });

    it('should return full config for HYBRID', () => {
      const config = getDisciplineConfig(Discipline.HYBRID);
      expect(config.name).toBe('Hybrid');
      expect(config.icon).toBe('Sparkles');
    });

    it('should return full config for REST', () => {
      const config = getDisciplineConfig(Discipline.REST);
      expect(config.name).toBe('Rest');
      expect(config.icon).toBe('Coffee');
    });
  });

  describe('getDisciplineClasses', () => {
    it('should return Tailwind classes for HYROX', () => {
      const classes = getDisciplineClasses(Discipline.HYROX);
      expect(classes.bg).toContain('amber');
      expect(classes.text).toContain('orange');
      expect(classes.border).toContain('amber');
    });

    it('should return Tailwind classes for RUNNING', () => {
      const classes = getDisciplineClasses(Discipline.RUNNING);
      expect(classes.bg).toContain('blue');
      expect(classes.text).toContain('blue');
    });

    it('should return Tailwind classes for STRENGTH', () => {
      const classes = getDisciplineClasses(Discipline.STRENGTH);
      expect(classes.bg).toContain('purple');
      expect(classes.text).toContain('purple');
    });

    it('should return gradient for HYBRID', () => {
      const classes = getDisciplineClasses(Discipline.HYBRID);
      expect(classes.bg).toContain('gradient');
    });

    it('should return gray classes for REST', () => {
      const classes = getDisciplineClasses(Discipline.REST);
      expect(classes.bg).toContain('gray');
      expect(classes.text).toContain('gray');
    });
  });
});
