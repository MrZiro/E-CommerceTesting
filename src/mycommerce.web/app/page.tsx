import { apiClient } from '@/lib/api-client';
import { Product } from '@/types';
import { ProductGrid } from '@/components/features/ProductGrid';

export default async function Home() {
  let products: Product[] = [];
  let errorMessage: string | null = null;

  try {
    products = await apiClient<Product[]>('/products', {
      isPublic: true,
      next: { tags: ['products'] },
    });
  } catch (error: any) {
    console.error('Failed to fetch products:', error);
    errorMessage = error.detail || error.message || 'Failed to load products. Please try again later.';
  }

  return (
    <div className="min-h-screen bg-white">
      <main className="mx-auto max-w-7xl px-4 py-16 sm:px-6 lg:px-8">
        <div className="mb-10 text-center">
          <h1 className="text-4xl font-bold tracking-tight text-gray-900 sm:text-5xl">
            MyCommerce Store
          </h1>
          <p className="mt-4 text-xl text-gray-500">
            Explore our latest collection.
          </p>
        </div>

        {errorMessage ? (
          <div className="rounded-md bg-red-50 p-4">
            <div className="flex">
              <div className="ml-3">
                <h3 className="text-sm font-medium text-red-800">Error loading products</h3>
                <div className="mt-2 text-sm text-red-700">
                  <p>{errorMessage}</p>
                </div>
              </div>
            </div>
          </div>
        ) : (
          <ProductGrid products={products} />
        )}
      </main>
    </div>
  );
}
