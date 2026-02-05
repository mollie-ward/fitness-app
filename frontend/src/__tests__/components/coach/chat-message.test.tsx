/**
 * Tests for ChatMessage component
 */
import { describe, it, expect } from '@jest/globals';
import { render, screen } from '@testing-library/react';
import { ChatMessage } from '@/components/coach/ChatMessage';
import { MessageRole, type Message } from '@/types/coach';

describe('ChatMessage', () => {
  const mockUserMessage: Message = {
    id: '1',
    role: MessageRole.User,
    content: 'Hello Coach Tom!',
    timestamp: new Date('2026-02-05T10:00:00Z'),
  };

  const mockAssistantMessage: Message = {
    id: '2',
    role: MessageRole.Assistant,
    content: 'Hello! How can I help you today?',
    timestamp: new Date('2026-02-05T10:00:05Z'),
  };

  const mockAssistantMessageWithMarkdown: Message = {
    id: '3',
    role: MessageRole.Assistant,
    content: 'Bold text and a list with items',
    timestamp: new Date('2026-02-05T10:00:10Z'),
  };

  const mockMessageWithAction: Message = {
    id: '4',
    role: MessageRole.Assistant,
    content: 'I have adjusted your plan.',
    timestamp: new Date('2026-02-05T10:00:15Z'),
    triggeredAction: 'Plan modified: intensity increased',
  };

  it('should render user message with right alignment', () => {
    render(<ChatMessage message={mockUserMessage} />);
    expect(screen.getByText('Hello Coach Tom!')).toBeDefined();
    expect(screen.getByLabelText('You message')).toBeDefined();
  });

  it('should render assistant message with left alignment and avatar', () => {
    render(<ChatMessage message={mockAssistantMessage} />);
    expect(screen.getByText('Hello! How can I help you today?')).toBeDefined();
    expect(screen.getByAltText('Coach Tom')).toBeDefined();
    expect(screen.getByLabelText('Coach Tom message')).toBeDefined();
  });

  it('should render assistant message without avatar when showAvatar is false', () => {
    render(<ChatMessage message={mockAssistantMessage} showAvatar={false} />);
    expect(screen.queryByAltText('Coach Tom')).toBeNull();
  });

  it('should render content in assistant messages', () => {
    render(<ChatMessage message={mockAssistantMessageWithMarkdown} />);
    expect(screen.getByText(/Bold text/)).toBeDefined();
  });

  it('should display timestamp', () => {
    render(<ChatMessage message={mockUserMessage} />);
    // Timestamp should be displayed
    const timestamp = screen.getByText(/ago|Just now/);
    expect(timestamp).toBeDefined();
  });

  it('should display triggered action indicator', () => {
    render(<ChatMessage message={mockMessageWithAction} />);
    expect(screen.getByText(/Plan modified: intensity increased/)).toBeDefined();
  });

  it('should format timestamps correctly for different time ranges', () => {
    const now = new Date();
    const justNow: Message = {
      ...mockUserMessage,
      timestamp: new Date(now.getTime() - 30000), // 30 seconds ago
    };
    
    const { rerender } = render(<ChatMessage message={justNow} />);
    expect(screen.getByText('Just now')).toBeDefined();

    const oneHourAgo: Message = {
      ...mockUserMessage,
      timestamp: new Date(now.getTime() - 3600000), // 1 hour ago
    };
    
    rerender(<ChatMessage message={oneHourAgo} />);
    expect(screen.getByText(/1h ago/)).toBeDefined();
  });
});
