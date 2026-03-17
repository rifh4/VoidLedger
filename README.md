# Void Ledger API

Void Ledger is a REST API I built to simulate a sci-fi commodity trading market. I created this project to get hands-on experience building a full backend system with ASP.NET Core, Entity Framework Core, and Azure SQL.

You can view the live Swagger UI [here](https://voidledger-api-rifh.azurewebsites.net/swagger/index.html).

**Note on the live demo:** The database is publicly shared, so the state can change while you are using it if someone else is sending requests at the same time. If you want to test the endpoints, it helps to use a unique commodity name such as `TEST_ORE_948` so your data does not overlap with other users.

## Screenshots

### Swagger UI overview
<img src="docs/images/swagger-overview.png" alt="Swagger UI overview" width="400" />

### Price movement example
<img src="docs/images/price-movement.png" alt="GET /prices/{name} showing previous price, change amount, and direction" width="300" />

### Portfolio valuation
<img src="docs/images/portfolio-valuation.png" alt="GET /portfolio/valuation structured JSON response" width="300" />

## Project Goals

My main goal with this project was to move beyond tutorials and build something with a proper layered backend structure. I focused on learning how to:

- connect an API to a real SQL database with EF Core
- keep controllers separate from business logic
- handle expected failures cleanly without relying on exceptions for normal flow
- deploy a Dockerized application to Azure App Service

## Core Features

The API lets a client manage a simulated trading account. It supports:

- depositing virtual cash into the account
- setting and updating commodity prices
- buying and selling commodities at the current market price
- generating a detailed portfolio valuation
- viewing a history of recent account actions

To make the market feel a little less static, I also added price movement metadata. When a user requests a price, the API returns the previous price, the difference, and a direction string such as `Up`, `Down`, or `Flat`.

## How the Code Is Organized

I split the solution into three projects to keep responsibilities separate.

**1. VoidLedger.Api**  
This project handles the web layer. It contains the controllers, dependency injection setup, and the Entity Framework Core `DbContext`. I tried to keep the controllers thin so they mostly receive HTTP requests, pass work to the service layer, and map the results back to HTTP responses.

**2. VoidLedger.Core**  
This is where the business logic lives. It contains services such as `LedgerService` and `TradeService`. I also defined store interfaces here so the core logic does not depend directly on Entity Framework.

**3. VoidLedger.Core.Tests**  
This project contains the unit tests. I used xUnit and a fake in-memory store so I could test buy, sell, and valuation behavior without needing a real database connection.

## Design Choices

### The OpResult pattern
Instead of letting exceptions drive normal business failures, I created an `OpResult` class. Core service methods return an `OpResult` with a success flag, an `ErrorCode`, and a message. In the API layer, a mapper translates those results into the correct HTTP responses.

### Portfolio valuation logic
I spent extra time on the `/portfolio/valuation` endpoint. A user might own a commodity even when its current price is missing from the database. Instead of failing or pretending the value is zero, the API returns `null` for that position's value while still calculating totals for the positions that do have prices.

## Running the Project Locally

To run this on your own machine, you will need the .NET 8 SDK.

1. Clone the repository.
2. Open a terminal in the root folder.
3. Restore the packages:

```bash
dotnet restore
```

4. Run the API project:

```bash
dotnet run --project VoidLedger.Api
```

You will also need to provide a SQL Server connection string named `VoidLedgerDb` in your user secrets or `appsettings.Development.json` file.

If you prefer to use Docker, the repository includes a `Dockerfile` that you can build and run as long as you pass the required environment variables.

## Testing

Run the test suite with:

```bash
dotnet test
```

The tests cover the main business paths, including:

- rejecting negative deposits
- preventing users from selling more than they own
- validating buy and sell flows
- checking portfolio valuation behavior under different pricing conditions
