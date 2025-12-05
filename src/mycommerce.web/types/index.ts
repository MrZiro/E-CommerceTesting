export interface Product {
  id: string;
  name: string;
  description: string;
  priceAmount: number;
  priceCurrency: string;
  sku: string;
  stock: number;
  categoryId: string;
  imageUrl?: string | null;
  createdOnUtc: string;
  updatedOnUtc?: string | null;
}

export interface Category {
  id: string;
  name: string;
  parentId?: string | null;
  createdOnUtc: string;
  updatedOnUtc?: string | null;
}

export interface CartItem {
  productId: string;
  productName: string;
  productImageUrl: string;
  unitPrice: number;
  currency: string;
  quantity: number;
  totalPrice: number;
}

export interface Cart {
  id: string;
  userId: string;
  items: CartItem[];
  totalAmount: number;
  currency: string;
}

export interface User {
  id: string;
  firstName: string;
  lastName: string;
  email: string;
  token: string;
}

export interface ApiError {
  title: string;
  status: number;
  detail?: string;
  errors?: Record<string, string[]>;
}
