import * as React from 'react';
import { cn } from '@/lib/utils';
import { X } from 'lucide-react';
import { Button } from './button';

export interface ModalProps {
  open: boolean;
  onClose: () => void;
  children: React.ReactNode;
  title?: string;
  description?: string;
  size?: 'sm' | 'md' | 'lg' | 'xl';
}

const Modal = React.forwardRef<HTMLDivElement, ModalProps>(
  ({ open, onClose, children, title, description, size = 'md' }, ref) => {
    const sizeClasses = {
      sm: 'max-w-sm',
      md: 'max-w-md',
      lg: 'max-w-lg',
      xl: 'max-w-xl',
    };

    React.useEffect(() => {
      const handleEscape = (e: KeyboardEvent) => {
        if (e.key === 'Escape') {
          onClose();
        }
      };

      if (open) {
        document.addEventListener('keydown', handleEscape);
        document.body.style.overflow = 'hidden';
      }

      return () => {
        document.removeEventListener('keydown', handleEscape);
        document.body.style.overflow = 'unset';
      };
    }, [open, onClose]);

    if (!open) return null;

    return (
      <div
        className="fixed inset-0 z-50 flex items-center justify-center bg-black/50"
        onClick={onClose}
        role="dialog"
        aria-modal="true"
        aria-labelledby={title ? 'modal-title' : undefined}
        aria-describedby={description ? 'modal-description' : undefined}
      >
        <div
          ref={ref}
          className={cn(
            'relative w-full bg-white rounded-lg shadow-lg p-6 m-4',
            sizeClasses[size]
          )}
          onClick={(e) => e.stopPropagation()}
        >
          <Button
            variant="ghost"
            size="icon"
            className="absolute top-4 right-4"
            onClick={onClose}
            aria-label="Close modal"
          >
            <X className="h-4 w-4" />
          </Button>

          {title && (
            <h2 id="modal-title" className="text-xl font-semibold mb-2">
              {title}
            </h2>
          )}
          {description && (
            <p id="modal-description" className="text-sm text-gray-500 mb-4">
              {description}
            </p>
          )}

          <div className="mt-4">{children}</div>
        </div>
      </div>
    );
  }
);

Modal.displayName = 'Modal';

export { Modal };
