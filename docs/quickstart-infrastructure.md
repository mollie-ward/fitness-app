# Quick Start Guide - Infrastructure Setup

Get the Fitness App infrastructure running locally in 5 minutes!

## Prerequisites

- Docker Desktop installed and running
- Git installed
- Azure CLI (optional, for deployment)

## Local Development (docker-compose)

### 1. Clone the Repository

```bash
git clone https://github.com/mollie-ward/fitness-app.git
cd fitness-app
```

### 2. Start All Services

```bash
# Start backend, frontend, and database
docker compose up -d

# View logs
docker compose logs -f

# Check status
docker compose ps
```

### 3. Verify Services

**Backend API**:
```bash
# Health check
curl http://localhost:5000/health

# Swagger UI
open http://localhost:5000/swagger
```

**Frontend**:
```bash
# Health check
curl http://localhost:3000/api/health

# Open app
open http://localhost:3000
```

**Database**:
```bash
# Connect with psql
docker compose exec postgres psql -U postgres -d fitnessapp
```

### 4. Stop Services

```bash
# Stop all services
docker compose down

# Stop and remove data
docker compose down -v
```

## Azure Deployment (Quick Deploy)

### 1. Login to Azure

```bash
az login
az account set --subscription <your-subscription-id>
```

### 2. Create Resource Group

```bash
az group create \
  --name fitnessapp-dev-rg \
  --location eastus
```

### 3. Deploy Infrastructure

```bash
cd infrastructure/bicep

az deployment group create \
  --resource-group fitnessapp-dev-rg \
  --template-file main.bicep \
  --parameters environment=dev \
               sqlAdminUsername=sqladmin \
               sqlAdminPassword='YourSecurePassword123!' \
               jwtSecret='your-32-char-secret-key-here'
```

### 4. Get Deployment Outputs

```bash
az deployment group show \
  --resource-group fitnessapp-dev-rg \
  --name main \
  --query properties.outputs
```

## GitHub Actions Setup

### 1. Create Service Principal

```bash
az ad sp create-for-rbac \
  --name "fitnessapp-github-actions" \
  --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --sdk-auth > azure-credentials.json
```

### 2. Configure GitHub Secrets

Go to: Settings > Secrets and variables > Actions

Add these secrets:
- AZURE_CREDENTIALS: Contents of azure-credentials.json
- AZURE_RESOURCE_GROUP_DEV: fitnessapp-dev-rg
- SQL_ADMIN_USERNAME: Your SQL admin username
- SQL_ADMIN_PASSWORD: Your SQL admin password
- JWT_SECRET: Your JWT secret (min 32 chars)

### 3. Trigger Deployment

```bash
# Push to main branch to trigger CI/CD
git push origin main

# Or manually trigger workflows
gh workflow run infrastructure-deploy.yml -f environment=dev
```

## Troubleshooting

### Docker Issues

Problem: Port already in use
- Solution: Change ports in docker-compose.yml or stop conflicting services

Problem: Build fails
- Solution: Clean Docker cache with docker system prune -a

### Azure Issues

Problem: Deployment fails
- Solution: Check Azure Activity Log for detailed errors

Problem: Cannot connect to database
- Solution: Add your IP to SQL Server firewall rules

## Next Steps

1. Local Development
   - Explore the API at http://localhost:5000/swagger
   - Test the frontend at http://localhost:3000

2. Azure Deployment
   - Configure custom domains
   - Set up monitoring alerts

3. CI/CD Pipeline
   - Configure all GitHub Secrets
   - Test workflows

## Resources

- [Full Deployment Guide](./deployment-guide.md)
- [Security Checklist](./security-checklist.md)
- [Infrastructure README](../infrastructure/README.md)

---

Happy Deploying!
