import { getCart, removeFromCart } from '@/app/actions/cart';
import Link from 'next/link';
import Image from 'next/image';
import { CartItem } from '@/types';
import { RemoveButton } from './RemoveButton';

export default async function CartPage() {
  const cart = await getCart();

  if (!cart || cart.items.length === 0) {
    return (
      <div className="min-h-screen bg-white px-4 py-16 sm:px-6 lg:px-8">
        <div className="text-center">
          <h2 className="text-3xl font-bold tracking-tight text-gray-900">Your cart is empty</h2>
          <p className="mt-4 text-gray-500">
            Looks like you haven't added any items to the cart yet.
          </p>
          <div className="mt-6">
            <Link
              href="/"
              className="inline-flex items-center justify-center rounded-md border border-transparent bg-indigo-600 px-6 py-3 text-base font-medium text-white shadow-sm hover:bg-indigo-700"
            >
              Continue Shopping
            </Link>
          </div>
        </div>
      </div>
    );
  }

  return (
    <div className="min-h-screen bg-white">
      <div className="mx-auto max-w-2xl px-4 py-16 sm:px-6 lg:max-w-7xl lg:px-8">
        <h1 className="text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl">Shopping Cart</h1>

        <div className="mt-12 lg:grid lg:grid-cols-12 lg:gap-x-12 lg:items-start xl:gap-x-16">
          <section aria-labelledby="cart-heading" className="lg:col-span-7">
            <h2 id="cart-heading" className="sr-only">Items in your shopping cart</h2>

            <ul role="list" className="divide-y divide-gray-200 border-b border-t border-gray-200">
              {cart.items.map((item) => (
                <li key={item.productId} className="flex py-6 sm:py-10">
                  <div className="flex-shrink-0">
                    <div className="relative h-24 w-24 rounded-md object-cover object-center sm:h-48 sm:w-48 overflow-hidden bg-gray-100">
                        {item.productImageUrl ? (
                            <Image
                                src={item.productImageUrl}
                                alt={item.productName}
                                fill
                                className="object-cover object-center"
                            />
                        ) : (
                            <div className="flex h-full w-full items-center justify-center bg-gray-200 text-gray-400">No Image</div>
                        )}
                    </div>
                  </div>

                  <div className="ml-4 flex flex-1 flex-col justify-between sm:ml-6">
                    <div className="relative pr-9 sm:grid sm:grid-cols-2 sm:gap-x-6 sm:pr-0">
                      <div>
                        <div className="flex justify-between">
                          <h3 className="text-sm">
                            <Link href={`/products/${item.productId}`} className="font-medium text-gray-700 hover:text-gray-800">
                              {item.productName}
                            </Link>
                          </h3>
                        </div>
                        <p className="mt-1 text-sm font-medium text-gray-900">
                            {new Intl.NumberFormat('en-US', { style: 'currency', currency: item.currency }).format(item.unitPrice)}
                        </p>
                        <p className="mt-1 text-sm text-gray-500">Qty {item.quantity}</p>
                      </div>

                      <div className="mt-4 sm:mt-0 sm:pr-9">
                        <RemoveButton productId={item.productId} />
                      </div>
                    </div>
                  </div>
                </li>
              ))}
            </ul>
          </section>

          {/* Order summary */}
          <section
            aria-labelledby="summary-heading"
            className="mt-16 rounded-lg bg-gray-50 px-4 py-6 sm:p-6 lg:col-span-5 lg:mt-0 lg:p-8"
          >
            <h2 id="summary-heading" className="text-lg font-medium text-gray-900">
              Order summary
            </h2>

            <dl className="mt-6 space-y-4">
              <div className="flex items-center justify-between border-t border-gray-200 pt-4">
                <dt className="text-base font-medium text-gray-900">Order total</dt>
                <dd className="text-base font-medium text-gray-900">
                    {new Intl.NumberFormat('en-US', { style: 'currency', currency: cart.currency }).format(cart.totalAmount)}
                </dd>
              </div>
            </dl>

            <div className="mt-6">
              <Link
                href="/checkout"
                className="w-full rounded-md border border-transparent bg-indigo-600 px-4 py-3 text-base font-medium text-white shadow-sm hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 focus:ring-offset-gray-50 flex justify-center"
              >
                Checkout
              </Link>
            </div>
          </section>
        </div>
      </div>
    </div>
  );
}
