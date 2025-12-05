import Image from 'next/image';
import Link from 'next/link';
import { Product } from '@/types';

interface ProductCardProps {
  product: Product;
}

export function ProductCard({ product }: ProductCardProps) {
  return (
    <Link href={`/products/${product.id}`} className="group block space-y-2">
      <div className="relative aspect-square overflow-hidden rounded-lg bg-gray-100">
        {product.imageUrl ? (
           <Image
             src={product.imageUrl}
             alt={product.name}
             fill
             className="object-cover object-center transition-transform duration-300 group-hover:scale-105"
             sizes="(min-width: 1024px) 25vw, (min-width: 768px) 33vw, 50vw"
           />
        ) : (
          <div className="flex h-full w-full items-center justify-center bg-gray-200 text-gray-400">
            No Image
          </div>
        )}
      </div>
      <div className="flex justify-between">
        <div>
          <h3 className="text-sm font-medium text-gray-900 group-hover:underline">
            {product.name}
          </h3>
          <p className="text-sm text-gray-500">{product.sku}</p>
        </div>
        <p className="text-sm font-medium text-gray-900">
          {new Intl.NumberFormat('en-US', {
            style: 'currency',
            currency: product.priceCurrency,
          }).format(product.priceAmount)}
        </p>
      </div>
    </Link>
  );
}
