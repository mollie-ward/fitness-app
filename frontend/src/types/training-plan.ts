/**
 * Training plan types
 */

import { Workout } from './workout';

export interface TrainingPlan {
  id: string;
  userId: string;
  name: string;
  startDate: string; // ISO 8601 date string
  endDate: string; // ISO 8601 date string
  goalRaceDate?: string; // ISO 8601 date string
  description?: string;
  workouts: Workout[];
  createdAt: string;
  updatedAt: string;
}

export interface PlanWeek {
  weekNumber: number;
  startDate: string;
  endDate: string;
  workouts: Workout[];
}

export interface PlanMonth {
  month: number;
  year: number;
  startDate: string;
  endDate: string;
  workouts: Workout[];
}
