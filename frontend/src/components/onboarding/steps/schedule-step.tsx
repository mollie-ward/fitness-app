/**
 * Step 4: Schedule Configuration
 */
import React, { useMemo } from 'react';
import { useFormContext, Controller } from 'react-hook-form';
import { CardHeader, CardTitle, CardDescription, CardContent } from '@/components/ui/card';
import { DAYS_OF_WEEK, OnboardingFormData } from '@/types/onboarding';

export function ScheduleStep() {
  const {
    control,
    watch,
    formState: { errors },
  } = useFormContext<OnboardingFormData>();

  const schedule = watch('schedule');
  
  const selectedDaysCount = useMemo(() => {
    return Object.values(schedule).filter(Boolean).length;
  }, [schedule]);

  return (
    <>
      <CardHeader>
        <CardTitle>When Can You Train?</CardTitle>
        <CardDescription>
          Select the days of the week you're available for training sessions. We'll build your plan 
          around your schedule. You must select at least one day.
        </CardDescription>
      </CardHeader>

      <CardContent className="space-y-6">
        <div className="grid grid-cols-2 md:grid-cols-4 lg:grid-cols-7 gap-3">
          {DAYS_OF_WEEK.map(({ key, label, abbrev }) => (
            <Controller
              key={key}
              name={`schedule.${key}`}
              control={control}
              render={({ field }) => {
                const isSelected = field.value;
                return (
                  <button
                    type="button"
                    onClick={() => field.onChange(!field.value)}
                    className={`
                      flex flex-col items-center justify-center p-4 rounded-lg border-2 
                      transition-all aspect-square
                      ${
                        isSelected
                          ? 'border-blue-600 bg-blue-50 shadow-md'
                          : 'border-gray-200 hover:border-gray-300 bg-white'
                      }
                    `}
                    aria-pressed={isSelected}
                    aria-label={`${isSelected ? 'Unselect' : 'Select'} ${label}`}
                  >
                    <div
                      className={`
                        text-lg font-bold mb-1
                        ${isSelected ? 'text-blue-600' : 'text-gray-700'}
                      `}
                    >
                      {abbrev}
                    </div>
                    <div className="hidden md:block text-xs text-gray-500">
                      {label}
                    </div>
                    {isSelected && (
                      <svg
                        className="w-5 h-5 text-blue-600 mt-1"
                        fill="currentColor"
                        viewBox="0 0 20 20"
                      >
                        <path
                          fillRule="evenodd"
                          d="M16.707 5.293a1 1 0 010 1.414l-8 8a1 1 0 01-1.414 0l-4-4a1 1 0 011.414-1.414L8 12.586l7.293-7.293a1 1 0 011.414 0z"
                          clipRule="evenodd"
                        />
                      </svg>
                    )}
                  </button>
                );
              }}
            />
          ))}
        </div>

        <div className="bg-blue-50 border border-blue-200 rounded-lg p-4 text-center">
          <p className="text-blue-900 font-semibold">
            {selectedDaysCount === 0 ? (
              <span className="text-red-600">Please select at least 1 training day</span>
            ) : (
              <>
                You've selected{' '}
                <span className="text-2xl font-bold">{selectedDaysCount}</span>{' '}
                {selectedDaysCount === 1 ? 'day' : 'days'} per week
              </>
            )}
          </p>
          <p className="text-sm text-blue-700 mt-1">
            {selectedDaysCount >= 3 &&
              selectedDaysCount <= 5 &&
              'Great choice! This gives us flexibility to create an effective plan.'}
            {selectedDaysCount === 1 &&
              'We can work with that, though more days allows for better progress.'}
            {selectedDaysCount === 2 &&
              'Good start! Consider adding more days if your schedule allows.'}
            {selectedDaysCount >= 6 &&
              'Awesome commitment! Make sure to include rest days for recovery.'}
          </p>
        </div>

        {errors.schedule && (
          <p className="text-sm text-red-600 text-center">{errors.schedule.message}</p>
        )}

        <div className="bg-gray-50 border border-gray-200 rounded-lg p-4">
          <h4 className="font-semibold text-gray-900 mb-2">💡 Training Frequency Tips</h4>
          <ul className="text-sm text-gray-700 space-y-1">
            <li>• <strong>Beginners:</strong> Start with 3-4 days per week</li>
            <li>• <strong>Intermediate:</strong> 4-5 days works well for balanced progress</li>
            <li>• <strong>Advanced:</strong> 5-6 days with proper recovery planning</li>
            <li>• Always include at least 1-2 rest days for recovery</li>
          </ul>
        </div>
      </CardContent>
    </>
  );
}
