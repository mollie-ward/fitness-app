/**
 * Types for Coach Tom chat functionality
 */

export enum MessageRole {
  User = 0,
  Assistant = 1,
  System = 2,
}

export interface Message {
  id: string;
  role: MessageRole;
  content: string;
  timestamp: Date;
  triggeredAction?: string;
}

export interface ChatMessageRequest {
  message: string;
  conversationId?: string;
}

export interface ChatMessageResponse {
  response: string;
  conversationId: string;
  timestamp: string;
  triggeredAction?: string;
}

export interface ConversationHistory {
  conversationId: string;
  userId: string;
  messages: Message[];
  totalMessages: number;
  createdAt: string;
  updatedAt?: string;
}

export interface CoachAvatar {
  avatarUrl: string;
  name: string;
  description?: string;
}
