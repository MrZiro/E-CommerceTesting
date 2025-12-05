'use client';

import { addToCart } from '@/app/actions/cart';
import { useTransition } from 'react';
import { useRouter } from 'next/navigation';

interface AddToCartButtonProps {
  productId: string;
}

export function AddToCartButton({ productId }: AddToCartButtonProps) {
  const [isPending, startTransition] = useTransition();
  const router = useRouter();

  const handleAddToCart = () => {
    startTransition(async () => {
      try {
        await addToCart(productId);
        alert('Added to cart!'); // Temporary feedback
        router.refresh(); // Update UI if needed (e.g. cart count in header)
      } catch (error: any) {
        if (error.message === 'Unauthorized') {
            router.push('/login');
        } else {
            alert('Failed to add to cart. Please try again.');
        }
      }
    });
  };

  return (
    <button
      type="button"
      onClick={handleAddToCart}
      disabled={isPending}
      className="flex max-w-xs flex-1 items-center justify-center rounded-md border border-transparent bg-indigo-600 px-8 py-3 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 focus:ring-offset-gray-50 sm:w-full disabled:opacity-50 disabled:cursor-not-allowed"
    >
      {isPending ? 'Adding...' : 'Add to bag'}
    </button>
  );
}
