/**
 * Zod validation schemas for onboarding form
 */
import { z } from 'zod';
import { FitnessLevel, GoalType, InjuryType } from '@/types/onboarding';

export const fitnessLevelSchema = z.object({
  hyroxLevel: z.nativeEnum(FitnessLevel),
  runningLevel: z.nativeEnum(FitnessLevel),
  strengthLevel: z.nativeEnum(FitnessLevel),
});

export const goalSchema = z.object({
  goalType: z.nativeEnum(GoalType),
  description: z.string().min(3, 'Description must be at least 3 characters'),
  targetDate: z.date().nullable().optional(),
  isPrimary: z.boolean(),
});

export const goalsSchema = z.object({
  goals: z.array(goalSchema).min(1, 'At least one goal is required'),
});

export const scheduleSchema = z
  .object({
    monday: z.boolean(),
    tuesday: z.boolean(),
    wednesday: z.boolean(),
    thursday: z.boolean(),
    friday: z.boolean(),
    saturday: z.boolean(),
    sunday: z.boolean(),
  })
  .refine(
    (data) => {
      const selectedDays = Object.values(data).filter(Boolean).length;
      return selectedDays >= 1;
    },
    {
      message: 'At least 1 training day must be selected',
    }
  );

export const injurySchema = z.object({
  bodyPart: z.string().min(1, 'Body part is required'),
  injuryType: z.nativeEnum(InjuryType),
  movementRestrictions: z.string().optional(),
});

export const injuriesSchema = z.object({
  hasInjuries: z.boolean(),
  injuries: z.array(injurySchema).optional(),
});

export const trainingBackgroundSchema = z.object({
  trainingExperience: z.enum(['none', 'some', 'extensive']),
  equipmentFamiliarity: z.string().optional(),
  additionalNotes: z.string().optional(),
});

export const onboardingFormSchema = z.object({
  hyroxLevel: z.nativeEnum(FitnessLevel),
  runningLevel: z.nativeEnum(FitnessLevel),
  strengthLevel: z.nativeEnum(FitnessLevel),
  goals: z.array(goalSchema).min(1, 'At least one goal is required'),
  schedule: scheduleSchema,
  hasInjuries: z.boolean(),
  injuries: z.array(injurySchema),
  trainingExperience: z.enum(['none', 'some', 'extensive']),
  equipmentFamiliarity: z.string().optional(),
  additionalNotes: z.string().optional(),
});

export type OnboardingFormSchema = z.infer<typeof onboardingFormSchema>;
