/**
 * Component tests for WorkoutCard
 */

import { describe, it, expect, jest } from '@jest/globals';
import { render, screen, fireEvent } from '@testing-library/react';
import { WorkoutCard } from '@/components/calendar/workout/WorkoutCard';
import { Workout, WorkoutStatus } from '@/types/workout';
import { Discipline } from '@/types/discipline';

const mockWorkout: Workout = {
  id: '1',
  name: 'HYROX Simulation',
  discipline: Discipline.HYROX,
  scheduledDate: '2026-02-15',
  duration: 60,
  description: 'Full HYROX simulation focusing on transitions',
  status: WorkoutStatus.NOT_STARTED,
  exercises: [
    { id: 'e1', name: '1km Run', duration: 300 },
    { id: 'e2', name: 'SkiErg', duration: 240 },
  ],
  focusArea: 'Transitions',
  isKeyWorkout: true,
};

describe('WorkoutCard', () => {
  it('should render workout name and discipline', () => {
    render(<WorkoutCard workout={mockWorkout} />);
    expect(screen.getByText('HYROX Simulation')).toBeInTheDocument();
  });

  it('should render workout duration', () => {
    render(<WorkoutCard workout={mockWorkout} />);
    expect(screen.getByText(/60 min/i)).toBeInTheDocument();
  });

  it('should render workout description when not compact', () => {
    render(<WorkoutCard workout={mockWorkout} compact={false} />);
    expect(screen.getByText(/Full HYROX simulation/i)).toBeInTheDocument();
  });

  it('should not render description in compact mode', () => {
    render(<WorkoutCard workout={mockWorkout} compact={true} />);
    expect(screen.queryByText(/Full HYROX simulation/i)).not.toBeInTheDocument();
  });

  it('should show focus area when not compact', () => {
    render(<WorkoutCard workout={mockWorkout} compact={false} />);
    // Focus area shows as bullet point in UI
    expect(screen.getByText((content, element) => {
      return element !== null && content.includes('Transitions');
    })).toBeInTheDocument();
  });

  it('should call onClick when card is clicked', () => {
    const handleClick = jest.fn();
    render(<WorkoutCard workout={mockWorkout} onClick={handleClick} />);
    
    const card = screen.getByRole('button');
    fireEvent.click(card);
    
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('should be keyboard accessible', () => {
    const handleClick = jest.fn();
    render(<WorkoutCard workout={mockWorkout} onClick={handleClick} />);
    
    const card = screen.getByRole('button');
    fireEvent.keyDown(card, { key: 'Enter' });
    
    expect(handleClick).toHaveBeenCalledTimes(1);
  });

  it('should show status badge', () => {
    render(<WorkoutCard workout={mockWorkout} />);
    const badge = screen.getByRole('status');
    expect(badge).toBeInTheDocument();
  });

  it('should show key workout indicator', () => {
    render(<WorkoutCard workout={mockWorkout} />);
    const keyIndicator = screen.getByLabelText(/key workout/i);
    expect(keyIndicator).toBeInTheDocument();
  });

  it('should not show key workout indicator for regular workouts', () => {
    const regularWorkout = { ...mockWorkout, isKeyWorkout: false };
    render(<WorkoutCard workout={regularWorkout} />);
    expect(screen.queryByLabelText(/key workout/i)).not.toBeInTheDocument();
  });

  it('should show date when showDate is true', () => {
    render(<WorkoutCard workout={mockWorkout} showDate={true} />);
    expect(screen.getByText(/Feb/i)).toBeInTheDocument();
  });
});
