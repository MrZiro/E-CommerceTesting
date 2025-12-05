export default function CheckoutPage() {
  return (
    <div className="min-h-screen bg-white px-4 py-16 sm:px-6 lg:px-8">
      <div className="mx-auto max-w-2xl">
        <div className="text-center">
            <div className="mx-auto flex h-12 w-12 items-center justify-center rounded-full bg-green-100">
                <svg className="h-6 w-6 text-green-600" fill="none" viewBox="0 0 24 24" strokeWidth="1.5" stroke="currentColor" aria-hidden="true">
                    <path strokeLinecap="round" strokeLinejoin="round" d="M4.5 12.75l6 6 9-13.5" />
                </svg>
            </div>
            <h2 className="mt-4 text-3xl font-bold tracking-tight text-gray-900 sm:text-4xl">Checkout Prototype</h2>
            <p className="mt-2 text-lg text-gray-500">
                The backend payment service is currently a mock. 
            </p>
            <p className="mt-2 text-sm text-gray-400">
                (Real implementation requires Stripe/Payment Gateway integration which is planned for Phase 4)
            </p>
            <div className="mt-10 flex justify-center">
                <a href="/" className="text-sm font-semibold leading-6 text-indigo-600">
                    <span aria-hidden="true">&larr;</span> Back to store
                </a>
            </div>
        </div>
      </div>
    </div>
  );
}
