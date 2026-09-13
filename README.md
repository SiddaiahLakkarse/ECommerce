# ECommerce

A full-stack e-commerce catalog application built with ASP.NET Core Web API, Entity Framework Core, SQLite, and Angular. The project provides product browsing, search, sorting, brand/type filtering, pagination, product details, a client-side shopping cart, and a contact page.

## Contents

- [Project overview](#project-overview)
- [Features](#features)
- [Technology stack](#technology-stack)
- [Repository structure](#repository-structure)
- [Prerequisites](#prerequisites)
- [Installation](#installation)
- [Running the application](#running-the-application)
- [Application routes](#application-routes)
- [API endpoints](#api-endpoints)
- [Database](#database)
- [Configuration](#configuration)
- [Testing and validation](#testing-and-validation)
- [Troubleshooting](#troubleshooting)
- [Documentation](#documentation)
- [Current limitations](#current-limitations)
- [Recommended future work](#recommended-future-work)

## Project overview

The application is split into two independently runnable applications:

1. **API**: An ASP.NET Core 7 Web API that exposes the product catalog and manages the SQLite database.
2. **Client**: An Angular 15 single-page application that consumes the API and provides the storefront experience.

The API applies EF Core migrations and seeds catalog data when it starts. The Angular client calls the API at `https://localhost:5001/api/` by default.

## Features

### Storefront

- Responsive home page with hero section and featured products.
- Product catalog with responsive product cards.
- Product search using the Enter key or Search button.
- Sorting by name, price ascending, or price descending.
- Filtering by product brand and product type.
- Pagination for product results.
- Product detail page with description, category, price, and image.
- Quantity controls on the product detail page.

### Shopping cart

- Add products from product cards or the product detail page.
- Increase or decrease quantities.
- Remove individual items.
- Clear the complete cart.
- Calculate item count and total price.
- Persist cart data in browser `localStorage`.

### User experience

- Loading indicators during API requests.
- Retryable API error messages.
- Empty-result messages for product searches and filters.
- Product not-found state.
- Responsive Bootstrap layout.
- Contact form with client-side validation and confirmation state.

### API and data

- Product catalog endpoints.
- Brand and type lookup endpoints.
- Filtering, sorting, searching, and pagination through query parameters.
- Repository and specification-based data access.
- AutoMapper DTO mapping.
- Global exception middleware and standardized API error responses.
- Swagger UI in the development environment.
- Automatic database migration and seed execution at API startup.

## Technology stack

| Layer | Technology |
|---|---|
| Backend framework | ASP.NET Core 7 Web API |
| Language | C# |
| ORM | Entity Framework Core 7 |
| Database | SQLite |
| API documentation | Swagger / Swashbuckle |
| Object mapping | AutoMapper |
| Frontend framework | Angular 15 |
| Frontend language | TypeScript |
| UI | Bootstrap 5, SCSS |
| Reactive programming | RxJS |
| Client pagination | ngx-bootstrap |
| Icons | Font Awesome |

## Repository structure

```text
ECommerce/
├── API/
│   ├── Controllers/          HTTP API controllers
│   ├── Errors/               API error response models
│   ├── Helpers/              AutoMapper and URL helpers
│   ├── Middleware/           Exception middleware
│   ├── ECommerce.db          SQLite development database
│   ├── Program.cs            API startup and migration/seed execution
│   └── appsettings.json      API configuration
├── Core/
│   ├── Entities/             Product, brand, type, and base entities
│   ├── Interfaces/           Repository contracts
│   └── Specifications/       Query specifications
├── Infrastructure/
│   └── Data/
│       ├── Migrations/       EF Core migrations
│       ├── SeedData/         JSON seed files
│       ├── DataContext.cs    EF Core DbContext
│       └── GenericRepository.cs
├── clientApp/
│   ├── src/app/core/         Shared navigation and cart services
│   ├── src/app/home/         Home page
│   ├── src/app/shop/         Product catalog and product details
│   ├── src/app/cart/         Cart page
│   ├── src/app/contact/      Contact page
│   ├── src/app/shared/       Shared models and pagination components
│   ├── angular.json          Angular CLI configuration
│   └── package.json          Frontend dependencies and scripts
├── docs/
│   ├── DATABASE.md           Database design and operations
│   └── UML.md                UML and architecture diagrams
└── ECommerce.sln             Visual Studio solution
```

## Prerequisites

Install the following tools:

- .NET SDK 7.0 or later with .NET 7 targeting-pack support.
- Node.js compatible with Angular CLI 15. Node.js 16.14+ or 18.x is recommended for this project.
- npm.
- Git.
- Optional: Visual Studio with ASP.NET and web development workload, or Visual Studio Code.
- Optional: .NET EF Core CLI tools for migration commands.

Install EF Core CLI tools if they are not already available:

```powershell
dotnet tool install --global dotnet-ef --version 7.*
```

Verify installations:

```powershell
dotnet --version
node --version
npm --version
dotnet ef --version
```

## Installation

Clone the repository and enter the project directory:

```powershell
git clone https://github.com/SiddaiahLakkarse/ECommerce.git
Set-Location ECommerce
```

Restore backend dependencies:

```powershell
dotnet restore ECommerce.sln
```

Install client dependencies:

```powershell
Set-Location clientApp
npm install
Set-Location ..
```

If the existing legacy lockfile prevents installation, see [Troubleshooting](#troubleshooting).

## Running the application

The API and Angular client run as separate processes.

### Start the API

From the repository root:

```powershell
dotnet run --project API/API.csproj
```

The configured development URL is:

```text
https://localhost:5001
```

On startup, the API:

1. Registers application services.
2. Applies pending EF Core migrations.
3. Seeds catalog data.
4. Starts the HTTP API.

### Start the Angular client

Open a second terminal:

```powershell
Set-Location clientApp
npm start
```

Open the client at:

```text
http://localhost:4200
```

The frontend service currently uses:

```text
https://localhost:5001/api/
```

If the API certificate is not trusted locally, trust the ASP.NET development certificate:

```powershell
dotnet dev-certs https --trust
```

### Run from Visual Studio

1. Open `ECommerce.sln`.
2. Set `API` as the startup project.
3. Start the API with the HTTPS profile.
4. In a terminal, run `npm start` from `clientApp`.
5. Browse to `http://localhost:4200`.

## Application routes

| Route | Description |
|---|---|
| `/` | Home page and featured products |
| `/shop` | Product catalog, search, filters, sorting, and pagination |
| `/shop/:id` | Product details and add-to-cart controls |
| `/cart` | Shopping cart contents and totals |
| `/contact` | Contact form |

Unknown frontend routes redirect to `/`.

## API endpoints

The API base path is `/api`.

| Method | Endpoint | Description |
|---|---|---|
| `GET` | `/api/products` | Get a paginated product list |
| `GET` | `/api/products/{id}` | Get one product by ID |
| `GET` | `/api/products/brands` | Get product brands |
| `GET` | `/api/products/types` | Get product types |
| `GET` | `/swagger` | Open Swagger UI in development |

### Product list query parameters

| Parameter | Example | Description |
|---|---|---|
| `brandId` | `1` | Filter by brand ID |
| `typeId` | `2` | Filter by type ID |
| `sort` | `priceAsc` | Sort by `name`, `priceAsc`, or `priceDesc` |
| `pageIndex` | `1` | One-based page number |
| `pageSize` | `6` | Number of records per page |
| `search` | `phone` | Search product names and descriptions according to the API specification |

Example request:

```text
https://localhost:5001/api/products?sort=priceAsc&pageIndex=1&pageSize=6&brandId=1
```

## Database

The application uses SQLite with the database file `API/ECommerce.db`. The primary tables are:

- `Products`
- `ProductBrands`
- `ProductTypes`

Products reference one brand and one type. EF Core migrations are stored under `Infrastructure/Data/Migrations`.

### Migration commands

```powershell
# List migrations
dotnet ef migrations list --project Infrastructure --startup-project API

# Apply migrations
dotnet ef database update --project Infrastructure --startup-project API

# Create a new migration
dotnet ef migrations add MigrationName --project Infrastructure --startup-project API
```

For the complete schema, relationship, backup, and maintenance documentation, see [docs/DATABASE.md](docs/DATABASE.md).

## Configuration

### API configuration

Main API settings are in `API/appsettings.json`:

```json
{
  "ConnectionStrings": {
	"DefaultConnection": "Data source=ECommerce.db"
  },
  "ApiUrl": "http://localhost:4200/assets/Content/"
}
```

Use `API/appsettings.Development.json` for development-only overrides. Do not commit secrets or production credentials.

### Frontend API URL

The current API URL is defined in:

```text
clientApp/src/app/shop/shop.service.ts
```

For multiple environments, this should be moved to Angular environment files before deployment.

## Testing and validation

### Backend build

```powershell
dotnet build ECommerce.sln
```

### Frontend build

```powershell
Set-Location clientApp
npm run build
```

### Frontend unit tests

```powershell
Set-Location clientApp
npm test
```

The project uses Karma and Jasmine for Angular unit tests. Browser-based end-to-end testing is not currently configured.

### Recommended manual checks

1. Start the API and confirm Swagger loads.
2. Start the Angular client and open the home page.
3. Search for a product.
4. Select a brand and verify the product list refreshes.
5. Select a type and verify the product list refreshes.
6. Open a product detail page.
7. Add a product to the cart and change its quantity.
8. Refresh the browser and verify the cart persists.
9. Remove an item and clear the cart.
10. Submit the contact form.

## Troubleshooting

### `npm install` fails with unavailable legacy package versions

The repository's existing `clientApp/package-lock.json` was generated by an older npm version and may reference unavailable transitive versions such as:

- `string-width-cjs@4.2.3`
- `wrap-ansi-cjs@7.0.0`
- `strip-ansi-cjs@6.0.1`

For a local development installation, back up the lockfile and regenerate it from `package.json`:

```powershell
Set-Location clientApp
Copy-Item package-lock.json package-lock.legacy.json
Remove-Item package-lock.json
npm install
```

Review the generated dependency changes before committing them. Do not use this procedure automatically in a production build without reviewing lockfile reproducibility.

### Angular cannot connect to the API

Check the following:

- The API is running on `https://localhost:5001`.
- The frontend URL in `shop.service.ts` matches the API URL.
- The ASP.NET HTTPS development certificate is trusted.
- The browser console does not report a CORS or certificate error.

### The database is locked

Stop all running API processes before copying, deleting, or manually inspecting `API/ECommerce.db`. SQLite may create `-shm` and `-wal` files while the application is running.

### Products do not appear

Check that:

- The API started successfully.
- Database migrations completed.
- Seed data exists.
- The browser can reach the API endpoint.
- The API response is not returning an empty filtered result.

## Documentation

- [Database documentation](docs/DATABASE.md)
- [UML and architecture diagrams](docs/UML.md)
- [Angular client guide](clientApp/README.md)

## Current limitations

The following capabilities are not currently implemented:

- User registration and login.
- Authentication and authorization.
- Server-side shopping carts.
- Checkout and payment processing.
- Order creation and order history.
- Inventory tracking.
- A server-side contact form submission endpoint.
- Production deployment configuration.
- Automated end-to-end tests.

The cart currently exists only in the browser and is not associated with a user account.

## Recommended future work

1. Add ASP.NET Identity and authenticated user accounts.
2. Add server-side carts and order tables.
3. Implement checkout and payment provider integration.
4. Add inventory and stock validation.
5. Add Angular environment configuration for development, staging, and production.
6. Add API and Angular unit tests for filters, pagination, details, and cart behavior.
7. Add end-to-end tests with Playwright or Cypress.
8. Add structured logging, health checks, and deployment configuration.
9. Add CI validation for backend builds, frontend builds, tests, and database migrations.
10. Upgrade the legacy Angular and .NET dependencies under a separate, tested migration effort.

## License

No license file is currently included in the repository. Add a `LICENSE` file before distributing the project publicly or using it as a dependency.
