resource "azurerm_key_vault" "kv" {
  name                       = var.kv_name
  location                   = data.azurerm_resource_group.rg.location
  resource_group_name        = data.azurerm_resource_group.rg.name
  tenant_id                  = var.tenant_id
  sku_name                   = "standard"
  purge_protection_enabled   = true
  soft_delete_retention_days = 7
}

resource "azurerm_key_vault_secret" "ai_connection_string" {
  name         = "ApplicationInsights--ConnectionString"
  value        = azurerm_application_insights.ai.connection_string
  key_vault_id = azurerm_key_vault.kv.id
}
