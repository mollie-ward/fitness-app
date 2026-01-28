# Deployment Guide - Fitness App

This guide provides comprehensive instructions for deploying the Fitness App to Azure using Infrastructure as Code (Bicep) and GitHub Actions.

## Table of Contents

1. [Prerequisites](#prerequisites)
2. [Local Development Setup](#local-development-setup)
3. [Azure Infrastructure Setup](#azure-infrastructure-setup)
4. [GitHub Secrets Configuration](#github-secrets-configuration)
5. [CI/CD Pipeline](#cicd-pipeline)
6. [Manual Deployment](#manual-deployment)
7. [Monitoring and Logging](#monitoring-and-logging)
8. [Domain and SSL Configuration](#domain-and-ssl-configuration)
9. [Troubleshooting](#troubleshooting)

---

## Prerequisites

### Required Tools

- **Azure CLI** (v2.50.0 or later)
  ```bash
  az --version
  az login
  ```

- **Docker Desktop** (for local development)
  ```bash
  docker --version
  docker-compose --version
  ```

- **.NET SDK 8.0**
  ```bash
  dotnet --version
  ```

- **Node.js 20.x**
  ```bash
  node --version
  npm --version
  ```

- **Bicep CLI**
  ```bash
  az bicep install
  az bicep version
  ```

### Azure Subscription

- Active Azure subscription with appropriate permissions
- Resource Group creation rights
- Service Principal for GitHub Actions

---

## Local Development Setup

### 1. Clone the Repository

```bash
git clone https://github.com/mollie-ward/fitness-app.git
cd fitness-app
```

### 2. Start Local Environment with Docker Compose

```bash
# Start all services (backend, frontend, database)
docker-compose up -d

# View logs
docker-compose logs -f

# Stop services
docker-compose down

# Stop and remove volumes
docker-compose down -v
```

### 3. Verify Local Deployment

- **Backend API**: http://localhost:5000
  - Health check: http://localhost:5000/health
  - Swagger UI: http://localhost:5000/swagger

- **Frontend**: http://localhost:3000
  - Health check: http://localhost:3000/api/health

- **Database**: localhost:5432
  - Database: fitnessapp
  - User: postgres
  - Password: postgres

### 4. Build Docker Images Locally

```bash
# Build backend image
docker build -t fitnessapp-backend:local -f Dockerfile .

# Build frontend image
cd frontend
docker build -t fitnessapp-frontend:local .
cd ..
```

---

## Azure Infrastructure Setup

### 1. Create Azure Service Principal

```bash
# Create service principal for GitHub Actions
az ad sp create-for-rbac \
  --name "fitnessapp-github-actions" \
  --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --sdk-auth

# Save the JSON output - you'll need it for GitHub Secrets
```

### 2. Create Resource Groups

```bash
# Development environment
az group create \
  --name fitnessapp-dev-rg \
  --location eastus

# Production environment
az group create \
  --name fitnessapp-prod-rg \
  --location eastus
```

### 3. Deploy Infrastructure Using Bicep

#### Development Environment

```bash
cd infrastructure/bicep

# Validate template
az bicep build --file main.bicep

# Deploy to dev
az deployment group create \
  --resource-group fitnessapp-dev-rg \
  --template-file main.bicep \
  --parameters environment=dev \
               sqlAdminUsername=<your-username> \
               sqlAdminPassword=<your-password> \
               jwtSecret=<your-jwt-secret>
```

#### Production Environment

```bash
# Deploy to prod
az deployment group create \
  --resource-group fitnessapp-prod-rg \
  --template-file main.bicep \
  --parameters environment=prod \
               sqlAdminUsername=<your-username> \
               sqlAdminPassword=<your-password> \
               jwtSecret=<your-jwt-secret>
```

### 4. Retrieve Deployment Outputs

```bash
# Get deployment outputs
az deployment group show \
  --resource-group fitnessapp-dev-rg \
  --name main \
  --query properties.outputs

# Important outputs:
# - containerRegistryLoginServer
# - backendUrl
# - frontendUrl
# - keyVaultUri
# - appInsightsInstrumentationKey
```

---

## GitHub Secrets Configuration

Configure the following secrets in your GitHub repository:

### Repository Secrets

Navigate to: `Settings > Secrets and variables > Actions > New repository secret`

#### Azure Credentials

- **AZURE_CREDENTIALS**: JSON output from service principal creation
  ```json
  {
    "clientId": "...",
    "clientSecret": "...",
    "subscriptionId": "...",
    "tenantId": "..."
  }
  ```

#### Resource Groups

- **AZURE_RESOURCE_GROUP_DEV**: `fitnessapp-dev-rg`
- **AZURE_RESOURCE_GROUP_PROD**: `fitnessapp-prod-rg`

#### Container Registry

- **AZURE_CONTAINER_REGISTRY**: Registry login server (e.g., `fitnessappdev123.azurecr.io`)
- **AZURE_CONTAINER_REGISTRY_USERNAME**: Get from ACR access keys
- **AZURE_CONTAINER_REGISTRY_PASSWORD**: Get from ACR access keys

```bash
# Retrieve ACR credentials
az acr credential show --name <registry-name>
```

#### Backend Deployment

- **AZURE_CONTAINER_APP_NAME**: Backend container app name (e.g., `fitnessapp-dev-backend`)

#### Frontend Deployment

- **AZURE_STATIC_WEB_APPS_API_TOKEN**: Get from Static Web App deployment token
- **NEXT_PUBLIC_API_URL**: Backend API URL (e.g., `https://fitnessapp-dev-backend.azurecontainerapps.io`)

```bash
# Retrieve Static Web App deployment token
az staticwebapp secrets list \
  --name <static-web-app-name> \
  --resource-group <resource-group> \
  --query properties.apiKey -o tsv
```

#### Database and Security

- **SQL_ADMIN_USERNAME**: SQL Server administrator username
- **SQL_ADMIN_PASSWORD**: SQL Server administrator password (strong password)
- **JWT_SECRET**: JWT signing key (min 32 characters, cryptographically secure)

```bash
# Generate secure JWT secret
openssl rand -base64 32
```

---

## CI/CD Pipeline

### Workflow Overview

The application uses three main GitHub Actions workflows:

#### 1. Backend CI/CD (`backend-ci-cd.yml`)

**Triggers:**
- Push to `main` branch (backend changes)
- Pull requests affecting backend
- Manual workflow dispatch

**Jobs:**
1. **Build and Test**: Compile .NET code, run unit tests
2. **Build Docker**: Create container image, push to ACR
3. **Deploy to Azure**: Deploy to Azure Container Apps, verify health

#### 2. Frontend CI/CD (`frontend-ci-cd.yml`)

**Triggers:**
- Push to `main` branch (frontend changes)
- Pull requests affecting frontend
- Manual workflow dispatch

**Jobs:**
1. **Build and Test**: Lint, test, build Next.js app
2. **Deploy to Azure**: Deploy to Static Web Apps, verify deployment

#### 3. Infrastructure Deployment (`infrastructure-deploy.yml`)

**Triggers:**
- Push to `main` branch (infrastructure changes)
- Manual workflow dispatch with environment selection

**Jobs:**
1. **Validate**: Validate Bicep templates
2. **Deploy**: Deploy infrastructure to selected environment

#### 4. PR Validation (`pr-validation.yml`)

**Triggers:**
- Pull requests to `main` or `develop`

**Jobs:**
- Validate backend code and Docker build
- Validate frontend code and Docker build
- Validate infrastructure templates
- Validate docker-compose configuration
- Run security scans with Trivy

### Manual Workflow Execution

```bash
# Trigger backend deployment
gh workflow run backend-ci-cd.yml

# Trigger frontend deployment
gh workflow run frontend-ci-cd.yml

# Trigger infrastructure deployment (dev)
gh workflow run infrastructure-deploy.yml -f environment=dev

# Trigger infrastructure deployment (prod)
gh workflow run infrastructure-deploy.yml -f environment=prod
```

---

## Manual Deployment

### Push Docker Images to ACR

```bash
# Login to ACR
az acr login --name <registry-name>

# Tag images
docker tag fitnessapp-backend:local <registry>.azurecr.io/fitnessapp-backend:latest
docker tag fitnessapp-frontend:local <registry>.azurecr.io/fitnessapp-frontend:latest

# Push images
docker push <registry>.azurecr.io/fitnessapp-backend:latest
docker push <registry>.azurecr.io/fitnessapp-frontend:latest
```

### Update Container App

```bash
# Update backend container app
az containerapp update \
  --name fitnessapp-dev-backend \
  --resource-group fitnessapp-dev-rg \
  --image <registry>.azurecr.io/fitnessapp-backend:latest
```

### Run Database Migrations

Database migrations run automatically on backend startup. To run manually:

```bash
# Connect to container app
az containerapp exec \
  --name fitnessapp-dev-backend \
  --resource-group fitnessapp-dev-rg \
  --command /bin/sh

# Inside container
dotnet ef database update
```

---

## Monitoring and Logging

### Application Insights

1. **Access Application Insights**
   ```bash
   # Get Application Insights connection string
   az monitor app-insights component show \
     --app <app-insights-name> \
     --resource-group <resource-group> \
     --query connectionString -o tsv
   ```

2. **View in Azure Portal**
   - Navigate to Application Insights resource
   - View Live Metrics, Failures, Performance, Logs

### Container App Logs

```bash
# Stream backend logs
az containerapp logs show \
  --name fitnessapp-dev-backend \
  --resource-group fitnessapp-dev-rg \
  --follow

# View recent logs
az containerapp logs show \
  --name fitnessapp-dev-backend \
  --resource-group fitnessapp-dev-rg \
  --tail 100
```

### SQL Database Monitoring

```bash
# View database metrics
az sql db show \
  --name fitnessapp \
  --server <sql-server-name> \
  --resource-group <resource-group>
```

### Health Checks

- **Backend Health**: `https://<backend-url>/health`
- **Frontend Health**: `https://<frontend-url>/api/health`

---

## Domain and SSL Configuration

### Custom Domain Setup

#### For Static Web App (Frontend)

1. **Add Custom Domain**
   ```bash
   az staticwebapp hostname set \
     --name <static-web-app-name> \
     --resource-group <resource-group> \
     --hostname www.yourdomain.com
   ```

2. **Configure DNS Records**
   - Add CNAME record: `www` → `<app-name>.azurestaticapps.net`
   - For apex domain, add TXT record for validation

#### For Container App (Backend)

1. **Add Custom Domain**
   ```bash
   az containerapp hostname add \
     --name <container-app-name> \
     --resource-group <resource-group> \
     --hostname api.yourdomain.com
   ```

2. **Configure DNS Records**
   - Add CNAME record: `api` → `<app>.azurecontainerapps.io`

### SSL/TLS Certificates

Azure automatically provisions and manages SSL certificates for:
- Static Web Apps (free, auto-renewing)
- Container Apps (free, auto-renewing)

For custom certificates:
```bash
az containerapp ssl upload \
  --name <container-app-name> \
  --resource-group <resource-group> \
  --certificate-file cert.pfx \
  --password <cert-password>
```

---

## Troubleshooting

### Common Issues

#### 1. Docker Build Fails

```bash
# Clean Docker cache
docker system prune -a

# Rebuild without cache
docker build --no-cache -t fitnessapp-backend:local .
```

#### 2. Container App Won't Start

```bash
# Check logs
az containerapp logs show \
  --name <app-name> \
  --resource-group <resource-group> \
  --tail 100

# Check environment variables
az containerapp show \
  --name <app-name> \
  --resource-group <resource-group> \
  --query properties.template.containers[0].env
```

#### 3. Database Connection Issues

- Verify firewall rules allow Container App IP
- Check connection string format
- Ensure database migrations ran successfully
- Verify SQL admin credentials

#### 4. GitHub Actions Failing

- Verify all secrets are configured correctly
- Check Azure service principal has required permissions
- Review workflow run logs for specific errors
- Ensure Bicep templates are valid: `az bicep build --file main.bicep`

#### 5. Health Check Failures

- Verify application is listening on correct port
- Check ASPNETCORE_URLS environment variable
- Review application logs for startup errors
- Ensure database is accessible

### Support Resources

- **Azure Documentation**: https://docs.microsoft.com/azure
- **Bicep Documentation**: https://docs.microsoft.com/azure/azure-resource-manager/bicep
- **GitHub Actions**: https://docs.github.com/actions
- **Application Insights**: https://docs.microsoft.com/azure/azure-monitor/app/app-insights-overview

---

## Next Steps

1. Configure custom domains for production
2. Set up automated backups for SQL Database
3. Implement Azure Front Door for global distribution
4. Configure Azure Key Vault for enhanced security
5. Set up monitoring alerts and dashboards
6. Implement blue-green deployment strategy
7. Configure auto-scaling rules based on metrics
