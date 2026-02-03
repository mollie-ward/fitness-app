/**
 * Progress tracking state management with Zustand
 */

import { create } from 'zustand';
import type {
  CompletionStatsDto,
  OverallStatsDto,
  StreakInfoDto,
  CompletionHistoryDto,
} from '@/types/progress';

interface ProgressState {
  // Cached stats
  weeklyStats: CompletionStatsDto | null;
  monthlyStats: CompletionStatsDto | null;
  overallStats: OverallStatsDto | null;
  streakInfo: StreakInfoDto | null;
  completionHistory: CompletionHistoryDto[];
  
  // Loading states
  isLoadingWeekly: boolean;
  isLoadingMonthly: boolean;
  isLoadingOverall: boolean;
  isLoadingStreak: boolean;
  isLoadingHistory: boolean;
  
  // Actions
  setWeeklyStats: (stats: CompletionStatsDto | null) => void;
  setMonthlyStats: (stats: CompletionStatsDto | null) => void;
  setOverallStats: (stats: OverallStatsDto | null) => void;
  setStreakInfo: (streak: StreakInfoDto | null) => void;
  setCompletionHistory: (history: CompletionHistoryDto[]) => void;
  
  setLoadingWeekly: (loading: boolean) => void;
  setLoadingMonthly: (loading: boolean) => void;
  setLoadingOverall: (loading: boolean) => void;
  setLoadingStreak: (loading: boolean) => void;
  setLoadingHistory: (loading: boolean) => void;
  
  // Clear all cached data
  clearCache: () => void;
  
  // Invalidate specific data (trigger refetch)
  invalidateWeekly: () => void;
  invalidateMonthly: () => void;
  invalidateOverall: () => void;
  invalidateStreak: () => void;
  invalidateHistory: () => void;
  
  // Invalidate all data after completion/undo
  invalidateAll: () => void;
}

const useProgressStore = create<ProgressState>((set) => ({
  // Initial state
  weeklyStats: null,
  monthlyStats: null,
  overallStats: null,
  streakInfo: null,
  completionHistory: [],
  
  isLoadingWeekly: false,
  isLoadingMonthly: false,
  isLoadingOverall: false,
  isLoadingStreak: false,
  isLoadingHistory: false,
  
  // Setters
  setWeeklyStats: (stats) => set({ weeklyStats: stats }),
  setMonthlyStats: (stats) => set({ monthlyStats: stats }),
  setOverallStats: (stats) => set({ overallStats: stats }),
  setStreakInfo: (streak) => set({ streakInfo: streak }),
  setCompletionHistory: (history) => set({ completionHistory: history }),
  
  setLoadingWeekly: (loading) => set({ isLoadingWeekly: loading }),
  setLoadingMonthly: (loading) => set({ isLoadingMonthly: loading }),
  setLoadingOverall: (loading) => set({ isLoadingOverall: loading }),
  setLoadingStreak: (loading) => set({ isLoadingStreak: loading }),
  setLoadingHistory: (loading) => set({ isLoadingHistory: loading }),
  
  // Cache management
  clearCache: () =>
    set({
      weeklyStats: null,
      monthlyStats: null,
      overallStats: null,
      streakInfo: null,
      completionHistory: [],
    }),
  
  invalidateWeekly: () => set({ weeklyStats: null }),
  invalidateMonthly: () => set({ monthlyStats: null }),
  invalidateOverall: () => set({ overallStats: null }),
  invalidateStreak: () => set({ streakInfo: null }),
  invalidateHistory: () => set({ completionHistory: [] }),
  
  invalidateAll: () =>
    set({
      weeklyStats: null,
      monthlyStats: null,
      overallStats: null,
      streakInfo: null,
      completionHistory: [],
    }),
}));

export default useProgressStore;
