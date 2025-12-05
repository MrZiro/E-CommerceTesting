import Link from 'next/link';

export function Footer() {
  return (
    <footer className="bg-white border-t border-gray-200">
      <div className="mx-auto max-w-7xl px-4 py-12 sm:px-6 lg:px-8">
        <div className="grid grid-cols-1 md:grid-cols-3 gap-8">
          <div>
            <h3 className="text-sm font-semibold leading-6 text-gray-900">About Us</h3>
            <p className="mt-4 text-sm leading-6 text-gray-600">
              MyCommerce is your one-stop shop for everything. We provide quality products at affordable prices.
            </p>
          </div>
          <div>
            <h3 className="text-sm font-semibold leading-6 text-gray-900">Customer Service</h3>
            <ul role="list" className="mt-4 space-y-4">
              <li>
                <Link href="#" className="text-sm leading-6 text-gray-600 hover:text-gray-900">
                  Contact
                </Link>
              </li>
              <li>
                <Link href="#" className="text-sm leading-6 text-gray-600 hover:text-gray-900">
                  Shipping
                </Link>
              </li>
              <li>
                <Link href="#" className="text-sm leading-6 text-gray-600 hover:text-gray-900">
                  Returns
                </Link>
              </li>
            </ul>
          </div>
          <div>
            <h3 className="text-sm font-semibold leading-6 text-gray-900">Connect</h3>
            <ul role="list" className="mt-4 space-y-4">
              <li>
                <Link href="#" className="text-sm leading-6 text-gray-600 hover:text-gray-900">
                  Twitter
                </Link>
              </li>
              <li>
                <Link href="#" className="text-sm leading-6 text-gray-600 hover:text-gray-900">
                  Instagram
                </Link>
              </li>
            </ul>
          </div>
        </div>
        <div className="mt-8 border-t border-gray-900/10 pt-8">
          <p className="text-xs leading-5 text-gray-500">
            &copy; 2025 MyCommerce, Inc. All rights reserved.
          </p>
        </div>
      </div>
    </footer>
  );
}
