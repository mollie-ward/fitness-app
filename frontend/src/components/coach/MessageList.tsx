/**
 * MessageList Component - Displays list of messages with virtualization
 */
'use client';

import React, { useRef, useEffect } from 'react';
import { useVirtualizer } from '@tanstack/react-virtual';
import { ChatMessage } from './ChatMessage';
import { TypingIndicator } from './TypingIndicator';
import type { Message } from '@/types/coach';

interface MessageListProps {
  messages: Message[];
  isTyping?: boolean;
  autoScroll?: boolean;
}

export function MessageList({
  messages,
  isTyping = false,
  autoScroll = true,
}: MessageListProps) {
  const parentRef = useRef<HTMLDivElement>(null);
  const messagesEndRef = useRef<HTMLDivElement>(null);

  // Use virtualizer only for long message lists
  const shouldVirtualize = messages.length > 50;

  const virtualizer = useVirtualizer({
    count: messages.length,
    getScrollElement: () => parentRef.current,
    estimateSize: () => 100,
    enabled: shouldVirtualize,
  });

  // Auto-scroll to bottom on new messages
  useEffect(() => {
    if (autoScroll && messagesEndRef.current) {
      messagesEndRef.current.scrollIntoView({ behavior: 'smooth' });
    }
  }, [messages, isTyping, autoScroll]);

  return (
    <div
      ref={parentRef}
      className="flex-1 overflow-y-auto px-4 py-4"
      role="log"
      aria-live="polite"
      aria-label="Chat messages"
    >
      {/* Empty state */}
      {messages.length === 0 && !isTyping && (
        <div className="flex flex-col items-center justify-center h-full text-center">
          <div className="max-w-md">
            <h3 className="text-lg font-semibold text-gray-900 mb-2">
              Hi! I&apos;m Coach Tom 👋
            </h3>
            <p className="text-gray-600 mb-4">
              I&apos;m here to help you with your training. Ask me anything about your
              workouts, schedule, or progress!
            </p>
            <p className="text-sm text-gray-500">
              Try asking me why you&apos;re doing a specific workout, or let me know if you
              need to adjust your plan.
            </p>
          </div>
        </div>
      )}

      {/* Messages */}
      {shouldVirtualize ? (
        <div
          style={{
            height: `${virtualizer.getTotalSize()}px`,
            width: '100%',
            position: 'relative',
          }}
        >
          {virtualizer.getVirtualItems().map((virtualItem) => {
            const message = messages[virtualItem.index];
            return (
              <div
                key={virtualItem.key}
                style={{
                  position: 'absolute',
                  top: 0,
                  left: 0,
                  width: '100%',
                  transform: `translateY(${virtualItem.start}px)`,
                }}
              >
                <ChatMessage message={message} />
              </div>
            );
          })}
        </div>
      ) : (
        messages.map((message) => <ChatMessage key={message.id} message={message} />)
      )}

      {/* Typing indicator */}
      {isTyping && <TypingIndicator />}

      {/* Scroll anchor */}
      <div ref={messagesEndRef} />
    </div>
  );
}
