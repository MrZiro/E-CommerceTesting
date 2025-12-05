import { apiClient } from '@/lib/api-client';
import { Product } from '@/types';
import Image from 'next/image';
import { notFound } from 'next/navigation';

interface ProductPageProps {
  params: Promise<{
    id: string;
  }>;
}

/**
 * Render the product detail page for the product identified by the given route params.
 *
 * @param params - Promise resolving to route parameters containing the product `id` string
 * @returns The JSX for the product detail page. If the product cannot be fetched or is not found, `notFound()` is invoked instead.
 */
export default async function ProductPage({ params }: ProductPageProps) {
  const { id } = await params;
  let product: Product | null = null;

  try {
    product = await apiClient<Product>(`/products/${id}`, {
      isPublic: true,
      next: { tags: [`product-${id}`] },
    });
  } catch (error: any) {
    if (error.status === 404) {
      notFound();
    }
    console.error('Failed to fetch product:', error);
  }

  if (!product) {
    return notFound();
  }

  return (
    <div className="bg-white">
      <div className="mx-auto max-w-2xl px-4 py-16 sm:px-6 sm:py-24 lg:max-w-7xl lg:px-8">
        <div className="lg:grid lg:grid-cols-2 lg:items-start lg:gap-x-8">
          {/* Image Gallery */}
          <div className="relative aspect-square w-full overflow-hidden rounded-lg bg-gray-100 sm:col-span-1">
            {product.imageUrl ? (
              <Image
                src={product.imageUrl}
                alt={product.name}
                fill
                className="object-cover object-center"
                priority
              />
            ) : (
              <div className="flex h-full w-full items-center justify-center bg-gray-200 text-gray-400">
                No Image
              </div>
            )}
          </div>

          {/* Product Info */}
          <div className="mt-10 px-4 sm:mt-16 sm:px-0 lg:mt-0">
            <h1 className="text-3xl font-bold tracking-tight text-gray-900">
              {product.name}
            </h1>
            <div className="mt-3">
              <h2 className="sr-only">Product information</h2>
              <p className="text-3xl tracking-tight text-gray-900">
                {new Intl.NumberFormat('en-US', {
                  style: 'currency',
                  currency: product.priceCurrency,
                }).format(product.priceAmount)}
              </p>
            </div>
            <div className="mt-6">
              <h3 className="sr-only">Description</h3>
              <div className="space-y-6 text-base text-gray-700">
                <p>{product.description}</p>
              </div>
            </div>

            <div className="mt-10 flex">
              <button
                type="button"
                className="flex max-w-xs flex-1 items-center justify-center rounded-md border border-transparent bg-indigo-600 px-8 py-3 text-base font-medium text-white hover:bg-indigo-700 focus:outline-none focus:ring-2 focus:ring-indigo-500 focus:ring-offset-2 focus:ring-offset-gray-50 sm:w-full"
              >
                Add to bag
              </button>
            </div>
          </div>
        </div>
      </div>
    </div>
  );
}