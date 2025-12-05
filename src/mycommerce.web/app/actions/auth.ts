'use server'

import { apiClient } from '@/lib/api-client';
import { cookies } from 'next/headers';
import { redirect } from 'next/navigation';
import { ApiError, User } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

interface LoginState {
  errors?: {
    email?: string[];
    password?: string[];
    _form?: string[];
  };
  message?: string | null;
}

export async function login(prevState: LoginState, formData: FormData): Promise<LoginState> {
  const email = formData.get('email') as string;
  const password = formData.get('password') as string;

  // Basic validation
  if (!email || !password) {
    return {
      errors: {
        email: !email ? ['Email is required'] : [],
        password: !password ? ['Password is required'] : [],
      },
      message: 'Please check your inputs.',
    };
  }

  try {
    const user = await apiClient<User>('/auth/login', {
      method: 'POST',
      body: JSON.stringify({ email, password }),
      isPublic: true,
    });

    if (user && user.token) {
      const cookieStore = await cookies();
      cookieStore.set('auth_token', user.token, {
        httpOnly: true,
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'lax',
        maxAge: 60 * 60 * 24 * 7, // 1 week
        path: '/',
      });
      
      cookieStore.set('user_info', JSON.stringify({ 
          id: user.id, 
          firstName: user.firstName, 
          lastName: user.lastName, 
          email: user.email 
      }), {
        httpOnly: false, // Accessible to client for UI
        secure: process.env.NODE_ENV === 'production',
        sameSite: 'lax',
        maxAge: 60 * 60 * 24 * 7, // 1 week
        path: '/',
      });
    }
  } catch (error: any) {
    console.error('Login failed:', error);
    const apiError = error as ApiError;
    return {
        message: apiError.detail || 'Invalid credentials',
        errors: {
            _form: [apiError.title || 'Login failed']
        }
    };
  }

  redirect('/');
}

export async function logout() {
  const cookieStore = await cookies();
  cookieStore.delete('auth_token');
  cookieStore.delete('user_info');
  redirect('/login');
}
