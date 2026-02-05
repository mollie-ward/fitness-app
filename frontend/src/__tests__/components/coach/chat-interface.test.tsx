/**
 * Tests for ChatInterface component
 */
import { describe, it, expect, jest } from '@jest/globals';
import { render, screen, fireEvent, waitFor } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { ChatInterface } from '@/components/coach/ChatInterface';

// Simple mocks for the API
const mockSendMessage = jest.fn();
const mockGetHistory = jest.fn();
const mockClearConversation = jest.fn();

// Mock the API functions before import
jest.mock('@/services/coach-api', () => ({
  __esModule: true,
  sendChatMessage: (...args: any[]) => mockSendMessage(...args),
  getConversationHistory: (...args: any[]) => mockGetHistory(...args),
  clearConversation: (...args: any[]) => mockClearConversation(...args),
}));

describe('ChatInterface', () => {
  beforeEach(() => {
    mockSendMessage.mockClear();
    mockGetHistory.mockClear();
    mockClearConversation.mockClear();
  });

  it('should render chat interface with header', () => {
    render(<ChatInterface />);
    expect(screen.getByText('Coach Tom')).toBeDefined();
    expect(screen.getByText(/Your HYROX and endurance coach/i)).toBeDefined();
  });

  it('should show empty state when no messages', () => {
    render(<ChatInterface />);
    expect(screen.getByText(/Hi! I'm Coach Tom/i)).toBeDefined();
  });

  it('should display quick actions when no messages', () => {
    render(<ChatInterface />);
    expect(screen.getByText('Why this workout?')).toBeDefined();
    expect(screen.getByText('Make it harder')).toBeDefined();
  });

  it('should have message input field', () => {
    render(<ChatInterface />);
    expect(screen.getByPlaceholderText(/Ask me anything about your training/i)).toBeDefined();
  });

  it('should send a message when user types and clicks send', async () => {
    mockSendMessage.mockImplementation(() =>
      Promise.resolve({
        response: 'Hello! How can I help?',
        conversationId: 'conv-123',
        timestamp: new Date().toISOString(),
      })
    );

    render(<ChatInterface />);
    
    const input = screen.getByPlaceholderText(/Ask me anything about your training/i);
    const sendButton = screen.getByLabelText('Send message');

    await userEvent.type(input, 'Hello Coach Tom!');
    fireEvent.click(sendButton);

    // Message should appear in the chat
    await waitFor(() => {
      expect(screen.getByText('Hello Coach Tom!')).toBeDefined();
    });
  });

  it('should show typing indicator while waiting for response', async () => {
    // Make the API call take some time
    mockSendMessage.mockImplementation(
      () => new Promise((resolve) => {
        setTimeout(() => {
          resolve({
            response: 'Hello!',
            conversationId: 'conv-123',
            timestamp: new Date().toISOString(),
          });
        }, 100);
      })
    );

    render(<ChatInterface />);
    
    const input = screen.getByPlaceholderText(/Ask me anything about your training/i);
    const sendButton = screen.getByLabelText('Send message');

    await userEvent.type(input, 'Hello');
    fireEvent.click(sendButton);

    // Should show typing indicator
    await waitFor(() => {
      expect(screen.getByLabelText('Coach Tom is typing')).toBeDefined();
    });
  });

  it('should load conversation history when conversationId is provided', async () => {
    const mockHistory = {
      conversationId: 'conv-123',
      userId: 'user-456',
      messages: [
        {
          id: 'msg-1',
          role: 0,
          content: 'Previous message',
          timestamp: new Date().toISOString(),
        },
      ],
      totalMessages: 1,
      createdAt: new Date().toISOString(),
    };

    mockGetHistory.mockImplementation(() => Promise.resolve(mockHistory));

    render(<ChatInterface initialConversationId="conv-123" />);

    // Should show loading state
    expect(screen.getByText(/Loading conversation/i)).toBeDefined();

    // Should load and display the message
    await waitFor(() => {
      expect(screen.getByText('Previous message')).toBeDefined();
    });
  });

  it('should display error when message fails to send', async () => {
    mockSendMessage.mockImplementation(() => Promise.reject(new Error('Network error')));

    render(<ChatInterface />);
    
    const input = screen.getByPlaceholderText(/Ask me anything about your training/i);
    const sendButton = screen.getByLabelText('Send message');

    await userEvent.type(input, 'Test message');
    fireEvent.click(sendButton);

    // Should show error message
    await waitFor(() => {
      expect(screen.getByText(/Failed to send message/i)).toBeDefined();
    });
  });
});
