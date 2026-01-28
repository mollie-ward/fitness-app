# Infrastructure as Code - Fitness App

This directory contains the Azure infrastructure definitions using Bicep templates for deploying the Fitness App.

## Directory Structure

```
infrastructure/
├── bicep/
│   ├── main.bicep                    # Main orchestration template
│   └── modules/
│       ├── application-insights.bicep # Application Insights and Log Analytics
│       ├── backend-container-app.bicep # Backend API Container App
│       ├── container-app-environment.bicep # Container App Environment
│       ├── container-registry.bicep   # Azure Container Registry
│       ├── key-vault.bicep           # Azure Key Vault
│       ├── sql-database.bicep        # Azure SQL Database
│       └── static-web-app.bicep      # Frontend Static Web App
├── parameters/
│   ├── dev.parameters.json           # Development environment parameters
│   └── prod.parameters.json          # Production environment parameters
└── README.md                         # This file
```

## Resources Deployed

### Core Services

1. **Azure Container Registry**
   - Stores Docker images for backend API
   - SKU: Basic (dev), Premium (prod)
   - Admin user enabled for initial deployment

2. **Azure Container Apps**
   - Runs backend .NET API
   - Auto-scaling: 1-3 replicas
   - Health checks: Liveness and Readiness probes
   - Managed identity for secure service access

3. **Azure Static Web Apps**
   - Hosts Next.js frontend
   - SKU: Free (dev), Standard (prod)
   - Integrated with GitHub for CI/CD

4. **Azure SQL Database**
   - Microsoft SQL Server database
   - SKU: Basic (dev), S1 (prod)
   - Automatic backups enabled

5. **Azure Key Vault**
   - Stores secrets and connection strings
   - RBAC-based access control
   - Soft delete enabled

6. **Application Insights**
   - Application monitoring and logging
   - Integrated with Log Analytics workspace
   - 30-day log retention

### Networking

- Container App Environment with built-in networking
- Azure SQL Database with Azure service access
- Static Web App with custom domain support

## Prerequisites

1. Azure CLI with Bicep extension
   ```bash
   az bicep install
   ```

2. Azure subscription with appropriate permissions

3. Service Principal for automated deployments

## Deployment

### Quick Deploy

```bash
# Login to Azure
az login

# Set subscription
az account set --subscription <subscription-id>

# Create resource group
az group create --name fitnessapp-dev-rg --location eastus

# Deploy infrastructure
az deployment group create \
  --resource-group fitnessapp-dev-rg \
  --template-file bicep/main.bicep \
  --parameters environment=dev \
               sqlAdminUsername=<username> \
               sqlAdminPassword=<password> \
               jwtSecret=<secret>
```

### Validate Before Deploy

```bash
# Validate Bicep syntax
az bicep build --file bicep/main.bicep

# What-if analysis
az deployment group what-if \
  --resource-group fitnessapp-dev-rg \
  --template-file bicep/main.bicep \
  --parameters environment=dev
```

### Deploy with Parameter File

```bash
# Update parameter file with your values
# Edit parameters/dev.parameters.json

# Deploy using parameter file
az deployment group create \
  --resource-group fitnessapp-dev-rg \
  --template-file bicep/main.bicep \
  --parameters parameters/dev.parameters.json
```

## Outputs

After successful deployment, the following outputs are available:

- `containerRegistryLoginServer`: ACR login server URL
- `backendUrl`: Backend API URL
- `frontendUrl`: Frontend application URL
- `sqlServerFqdn`: SQL Server FQDN
- `keyVaultUri`: Key Vault URI
- `appInsightsInstrumentationKey`: Application Insights key
- `appInsightsConnectionString`: Application Insights connection string

Retrieve outputs:

```bash
az deployment group show \
  --resource-group fitnessapp-dev-rg \
  --name main \
  --query properties.outputs
```

## Environment-Specific Configuration

### Development (dev)

- Basic/smaller SKUs for cost savings
- Relaxed security policies
- Development-friendly settings

### Production (prod)

- Premium/larger SKUs for performance
- Enhanced security and compliance
- High availability configuration
- Geo-redundant backups

## Security Considerations

### Secrets Management

- All secrets stored in Azure Key Vault
- Managed identities for service-to-service authentication
- No credentials in template files or source control

### Network Security

- SQL Database accessible only from Azure services
- Container Apps with ingress controls
- Key Vault with RBAC authorization

### Compliance

- Soft delete enabled on Key Vault
- SQL Database audit logging
- Application Insights for monitoring and compliance

## Cost Estimation

### Development Environment (~$50-100/month)

- Container Registry (Basic): ~$5/month
- Container Apps: ~$15-30/month
- Static Web App (Free): $0
- SQL Database (Basic): ~$5/month
- Application Insights: ~$5-10/month
- Key Vault: ~$1/month

### Production Environment (~$200-500/month)

- Container Registry (Premium): ~$50/month
- Container Apps: ~$50-150/month
- Static Web App (Standard): ~$9/month
- SQL Database (S1): ~$30/month
- Application Insights: ~$20-50/month
- Key Vault: ~$1/month

*Costs vary based on usage, region, and scaling*

## Maintenance

### Update Infrastructure

```bash
# Pull latest changes
git pull

# Deploy updates
az deployment group create \
  --resource-group fitnessapp-dev-rg \
  --template-file bicep/main.bicep \
  --parameters environment=dev
```

### Backup and Disaster Recovery

- SQL Database: Automated backups (7-35 days)
- Container Apps: Stateless, easily recreated
- Static Web App: Content in source control
- Key Vault: Soft delete and purge protection

### Monitoring

- Application Insights dashboards
- Azure Monitor alerts
- Log Analytics queries
- Health check endpoints

## Troubleshooting

### Common Issues

1. **Deployment Fails**: Check Azure quotas and limits
2. **Name Conflicts**: Use unique resource name prefix
3. **Permission Errors**: Verify service principal roles
4. **Validation Errors**: Run `az bicep build` first

### Support

For deployment issues:
1. Check Azure Activity Log
2. Review deployment error details
3. Validate Bicep templates locally
4. Check parameter values

## CI/CD Integration

This infrastructure is deployed via GitHub Actions workflows:

- **Manual**: `infrastructure-deploy.yml` workflow
- **Automatic**: On push to infrastructure changes
- **Validation**: On pull requests

See `.github/workflows/` for workflow definitions.

## Next Steps

1. Configure custom domains
2. Set up monitoring alerts
3. Enable geo-replication (prod)
4. Configure backup policies
5. Implement disaster recovery plan
6. Set up staging environments

## References

- [Azure Bicep Documentation](https://docs.microsoft.com/azure/azure-resource-manager/bicep)
- [Azure Container Apps](https://docs.microsoft.com/azure/container-apps)
- [Azure Static Web Apps](https://docs.microsoft.com/azure/static-web-apps)
- [Deployment Guide](../docs/deployment-guide.md)
