import { useAuthStore } from '@/lib/auth-store';

describe('Auth Store', () => {
  beforeEach(() => {
    // Clear the store before each test
    useAuthStore.setState({
      user: null,
      accessToken: null,
      refreshToken: null,
      isAuthenticated: false,
    });
  });

  it('should initialize with default state', () => {
    const state = useAuthStore.getState();
    expect(state.user).toBeNull();
    expect(state.accessToken).toBeNull();
    expect(state.refreshToken).toBeNull();
    expect(state.isAuthenticated).toBe(false);
  });

  it('should set authentication state', () => {
    const mockUser = {
      id: '123',
      email: 'test@example.com',
      firstName: 'Test',
      lastName: 'User',
    };
    const mockAccessToken = 'access-token-123';
    const mockRefreshToken = 'refresh-token-123';

    useAuthStore.getState().setAuth(mockUser, mockAccessToken, mockRefreshToken);

    const state = useAuthStore.getState();
    expect(state.user).toEqual(mockUser);
    expect(state.accessToken).toBe(mockAccessToken);
    expect(state.refreshToken).toBe(mockRefreshToken);
    expect(state.isAuthenticated).toBe(true);
  });

  it('should clear authentication state', () => {
    const mockUser = {
      id: '123',
      email: 'test@example.com',
    };
    
    useAuthStore.getState().setAuth(mockUser, 'access', 'refresh');
    useAuthStore.getState().clearAuth();

    const state = useAuthStore.getState();
    expect(state.user).toBeNull();
    expect(state.accessToken).toBeNull();
    expect(state.refreshToken).toBeNull();
    expect(state.isAuthenticated).toBe(false);
  });

  it('should update tokens', () => {
    const mockUser = {
      id: '123',
      email: 'test@example.com',
    };
    
    useAuthStore.getState().setAuth(mockUser, 'old-access', 'old-refresh');
    useAuthStore.getState().updateTokens('new-access', 'new-refresh');

    const state = useAuthStore.getState();
    expect(state.user).toEqual(mockUser);
    expect(state.accessToken).toBe('new-access');
    expect(state.refreshToken).toBe('new-refresh');
    expect(state.isAuthenticated).toBe(true);
  });
});
