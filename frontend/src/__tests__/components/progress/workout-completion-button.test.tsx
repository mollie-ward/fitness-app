/**
 * Component tests for WorkoutCompletionButton
 */

import { describe, it, expect, jest } from '@jest/globals';
import { render, screen, fireEvent } from '@testing-library/react';
import { WorkoutCompletionButton } from '@/components/progress/WorkoutCompletionButton';

describe('WorkoutCompletionButton', () => {
  it('should render "Mark Complete" button when not completed', () => {
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        onComplete={jest.fn()}
      />
    );

    expect(screen.getByRole('button', { name: /mark workout complete/i })).toBeInTheDocument();
    expect(screen.getByText(/mark complete/i)).toBeInTheDocument();
  });

  it('should render "Completed" button when completed', () => {
    render(
      <WorkoutCompletionButton
        isCompleted={true}
        onComplete={jest.fn()}
      />
    );

    expect(screen.getByRole('button', { name: /workout completed/i })).toBeInTheDocument();
    expect(screen.getByText(/completed/i)).toBeInTheDocument();
  });

  it('should render "Undo" button when completed and showUndo is true', () => {
    render(
      <WorkoutCompletionButton
        isCompleted={true}
        onComplete={jest.fn()}
        onUndo={jest.fn()}
        showUndo={true}
      />
    );

    expect(screen.getByRole('button', { name: /undo completion/i })).toBeInTheDocument();
    expect(screen.getByText(/undo/i)).toBeInTheDocument();
  });

  it('should call onComplete when clicked and not completed', () => {
    const handleComplete = jest.fn();
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        onComplete={handleComplete}
      />
    );

    fireEvent.click(screen.getByRole('button'));
    expect(handleComplete).toHaveBeenCalledTimes(1);
  });

  it('should call onUndo when clicked and showUndo is true', () => {
    const handleUndo = jest.fn();
    render(
      <WorkoutCompletionButton
        isCompleted={true}
        onComplete={jest.fn()}
        onUndo={handleUndo}
        showUndo={true}
      />
    );

    fireEvent.click(screen.getByRole('button'));
    expect(handleUndo).toHaveBeenCalledTimes(1);
  });

  it('should show loading state', () => {
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        isLoading={true}
        onComplete={jest.fn()}
      />
    );

    expect(screen.getByRole('button', { name: /processing/i })).toBeInTheDocument();
    expect(screen.getByText(/processing/i)).toBeInTheDocument();
  });

  it('should be disabled when disabled prop is true', () => {
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        disabled={true}
        onComplete={jest.fn()}
      />
    );

    expect(screen.getByRole('button')).toBeDisabled();
  });

  it('should be disabled when loading', () => {
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        isLoading={true}
        onComplete={jest.fn()}
      />
    );

    expect(screen.getByRole('button')).toBeDisabled();
  });

  it('should be keyboard accessible with Enter key', () => {
    const handleComplete = jest.fn();
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        onComplete={handleComplete}
      />
    );

    const button = screen.getByRole('button');
    fireEvent.keyDown(button, { key: 'Enter' });
    expect(handleComplete).toHaveBeenCalledTimes(1);
  });

  it('should be keyboard accessible with Space key', () => {
    const handleComplete = jest.fn();
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        onComplete={handleComplete}
      />
    );

    const button = screen.getByRole('button');
    fireEvent.keyDown(button, { key: ' ' });
    expect(handleComplete).toHaveBeenCalledTimes(1);
  });

  it('should have proper ARIA attributes', () => {
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        onComplete={jest.fn()}
      />
    );

    const button = screen.getByRole('button');
    expect(button).toHaveAttribute('aria-live', 'polite');
    expect(button).toHaveAttribute('aria-busy', 'false');
  });

  it('should update ARIA busy when loading', () => {
    render(
      <WorkoutCompletionButton
        isCompleted={false}
        isLoading={true}
        onComplete={jest.fn()}
      />
    );

    const button = screen.getByRole('button');
    expect(button).toHaveAttribute('aria-busy', 'true');
  });
});
