# Game News Hub

Game News Hub is an MVP project focused on aggregating and personalizing video game event/news data (releases, updates, related metadata).  
The current implementation delivers a backend-first foundation with IGDB integration, user accounts, interest profiling, and recommendation endpoints.

## Key Features

- External data sync pipeline for games, genres, and events (IGDB + background workers).
- User authentication and authorization (JWT for production, development auth scheme for local work).
- User interest profiling (follow/unfollow games and genres).
- Personalized event recommendations based on user interest weights.
- Vertical slice architecture in the API (`Features/*`), where each feature owns its controller, service, and registration.

## Tech Stack

- Backend: .NET 10 Web API (C#), ASP.NET Core Identity, Entity Framework Core.
- API Documentation: OpenAPI + Swagger UI + Scalar.
- Data: currently SQLite in development (`sqlite.db`), with PostgreSQL planned as the target relational DB for full scope.
- Integrations: IGDB/Twitch API.
- Tests: xUnit + Moq (unit tests for selected core services).
- Planned/target frontend: Angular (TypeScript) responsive UI.

## How To Run

### Prerequisites

- .NET 10 SDK
- (Optional) Docker, if you want to extend local setup

### 1) Configure secrets/env vars

Set required configuration values for IGDB and JWT:

- `Api:ClientId`
- `Api:ClientSecret`
- `Jwt:Key`

You can provide them using user secrets or environment variables.

### 2) Start the API

From the `backend` directory:

```bash
dotnet restore
dotnet run --project GameNewsHub.Api
```

On startup, EF Core migrations are applied automatically.

### 3) Explore endpoints

- Swagger UI: `http://localhost:5190/swagger`
- Scalar: `http://localhost:5190/scalar`

### 4) Run tests

From the `backend` directory:

```bash
dotnet test
```

## Project Status

This repository represents an active MVP.
Core backend capabilities are implemented, while parts of the full scope (including complete admin tooling, broader security hardening, and full Angular frontend) are planned next.