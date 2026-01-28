# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src

# Copy solution and project files
COPY ["backend/FitnessApp.sln", "backend/"]
COPY ["backend/src/FitnessApp.API/FitnessApp.API.csproj", "backend/src/FitnessApp.API/"]
COPY ["backend/src/FitnessApp.Application/FitnessApp.Application.csproj", "backend/src/FitnessApp.Application/"]
COPY ["backend/src/FitnessApp.Domain/FitnessApp.Domain.csproj", "backend/src/FitnessApp.Domain/"]
COPY ["backend/src/FitnessApp.Infrastructure/FitnessApp.Infrastructure.csproj", "backend/src/FitnessApp.Infrastructure/"]

# Restore dependencies
RUN dotnet restore "backend/src/FitnessApp.API/FitnessApp.API.csproj"

# Copy the rest of the source code
COPY backend/ backend/

# Build and publish
WORKDIR "/src/backend/src/FitnessApp.API"
RUN dotnet build "FitnessApp.API.csproj" -c Release -o /app/build
RUN dotnet publish "FitnessApp.API.csproj" -c Release -o /app/publish /p:UseAppHost=false

# Runtime stage
FROM mcr.microsoft.com/dotnet/aspnet:8.0 AS runtime
WORKDIR /app
EXPOSE 8080
EXPOSE 8081

# Copy published output from build stage
COPY --from=build /app/publish .

# Set environment variables
ENV ASPNETCORE_URLS=http://+:8080
ENV ASPNETCORE_ENVIRONMENT=Production

# Health check
HEALTHCHECK --interval=30s --timeout=5s --start-period=30s --retries=3 \
  CMD curl --fail http://localhost:8080/health || exit 1

ENTRYPOINT ["dotnet", "FitnessApp.API.dll"]
