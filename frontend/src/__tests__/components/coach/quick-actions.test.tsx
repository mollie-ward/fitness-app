/**
 * Tests for QuickActions component
 */
import { describe, it, expect, jest } from '@jest/globals';
import { render, screen, fireEvent } from '@testing-library/react';
import { QuickActions } from '@/components/coach/QuickActions';

describe('QuickActions', () => {
  it('should render all quick action buttons', () => {
    const mockOnSelect = jest.fn();
    render(<QuickActions onSelectAction={mockOnSelect} />);
    
    expect(screen.getByText('Why this workout?')).toBeDefined();
    expect(screen.getByText('Make it harder')).toBeDefined();
    expect(screen.getByText('Make it easier')).toBeDefined();
    expect(screen.getByText("I'm injured")).toBeDefined();
    expect(screen.getByText('Change my schedule')).toBeDefined();
    expect(screen.getByText('How am I doing?')).toBeDefined();
  });

  it('should call onSelectAction with correct prompt when button is clicked', () => {
    const mockOnSelect = jest.fn();
    render(<QuickActions onSelectAction={mockOnSelect} />);
    
    const button = screen.getByText('Why this workout?');
    fireEvent.click(button);
    
    expect(mockOnSelect).toHaveBeenCalledWith('Why am I doing this workout today?');
  });

  it('should disable all buttons when disabled prop is true', () => {
    const mockOnSelect = jest.fn();
    render(<QuickActions onSelectAction={mockOnSelect} disabled={true} />);
    
    const buttons = screen.getAllByRole('button');
    buttons.forEach((button) => {
      expect(button).toHaveProperty('disabled', true);
    });
  });

  it('should enable all buttons when disabled prop is false', () => {
    const mockOnSelect = jest.fn();
    render(<QuickActions onSelectAction={mockOnSelect} disabled={false} />);
    
    const buttons = screen.getAllByRole('button');
    buttons.forEach((button) => {
      expect(button).toHaveProperty('disabled', false);
    });
  });

  it('should display label for quick actions', () => {
    const mockOnSelect = jest.fn();
    render(<QuickActions onSelectAction={mockOnSelect} />);
    
    expect(screen.getByText('Quick actions:')).toBeDefined();
  });

  it('should trigger different prompts for different buttons', () => {
    const mockOnSelect = jest.fn();
    render(<QuickActions onSelectAction={mockOnSelect} />);
    
    fireEvent.click(screen.getByText('Make it harder'));
    expect(mockOnSelect).toHaveBeenCalledWith('Can you make my workouts harder?');
    
    fireEvent.click(screen.getByText('How am I doing?'));
    expect(mockOnSelect).toHaveBeenCalledWith('How is my progress looking?');
  });
});
