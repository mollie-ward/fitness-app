/**
 * Discipline color mapping and utilities
 */

import { Discipline, DisciplineColors, DisciplineConfig } from '@/types/discipline';

export const disciplineColorMap: Record<Discipline, DisciplineColors> = {
  [Discipline.HYROX]: {
    primary: 'rgb(249, 115, 22)', // orange-500
    secondary: 'rgb(220, 38, 38)', // red-600
    bg: 'rgb(254, 243, 199)', // amber-100
    text: 'rgb(124, 45, 18)', // orange-900
    border: 'rgb(251, 191, 36)', // amber-400
  },
  [Discipline.RUNNING]: {
    primary: 'rgb(59, 130, 246)', // blue-500
    secondary: 'rgb(37, 99, 235)', // blue-600
    bg: 'rgb(219, 234, 254)', // blue-100
    text: 'rgb(30, 58, 138)', // blue-900
    border: 'rgb(96, 165, 250)', // blue-400
  },
  [Discipline.STRENGTH]: {
    primary: 'rgb(168, 85, 247)', // purple-500
    secondary: 'rgb(34, 197, 94)', // green-500
    bg: 'rgb(243, 232, 255)', // purple-100
    text: 'rgb(88, 28, 135)', // purple-900
    border: 'rgb(192, 132, 252)', // purple-400
  },
  [Discipline.HYBRID]: {
    primary: 'rgb(168, 85, 247)', // purple-500
    secondary: 'rgb(249, 115, 22)', // orange-500
    bg: 'rgb(243, 232, 255)', // purple-100
    text: 'rgb(88, 28, 135)', // purple-900
    border: 'rgb(192, 132, 252)', // purple-400
  },
  [Discipline.REST]: {
    primary: 'rgb(107, 114, 128)', // gray-500
    secondary: 'rgb(75, 85, 99)', // gray-600
    bg: 'rgb(243, 244, 246)', // gray-100
    text: 'rgb(31, 41, 55)', // gray-800
    border: 'rgb(156, 163, 175)', // gray-400
  },
};

export const disciplineConfig: Record<Discipline, DisciplineConfig> = {
  [Discipline.HYROX]: {
    name: 'HYROX',
    colors: disciplineColorMap[Discipline.HYROX],
    icon: 'Zap',
    description: 'HYROX race simulation and training',
  },
  [Discipline.RUNNING]: {
    name: 'Running',
    colors: disciplineColorMap[Discipline.RUNNING],
    icon: 'Footprints',
    description: 'Running and endurance training',
  },
  [Discipline.STRENGTH]: {
    name: 'Strength',
    colors: disciplineColorMap[Discipline.STRENGTH],
    icon: 'Dumbbell',
    description: 'Strength and resistance training',
  },
  [Discipline.HYBRID]: {
    name: 'Hybrid',
    colors: disciplineColorMap[Discipline.HYBRID],
    icon: 'Sparkles',
    description: 'Mixed discipline training',
  },
  [Discipline.REST]: {
    name: 'Rest',
    colors: disciplineColorMap[Discipline.REST],
    icon: 'Coffee',
    description: 'Rest and recovery day',
  },
};

/**
 * Get color configuration for a discipline
 */
export function getDisciplineColors(discipline: Discipline): DisciplineColors {
  return disciplineColorMap[discipline];
}

/**
 * Get full configuration for a discipline
 */
export function getDisciplineConfig(discipline: Discipline): DisciplineConfig {
  return disciplineConfig[discipline];
}

/**
 * Get Tailwind CSS classes for a discipline
 */
export function getDisciplineClasses(discipline: Discipline) {
  const colorMap: Record<Discipline, { bg: string; text: string; border: string }> = {
    [Discipline.HYROX]: {
      bg: 'bg-amber-100',
      text: 'text-orange-900',
      border: 'border-amber-400',
    },
    [Discipline.RUNNING]: {
      bg: 'bg-blue-100',
      text: 'text-blue-900',
      border: 'border-blue-400',
    },
    [Discipline.STRENGTH]: {
      bg: 'bg-purple-100',
      text: 'text-purple-900',
      border: 'border-purple-400',
    },
    [Discipline.HYBRID]: {
      bg: 'bg-gradient-to-r from-purple-100 to-orange-100',
      text: 'text-purple-900',
      border: 'border-purple-400',
    },
    [Discipline.REST]: {
      bg: 'bg-gray-100',
      text: 'text-gray-800',
      border: 'border-gray-400',
    },
  };

  return colorMap[discipline];
}
