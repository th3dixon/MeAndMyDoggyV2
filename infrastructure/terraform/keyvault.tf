# =============================================================================
# Azure Key Vault Configuration for MeAndMyDoggyV2
# =============================================================================

# Data sources
data "azurerm_client_config" "current" {}

# Resource Group (if not exists)
resource "azurerm_resource_group" "main" {
  count    = var.create_resource_group ? 1 : 0
  name     = var.resource_group_name
  location = var.location

  tags = var.tags
}

data "azurerm_resource_group" "existing" {
  count = var.create_resource_group ? 0 : 1
  name  = var.resource_group_name
}

locals {
  resource_group_name = var.create_resource_group ? azurerm_resource_group.main[0].name : data.azurerm_resource_group.existing[0].name
  resource_group_location = var.create_resource_group ? azurerm_resource_group.main[0].location : data.azurerm_resource_group.existing[0].location
}

# Key Vault
resource "azurerm_key_vault" "main" {
  name                = var.key_vault_name
  location            = local.resource_group_location
  resource_group_name = local.resource_group_name
  tenant_id           = data.azurerm_client_config.current.tenant_id
  sku_name            = var.key_vault_sku

  # Advanced security features
  enabled_for_disk_encryption     = true
  enabled_for_deployment          = false
  enabled_for_template_deployment = false
  purge_protection_enabled        = var.environment == "production"
  soft_delete_retention_days      = var.soft_delete_retention_days

  # Network access rules
  network_acls {
    default_action = var.key_vault_default_action
    bypass         = "AzureServices"
    
    # Allow access from specific IP ranges (if configured)
    dynamic "ip_rules" {
      for_each = var.allowed_ip_ranges
      content {
        value = ip_rules.value
      }
    }

    # Allow access from specific virtual networks (if configured)
    dynamic "virtual_network_subnet_ids" {
      for_each = var.allowed_subnet_ids
      content {
        value = virtual_network_subnet_ids.value
      }
    }
  }

  tags = var.tags
}

# Key Vault Access Policy for current user/service principal
resource "azurerm_key_vault_access_policy" "deployer" {
  key_vault_id = azurerm_key_vault.main.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = data.azurerm_client_config.current.object_id

  secret_permissions = [
    "Get",
    "List",
    "Set",
    "Delete",
    "Purge",
    "Recover"
  ]

  certificate_permissions = [
    "Get",
    "List",
    "Create",
    "Delete",
    "Update",
    "ManageContacts",
    "ManageIssuers"
  ]

  key_permissions = [
    "Get",
    "List",
    "Create",
    "Delete",
    "Update",
    "Encrypt",
    "Decrypt"
  ]
}

# App Service Managed Identity Access Policy (will be created later)
resource "azurerm_key_vault_access_policy" "app_service" {
  count = var.app_service_principal_id != null ? 1 : 0
  
  key_vault_id = azurerm_key_vault.main.id
  tenant_id    = data.azurerm_client_config.current.tenant_id
  object_id    = var.app_service_principal_id

  secret_permissions = [
    "Get",
    "List"
  ]
}

# Store critical secrets
resource "azurerm_key_vault_secret" "database_connection" {
  name         = "DefaultConnection"
  value        = var.database_connection_string
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]

  tags = merge(var.tags, {
    Environment = var.environment
    Purpose     = "Database connection string"
  })
}

resource "azurerm_key_vault_secret" "jwt_secret_key" {
  name         = "JWT--SecretKey"
  value        = var.jwt_secret_key
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]

  tags = merge(var.tags, {
    Environment = var.environment
    Purpose     = "JWT token signing key"
  })
}

resource "azurerm_key_vault_secret" "jwt_refresh_key" {
  name         = "JWT--RefreshKey"
  value        = var.jwt_refresh_key
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]

  tags = merge(var.tags, {
    Environment = var.environment
    Purpose     = "JWT refresh token signing key"
  })
}

resource "azurerm_key_vault_secret" "didit_api_key" {
  name         = "Didit--ApiKey"
  value        = var.didit_api_key
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]

  tags = merge(var.tags, {
    Environment = var.environment
    Purpose     = "Didit KYC API key"
  })
}

resource "azurerm_key_vault_secret" "gemini_api_key" {
  name         = "Gemini--ApiKey"
  value        = var.gemini_api_key
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]

  tags = merge(var.tags, {
    Environment = var.environment
    Purpose     = "Google Gemini AI API key"
  })
}

resource "azurerm_key_vault_secret" "google_calendar_client_id" {
  name         = "Google--Calendar--ClientId"
  value        = var.google_calendar_client_id
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]

  tags = merge(var.tags, {
    Environment = var.environment
    Purpose     = "Google Calendar API client ID"
  })
}

resource "azurerm_key_vault_secret" "google_calendar_client_secret" {
  name         = "Google--Calendar--ClientSecret"
  value        = var.google_calendar_client_secret
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]

  tags = merge(var.tags, {
    Environment = var.environment
    Purpose     = "Google Calendar API client secret"
  })
}

resource "azurerm_key_vault_secret" "signalr_connection_string" {
  name         = "SignalR--ConnectionString"
  value        = var.signalr_connection_string
  key_vault_id = azurerm_key_vault.main.id

  depends_on = [azurerm_key_vault_access_policy.deployer]

  tags = merge(var.tags, {
    Environment = var.environment
    Purpose     = "SignalR service connection string"
  })
}

# Diagnostic settings for Key Vault audit logging
resource "azurerm_monitor_diagnostic_setting" "key_vault" {
  count = var.enable_diagnostics ? 1 : 0
  
  name               = "kv-diagnostics"
  target_resource_id = azurerm_key_vault.main.id
  log_analytics_workspace_id = var.log_analytics_workspace_id

  enabled_log {
    category = "AuditEvent"
  }

  enabled_log {
    category = "AzurePolicyEvaluationDetails"
  }

  metric {
    category = "AllMetrics"
    enabled  = true
  }
}

# Key Vault alerts for security monitoring
resource "azurerm_monitor_metric_alert" "key_vault_requests" {
  count = var.enable_monitoring ? 1 : 0
  
  name                = "keyvault-high-request-rate"
  resource_group_name = local.resource_group_name
  scopes              = [azurerm_key_vault.main.id]
  description         = "Alert when Key Vault request rate is high"

  criteria {
    metric_namespace = "Microsoft.KeyVault/vaults"
    metric_name      = "ServiceApiHit"
    aggregation      = "Total"
    operator         = "GreaterThan"
    threshold        = 1000
  }

  window_size        = "PT5M"
  frequency          = "PT1M"
  severity           = 2

  action {
    action_group_id = var.action_group_id
  }

  tags = var.tags
}

# Output values
output "key_vault_id" {
  description = "The ID of the Key Vault"
  value       = azurerm_key_vault.main.id
}

output "key_vault_uri" {
  description = "The URI of the Key Vault"
  value       = azurerm_key_vault.main.vault_uri
}

output "key_vault_name" {
  description = "The name of the Key Vault"
  value       = azurerm_key_vault.main.name
}