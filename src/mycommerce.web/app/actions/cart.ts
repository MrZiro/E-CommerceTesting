'use server'

import { apiClient } from '@/lib/api-client';
import { revalidatePath } from 'next/cache';
import { Cart } from '@/types';

export async function getCart(): Promise<Cart | null> {
  try {
    const cart = await apiClient<Cart>('/cart', {
      next: { tags: ['cart'] },
    });
    return cart;
  } catch (error: any) {
    // If 401/404, it means no cart or not logged in
    if (error.status === 404 || error.status === 401) {
        return null;
    }
    console.error('Failed to fetch cart:', error);
    return null;
  }
}

export async function addToCart(productId: string, quantity: number = 1) {
  try {
    await apiClient('/cart/items', {
       method: 'POST',
       body: JSON.stringify({ productId, quantity }),
    });

    revalidatePath('/cart');
  } catch (error: any) {
      if (error.status === 401) {
          // Redirect to login if unauthorized
          // We can't easily redirect from here in a way that preserves state without helper logic
          // But throwing an error allows the client component to handle it.
          throw new Error('Unauthorized');
      }
      console.error("Add to cart failed", error);
      throw error;
  }
}

export async function removeFromCart(productId: string) {
    try {
        await apiClient(`/cart/items/${productId}`, {
            method: 'DELETE'
        });
        revalidatePath('/cart');
    } catch (error) {
        console.error("Remove from cart failed", error);
        throw error;
    }
}
