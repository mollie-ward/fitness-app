/**
 * Tests for MessageInput component
 */
import { describe, it, expect, jest } from '@jest/globals';
import { render, screen, fireEvent } from '@testing-library/react';
import userEvent from '@testing-library/user-event';
import { MessageInput } from '@/components/coach/MessageInput';

describe('MessageInput', () => {
  it('should render textarea and send button', () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    expect(screen.getByPlaceholderText(/Type your message/i)).toBeDefined();
    expect(screen.getByLabelText('Send message')).toBeDefined();
  });

  it('should update textarea value when typing', async () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    const textarea = screen.getByPlaceholderText(/Type your message/i) as HTMLTextAreaElement;
    await userEvent.type(textarea, 'Hello Coach!');
    
    expect(textarea.value).toBe('Hello Coach!');
  });

  it('should call onSendMessage when send button is clicked', async () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    const textarea = screen.getByPlaceholderText(/Type your message/i);
    const sendButton = screen.getByLabelText('Send message');
    
    await userEvent.type(textarea, 'Test message');
    fireEvent.click(sendButton);
    
    expect(mockOnSend).toHaveBeenCalledWith('Test message');
  });

  it('should call onSendMessage when Enter key is pressed', async () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    const textarea = screen.getByPlaceholderText(/Type your message/i);
    await userEvent.type(textarea, 'Test message{Enter}');
    
    expect(mockOnSend).toHaveBeenCalledWith('Test message');
  });

  it('should not call onSendMessage when Shift+Enter is pressed', async () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    const textarea = screen.getByPlaceholderText(/Type your message/i);
    await userEvent.type(textarea, 'Line 1{Shift>}{Enter}{/Shift}Line 2');
    
    expect(mockOnSend).not.toHaveBeenCalled();
    expect((textarea as HTMLTextAreaElement).value).toContain('Line 1');
  });

  it('should clear textarea after sending message', async () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    const textarea = screen.getByPlaceholderText(/Type your message/i) as HTMLTextAreaElement;
    const sendButton = screen.getByLabelText('Send message');
    
    await userEvent.type(textarea, 'Test message');
    fireEvent.click(sendButton);
    
    expect(textarea.value).toBe('');
  });

  it('should disable send button when message is empty', () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    const sendButton = screen.getByLabelText('Send message');
    expect(sendButton).toHaveProperty('disabled', true);
  });

  it('should disable input and button when disabled prop is true', () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} disabled={true} />);
    
    const textarea = screen.getByPlaceholderText(/Type your message/i);
    const sendButton = screen.getByLabelText('Send message');
    
    expect(textarea).toHaveProperty('disabled', true);
    expect(sendButton).toHaveProperty('disabled', true);
  });

  it('should not send empty or whitespace-only messages', async () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    const textarea = screen.getByPlaceholderText(/Type your message/i);
    const sendButton = screen.getByLabelText('Send message');
    
    await userEvent.type(textarea, '   ');
    fireEvent.click(sendButton);
    
    expect(mockOnSend).not.toHaveBeenCalled();
  });

  it('should display hint about keyboard shortcuts', () => {
    const mockOnSend = jest.fn();
    render(<MessageInput onSendMessage={mockOnSend} />);
    
    expect(screen.getByText(/Press Enter to send/i)).toBeDefined();
    expect(screen.getByText(/Shift\+Enter for new line/i)).toBeDefined();
  });
});
