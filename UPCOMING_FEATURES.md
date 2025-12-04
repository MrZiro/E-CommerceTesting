# Upcoming Features & Enhancements Plan

Based on the current codebase analysis, here are the missing functional requirements and enhancements needed to make the system fully operational and professional.

## 1. Missing CRUD Operations (Content Management)
Currently, Admins can create content, but they cannot edit or remove it.

### A. Products
- [ ] **Update Product (`PUT /api/products/{id}`):**
    -   Allow Admins to update Name, Description, Price, Stock, and ImageUrl.
- [ ] **Delete Product (`DELETE /api/products/{id}`):**
    -   **Soft Delete:** Instead of physically removing the row (which breaks historic Orders), set a `IsDeleted` flag.
    -   **Hard Delete:** Only allowed if the product has no associated Orders.

### B. Categories
- [ ] **Update Category (`PUT /api/categories/{id}`):**
    -   Rename categories or move them to a different parent (hierarchy change).
- [ ] **Delete Category (`DELETE /api/categories/{id}`):**
    -   Logic to handle sub-categories and products inside the deleted category (e.g., prevent delete if not empty).

### C. Users (Profile Management)
- [ ] **Update Profile (`PUT /api/users/profile`):**
    -   Allow logged-in users to change their First/Last Name.
- [ ] **Change Password (`PUT /api/users/change-password`):**
    -   Secure endpoint requiring "Current Password" and "New Password".

---

## 2. Real Services Implementation

### A. Email Service (SMTP)
*Current Status: Console Logger*
- [ ] **Implementation:** Replace `ConsoleEmailService` with `SmtpEmailService` using **MailKit**.
- [ ] **Features:**
    -   Send real HTML emails for "Welcome", "Password Reset", and "Order Confirmation".
    -   Configuration in `appsettings.json` (Host, Port, Username, Password).

### B. Payment Gateway (Multi-Provider)
*Current Status: Mock Service*
- [ ] **Architecture:** Implement the **Strategy Pattern** to switch between providers dynamically based on user selection.
    -   `IPaymentStrategy` interface.
    -   `StripePaymentStrategy` implementation.
    -   `PayPalPaymentStrategy` implementation.
- [ ] **Stripe Integration:**
    -   Use **Stripe.net** SDK.
    -   Implement PaymentIntent flow (Secure, SCA ready).
- [ ] **PayPal Integration:**
    -   Use **PayPal Checkout** SDK (REST API).
    -   Implement Order Capture flow.
- [ ] **Checkout API Update:**
    -   Update `POST /checkout` to accept `paymentProvider` ("stripe" or "paypal") and necessary tokens/IDs.

---

## 3. Order Management Enhancements
- [ ] **Cancel Order (`PUT /api/orders/{id}/cancel`):**
    -   Allow users to cancel "Pending" orders.
    -   Must restore Product Stock automatically.
- [ ] **Order Notes:**
    -   Add a field for users to leave notes (e.g., "Leave at front door").

---

## 4. Roadmap
1.  **Complete CRUD:**
    - [X] **Products:** Update/Delete implemented.
    - [ ] **Categories:** Need Update/Delete.
2.  **Email:**
    - [X] **Integration:** `SmtpEmailService` structure created and registered.
3.  **Payments:** Integrate Stripe/PayPal.
