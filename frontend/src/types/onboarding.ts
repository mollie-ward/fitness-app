/**
 * TypeScript types for onboarding flow
 * Maps to backend DTOs for user profile creation
 */

// Enums matching backend
export enum FitnessLevel {
  Beginner = 0,
  Intermediate = 1,
  Advanced = 2,
}

export enum GoalType {
  HyroxRace = 0,
  RunningDistance = 1,
  StrengthMilestone = 2,
  GeneralFitness = 3,
}

export enum GoalStatus {
  Active = 0,
  Completed = 1,
  Abandoned = 2,
}

export enum InjuryType {
  Acute = 0,
  Chronic = 1,
}

export enum InjuryStatus {
  Active = 0,
  Improving = 1,
  Resolved = 2,
}

// DTOs matching backend structure
export interface ScheduleAvailabilityDto {
  monday: boolean;
  tuesday: boolean;
  wednesday: boolean;
  thursday: boolean;
  friday: boolean;
  saturday: boolean;
  sunday: boolean;
  minimumSessionsPerWeek: number;
  maximumSessionsPerWeek: number;
}

export interface TrainingGoalDto {
  id?: string;
  goalType: GoalType;
  description: string;
  targetDate?: string | null;
  priority: number;
  status: GoalStatus;
}

export interface InjuryLimitationDto {
  id?: string;
  bodyPart: string;
  injuryType: InjuryType;
  reportedDate: string;
  status: InjuryStatus;
  movementRestrictions?: string | null;
}

export interface TrainingBackgroundDto {
  hasStructuredTrainingExperience: boolean;
  previousTrainingDetails?: string | null;
  equipmentFamiliarity?: string | null;
  trainingHistoryDetails?: string | null;
}

export interface UserProfileDto {
  id?: string;
  name: string;
  email: string;
  hyroxLevel: FitnessLevel;
  runningLevel: FitnessLevel;
  strengthLevel: FitnessLevel;
  scheduleAvailability?: ScheduleAvailabilityDto | null;
  trainingBackground?: TrainingBackgroundDto | null;
  trainingGoals: TrainingGoalDto[];
  injuryLimitations: InjuryLimitationDto[];
}

// Form state types for the onboarding wizard
export interface OnboardingFormData {
  // Step 2: Fitness Levels
  hyroxLevel: FitnessLevel;
  runningLevel: FitnessLevel;
  strengthLevel: FitnessLevel;

  // Step 3: Goals
  goals: Array<{
    goalType: GoalType;
    description: string;
    targetDate?: Date | null;
    isPrimary: boolean;
  }>;

  // Step 4: Schedule
  schedule: {
    monday: boolean;
    tuesday: boolean;
    wednesday: boolean;
    thursday: boolean;
    friday: boolean;
    saturday: boolean;
    sunday: boolean;
  };

  // Step 5: Injuries (optional)
  hasInjuries: boolean;
  injuries: Array<{
    bodyPart: string;
    injuryType: InjuryType;
    movementRestrictions?: string;
  }>;

  // Step 6: Training Background
  trainingExperience: 'none' | 'some' | 'extensive';
  equipmentFamiliarity?: string;
  additionalNotes?: string;
}

export type OnboardingStep = 1 | 2 | 3 | 4 | 5 | 6;

export const FITNESS_LEVEL_LABELS: Record<FitnessLevel, string> = {
  [FitnessLevel.Beginner]: 'Beginner',
  [FitnessLevel.Intermediate]: 'Intermediate',
  [FitnessLevel.Advanced]: 'Advanced',
};

export const FITNESS_LEVEL_DESCRIPTIONS: Record<FitnessLevel, string> = {
  [FitnessLevel.Beginner]: 'New to this discipline or just getting started',
  [FitnessLevel.Intermediate]: 'Some experience and comfortable with basics',
  [FitnessLevel.Advanced]: 'Experienced and consistently training',
};

export const GOAL_TYPE_LABELS: Record<GoalType, string> = {
  [GoalType.HyroxRace]: 'HYROX Race',
  [GoalType.RunningDistance]: 'Running Event',
  [GoalType.StrengthMilestone]: 'Strength Milestone',
  [GoalType.GeneralFitness]: 'General Fitness',
};

export const INJURY_TYPE_LABELS: Record<InjuryType, string> = {
  [InjuryType.Acute]: 'Acute (Recent)',
  [InjuryType.Chronic]: 'Chronic (Long-term)',
};

export const BODY_PARTS = [
  'Shoulder',
  'Elbow',
  'Wrist',
  'Lower Back',
  'Upper Back',
  'Hip',
  'Knee',
  'Ankle',
  'Foot',
  'Neck',
  'Other',
] as const;

export const DAYS_OF_WEEK = [
  { key: 'monday' as const, label: 'Monday', abbrev: 'Mon' },
  { key: 'tuesday' as const, label: 'Tuesday', abbrev: 'Tue' },
  { key: 'wednesday' as const, label: 'Wednesday', abbrev: 'Wed' },
  { key: 'thursday' as const, label: 'Thursday', abbrev: 'Thu' },
  { key: 'friday' as const, label: 'Friday', abbrev: 'Fri' },
  { key: 'saturday' as const, label: 'Saturday', abbrev: 'Sat' },
  { key: 'sunday' as const, label: 'Sunday', abbrev: 'Sun' },
] as const;
