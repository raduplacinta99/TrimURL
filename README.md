# TrimURL

![.NET](https://img.shields.io/badge/.NET-8.0-blue)
![Docker](https://img.shields.io/badge/Docker-2496ED?logo=docker&logoColor=white)
![PostgreSQL](https://img.shields.io/badge/PostgreSQL-4169E1?logo=postgresql&logoColor=white)
![Redis](https://img.shields.io/badge/Redis-DC382D?logo=redis&logoColor=white)
![License](https://img.shields.io/badge/License-MIT-green)
![Build](https://github.com/raduplacinta99/TrimURL/actions/workflows/api.yml/badge.svg)
![Build](https://github.com/raduplacinta99/TrimURL/actions/workflows/webapp.yml/badge.svg)

TrimURL is a full-stack URL shortener built with ASP.NET Core and React. It allows users to create, manage, and track shortened URLs while demonstrating modern backend development practices such as layered architecture, authentication, caching, unit testing, Docker, and CI/CD.

This project was built as part of my portfolio to showcase clean architecture and production-ready development practices.

## Live Demo

- **Swagger UI:** https://trimurlapi-g2hkcfhnc6gsevaa.westeurope-01.azurewebsites.net/

## Features

### URL Management

- Create shortened URLs
- Redirect shortened URLs
- Set URL expiration dates
- Update existing URLs
- Delete URLs
- Retrieve URLs by creator

### Authentication & Authorization

- JWT Authentication & Authorization
- User registration and login
- Role-based authorization

### Performance

- Redis distributed caching
- In-memory caching
- Rate limiting

### Quality & Reliability

- Global exception handling middleware
- Input validation
- Redis and In-Memory caching
- Unit tests using xUnit + Moq
- Integration tests
- GitHub Actions CI/CD

## Tech Stack

**Backend**
- ASP.NET Core 8

**Frontend**
- React
- Bootstrap

**Database**
- PostgreSQL
- Entity Framework Core

**Caching**
- Redis
- IMemoryCache

**Unit/Integration Testing**
- xUnit

**DevOps**
- Docker
- Github Actions
- Azure App Service

## Architecture

The project follows a layered architecture.

```
Controllers
      ↓
Services
      ↓
Repositories
      ↓
Entity Framework Core
      ↓
PostgreSQL
```

Responsibilities:

- Controllers handle HTTP requests.
- Services contain business logic.
- Repositories handle database access.

## Getting Started

### Prerequisites

- .NET 8 SDK
- Docker (optional)
- Redis (optional)

### Clone the repository

```bash
git clone https://github.com/raduplacinta99/TrimURL

cd TrimURL/TrimUrlApi
```

### Restore packages

```bash
dotnet restore
```

### Configuration

The application uses ASP.NET Core configuration and environment variables.

| Setting | Description |
|---------|-------------|
| `ConnectionStrings__TrimUrlDatabase` | PostgreSQL connection string |
| `ConnectionStrings__Redis` | Redis connection string (optional) |
| `Jwt__Secret` | Secret used to sign JWT tokens |

### Apply migrations

```bash
dotnet ef database update
```

### Run

```bash
dotnet run
```

### Run with Docker Compose

```bash
docker compose up --build
```

### Running Tests

```bash
dotnet test
```

## CI/CD

Every push to the `main` branch automatically:

1. Builds the application.
2. Runs the test suite.
3. Builds a Docker image.
4. Pushes the image to Docker Hub.
5. Deploys the latest image to Azure App Service.

## Future Improvements

- Custom short codes
- Improved URL analytics (browsers, locations etc.)
- QR code generation
- Email verification
- Metrics and monitoring

## License

This project is licensed under the MIT License. See the `LICENSE` file for details.
