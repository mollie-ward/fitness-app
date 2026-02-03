/**
 * Workout-related types
 */

import { Discipline } from './discipline';

export enum WorkoutStatus {
  NOT_STARTED = 'NOT_STARTED',
  IN_PROGRESS = 'IN_PROGRESS',
  COMPLETED = 'COMPLETED',
  MISSED = 'MISSED',
  SKIPPED = 'SKIPPED',
}

export interface Exercise {
  id: string;
  name: string;
  sets?: number;
  reps?: number;
  duration?: number; // in seconds
  distance?: number; // in meters
  notes?: string;
}

export interface Workout {
  id: string;
  name: string;
  discipline: Discipline;
  scheduledDate: string; // ISO 8601 date string
  duration: number; // in minutes
  description: string;
  status: WorkoutStatus;
  exercises: Exercise[];
  rationale?: string;
  focusArea?: string;
  isKeyWorkout?: boolean;
  completedAt?: string; // ISO 8601 datetime string
}

export interface WorkoutSummary {
  total: number;
  completed: number;
  missed: number;
  upcoming: number;
}
