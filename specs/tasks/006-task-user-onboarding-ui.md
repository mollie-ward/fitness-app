# Task: User Onboarding UI Flow

**Task ID:** 006  
**GitHub Issue:** [#11](https://github.com/mollie-ward/fitness-app/issues/11)  
**Feature:** User Onboarding & Fitness Profiling (FRD-001)  
**Priority:** P0 (Critical)  
**Estimated Effort:** Large  

---

## Description

Implement the frontend user onboarding experience as a multi-step form that collects fitness levels, goals, schedule availability, injuries, and training background. Integrate with backend API to create user profiles.

---

## Dependencies

- **Task 002:** Frontend scaffolding must be complete
- **Task 005:** User onboarding API endpoints must be available

---

## Technical Requirements

### Multi-Step Form Component
- Create onboarding wizard with clear progress indication
- Implement navigation between steps (Next, Back, Skip)
- Support step validation before proceeding
- Persist form state across steps (prevent data loss)
- Show progress indicator (e.g., "Step 2 of 6")

### Onboarding Steps

#### Step 1: Welcome & Introduction
- Display Coach Tom avatar
- Brief explanation of app purpose
- Call-to-action to begin onboarding
- No data collection on this step

#### Step 2: Fitness Level Assessment
- Three separate selectors for HYROX, Running, Strength
- Clear definitions for Beginner, Intermediate, Advanced
- Visual indicators or icons for each level
- Help text explaining what each level means
- Allow setting different levels per discipline

#### Step 3: Goal Setting
- Goal type selector (HYROX race, running event, strength milestone, general fitness)
- Optional target date picker for time-bound goals
- Goal description text input
- Support for adding multiple goals
- Ability to mark primary goal

#### Step 4: Schedule Configuration
- Weekly calendar showing days of the week
- Tap/click to select available training days
- Visual indication of selected days
- Display count of selected days (e.g., "4 days/week")
- Validation: At least 1 day must be selected

#### Step 5: Injury & Limitations (Optional)
- Option to skip if no injuries
- Body part selector (shoulder, knee, back, etc.)
- Injury type (acute, chronic)
- Movement restrictions description
- Ability to add multiple injuries
- "No injuries" checkbox to skip

#### Step 6: Training Background
- Dropdown for structured training experience (None, Some, Extensive)
- Equipment familiarity questions
- Optional additional notes
- Review & submit screen showing summary

### Form State Management
- Use React Hook Form or similar for form management
- Implement field validation with error messages
- Store partial progress in localStorage (restore on return)
- Clear onboarding state after successful submission

### API Integration
- Generate typed API client from OpenAPI spec
- Call POST /api/users/profile on final submission
- Handle loading states during API call
- Display error messages if submission fails
- Retry mechanism for failed requests

### Responsive Design
- Optimize for mobile-first experience
- Ensure form inputs are touch-friendly
- Adapt layouts for tablet and desktop screens
- Maintain readability across all screen sizes

### Accessibility
- Proper form labels and ARIA attributes
- Keyboard navigation between form fields
- Focus management between steps
- Screen reader announcements for step changes
- Error messages are associated with form fields

### User Experience
- Smooth transitions between steps
- Inline validation with helpful error messages
- Confirmation before skipping important sections
- Success animation/message after completion
- Redirect to main app after successful onboarding

---

## Acceptance Criteria

- ✅ Onboarding wizard displays with clear progress indication
- ✅ Users can navigate forward and backward through steps
- ✅ Fitness levels can be selected independently for each discipline
- ✅ Goals can be created with optional target dates
- ✅ Weekly schedule can be configured with at least 1 day required
- ✅ Injuries can be added or skipped entirely
- ✅ Form validates required fields and shows errors
- ✅ Partial progress is saved in localStorage
- ✅ Submission calls backend API with correct data structure
- ✅ Success state redirects user to main app
- ✅ Error states display helpful messages and allow retry
- ✅ Coach Tom avatar is visible and consistent throughout

---

## Testing Requirements

### Unit Tests
- Test form validation logic
- Test step navigation (next, back, skip)
- Test state persistence to localStorage
- Test DTO mapping from form state to API request
- **Minimum coverage:** ≥85% for form logic

### Component Tests
- Test each onboarding step renders correctly
- Test form field interactions (select, input, date picker)
- Test error messages display for validation failures
- Test progress indicator updates correctly
- Test navigation buttons enable/disable appropriately
- **Minimum coverage:** ≥85% for UI components

### Integration Tests
- Test complete onboarding flow from start to finish
- Test API integration submits correct data structure
- Test error handling when API call fails
- Test success state transitions to main app
- Test localStorage persistence and restoration
- **Minimum coverage:** ≥85% for user flows

### Accessibility Tests
- Test keyboard navigation through entire flow
- Test screen reader compatibility
- Test focus management between steps
- Test ARIA labels are present and correct

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Code follows React and TypeScript best practices
- Form is accessible (keyboard navigation, screen reader)
- Responsive design works on mobile, tablet, and desktop
- Error states are user-friendly and actionable
- Coach Tom avatar displays consistently
- No console errors or warnings
- Form data maps correctly to API DTOs
