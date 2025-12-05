import Link from 'next/link';
import { getCart } from '@/app/actions/cart';
import { cookies } from 'next/headers';
import { UserMenu } from './UserMenu';

export async function Navbar() {
  const cart = await getCart();
  const cartCount = cart?.items.reduce((acc, item) => acc + item.quantity, 0) || 0;

  const cookieStore = await cookies();
  const userInfoCookie = cookieStore.get('user_info');
  let user = null;
  
  if (userInfoCookie) {
    try {
      user = JSON.parse(userInfoCookie.value);
    } catch (e) {
      // ignore invalid cookie
    }
  }

  return (
    <header className="bg-white border-b border-gray-200">
      <nav className="mx-auto max-w-7xl px-4 sm:px-6 lg:px-8">
        <div className="flex h-16 items-center justify-between">
          {/* Logo */}
          <div className="flex lg:min-w-0 lg:flex-1">
            <Link href="/" className="-m-1.5 p-1.5 text-2xl font-bold text-indigo-600">
              MyCommerce
            </Link>
          </div>

          {/* Right section */}
          <div className="flex items-center gap-x-8">
            <UserMenu user={user} />

            {/* Cart */}
            <div className="flex lg:ml-6">
              <Link href="/cart" className="group -m-2 flex items-center p-2">
                <svg
                  className="h-6 w-6 flex-shrink-0 text-gray-400 group-hover:text-gray-500"
                  fill="none"
                  viewBox="0 0 24 24"
                  strokeWidth="1.5"
                  stroke="currentColor"
                  aria-hidden="true"
                >
                  <path
                    strokeLinecap="round"
                    strokeLinejoin="round"
                    d="M15.75 10.5V6a3.75 3.75 0 10-7.5 0v4.5m11.356-1.993l1.263 12c.07.665-.45 1.243-1.119 1.243H4.25a1.125 1.125 0 01-1.12-1.243l1.264-12A1.125 1.125 0 015.513 7.5h12.974c.576 0 1.059.435 1.119 1.007zM8.625 10.5a.375.375 0 11-.75 0 .375.375 0 01.75 0zm7.5 0a.375.375 0 11-.75 0 .375.375 0 01.75 0z"
                  />
                </svg>
                <span className="ml-2 text-sm font-medium text-gray-700 group-hover:text-gray-800">
                  {cartCount}
                </span>
                <span className="sr-only">items in cart, view bag</span>
              </Link>
            </div>
          </div>
        </div>
      </nav>
    </header>
  );
}
