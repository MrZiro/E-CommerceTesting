'use client';

import { logout } from '@/app/actions/auth';
import Link from 'next/link';
import { useTransition } from 'react';

interface UserMenuProps {
  user: {
    firstName: string;
    email: string;
  } | null;
}

export function UserMenu({ user }: UserMenuProps) {
  const [isPending, startTransition] = useTransition();

  if (!user) {
    return (
      <div className="flex items-center gap-4">
        <Link href="/login" className="text-sm font-medium text-gray-700 hover:text-gray-800">
          Sign in
        </Link>
        <span className="h-6 w-px bg-gray-200" aria-hidden="true" />
        <Link href="#" className="text-sm font-medium text-gray-700 hover:text-gray-800">
          Create account
        </Link>
      </div>
    );
  }

  return (
    <div className="flex items-center gap-4">
      <span className="text-sm text-gray-700">Hello, {user.firstName}</span>
      <span className="h-6 w-px bg-gray-200" aria-hidden="true" />
      <button
        onClick={() => startTransition(() => logout())}
        disabled={isPending}
        className="text-sm font-medium text-gray-700 hover:text-red-600 disabled:opacity-50"
      >
        {isPending ? 'Signing out...' : 'Sign out'}
      </button>
    </div>
  );
}
