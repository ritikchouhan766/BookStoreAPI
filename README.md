# BookStore API

A production-grade RESTful API built with **.NET 8**, **Clean Architecture**, **CQRS + MediatR**, **JWT Authentication**, **Docker**, and **xUnit** — demonstrating enterprise-level backend development patterns.

![CI](https://github.com/ritikchouhan766/BookStoreAPI/actions/workflows/ci.yml/badge.svg)
![.NET](https://img.shields.io/badge/.NET-8.0-purple)
![Docker](https://img.shields.io/badge/Docker-ready-blue)
![License](https://img.shields.io/badge/license-MIT-green)

---

## Architecture

```
BookStoreAPI/
├── src/
│   ├── BookStore.Domain/          # Entities, Interfaces — no dependencies
│   ├── BookStore.Application/     # CQRS Commands/Queries, DTOs, Validators
│   ├── BookStore.Infrastructure/  # EF Core, Repositories, JWT, SQL Server
│   └── BookStore.API/             # Controllers, Middleware, Program.cs
└── tests/
    └── BookStore.Tests/           # xUnit + Moq + FluentAssertions
```

**Pattern:** Domain → Application → Infrastructure → API (dependency rule: outer layers depend on inner layers, never the reverse)

---

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | .NET 8 / ASP.NET Core Web API |
| Architecture | Clean Architecture + CQRS (MediatR) |
| ORM | Entity Framework Core 8 |
| Database | SQL Server 2022 |
| Auth | JWT Bearer Tokens + BCrypt password hashing |
| Validation | FluentValidation (pipeline behaviour — auto-validates all commands) |
| Testing | xUnit + Moq + FluentAssertions + EF In-Memory |
| Containerisation | Docker (multi-stage build) + Docker Compose |
| CI/CD | GitHub Actions (build → test → push to GHCR) |
| API Docs | Swagger / OpenAPI with JWT support |

---

## Quick Start

### Option 1 — Docker Compose (recommended, no setup needed)

```bash
git clone https://github.com/ritikchouhan766/BookStoreAPI.git
cd BookStoreAPI
docker-compose up --build
```

API available at: **http://localhost:8080**  
Swagger UI at: **http://localhost:8080/index.html**

SQL Server starts automatically inside Docker — no installation required.

### Option 2 — Local development

**Prerequisites:** .NET 8 SDK, SQL Server (local or Docker)

```bash
git clone https://github.com/ritikchouhan766/BookStoreAPI.git
cd BookStoreAPI

# Update connection string in appsettings.Development.json
# Then run migrations and start
cd src/BookStore.API
dotnet ef database update
dotnet run
```

---

## API Endpoints

### Auth
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| POST | `/api/auth/register` | None | Register new user |
| POST | `/api/auth/login` | None | Login → returns JWT |

### Books
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/books` | None | Get all books (paginated, genre filter) |
| GET | `/api/books/{id}` | None | Get book by ID |
| POST | `/api/books` | Bearer | Create book |
| PUT | `/api/books/{id}` | Bearer | Update book |
| DELETE | `/api/books/{id}` | Admin only | Delete book |

### Authors & Publishers
| Method | Endpoint | Auth | Description |
|---|---|---|---|
| GET | `/api/authors` | None | Get all authors |
| POST | `/api/authors` | Admin | Create author |
| GET | `/api/publishers` | None | Get all publishers |
| POST | `/api/publishers` | Admin | Create publisher |

---

## Testing

```bash
# Run all tests
dotnet test

# Run with coverage report
dotnet test --collect:"XPlat Code Coverage"
```

**Test coverage:** 12 unit tests covering all CQRS handlers using EF Core In-Memory database — no SQL Server required for tests.

### Seeded Admin Account
After running migrations, a default admin is seeded:
- **Email:** `admin@bookstore.com`
- **Password:** `Admin@123`

---

## Key Design Decisions

**Why Clean Architecture?**  
Each layer has a single responsibility. The Domain has zero dependencies — it can be unit tested in complete isolation. Business logic lives in Application, not in controllers.

**Why CQRS?**  
Commands (write) and Queries (read) are separated. This makes the codebase easier to reason about, test independently, and scale (separate read/write models in the future).

**Why MediatR pipeline behaviour for validation?**  
FluentValidation runs automatically on every command via a registered `IPipelineBehavior`. Controllers never handle validation manually — it's cross-cutting and consistent.

**Why multi-stage Dockerfile?**  
Tests run inside Docker during the build stage. If any test fails, the image does not build — enforcing quality at the container level, not just in CI.

---

## CI/CD Pipeline

```
Push to main
    │
    ├── dotnet restore
    ├── dotnet build (Release)
    ├── dotnet test (xUnit — 12 tests)
    │
    └── Docker build (multi-stage, tests run inside)
        └── Push to GitHub Container Registry (ghcr.io)
```

---

## Author

**Ritik Chouhan**  
Full Stack Developer — .NET Core · Angular · Azure  
[LinkedIn](https://linkedin.com/in/ritik-chouhan-4a297b192) · [GitHub](https://github.com/ritikchouhan766)
