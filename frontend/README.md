# FitnessApp Frontend

This is the frontend application for FitnessApp, built with Next.js 16, React, TypeScript, and Tailwind CSS.

## 🚀 Getting Started

### Prerequisites

- Node.js 20.x or later
- npm 10.x or later

### Installation

```bash
npm install
```

### Development

```bash
npm run dev
```

Open [http://localhost:3000](http://localhost:3000) with your browser to see the result.

### Environment Variables

Copy `.env.local.example` to `.env.local` and configure:

```bash
cp .env.local.example .env.local
```

Required variables:
- `NEXT_PUBLIC_API_URL` - Backend API URL (default: http://localhost:5000)
- `NEXT_PUBLIC_API_VERSION` - API version (default: v1)

## 📦 Available Scripts

- `npm run dev` - Start development server
- `npm run build` - Create production build
- `npm start` - Start production server
- `npm run lint` - Run ESLint
- `npm run lint:fix` - Fix ESLint errors
- `npm run format` - Format code with Prettier
- `npm run format:check` - Check code formatting
- `npm test` - Run tests
- `npm run test:watch` - Run tests in watch mode
- `npm run test:coverage` - Run tests with coverage report
- `npm run type-check` - Run TypeScript type checking
- `npm run generate:api` - Generate API client from OpenAPI spec

## 🏗️ Project Structure

```
src/
├── app/                      # Next.js App Router pages
│   ├── layout.tsx            # Root layout
│   ├── page.tsx              # Home page
│   ├── login/                # Login page
│   ├── register/             # Registration page
│   └── dashboard/            # Dashboard (protected)
├── components/               # React components
│   ├── auth/                 # Authentication components
│   ├── layout/               # Layout components
│   ├── ui/                   # Reusable UI components
│   └── features/             # Feature-specific components
├── hooks/                    # Custom React hooks
├── lib/                      # Utility functions
│   ├── auth-store.ts         # Authentication state
│   ├── utils.ts              # Helper functions
│   └── api/                  # API client configuration
├── services/                 # API services
├── types/                    # TypeScript types
└── __tests__/                # Test files
    ├── unit/                 # Unit tests
    ├── component/            # Component tests
    └── integration/          # Integration tests
```

## 🧪 Testing

Tests are written using Jest and React Testing Library.

### Running Tests

```bash
# Run all tests
npm test

# Run tests in watch mode
npm run test:watch

# Run with coverage
npm run test:coverage
```

### Test Coverage

Current coverage targets:
- ✅ **lib/**: 85%+ coverage (core utilities and state)
- ✅ **components/auth**: 100% coverage (authentication components)
- ✅ **components/ui**: Test coverage for Button component

## 🎨 Styling

This project uses:
- **Tailwind CSS 4** - Utility-first CSS framework
- **CSS Variables** - For theming support
- **shadcn/ui patterns** - For component variants

## 🔐 Authentication

Authentication is handled via:
- JWT tokens (access + refresh)
- Zustand store with localStorage persistence
- Automatic token refresh on 401 responses
- Protected route wrapper for authenticated pages

## 📚 Documentation

See `/docs/frontend-architecture.md` for detailed architecture documentation.

## 🔧 API Integration

The frontend communicates with the backend API using:
- Axios HTTP client
- Auto-generated TypeScript API client (from OpenAPI spec)
- Request/response interceptors for authentication

### Generating API Client

Make sure the backend is running, then:

```bash
npm run generate:api
```

This will generate typed API methods in `src/services/api-client/`.

## 🚢 Deployment

### Production Build

```bash
npm run build
npm start
```

The build output is optimized and ready for deployment to any Node.js hosting platform.

### Environment Variables

Ensure all environment variables are set in your production environment:
- `NEXT_PUBLIC_API_URL` - Production API URL
- `NEXT_PUBLIC_API_VERSION` - API version

## 🐛 Troubleshooting

### Build Issues

If you encounter build errors:
1. Clear `.next` cache: `rm -rf .next`
2. Reinstall dependencies: `rm -rf node_modules package-lock.json && npm install`
3. Run type check: `npm run type-check`

### API Connection Issues

If API calls fail:
1. Check backend is running
2. Verify `NEXT_PUBLIC_API_URL` in `.env.local`
3. Check browser console for CORS errors
4. Verify tokens in localStorage

## 📝 License

See LICENSE.md in the repository root.
