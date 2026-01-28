# Task: Frontend Scaffolding

**Task ID:** 002  
**Feature:** Infrastructure  
**Priority:** P0 (Critical)  
**Estimated Effort:** Large  

---

## Description

Set up the complete frontend infrastructure using Next.js, React, and TypeScript. Establish project structure, routing, state management, API client generation, styling framework, and development tooling. This task creates the foundation for all frontend features.

---

## Dependencies

- **Task 001:** Backend scaffolding must be complete to generate typed API clients from OpenAPI spec

---

## Technical Requirements

### Project Initialization
- Create Next.js 14+ project with App Router
- Configure TypeScript with strict mode enabled
- Set up ESLint and Prettier for code quality
- Configure path aliases (@/ for imports)
- Set up package.json scripts for development, build, and testing

### Project Structure
- Organize app directory following Next.js conventions:
  - `app/` - Pages and layouts
  - `components/` - Reusable UI components
  - `lib/` - Utility functions and shared logic
  - `hooks/` - Custom React hooks
  - `services/` - API client and service layer
  - `types/` - TypeScript type definitions
  - `public/` - Static assets
- Implement consistent folder naming and organization

### Styling & UI Framework
- Set up Tailwind CSS for utility-first styling
- Configure design system foundation (colors, typography, spacing)
- Set up component library or UI primitives (shadcn/ui, Radix UI, or similar)
- Configure responsive design breakpoints
- Set up CSS variables for theming support

### API Client Generation
- Install and configure OpenAPI TypeScript code generator
- Generate typed API client from backend OpenAPI spec
- Set up API client service with base URL configuration
- Implement HTTP interceptors for authentication headers
- Configure error handling for API responses

### State Management
- Set up React Context for global state (or Zustand/Redux if needed)
- Implement authentication state management
- Create state management patterns for user profile, training plan, etc.
- Configure state persistence (localStorage/sessionStorage where appropriate)

### Routing & Navigation
- Configure Next.js App Router with layouts
- Set up protected route wrapper for authenticated pages
- Create navigation component structure
- Implement route-based code splitting
- Configure route transitions and loading states

### Authentication Integration
- Implement JWT token storage and refresh logic
- Create authentication context and hooks
- Set up login/logout functionality
- Implement protected route guards
- Configure automatic token refresh mechanism

### Development Tooling
- Configure TypeScript path mapping
- Set up hot module replacement
- Configure environment variables (.env.local)
- Set up Storybook for component development (optional but recommended)
- Configure bundle analyzer for optimization

### Testing Setup
- Install and configure Jest for unit tests
- Set up React Testing Library
- Configure test utilities and custom renders
- Set up test coverage reporting
- Create test utilities for mocking API calls

---

## Acceptance Criteria

- ✅ Next.js development server runs successfully on localhost
- ✅ TypeScript compilation completes without errors
- ✅ ESLint and Prettier run without errors
- ✅ Tailwind CSS styles apply correctly to components
- ✅ API client is generated from backend OpenAPI spec with typed methods
- ✅ Navigation between routes works smoothly
- ✅ Authentication state persists across page refreshes
- ✅ Environment variables load correctly in development
- ✅ Production build completes successfully with `npm run build`
- ✅ All imports use path aliases consistently
- ✅ Component library/UI primitives are accessible and styled

---

## Testing Requirements

### Unit Tests
- Test API client initialization and configuration
- Test authentication context and hooks
- Test utility functions and helpers
- Test custom React hooks
- **Minimum coverage:** ≥85% for utility functions and hooks

### Component Tests
- Test protected route guards redirect correctly
- Test navigation component renders and links work
- Test error boundary catches and displays errors
- Test loading states display correctly
- **Minimum coverage:** ≥85% for shared components

### Integration Tests
- Test API client makes authenticated requests with correct headers
- Test token refresh mechanism works correctly
- Test state persistence across page reloads
- **Minimum coverage:** ≥85% for authentication flows

---

## Definition of Done

- All acceptance criteria met
- All tests pass with ≥85% coverage
- Code follows React and TypeScript best practices
- ESLint configuration enforces code quality rules
- OpenAPI-generated client is properly typed and working
- Documentation updated in `/docs` with frontend architecture
- No console errors or warnings in development
- Production build size is optimized (analyze bundle)
- Accessibility basics configured (semantic HTML, ARIA where needed)
