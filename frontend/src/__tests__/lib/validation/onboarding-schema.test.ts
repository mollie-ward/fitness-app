/**
 * Unit tests for onboarding validation schemas
 */
import { describe, it, expect } from '@jest/globals';
import {
  fitnessLevelSchema,
  goalsSchema,
  scheduleSchema,
  injuriesSchema,
  trainingBackgroundSchema,
  onboardingFormSchema,
} from '@/lib/validation/onboarding-schema';
import { FitnessLevel, GoalType, InjuryType } from '@/types/onboarding';

describe('Onboarding Validation Schemas', () => {
  describe('fitnessLevelSchema', () => {
    it('should accept valid fitness levels', () => {
      const validData = {
        hyroxLevel: FitnessLevel.Intermediate,
        runningLevel: FitnessLevel.Advanced,
        strengthLevel: FitnessLevel.Beginner,
      };

      const result = fitnessLevelSchema.safeParse(validData);
      expect(result.success).toBe(true);
    });

    it('should reject invalid fitness level values', () => {
      const invalidData = {
        hyroxLevel: 999,
        runningLevel: FitnessLevel.Beginner,
        strengthLevel: FitnessLevel.Beginner,
      };

      const result = fitnessLevelSchema.safeParse(invalidData);
      expect(result.success).toBe(false);
    });
  });

  describe('goalsSchema', () => {
    it('should accept valid goal data', () => {
      const validData = {
        goals: [
          {
            goalType: GoalType.HyroxRace,
            description: 'Complete my first HYROX race',
            targetDate: new Date('2026-06-01'),
            isPrimary: true,
          },
        ],
      };

      const result = goalsSchema.safeParse(validData);
      expect(result.success).toBe(true);
    });

    it('should reject empty goals array', () => {
      const invalidData = {
        goals: [],
      };

      const result = goalsSchema.safeParse(invalidData);
      expect(result.success).toBe(false);
    });

    it('should reject goal with short description', () => {
      const invalidData = {
        goals: [
          {
            goalType: GoalType.GeneralFitness,
            description: 'ab', // Too short
            isPrimary: false,
          },
        ],
      };

      const result = goalsSchema.safeParse(invalidData);
      expect(result.success).toBe(false);
    });
  });

  describe('scheduleSchema', () => {
    it('should accept valid schedule with at least 1 day selected', () => {
      const validData = {
        monday: true,
        tuesday: false,
        wednesday: true,
        thursday: false,
        friday: true,
        saturday: false,
        sunday: false,
      };

      const result = scheduleSchema.safeParse(validData);
      expect(result.success).toBe(true);
    });

    it('should reject schedule with no days selected', () => {
      const invalidData = {
        monday: false,
        tuesday: false,
        wednesday: false,
        thursday: false,
        friday: false,
        saturday: false,
        sunday: false,
      };

      const result = scheduleSchema.safeParse(invalidData);
      expect(result.success).toBe(false);
    });
  });

  describe('injuriesSchema', () => {
    it('should accept no injuries', () => {
      const validData = {
        hasInjuries: false,
        injuries: [],
      };

      const result = injuriesSchema.safeParse(validData);
      expect(result.success).toBe(true);
    });

    it('should accept valid injury data', () => {
      const validData = {
        hasInjuries: true,
        injuries: [
          {
            bodyPart: 'Knee',
            injuryType: InjuryType.Chronic,
            movementRestrictions: 'Avoid deep squats',
          },
        ],
      };

      const result = injuriesSchema.safeParse(validData);
      expect(result.success).toBe(true);
    });
  });

  describe('trainingBackgroundSchema', () => {
    it('should accept valid training background', () => {
      const validData = {
        trainingExperience: 'some',
        equipmentFamiliarity: 'Comfortable with barbells',
        additionalNotes: 'Prefer morning workouts',
      };

      const result = trainingBackgroundSchema.safeParse(validData);
      expect(result.success).toBe(true);
    });

    it('should reject invalid training experience value', () => {
      const invalidData = {
        trainingExperience: 'invalid',
      };

      const result = trainingBackgroundSchema.safeParse(invalidData);
      expect(result.success).toBe(false);
    });
  });

  describe('onboardingFormSchema', () => {
    it('should accept complete valid form data', () => {
      const validData = {
        hyroxLevel: FitnessLevel.Intermediate,
        runningLevel: FitnessLevel.Advanced,
        strengthLevel: FitnessLevel.Beginner,
        goals: [
          {
            goalType: GoalType.HyroxRace,
            description: 'Complete HYROX race',
            targetDate: new Date('2026-06-01'),
            isPrimary: true,
          },
        ],
        schedule: {
          monday: true,
          tuesday: true,
          wednesday: false,
          thursday: true,
          friday: false,
          saturday: false,
          sunday: false,
        },
        hasInjuries: false,
        injuries: [],
        trainingExperience: 'some',
        equipmentFamiliarity: 'Intermediate',
        additionalNotes: 'Prefer evening sessions',
      };

      const result = onboardingFormSchema.safeParse(validData);
      expect(result.success).toBe(true);
    });
  });
});
