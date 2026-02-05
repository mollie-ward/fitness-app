/**
 * API service for Coach Tom chat operations
 */
import axios from 'axios';
import type {
  ChatMessageRequest,
  ChatMessageResponse,
  ConversationHistory,
  CoachAvatar,
} from '@/types/coach';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_URL || 'http://localhost:5000';

/**
 * Send a message to Coach Tom
 */
export async function sendChatMessage(
  message: string,
  conversationId?: string
): Promise<ChatMessageResponse> {
  const request: ChatMessageRequest = {
    message,
    conversationId,
  };

  const response = await axios.post<ChatMessageResponse>(
    `${API_BASE_URL}/api/v1/coach/chat`,
    request,
    {
      headers: {
        'Content-Type': 'application/json',
      },
      withCredentials: true,
    }
  );

  return response.data;
}

/**
 * Get conversation history
 */
export async function getConversationHistory(
  conversationId: string,
  limit: number = 50
): Promise<ConversationHistory> {
  const response = await axios.get<ConversationHistory>(
    `${API_BASE_URL}/api/v1/coach/conversations/${conversationId}/history`,
    {
      params: { limit },
      withCredentials: true,
    }
  );

  return response.data;
}

/**
 * Clear a conversation
 */
export async function clearConversation(conversationId: string): Promise<void> {
  await axios.delete(
    `${API_BASE_URL}/api/v1/coach/conversations/${conversationId}`,
    {
      withCredentials: true,
    }
  );
}

/**
 * Get Coach Tom's avatar information
 */
export async function getCoachAvatar(): Promise<CoachAvatar> {
  const response = await axios.get<CoachAvatar>(
    `${API_BASE_URL}/api/v1/coach/avatar`
  );

  return response.data;
}
