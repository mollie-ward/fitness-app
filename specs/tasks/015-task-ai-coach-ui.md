# Task: Conversational AI Coach UI

**Task ID:** 015  
**Feature:** Conversational AI Coach (FRD-003)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Large  

---

## Description

Implement the frontend chat interface for Coach Tom, allowing users to send messages, view conversation history, and see plan modifications triggered by conversations. Display Coach Tom's avatar consistently throughout the experience.

---

## Dependencies

- **Task 002:** Frontend scaffolding must be complete
- **Task 014:** AI coach backend must be complete

---

## Technical Requirements

### Chat Interface Component

#### Chat Window
- Create modal or dedicated page for chat interface
- Display Coach Tom avatar prominently
- Show conversation history (scrollable)
- Message input field with send button
- "Typing..." indicator when AI is responding
- Smooth auto-scroll to latest message

#### Message Display
- **User messages:**
  - Right-aligned bubbles
  - Different color from assistant messages
  - User avatar or initial (optional)
  - Timestamp (relative or absolute)
  
- **Assistant (Coach Tom) messages:**
  - Left-aligned bubbles
  - Coach Tom avatar next to each message
  - Markdown formatting support (bold, lists, links)
  - Timestamp
  - Action buttons if message contains suggestions (e.g., "Yes, adjust my plan")

#### Message Input
- Text area for user input
- Character limit indicator (if applicable)
- Send button (enabled when text present)
- Enter key sends message (Shift+Enter for new line)
- Loading state when sending
- Clear field after successful send

### Conversation Features

#### Quick Actions / Suggested Prompts
- Display common questions as quick-tap buttons:
  - "Why this workout?"
  - "Make it harder"
  - "Make it easier"
  - "I'm injured"
  - "Change my schedule"
  - "How am I doing?"
- Quick actions insert text or send directly

#### Plan Modification Confirmation
- When Coach Tom suggests plan change:
  - Display change summary in message
  - Show "Confirm" / "Cancel" action buttons
  - On confirm, trigger backend modification
  - Show success confirmation with updated plan

#### Conversation History
- Load recent conversation on chat open
- Pagination for older messages ("Load more")
- Persist history across sessions
- Option to start new conversation (clear history with confirmation)

### Coach Tom Avatar Integration
- Display avatar in:
  - Chat interface (next to all Coach Tom messages)
  - Chat entry point (floating action button or menu item)
  - Onboarding flow
  - Calendar view (optional motivational messages)
  - Progress milestones
- Ensure consistent styling and sizing
- Optimize image loading

### API Integration
- Call POST /api/coach/chat for each message
- Fetch GET /api/coach/conversations/{id}/history on load
- Implement streaming for real-time response (if supported)
- Handle loading states during API calls
- Retry logic for failed requests

### Real-Time Response Rendering
- Show "Typing..." indicator while waiting for response
- Stream response word-by-word or sentence-by-sentence (if supported)
- Render markdown formatting in real-time
- Auto-scroll as message appears

### Error Handling
- Display user-friendly error if message fails to send
- Retry button for failed messages
- Offline detection and queuing
- Timeout handling for slow responses

### Accessibility
- Chat is keyboard navigable
- Screen reader announces new messages
- Proper ARIA labels for chat elements
- Focus management (input field after send)
- Alt text for Coach Tom avatar

### State Management
- Track conversation ID for current chat session
- Store messages in local state
- Sync with backend periodically
- Handle concurrent message sends

### Performance Optimization
- Virtualize message list for long conversations
- Lazy load conversation history
- Optimize re-renders with React.memo
- Debounce typing indicators (if implemented)

---

## Acceptance Criteria

- ✅ Users can open chat interface from app navigation
- ✅ Users can send messages and receive responses
- ✅ Coach Tom avatar displays consistently
- ✅ Conversation history loads and displays correctly
- ✅ Messages support basic markdown formatting
- ✅ Quick action buttons insert or send common prompts
- ✅ Plan modification confirmations work end-to-end
- ✅ "Typing..." indicator shows while awaiting response
- ✅ Chat is accessible via keyboard and screen reader
- ✅ Error states display helpful messages and retry options
- ✅ Works responsively on mobile, tablet, and desktop

---

## Testing Requirements

### Unit Tests
- Test message sending logic
- Test conversation state management
- Test markdown rendering
- **Minimum coverage:** ≥85% for component logic

### Component Tests
- Test chat window renders correctly
- Test message input and send button
- Test message bubbles display (user vs assistant)
- Test quick action buttons trigger messages
- Test "Typing..." indicator appears and disappears
- Test conversation history loads
- Test plan modification confirmation flow
- **Minimum coverage:** ≥85% for UI components

### Integration Tests
- Test sending message calls API and displays response
- Test conversation history fetches from API
- Test plan modification confirmation triggers backend
- Test error handling when API fails
- Test retry logic for failed messages
- **Minimum coverage:** ≥85% for data integration

### Accessibility Tests
- Test keyboard navigation through chat
- Test screen reader announces messages
- Test focus management after sending
- Test ARIA labels are present

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Code follows React and TypeScript best practices
- Chat interface is accessible
- Responsive design works on all devices
- Coach Tom avatar displays consistently
- Markdown formatting renders correctly
- Error handling is user-friendly
- API integration is efficient
