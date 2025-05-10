data "azurerm_resource_group" "rg" {
  name = var.rg_name
}

resource "azurerm_key_vault" "kv" {
  name                       = var.kv_name
  location                   = data.azurerm_resource_group.rg.location
  resource_group_name        = data.azurerm_resource_group.rg.name
  tenant_id                  = var.tenant_id
  sku_name                   = "standard"
  purge_protection_enabled   = true
  soft_delete_retention_days = 7
}

resource "azurerm_log_analytics_workspace" "law" {
  name                = var.law_name
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  sku                 = "PerGB2018"
  retention_in_days   = 30
}

resource "azurerm_application_insights" "ai" {
  name                = var.ai_name
  location            = data.azurerm_resource_group.rg.location
  resource_group_name = data.azurerm_resource_group.rg.name
  application_type    = "web"
  workspace_id        = azurerm_log_analytics_workspace.law.id
  retention_in_days   = 30
}

resource "azurerm_key_vault_secret" "ai_connection_string" {
  name         = "ApplicationInsights--ConnectionString"
  value        = azurerm_application_insights.ai.connection_string
  key_vault_id = azurerm_key_vault.kv.id
}

resource "azurerm_container_app_environment" "ca_env" {
  name                       = var.ca_env_name
  location                   = data.azurerm_resource_group.rg.location
  resource_group_name        = data.azurerm_resource_group.rg.name
  log_analytics_workspace_id = azurerm_log_analytics_workspace.law.id
}

resource "azurerm_container_app" "ca_app" {
  name                         = var.ca_app_name
  resource_group_name          = data.azurerm_resource_group.rg.name
  container_app_environment_id = azurerm_container_app_environment.ca_env.id
  revision_mode                = "Single"

  identity {
    type = "SystemAssigned"
  }

  ingress {
    external_enabled = true
    target_port      = var.container_port

    traffic_weight {
      percentage      = 100
      latest_revision = true
    }
  }

  registry {
    server               = var.registry_server
    username             = var.registry_username
    password_secret_name = "registry-pwd"
  }

  secret {
    name  = "registry-pwd"
    value = var.registry_password
  }

  template {
    min_replicas = 0
    max_replicas = 1

    container {
      name   = var.ca_app_name
      image  = var.registry_image
      cpu    = 0.25
      memory = "0.5Gi"

      env {
        name  = "ASPNETCORE_ENVIRONMENT"
        value = var.environment
      }

      env {
        name  = "KEY_VAULT_NAME"
        value = var.kv_name
      }

      liveness_probe {
        transport               = "HTTP"
        port                    = var.container_port
        path                    = "/health"
        interval_seconds        = 10
        timeout                 = 5
        failure_count_threshold = 3
      }
    }

    http_scale_rule {
      name                = "http-scaler"
      concurrent_requests = 10
    }
  }
}

resource "azurerm_key_vault_access_policy" "ca_app_kv_policy" {
  key_vault_id = azurerm_key_vault.kv.id
  tenant_id    = var.tenant_id
  object_id    = azurerm_container_app.ca_app.identity[0].principal_id

  secret_permissions = [
    "List",
    "Get"
  ]

  depends_on = [azurerm_container_app.ca_app]
}
