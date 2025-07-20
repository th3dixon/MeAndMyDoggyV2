# =============================================================================
# Terraform Variables for MeAndMyDoggyV2 - Development Environment
# =============================================================================

# Environment Configuration
environment             = "dev"
location               = "East US"
resource_group_name    = "rg-meandmydoggyv2-dev"
create_resource_group  = true

# Key Vault Configuration
key_vault_name          = "kv-meandmydoggy-dev"
key_vault_sku          = "standard"
key_vault_default_action = "Allow"
soft_delete_retention_days = 7

# Network Access (public access for development)
allowed_ip_ranges = []
allowed_subnet_ids = []

# ============================================================================= 
# IMPORTANT: You must replace these placeholder values with actual secrets
# These are required for the deployment to succeed
# =============================================================================

# Database Connection (replace with your actual Azure SQL connection string)
database_connection_string = "PLACEHOLDER_DATABASE_CONNECTION_STRING"

# JWT Keys (replace with strong random keys - minimum 32 characters each)
jwt_secret_key = "PLACEHOLDER_JWT_SECRET_KEY_32_CHARS_MINIMUM"
jwt_refresh_key = "PLACEHOLDER_JWT_REFRESH_KEY_32_CHARS_MINIMUM"

# External API Keys (replace with your actual API keys)
didit_api_key = "PLACEHOLDER_DIDIT_API_KEY"
gemini_api_key = "PLACEHOLDER_GEMINI_API_KEY"

# Google Calendar Integration (replace with your actual Google API credentials)
google_calendar_client_id = "PLACEHOLDER_GOOGLE_CALENDAR_CLIENT_ID"
google_calendar_client_secret = "PLACEHOLDER_GOOGLE_CALENDAR_CLIENT_SECRET"

# SignalR Service (replace with your actual SignalR connection string)
signalr_connection_string = "PLACEHOLDER_SIGNALR_CONNECTION_STRING"

# Monitoring Configuration
enable_diagnostics = true
enable_monitoring = true

# Resource Tags
tags = {
  Project     = "MeAndMyDoggyV2"
  Environment = "dev"
  ManagedBy   = "Terraform"
  Owner       = "Development Team"
  CostCenter  = "Engineering"
}