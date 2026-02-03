/**
 * UndoCompletionToast - Toast notification with undo option
 */

import * as React from 'react';
import { cn } from '@/lib/utils';
import { CheckCircle, X, Undo2 } from 'lucide-react';
import { Button } from '@/components/ui/button';

export interface UndoCompletionToastProps {
  show: boolean;
  onUndo: () => void;
  onDismiss: () => void;
  autoHideDuration?: number;
  message?: string;
}

export const UndoCompletionToast: React.FC<UndoCompletionToastProps> = ({
  show,
  onUndo,
  onDismiss,
  autoHideDuration = 5000,
  message = 'Workout marked complete',
}) => {
  const [isVisible, setIsVisible] = React.useState(false);

  React.useEffect(() => {
    if (show) {
      setIsVisible(true);
      const timer = setTimeout(() => {
        setIsVisible(false);
        onDismiss();
      }, autoHideDuration);

      return () => clearTimeout(timer);
    } else {
      setIsVisible(false);
    }
  }, [show, autoHideDuration, onDismiss]);

  const handleUndo = () => {
    setIsVisible(false);
    onUndo();
  };

  const handleDismiss = () => {
    setIsVisible(false);
    onDismiss();
  };

  if (!isVisible) return null;

  return (
    <div
      className={cn(
        'fixed bottom-4 right-4 z-50',
        'transform transition-all duration-300',
        'animate-slide-in-bottom'
      )}
      role="status"
      aria-live="polite"
      aria-atomic="true"
    >
      <div className="flex items-center gap-3 rounded-lg bg-gray-900 px-4 py-3 text-white shadow-lg">
        {/* Success icon */}
        <CheckCircle className="h-5 w-5 text-green-400" aria-hidden="true" />

        {/* Message */}
        <p className="text-sm font-medium">{message}</p>

        {/* Undo button */}
        <Button
          onClick={handleUndo}
          variant="ghost"
          size="sm"
          className="ml-2 h-auto px-3 py-1 text-sm text-blue-400 hover:bg-gray-800 hover:text-blue-300"
          aria-label="Undo workout completion"
        >
          <Undo2 className="mr-1 h-3 w-3" aria-hidden="true" />
          Undo
        </Button>

        {/* Close button */}
        <button
          onClick={handleDismiss}
          className="ml-2 rounded p-1 hover:bg-gray-800 focus:outline-none focus:ring-2 focus:ring-blue-500"
          aria-label="Dismiss notification"
        >
          <X className="h-4 w-4" aria-hidden="true" />
        </button>
      </div>
    </div>
  );
};
