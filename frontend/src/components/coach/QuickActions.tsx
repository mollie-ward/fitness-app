/**
 * QuickActions Component - Quick action buttons for common prompts
 */
'use client';

import React from 'react';
import { Button } from '@/components/ui/button';

interface QuickActionsProps {
  onSelectAction: (prompt: string) => void;
  disabled?: boolean;
}

const QUICK_PROMPTS = [
  { label: 'Why this workout?', prompt: 'Why am I doing this workout today?' },
  { label: 'Make it harder', prompt: 'Can you make my workouts harder?' },
  { label: 'Make it easier', prompt: 'Can you make my workouts easier?' },
  { label: "I'm injured", prompt: "I've got an injury I need to tell you about" },
  { label: 'Change my schedule', prompt: 'I need to adjust my training schedule' },
  { label: 'How am I doing?', prompt: 'How is my progress looking?' },
];

export function QuickActions({ onSelectAction, disabled = false }: QuickActionsProps) {
  return (
    <div className="mb-4">
      <p className="text-sm text-gray-600 mb-2 px-1">Quick actions:</p>
      <div className="flex flex-wrap gap-2">
        {QUICK_PROMPTS.map((action) => (
          <Button
            key={action.label}
            variant="outline"
            size="sm"
            onClick={() => onSelectAction(action.prompt)}
            disabled={disabled}
            className="text-xs"
          >
            {action.label}
          </Button>
        ))}
      </div>
    </div>
  );
}
