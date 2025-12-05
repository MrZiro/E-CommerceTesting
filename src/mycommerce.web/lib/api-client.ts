import { cookies } from 'next/headers';
import { ApiError } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

if (!API_BASE_URL) {
  throw new Error('NEXT_PUBLIC_API_BASE_URL is not defined');
}

type FetchOptions = Omit<RequestInit, 'headers'> & {
  headers?: Record<string, string>;
  isPublic?: boolean; // If true, skips auth token to avoid dynamic rendering opt-in on static pages
};

export async function apiClient<T>(endpoint: string, options: FetchOptions = {}): Promise<T> {
  const { isPublic = false, ...fetchOptions } = options;

  const headers: Record<string, string> = {
    'Content-Type': 'application/json',
    ...fetchOptions.headers,
  };

  if (!isPublic) {
    try {
      const cookieStore = await cookies();
      const tokenCookie = cookieStore.get('auth_token');
      const token = tokenCookie?.value;

      if (token) {
        headers['Authorization'] = `Bearer ${token}`;
      }
    } catch (error) {
      // This might happen if called from a context where cookies are not available
      // Ideally, we should ensure isPublic is set correctly by the caller.
    }
  }

  const response = await fetch(`${API_BASE_URL}${endpoint}`, {
    ...fetchOptions,
    headers,
  });

  if (!response.ok) {
    let errorDetails: ApiError;
    try {
      errorDetails = await response.json();
    } catch {
      errorDetails = {
        title: response.statusText,
        status: response.status,
      };
    }
    throw errorDetails;
  }

  // Handle 204 No Content
  if (response.status === 204) {
    return {} as T;
  }

  return response.json();
}
