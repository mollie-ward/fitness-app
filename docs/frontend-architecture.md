# Frontend Architecture Documentation

## Overview

The FitnessApp frontend is built with **Next.js 16** using the App Router, **TypeScript**, and **Tailwind CSS**. It follows modern React best practices with a component-based architecture, centralized state management, and comprehensive testing.

## Technology Stack

### Core Technologies
- **Next.js 16.1.6** - React framework with App Router
- **React 19.2.3** - UI library
- **TypeScript 5.x** - Type-safe development
- **Tailwind CSS 4.x** - Utility-first CSS framework

### State Management
- **Zustand** - Lightweight state management with persistence
- **React Context** - For component-level state when needed

### API & Data Fetching
- **Axios** - HTTP client for API requests
- **OpenAPI TypeScript Codegen** - Auto-generated typed API clients

### Testing
- **Jest** - Unit and integration testing
- **React Testing Library** - Component testing
- **@testing-library/user-event** - User interaction testing

### Development Tools
- **ESLint** - Code linting
- **Prettier** - Code formatting
- **TypeScript** - Static type checking

## Project Structure

```
frontend/
├── src/
│   ├── app/                      # Next.js App Router pages
│   │   ├── layout.tsx            # Root layout with navigation
│   │   ├── page.tsx              # Home page
│   │   └── globals.css           # Global styles
│   │
│   ├── components/               # React components
│   │   ├── auth/                 # Authentication components
│   │   │   └── protected-route.tsx
│   │   ├── layout/               # Layout components
│   │   │   ├── navigation.tsx
│   │   │   └── error-boundary.tsx
│   │   ├── ui/                   # Reusable UI primitives
│   │   │   ├── button.tsx
│   │   │   ├── input.tsx
│   │   │   └── card.tsx
│   │   └── features/             # Feature-specific components
│   │
│   ├── hooks/                    # Custom React hooks
│   │   └── use-auth.ts           # Authentication hook
│   │
│   ├── lib/                      # Utility functions and shared logic
│   │   ├── utils.ts              # General utilities (cn, etc.)
│   │   ├── auth-store.ts         # Zustand auth store
│   │   └── api/                  # API client configuration
│   │       └── api-client.ts     # Axios instance with interceptors
│   │
│   ├── services/                 # API services
│   │   └── api-client/           # Auto-generated from OpenAPI (gitignored)
│   │
│   ├── types/                    # TypeScript type definitions
│   │
│   └── __tests__/                # Test files
│       ├── unit/                 # Unit tests
│       ├── component/            # Component tests
│       └── integration/          # Integration tests
│
├── public/                       # Static assets
├── .env.local                    # Environment variables (gitignored)
├── .env.local.example            # Environment template
├── jest.config.ts                # Jest configuration
├── next.config.ts                # Next.js configuration
├── tsconfig.json                 # TypeScript configuration
└── package.json                  # Dependencies and scripts

```

## Key Features

### 1. Authentication System

**Implementation:** Zustand store with localStorage persistence

**Features:**
- JWT token management (access + refresh tokens)
- Automatic token refresh on 401 responses
- Protected route wrapper component
- Login/logout functionality
- User session persistence

**Files:**
- `src/lib/auth-store.ts` - Zustand store for auth state
- `src/hooks/use-auth.ts` - Custom hook for auth operations
- `src/components/auth/protected-route.tsx` - Route protection
- `src/lib/api/api-client.ts` - HTTP interceptors for auth headers

### 2. API Client

**Implementation:** Axios with request/response interceptors

**Features:**
- Automatic authentication header injection
- Token refresh on 401 errors
- Centralized error handling
- Base URL configuration from environment
- TypeScript typed responses

**Configuration:**
```typescript
// Base URL from environment
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_API_VERSION=v1

// Generates: http://localhost:5000/api/v1
```

### 3. State Management

**Zustand Store Pattern:**
```typescript
// Persistent auth store
useAuthStore
  - user: User | null
  - accessToken: string | null
  - refreshToken: string | null
  - isAuthenticated: boolean
  - setAuth(user, accessToken, refreshToken)
  - clearAuth()
  - updateTokens(accessToken, refreshToken)
```

**Persistence:**
- Auth state persists to localStorage
- Automatic hydration on app load
- Survives page refreshes

### 4. Component Library

**UI Primitives (shadcn/ui pattern):**
- Button - Multiple variants (default, destructive, outline, ghost, link)
- Input - Form input with consistent styling
- Card - Container with header, content, footer sections

**Styling Approach:**
- Tailwind CSS utility classes
- Class variance authority (CVA) for variants
- `cn()` utility for class merging

### 5. Routing & Navigation

**App Router Structure:**
- Server components by default
- Client components marked with 'use client'
- Automatic code splitting
- Nested layouts support

**Navigation:**
- Responsive navigation bar
- Conditional rendering based on auth state
- Active link highlighting
- Mobile-friendly design

### 6. Error Handling

**Error Boundary:**
- Class component wrapping the app
- Catches React rendering errors
- Displays user-friendly error UI
- Logs errors to console

**API Error Handling:**
- 401 → Token refresh attempt
- Failed refresh → Redirect to login
- Network errors → User notification

## Environment Variables

### Required Variables

```bash
# API Configuration
NEXT_PUBLIC_API_URL=http://localhost:5000
NEXT_PUBLIC_API_VERSION=v1

# Authentication
NEXT_PUBLIC_JWT_REFRESH_INTERVAL=840000  # 14 minutes in ms

# Feature Flags
NEXT_PUBLIC_ENABLE_STORYBOOK=false
```

### Usage

```typescript
const API_URL = process.env.NEXT_PUBLIC_API_URL;
```

**Note:** Only variables prefixed with `NEXT_PUBLIC_` are exposed to the browser.

## Testing Strategy

### Test Coverage Requirements
- **Unit tests:** ≥85% coverage for utilities and hooks
- **Component tests:** ≥85% coverage for shared components
- **Integration tests:** ≥85% coverage for auth flows

### Test Organization

```
__tests__/
├── unit/              # Pure function tests
│   ├── utils.test.ts
│   └── auth-store.test.ts
├── component/         # React component tests
│   ├── button.test.tsx
│   └── protected-route.test.tsx
└── integration/       # Multi-component/API tests
```

### Running Tests

```bash
npm test              # Run all tests
npm run test:watch    # Watch mode
npm run test:coverage # Coverage report
```

## Build & Deployment

### Development

```bash
npm run dev           # Start dev server (http://localhost:3000)
npm run lint          # Run ESLint
npm run format        # Format with Prettier
npm run type-check    # TypeScript validation
```

### Production

```bash
npm run build         # Create optimized production build
npm start             # Start production server
```

### Build Output

- Static pages are pre-rendered at build time
- Dynamic pages use server-side rendering
- Client components are code-split automatically
- Build artifacts in `.next/` directory

## API Client Generation

### OpenAPI Integration

```bash
npm run generate:api  # Generate TypeScript client from OpenAPI spec
```

**Requirements:**
- Backend API must be running
- OpenAPI spec available at `http://localhost:5000/swagger/v1/swagger.json`

**Output:**
- Generated files in `src/services/api-client/`
- Fully typed API methods
- Models matching backend DTOs

**Note:** Generated files are gitignored and should be regenerated in each environment.

## Performance Optimization

### Code Splitting
- Automatic route-based code splitting
- Dynamic imports for heavy components
- Lazy loading of non-critical features

### Caching Strategy
- Static assets cached by Next.js
- API responses can use SWR or React Query (future)
- Service worker for offline support (future)

### Bundle Analysis

```bash
# Analyze bundle size
npm run build
# Check build output for bundle sizes
```

## Security Considerations

### Authentication
- JWT tokens stored in localStorage (XSS risk acknowledged)
- HTTPS required in production
- Token refresh mechanism prevents long-lived tokens
- Automatic logout on token expiry

### API Security
- CORS configured on backend
- Authentication headers on all requests
- No sensitive data in URLs
- Input validation on forms

### Content Security
- Error boundary prevents info leakage
- No inline scripts (Next.js default)
- Secure headers configured

## Accessibility

### Current Implementation
- Semantic HTML elements
- Proper heading hierarchy
- Button and link roles
- Focus management in modals

### Future Enhancements
- ARIA labels for complex components
- Keyboard navigation improvements
- Screen reader testing
- WCAG 2.1 AA compliance

## Browser Support

- **Modern browsers:** Chrome, Firefox, Safari, Edge (latest 2 versions)
- **Mobile:** iOS Safari, Chrome Android
- **ES2017+** features used

## Development Workflow

### Adding a New Feature

1. **Create component** in appropriate directory
2. **Add types** in `src/types/` if needed
3. **Write tests** before or alongside implementation
4. **Update documentation** if adding new patterns
5. **Run linting and tests** before committing

### Code Style

- **TypeScript:** Strict mode enabled
- **React:** Functional components with hooks
- **Naming:** camelCase for functions, PascalCase for components
- **File naming:** kebab-case for files, PascalCase for components
- **Imports:** Use @ alias for src imports

## Known Limitations

1. **OpenAPI client generation** requires backend to be running
2. **Google Fonts** disabled due to build environment restrictions (using system fonts)
3. **Test coverage** currently at ~85% for tested modules
4. **Mobile navigation** needs hamburger menu implementation
5. **Accessibility** needs comprehensive audit

## Future Improvements

- [ ] Add React Query for API state management
- [ ] Implement Storybook for component documentation
- [ ] Add E2E tests with Playwright
- [ ] Implement PWA features (service worker, offline support)
- [ ] Add internationalization (i18n)
- [ ] Optimize images with Next.js Image component
- [ ] Add performance monitoring (Web Vitals)
- [ ] Implement comprehensive accessibility testing

## Troubleshooting

### Common Issues

**Build fails with font error:**
- Using Google Fonts requires internet access
- Solution: Use system fonts or local font files

**Tests fail with module resolution:**
- Ensure `@/` path alias is configured in both tsconfig.json and jest.config.ts

**API calls return 401:**
- Check if tokens are present in auth store
- Verify backend is running and accessible
- Check CORS configuration

**Hot reload not working:**
- Clear `.next` cache directory
- Restart development server

## References

- [Next.js Documentation](https://nextjs.org/docs)
- [React Documentation](https://react.dev)
- [Tailwind CSS](https://tailwindcss.com)
- [Zustand](https://zustand.docs.pmnd.rs/)
- [React Testing Library](https://testing-library.com/react)
