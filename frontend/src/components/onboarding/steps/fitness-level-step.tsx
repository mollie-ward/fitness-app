/**
 * Step 2: Fitness Level Assessment
 */
import React from 'react';
import { useFormContext, Controller } from 'react-hook-form';
import { CardHeader, CardTitle, CardDescription, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import {
  FitnessLevel,
  FITNESS_LEVEL_LABELS,
  FITNESS_LEVEL_DESCRIPTIONS,
  OnboardingFormData,
} from '@/types/onboarding';

interface FitnessLevelSelectorProps {
  name: 'hyroxLevel' | 'runningLevel' | 'strengthLevel';
  label: string;
  description: string;
}

function FitnessLevelSelector({ name, label, description }: FitnessLevelSelectorProps) {
  const { control, formState: { errors } } = useFormContext<OnboardingFormData>();
  
  return (
    <div className="space-y-3">
      <div>
        <Label className="text-base font-semibold">{label}</Label>
        <p className="text-sm text-gray-500 mt-1">{description}</p>
      </div>
      
      <Controller
        name={name}
        control={control}
        render={({ field }) => (
          <div className="grid grid-cols-1 md:grid-cols-3 gap-3">
            {Object.values(FitnessLevel)
              .filter((v) => typeof v === 'number')
              .map((level) => {
                const isSelected = field.value === level;
                return (
                  <button
                    key={level}
                    type="button"
                    onClick={() => field.onChange(level)}
                    className={`
                      relative p-4 rounded-lg border-2 text-left transition-all
                      ${
                        isSelected
                          ? 'border-blue-600 bg-blue-50 shadow-md'
                          : 'border-gray-200 hover:border-gray-300 bg-white'
                      }
                    `}
                    aria-pressed={isSelected}
                  >
                    <div className="flex items-start justify-between">
                      <div className="flex-1">
                        <div className="font-semibold text-gray-900">
                          {FITNESS_LEVEL_LABELS[level as FitnessLevel]}
                        </div>
                        <div className="text-sm text-gray-600 mt-1">
                          {FITNESS_LEVEL_DESCRIPTIONS[level as FitnessLevel]}
                        </div>
                      </div>
                      {isSelected && (
                        <div className="ml-2">
                          <svg
                            className="w-5 h-5 text-blue-600"
                            fill="currentColor"
                            viewBox="0 0 20 20"
                          >
                            <path
                              fillRule="evenodd"
                              d="M10 18a8 8 0 100-16 8 8 0 000 16zm3.707-9.293a1 1 0 00-1.414-1.414L9 10.586 7.707 9.293a1 1 0 00-1.414 1.414l2 2a1 1 0 001.414 0l4-4z"
                              clipRule="evenodd"
                            />
                          </svg>
                        </div>
                      )}
                    </div>
                  </button>
                );
              })}
          </div>
        )}
      />
      {errors[name] && (
        <p className="text-sm text-red-600">{errors[name]?.message}</p>
      )}
    </div>
  );
}

export function FitnessLevelStep() {
  return (
    <>
      <CardHeader>
        <CardTitle>What's Your Current Fitness Level?</CardTitle>
        <CardDescription>
          Be honest about your current abilities. This helps us create a plan that's challenging 
          but not overwhelming. You can set different levels for each discipline.
        </CardDescription>
      </CardHeader>

      <CardContent className="space-y-6">
        <FitnessLevelSelector
          name="hyroxLevel"
          label="HYROX"
          description="Hybrid fitness combining running and functional exercises"
        />

        <div className="border-t pt-6" />

        <FitnessLevelSelector
          name="runningLevel"
          label="Running"
          description="Cardiovascular endurance and running ability"
        />

        <div className="border-t pt-6" />

        <FitnessLevelSelector
          name="strengthLevel"
          label="Strength Training"
          description="Weightlifting and resistance training experience"
        />
      </CardContent>
    </>
  );
}
