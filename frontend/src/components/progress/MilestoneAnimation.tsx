/**
 * MilestoneAnimation - Celebration animation when streak milestone is reached
 */

import * as React from 'react';
import { cn } from '@/lib/utils';
import { Trophy } from 'lucide-react';

export interface MilestoneAnimationProps {
  show: boolean;
  milestone: number;
  onComplete?: () => void;
  reducedMotion?: boolean;
}

export const MilestoneAnimation: React.FC<MilestoneAnimationProps> = ({
  show,
  milestone,
  onComplete,
  reducedMotion = false,
}) => {
  const [isVisible, setIsVisible] = React.useState(false);

  React.useEffect(() => {
    if (show) {
      setIsVisible(true);
      const timer = setTimeout(() => {
        setIsVisible(false);
        onComplete?.();
      }, reducedMotion ? 500 : 3000);

      return () => clearTimeout(timer);
    }
  }, [show, onComplete, reducedMotion]);

  if (!isVisible) return null;

  const getMilestoneMessage = () => {
    if (milestone >= 365) return '🎉 One Year Streak!';
    if (milestone >= 180) return '🌟 6-Month Streak!';
    if (milestone >= 90) return '💫 90-Day Streak!';
    if (milestone >= 60) return '🔥 2-Month Streak!';
    if (milestone >= 30) return '⭐ 30-Day Streak!';
    if (milestone >= 14) return '✨ 2-Week Streak!';
    if (milestone >= 7) return '🎯 1-Week Streak!';
    return '🎊 Milestone Reached!';
  };

  return (
    <div
      className={cn(
        'fixed inset-0 z-50 flex items-center justify-center bg-black/30',
        'backdrop-blur-sm'
      )}
      role="status"
      aria-live="polite"
      aria-label={`Milestone reached: ${milestone} day streak`}
    >
      <div
        className={cn(
          'relative flex flex-col items-center justify-center p-8',
          'rounded-2xl bg-gradient-to-br from-yellow-50 to-orange-50 shadow-2xl border-4 border-yellow-400',
          !reducedMotion && 'animate-bounce-in'
        )}
      >
        {/* Trophy icon */}
        <div
          className={cn(
            'mb-4 rounded-full bg-yellow-100 p-8',
            !reducedMotion && 'animate-scale-in'
          )}
        >
          <Trophy
            className="h-20 w-20 text-yellow-600"
            aria-hidden="true"
          />
        </div>

        {/* Milestone text */}
        <h2 className="text-3xl font-bold text-gray-900 mb-2">
          {getMilestoneMessage()}
        </h2>
        <p className="text-lg text-gray-700 mb-4">
          {milestone} days of consistent training!
        </p>
        <p className="text-sm text-gray-600">
          You're building something special 💪
        </p>

        {/* Celebration particles - only if not reduced motion */}
        {!reducedMotion && (
          <div className="pointer-events-none absolute inset-0" aria-hidden="true">
            {[...Array(20)].map((_, i) => (
              <div
                key={i}
                className={cn(
                  'absolute h-3 w-3 rounded-full',
                  i % 5 === 0 && 'bg-yellow-400',
                  i % 5 === 1 && 'bg-orange-400',
                  i % 5 === 2 && 'bg-red-400',
                  i % 5 === 3 && 'bg-pink-400',
                  i % 5 === 4 && 'bg-purple-400',
                  'animate-confetti'
                )}
                style={{
                  left: '50%',
                  top: '50%',
                  animationDelay: `${i * 0.08}s`,
                  '--tx': `${Math.cos((i / 20) * 2 * Math.PI) * 150}px`,
                  '--ty': `${Math.sin((i / 20) * 2 * Math.PI) * 150}px`,
                } as React.CSSProperties}
              />
            ))}
          </div>
        )}

        {/* Sparkle effects */}
        {!reducedMotion && (
          <>
            <div className="absolute top-4 right-4 text-2xl animate-pulse">✨</div>
            <div className="absolute top-8 left-4 text-2xl animate-pulse" style={{ animationDelay: '0.3s' }}>🌟</div>
            <div className="absolute bottom-8 right-8 text-2xl animate-pulse" style={{ animationDelay: '0.6s' }}>⭐</div>
          </>
        )}
      </div>
    </div>
  );
};
