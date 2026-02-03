/**
 * Component tests for StreakDisplay
 */

import { describe, it, expect, jest, beforeEach } from '@jest/globals';
import { render, screen } from '@testing-library/react';
import { StreakDisplay } from '@/components/progress/StreakDisplay';

// Mock the hooks module
const mockUseStreakInfo = jest.fn();

jest.mock('@/hooks/progress/useProgressStats', () => ({
  useStreakInfo: () => mockUseStreakInfo(),
}));

describe('StreakDisplay', () => {
  beforeEach(() => {
    // Reset mocks
    jest.clearAllMocks();
  });

  it('should render loading state', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: null,
      isLoading: true,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    // Skeleton should be in document during loading (checking for a skeleton element)
    const skeletons = screen.queryAllByRole('presentation');
    expect(skeletons.length).toBeGreaterThan(0);
  });

  it('should render empty state when no streak data', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: null,
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    expect(screen.getByText(/start your streak today/i)).toBeInTheDocument();
  });

  it('should display current streak correctly', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 7,
        longestStreak: 10,
        currentWeeklyStreak: 1,
        longestWeeklyStreak: 2,
        daysUntilNextMilestone: 7,
        nextMilestone: 14,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    
    expect(screen.getByText('7')).toBeInTheDocument();
    expect(screen.getByText(/days streak/i)).toBeInTheDocument();
    expect(screen.getByText('🔥')).toBeInTheDocument();
  });

  it('should display singular "day" for 1-day streak', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 1,
        longestStreak: 5,
        currentWeeklyStreak: 1,
        longestWeeklyStreak: 2,
        daysUntilNextMilestone: 6,
        nextMilestone: 7,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    expect(screen.getByText(/day streak/i)).toBeInTheDocument();
  });

  it('should display personal best', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 7,
        longestStreak: 15,
        currentWeeklyStreak: 1,
        longestWeeklyStreak: 3,
        daysUntilNextMilestone: 7,
        nextMilestone: 14,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    expect(screen.getByText(/personal best:/i)).toBeInTheDocument();
    expect(screen.getByText('15')).toBeInTheDocument();
  });

  it('should show new record indicator when current equals longest', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 20,
        longestStreak: 20,
        currentWeeklyStreak: 3,
        longestWeeklyStreak: 3,
        daysUntilNextMilestone: 10,
        nextMilestone: 30,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    const newRecordIcon = screen.getByLabelText(/new record/i);
    expect(newRecordIcon).toBeInTheDocument();
  });

  it('should display milestone progress', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 10,
        longestStreak: 15,
        currentWeeklyStreak: 2,
        longestWeeklyStreak: 3,
        daysUntilNextMilestone: 4,
        nextMilestone: 14,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    expect(screen.getByText(/next milestone:/i)).toBeInTheDocument();
    expect(screen.getByText(/14 days/i)).toBeInTheDocument();
    expect(screen.getByText(/4.*days to go/i)).toBeInTheDocument();
  });

  it('should display appropriate motivational message for 0-day streak', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 0,
        longestStreak: 0,
        currentWeeklyStreak: 0,
        longestWeeklyStreak: 0,
        daysUntilNextMilestone: 7,
        nextMilestone: 7,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    expect(screen.getByText(/let's start your streak today/i)).toBeInTheDocument();
  });

  it('should display appropriate motivational message for 7+ day streak', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 7,
        longestStreak: 10,
        currentWeeklyStreak: 1,
        longestWeeklyStreak: 2,
        daysUntilNextMilestone: 7,
        nextMilestone: 14,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay />);
    expect(screen.getByText(/one week down/i)).toBeInTheDocument();
  });

  it('should render in compact mode', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 5,
        longestStreak: 10,
        currentWeeklyStreak: 1,
        longestWeeklyStreak: 2,
        daysUntilNextMilestone: 2,
        nextMilestone: 7,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay compact={true} />);
    expect(screen.getByText('Streak')).toBeInTheDocument();
    // Should not show motivational message in compact mode
    expect(screen.queryByText(/building momentum/i)).not.toBeInTheDocument();
  });

  it('should hide milestone progress when showMilestoneProgress is false', () => {
    mockUseStreakInfo.mockReturnValue({
      streakInfo: {
        currentStreak: 5,
        longestStreak: 10,
        currentWeeklyStreak: 1,
        longestWeeklyStreak: 2,
        daysUntilNextMilestone: 2,
        nextMilestone: 7,
      },
      isLoading: false,
      refetch: jest.fn(),
    });

    render(<StreakDisplay showMilestoneProgress={false} />);
    expect(screen.queryByText(/next milestone/i)).not.toBeInTheDocument();
  });
});
