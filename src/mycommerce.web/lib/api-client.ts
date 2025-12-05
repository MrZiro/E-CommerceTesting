import { cookies } from 'next/headers';
import { ApiError } from '@/types';

const API_BASE_URL = process.env.NEXT_PUBLIC_API_BASE_URL;

type FetchOptions = Omit<RequestInit, 'headers'> & {
  headers?: Record<string, string>;
  isPublic?: boolean; // If true, skips auth token to avoid dynamic rendering opt-in on static pages
};

/**
 * Perform an HTTP request against the configured API and return the parsed response body.
 *
 * @param endpoint - The request path appended to the API base URL (e.g., "/users").
 * @param options - Fetch options; when `isPublic` is false (default) an `Authorization: Bearer <token>` header
 *                  will be added from the `auth_token` cookie if present. Other provided headers are merged.
 * @returns The parsed JSON response as type `T`. Returns an empty object cast to `T` when the response status is 204.
 * @throws An `ApiError` containing `status` and `title` (and any returned error details) for non-successful responses.
 */
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