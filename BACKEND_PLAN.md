# Production-Ready E-Commerce Backend Plan (.NET 10)

## 1. Architectural & Design Choices

### The "Clean Architecture" (Onion)
We will strictly separate concerns to ensure maintainability and testability.

1.  **Domain Layer (Inner Circle):**
    *   **What:** Entities, Value Objects, Enums, Domain Events, Repository Interfaces (only if strictly needed).
    *   **Dependencies:** None. Pure C#.
2.  **Application Layer:**
    *   **What:** Business logic (Services/Use Cases), DTOs, Interfaces (Abstractions), Validators, Mappings.
    *   **Dependencies:** Domain Layer.
3.  **Infrastructure Layer:**
    *   **What:** Database (EF Core), File System, Email Service, Stripe Wrapper, Identity.
    *   **Dependencies:** Application Layer.
4.  **API Layer (Presentation):**
    *   **What:** Controllers/Endpoints, Middleware, DI Composition.
    *   **Dependencies:** Application Layer, Infrastructure Layer.

### Key Patterns & Standards
- **Result Pattern:** Use a library like `ErrorOr` or custom `Result<T>` to handle success/failure gracefully without throwing exceptions for flow control.
- **FluentValidation:** Decouple validation logic from DTOs and Controllers.
- **Structured Logging:** Serilog + Seq/OpenTelemetry (essential for production debugging).
- **Global Exception Handling:** `IExceptionHandler` middleware (RFC 7807 Problem Details).
- **Documentation:** **Scalar UI** (`Scalar.AspNetCore`) for a beautiful, modern API reference (replacing SwaggerUI).
- **Testing (Non-Negotiable):**
    - **xUnit + FluentAssertions:** For expressive assertions.
    - **Integration Testing:** Use a **dedicated local test database**.
    - **Respawn:** Use the `Respawn` library to intelligently reset the database state between tests (faster and more reliable than dropping/recreating DBs).
    - **NSubstitute:** For mocking external services (Email, Stripe).

---

## 2. Detailed Tech Stack

- **Framework:** .NET 10
- **Database:** **Microsoft SQL Server (MSSQL)** (Local Instance)
- **ORM:** Entity Framework Core (Code First)
- **Caching:** Redis (Distributed Cache)
- **Object Mapping:** Mapster (Performance superior to AutoMapper)
- **Validation:** FluentValidation
- **Documentation:** **Scalar UI**
- **Auth:** JWT (JSON Web Tokens) with Refresh Token rotation.

---

## 3. Project Structure (Solution View)

```text
MyCommerce.sln
├── src
│   ├── MyCommerce.Domain         # The Core
│   │   ├── Entities              # e.g., Product, Order, User
│   │   ├── ValueObjects          # e.g., Money, Address, SKU
│   │   ├── Errors                # Domain Errors (static classes)
│   │   └── Events                # Domain Events
│   │
│   ├── MyCommerce.Application    # The Business Logic
│   │   ├── Common                # Interfaces (IEmailService, IDateTimeProvider)
│   │   ├── Products              # Feature Folders
│   │   │   ├── Create            # CreateProductCommand + Handler + Validator
│   │   │   ├── GetById           # GetProductQuery + Handler
│   │   │   └── Dtos
│   │   └── Orders
│   │
│   ├── MyCommerce.Infrastructure # The Implementation
│   │   ├── Persistence           # DbContext, Configurations (Entity config)
│   │   ├── Authentication        # JwtTokenGenerator
│   │   └── Services              # SystemDateTimeProvider, EmailService
│   │
│   └── MyCommerce.Api            # The Entry Point
│       ├── Controllers
│       ├── Middleware
│       └── Extensions            # DI Setup
│
└── tests
    ├── MyCommerce.UnitTests      # Domain logic tests
    └── MyCommerce.IntegrationTests # API endpoint tests (using Local DB + Respawn)
```

---

## 4. Implementation Steps

### Phase 1: Core Infrastructure & Setup
1.  **Solution Setup:** Create the 4 projects (`Domain`, `Application`, `Infrastructure`, `Api`).
2.  **Strict Mode:** Enable `<Nullable>enable</Nullable>` and `<TreatWarningsAsErrors>true</TreatWarningsAsErrors>` in `Directory.Build.props`.
3.  **Logging:** Configure Serilog to write to Console and File (or Seq).
4.  **API Shell:** Setup **Scalar UI**, Global Exception Handler, and CORS.

### Phase 2: Domain Modeling (The "Heart")
1.  **Base Entity:** Create `Entity` and `AggregateRoot` base classes (handling ID equality and Domain Events).
2.  **Value Objects:** Create `Money`, `Sku`, `Email` value objects to prevent "primitive obsession".
    *   *Example:* Instead of `decimal price`, use `Money Price`.
3.  **Entities:** Implement `Product`, `Category`, `User`, `Order`.
    *   **Rule:** Private setters. Use factory methods (e.g., `Product.Create(...)`) to enforce invariants.

### Phase 3: Persistence & Data Access
1.  **EF Configuration:** Use `IEntityTypeConfiguration<T>` classes for every entity (keep `DbContext` clean).
2.  **Connection:** Configure `appsettings.json` to point to local MSSQL instance.
3.  **Interceptors:** Add `AuditableEntityInterceptor` to automatically set `CreatedAt`/`UpdatedAt`.
4.  **Migrations:** Setup initial **MSSQL** migration.

### Phase 4: Vertical Slices (Features)
*Instead of massive Services (`ProductService`), we will use focused requests. (MediatR is optional, but the pattern is vital).*

**Feature A: Create Product**
1.  **DTO:** `CreateProductRequest` (record).
2.  **Validator:** `CreateProductValidator` (FluentValidation).
3.  **Service Method/Handler:**
    *   Validate Input.
    *   Create Domain Entity (`Product.Create(...)`).
    *   Save to DB.
    *   Return `Result<Guid>` (Product ID).
4.  **Controller:** Call Service -> Match Result (Success -> 201, Failure -> 400).

**Feature B: Authentication**
1.  **Identity:** Use ASP.NET Core Identity or custom User table (recommended for full control).
2.  **JWT:** Implement `JwtProvider` to generate tokens with Claims (Role, Id).

### Phase 5: Reliability & Performance
1.  **Caching:** Implement `ICacheService` (Redis). Use Cached-Aside pattern for Product details.
2.  **Background Jobs:** Setup `Quartz.NET` or `Hangfire` (if needed for email sending or order processing).
3.  **Health Checks:** Add DB (Local MSSQL) and Redis health checks.

---

## 5. Why This is "Professional"
- **Separation of Concerns:** UI doesn't know about DB.
- **Testability:** Everything in `Application` is pure C# and easily unit tested.
- **Maintainability:** Changing the DB schema or switching email providers only affects `Infrastructure`.
- **No Bloat:** We avoid Generic Repositories (`IRepository<T>`) which hide useful EF Core features. We use `DbContext` specifically where it shines, encapsulated within the Application layer.