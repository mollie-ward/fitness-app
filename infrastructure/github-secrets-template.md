# GitHub Secrets Configuration Template

This file documents all required GitHub Secrets for CI/CD pipelines. **DO NOT** commit actual secret values.

## Required Secrets

### Azure Authentication

#### AZURE_CREDENTIALS
Service Principal JSON for GitHub Actions authentication to Azure.

**How to create:**
```bash
az ad sp create-for-rbac \
  --name "fitnessapp-github-actions" \
  --role contributor \
  --scopes /subscriptions/{subscription-id} \
  --sdk-auth
```

**Format:**
```json
{
  "clientId": "<client-id>",
  "clientSecret": "<client-secret>",
  "subscriptionId": "<subscription-id>",
  "tenantId": "<tenant-id>"
}
```

### Resource Groups

#### AZURE_RESOURCE_GROUP_DEV
- **Value:** `fitnessapp-dev-rg`
- **Description:** Development environment resource group name

#### AZURE_RESOURCE_GROUP_PROD
- **Value:** `fitnessapp-prod-rg`
- **Description:** Production environment resource group name

### Container Registry

#### AZURE_CONTAINER_REGISTRY
- **Value:** `<registry-name>.azurecr.io` (e.g., `fitnessappdev123.azurecr.io`)
- **Description:** Container Registry login server

**How to get:**
```bash
az acr list --query "[].{name:name, loginServer:loginServer}" -o table
```

#### AZURE_CONTAINER_REGISTRY_USERNAME
- **Value:** Admin username from ACR
- **Description:** Container Registry admin username

**How to get:**
```bash
az acr credential show --name <registry-name> --query username -o tsv
```

#### AZURE_CONTAINER_REGISTRY_PASSWORD
- **Value:** Admin password from ACR
- **Description:** Container Registry admin password

**How to get:**
```bash
az acr credential show --name <registry-name> --query "passwords[0].value" -o tsv
```

### Backend Deployment

#### AZURE_CONTAINER_APP_NAME
- **Value:** `fitnessapp-dev-backend` or `fitnessapp-prod-backend`
- **Description:** Backend Container App name

**How to get:**
```bash
az containerapp list --query "[].{name:name}" -o table
```

### Frontend Deployment

#### AZURE_STATIC_WEB_APPS_API_TOKEN
- **Value:** Deployment token from Static Web App
- **Description:** Static Web App deployment token for GitHub Actions

**How to get:**
```bash
az staticwebapp secrets list \
  --name <static-web-app-name> \
  --resource-group <resource-group> \
  --query properties.apiKey -o tsv
```

#### NEXT_PUBLIC_API_URL
- **Value:** `https://<backend-url>` (e.g., `https://fitnessapp-dev-backend.azurecontainerapps.io`)
- **Description:** Backend API URL for frontend

**How to get:**
```bash
az containerapp show \
  --name <app-name> \
  --resource-group <resource-group> \
  --query properties.configuration.ingress.fqdn -o tsv
```

### Database Configuration

#### SQL_ADMIN_USERNAME
- **Value:** SQL Server administrator username (e.g., `sqladmin`)
- **Description:** Azure SQL Database administrator username
- **Requirements:** 
  - Cannot be 'admin', 'administrator', 'sa', 'root'
  - 1-128 characters

#### SQL_ADMIN_PASSWORD
- **Value:** Strong password for SQL Server
- **Description:** Azure SQL Database administrator password
- **Requirements:**
  - Minimum 8 characters
  - Contains uppercase, lowercase, numbers, and special characters
  - Cannot contain username

**How to generate:**
```bash
# Generate strong password
openssl rand -base64 24
```

### Security Configuration

#### JWT_SECRET
- **Value:** Cryptographically secure random string (minimum 32 characters)
- **Description:** Secret key for JWT token signing
- **Requirements:** Minimum 32 characters

**How to generate:**
```bash
# Generate secure JWT secret
openssl rand -base64 32
```

## Setting Secrets in GitHub

### Via GitHub UI

1. Navigate to repository: `https://github.com/mollie-ward/fitness-app`
2. Go to: `Settings` → `Secrets and variables` → `Actions`
3. Click: `New repository secret`
4. Enter name and value
5. Click: `Add secret`

### Via GitHub CLI

```bash
# Set a secret
gh secret set AZURE_CREDENTIALS < azure-credentials.json

# Set from stdin
echo "your-secret-value" | gh secret set SECRET_NAME

# List all secrets
gh secret list
```

## Verification Checklist

Before running workflows, verify all secrets are configured:

- [ ] AZURE_CREDENTIALS
- [ ] AZURE_RESOURCE_GROUP_DEV
- [ ] AZURE_RESOURCE_GROUP_PROD
- [ ] AZURE_CONTAINER_REGISTRY
- [ ] AZURE_CONTAINER_REGISTRY_USERNAME
- [ ] AZURE_CONTAINER_REGISTRY_PASSWORD
- [ ] AZURE_CONTAINER_APP_NAME
- [ ] AZURE_STATIC_WEB_APPS_API_TOKEN
- [ ] NEXT_PUBLIC_API_URL
- [ ] SQL_ADMIN_USERNAME
- [ ] SQL_ADMIN_PASSWORD
- [ ] JWT_SECRET

## Security Best Practices

1. **Rotate Secrets Regularly**
   - Service principal credentials: Every 90 days
   - Database passwords: Every 90 days
   - JWT secrets: Every 6 months

2. **Use Azure Key Vault**
   - Store production secrets in Key Vault
   - Reference Key Vault secrets in parameter files
   - Use managed identities when possible

3. **Least Privilege**
   - Service Principal should have minimum required permissions
   - Use separate service principals for dev/prod if possible

4. **Audit Access**
   - Monitor secret usage in GitHub Actions logs
   - Review Azure Activity Log for suspicious access

5. **Never Commit Secrets**
   - Use `.gitignore` for sensitive files
   - Scan commits for exposed secrets
   - Revoke and rotate if secrets are exposed

## Troubleshooting

### Secret Not Found Error

**Problem:** Workflow fails with "Secret not found"

**Solution:**
1. Verify secret name matches exactly (case-sensitive)
2. Check secret is in correct repository
3. Ensure secret is not expired

### Invalid Credentials Error

**Problem:** Azure login fails

**Solution:**
1. Verify AZURE_CREDENTIALS JSON format is correct
2. Ensure service principal has required permissions
3. Check if service principal secret is expired

### Connection String Errors

**Problem:** Application can't connect to database

**Solution:**
1. Verify SQL_ADMIN_USERNAME and SQL_ADMIN_PASSWORD are correct
2. Check SQL Server firewall rules
3. Ensure connection string format is correct

## Support

For issues with secrets configuration:
1. Review GitHub Actions workflow logs
2. Check Azure Portal for resource details
3. Verify service principal permissions
4. Contact DevOps team for assistance

## References

- [GitHub Encrypted Secrets](https://docs.github.com/en/actions/security-guides/encrypted-secrets)
- [Azure Service Principals](https://docs.microsoft.com/azure/active-directory/develop/app-objects-and-service-principals)
- [Azure Key Vault](https://docs.microsoft.com/azure/key-vault)
