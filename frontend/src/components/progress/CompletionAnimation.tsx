/**
 * CompletionAnimation - Celebration animation when workout is marked complete
 */

import * as React from 'react';
import { cn } from '@/lib/utils';
import { CheckCircle } from 'lucide-react';

export interface CompletionAnimationProps {
  show: boolean;
  onComplete?: () => void;
  reducedMotion?: boolean;
}

export const CompletionAnimation: React.FC<CompletionAnimationProps> = ({
  show,
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
      }, reducedMotion ? 500 : 2000);

      return () => clearTimeout(timer);
    }
  }, [show, onComplete, reducedMotion]);

  if (!isVisible) return null;

  return (
    <div
      className={cn(
        'fixed inset-0 z-50 flex items-center justify-center bg-black/20',
        'backdrop-blur-sm'
      )}
      role="status"
      aria-live="polite"
      aria-label="Workout marked complete"
    >
      <div
        className={cn(
          'relative flex flex-col items-center justify-center p-8',
          'rounded-2xl bg-white shadow-2xl',
          !reducedMotion && 'animate-bounce-in'
        )}
      >
        {/* Main checkmark */}
        <div
          className={cn(
            'mb-4 rounded-full bg-green-100 p-6',
            !reducedMotion && 'animate-scale-in'
          )}
        >
          <CheckCircle
            className="h-16 w-16 text-green-600"
            aria-hidden="true"
          />
        </div>

        {/* Success message */}
        <h2 className="text-2xl font-bold text-gray-900">Great Work!</h2>
        <p className="mt-2 text-gray-600">Workout completed</p>

        {/* Confetti particles - only if not reduced motion */}
        {!reducedMotion && (
          <div className="pointer-events-none absolute inset-0" aria-hidden="true">
            {[...Array(12)].map((_, i) => (
              <div
                key={i}
                className={cn(
                  'absolute h-2 w-2 rounded-full',
                  i % 4 === 0 && 'bg-blue-500',
                  i % 4 === 1 && 'bg-green-500',
                  i % 4 === 2 && 'bg-yellow-500',
                  i % 4 === 3 && 'bg-red-500',
                  'animate-confetti'
                )}
                style={{
                  left: '50%',
                  top: '50%',
                  animationDelay: `${i * 0.1}s`,
                  '--tx': `${Math.cos((i / 12) * 2 * Math.PI) * 100}px`,
                  '--ty': `${Math.sin((i / 12) * 2 * Math.PI) * 100}px`,
                } as React.CSSProperties}
              />
            ))}
          </div>
        )}
      </div>
    </div>
  );
};
