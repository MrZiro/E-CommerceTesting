import { NextResponse } from 'next/server';
import type { NextRequest } from 'next/server';

export function middleware(request: NextRequest) {
  const token = request.cookies.get('auth_token')?.value;
  const { pathname } = request.nextUrl;

  // Paths that require authentication
  const protectedPaths = ['/admin'];

  // Paths that are only for non-authenticated users (like login)
  // If user is logged in, redirect to home or dashboard
  const authPaths = ['/login', '/register'];

  const isProtectedPath = protectedPaths.some((path) => pathname.startsWith(path));
  const isAuthPath = authPaths.some((path) => pathname.startsWith(path));

  if (isProtectedPath && !token) {
    const loginUrl = new URL('/login', request.url);
    // loginUrl.searchParams.set('callbackUrl', pathname); // Optional: redirect back after login
    return NextResponse.redirect(loginUrl);
  }

  if (isAuthPath && token) {
     return NextResponse.redirect(new URL('/', request.url));
  }

  return NextResponse.next();
}

export const config = {
  matcher: [
    /*
     * Match all request paths except for the ones starting with:
     * - api (API routes)
     * - _next/static (static files)
     * - _next/image (image optimization files)
     * - favicon.ico (favicon file)
     * - public images (svg, png, jpg, etc.)
     */
    '/((?!api|_next/static|_next/image|favicon.ico|.*\.(?:svg|png|jpg|jpeg|gif|webp)$).*)',
  ],
};
