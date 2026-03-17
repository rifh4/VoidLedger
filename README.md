# Void Ledger API

Void Ledger is a REST API I built to simulate a sci-fi commodity trading market. I created this project specifically to get showcase experience building a complete backend system from scratch using ASP.NET Core, Entity Framework Core, and Azure SQL.

You can view the live Swagger UI [here](https://voidledger-api-rifh.azurewebsites.net/swagger/index.html).

**Note on the live demo:** The database is publicly shared. Because of this, the state might change while you are using it if someone else is also sending requests. If you want to test the endpoints, it helps to create a commodity with a unique name (for example, `TEST_ORE_948`) so your data does not overlap with other visitors.

## Screenshots

### Swagger UI overview
<img src="docs/images/swagger-overview.png" alt="Swagger UI overview" width="400" />

### Price movement example
<img src="docs/images/price-movement.png" alt="GET /prices/{name} showing previous price, change amount, and direction" width="300" />

### Portfolio valuation
<img src="docs/images/portfolio-valuation.png" alt="GET /portfolio/valuation structured JSON response" width="300" />

## Project Goals

My main goal with this project was to  build an application with a proper layered architecture. I focused on how to:

* Connect an API to a real SQL database using EF Core.
* Separate my HTTP controllers from my core business logic.
* Handle expected errors cleanly without throwing exceptions everywhere.
* Automate my testing process using GitHub Actions.
* Containerize and deploy the application using Docker and Azure App Service.

## Core Features

The API allows a client to manage a simulated trading account. The available actions are:

* Depositing virtual cash into the account.
* Setting and updating the market prices of commodities.
* Buying and selling commodities based on the current market price.
* Generating a detailed portfolio valuation.
* Viewing a history of recent account actions.

To make the market feel slightly more dynamic, I added logic to track price movements. When you request a price, the API also returns the previous price, the mathematical difference, and a direction string (`Up`, `Down`, or `Flat`).

## Main Endpoints

### Prices
* `POST /prices`
* `GET /prices`
* `GET /prices/{name}`

### Trading
* `POST /deposit`
* `POST /trade/buy`
* `POST /trade/sell`

### Portfolio and reporting
* `GET /portfolio`
* `GET /portfolio/valuation`
* `GET /actions/recent?take=10`
* `GET /reports/totals`
* `GET /reports/actions/by-type?type=Buy&take=10`

## How the Code is Organized

I divided the solution into three distinct projects to keep responsibilities separate:

**1. VoidLedger.Api**  
This project handles the web layer. It contains the Controllers, the setup for Dependency Injection, and the Entity Framework Core `DbContext`. I tried to keep the controllers as thin as possible. Their job is to receive the HTTP request, pass the data to the service layer, and then map the result back to an HTTP response.

**2. VoidLedger.Core**  
This is where the actual business logic lives. It contains the `LedgerService` and `TradeService`. I created store interfaces (like `ILedgerStore`) here so that the core logic does not have a direct dependency on Entity Framework.

**3. VoidLedger.Core.Tests**  
This project contains my unit tests. I used xUnit and created a fake in-memory version of the store so I could test the buy, sell, and valuation logic without needing a real database connection.

## Design Choices

### The OpResult Pattern

Instead of letting exceptions bubble up when a user tries to buy something they cannot afford, I created an `OpResult` class. Every core service method returns an `OpResult` that contains a success flag, an `ErrorCode` enum, and a message. In the API layer, I wrote a mapper that checks these results and translates them into the correct HTTP status codes.

### Portfolio Valuation Logic

I spent extra time on the `/portfolio/valuation` endpoint. A user might own a commodity even when the current price for that commodity is missing from the database. Instead of failing or assuming the value is zero, the endpoint returns `null` for that specific position value while still calculating totals for the assets that do have prices.

## Infrastructure and Automation

I wanted to get some hands-on experience with how applications are actually built and deployed in the real world, so I did not stop at just running it locally in my IDE.

### Continuous Integration (GitHub Actions)

To make sure I was not breaking old features while adding new ones, I wrote a simple YAML workflow (`dotnet-tests.yml`). Every time I push code to the repository, a GitHub Action restores the dependencies, builds the project, and runs the xUnit test suite.

### Docker

I wrote a `Dockerfile` for the API using a multi-stage build. It uses the official .NET 8 SDK image to build and publish the application, and then copies the output into a lighter ASP.NET runtime image. This helps keep the runtime image smaller and makes local and deployed runs more consistent.

### Azure Deployment

The project is deployed to Azure App Service and uses Azure SQL for persistence. I used this project to get hands-on practice with publishing a containerized app, configuring connection strings and app settings, and making sure the persisted state survives redeploys.

## Tech Stack

* C# / .NET 8
* ASP.NET Core Web API
* Entity Framework Core
* Azure SQL
* Azure App Service
* xUnit
* GitHub Actions
* Docker

## Running the Project Locally

If you have the .NET 8 SDK installed, you can run the project directly:

1. Clone the repository and open a terminal in the root folder.
2. Restore the packages: `dotnet restore`
3. Run the API project: `dotnet run --project VoidLedger.Api`

**Note:** You will need to provide a SQL Server connection string named `VoidLedgerDb` in your user secrets.

## Running with Docker

If you prefer not to install the .NET SDK, you can build and run the container directly:

`docker build -t voidledger-api:dev .`  
`docker run --rm --name voidledger-api-dev -p 8080:8080 -e ASPNETCORE_ENVIRONMENT=Development voidledger-api:dev`

## Testing

You can run the full test suite with:

`dotnet test`

The tests cover the main business paths, including:

* rejecting negative deposits
* rejecting invalid price updates
* preventing users from selling more than they own
* handling missing prices and missing holdings
* portfolio valuation behavior under different conditions

## What I’d Improve Next

If I kept expanding the project, the next things I would look at are:

* moving beyond the current single-account assumption
* adding authentication and account isolation
* storing full price history instead of only the latest and previous price
