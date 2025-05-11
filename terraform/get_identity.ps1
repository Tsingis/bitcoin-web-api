param (
    [string]$resourceGroup,
    [string]$appName
)

$principalId = az containerapp show `
    --name $appName `
    --resource-group $resourceGroup `
    --query "identity.principalId" `
    -o tsv

@{ principal_id = $principalId } | ConvertTo-Json -Compres
