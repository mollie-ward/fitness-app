/**
 * MotivationalMessage - Coach Tom avatar with context-aware messages
 */

import * as React from 'react';
import { Card } from '@/components/ui/card';
import { cn } from '@/lib/utils';
import Image from 'next/image';

export interface MotivationalMessageProps {
  type: 'completion' | 'streak' | 'week-complete' | 'low-completion' | 'milestone';
  data?: {
    streakDays?: number;
    weeklyComplete?: number;
    weeklyTotal?: number;
    milestone?: number;
  };
  className?: string;
}

export const MotivationalMessage: React.FC<MotivationalMessageProps> = ({
  type,
  data = {},
  className = '',
}) => {
  const getMessage = (): string => {
    switch (type) {
      case 'completion':
        return 'Great work! Another workout in the books. Keep building that momentum!';
      
      case 'streak':
        return data.streakDays
          ? `${data.streakDays} days in a row - amazing! Your consistency is truly impressive.`
          : 'Great start! Keep this streak going strong.';
      
      case 'week-complete':
        return data.weeklyComplete && data.weeklyTotal
          ? `All ${data.weeklyComplete} of ${data.weeklyTotal} workouts done this week! Outstanding dedication.`
          : 'Week complete! Excellent work staying committed to your training.';
      
      case 'low-completion':
        return "I know life gets busy. Let's get back on track together - even one workout makes a difference!";
      
      case 'milestone':
        return data.milestone
          ? `${data.milestone} days! You've hit a major milestone. This is what champions are made of! 🏆`
          : 'Milestone achieved! Keep up the incredible work!';
      
      default:
        return 'Keep pushing forward. Every workout brings you closer to your goals!';
    }
  };

  const message = getMessage();

  return (
    <Card className={cn('p-4', className)}>
      <div className="flex items-start gap-4">
        {/* Coach Tom avatar */}
        <div className="flex-shrink-0">
          <div className="relative w-12 h-12 rounded-full overflow-hidden bg-blue-100 border-2 border-blue-600">
            <Image
              src="/coachTom.png"
              alt="Coach Tom"
              fill
              className="object-cover"
              sizes="48px"
            />
          </div>
        </div>

        {/* Message bubble */}
        <div className="flex-1">
          <div className="flex items-center gap-2 mb-1">
            <span className="text-sm font-semibold text-blue-700">Coach Tom</span>
          </div>
          <p className="text-sm text-gray-700 leading-relaxed">{message}</p>
        </div>
      </div>
    </Card>
  );
};
