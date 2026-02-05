/**
 * ChatMessage Component - Displays individual chat messages
 */
'use client';

import React from 'react';
import ReactMarkdown from 'react-markdown';
import remarkGfm from 'remark-gfm';
import { cn } from '@/lib/utils';
import { MessageRole, type Message } from '@/types/coach';
import Image from 'next/image';

interface ChatMessageProps {
  message: Message;
  showAvatar?: boolean;
}

export const ChatMessage = React.memo(function ChatMessage({
  message,
  showAvatar = true,
}: ChatMessageProps) {
  const isUser = message.role === MessageRole.User;
  const isAssistant = message.role === MessageRole.Assistant;

  // Format timestamp
  const formatTimestamp = (date: Date) => {
    const now = new Date();
    const diffMs = now.getTime() - date.getTime();
    const diffMins = Math.floor(diffMs / 60000);
    const diffHours = Math.floor(diffMs / 3600000);
    const diffDays = Math.floor(diffMs / 86400000);

    if (diffMins < 1) return 'Just now';
    if (diffMins < 60) return `${diffMins}m ago`;
    if (diffHours < 24) return `${diffHours}h ago`;
    if (diffDays < 7) return `${diffDays}d ago`;
    return date.toLocaleDateString();
  };

  return (
    <div
      className={cn(
        'flex gap-3 mb-4',
        isUser ? 'justify-end' : 'justify-start'
      )}
      role="article"
      aria-label={`${isUser ? 'You' : 'Coach Tom'} message`}
    >
      {/* Avatar for assistant messages (left side) */}
      {isAssistant && showAvatar && (
        <div className="flex-shrink-0">
          <Image
            src="/coach-tom.png"
            alt="Coach Tom"
            width={40}
            height={40}
            className="rounded-full"
          />
        </div>
      )}

      {/* Message bubble */}
      <div
        className={cn(
          'flex flex-col max-w-[80%] sm:max-w-[70%]',
          isUser ? 'items-end' : 'items-start'
        )}
      >
        {/* Message content */}
        <div
          className={cn(
            'rounded-lg px-4 py-2 break-words',
            isUser
              ? 'bg-blue-600 text-white'
              : 'bg-gray-100 text-gray-900',
            isAssistant && 'prose prose-sm max-w-none'
          )}
        >
          {isAssistant ? (
            <ReactMarkdown
              remarkPlugins={[remarkGfm]}
              components={{
                p: ({ children }) => <p className="mb-2 last:mb-0">{children}</p>,
                ul: ({ children }) => <ul className="mb-2 ml-4 list-disc">{children}</ul>,
                ol: ({ children }) => <ol className="mb-2 ml-4 list-decimal">{children}</ol>,
                li: ({ children }) => <li className="mb-1">{children}</li>,
                strong: ({ children }) => <strong className="font-semibold">{children}</strong>,
                a: ({ children, href }) => (
                  <a
                    href={href}
                    target="_blank"
                    rel="noopener noreferrer"
                    className="text-blue-600 hover:underline"
                  >
                    {children}
                  </a>
                ),
              }}
            >
              {message.content}
            </ReactMarkdown>
          ) : (
            <p className="whitespace-pre-wrap">{message.content}</p>
          )}
        </div>

        {/* Timestamp */}
        <span className="text-xs text-gray-500 mt-1 px-1">
          {formatTimestamp(message.timestamp)}
        </span>

        {/* Action indicator */}
        {message.triggeredAction && (
          <div className="mt-2 px-3 py-1 bg-green-50 text-green-700 text-xs rounded-md border border-green-200">
            ✓ {message.triggeredAction}
          </div>
        )}
      </div>

      {/* Spacer for user messages */}
      {isUser && showAvatar && <div className="w-10 flex-shrink-0" />}
    </div>
  );
});
