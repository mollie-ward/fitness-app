# Task: Infrastructure & Deployment Scaffolding

**Task ID:** 003  
**GitHub Issue:** [#5](https://github.com/mollie-ward/fitness-app/issues/5)  
**Feature:** Infrastructure  
**Priority:** P0 (Critical)  
**Estimated Effort:** Medium  

---

## Description

Set up deployment infrastructure, CI/CD pipelines, container configuration, and Azure resource definitions. Prepare the application for deployment to Azure using Infrastructure as Code (Bicep) and GitHub Actions.

---

## Dependencies

- **Task 001:** Backend scaffolding must be complete
- **Task 002:** Frontend scaffolding must be complete

---

## Technical Requirements

### Containerization
- Create Dockerfile for backend API
  - Multi-stage build for optimization
  - Include .NET runtime
  - Configure health check
  - Set proper working directory and entry point
- Create Dockerfile for frontend (if separate deployment)
  - Node.js base image
  - Build Next.js production bundle
  - Serve static files efficiently
- Create docker-compose.yml for local development
  - Backend service
  - Frontend service
  - Database service
  - Configuration for networking between services

### Azure Infrastructure (Bicep/ARM)
- Define Azure resource templates:
  - Azure App Service or Container Apps for backend
  - Azure Static Web Apps or App Service for frontend
  - Azure SQL Database or Cosmos DB
  - Azure Key Vault for secrets
  - Application Insights for monitoring
  - Azure Container Registry (if using containers)
- Parameterize templates for different environments (dev, staging, prod)
- Configure managed identities for secure service-to-service communication
- Set up virtual network integration if required

### CI/CD Pipeline (GitHub Actions)
- Create workflow for backend:
  - Build and test .NET API
  - Run code quality checks
  - Build Docker image
  - Push to container registry
  - Deploy to Azure
- Create workflow for frontend:
  - Build and test Next.js app
  - Run linting and type checking
  - Build production bundle
  - Deploy to Azure Static Web Apps or App Service
- Configure separate workflows for PR validation
- Set up environment-specific deployment workflows (dev, prod)

### Environment Configuration
- Set up GitHub Secrets for sensitive values:
  - Azure credentials
  - Database connection strings
  - API keys for external services
  - JWT signing keys
- Configure environment variables in Azure App Service/Container Apps
- Set up Azure Key Vault integration for secrets management

### Monitoring & Logging
- Configure Application Insights instrumentation
- Set up log aggregation from containers
- Configure alerts for critical errors
- Set up availability monitoring (ping tests)

### Domain & SSL
- Document custom domain configuration steps
- Prepare SSL/TLS certificate setup (Azure-managed or custom)
- Configure DNS records (to be executed later)

---

## Acceptance Criteria

- ✅ Docker containers build successfully for both backend and frontend
- ✅ docker-compose runs full stack locally (backend + frontend + database)
- ✅ Bicep templates validate successfully with `az bicep build`
- ✅ Infrastructure can be deployed to Azure using Bicep
- ✅ GitHub Actions workflows execute successfully on push to main
- ✅ Backend deploys to Azure and health check endpoint returns 200
- ✅ Frontend deploys to Azure and root page loads successfully
- ✅ Application Insights receives telemetry from deployed services
- ✅ Environment variables load correctly in deployed environments
- ✅ Database connection works from deployed backend service

---

## Testing Requirements

### Infrastructure Tests
- Verify Bicep templates are syntactically correct
- Validate docker-compose services start successfully
- Test database migrations run successfully in containerized environment
- **Minimum coverage:** Not applicable (infrastructure validation)

### Deployment Tests
- Verify GitHub Actions workflows complete without errors
- Test deployed backend API responds to health checks
- Test deployed frontend serves pages correctly
- Verify Application Insights receives logs and metrics
- **Minimum coverage:** Not applicable (deployment validation)

### Security Tests
- Verify secrets are not exposed in logs or source code
- Validate managed identities work correctly
- Test HTTPS enforcement on deployed services
- Verify database is not publicly accessible

---

## Definition of Done

- All acceptance criteria met
- Docker containers run locally and in Azure
- CI/CD pipelines deploy successfully to dev environment
- Infrastructure as Code is in source control
- Documentation updated in `/docs` with deployment guide
- Secrets are stored securely in Azure Key Vault or GitHub Secrets
- Monitoring dashboards are accessible in Azure Portal
- No credentials or secrets committed to repository
