/**
 * Coach Tom Chat Page
 */
'use client';

import { ChatInterface } from '@/components/coach/ChatInterface';
import { ProtectedRoute } from '@/components/auth/protected-route';

export default function CoachPage() {
  return (
    <ProtectedRoute>
      <div className="container mx-auto px-4 py-8 max-w-4xl h-[calc(100vh-8rem)]">
        <ChatInterface />
      </div>
    </ProtectedRoute>
  );
}
