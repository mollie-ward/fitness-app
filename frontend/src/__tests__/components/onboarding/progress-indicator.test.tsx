/**
 * Component tests for ProgressIndicator
 */
import { describe, it, expect } from '@jest/globals';
import { render, screen } from '@testing-library/react';
import { ProgressIndicator } from '@/components/onboarding/progress-indicator';

describe('ProgressIndicator', () => {
  it('should display current step and total steps', () => {
    render(<ProgressIndicator currentStep={2} totalSteps={6} />);

    expect(screen.getByText('Step 2 of 6')).toBeInTheDocument();
  });

  it('should display step label', () => {
    render(<ProgressIndicator currentStep={2} totalSteps={6} />);

    expect(screen.getByText('Fitness Levels')).toBeInTheDocument();
  });

  it('should calculate progress percentage correctly for first step', () => {
    const { container } = render(<ProgressIndicator currentStep={2} totalSteps={6} />);

    const progressBar = container.querySelector('.bg-blue-600');
    expect(progressBar).toHaveStyle({ width: '20%' });
  });

  it('should calculate progress percentage correctly for middle step', () => {
    const { container } = render(<ProgressIndicator currentStep={4} totalSteps={6} />);

    const progressBar = container.querySelector('.bg-blue-600');
    expect(progressBar).toHaveStyle({ width: '60%' });
  });

  it('should calculate progress percentage correctly for last step', () => {
    const { container } = render(<ProgressIndicator currentStep={6} totalSteps={6} />);

    const progressBar = container.querySelector('.bg-blue-600');
    expect(progressBar).toHaveStyle({ width: '100%' });
  });

  it('should have proper ARIA attributes', () => {
    const { container } = render(<ProgressIndicator currentStep={3} totalSteps={6} />);

    const progressBar = container.querySelector('[role="progressbar"]');
    expect(progressBar).toHaveAttribute('aria-valuenow', '3');
    expect(progressBar).toHaveAttribute('aria-valuemin', '1');
    expect(progressBar).toHaveAttribute('aria-valuemax', '6');
  });

  it('should display correct step labels for all steps', () => {
    const stepLabels = [
      { step: 1, label: 'Welcome' },
      { step: 2, label: 'Fitness Levels' },
      { step: 3, label: 'Goals' },
      { step: 4, label: 'Schedule' },
      { step: 5, label: 'Injuries' },
      { step: 6, label: 'Training Background' },
    ];

    stepLabels.forEach(({ step, label }) => {
      const { rerender } = render(<ProgressIndicator currentStep={step as any} totalSteps={6} />);
      expect(screen.getByText(label)).toBeInTheDocument();
      rerender(<div />);
    });
  });
});
