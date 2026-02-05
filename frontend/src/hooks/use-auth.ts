'use client';

import { useAuthStore } from '@/lib/auth-store';
import { useRouter } from 'next/navigation';
import { useCallback } from 'react';
import apiClient from '@/lib/api/api-client';

interface LoginCredentials {
  email: string;
  password: string;
}

interface RegisterCredentials {
  email: string;
  password: string;
  name: string;
}

interface LoginResponse {
  user: {
    id: string;
    email: string;
    name: string;
    emailVerified: boolean;
  };
  accessToken: string;
  refreshToken: string;
  expiresIn: number;
}

export function useAuth() {
  const router = useRouter();
  const { user, isAuthenticated, setAuth, clearAuth } = useAuthStore();

  const login = useCallback(
    async (credentials: LoginCredentials) => {
      try {
        const response = await apiClient.post<LoginResponse>('/auth/login', credentials);
        const { user, accessToken, refreshToken } = response.data;
        
        setAuth(user, accessToken, refreshToken);
        router.push('/dashboard');
      } catch (error: unknown) {
        console.error('Login failed:', error);
        throw error;
      }
    },
    [setAuth, router]
  );

  const logout = useCallback(async () => {
    try {
      const { refreshToken } = useAuthStore.getState();
      if (refreshToken) {
        await apiClient.post('/auth/logout', { refreshToken });
      }
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      clearAuth();
      router.push('/login');
    }
  }, [clearAuth, router]);

  const register = useCallback(
    async (userData: RegisterCredentials) => {
      try {
        // Call the register endpoint - it returns a success message
        await apiClient.post('/auth/register', userData);
        
        // After successful registration, automatically log in
        await login({ email: userData.email, password: userData.password });
      } catch (error: unknown) {
        console.error('Registration failed:', error);
        throw error;
      }
    },
    [login]
  );

  return {
    user,
    isAuthenticated,
    login,
    logout,
    register,
  };
}
