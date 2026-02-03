/**
 * Hook for fetching progress statistics
 */

import * as React from 'react';
import {
  getWeeklyStats,
  getMonthlyStats,
  getOverallStats,
  getStreakInfo,
  getCompletionHistory,
} from '@/services/progress-api';
import useProgressStore from '@/lib/progress-store';
import type {
  CompletionStatsDto,
  OverallStatsDto,
  StreakInfoDto,
  CompletionHistoryDto,
} from '@/types/progress';

/**
 * Hook to fetch and cache weekly statistics
 */
export function useWeeklyStats() {
  const {
    weeklyStats,
    isLoadingWeekly,
    setWeeklyStats,
    setLoadingWeekly,
  } = useProgressStore();

  const fetchWeeklyStats = React.useCallback(async () => {
    if (weeklyStats) return; // Use cached data if available
    
    setLoadingWeekly(true);
    try {
      const stats = await getWeeklyStats();
      setWeeklyStats(stats);
    } catch (error) {
      console.error('Failed to fetch weekly stats:', error);
      setWeeklyStats(null);
    } finally {
      setLoadingWeekly(false);
    }
  }, [weeklyStats, setWeeklyStats, setLoadingWeekly]);

  React.useEffect(() => {
    fetchWeeklyStats();
  }, [fetchWeeklyStats]);

  return {
    weeklyStats,
    isLoading: isLoadingWeekly,
    refetch: fetchWeeklyStats,
  };
}

/**
 * Hook to fetch and cache monthly statistics
 */
export function useMonthlyStats() {
  const {
    monthlyStats,
    isLoadingMonthly,
    setMonthlyStats,
    setLoadingMonthly,
  } = useProgressStore();

  const fetchMonthlyStats = React.useCallback(async () => {
    if (monthlyStats) return; // Use cached data if available
    
    setLoadingMonthly(true);
    try {
      const stats = await getMonthlyStats();
      setMonthlyStats(stats);
    } catch (error) {
      console.error('Failed to fetch monthly stats:', error);
      setMonthlyStats(null);
    } finally {
      setLoadingMonthly(false);
    }
  }, [monthlyStats, setMonthlyStats, setLoadingMonthly]);

  React.useEffect(() => {
    fetchMonthlyStats();
  }, [fetchMonthlyStats]);

  return {
    monthlyStats,
    isLoading: isLoadingMonthly,
    refetch: fetchMonthlyStats,
  };
}

/**
 * Hook to fetch and cache overall statistics
 */
export function useOverallStats() {
  const {
    overallStats,
    isLoadingOverall,
    setOverallStats,
    setLoadingOverall,
  } = useProgressStore();

  const fetchOverallStats = React.useCallback(async () => {
    if (overallStats) return; // Use cached data if available
    
    setLoadingOverall(true);
    try {
      const stats = await getOverallStats();
      setOverallStats(stats);
    } catch (error) {
      console.error('Failed to fetch overall stats:', error);
      setOverallStats(null);
    } finally {
      setLoadingOverall(false);
    }
  }, [overallStats, setOverallStats, setLoadingOverall]);

  React.useEffect(() => {
    fetchOverallStats();
  }, [fetchOverallStats]);

  return {
    overallStats,
    isLoading: isLoadingOverall,
    refetch: fetchOverallStats,
  };
}

/**
 * Hook to fetch and cache streak information
 */
export function useStreakInfo() {
  const {
    streakInfo,
    isLoadingStreak,
    setStreakInfo,
    setLoadingStreak,
  } = useProgressStore();

  const fetchStreakInfo = React.useCallback(async () => {
    if (streakInfo) return; // Use cached data if available
    
    setLoadingStreak(true);
    try {
      const streak = await getStreakInfo();
      setStreakInfo(streak);
    } catch (error) {
      console.error('Failed to fetch streak info:', error);
      setStreakInfo(null);
    } finally {
      setLoadingStreak(false);
    }
  }, [streakInfo, setStreakInfo, setLoadingStreak]);

  React.useEffect(() => {
    fetchStreakInfo();
  }, [fetchStreakInfo]);

  return {
    streakInfo,
    isLoading: isLoadingStreak,
    refetch: fetchStreakInfo,
  };
}

/**
 * Hook to fetch completion history
 */
export function useCompletionHistory(startDate?: Date, endDate?: Date) {
  const {
    completionHistory,
    isLoadingHistory,
    setCompletionHistory,
    setLoadingHistory,
  } = useProgressStore();

  // Convert dates to strings for comparison
  const dateKey = React.useMemo(
    () => `${startDate?.toISOString() || 'none'}_${endDate?.toISOString() || 'none'}`,
    [startDate, endDate]
  );

  const fetchHistory = React.useCallback(async () => {
    setLoadingHistory(true);
    try {
      const history = await getCompletionHistory(startDate, endDate);
      setCompletionHistory(history);
    } catch (error) {
      console.error('Failed to fetch completion history:', error);
      setCompletionHistory([]);
    } finally {
      setLoadingHistory(false);
    }
  }, [setCompletionHistory, setLoadingHistory, startDate, endDate]);

  React.useEffect(() => {
    fetchHistory();
  }, [fetchHistory]);

  return {
    completionHistory,
    isLoading: isLoadingHistory,
    refetch: fetchHistory,
  };
}
