'use client';

import { createContext, useCallback, useContext, useEffect, useMemo, useState } from 'react';
import { apiRequest } from '@/lib/api';
import type { AuthResponse, CurrentUser } from '@/types';

interface AuthContextValue {
  user: CurrentUser | null;
  loading: boolean;
  login: (email: string, password: string) => Promise<CurrentUser>;
  logout: () => void;
  refreshUser: () => Promise<void>;
}

const AuthContext = createContext<AuthContextValue | undefined>(undefined);
const TOKEN_KEY = 'assignment_token';
const USER_KEY = 'assignment_user';

export function AuthProvider({ children }: { children: React.ReactNode }) {
  const [user, setUser] = useState<CurrentUser | null>(null);
  const [loading, setLoading] = useState(true);

  const clearSession = useCallback(() => {
    localStorage.removeItem(TOKEN_KEY);
    localStorage.removeItem(USER_KEY);
    setUser(null);
  }, []);

  const refreshUser = useCallback(async () => {
    const token = localStorage.getItem(TOKEN_KEY);
    if (!token) {
      setLoading(false);
      return;
    }

    try {
      const currentUser = await apiRequest<CurrentUser>('/auth/me');
      localStorage.setItem(USER_KEY, JSON.stringify(currentUser));
      setUser(currentUser);
    } catch {
      clearSession();
    } finally {
      setLoading(false);
    }
  }, [clearSession]);

  useEffect(() => {
    const storedUser = localStorage.getItem(USER_KEY);
    if (storedUser) {
      try {
        setUser(JSON.parse(storedUser) as CurrentUser);
      } catch {
        localStorage.removeItem(USER_KEY);
      }
    }
    void refreshUser();
  }, [refreshUser]);

  const login = useCallback(async (email: string, password: string) => {
    const response = await apiRequest<AuthResponse>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
    });
    localStorage.setItem(TOKEN_KEY, response.token);
    localStorage.setItem(USER_KEY, JSON.stringify(response.user));
    setUser(response.user);
    return response.user;
  }, []);

  const value = useMemo(
    () => ({ user, loading, login, logout: clearSession, refreshUser }),
    [user, loading, login, clearSession, refreshUser],
  );

  return <AuthContext.Provider value={value}>{children}</AuthContext.Provider>;
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used inside AuthProvider.');
  }
  return context;
}
