/**
 * ChatInterface Component - Main chat interface for Coach Tom
 */
'use client';

import React, { useState, useEffect, useCallback } from 'react';
import Image from 'next/image';
import { MessageList } from './MessageList';
import { MessageInput } from './MessageInput';
import { QuickActions } from './QuickActions';
import { Button } from '@/components/ui/button';
import { AlertCircle, RefreshCw, Trash2 } from 'lucide-react';
import { MessageRole, type Message } from '@/types/coach';
import { sendChatMessage, getConversationHistory, clearConversation } from '@/services/coach-api';

interface ChatInterfaceProps {
  initialConversationId?: string;
}

export function ChatInterface({ initialConversationId }: ChatInterfaceProps) {
  const [messages, setMessages] = useState<Message[]>([]);
  const [conversationId, setConversationId] = useState<string | undefined>(
    initialConversationId
  );
  const [isLoading, setIsLoading] = useState(false);
  const [isTyping, setIsTyping] = useState(false);
  const [error, setError] = useState<string | null>(null);
  const [isLoadingHistory, setIsLoadingHistory] = useState(false);

  // Load conversation history on mount
  useEffect(() => {
    if (conversationId) {
      loadConversationHistory(conversationId);
    }
  }, [conversationId]);

  const loadConversationHistory = async (convId: string) => {
    setIsLoadingHistory(true);
    setError(null);
    try {
      const history = await getConversationHistory(convId);
      const formattedMessages: Message[] = history.messages.map((msg) => ({
        id: msg.id,
        role: msg.role,
        content: msg.content,
        timestamp: new Date(msg.timestamp),
        triggeredAction: msg.triggeredAction,
      }));
      setMessages(formattedMessages);
    } catch (err) {
      console.error('Error loading conversation history:', err);
      setError('Failed to load conversation history');
    } finally {
      setIsLoadingHistory(false);
    }
  };

  const handleSendMessage = useCallback(
    async (messageText: string) => {
      if (!messageText.trim() || isLoading) return;

      setError(null);
      setIsLoading(true);

      // Add user message immediately
      const userMessage: Message = {
        id: `temp-${Date.now()}`,
        role: MessageRole.User,
        content: messageText,
        timestamp: new Date(),
      };
      setMessages((prev) => [...prev, userMessage]);

      // Show typing indicator
      setIsTyping(true);

      try {
        const response = await sendChatMessage(messageText, conversationId);

        // Update conversation ID if it's the first message
        if (!conversationId) {
          setConversationId(response.conversationId);
        }

        // Add assistant response
        const assistantMessage: Message = {
          id: `msg-${Date.now()}`,
          role: MessageRole.Assistant,
          content: response.response,
          timestamp: new Date(response.timestamp),
          triggeredAction: response.triggeredAction,
        };

        setMessages((prev) => [...prev, assistantMessage]);
      } catch (err: unknown) {
        console.error('Error sending message:', err);
        setError(
          (err as {response?: {data?: {message?: string}}})?.response?.data?.message ||
            'Failed to send message. Please try again.'
        );
        // Remove the temporary user message on error
        setMessages((prev) => prev.filter((msg) => msg.id !== userMessage.id));
      } finally {
        setIsLoading(false);
        setIsTyping(false);
      }
    },
    [conversationId, isLoading]
  );

  const handleRetry = useCallback(() => {
    setError(null);
  }, []);

  const handleClearConversation = useCallback(async () => {
    if (!conversationId) return;

    const confirmed = window.confirm(
      'Are you sure you want to clear this conversation? This cannot be undone.'
    );
    if (!confirmed) return;

    try {
      await clearConversation(conversationId);
      setMessages([]);
      setConversationId(undefined);
      setError(null);
    } catch (err) {
      console.error('Error clearing conversation:', err);
      setError('Failed to clear conversation');
    }
  }, [conversationId]);

  return (
    <div className="flex flex-col h-full bg-white rounded-lg shadow-lg">
      {/* Header */}
      <div className="flex items-center justify-between px-6 py-4 border-b border-gray-200">
        <div className="flex items-center gap-3">
          <Image
            src="/coach-tom.png"
            alt="Coach Tom"
            width={48}
            height={48}
            className="rounded-full"
          />
          <div>
            <h2 className="text-lg font-semibold text-gray-900">Coach Tom</h2>
            <p className="text-sm text-gray-600">Your HYROX and endurance coach</p>
          </div>
        </div>
        {conversationId && (
          <Button
            variant="ghost"
            size="sm"
            onClick={handleClearConversation}
            aria-label="Clear conversation"
          >
            <Trash2 className="h-4 w-4" />
          </Button>
        )}
      </div>

      {/* Error Banner */}
      {error && (
        <div className="mx-4 mt-4 p-3 bg-red-50 border border-red-200 rounded-lg flex items-start gap-2">
          <AlertCircle className="h-5 w-5 text-red-600 flex-shrink-0 mt-0.5" />
          <div className="flex-1">
            <p className="text-sm text-red-800">{error}</p>
          </div>
          <Button
            variant="ghost"
            size="sm"
            onClick={handleRetry}
            aria-label="Retry"
          >
            <RefreshCw className="h-4 w-4" />
          </Button>
        </div>
      )}

      {/* Loading state */}
      {isLoadingHistory && (
        <div className="flex-1 flex items-center justify-center">
          <div className="text-center">
            <div className="inline-block animate-spin rounded-full h-8 w-8 border-b-2 border-blue-600 mb-2"></div>
            <p className="text-gray-600">Loading conversation...</p>
          </div>
        </div>
      )}

      {/* Messages */}
      {!isLoadingHistory && (
        <>
          <MessageList messages={messages} isTyping={isTyping} />

          {/* Quick Actions */}
          {messages.length === 0 && !isTyping && (
            <div className="px-4 pb-2">
              <QuickActions
                onSelectAction={handleSendMessage}
                disabled={isLoading}
              />
            </div>
          )}

          {/* Input */}
          <div className="px-4 py-4 border-t border-gray-200">
            <MessageInput
              onSendMessage={handleSendMessage}
              disabled={isLoading}
              placeholder="Ask me anything about your training..."
            />
          </div>
        </>
      )}
    </div>
  );
}
