// Main Bicep template for Fitness App infrastructure
targetScope = 'resourceGroup'

@description('Environment name (dev, staging, prod)')
@allowed([
  'dev'
  'staging'
  'prod'
])
param environment string

@description('Azure region for resources')
param location string = resourceGroup().location

@description('Base name for resources')
param appName string = 'fitnessapp'

@description('SQL Admin username')
@secure()
param sqlAdminUsername string

@description('SQL Admin password')
@secure()
param sqlAdminPassword string

@description('JWT Secret for API')
@secure()
param jwtSecret string

// Variables
var uniqueSuffix = uniqueString(resourceGroup().id)
var resourceNamePrefix = '${appName}-${environment}'

// Module: Azure Container Registry
module containerRegistry 'modules/container-registry.bicep' = {
  name: 'containerRegistryDeploy'
  params: {
    name: '${appName}${environment}${uniqueSuffix}'
    location: location
    sku: environment == 'prod' ? 'Premium' : 'Basic'
  }
}

// Module: Application Insights
module appInsights 'modules/application-insights.bicep' = {
  name: 'appInsightsDeploy'
  params: {
    name: '${resourceNamePrefix}-insights'
    location: location
  }
}

// Module: Key Vault
module keyVault 'modules/key-vault.bicep' = {
  name: 'keyVaultDeploy'
  params: {
    name: '${appName}-${environment}-${uniqueSuffix}'
    location: location
    tenantId: subscription().tenantId
  }
}

// Module: Azure SQL Database
module sqlDatabase 'modules/sql-database.bicep' = {
  name: 'sqlDatabaseDeploy'
  params: {
    name: '${resourceNamePrefix}-sql'
    location: location
    administratorLogin: sqlAdminUsername
    administratorLoginPassword: sqlAdminPassword
    databaseName: 'fitnessapp'
    sku: environment == 'prod' ? 'S1' : 'Basic'
  }
}

// Module: Container App Environment
module containerAppEnv 'modules/container-app-environment.bicep' = {
  name: 'containerAppEnvDeploy'
  params: {
    name: '${resourceNamePrefix}-env'
    location: location
    appInsightsInstrumentationKey: appInsights.outputs.instrumentationKey
  }
}

// Module: Backend Container App
module backendContainerApp 'modules/backend-container-app.bicep' = {
  name: 'backendContainerAppDeploy'
  params: {
    name: '${resourceNamePrefix}-backend'
    location: location
    containerAppEnvironmentId: containerAppEnv.outputs.id
    containerRegistryName: containerRegistry.outputs.name
    containerImage: '${containerRegistry.outputs.loginServer}/fitnessapp-backend:latest'
    sqlConnectionString: 'Server=tcp:${sqlDatabase.outputs.fqdn},1433;Initial Catalog=fitnessapp;Persist Security Info=False;User ID=${sqlAdminUsername};Password=${sqlAdminPassword};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;'
    jwtSecret: jwtSecret
    appInsightsConnectionString: appInsights.outputs.connectionString
    keyVaultName: keyVault.outputs.name
  }
  dependsOn: [
    containerRegistry
    sqlDatabase
    keyVault
  ]
}

// Module: Frontend Static Web App
module frontendStaticWebApp 'modules/static-web-app.bicep' = {
  name: 'frontendStaticWebAppDeploy'
  params: {
    name: '${resourceNamePrefix}-frontend'
    location: location
    sku: environment == 'prod' ? 'Standard' : 'Free'
    backendApiUrl: backendContainerApp.outputs.fqdn
  }
}

// Outputs
output containerRegistryLoginServer string = containerRegistry.outputs.loginServer
output containerRegistryName string = containerRegistry.outputs.name
output backendUrl string = backendContainerApp.outputs.fqdn
output frontendUrl string = frontendStaticWebApp.outputs.defaultHostname
output sqlServerFqdn string = sqlDatabase.outputs.fqdn
output keyVaultUri string = keyVault.outputs.uri
output appInsightsInstrumentationKey string = appInsights.outputs.instrumentationKey
output appInsightsConnectionString string = appInsights.outputs.connectionString
