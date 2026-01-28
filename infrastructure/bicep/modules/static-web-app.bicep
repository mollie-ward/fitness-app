// Azure Static Web App module
@description('Static Web App name')
param name string

@description('Location for the resource')
param location string

@description('SKU for Static Web App')
@allowed([
  'Free'
  'Standard'
])
param sku string = 'Free'

@description('Backend API URL')
param backendApiUrl string

resource staticWebApp 'Microsoft.Web/staticSites@2023-01-01' = {
  name: name
  location: location
  sku: {
    name: sku
    tier: sku
  }
  properties: {
    buildProperties: {
      skipGithubActionWorkflowGeneration: true
    }
    stagingEnvironmentPolicy: 'Enabled'
    allowConfigFileUpdates: true
  }
}

// Configure backend API as linked backend
resource apiBackend 'Microsoft.Web/staticSites/linkedBackends@2023-01-01' = {
  parent: staticWebApp
  name: 'backend'
  properties: {
    backendResourceId: backendApiUrl
    region: location
  }
}

output id string = staticWebApp.id
output name string = staticWebApp.name
output defaultHostname string = staticWebApp.properties.defaultHostname
output apiKey string = staticWebApp.listSecrets().properties.apiKey
