# VoidLedger

VoidLedger is a backend-first ASP.NET Core project for a fictional commodity trading system.  
It currently includes a Web API, a separated Core library, automated tests, CI, and local Docker support.

## Current status

The project currently has:
- `VoidLedger.Api` — ASP.NET Core Web API
- `VoidLedger.Core` — domain/application logic
- `VoidLedger.Core.Tests` — xUnit test project

Implemented so far:
- Thin controllers + service-layer business logic
- Structured domain results with `OpResult` + `ErrorCode`
- Centralized exception handling for unexpected failures
- Automated unit tests for critical paths
- GitHub Actions CI running `dotnet test`
- Docker support for local containerized runs

## Main features

Current API supports:
- Set commodity prices
- Deposit funds
- Buy commodities
- Sell commodities
- View portfolio report
- View recent actions
- View totals report
- View actions by type

## Architecture summary

The project is currently organized as:
- **VoidLedger.Api**
  - Controllers
  - Request DTOs / HTTP contracts
  - `OpResult` to HTTP / ProblemDetails mapping
  - ASP.NET Core startup and middleware
- **VoidLedger.Core**
  - Enums (`ActionType`, `ErrorCode`)
  - Results (`OpResult`, `TradeResult`)
  - Services (`LedgerService`, `TradeService`)
  - Action record hierarchy
  - In-memory state and helper services
  - Time abstraction (`IClock`, `SystemClock`, `FixedClock`)
- **VoidLedger.Core.Tests**
  - xUnit tests
  - Deterministic test factory using `FixedClock`

## Local development

### Run with `dotnet`

From the repository root:

1. Restore dependencies
2. Build the solution
3. Run the API project

Typical flow:
- `dotnet restore`
- `dotnet build`
- run the API project from `VoidLedger.Api`

## Running tests

From the repository root:
- `dotnet test`

Current test suite covers the critical business paths, including:
- valid/invalid price setting
- valid/invalid deposit
- buy/sell success and failure cases
- invalid name handling
- totals report correctness

## Run with Docker

The repository includes Docker support using:
- a **repo-root Dockerfile**
- a `.dockerignore`

### Build the image

From the repository root:
- `docker build -t voidledger-api:dev .`

### Run the container

From the repository root:
- `docker run --rm --name voidledger-api-dev -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development voidledger-api:dev`

### Access Swagger

After the container starts, open:
- `http://localhost:8080/swagger`

## Error handling

The API uses two layers of error handling:
- **Expected business failures** use `OpResult + ErrorCode` and are mapped to HTTP responses.
- **Unexpected exceptions** are handled globally and returned as clean HTTP 500 ProblemDetails responses.

## CI

GitHub Actions is configured to run `dotnet test` on the configured branches / PR flow.

## EF Core groundwork

The project already includes EF Core groundwork:
- `Accounts`
- `Prices`
- `Holdings`
- migrations applied successfully

However, runtime persistence is **not yet wired into the API flow**.

## Current limitation

At the moment, **runtime behavior is still in-memory**.  
That means API state does **not** yet use the SQL database as the source of truth during normal execution.

Azure SQL integration and persisted runtime behavior are intentionally deferred to a later phase.

## Planned next steps

Short-term next steps:
- polish README and usage docs
- continue deployment readiness work
- prepare Azure deployment
- later wire real persistence against Azure SQL

## Notes

This README is a temporary project-state document and will likely be updated as deployment and persistence phases are completed.
