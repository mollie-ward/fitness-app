/**
 * Unit tests for onboarding API service
 */
import { describe, it, expect } from '@jest/globals';
import { transformFormDataToDto } from '@/services/onboarding-api';
import { FitnessLevel, GoalType, InjuryType, OnboardingFormData } from '@/types/onboarding';

describe('Onboarding API Service', () => {
  describe('transformFormDataToDto', () => {
    it('should correctly transform form data to DTO', () => {
      const formData: OnboardingFormData = {
        hyroxLevel: FitnessLevel.Intermediate,
        runningLevel: FitnessLevel.Advanced,
        strengthLevel: FitnessLevel.Beginner,
        goals: [
          {
            goalType: GoalType.HyroxRace,
            description: 'Complete my first HYROX race',
            targetDate: new Date('2026-06-01'),
            isPrimary: true,
          },
          {
            goalType: GoalType.GeneralFitness,
            description: 'Improve overall fitness',
            targetDate: null,
            isPrimary: false,
          },
        ],
        schedule: {
          monday: true,
          tuesday: false,
          wednesday: true,
          thursday: false,
          friday: true,
          saturday: false,
          sunday: false,
        },
        hasInjuries: false,
        injuries: [],
        trainingExperience: 'some',
        equipmentFamiliarity: 'Comfortable with basic equipment',
        additionalNotes: 'Prefer morning workouts',
      };

      const result = transformFormDataToDto(formData, 'John Doe', 'john@example.com');

      expect(result.name).toBe('John Doe');
      expect(result.email).toBe('john@example.com');
      expect(result.hyroxLevel).toBe(FitnessLevel.Intermediate);
      expect(result.runningLevel).toBe(FitnessLevel.Advanced);
      expect(result.strengthLevel).toBe(FitnessLevel.Beginner);

      // Check schedule availability
      expect(result.scheduleAvailability).toEqual({
        monday: true,
        tuesday: false,
        wednesday: true,
        thursday: false,
        friday: true,
        saturday: false,
        sunday: false,
        minimumSessionsPerWeek: 3,
        maximumSessionsPerWeek: 3,
      });

      // Check training goals
      expect(result.trainingGoals).toHaveLength(2);
      expect(result.trainingGoals[0].goalType).toBe(GoalType.HyroxRace);
      expect(result.trainingGoals[0].priority).toBe(1); // Primary goal
      expect(result.trainingGoals[1].priority).toBe(3); // Secondary goal (index 1 + 2)

      // Check training background
      expect(result.trainingBackground?.hasStructuredTrainingExperience).toBe(true);
      expect(result.trainingBackground?.equipmentFamiliarity).toBe('Comfortable with basic equipment');

      // Check injuries
      expect(result.injuryLimitations).toHaveLength(0);
    });

    it('should handle injuries when hasInjuries is true', () => {
      const formData: OnboardingFormData = {
        hyroxLevel: FitnessLevel.Beginner,
        runningLevel: FitnessLevel.Beginner,
        strengthLevel: FitnessLevel.Beginner,
        goals: [
          {
            goalType: GoalType.GeneralFitness,
            description: 'Get fit',
            targetDate: null,
            isPrimary: true,
          },
        ],
        schedule: {
          monday: true,
          tuesday: true,
          wednesday: false,
          thursday: false,
          friday: false,
          saturday: false,
          sunday: false,
        },
        hasInjuries: true,
        injuries: [
          {
            bodyPart: 'Knee',
            injuryType: InjuryType.Chronic,
            movementRestrictions: 'Avoid deep squats',
          },
        ],
        trainingExperience: 'none',
      };

      const result = transformFormDataToDto(formData, 'Jane Doe', 'jane@example.com');

      expect(result.injuryLimitations).toHaveLength(1);
      expect(result.injuryLimitations[0].bodyPart).toBe('Knee');
      expect(result.injuryLimitations[0].injuryType).toBe(InjuryType.Chronic);
      expect(result.injuryLimitations[0].movementRestrictions).toBe('Avoid deep squats');
    });

    it('should handle no training experience', () => {
      const formData: OnboardingFormData = {
        hyroxLevel: FitnessLevel.Beginner,
        runningLevel: FitnessLevel.Beginner,
        strengthLevel: FitnessLevel.Beginner,
        goals: [
          {
            goalType: GoalType.GeneralFitness,
            description: 'Get started with fitness',
            targetDate: null,
            isPrimary: true,
          },
        ],
        schedule: {
          monday: true,
          tuesday: false,
          wednesday: false,
          thursday: false,
          friday: false,
          saturday: false,
          sunday: false,
        },
        hasInjuries: false,
        injuries: [],
        trainingExperience: 'none',
      };

      const result = transformFormDataToDto(formData, 'Test User', 'test@example.com');

      expect(result.trainingBackground?.hasStructuredTrainingExperience).toBe(false);
      expect(result.trainingBackground?.previousTrainingDetails).toBe(null);
    });

    it('should handle extensive training experience', () => {
      const formData: OnboardingFormData = {
        hyroxLevel: FitnessLevel.Advanced,
        runningLevel: FitnessLevel.Advanced,
        strengthLevel: FitnessLevel.Advanced,
        goals: [
          {
            goalType: GoalType.HyroxRace,
            description: 'Win HYROX competition',
            targetDate: new Date('2026-12-01'),
            isPrimary: true,
          },
        ],
        schedule: {
          monday: true,
          tuesday: true,
          wednesday: true,
          thursday: true,
          friday: true,
          saturday: true,
          sunday: false,
        },
        hasInjuries: false,
        injuries: [],
        trainingExperience: 'extensive',
      };

      const result = transformFormDataToDto(formData, 'Pro Athlete', 'pro@example.com');

      expect(result.trainingBackground?.hasStructuredTrainingExperience).toBe(true);
      expect(result.trainingBackground?.previousTrainingDetails).toBe(
        'Extensive structured training experience'
      );
    });

    it('should correctly count selected days for min/max sessions', () => {
      const formData: OnboardingFormData = {
        hyroxLevel: FitnessLevel.Beginner,
        runningLevel: FitnessLevel.Beginner,
        strengthLevel: FitnessLevel.Beginner,
        goals: [
          {
            goalType: GoalType.GeneralFitness,
            description: 'Test',
            targetDate: null,
            isPrimary: true,
          },
        ],
        schedule: {
          monday: true,
          tuesday: true,
          wednesday: true,
          thursday: true,
          friday: true,
          saturday: false,
          sunday: false,
        },
        hasInjuries: false,
        injuries: [],
        trainingExperience: 'none',
      };

      const result = transformFormDataToDto(formData, 'User', 'user@example.com');

      expect(result.scheduleAvailability?.minimumSessionsPerWeek).toBe(5);
      expect(result.scheduleAvailability?.maximumSessionsPerWeek).toBe(5);
    });
  });
});
