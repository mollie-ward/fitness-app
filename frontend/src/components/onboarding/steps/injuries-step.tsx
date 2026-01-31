/**
 * Step 5: Injury & Limitations (Optional)
 */
import React from 'react';
import { useFormContext, useFieldArray } from 'react-hook-form';
import { CardHeader, CardTitle, CardDescription, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Select } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { Button } from '@/components/ui/button';
import { InjuryType, INJURY_TYPE_LABELS, BODY_PARTS, OnboardingFormData } from '@/types/onboarding';

export function InjuriesStep() {
  const {
    control,
    register,
    watch,
    setValue,
    formState: { errors },
  } = useFormContext<OnboardingFormData>();

  const hasInjuries = watch('hasInjuries');

  const { fields, append, remove } = useFieldArray({
    control,
    name: 'injuries',
  });

  const handleAddInjury = () => {
    append({
      bodyPart: '',
      injuryType: InjuryType.Chronic,
      movementRestrictions: '',
    });
  };

  const handleToggleInjuries = (value: boolean) => {
    setValue('hasInjuries', value);
    if (!value) {
      // Clear all injuries when toggling off
      setValue('injuries', []);
    } else if (fields.length === 0) {
      // Add one injury field when toggling on
      handleAddInjury();
    }
  };

  return (
    <>
      <CardHeader>
        <CardTitle>Any Injuries or Limitations?</CardTitle>
        <CardDescription>
          Help us create a safe training plan by letting us know about any current injuries, chronic 
          conditions, or physical limitations. You can skip this step if you have none.
        </CardDescription>
      </CardHeader>

      <CardContent className="space-y-6">
        <div className="flex items-center justify-center gap-4">
          <button
            type="button"
            onClick={() => handleToggleInjuries(false)}
            className={`
              px-6 py-3 rounded-lg border-2 font-medium transition-all
              ${
                !hasInjuries
                  ? 'border-blue-600 bg-blue-50 text-blue-900'
                  : 'border-gray-200 bg-white text-gray-700 hover:border-gray-300'
              }
            `}
            aria-pressed={!hasInjuries}
          >
            No Injuries
          </button>
          <button
            type="button"
            onClick={() => handleToggleInjuries(true)}
            className={`
              px-6 py-3 rounded-lg border-2 font-medium transition-all
              ${
                hasInjuries
                  ? 'border-blue-600 bg-blue-50 text-blue-900'
                  : 'border-gray-200 bg-white text-gray-700 hover:border-gray-300'
              }
            `}
            aria-pressed={hasInjuries}
          >
            I Have Injuries/Limitations
          </button>
        </div>

        {hasInjuries && (
          <div className="space-y-4">
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
                    aria-label="Remove injury"
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

                <div className="grid grid-cols-1 md:grid-cols-2 gap-4">
                  <div>
                    <Label htmlFor={`injuries.${index}.bodyPart`}>Body Part *</Label>
                    <Select
                      id={`injuries.${index}.bodyPart`}
                      {...register(`injuries.${index}.bodyPart` as const)}
                      className="mt-1"
                    >
                      <option value="">Select body part</option>
                      {BODY_PARTS.map((part) => (
                        <option key={part} value={part}>
                          {part}
                        </option>
                      ))}
                    </Select>
                    {errors.injuries?.[index]?.bodyPart && (
                      <p className="text-sm text-red-600 mt-1">
                        {errors.injuries[index]?.bodyPart?.message}
                      </p>
                    )}
                  </div>

                  <div>
                    <Label htmlFor={`injuries.${index}.injuryType`}>Injury Type *</Label>
                    <Select
                      id={`injuries.${index}.injuryType`}
                      {...register(`injuries.${index}.injuryType` as const, { valueAsNumber: true })}
                      className="mt-1"
                    >
                      {Object.entries(INJURY_TYPE_LABELS).map(([value, label]) => (
                        <option key={value} value={value}>
                          {label}
                        </option>
                      ))}
                    </Select>
                  </div>
                </div>

                <div>
                  <Label htmlFor={`injuries.${index}.movementRestrictions`}>
                    Movement Restrictions (Optional)
                  </Label>
                  <Textarea
                    id={`injuries.${index}.movementRestrictions`}
                    {...register(`injuries.${index}.movementRestrictions` as const)}
                    placeholder="e.g., Avoid overhead pressing, limit running to 20 minutes"
                    className="mt-1"
                    rows={2}
                  />
                </div>
              </div>
            ))}

            <Button type="button" variant="outline" onClick={handleAddInjury} className="w-full">
              + Add Another Injury/Limitation
            </Button>
          </div>
        )}

        {!hasInjuries && (
          <div className="bg-green-50 border border-green-200 rounded-lg p-4 text-center">
            <p className="text-green-800 font-medium">
              ✓ Great! We&apos;ll design your plan without restrictions.
            </p>
            <p className="text-sm text-green-700 mt-1">
              You can always add injuries later from your profile settings.
            </p>
          </div>
        )}
      </CardContent>
    </>
  );
}
