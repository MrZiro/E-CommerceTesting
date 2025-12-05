import { apiClient } from '@/lib/api-client';
import { Product } from '@/types';
import { ProductGrid } from '@/components/features/ProductGrid';

/**
 * Render the home page showing a list of products.
 *
 * Fetches products from the API and renders the page header and a ProductGrid populated with the fetched products.
 * If fetching fails the product list will remain empty and the error is logged to the console.
 *
 * @returns The page's JSX element containing the header and product grid.
 */
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