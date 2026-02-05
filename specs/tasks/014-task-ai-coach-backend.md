# Task: Conversational AI Coach Backend Integration

**Task ID:** 014  
**GitHub Issue:** [#27](https://github.com/mollie-ward/fitness-app/issues/27)  
**Feature:** Conversational AI Coach (FRD-003)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Extra Large  

---

## Description

Implement backend service for the conversational AI coach (Coach Tom), including integration with Azure OpenAI or similar LLM service, context management, conversation history, and plan modification capabilities triggered by chat interactions.

---

## Dependencies

- **Task 004:** User profile data model must be complete
- **Task 008:** Training plan data model must be complete
- **Task 012:** Progress tracking backend must be complete

---

## Technical Requirements

### AI Coach Service Architecture

#### Core Service Interface
- Create `IAICoachService` interface with methods:
  - SendMessageAsync(userId, message)
  - GetConversationHistoryAsync(userId, limit)
  - ClearConversationAsync(userId)
  - GetCoachResponseAsync(userId, userMessage, context)

#### LLM Integration
- Integrate with Azure OpenAI Service (or alternative LLM provider)
- Configure GPT-4 or Claude model for coaching conversations
- Set up API credentials and endpoint configuration
- Implement retry logic and fallback mechanisms
- Configure rate limiting and cost management

#### Prompt Engineering & System Instructions
- Create system prompt defining Coach Tom's personality:
  - Knowledgeable fitness expert
  - Supportive and motivational tone
  - Adaptive communication style (beginner-friendly to technical)
  - Honest and realistic expectations
  
- Define conversation guidelines:
  - Explain workout rationale in accessible language
  - Provide motivation without being pushy
  - Acknowledge challenges without judgment
  - Set realistic expectations
  
- Include safety disclaimers:
  - No medical diagnosis or treatment
  - Recommend professional help when appropriate
  - Avoid guaranteeing specific outcomes

#### Context Window Management
- Build dynamic context for each conversation:
  - User profile (name, fitness levels, goals)
  - Current training plan summary
  - Recent workout completions and misses
  - Active injuries or limitations
  - Conversation history (last 10-20 exchanges)
  
- Implement context compression for long conversations
- Prioritize recent and relevant context

#### Intent Recognition & Routing
- Identify user intent from message:
  - Asking for workout rationale
  - Reporting injury or limitation
  - Requesting plan modification (easier/harder)
  - Reporting schedule change
  - Seeking motivation
  - General fitness question
  - Out-of-scope query
  
- Route to appropriate handler:
  - Informational response (direct LLM)
  - Plan modification (trigger adaptation service)
  - Injury update (update profile + adapt plan)
  - Schedule change (trigger plan regeneration)

#### Plan Modification Handlers
- Create handlers for common modification requests:
  
  **Intensity Adjustment:**
  - Parse request ("make it harder", "too easy")
  - Trigger training plan adaptation service
  - Confirm changes to user with explanation
  
  **Schedule Changes:**
  - Parse new availability
  - Update user profile
  - Trigger plan regeneration
  - Explain impact on goals
  
  **Injury Reporting:**
  - Extract injury details (body part, pain type)
  - Update user profile injuries
  - Trigger injury-aware plan adaptation
  - Provide substitute exercises explanation

#### Conversation Storage
- Create `Conversation` entity:
  - UserId
  - Messages (list of user/assistant exchanges)
  - CreatedAt, UpdatedAt
  - ConversationContext (metadata)
  
- Create `Message` entity:
  - MessageId
  - ConversationId
  - Role (User, Assistant, System)
  - Content
  - Timestamp
  - Intent (recognized intent)
  - TriggeredAction (if message caused plan modification)

### API Endpoints

#### POST /api/coach/chat
- Send message to Coach Tom
- Request: User message text
- Response: Assistant response with conversation ID
- Includes conversation context automatically

#### GET /api/coach/conversations/{conversationId}/history
- Get conversation history
- Response: List of messages in conversation
- Paginated for long conversations

#### DELETE /api/coach/conversations/{conversationId}
- Clear conversation history
- Response: 204 No Content
- Useful for starting fresh or privacy

#### GET /api/coach/avatar
- Get Coach Tom avatar image URL
- Response: Avatar asset URL

### Safety & Content Filtering
- Implement content moderation:
  - Filter inappropriate requests
  - Detect and deflect medical diagnosis requests
  - Reject dangerous advice requests
- Include disclaimers in responses about:
  - Not replacing medical professionals
  - Seeking professional help for injuries
  - App limitations

### Monitoring & Logging
- Log all conversations for quality monitoring
- Track intent recognition accuracy
- Monitor LLM API costs and usage
- Alert on repeated failures or errors
- Track user satisfaction (thumbs up/down on responses)

### Fallback Mechanisms
- Canned responses for common scenarios when LLM unavailable
- Graceful degradation if API fails
- Clear error messages to users

---

## Acceptance Criteria

- ✅ Users can send messages and receive contextual responses
- ✅ Coach Tom references user's profile, plan, and progress
- ✅ Workout rationale questions receive clear explanations
- ✅ Plan modification requests trigger appropriate backend changes
- ✅ Injury reports update profile and trigger plan adaptation
- ✅ Schedule changes update availability and regenerate plan
- ✅ Conversation history is persisted and retrievable
- ✅ Out-of-scope questions are handled gracefully
- ✅ Responses include appropriate disclaimers
- ✅ LLM integration is secure and rate-limited

---

## Testing Requirements

### Unit Tests
- Test intent recognition logic
- Test context window building
- Test prompt construction
- Test message routing to handlers
- **Minimum coverage:** ≥85% for service logic

### Integration Tests
- Test LLM API integration returns responses
- Test conversation with context retrieval
- Test plan modification triggers work
- Test injury reporting updates profile
- Test conversation storage and retrieval
- Test rate limiting prevents abuse
- **Minimum coverage:** ≥85% for API and LLM integration

### Intent Recognition Tests
- Test detecting workout rationale questions
- Test detecting injury reports
- Test detecting plan modification requests
- Test detecting schedule changes
- Test detecting out-of-scope queries

### Safety Tests
- Test medical advice requests are deflected
- Test inappropriate content is filtered
- Test dangerous requests are rejected

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- LLM integration is secure and configured
- API endpoints are documented
- Conversation context includes relevant user data
- Plan modifications work end-to-end from chat
- Safety disclaimers are included
- Monitoring and logging are in place
- Cost management controls are configured
