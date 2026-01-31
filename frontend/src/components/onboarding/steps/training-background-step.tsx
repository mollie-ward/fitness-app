/**
 * Step 6: Training Background & Review
 */
import React from 'react';
import { useFormContext } from 'react-hook-form';
import { CardHeader, CardTitle, CardDescription, CardContent } from '@/components/ui/card';
import { Label } from '@/components/ui/label';
import { Select } from '@/components/ui/select';
import { Textarea } from '@/components/ui/textarea';
import { Input } from '@/components/ui/input';
import { OnboardingFormData, FITNESS_LEVEL_LABELS, GOAL_TYPE_LABELS } from '@/types/onboarding';

export function TrainingBackgroundStep() {
  const {
    register,
    watch,
    formState: { errors },
  } = useFormContext<OnboardingFormData>();

  const formData = watch();

  return (
    <>
      <CardHeader>
        <CardTitle>Training Background & Review</CardTitle>
        <CardDescription>
          Almost done! Tell us about your training history and review your profile before we create 
          your personalized plan.
        </CardDescription>
      </CardHeader>

      <CardContent className="space-y-6">
        {/* Training Background */}
        <div className="space-y-4">
          <h3 className="text-lg font-semibold text-gray-900">Training Experience</h3>

          <div>
            <Label htmlFor="trainingExperience">Structured Training Experience *</Label>
            <Select
              id="trainingExperience"
              {...register('trainingExperience')}
              className="mt-1"
            >
              <option value="none">None - I&apos;m new to structured training</option>
              <option value="some">Some - I&apos;ve followed programs before</option>
              <option value="extensive">Extensive - I&apos;ve trained consistently for years</option>
            </Select>
            {errors.trainingExperience && (
              <p className="text-sm text-red-600 mt-1">{errors.trainingExperience.message}</p>
            )}
          </div>

          <div>
            <Label htmlFor="equipmentFamiliarity">Equipment Familiarity (Optional)</Label>
            <Input
              id="equipmentFamiliarity"
              {...register('equipmentFamiliarity')}
              placeholder="e.g., Comfortable with barbells, new to rowing machine"
              className="mt-1"
            />
          </div>

          <div>
            <Label htmlFor="additionalNotes">Additional Notes (Optional)</Label>
            <Textarea
              id="additionalNotes"
              {...register('additionalNotes')}
              placeholder="Anything else we should know? Past training programs, specific preferences, etc."
              className="mt-1"
              rows={3}
            />
          </div>
        </div>

        <div className="border-t pt-6" />

        {/* Profile Summary */}
        <div className="space-y-4">
          <h3 className="text-lg font-semibold text-gray-900">Profile Summary</h3>

          <div className="bg-gray-50 border border-gray-200 rounded-lg p-4 space-y-3">
            <div>
              <h4 className="text-sm font-semibold text-gray-700">Fitness Levels</h4>
              <div className="grid grid-cols-3 gap-2 mt-2 text-sm">
                <div>
                  <span className="text-gray-600">HYROX:</span>{' '}
                  <span className="font-medium">{FITNESS_LEVEL_LABELS[formData.hyroxLevel]}</span>
                </div>
                <div>
                  <span className="text-gray-600">Running:</span>{' '}
                  <span className="font-medium">{FITNESS_LEVEL_LABELS[formData.runningLevel]}</span>
                </div>
                <div>
                  <span className="text-gray-600">Strength:</span>{' '}
                  <span className="font-medium">{FITNESS_LEVEL_LABELS[formData.strengthLevel]}</span>
                </div>
              </div>
            </div>

            <div>
              <h4 className="text-sm font-semibold text-gray-700">Goals</h4>
              <ul className="mt-2 space-y-1 text-sm">
                {formData.goals.map((goal, index) => (
                  <li key={index}>
                    • {GOAL_TYPE_LABELS[goal.goalType]}: {goal.description}
                    {goal.isPrimary && <span className="text-blue-600"> (Primary)</span>}
                  </li>
                ))}
              </ul>
            </div>

            <div>
              <h4 className="text-sm font-semibold text-gray-700">Training Schedule</h4>
              <p className="mt-1 text-sm">
                {Object.entries(formData.schedule)
                  .filter(([, selected]) => selected)
                  .map(([day]) => day.charAt(0).toUpperCase() + day.slice(1))
                  .join(', ')}
              </p>
            </div>

            {formData.hasInjuries && formData.injuries.length > 0 && (
              <div>
                <h4 className="text-sm font-semibold text-gray-700">Injuries/Limitations</h4>
                <ul className="mt-1 space-y-1 text-sm">
                  {formData.injuries.map((injury, index) => (
                    <li key={index}>• {injury.bodyPart}</li>
                  ))}
                </ul>
              </div>
            )}
          </div>

          <div className="bg-blue-50 border border-blue-200 rounded-lg p-4">
            <p className="text-blue-900 font-medium">
              🎯 Ready to create your personalized training plan!
            </p>
            <p className="text-sm text-blue-700 mt-1">
              Click &quot;Complete Setup&quot; to save your profile and generate your first workout plan.
            </p>
          </div>
        </div>
      </CardContent>
    </>
  );
}
