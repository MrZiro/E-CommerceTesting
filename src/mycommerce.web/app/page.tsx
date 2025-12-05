import { apiClient } from '@/lib/api-client';
import { Product } from '@/types';
import { ProductGrid } from '@/components/features/ProductGrid';

export default async function Home() {
  let products: Product[] = [];

  try {
    products = await apiClient<Product[]>('/products', {
      isPublic: true,
      next: { tags: ['products'] },
    });
  } catch (error) {
    console.error('Failed to fetch products:', error);
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

        <ProductGrid products={products} />
      </main>
    </div>
  );
}
