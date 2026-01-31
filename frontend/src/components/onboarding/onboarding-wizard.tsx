/**
 * Main onboarding wizard container with step management
 */
'use client';

import React, { useState } from 'react';
import { useRouter } from 'next/navigation';
import { FormProvider } from 'react-hook-form';
import { useOnboardingForm, clearOnboardingState } from '@/hooks/use-onboarding-form';
import { transformFormDataToDto, createUserProfile } from '@/services/onboarding-api';
import { useAuthStore } from '@/lib/auth-store';
import { OnboardingStep } from '@/types/onboarding';
import { WelcomeStep } from './steps/welcome-step';
import { FitnessLevelStep } from './steps/fitness-level-step';
import { GoalsStep } from './steps/goals-step';
import { ScheduleStep } from './steps/schedule-step';
import { InjuriesStep } from './steps/injuries-step';
import { TrainingBackgroundStep } from './steps/training-background-step';
import { ProgressIndicator } from './progress-indicator';
import { Card } from '@/components/ui/card';

const TOTAL_STEPS = 6;

export function OnboardingWizard() {
  const [currentStep, setCurrentStep] = useState<OnboardingStep>(1);
  const [isSubmitting, setIsSubmitting] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const router = useRouter();
  const { user } = useAuthStore();
  const form = useOnboardingForm();

  const handleNext = async () => {
    // Validate current step before moving forward
    let isValid = true;

    if (currentStep === 2) {
      isValid = await form.trigger(['hyroxLevel', 'runningLevel', 'strengthLevel']);
    } else if (currentStep === 3) {
      isValid = await form.trigger(['goals']);
    } else if (currentStep === 4) {
      isValid = await form.trigger(['schedule']);
    } else if (currentStep === 5) {
      isValid = await form.trigger(['hasInjuries', 'injuries']);
    } else if (currentStep === 6) {
      isValid = await form.trigger(['trainingExperience']);
    }

    if (!isValid) {
      return;
    }

    if (currentStep < TOTAL_STEPS) {
      setCurrentStep((prev) => (prev + 1) as OnboardingStep);
      form.saveProgress();
    } else {
      await handleSubmit();
    }
  };

  const handleBack = () => {
    if (currentStep > 1) {
      setCurrentStep((prev) => (prev - 1) as OnboardingStep);
    }
  };

  const handleSkip = () => {
    // Only allow skipping on optional steps (Step 5: Injuries)
    if (currentStep === 5) {
      form.setValue('hasInjuries', false);
      form.setValue('injuries', []);
      setCurrentStep(6);
    }
  };

  const handleSubmit = async () => {
    setIsSubmitting(true);
    setError(null);

    try {
      // Validate entire form
      const isValid = await form.trigger();
      if (!isValid) {
        setError('Please complete all required fields');
        setIsSubmitting(false);
        return;
      }

      const formData = form.getValues();
      
      // Get user info from auth store
      const userName = user?.name || 'User';
      const userEmail = user?.email || '';

      // Transform form data to DTO
      const profileDto = transformFormDataToDto(formData, userName, userEmail);

      // Submit to API
      await createUserProfile(profileDto);

      // Clear localStorage state on success
      clearOnboardingState();

      // Redirect to dashboard
      router.push('/dashboard');
    } catch (err: any) {
      console.error('Failed to create profile:', err);
      setError(
        err.response?.data?.message || 
        'Failed to create profile. Please try again.'
      );
      setIsSubmitting(false);
    }
  };

  const renderStep = () => {
    switch (currentStep) {
      case 1:
        return <WelcomeStep onNext={handleNext} />;
      case 2:
        return <FitnessLevelStep />;
      case 3:
        return <GoalsStep />;
      case 4:
        return <ScheduleStep />;
      case 5:
        return <InjuriesStep />;
      case 6:
        return <TrainingBackgroundStep />;
      default:
        return null;
    }
  };

  const canGoBack = currentStep > 1;
  const canSkip = currentStep === 5; // Only injuries step is skippable
  const isLastStep = currentStep === TOTAL_STEPS;

  return (
    <div className="min-h-screen bg-gray-50 py-8 px-4">
      <div className="max-w-3xl mx-auto">
        {currentStep > 1 && (
          <ProgressIndicator currentStep={currentStep} totalSteps={TOTAL_STEPS} />
        )}

        <FormProvider {...form}>
          <form onSubmit={(e) => e.preventDefault()}>
            <Card className="mt-6">
              {renderStep()}

              {/* Navigation buttons */}
              {currentStep > 1 && (
                <div className="p-6 pt-0 flex justify-between items-center">
                  <div>
                    {canGoBack && (
                      <button
                        type="button"
                        onClick={handleBack}
                        className="text-gray-600 hover:text-gray-900 font-medium"
                        disabled={isSubmitting}
                      >
                        ← Back
                      </button>
                    )}
                  </div>

                  <div className="flex gap-3">
                    {canSkip && (
                      <button
                        type="button"
                        onClick={handleSkip}
                        className="px-4 py-2 text-gray-600 hover:text-gray-900 font-medium"
                        disabled={isSubmitting}
                      >
                        Skip
                      </button>
                    )}
                    <button
                      type="button"
                      onClick={handleNext}
                      disabled={isSubmitting}
                      className="px-6 py-2 bg-blue-600 text-white rounded-md hover:bg-blue-700 disabled:opacity-50 disabled:cursor-not-allowed font-medium"
                    >
                      {isSubmitting
                        ? 'Submitting...'
                        : isLastStep
                          ? 'Complete Setup'
                          : 'Next →'}
                    </button>
                  </div>
                </div>
              )}

              {error && (
                <div className="px-6 pb-6">
                  <div className="bg-red-50 border border-red-200 text-red-800 px-4 py-3 rounded">
                    {error}
                  </div>
                </div>
              )}
            </Card>
          </form>
        </FormProvider>
      </div>
    </div>
  );
}
