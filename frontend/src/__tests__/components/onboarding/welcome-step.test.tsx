/**
 * Component tests for WelcomeStep
 */
import { describe, it, expect, jest } from '@jest/globals';
import { render, screen, fireEvent } from '@testing-library/react';
import { WelcomeStep } from '@/components/onboarding/steps/welcome-step';

describe('WelcomeStep', () => {
  it('should render welcome message and Coach Tom avatar', () => {
    const mockOnNext = jest.fn();
    render(<WelcomeStep onNext={mockOnNext} />);

    expect(screen.getByText(/Welcome to Your Fitness Journey!/i)).toBeInTheDocument();
    expect(screen.getByText(/I'm Coach Tom!/i)).toBeInTheDocument();
    expect(screen.getByAltText('Coach Tom')).toBeInTheDocument();
  });

  it('should display onboarding overview items', () => {
    const mockOnNext = jest.fn();
    render(<WelcomeStep onNext={mockOnNext} />);

    expect(screen.getByText(/fitness level across different disciplines/i)).toBeInTheDocument();
    expect(screen.getByText(/training goals and target dates/i)).toBeInTheDocument();
    expect(screen.getByText(/weekly training availability/i)).toBeInTheDocument();
    expect(screen.getByText(/injuries or limitations/i)).toBeInTheDocument();
  });

  it('should call onNext when Get Started button is clicked', () => {
    const mockOnNext = jest.fn();
    render(<WelcomeStep onNext={mockOnNext} />);

    const button = screen.getByRole('button', { name: /Let's Get Started!/i });
    fireEvent.click(button);

    expect(mockOnNext).toHaveBeenCalledTimes(1);
  });

  it('should mention estimated time to complete', () => {
    const mockOnNext = jest.fn();
    render(<WelcomeStep onNext={mockOnNext} />);

    expect(screen.getByText(/4-5 minutes/i)).toBeInTheDocument();
  });
});
