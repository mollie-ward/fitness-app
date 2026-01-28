# Backend Architecture Documentation

## Overview

The FitnessApp backend is built using **ASP.NET Core 8.0** and follows **Clean Architecture** principles with a clear separation of concerns across multiple layers.

## Architecture Layers

### 1. Domain Layer (`FitnessApp.Domain`)
**Purpose**: Contains the core business entities and domain logic.

**Responsibilities**:
- Define domain entities (e.g., `User`)
- Define value objects
- Contains no dependencies on other layers
- Represents the heart of the business logic

**Key Components**:
- `Common/BaseEntity.cs` - Base class for all entities with audit fields
- `Entities/User.cs` - User domain entity

### 2. Application Layer (`FitnessApp.Application`)
**Purpose**: Defines application business rules and orchestrates the flow of data.

**Responsibilities**:
- Define interfaces for infrastructure concerns
- Define DTOs and models for API responses
- Coordinate application behavior
- Independent of frameworks and external concerns

**Key Components**:
- `Common/Interfaces/IApplicationDbContext.cs` - Database context interface
- `Common/Models/Result.cs` - Standardized result wrapper

### 3. Infrastructure Layer (`FitnessApp.Infrastructure`)
**Purpose**: Implements interfaces defined in the Application layer and provides external services.

**Responsibilities**:
- Database implementation (Entity Framework Core)
- External service integrations
- File system access
- Email services (future)

**Key Components**:
- `Persistence/ApplicationDbContext.cs` - EF Core database context
- `Persistence/ApplicationDbContextFactory.cs` - Design-time DB context factory
- `DependencyInjection.cs` - Infrastructure service registration

**Database Configuration**:
- **Provider**: PostgreSQL (via Npgsql.EntityFrameworkCore.PostgreSQL)
- **ORM**: Entity Framework Core 8.0
- **Migrations**: Code-first migrations
- **Features**: Connection resiliency with automatic retries

### 4. API Layer (`FitnessApp.API`)
**Purpose**: Exposes HTTP endpoints and handles web concerns.

**Responsibilities**:
- Define API endpoints (controllers)
- Request/response handling
- Authentication and authorization
- API versioning
- Cross-cutting concerns (logging, error handling)

**Key Components**:
- `Program.cs` - Application bootstrap and middleware configuration
- `Controllers/StatusController.cs` - Health and status endpoints
- `Middleware/GlobalExceptionHandlerMiddleware.cs` - Global error handling
- `Middleware/RequestLoggingMiddleware.cs` - HTTP request/response logging
- `Configuration/` - Strongly-typed configuration classes

## Infrastructure Configuration

### Authentication & Security
- **Authentication**: JWT Bearer token authentication
- **Authorization**: Policy-based authorization (configured, ready for use)
- **Security Headers**: 
  - X-Content-Type-Options: nosniff
  - X-Frame-Options: DENY
  - X-XSS-Protection: 1; mode=block
  - Referrer-Policy: no-referrer
- **HTTPS**: Enforced via middleware

### API Documentation
- **Swagger/OpenAPI**: Fully configured with JWT authentication support
- **Endpoint**: `/swagger`
- **Features**:
  - Interactive API documentation
  - JWT authentication UI
  - XML documentation comments

### CORS (Cross-Origin Resource Sharing)
- **Policy Name**: DefaultCorsPolicy
- **Allowed Origins**: Configured via `appsettings.json`
- **Default Origins**: 
  - http://localhost:3000 (React)
  - http://localhost:4200 (Angular)
- **Methods**: All HTTP methods allowed
- **Headers**: All headers allowed
- **Credentials**: Allowed

### API Versioning
- **Strategy**: URL segment versioning (e.g., `/api/v1/status`)
- **Default Version**: 1.0
- **Version Format**: Major.Minor (e.g., v1, v2)

### Logging & Monitoring
- **Logging Framework**: Serilog
- **Structured Logging**: Yes
- **Log Outputs**:
  - Console (colored output)
  - Rolling file (`logs/fitnessapp-{Date}.txt`)
- **Application Insights**: Configured (optional connection string)
- **Request Logging**: Custom middleware logs all HTTP requests with timing

### Health Checks
- **Endpoint**: `/health`
- **Checks**:
  - Database connectivity (PostgreSQL)
- **Response Format**: Plain text or JSON
- **Status Codes**: 
  - 200 (Healthy)
  - 503 (Unhealthy)

## Dependency Injection

All services are registered using ASP.NET Core's built-in DI container:

```csharp
// Infrastructure services
builder.Services.AddInfrastructure(configuration);

// Authentication
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(...);

// CORS
builder.Services.AddCors(...);

// API Versioning
builder.Services.AddApiVersioning(...);
```

## Configuration Management

Configuration is managed through:
- `appsettings.json` - Base configuration
- `appsettings.Development.json` - Development overrides
- Environment variables - Production secrets
- User secrets - Local development secrets

**Key Configuration Sections**:
- `ConnectionStrings` - Database connection strings
- `JwtSettings` - JWT authentication settings
- `CorsSettings` - CORS allowed origins
- `Serilog` - Logging configuration
- `ApplicationInsights` - Telemetry configuration

## Middleware Pipeline

The request pipeline is configured in this order:

1. **GlobalExceptionHandlerMiddleware** - Catches and formats all exceptions
2. **RequestLoggingMiddleware** - Logs HTTP requests/responses
3. **Swagger** - API documentation UI
4. **HTTPS Redirection** - Forces HTTPS
5. **Security Headers** - Adds security headers
6. **CORS** - Handles cross-origin requests
7. **Authentication** - Validates JWT tokens
8. **Authorization** - Checks user permissions
9. **Controller Routing** - Routes to API endpoints

## Database Migrations

**Running Migrations**:
```bash
# Create a new migration
dotnet ef migrations add <MigrationName> --project src/FitnessApp.Infrastructure --startup-project src/FitnessApp.API

# Update database
dotnet ef database update --project src/FitnessApp.Infrastructure --startup-project src/FitnessApp.API
```

**Auto-Migration**: The application automatically applies pending migrations on startup (configurable).

## Testing Strategy

### Unit Tests (`FitnessApp.UnitTests`)
- Tests for configuration loading
- Tests for middleware behavior
- Tests for dependency injection setup
- Frameworks: xUnit, Moq, FluentAssertions

### Integration Tests (`FitnessApp.IntegrationTests`)
- Tests for API endpoints
- Tests for database connectivity
- Tests for CORS configuration
- Uses in-memory database for isolation
- Framework: Microsoft.AspNetCore.Mvc.Testing

**Running Tests**:
```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"
```

## Deployment

### Docker Support
The application includes:
- `Dockerfile` - Multi-stage build
- `docker-compose.yml` - Local development with PostgreSQL
- `.dockerignore` - Optimized build context

**Running with Docker**:
```bash
# Build and run
docker-compose up --build

# Access API
http://localhost:5000/swagger
```

### Environment Configuration
- **Development**: Uses local PostgreSQL, detailed logging
- **Production**: Uses Azure-hosted PostgreSQL, Application Insights, minimal logging

## Security Considerations

1. **JWT Secrets**: Must be changed in production (minimum 32 characters)
2. **Connection Strings**: Should use Azure Key Vault in production
3. **CORS Origins**: Must be restricted to actual frontend domains
4. **HTTPS**: Always enforced
5. **Error Details**: Not exposed in production environment

## Performance Optimizations

- **Connection Pooling**: Enabled by default for database connections
- **Connection Resiliency**: Automatic retry on transient failures
- **Async/Await**: Used throughout for I/O operations
- **Response Compression**: Can be enabled (not configured yet)
- **Output Caching**: Can be enabled (not configured yet)

## Extensibility Points

The architecture is designed for easy extension:

1. **Adding New Entities**: Create in Domain layer, add to DbContext
2. **Adding Services**: Define interface in Application, implement in Infrastructure
3. **Adding Endpoints**: Create controller in API layer
4. **Adding Middleware**: Implement and register in Program.cs
5. **Adding Authentication Schemes**: Register in authentication configuration

## Future Enhancements

Prepared infrastructure for:
- Email services
- File storage (Azure Blob)
- Caching (Redis)
- Message queues (Azure Service Bus)
- Real-time communication (SignalR)
- Azure Key Vault integration
