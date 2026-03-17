# Void Ledger

Void Ledger is a sci-fi trading API built with ASP.NET Core and Azure SQL.

I used this as my main backend project to get showcase EF Core persistence, service-layer design, and deployment.

The API models a simple trading account where you can set prices, deposit cash, buy and sell commodities, inspect holdings, and view portfolio/reporting data through HTTP endpoints.

## Live Demo

Swagger UI:  
https://voidledger-api-rifh.azurewebsites.net/swagger/index.html

This is a **public shared demo instance**, so the data is shared between visitors and may change between sessions.

When testing the API, it helps to use a unique commodity name such as `DEMO_ORE_123`.

---

## Screenshots

### Swagger UI overview
<img src="docs/images/swagger-overview.png" alt="Swagger UI overview" width="400" />

### Price movement example
<img src="docs/images/price-movement.png" alt="GET /prices/{name} showing previous price, change amount, and direction" width="300" />

### Portfolio valuation
<img src="docs/images/portfolio-valuation.png" alt="GET /portfolio/valuation structured JSON response" width="300" />

---

## What the API Covers

Void Ledger supports:

- setting commodity prices
- depositing cash
- buying commodities
- selling commodities
- viewing holdings
- viewing portfolio valuation
- viewing recent actions and totals
- viewing price movement metadata

To make prices a little more interesting than a single number, the price endpoints also expose:

- current price
- previous price
- updated-at UTC timestamp
- derived change amount
- derived direction (`Up`, `Down`, `Flat`, `Unknown`)

---

## Suggested Demo Flow

A user can try the API in this order:

1. `POST /prices` with a new commodity name
2. `POST /prices` again with a different price
3. `GET /prices/{name}` to see `previousPrice`, `changeAmount`, and `direction`
4. `POST /deposit`
5. `POST /trade/buy`
6. `POST /trade/sell`
7. `GET /portfolio/valuation`
8. `GET /actions/recent?take=10`
9. `GET /reports/totals`

---

## Main Endpoints

### Prices
- `POST /prices`
- `GET /prices`
- `GET /prices/{name}`

### Trading
- `POST /deposit`
- `POST /trade/buy`
- `POST /trade/sell`

### Portfolio
- `GET /portfolio`  
  Legacy text report
- `GET /portfolio/valuation`  
  Structured portfolio response

### Actions / Reports
- `GET /actions/recent?take=10`
- `GET /reports/totals`
- `GET /reports/actions/by-type?type=Buy&take=10`

---

## Example Responses

### `GET /prices/{name}`

```json
{
  "name": "DEMO_ORE",
  "price": 8,
  "previousPrice": 5,
  "updatedAtUtc": "2026-03-12T19:20:48.6114274",
  "changeAmount": 3,
  "direction": "Up"
}
```

### `GET /portfolio/valuation`

```json
{
  "positions": [
    {
      "name": "DEMO_ORE",
      "quantity": 3,
      "currentPrice": 8,
      "positionValue": 24
    }
  ],
  "cashBalance": 276,
  "totalPortfolioValue": 24,
  "totalAccountValue": 300
}
```

### `GET /reports/totals`

```json
{
  "actionCount": 5,
  "totalDeposited": 300,
  "totalSpentOnBuys": 32,
  "totalEarnedFromSells": 8,
  "netCashflow": 276
}
```

---

## Tech Stack

- C# / .NET 8
- ASP.NET Core Web API
- Entity Framework Core
- Azure SQL
- Azure App Service
- xUnit
- GitHub Actions
- Docker

---

## Solution Structure

The solution is split into three projects:

### `VoidLedger.Api`
Handles the HTTP layer and runtime setup.

Responsibilities:
- controllers
- request/response DTOs
- EF Core DbContext, entities, and migrations
- HTTP result mapping
- dependency injection
- deployment/runtime configuration

### `VoidLedger.Core`
Contains the application logic.

Responsibilities:
- service layer
- result contracts
- business rules
- store abstractions
- trading orchestration
- read models

### `VoidLedger.Core.Tests`
Covers the service layer with automated tests.

Responsibilities:
- unit tests
- fake store/test support
- regression coverage for core flows

---

## Design Notes

### SQL as the source of truth
The runtime does not depend on in-memory state. Prices, holdings, balances, and action logs are persisted in Azure SQL.

### Thin controllers
Controllers delegate behavior to services and stay focused on HTTP concerns.

### Consistent failure handling
Expected business failures are returned through `OpResult + ErrorCode` and mapped to HTTP responses consistently.

### Additive API evolution
Older text-style endpoints were kept where still useful, while newer structured endpoints were added for cleaner machine-readable responses.

---

## Running Locally

### 1. Restore and build

```bash
dotnet restore
dotnet build
```

### 2. Run the API

```bash
dotnet run --project VoidLedger.Api
```

### 3. Open Swagger

Open the local Swagger URL shown by ASP.NET Core at startup.

---

## Local Configuration

The project uses this connection string name:

`ConnectionStrings:VoidLedgerDb`

For local development, secrets should stay outside the repo, for example with User Secrets.

The live deployment uses Azure App Service configuration for environment-specific settings.

---

## Running Tests

```bash
dotnet test
```

The test suite covers backend behavior such as:

- valid and invalid price setting
- valid and invalid deposits
- buy/sell success and failure paths
- missing price / missing holding / oversell
- invalid name handling
- portfolio valuation behavior
- reporting behavior

---

## Running with Docker

Build the image:

```bash
docker build -t voidledger-api:dev .
```

Run the container:

```bash
docker run --rm --name voidledger-api-dev -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development voidledger-api:dev
```

Then open Swagger through the containerized app.

---

## Deployment

The API is deployed to **Azure App Service** and uses **Azure SQL** for persistence.

Live Swagger UI:  
https://voidledger-api-rifh.azurewebsites.net/swagger/index.html

Deployment work in this project included:

- Dockerizing the API
- publishing to Azure App Service
- configuring Azure settings and connection strings
- applying EF Core migrations to Azure SQL
- verifying persisted behavior across restarts and redeploys

---

## Scope

This project is intentionally scoped as a backend portfolio project.

### Included
- single-account trading flow
- persisted prices, holdings, actions, and balance
- reporting endpoints
- price movement metadata
- live Azure deployment

### Not included
- authentication / authorization
- multi-user account isolation
- frontend client
- background market simulation
- stations, factions, travel time, or shipments
- production-grade security / rate limiting

---

## What I’d Improve Next

If I kept expanding this project, the next things I would look at are:

- moving beyond the current single-account assumption
- adding authentication and account isolation
- storing full price history instead of only the latest and previous price

---

## Notes for Users

- The live app is a shared public demo.
- Data can change between sessions.
- Use a unique commodity name when testing.
- `/portfolio` is intentionally kept as a legacy text endpoint.
- `/portfolio/valuation` is the structured portfolio endpoint.
