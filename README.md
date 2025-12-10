# Purpose:Punch!

![.NET 10](https://img.shields.io/badge/.NET-10.0-522cd4)
![PostgreSQL 17.7](https://img.shields.io/badge/PostgreSQL-17.7-2f6792)

**Purpose** of this app: To **punch** you for every decision you have made but never achieved. Now every such uncompleted decision will be put on public display, and you can already feel **punched** out!

## Concept

Users log significant decisions with reasoning and expectations.  
After a reflection period, users record outcomes and lessons learned.  
Users can optionally publish anonymized reflections for others to browse, learn from, or get inspired by.

## Tech Stack & Architecture

- **Framework:** .NET 10 / ASP.NET Core Web API
- **Database:** PostgreSQL (running via Docker)
- **ORM:** Entity Framework Core (Code-First)
- **Architecture:** Clean Architecture (Domain, Application, Infrastructure, API)
- **Patterns:** CQRS (via MediatR), Repository Pattern, Result Pattern
- **Testing:** xUnit, NSubstitute, Testcontainers, FluentAssertions

## Getting Started

### Prerequisites

- Docker & Docker Compose
- .NET SDK (10.0 or compatible)

### Installation

```pwsh
docker-compose up -d
dotnet run --project src/PurposePunch.Api
```

API endpoints via Swagger UI: http://localhost:7293/swagger.

## API Overview

### Auth

- `POST /api/Auth/register`: Creates a new user and auto-generates a unique anonymous nickname (e.g., "BraveToaster42") using the Bogus library.
- `POST /api/Auth/login`: Returns a JWT Token.

### Decisions (Private)

- `POST /api/Decisions`: Log a new decision with an expected reflection date.
- `PUT /api/Decisions/{id}`: Reflection phase. Updates outcomes and satisfaction. Automatically transitions status to Reflected.
- `POST /api/Decisions/{id}/publish`: Anonymizes the decision and pushes it to the public feed.

### Public Posts (Community)

- `GET /api/PublicPosts`: Paginated list of anonymized lessons from other users.
- `POST /api/PublicPosts/{id}/upvote`: Upvote post (Rate-limited per user (via JWT)/anonymous user (via `X-Device-Id` header)).

## Tests

```pwsh
dotnet test
```
