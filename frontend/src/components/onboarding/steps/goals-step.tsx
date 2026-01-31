/**
 * Step 3: Goal Setting
 */
import React from 'react';
import { useFormContext, useFieldArray, Controller } from 'react-hook-form';
import { CardHeader, CardTitle, CardDescription, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Input } from '@/components/ui/input';
import { Select } from '@/components/ui/select';
import { Button } from '@/components/ui/button';
import { GoalType, GOAL_TYPE_LABELS, OnboardingFormData } from '@/types/onboarding';

export function GoalsStep() {
  const {
    control,
    register,
    formState: { errors },
  } = useFormContext<OnboardingFormData>();

  const { fields, append, remove } = useFieldArray({
    control,
    name: 'goals',
  });

  const handleAddGoal = () => {
    append({
      goalType: GoalType.GeneralFitness,
      description: '',
      targetDate: null,
      isPrimary: false,
    });
  };

  return (
    <>
      <CardHeader>
        <CardTitle>What Are Your Training Goals?</CardTitle>
        <CardDescription>
          Setting clear goals helps us tailor your training plan. Add at least one goal, and mark 
          your primary goal if you have multiple.
        </CardDescription>
      </CardHeader>

      <CardContent className="space-y-6">
        {fields.map((field, index) => (
          <div
            key={field.id}
            className="p-4 border border-gray-200 rounded-lg space-y-4 relative"
          >
            {fields.length > 1 && (
              <button
                type="button"
                onClick={() => remove(index)}
                className="absolute top-2 right-2 text-gray-400 hover:text-red-600"
                aria-label="Remove goal"
              >
                <svg className="w-5 h-5" fill="currentColor" viewBox="0 0 20 20">
                  <path
                    fillRule="evenodd"
                    d="M4.293 4.293a1 1 0 011.414 0L10 8.586l4.293-4.293a1 1 0 111.414 1.414L11.414 10l4.293 4.293a1 1 0 01-1.414 1.414L10 11.414l-4.293 4.293a1 1 0 01-1.414-1.414L8.586 10 4.293 5.707a1 1 0 010-1.414z"
                    clipRule="evenodd"
                  />
                </svg>
              </button>
            )}

            <div>
              <Label htmlFor={`goals.${index}.goalType`}>Goal Type *</Label>
              <Select
                id={`goals.${index}.goalType`}
                {...register(`goals.${index}.goalType` as const, { valueAsNumber: true })}
                className="mt-1"
              >
                {Object.entries(GOAL_TYPE_LABELS).map(([value, label]) => (
                  <option key={value} value={value}>
                    {label}
                  </option>
                ))}
              </Select>
            </div>

            <div>
              <Label htmlFor={`goals.${index}.description`}>Description *</Label>
              <Input
                id={`goals.${index}.description`}
                {...register(`goals.${index}.description` as const)}
                placeholder="e.g., Complete my first HYROX race"
                className="mt-1"
              />
              {errors.goals?.[index]?.description && (
                <p className="text-sm text-red-600 mt-1">
                  {errors.goals[index]?.description?.message}
                </p>
              )}
            </div>

            <div>
              <Label htmlFor={`goals.${index}.targetDate`}>Target Date (Optional)</Label>
              <Controller
                name={`goals.${index}.targetDate` as const}
                control={control}
                render={({ field }) => (
                  <Input
                    id={`goals.${index}.targetDate`}
                    type="date"
                    value={field.value ? new Date(field.value).toISOString().split('T')[0] : ''}
                    onChange={(e) => {
                      field.onChange(e.target.value ? new Date(e.target.value) : null);
                    }}
                    min={new Date().toISOString().split('T')[0]}
                    className="mt-1"
                  />
                )}
              />
            </div>

            <div className="flex items-center">
              <input
                id={`goals.${index}.isPrimary`}
                type="checkbox"
                {...register(`goals.${index}.isPrimary` as const)}
                className="h-4 w-4 text-blue-600 focus:ring-blue-500 border-gray-300 rounded"
              />
              <Label htmlFor={`goals.${index}.isPrimary`} className="ml-2 font-normal">
                Make this my primary goal
              </Label>
            </div>
          </div>
        ))}

        <Button
          type="button"
          variant="outline"
          onClick={handleAddGoal}
          className="w-full"
        >
          + Add Another Goal
        </Button>

        {errors.goals?.root && (
          <p className="text-sm text-red-600">{errors.goals.root.message}</p>
        )}
      </CardContent>
    </>
  );
}
