# UML and Architecture Documentation

## 1. Scope

This document describes the current ECommerce system as implemented by the ASP.NET Core API, the Angular client, and the SQLite data store. The diagrams use Mermaid syntax so they can be rendered in GitHub, Visual Studio extensions, Mermaid Live, or other compatible Markdown viewers.

## 2. System context

```mermaid
flowchart TB
	Customer[Customer using browser] --> Client[Angular client application]
	Client -->|HTTP JSON| API[ASP.NET Core Web API]
	API --> Database[(SQLite database)]
	API --> Images[Static product images]
	API --> Swagger[Swagger UI in development]
```

### Main responsibilities

| Subsystem | Responsibilities |
|---|---|
| Angular client | Navigation, product search/filtering, pagination, product details, local cart, contact form |
| ASP.NET Core API | Product catalog endpoints, filtering, pagination, mapping, errors, CORS, migrations, seeding |
| Core project | Domain entities, repository/specification interfaces, shared business abstractions |
| Infrastructure project | EF Core `DataContext`, repositories, specifications, migrations, seed data |
| SQLite | Product, brand, and type persistence |

## 3. Actors and use cases

```mermaid
flowchart LR
	Customer((Customer))
	Developer((Developer))

	subgraph Storefront[Storefront]
		Browse[Browse products]
		Search[Search products]
		Filter[Filter by brand/type]
		Sort[Sort products]
		ViewDetails[View product details]
		Cart[Manage shopping cart]
		Contact[Submit contact form]
	end

	subgraph Operations[Development operations]
		RunApi[Run API]
		RunClient[Run Angular client]
		Migrate[Apply database migrations]
		Seed[Seed catalog data]
		Swagger[Inspect API with Swagger]
	end

	Customer --> Browse
	Customer --> Search
	Customer --> Filter
	Customer --> Sort
	Customer --> ViewDetails
	Customer --> Cart
	Customer --> Contact

	Developer --> RunApi
	Developer --> RunClient
	Developer --> Migrate
	Developer --> Seed
	Developer --> Swagger
```

## 4. Angular component/module structure

```mermaid
flowchart TD
	AppModule --> AppComponent
	AppModule --> AppRoutingModule
	AppModule --> CoreModule
	AppModule --> HomeModule
	AppModule --> ShopModule
	AppModule --> CartComponent
	AppModule --> ContactComponent

	CoreModule --> NavBarComponent
	NavBarComponent --> CartService

	HomeModule --> HomeComponent
	HomeComponent --> ShopService

	ShopModule --> ShopComponent
	ShopModule --> ProductItemComponent
	ShopModule --> ProductDetailsComponent
	ShopModule --> SharedModule
	ShopComponent --> ShopService
	ProductDetailsComponent --> ShopService
	ProductItemComponent --> CartService
	ProductDetailsComponent --> CartService

	CartComponent --> CartService
	CartService --> LocalStorage[(Browser localStorage)]
	SharedModule --> PagingHeaderComponent
	SharedModule --> PagerComponent
```

## 5. Angular route diagram

```mermaid
flowchart LR
	Root[Browser root] --> Home[/]
	Root --> Shop[/shop]
	Root --> Details[/shop/:id]
	Root --> Cart[/cart]
	Root --> Contact[/contact]
	Root --> Fallback[Unknown route]
	Fallback --> Home
```

| Route | Component | Purpose |
|---|---|---|
| `/` | `HomeComponent` | Hero section and featured products |
| `/shop` | `ShopComponent` | Product catalog, filtering, sorting, search, pagination |
| `/shop/:id` | `ProductDetailsComponent` | Product details and quantity selection |
| `/cart` | `CartComponent` | Cart contents, quantities, totals, and removal |
| `/contact` | `ContactComponent` | Client-side contact form presentation |
| `**` | Redirect to `/` | Fallback for unknown routes |

## 6. Domain class diagram

```mermaid
classDiagram
	class BaseEntity {
		+int Id
	}

	class Product {
		+int Id
		+string Name
		+string Description
		+decimal Price
		+string PictureUrl
		+int ProductTypeId
		+int ProductBrandId
		+ProductType ProductType
		+ProductBrand ProductBrand
	}

	class ProductBrand {
		+int Id
		+string Name
	}

	class ProductType {
		+int Id
		+string Name
	}

	class ProductSpecParams {
		+int BrandId
		+int TypeId
		+string Sort
		+int PageIndex
		+int PageSize
		+string Search
	}

	class Pagination~T~ {
		+int PageIndex
		+int PageSize
		+int Count
		+T Data
	}

	BaseEntity <|-- Product
	BaseEntity <|-- ProductBrand
	BaseEntity <|-- ProductType
	ProductBrand "1" --> "0..*" Product : classifies
	ProductType "1" --> "0..*" Product : categorizes
	ProductSpecParams ..> Product : filters
	Pagination ..> Product : returns collection
```

## 7. Client-side cart class diagram

```mermaid
classDiagram
	class Product {
		+number id
		+string name
		+number price
		+string pictureUrl
	}

	class CartItem {
		+Product product
		+number quantity
	}

	class CartService {
		-BehaviorSubject~CartItem[]~ itemsSubject
		+Observable items$
		+addItem(product, quantity)
		+updateQuantity(productId, quantity)
		+removeItem(productId)
		+clear()
		+getItemCount(items)
		+getTotal(items)
	}

	class NavBarComponent {
		+Observable cartItemCount$
	}

	class ProductItemComponent {
		+Product product
		+addToCart()
	}

	class ProductDetailsComponent {
		+Product product
		+number quantity
		+addToCart()
	}

	class CartComponent {
		+CartItem[] items
		+number total
		+updateQuantity(item, event)
		+removeItem(productId)
		+clearCart()
	}

	CartItem --> Product
	NavBarComponent --> CartService
	ProductItemComponent --> CartService
	ProductDetailsComponent --> CartService
	CartComponent --> CartService
	CartService ..> CartItem : stores
```

## 8. Product browsing sequence

```mermaid
sequenceDiagram
	actor Customer
	participant Browser as Angular ShopComponent
	participant Service as ShopService
	participant API as ProductsController
	participant Repo as GenericRepository
	participant DB as SQLite

	Customer->>Browser: Open /shop
	Browser->>Service: getProducts(ShopParams)
	Service->>API: GET /api/products?sort=name&pageIndex=1&pageSize=6
	API->>Repo: List products with specifications
	Repo->>DB: Execute filtered query
	DB-->>Repo: Product rows
	Repo-->>API: Products and count
	API-->>Service: Pagination<Product[]>
	Service-->>Browser: Product response
	Browser-->>Customer: Render product cards and pager

	Customer->>Browser: Select brand or type
	Browser->>Service: getProducts(updated ShopParams)
	Service->>API: GET /api/products with filter
	API-->>Service: Filtered pagination response
	Service-->>Browser: Updated products
	Browser-->>Customer: Render filtered list
```

## 9. Add-to-cart sequence

```mermaid
sequenceDiagram
	actor Customer
	participant Card as ProductItemComponent
	participant Cart as CartService
	participant Storage as Browser localStorage
	participant Nav as NavBarComponent

	Customer->>Card: Click Add to cart
	Card->>Cart: addItem(product, 1)
	Cart->>Cart: Merge quantity or create CartItem
	Cart->>Storage: Save serialized cart
	Cart-->>Card: Publish updated items
	Cart-->>Nav: Publish updated items
	Nav-->>Customer: Update cart badge
```

## 10. Product details sequence

```mermaid
sequenceDiagram
	actor Customer
	participant Browser as ProductDetailsComponent
	participant Route as ActivatedRoute
	participant Service as ShopService
	participant API as ProductsController
	participant DB as SQLite

	Customer->>Browser: Open /shop/:id
	Browser->>Route: Read id parameter
	Browser->>Service: getProduct(id)
	Service->>API: GET /api/products/{id}
	API->>DB: Query product with brand and type
	DB-->>API: Product or no result
	alt Product exists
		API-->>Service: Product DTO
		Service-->>Browser: Product data
		Browser-->>Customer: Render detail page
		Customer->>Browser: Select quantity and add to cart
	else Product does not exist
		API-->>Service: 404 response
		Service-->>Browser: Error
		Browser-->>Customer: Show not-found message
	end
```

## 11. API layering

```mermaid
flowchart TD
	HTTP[HTTP request] --> Controller[API Controllers]
	Controller --> DTO[DTOs and AutoMapper]
	Controller --> Spec[Specifications]
	Spec --> Repository[Generic repository]
	Repository --> Context[DataContext]
	Context --> EF[Entity Framework Core]
	EF --> SQLite[(SQLite)]

	Middleware[ExceptionMiddleware] -. wraps .-> HTTP
	ErrorController[ErrorController] --> Middleware
```

## 12. Design notes and current limitations

- The cart is client-side only and persists in browser local storage.
- Login, registration, authorization, checkout, payments, orders, and server-side carts are not implemented.
- The contact form currently displays a success state locally; it does not submit to an API endpoint.
- Swagger is enabled only in the ASP.NET development environment.
- Product catalog data is the primary persisted domain currently represented by the database.
- The diagrams describe the current implementation and should be updated whenever routes, domain entities, or API contracts change.
