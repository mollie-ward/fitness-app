/**
 * DisciplineIcon - Display icon for workout discipline
 */

import * as React from 'react';
import { Discipline } from '@/types/discipline';
import { Zap, Footprints, Dumbbell, Sparkles, Coffee } from 'lucide-react';
import { getDisciplineConfig } from '../utils/discipline-colors';

export interface DisciplineIconProps {
  discipline: Discipline;
  size?: number;
  className?: string;
}

const iconMap = {
  Zap,
  Footprints,
  Dumbbell,
  Sparkles,
  Coffee,
};

export const DisciplineIcon: React.FC<DisciplineIconProps> = ({
  discipline,
  size = 20,
  className = '',
}) => {
  const config = getDisciplineConfig(discipline);
  const IconComponent = iconMap[config.icon as keyof typeof iconMap];

  if (!IconComponent) {
    return null;
  }

  return (
    <IconComponent
      size={size}
      className={className}
      aria-label={`${config.name} discipline`}
    />
  );
};
