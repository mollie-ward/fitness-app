/**
 * Custom hook for managing onboarding form state with localStorage persistence
 */
import { useEffect } from 'react';
import { useForm, UseFormReturn } from 'react-hook-form';
import { zodResolver } from '@hookform/resolvers/zod';
import { OnboardingFormData, FitnessLevel, GoalType, InjuryType } from '@/types/onboarding';
import { onboardingFormSchema } from '@/lib/validation/onboarding-schema';

const STORAGE_KEY = 'onboarding_form_state';

const DEFAULT_FORM_VALUES: OnboardingFormData = {
  hyroxLevel: FitnessLevel.Beginner,
  runningLevel: FitnessLevel.Beginner,
  strengthLevel: FitnessLevel.Beginner,
  goals: [
    {
      goalType: GoalType.GeneralFitness,
      description: '',
      targetDate: null,
      isPrimary: true,
    },
  ],
  schedule: {
    monday: false,
    tuesday: false,
    wednesday: false,
    thursday: false,
    friday: false,
    saturday: false,
    sunday: false,
  },
  hasInjuries: false,
  injuries: [],
  trainingExperience: 'none',
  equipmentFamiliarity: '',
  additionalNotes: '',
};

/**
 * Load form state from localStorage
 */
function loadFormState(): OnboardingFormData | null {
  if (typeof window === 'undefined') return null;

  try {
    const saved = localStorage.getItem(STORAGE_KEY);
    if (!saved) return null;

    const parsed = JSON.parse(saved);
    
    // Convert date strings back to Date objects for goals
    if (parsed.goals) {
      parsed.goals = parsed.goals.map((goal: any) => ({
        ...goal,
        targetDate: goal.targetDate ? new Date(goal.targetDate) : null,
      }));
    }

    return parsed;
  } catch (error) {
    console.error('Failed to load onboarding state from localStorage:', error);
    return null;
  }
}

/**
 * Save form state to localStorage
 */
function saveFormState(data: OnboardingFormData): void {
  if (typeof window === 'undefined') return;

  try {
    localStorage.setItem(STORAGE_KEY, JSON.stringify(data));
  } catch (error) {
    console.error('Failed to save onboarding state to localStorage:', error);
  }
}

/**
 * Clear form state from localStorage
 */
export function clearOnboardingState(): void {
  if (typeof window === 'undefined') return;

  try {
    localStorage.removeItem(STORAGE_KEY);
  } catch (error) {
    console.error('Failed to clear onboarding state from localStorage:', error);
  }
}

/**
 * Hook for onboarding form with persistence
 */
export function useOnboardingForm(): UseFormReturn<OnboardingFormData> & {
  saveProgress: () => void;
} {
  const form = useForm<OnboardingFormData>({
    resolver: zodResolver(onboardingFormSchema),
    defaultValues: loadFormState() || DEFAULT_FORM_VALUES,
    mode: 'onChange',
  });

  // Save to localStorage whenever form data changes
  const { watch } = form;

  useEffect(() => {
    const subscription = watch((value) => {
      saveFormState(value as OnboardingFormData);
    });
    return () => subscription.unsubscribe();
  }, [watch]);

  const saveProgress = () => {
    const currentValues = form.getValues();
    saveFormState(currentValues);
  };

  return {
    ...form,
    saveProgress,
  };
}
