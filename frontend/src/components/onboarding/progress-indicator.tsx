/**
 * Progress indicator showing current step in onboarding flow
 */
import React from 'react';
import { OnboardingStep } from '@/types/onboarding';

interface ProgressIndicatorProps {
  currentStep: OnboardingStep;
  totalSteps: number;
}

const STEP_LABELS = [
  '', // Index 0 (unused)
  'Welcome',
  'Fitness Levels',
  'Goals',
  'Schedule',
  'Injuries',
  'Training Background',
];

export function ProgressIndicator({ currentStep, totalSteps }: ProgressIndicatorProps) {
  const progress = ((currentStep - 1) / (totalSteps - 1)) * 100;

  return (
    <div className="w-full" role="progressbar" aria-valuenow={currentStep} aria-valuemin={1} aria-valuemax={totalSteps}>
      <div className="flex justify-between mb-2">
        <span className="text-sm font-medium text-gray-700">
          {STEP_LABELS[currentStep]}
        </span>
        <span className="text-sm text-gray-500">
          Step {currentStep} of {totalSteps}
        </span>
      </div>
      <div className="w-full bg-gray-200 rounded-full h-2">
        <div
          className="bg-blue-600 h-2 rounded-full transition-all duration-300"
          style={{ width: `${progress}%` }}
        />
      </div>
    </div>
  );
}
