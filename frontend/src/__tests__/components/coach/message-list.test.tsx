/**
 * Tests for MessageList component
 */
import { describe, it, expect } from '@jest/globals';
import { render, screen } from '@testing-library/react';
import { MessageList } from '@/components/coach/MessageList';
import { MessageRole, type Message } from '@/types/coach';

describe('MessageList', () => {
  const mockMessages: Message[] = [
    {
      id: '1',
      role: MessageRole.User,
      content: 'Hello Coach Tom!',
      timestamp: new Date('2026-02-05T10:00:00Z'),
    },
    {
      id: '2',
      role: MessageRole.Assistant,
      content: 'Hello! How can I help you today?',
      timestamp: new Date('2026-02-05T10:00:05Z'),
    },
    {
      id: '3',
      role: MessageRole.User,
      content: 'Tell me about my workout',
      timestamp: new Date('2026-02-05T10:00:10Z'),
    },
  ];

  it('should render empty state when no messages', () => {
    render(<MessageList messages={[]} />);
    expect(screen.getByText(/Hi! I'm Coach Tom/i)).toBeDefined();
    expect(screen.getByText(/I'm here to help you with your training/i)).toBeDefined();
  });

  it('should render all messages', () => {
    render(<MessageList messages={mockMessages} />);
    expect(screen.getByText('Hello Coach Tom!')).toBeDefined();
    expect(screen.getByText('Hello! How can I help you today?')).toBeDefined();
    expect(screen.getByText('Tell me about my workout')).toBeDefined();
  });

  it('should render typing indicator when isTyping is true', () => {
    render(<MessageList messages={mockMessages} isTyping={true} />);
    expect(screen.getByLabelText('Coach Tom is typing')).toBeDefined();
  });

  it('should not render typing indicator when isTyping is false', () => {
    render(<MessageList messages={mockMessages} isTyping={false} />);
    expect(screen.queryByLabelText('Coach Tom is typing')).toBeNull();
  });

  it('should have proper ARIA attributes for accessibility', () => {
    render(<MessageList messages={mockMessages} />);
    const messageContainer = screen.getByRole('log');
    expect(messageContainer).toBeDefined();
    expect(messageContainer.getAttribute('aria-live')).toBe('polite');
    expect(messageContainer.getAttribute('aria-label')).toBe('Chat messages');
  });

  it('should render messages even when typing indicator is shown', () => {
    render(<MessageList messages={mockMessages} isTyping={true} />);
    expect(screen.getByText('Hello Coach Tom!')).toBeDefined();
    expect(screen.getByLabelText('Coach Tom is typing')).toBeDefined();
  });

  it('should not show empty state when typing indicator is shown with no messages', () => {
    render(<MessageList messages={[]} isTyping={true} />);
    expect(screen.queryByText(/Hi! I'm Coach Tom/i)).toBeNull();
    expect(screen.getByLabelText('Coach Tom is typing')).toBeDefined();
  });
});
