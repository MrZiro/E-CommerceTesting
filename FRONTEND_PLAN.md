# E-Commerce Frontend Plan (Next.js 16)

## 1. Project Overview
**Goal:** leverage the platform to build a fast, interactive storefront with minimal client-side boilerplate.
**Tech Stack:** Next.js 16 (App Router), React Server Components, Tailwind CSS.

---

## 2. Frontend Strategy
*Goal: "Use the Platform". Replace complex libraries with native Next.js features.*

### Key Decisions
- **App Router:** Full adoption of the `app/` directory.
- **Server Components (RSC):** Default to Server Components.
    - Fetch data directly in components (`async/await`).
    - No `useEffect` for data fetching.
- **No "TanStack Query" Needed:**
    - **Fetching:** Use native `fetch` with Next.js caching tags.
    - **Caching:** `fetch('api/products', { next: { tags: ['products'] } })`.
    - **Revalidation:** Use `revalidateTag('products')` in Server Actions to refresh data.
- **Server Actions:** Replace API routes for data mutations.
    - Mask the backend API: `Client -> Next.js Server Action -> .NET Backend`.
- **Authentication Strategy:**
    - Store the JWT returned by .NET in an **HTTP-Only Cookie** within the Next.js server (Server Action).
    - Middleware (`middleware.ts`) will read this cookie to handle protected routes (`/admin`).
    - `api-client.ts` will automatically attach this cookie to outgoing requests to the backend.
- **Error Handling:**
    - Map .NET `ProblemDetails` responses to UI-friendly errors.
    - Use `error.tsx` boundaries for critical failures.
    - Return structured error objects `{ success: false, error: "..." }` from Server Actions for form validation.

### Folder Structure
```text
/src
  /app
    /(store)              # Public layout (Navbar, Footer)
      /page.tsx           # Home
      /products/[slug]    # Product Details
    /(checkout)           # Checkout layout (Minimal header)
    /admin                # Admin Dashboard (Protected)
  /components
    /ui                   # Atoms (Buttons, Inputs - Shadcn/UI style)
    /features             # Complex blocks (ProductCard, CartSheet)
  /lib
    api-client.ts         # Typed fetch wrapper for communicating with .NET
    utils.ts              # Helper functions
```

---

## 3. Native Data Flow & "Depbud" Concept
Instead of managing global state for server data (Redux/Query), we use the request/response lifecycle.

1.  **Read (Server Component):**
    ```tsx
    // page.tsx
    const products = await getProducts(); // Direct fetch
    return <ProductList items={products} />;
    ```

2.  **Write (Server Action):**
    ```tsx
    // actions.ts
    'use server'
    export async function addToCart(formData: FormData) {
      await fetchBackend('/cart', 'POST', data);
      revalidatePath('/cart'); // Tells Next.js to refresh the UI
    }
    ```

3.  **UI Feedback (Client Component):**
    - Use `useFormStatus` to show loading spinners during Server Actions.
    - Use `useOptimistic` to update the UI immediately (e.g., increment cart count) while the server action is pending.

---

## 4. Implementation Plan (Frontend)

### Phase 1: Foundation
1.  Initialize Next.js 16 app with Tailwind CSS.
2.  Configure Fonts and Global Styles.
3.  Create `api-client.ts`: A strongly-typed wrapper around `fetch` to handle base URLs, headers, and error parsing from the .NET backend.

### Phase 2: Product Catalog
1.  Create `ProductCard` and `ProductGrid` components.
2.  Implement **Product Listing Page**:
    - Fetch products in `page.tsx`.
    - Pass data to grid.
3.  Implement **Product Details Page**:
    - Use `generateStaticParams` to pre-render popular products.

### Phase 3: Cart & Auth
1.  Create **Login Form** using Server Actions.
2.  Create **Cart Drawer/Page**:
    - Fetch current cart state.
    - Implement "Add to Cart" button with `useOptimistic` for instant feedback.

### Phase 4: Checkout
1.  Build **Checkout Form** (Multi-step or Single page).
2.  Integrate Stripe Elements (or similar) for payment.
3.  Handle Order Success/Failure pages.
