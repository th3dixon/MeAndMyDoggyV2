# =============================================================================
# Terraform Variables for MeAndMyDoggyV2 Infrastructure
# =============================================================================

# General Configuration
variable "environment" {
  description = "Environment name (dev, staging, prod)"
  type        = string
  default     = "dev"
  
  validation {
    condition     = contains(["dev", "staging", "prod"], var.environment)
    error_message = "Environment must be one of: dev, staging, prod."
  }
}

variable "location" {
  description = "Azure region for resources"
  type        = string
  default     = "East US"
}

variable "resource_group_name" {
  description = "Name of the resource group"
  type        = string
  default     = "rg-meandmydoggyv2"
}

variable "create_resource_group" {
  description = "Whether to create a new resource group"
  type        = bool
  default     = true
}

variable "tags" {
  description = "Tags to apply to resources"
  type        = map(string)
  default = {
    Project     = "MeAndMyDoggyV2"
    ManagedBy   = "Terraform"
    Owner       = "Architecture Team"
  }
}

# Key Vault Configuration
variable "key_vault_name" {
  description = "Name of the Key Vault"
  type        = string
  default     = "kv-meandmydoggy"
  
  validation {
    condition     = can(regex("^[a-zA-Z0-9-]{3,24}$", var.key_vault_name))
    error_message = "Key Vault name must be 3-24 characters and contain only alphanumeric characters and hyphens."
  }
}

variable "key_vault_sku" {
  description = "Key Vault SKU"
  type        = string
  default     = "standard"
  
  validation {
    condition     = contains(["standard", "premium"], var.key_vault_sku)
    error_message = "Key Vault SKU must be either 'standard' or 'premium'."
  }
}

variable "key_vault_default_action" {
  description = "Default network access action for Key Vault"
  type        = string
  default     = "Allow"
  
  validation {
    condition     = contains(["Allow", "Deny"], var.key_vault_default_action)
    error_message = "Key Vault default action must be either 'Allow' or 'Deny'."
  }
}

variable "soft_delete_retention_days" {
  description = "Number of days to retain deleted secrets"
  type        = number
  default     = 7
  
  validation {
    condition     = var.soft_delete_retention_days >= 7 && var.soft_delete_retention_days <= 90
    error_message = "Soft delete retention must be between 7 and 90 days."
  }
}

variable "allowed_ip_ranges" {
  description = "List of IP ranges allowed to access Key Vault"
  type        = list(string)
  default     = []
}

variable "allowed_subnet_ids" {
  description = "List of subnet IDs allowed to access Key Vault"
  type        = list(string)
  default     = []
}

# App Service Configuration
variable "app_service_principal_id" {
  description = "Object ID of the App Service Managed Identity"
  type        = string
  default     = null
}

# Secret Values (should be provided via terraform.tfvars or environment variables)
variable "database_connection_string" {
  description = "Database connection string"
  type        = string
  sensitive   = true
}

variable "jwt_secret_key" {
  description = "JWT secret signing key"
  type        = string
  sensitive   = true
  
  validation {
    condition     = length(var.jwt_secret_key) >= 32
    error_message = "JWT secret key must be at least 32 characters long."
  }
}

variable "jwt_refresh_key" {
  description = "JWT refresh token signing key"
  type        = string
  sensitive   = true
  
  validation {
    condition     = length(var.jwt_refresh_key) >= 32
    error_message = "JWT refresh key must be at least 32 characters long."
  }
}

variable "didit_api_key" {
  description = "Didit KYC API key"
  type        = string
  sensitive   = true
}

variable "gemini_api_key" {
  description = "Google Gemini AI API key"
  type        = string
  sensitive   = true
}

variable "google_calendar_client_id" {
  description = "Google Calendar API client ID"
  type        = string
  sensitive   = true
}

variable "google_calendar_client_secret" {
  description = "Google Calendar API client secret"
  type        = string
  sensitive   = true
}

variable "signalr_connection_string" {
  description = "SignalR service connection string"
  type        = string
  sensitive   = true
}

# Monitoring Configuration
variable "enable_diagnostics" {
  description = "Enable diagnostic settings for Key Vault"
  type        = bool
  default     = true
}

variable "enable_monitoring" {
  description = "Enable monitoring alerts for Key Vault"
  type        = bool
  default     = true
}

variable "log_analytics_workspace_id" {
  description = "Log Analytics workspace ID for diagnostics"
  type        = string
  default     = null
}

variable "action_group_id" {
  description = "Action group ID for alerts"
  type        = string
  default     = null
}