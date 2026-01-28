# Bicep Parameter Files

This directory contains example parameter files for deploying infrastructure to different environments.

## Usage

1. **Copy the example file** for your environment:
   ```bash
   cp dev.parameters.example.json dev.parameters.json
   cp prod.parameters.example.json prod.parameters.json
   ```

2. **Update the parameter values**:
   - Replace `{subscription-id}` with your Azure subscription ID
   - Replace `{rg-name}` with your Key Vault resource group name
   - Replace `{vault-name}` with your Key Vault name
   - Store secrets in Azure Key Vault before deployment

3. **Deploy using parameter file**:
   ```bash
   az deployment group create \
     --resource-group fitnessapp-dev-rg \
     --template-file ../bicep/main.bicep \
     --parameters dev.parameters.json
   ```

## Important Notes

- **DO NOT commit actual parameter files** (*.parameters.json) with real values
- The `.gitignore` file excludes `*.parameters.json` to prevent accidental commits
- Only commit `.example.json` files as templates
- Always use Azure Key Vault references for secrets in parameter files
- For CI/CD, pass parameters directly via GitHub Actions secrets

## Alternative: Pass Parameters Inline

Instead of using parameter files, you can pass parameters directly:

```bash
az deployment group create \
  --resource-group fitnessapp-dev-rg \
  --template-file ../bicep/main.bicep \
  --parameters environment=dev \
               sqlAdminUsername=sqladmin \
               sqlAdminPassword='YourSecurePassword!' \
               jwtSecret='your-secure-jwt-secret-32-chars'
```

## Security Best Practices

1. Never commit actual secrets to source control
2. Use Azure Key Vault for storing secrets
3. Reference Key Vault secrets in parameter files
4. Use different secrets for dev/staging/prod
5. Rotate secrets regularly
