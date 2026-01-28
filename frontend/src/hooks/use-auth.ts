'use client';

import { useAuthStore } from '@/lib/auth-store';
import { useRouter } from 'next/navigation';
import { useCallback } from 'react';
import apiClient from '@/lib/api/api-client';

interface LoginCredentials {
  email: string;
  password: string;
}

interface LoginResponse {
  user: {
    id: string;
    email: string;
    firstName?: string;
    lastName?: string;
  };
  accessToken: string;
  refreshToken: string;
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
      } catch (error) {
        console.error('Login failed:', error);
        throw error;
      }
    },
    [setAuth, router]
  );

  const logout = useCallback(async () => {
    try {
      await apiClient.post('/auth/logout');
    } catch (error) {
      console.error('Logout error:', error);
    } finally {
      clearAuth();
      router.push('/login');
    }
  }, [clearAuth, router]);

  const register = useCallback(
    async (userData: {
      email: string;
      password: string;
      firstName?: string;
      lastName?: string;
    }) => {
      try {
        const response = await apiClient.post<LoginResponse>('/auth/register', userData);
        const { user, accessToken, refreshToken } = response.data;
        
        setAuth(user, accessToken, refreshToken);
        router.push('/onboarding');
      } catch (error) {
        console.error('Registration failed:', error);
        throw error;
      }
    },
    [setAuth, router]
  );

  return {
    user,
    isAuthenticated,
    login,
    logout,
    register,
  };
}
