/**
 * API service for user onboarding and profile management
 */
import apiClient from '@/lib/api/api-client';
import type { UserProfileDto, OnboardingFormData } from '@/types/onboarding';
import { GoalStatus, InjuryStatus } from '@/types/onboarding';

/**
 * Transform onboarding form data to UserProfileDto for API submission
 */
export function transformFormDataToDto(
  formData: OnboardingFormData,
  userName: string,
  userEmail: string
): UserProfileDto {
  // Count selected days for min/max sessions
  const selectedDays = Object.values(formData.schedule).filter(Boolean).length;

  return {
    name: userName,
    email: userEmail,
    hyroxLevel: formData.hyroxLevel,
    runningLevel: formData.runningLevel,
    strengthLevel: formData.strengthLevel,
    scheduleAvailability: {
      monday: formData.schedule.monday,
      tuesday: formData.schedule.tuesday,
      wednesday: formData.schedule.wednesday,
      thursday: formData.schedule.thursday,
      friday: formData.schedule.friday,
      saturday: formData.schedule.saturday,
      sunday: formData.schedule.sunday,
      minimumSessionsPerWeek: selectedDays,
      maximumSessionsPerWeek: selectedDays,
    },
    trainingBackground: {
      hasStructuredTrainingExperience: formData.trainingExperience !== 'none',
      previousTrainingDetails:
        formData.trainingExperience === 'none'
          ? null
          : formData.trainingExperience === 'some'
            ? 'Some structured training experience'
            : 'Extensive structured training experience',
      equipmentFamiliarity: formData.equipmentFamiliarity || null,
      trainingHistoryDetails: formData.additionalNotes || null,
    },
    trainingGoals: formData.goals.map((goal, index) => ({
      goalType: goal.goalType,
      description: goal.description,
      targetDate: goal.targetDate ? goal.targetDate.toISOString() : null,
      priority: goal.isPrimary ? 1 : index + 2,
      status: GoalStatus.Active,
    })),
    injuryLimitations: formData.hasInjuries
      ? formData.injuries.map((injury) => ({
          bodyPart: injury.bodyPart,
          injuryType: injury.injuryType,
          reportedDate: new Date().toISOString(),
          status: InjuryStatus.Active,
          movementRestrictions: injury.movementRestrictions || null,
        }))
      : [],
  };
}

/**
 * Submit user profile during onboarding
 */
export async function createUserProfile(
  profileData: UserProfileDto
): Promise<UserProfileDto> {
  const response = await apiClient.post<UserProfileDto>('/users/profile', profileData);
  return response.data;
}

/**
 * Get existing user profile
 */
export async function getUserProfile(): Promise<UserProfileDto | null> {
  try {
    const response = await apiClient.get<UserProfileDto>('/users/profile');
    return response.data;
  } catch (error: unknown) {
    const err = error as { response?: { status?: number } };
    if (err.response?.status === 404) {
      return null;
    }
    throw error;
  }
}

/**
 * Update existing user profile
 */
export async function updateUserProfile(
  profileData: UserProfileDto
): Promise<UserProfileDto> {
  const response = await apiClient.put<UserProfileDto>('/users/profile', profileData);
  return response.data;
}
