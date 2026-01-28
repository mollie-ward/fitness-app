# FitnessApp Backend API

RESTful API for the FitnessApp - A personalized fitness coaching application with AI-powered training plans.

## 🚀 Quick Start

### Prerequisites
- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) or later
- [PostgreSQL 15+](https://www.postgresql.org/download/) (or use Docker)
- [Docker](https://www.docker.com/get-started) (optional, for containerization)

### Running Locally

1. **Clone the repository**
   ```bash
   git clone https://github.com/mollie-ward/fitness-app.git
   cd fitness-app/backend
   ```

2. **Update database connection string**
   
   Edit `src/FitnessApp.API/appsettings.Development.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Host=localhost;Database=fitnessapp_dev;Username=postgres;Password=your_password"
     }
   }
   ```

3. **Run migrations**
   ```bash
   dotnet ef database update --project src/FitnessApp.Infrastructure --startup-project src/FitnessApp.API
   ```
   
   *Or the application will auto-migrate on startup.*

4. **Run the application**
   ```bash
   cd src/FitnessApp.API
   dotnet run
   ```

5. **Access the API**
   - Swagger UI: https://localhost:7001/swagger
   - API Base: https://localhost:7001/api/v1/
   - Health Check: https://localhost:7001/health

### Running with Docker Compose

```bash
# From repository root
docker-compose up --build

# Access API
http://localhost:5000/swagger
```

This starts:
- API on port 5000
- PostgreSQL on port 5432

## 📁 Project Structure

```
backend/
├── src/
│   ├── FitnessApp.API/              # Web API layer
│   │   ├── Controllers/             # API endpoints
│   │   ├── Middleware/              # Custom middleware
│   │   ├── Configuration/           # Configuration classes
│   │   └── Program.cs               # Application entry point
│   ├── FitnessApp.Application/      # Application layer
│   │   └── Common/
│   │       ├── Interfaces/          # Application interfaces
│   │       └── Models/              # DTOs and models
│   ├── FitnessApp.Domain/           # Domain layer
│   │   ├── Entities/                # Domain entities
│   │   └── Common/                  # Base classes
│   └── FitnessApp.Infrastructure/   # Infrastructure layer
│       ├── Persistence/             # Database context & migrations
│       └── DependencyInjection.cs   # Service registration
├── tests/
│   ├── FitnessApp.UnitTests/        # Unit tests
│   └── FitnessApp.IntegrationTests/ # Integration tests
└── FitnessApp.sln                   # Solution file
```

## 🏗️ Architecture

This project follows **Clean Architecture** principles with:

- **Domain Layer**: Core business entities
- **Application Layer**: Business rules and interfaces
- **Infrastructure Layer**: External concerns (database, files, etc.)
- **API Layer**: HTTP endpoints and web concerns

See [docs/backend-architecture.md](../docs/backend-architecture.md) for detailed architecture documentation.

## 🔧 Development

### Building

```bash
dotnet build
```

### Testing

```bash
# Run all tests
dotnet test

# Run with coverage
dotnet test --collect:"XPlat Code Coverage"

# Run only unit tests
dotnet test tests/FitnessApp.UnitTests/

# Run only integration tests
dotnet test tests/FitnessApp.IntegrationTests/
```

### Database Migrations

```bash
# Create a new migration
dotnet ef migrations add <MigrationName> \
  --project src/FitnessApp.Infrastructure \
  --startup-project src/FitnessApp.API

# Update database
dotnet ef database update \
  --project src/FitnessApp.Infrastructure \
  --startup-project src/FitnessApp.API

# Remove last migration (if not applied)
dotnet ef migrations remove \
  --project src/FitnessApp.Infrastructure \
  --startup-project src/FitnessApp.API
```

### Code Quality

```bash
# Check for vulnerabilities
dotnet list package --vulnerable

# Format code (if using dotnet-format)
dotnet format
```

## 📚 API Documentation

### Interactive Documentation
Access Swagger UI at `/swagger` when running the application.

### Available Endpoints

#### Status & Health
- `GET /health` - Health check endpoint
- `GET /api/v1/status` - API status information
- `GET /api/v1/status/error` - Test error handling

### Authentication

The API uses JWT Bearer token authentication:

```bash
# Example request with authentication
curl -H "Authorization: Bearer <your-token>" \
  https://localhost:7001/api/v1/status
```

### CORS

Configured to allow requests from:
- `http://localhost:3000` (React default)
- `http://localhost:4200` (Angular default)

Update `CorsSettings` in `appsettings.json` for additional origins.

## 🔐 Configuration

### Required Configuration

#### Development
Edit `appsettings.Development.json`:
- `ConnectionStrings:DefaultConnection` - Database connection
- `JwtSettings:Secret` - JWT signing key (32+ characters)

#### Production
Use environment variables or Azure Key Vault:
- `ConnectionStrings__DefaultConnection`
- `JwtSettings__Secret`
- `ApplicationInsights__ConnectionString`

### Configuration Sections

| Section | Description |
|---------|-------------|
| `ConnectionStrings` | Database connection strings |
| `JwtSettings` | JWT authentication configuration |
| `CorsSettings` | Allowed CORS origins |
| `Serilog` | Logging configuration |
| `ApplicationInsights` | Telemetry configuration |

## 🧪 Testing

### Test Coverage

Current test coverage: **≥85%** for critical components

- Unit Tests: 21 tests
- Integration Tests: 9 tests
- Total: 30 tests

### Test Categories

- **Configuration Tests**: Configuration loading and validation
- **Middleware Tests**: Error handling, logging, CORS
- **Integration Tests**: End-to-end API tests
- **Database Tests**: Database connectivity and operations

## 🐳 Docker

### Build Image

```bash
docker build -t fitnessapp-api .
```

### Run Container

```bash
docker run -p 8080:8080 \
  -e ConnectionStrings__DefaultConnection="Host=host.docker.internal;Database=fitnessapp;Username=postgres;Password=postgres" \
  fitnessapp-api
```

### Docker Compose

```bash
# Start all services
docker-compose up -d

# View logs
docker-compose logs -f api

# Stop services
docker-compose down
```

## 📊 Monitoring & Logging

### Logging
- **Framework**: Serilog
- **Console**: Colored output with timestamps
- **File**: Rolling daily logs in `logs/` directory
- **Structured**: JSON-formatted for easy parsing

### Health Checks
Monitor application health at `/health`:
- Database connectivity
- API responsiveness

### Application Insights (Optional)
Configure connection string in `appsettings.json` to enable:
- Request tracking
- Dependency tracking
- Exception tracking
- Custom metrics

## 🔒 Security

### Best Practices Implemented
- ✅ JWT authentication
- ✅ HTTPS enforcement
- ✅ Security headers (X-Frame-Options, CSP, etc.)
- ✅ CORS configuration
- ✅ Input validation
- ✅ Global error handling (no sensitive data exposure)
- ✅ Connection string encryption
- ✅ No vulnerable packages

### Production Checklist
- [ ] Change JWT secret (minimum 32 characters)
- [ ] Use Azure Key Vault for secrets
- [ ] Restrict CORS to actual frontend domain
- [ ] Enable rate limiting
- [ ] Configure Application Insights
- [ ] Set up automated backups
- [ ] Enable HTTPS only
- [ ] Review and test error handling

## 🚢 Deployment

### Azure App Service

```bash
# Publish
dotnet publish -c Release -o ./publish

# Deploy (using Azure CLI)
az webapp up --name <app-name> --resource-group <rg-name>
```

### Azure Container Instances

```bash
# Build and tag
docker build -t <registry>.azurecr.io/fitnessapp-api:latest .

# Push to ACR
docker push <registry>.azurecr.io/fitnessapp-api:latest

# Deploy
az container create --resource-group <rg> --name fitnessapp-api \
  --image <registry>.azurecr.io/fitnessapp-api:latest \
  --dns-name-label fitnessapp-api --ports 8080
```

## 📝 License

This project is licensed under the MIT License - see the [LICENSE.md](../LICENSE.md) file for details.

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Follow Clean Architecture principles
- Write unit and integration tests
- Maintain ≥85% code coverage
- Use async/await for I/O operations
- Add XML documentation comments
- Follow .NET naming conventions

## 🐛 Troubleshooting

### Database Connection Issues
```bash
# Check PostgreSQL is running
pg_isready -h localhost -p 5432

# Test connection
psql -h localhost -U postgres -d fitnessapp_dev
```

### Migration Errors
```bash
# Drop database and recreate
dotnet ef database drop --force
dotnet ef database update
```

### Port Already in Use
```bash
# Change port in Properties/launchSettings.json
# Or use environment variable
export ASPNETCORE_URLS="http://localhost:5001"
```

## 📧 Support

For issues and questions:
- Create an issue in the repository
- Contact the development team
