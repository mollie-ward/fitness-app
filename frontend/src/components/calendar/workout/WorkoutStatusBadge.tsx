/**
 * WorkoutStatusBadge - Display workout status indicator
 */

import * as React from 'react';
import { WorkoutStatus } from '@/types/workout';
import { CheckCircle, XCircle, Calendar, PlayCircle, SkipForward } from 'lucide-react';
import { cn } from '@/lib/utils';

export interface WorkoutStatusBadgeProps {
  status: WorkoutStatus;
  size?: 'sm' | 'md' | 'lg';
  showText?: boolean;
  className?: string;
}

const statusConfig = {
  [WorkoutStatus.NOT_STARTED]: {
    icon: Calendar,
    label: 'Scheduled',
    color: 'text-gray-500',
    bgColor: 'bg-gray-100',
  },
  [WorkoutStatus.IN_PROGRESS]: {
    icon: PlayCircle,
    label: 'In Progress',
    color: 'text-yellow-600',
    bgColor: 'bg-yellow-100',
  },
  [WorkoutStatus.COMPLETED]: {
    icon: CheckCircle,
    label: 'Completed',
    color: 'text-green-600',
    bgColor: 'bg-green-100',
  },
  [WorkoutStatus.MISSED]: {
    icon: XCircle,
    label: 'Missed',
    color: 'text-red-600',
    bgColor: 'bg-red-100',
  },
  [WorkoutStatus.SKIPPED]: {
    icon: SkipForward,
    label: 'Skipped',
    color: 'text-gray-500',
    bgColor: 'bg-gray-100',
  },
};

export const WorkoutStatusBadge: React.FC<WorkoutStatusBadgeProps> = ({
  status,
  size = 'md',
  showText = false,
  className = '',
}) => {
  const config = statusConfig[status];
  const Icon = config.icon;

  const sizeClasses = {
    sm: { icon: 14, padding: 'px-2 py-1', text: 'text-xs' },
    md: { icon: 16, padding: 'px-3 py-1.5', text: 'text-sm' },
    lg: { icon: 18, padding: 'px-4 py-2', text: 'text-base' },
  };

  const sizeClass = sizeClasses[size];

  return (
    <span
      className={cn(
        'inline-flex items-center gap-1 rounded-full font-medium transition-colors',
        config.bgColor,
        config.color,
        sizeClass.padding,
        className
      )}
      role="status"
      aria-label={config.label}
    >
      <Icon size={sizeClass.icon} />
      {showText && <span className={sizeClass.text}>{config.label}</span>}
    </span>
  );
};
