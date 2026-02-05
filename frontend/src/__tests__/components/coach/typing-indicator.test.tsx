/**
 * Tests for TypingIndicator component
 */
import { describe, it, expect } from '@jest/globals';
import { render, screen } from '@testing-library/react';
import { TypingIndicator } from '@/components/coach/TypingIndicator';

describe('TypingIndicator', () => {
  it('should render Coach Tom avatar', () => {
    render(<TypingIndicator />);
    expect(screen.getByAltText('Coach Tom')).toBeDefined();
  });

  it('should have proper aria label for accessibility', () => {
    render(<TypingIndicator />);
    expect(screen.getByLabelText('Coach Tom is typing')).toBeDefined();
  });

  it('should render three animated dots', () => {
    const { container } = render(<TypingIndicator />);
    const dots = container.querySelectorAll('.animate-bounce');
    expect(dots.length).toBe(3);
  });
});
