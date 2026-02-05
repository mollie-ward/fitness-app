/**
 * TypingIndicator Component - Shows when Coach Tom is typing
 */
'use client';

import React from 'react';
import Image from 'next/image';

export function TypingIndicator() {
  return (
    <div className="flex gap-3 mb-4" role="status" aria-label="Coach Tom is typing">
      <div className="flex-shrink-0">
        <Image
          src="/coach-tom.png"
          alt="Coach Tom"
          width={40}
          height={40}
          className="rounded-full"
        />
      </div>
      <div className="flex items-center bg-gray-100 rounded-lg px-4 py-3">
        <div className="flex gap-1">
          <span
            className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"
            style={{ animationDelay: '0ms' }}
          />
          <span
            className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"
            style={{ animationDelay: '150ms' }}
          />
          <span
            className="w-2 h-2 bg-gray-400 rounded-full animate-bounce"
            style={{ animationDelay: '300ms' }}
          />
        </div>
      </div>
    </div>
  );
}
