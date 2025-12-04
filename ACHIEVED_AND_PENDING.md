# Achievement Summary & Pending Tasks

This document summarizes the progress made and outlines the immediate next steps, based on the last conversation.

---

## Achieved Since Last Update:

### 1. Product Management (CRUD Operations)
- [X] **Update Product (`PUT /api/products/{id}`):**
    -   Implemented a method `UpdateDetails` in the `Product` Domain entity.
    -   Created `UpdateProductRequest` and `UpdateProductService`.
    -   Exposed a `PUT` endpoint in `ProductsController` (Admin only).
- [X] **Delete Product (`DELETE /api/products/{id}`):**
    -   Created `DeleteProductService`.
    -   Implemented a `DELETE` endpoint in `ProductsController` (Admin only).

### 2. Email Service Preparation
- [X] **SmtpEmailService Structure:**
    -   Replaced the `ConsoleEmailService` with `SmtpEmailService`.
    -   The `SmtpEmailService` is now configured via `EmailSettings` from `appsettings.json`.
    -   *Note: Actual SMTP integration with MailKit is still pending; currently it logs email details to the console.*

### 3. Payment Gateway (Multi-Provider Strategy)
- [X] **Strategy Pattern Implementation:**
    -   Defined `IPaymentStrategy` interface (`ProviderName`, `ProcessPaymentAsync`).
    -   Created mock `StripePaymentStrategy` and `PayPalPaymentStrategy` implementations.
    -   Developed a central `PaymentService` that resolves the correct strategy based on the provider name.
    -   `IPaymentService` interface updated to accept the `provider` name.
- [X] **Order Service Integration:**
    -   `OrderService.PlaceOrderAsync` now accepts a `paymentProvider` parameter and uses the new `PaymentService`.
- [X] **Cleanup:** Removed the old `MockPaymentService`.

---

## Pending Tasks (from UPCOMING_FEATURES.md):

### 1. Missing CRUD Operations (Content Management)

#### A. Categories
- [ ] **Update Category (`PUT /api/categories/{id}`):**
    -   Rename categories or move them to a different parent.
- [ ] **Delete Category (`DELETE /api/categories/{id}`):**
    -   Logic to handle sub-categories and products.

#### B. Users (Profile Management)
- [ ] **Update Profile (`PUT /api/users/profile`):**
    -   Allow logged-in users to change their First/Last Name.
- [ ] **Change Password (`PUT /api/users/change-password`):**
    -   Secure endpoint requiring "Current Password" and "New Password".

### 2. Real Services Integration

#### A. Email Service (SMTP)
- [ ] **Actual MailKit Integration:** Implement the real email sending logic within `SmtpEmailService`.

#### B. Payment Gateway (Multi-Provider)
- [ ] **Stripe Integration:** Integrate with **Stripe.net** SDK for real payment processing.
- [ ] **PayPal Integration:** Integrate with **PayPal Checkout** SDK for real payment processing.

---

## Next Action:
Given the user's specific request for payments, we have the scaffolding ready.
Shall we proceed with integrating a **real Stripe payment flow** first, then PayPal? Or would you prefer to complete the **Category CRUD** operations first?
