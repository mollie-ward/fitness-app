/**
 * useWorkoutData - Hook for fetching and managing workout data
 * 
 * Note: This is a placeholder implementation. 
 * In production, this would use React Query or SWR to fetch from the backend API.
 */

import { useState, useEffect } from 'react';
import { Workout, WorkoutStatus } from '@/types/workout';
import { Discipline } from '@/types/discipline';

export interface UseWorkoutDataOptions {
  userId: string;
  startDate?: string;
  endDate?: string;
}

export interface UseWorkoutDataReturn {
  workouts: Workout[];
  isLoading: boolean;
  error: Error | null;
  refetch: () => void;
  updateWorkoutStatus: (workoutId: string, status: WorkoutStatus) => void;
}

// Mock data for development
const mockWorkouts: Workout[] = [
  {
    id: '1',
    name: 'HYROX Simulation',
    discipline: Discipline.HYROX,
    scheduledDate: new Date().toISOString().split('T')[0],
    duration: 60,
    description: 'Full HYROX simulation focusing on transitions',
    status: WorkoutStatus.NOT_STARTED,
    exercises: [
      { id: 'e1', name: '1km Run', duration: 300 },
      { id: 'e2', name: '1000m SkiErg', duration: 240 },
      { id: 'e3', name: '1km Run', duration: 300 },
      { id: 'e4', name: '50m Sled Push', distance: 50, sets: 1, reps: 1 },
    ],
    focusArea: 'Transition Speed',
    rationale: 'Building race-specific endurance',
    isKeyWorkout: true,
  },
  {
    id: '2',
    name: 'Easy Recovery Run',
    discipline: Discipline.RUNNING,
    scheduledDate: new Date(Date.now() + 86400000).toISOString().split('T')[0],
    duration: 30,
    description: 'Light recovery run at easy pace',
    status: WorkoutStatus.NOT_STARTED,
    exercises: [{ id: 'e5', name: 'Easy Run', duration: 1800, distance: 5000 }],
    focusArea: 'Recovery',
  },
  {
    id: '3',
    name: 'Upper Body Strength',
    discipline: Discipline.STRENGTH,
    scheduledDate: new Date(Date.now() + 2 * 86400000).toISOString().split('T')[0],
    duration: 45,
    description: 'Upper body strength training',
    status: WorkoutStatus.NOT_STARTED,
    exercises: [
      { id: 'e6', name: 'Bench Press', sets: 4, reps: 8 },
      { id: 'e7', name: 'Pull-ups', sets: 4, reps: 10 },
      { id: 'e8', name: 'Overhead Press', sets: 3, reps: 10 },
    ],
    focusArea: 'Upper Body Power',
  },
];

export function useWorkoutData(options: UseWorkoutDataOptions): UseWorkoutDataReturn {
  const { userId, startDate, endDate } = options;
  const [workouts, setWorkouts] = useState<Workout[]>([]);
  const [isLoading, setIsLoading] = useState(true);
  const [error, setError] = useState<Error | null>(null);

  const fetchWorkouts = () => {
    setIsLoading(true);
    setError(null);

    // Simulate API call
    setTimeout(() => {
      try {
        // In production, this would be:
        // const data = await trainingPlanService.getWorkouts(userId, startDate, endDate);
        
        let filteredWorkouts = [...mockWorkouts];

        // Filter by date range if provided
        if (startDate && endDate) {
          filteredWorkouts = filteredWorkouts.filter((workout) => {
            return workout.scheduledDate >= startDate && workout.scheduledDate <= endDate;
          });
        }

        setWorkouts(filteredWorkouts);
        setIsLoading(false);
      } catch (err) {
        setError(err as Error);
        setIsLoading(false);
      }
    }, 500);
  };

  useEffect(() => {
    fetchWorkouts();
    // eslint-disable-next-line react-hooks/exhaustive-deps
  }, [userId, startDate, endDate]);

  const updateWorkoutStatus = (workoutId: string, status: WorkoutStatus) => {
    setWorkouts((prev) =>
      prev.map((workout) =>
        workout.id === workoutId
          ? {
              ...workout,
              status,
              completedAt:
                status === WorkoutStatus.COMPLETED ? new Date().toISOString() : undefined,
            }
          : workout
      )
    );

    // In production, this would also update the backend:
    // await trainingPlanService.updateWorkoutStatus(workoutId, status);
  };

  return {
    workouts,
    isLoading,
    error,
    refetch: fetchWorkouts,
    updateWorkoutStatus,
  };
}
