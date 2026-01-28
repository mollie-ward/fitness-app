# Docker Compose Development Environment

## Important: Database Technology Difference

**Local Development (docker-compose)**: Uses PostgreSQL 15
**Azure Production**: Uses Azure SQL Database (Microsoft SQL Server)

### Why Different Databases?

- **PostgreSQL** for local development: Easy to run in Docker, no licensing costs
- **Azure SQL Database** for production: Managed service, enterprise features, Azure integration

### Considerations

1. **SQL Syntax**: Some queries may need adjustments between PostgreSQL and SQL Server
2. **Data Types**: Minor differences in data type names and behavior
3. **Migrations**: Test migrations in both environments
4. **Development**: Entity Framework Core abstracts most differences

### Best Practices

- Use Entity Framework Core's database-agnostic features
- Test migrations against SQL Server before deploying to Azure
- Use database-agnostic LINQ queries where possible
- Document any database-specific code clearly

### Alternative: Use SQL Server Locally

To match production exactly, update docker-compose.yml to use SQL Server:

```yaml
db:
  image: mcr.microsoft.com/mssql/server:2022-latest
  environment:
    - ACCEPT_EULA=Y
    - SA_PASSWORD=YourStrong@Passw0rd
    - MSSQL_PID=Developer
  ports:
    - "1433:1433"
```

Then update the connection string:
```
Server=localhost,1433;Database=fitnessapp;User Id=sa;Password=YourStrong@Passw0rd;TrustServerCertificate=True
```
