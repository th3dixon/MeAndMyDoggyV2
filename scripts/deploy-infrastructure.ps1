# =============================================================================
# Infrastructure Deployment Script for MeAndMyDoggyV2
# This script deploys Azure Key Vault and related infrastructure
# =============================================================================

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("dev", "staging", "prod")]
    [string]$Environment,
    
    [Parameter(Mandatory=$false)]
    [string]$ResourceGroupName = "rg-meandmydoggyv2-$Environment",
    
    [Parameter(Mandatory=$false)]
    [string]$Location = "East US",
    
    [Parameter(Mandatory=$false)]
    [switch]$WhatIf
)

# Set error action preference
$ErrorActionPreference = "Stop"

# Function to log messages with timestamp
function Write-Log {
    param([string]$Message, [string]$Level = "INFO")
    $timestamp = Get-Date -Format "yyyy-MM-dd HH:mm:ss"
    Write-Host "[$timestamp] [$Level] $Message" -ForegroundColor $(
        switch ($Level) {
            "ERROR" { "Red" }
            "WARN" { "Yellow" }
            "SUCCESS" { "Green" }
            default { "White" }
        }
    )
}

# Function to check if Azure CLI is installed
function Test-AzureCLI {
    try {
        $azVersion = az version --output table 2>$null
        if ($LASTEXITCODE -eq 0) {
            Write-Log "Azure CLI is installed and available"
            return $true
        }
    }
    catch {
        Write-Log "Azure CLI is not installed or not available in PATH" "ERROR"
        return $false
    }
    return $false
}

# Function to check if user is logged into Azure
function Test-AzureLogin {
    try {
        $account = az account show --output json 2>$null | ConvertFrom-Json
        if ($account) {
            Write-Log "Logged into Azure as: $($account.user.name)"
            return $true
        }
    }
    catch {
        Write-Log "Not logged into Azure CLI" "ERROR"
        return $false
    }
    return $false
}

# Function to validate Terraform configuration
function Test-TerraformConfig {
    param([string]$TerraformPath)
    
    Write-Log "Validating Terraform configuration..."
    
    Push-Location $TerraformPath
    try {
        # Initialize Terraform
        terraform init -upgrade
        if ($LASTEXITCODE -ne 0) {
            throw "Terraform init failed"
        }
        
        # Validate configuration
        terraform validate
        if ($LASTEXITCODE -ne 0) {
            throw "Terraform validation failed"
        }
        
        Write-Log "Terraform configuration is valid" "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Terraform validation failed: $($_.Exception.Message)" "ERROR"
        return $false
    }
    finally {
        Pop-Location
    }
}

# Function to deploy infrastructure
function Deploy-Infrastructure {
    param(
        [string]$TerraformPath,
        [string]$Environment,
        [string]$ResourceGroupName,
        [string]$Location,
        [bool]$WhatIfMode
    )
    
    Write-Log "Starting infrastructure deployment for environment: $Environment"
    
    # Check if terraform.tfvars exists
    $tfvarsPath = Join-Path $TerraformPath "terraform.tfvars"
    if (-not (Test-Path $tfvarsPath)) {
        Write-Log "terraform.tfvars not found. Please create it from terraform.tfvars.example" "ERROR"
        return $false
    }
    
    Push-Location $TerraformPath
    try {
        # Create workspace for environment if it doesn't exist
        $workspaces = terraform workspace list
        if ($workspaces -notcontains "*$Environment") {
            Write-Log "Creating Terraform workspace for $Environment"
            terraform workspace new $Environment
        } else {
            Write-Log "Selecting Terraform workspace: $Environment"
            terraform workspace select $Environment
        }
        
        # Generate execution plan
        Write-Log "Generating Terraform execution plan..."
        $planArgs = @(
            "plan",
            "-var", "environment=$Environment",
            "-var", "resource_group_name=$ResourceGroupName",
            "-var", "location=$Location",
            "-out=tfplan"
        )
        
        & terraform @planArgs
        if ($LASTEXITCODE -ne 0) {
            throw "Terraform plan generation failed"
        }
        
        # Show plan
        terraform show tfplan
        
        if ($WhatIfMode) {
            Write-Log "WhatIf mode enabled. Skipping actual deployment." "WARN"
            return $true
        }
        
        # Confirm deployment
        $confirmation = Read-Host "Do you want to proceed with the deployment? (y/N)"
        if ($confirmation -ne "y" -and $confirmation -ne "Y") {
            Write-Log "Deployment cancelled by user" "WARN"
            return $false
        }
        
        # Apply infrastructure changes
        Write-Log "Applying Terraform configuration..."
        terraform apply tfplan
        if ($LASTEXITCODE -ne 0) {
            throw "Terraform apply failed"
        }
        
        # Get outputs
        Write-Log "Retrieving infrastructure outputs..."
        $outputs = terraform output -json | ConvertFrom-Json
        
        # Display key vault information
        if ($outputs.key_vault_uri) {
            Write-Log "Key Vault URI: $($outputs.key_vault_uri.value)" "SUCCESS"
        }
        if ($outputs.key_vault_name) {
            Write-Log "Key Vault Name: $($outputs.key_vault_name.value)" "SUCCESS"
        }
        
        Write-Log "Infrastructure deployment completed successfully!" "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Infrastructure deployment failed: $($_.Exception.Message)" "ERROR"
        return $false
    }
    finally {
        Pop-Location
    }
}

# Function to update application configuration
function Update-AppConfiguration {
    param(
        [string]$Environment,
        [string]$KeyVaultUri
    )
    
    Write-Log "Updating application configuration for Key Vault integration..."
    
    $configPath = Join-Path $PSScriptRoot "..\src\API\MeAndMyDog.API\appsettings.$Environment.json"
    
    try {
        # Create environment-specific configuration if it doesn't exist
        if (-not (Test-Path $configPath)) {
            $baseConfig = @{
                "Logging" = @{
                    "LogLevel" = @{
                        "Default" = "Information"
                        "Microsoft.AspNetCore" = "Warning"
                    }
                }
                "KeyVault" = @{
                    "VaultUri" = $KeyVaultUri
                }
                "AllowedHosts" = "*"
            }
            
            $baseConfig | ConvertTo-Json -Depth 10 | Set-Content $configPath
            Write-Log "Created $configPath with Key Vault configuration" "SUCCESS"
        } else {
            # Update existing configuration
            $config = Get-Content $configPath | ConvertFrom-Json
            if (-not $config.KeyVault) {
                $config | Add-Member -MemberType NoteProperty -Name "KeyVault" -Value @{} -Force
            }
            $config.KeyVault.VaultUri = $KeyVaultUri
            
            $config | ConvertTo-Json -Depth 10 | Set-Content $configPath
            Write-Log "Updated $configPath with Key Vault URI" "SUCCESS"
        }
        
        return $true
    }
    catch {
        Write-Log "Failed to update application configuration: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Main execution
Write-Log "==============================================================================="
Write-Log "MeAndMyDoggyV2 Infrastructure Deployment Script"
Write-Log "Environment: $Environment"
Write-Log "Resource Group: $ResourceGroupName"
Write-Log "Location: $Location"
if ($WhatIf) { Write-Log "Mode: What-If (no actual changes will be made)" "WARN" }
Write-Log "==============================================================================="

# Pre-flight checks
Write-Log "Performing pre-flight checks..."

if (-not (Test-AzureCLI)) {
    Write-Log "Please install Azure CLI: https://docs.microsoft.com/en-us/cli/azure/install-azure-cli" "ERROR"
    exit 1
}

if (-not (Test-AzureLogin)) {
    Write-Log "Please login to Azure CLI: az login" "ERROR"
    exit 1
}

# Check if Terraform is installed
try {
    $terraformVersion = terraform version
    Write-Log "Terraform is available: $($terraformVersion.Split([Environment]::NewLine)[0])"
}
catch {
    Write-Log "Terraform is not installed or not available in PATH" "ERROR"
    Write-Log "Please install Terraform: https://www.terraform.io/downloads.html" "ERROR"
    exit 1
}

# Set Terraform path
$terraformPath = Join-Path $PSScriptRoot "..\infrastructure\terraform"
if (-not (Test-Path $terraformPath)) {
    Write-Log "Terraform configuration directory not found: $terraformPath" "ERROR"
    exit 1
}

# Validate Terraform configuration
if (-not (Test-TerraformConfig -TerraformPath $terraformPath)) {
    Write-Log "Terraform configuration validation failed" "ERROR"
    exit 1
}

# Deploy infrastructure
$deploymentSuccess = Deploy-Infrastructure -TerraformPath $terraformPath -Environment $Environment -ResourceGroupName $ResourceGroupName -Location $Location -WhatIfMode $WhatIf.IsPresent

if (-not $deploymentSuccess) {
    Write-Log "Infrastructure deployment failed" "ERROR"
    exit 1
}

# Update application configuration (only if not in WhatIf mode)
if (-not $WhatIf.IsPresent) {
    Push-Location $terraformPath
    try {
        $keyVaultUri = terraform output -raw key_vault_uri
        if ($keyVaultUri) {
            Update-AppConfiguration -Environment $Environment -KeyVaultUri $keyVaultUri
        }
    }
    catch {
        Write-Log "Could not retrieve Key Vault URI from Terraform outputs" "WARN"
    }
    finally {
        Pop-Location
    }
}

Write-Log "==============================================================================="
Write-Log "Infrastructure deployment script completed successfully!" "SUCCESS"
Write-Log "==============================================================================="

# Next steps guidance
Write-Log ""
Write-Log "NEXT STEPS:"
Write-Log "1. Verify Key Vault access: az keyvault secret list --vault-name <vault-name>"
Write-Log "2. Run database migration script: .\scripts\migrate-database.ps1 -Environment $Environment"
Write-Log "3. Test application startup with new configuration"
Write-Log "4. Verify Azure Key Vault integration is working"
Write-Log ""