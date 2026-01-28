// Azure Container App Environment module
@description('Container App Environment name')
param name string

@description('Location for the resource')
param location string

@description('Application Insights Instrumentation Key')
param appInsightsInstrumentationKey string

resource containerAppEnvironment 'Microsoft.App/managedEnvironments@2023-05-01' = {
  name: name
  location: location
  properties: {
    appLogsConfiguration: {
      destination: 'azure-monitor'
    }
    daprAIInstrumentationKey: appInsightsInstrumentationKey
  }
}

output id string = containerAppEnvironment.id
output name string = containerAppEnvironment.name
output defaultDomain string = containerAppEnvironment.properties.defaultDomain
