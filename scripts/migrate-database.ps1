# =============================================================================
# Database Migration Script for MeAndMyDoggyV2
# This script executes the comprehensive database migrations
# =============================================================================

param(
    [Parameter(Mandatory=$true)]
    [ValidateSet("dev", "staging", "prod")]
    [string]$Environment,
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("All", "Phase1", "Phase2", "Phase3", "Phase4", "Phase5")]
    [string]$Phase = "All",
    
    [Parameter(Mandatory=$false)]
    [switch]$WhatIf,
    
    [Parameter(Mandatory=$false)]
    [switch]$Force,
    
    [Parameter(Mandatory=$false)]
    [string]$ConnectionString
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
            "CRITICAL" { "Magenta" }
            default { "White" }
        }
    )
}

# Function to get connection string
function Get-ConnectionString {
    param([string]$Environment, [string]$ProvidedConnectionString)
    
    if ($ProvidedConnectionString) {
        return $ProvidedConnectionString
    }
    
    # Try to get from configuration
    $configPath = Join-Path $PSScriptRoot "..\src\API\MeAndMyDog.API\appsettings.$Environment.json"
    if (Test-Path $configPath) {
        $config = Get-Content $configPath | ConvertFrom-Json
        if ($config.ConnectionStrings.DefaultConnection) {
            return $config.ConnectionStrings.DefaultConnection
        }
    }
    
    # Prompt user for connection string
    Write-Log "Connection string not found in configuration" "WARN"
    $connectionString = Read-Host "Please enter the database connection string"
    return $connectionString
}

# Function to test database connection
function Test-DatabaseConnection {
    param([string]$ConnectionString)
    
    Write-Log "Testing database connection..."
    
    try {
        # Use SqlCmd to test connection
        $testQuery = "SELECT 1 as TestConnection"
        $result = Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $testQuery -ErrorAction Stop
        
        if ($result.TestConnection -eq 1) {
            Write-Log "Database connection successful" "SUCCESS"
            return $true
        }
    }
    catch {
        Write-Log "Database connection failed: $($_.Exception.Message)" "ERROR"
        return $false
    }
    
    return $false
}

# Function to backup database
function Backup-Database {
    param([string]$ConnectionString, [string]$Environment)
    
    Write-Log "Creating database backup before migration..."
    
    try {
        # Extract database name from connection string
        if ($ConnectionString -match "Database=([^;]+)") {
            $databaseName = $matches[1]
        } else {
            throw "Could not extract database name from connection string"
        }
        
        $backupFileName = "MeAndMyDoggyV2_${Environment}_$(Get-Date -Format 'yyyyMMdd_HHmmss').bak"
        $backupPath = Join-Path $PSScriptRoot "..\backups\$backupFileName"
        
        # Ensure backup directory exists
        $backupDir = Split-Path $backupPath -Parent
        if (-not (Test-Path $backupDir)) {
            New-Item -ItemType Directory -Path $backupDir -Force | Out-Null
        }
        
        $backupQuery = "BACKUP DATABASE [$databaseName] TO DISK = '$backupPath' WITH FORMAT, INIT, COMPRESSION"
        Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $backupQuery -QueryTimeout 300
        
        Write-Log "Database backup created: $backupPath" "SUCCESS"
        return $backupPath
    }
    catch {
        Write-Log "Database backup failed: $($_.Exception.Message)" "ERROR"
        return $null
    }
}

# Function to execute SQL migration script
function Invoke-Migration {
    param(
        [string]$ConnectionString,
        [string]$MigrationPath,
        [string]$PhaseName
    )
    
    Write-Log "Executing $PhaseName migration..."
    
    try {
        if (-not (Test-Path $MigrationPath)) {
            throw "Migration script not found: $MigrationPath"
        }
        
        # Read and execute the migration script
        $migrationScript = Get-Content $MigrationPath -Raw
        
        if ($WhatIf) {
            Write-Log "WhatIf mode: Migration script would execute $(($migrationScript -split "`n").Count) lines" "WARN"
            return $true
        }
        
        # Execute migration with extended timeout
        Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $migrationScript -QueryTimeout 600 -ErrorAction Stop
        
        Write-Log "$PhaseName migration completed successfully" "SUCCESS"
        return $true
    }
    catch {
        Write-Log "$PhaseName migration failed: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Function to validate migration results
function Test-MigrationResults {
    param([string]$ConnectionString)
    
    Write-Log "Validating migration results..."
    
    try {
        # Check if all expected tables exist
        $expectedTables = @(
            "Users", "Roles", "UserRoles", "Permissions", "RolePermissions",
            "UserSessions", "SystemSettings", "UserSettings", "AuditLogs",
            "DogProfiles", "MedicalRecords", "ServiceProviders", "Services",
            "ServiceProviderReviews", "Conversations", "Messages", "MessageAttachments",
            "Appointments", "KYCVerifications", "AIHealthRecommendations",
            "SubscriptionPlans", "UserSubscriptions"
        )
        
        $validationQuery = @"
SELECT 
    t.TABLE_NAME,
    COUNT(*) as ColumnCount
FROM INFORMATION_SCHEMA.TABLES t
LEFT JOIN INFORMATION_SCHEMA.COLUMNS c ON t.TABLE_NAME = c.TABLE_NAME
WHERE t.TABLE_SCHEMA = 'dbo' 
    AND t.TABLE_TYPE = 'BASE TABLE'
    AND t.TABLE_NAME IN ('$($expectedTables -join "','")')
GROUP BY t.TABLE_NAME
ORDER BY t.TABLE_NAME
"@
        
        $results = Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $validationQuery
        
        Write-Log "Migration validation results:"
        foreach ($result in $results) {
            Write-Log "  - $($result.TABLE_NAME): $($result.ColumnCount) columns" "SUCCESS"
        }
        
        $foundTables = $results | Select-Object -ExpandProperty TABLE_NAME
        $missingTables = $expectedTables | Where-Object { $_ -notin $foundTables }
        
        if ($missingTables.Count -gt 0) {
            Write-Log "Missing tables: $($missingTables -join ', ')" "ERROR"
            return $false
        }
        
        # Check total tables and indexes
        $statsQuery = @"
SELECT 
    'Tables' AS Type,
    COUNT(*) AS Count
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' AND TABLE_TYPE = 'BASE TABLE'
UNION ALL
SELECT 
    'Indexes' AS Type,
    COUNT(*) AS Count
FROM sys.indexes 
WHERE is_disabled = 0
UNION ALL
SELECT 
    'FullText Indexes' AS Type,
    COUNT(*) AS Count
FROM sys.fulltext_indexes
"@
        
        $stats = Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $statsQuery
        foreach ($stat in $stats) {
            Write-Log "  $($stat.Type): $($stat.Count)" "SUCCESS"
        }
        
        Write-Log "Migration validation completed successfully" "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Migration validation failed: $($_.Exception.Message)" "ERROR"
        return $false
    }
}

# Function to execute Entity Framework migrations
function Invoke-EFMigrations {
    param([string]$Environment)
    
    Write-Log "Executing Entity Framework migrations..."
    
    $apiProjectPath = Join-Path $PSScriptRoot "..\src\API\MeAndMyDog.API"
    
    try {
        Push-Location $apiProjectPath
        
        # Set environment
        $env:ASPNETCORE_ENVIRONMENT = $Environment
        
        if ($WhatIf) {
            Write-Log "WhatIf mode: Would execute 'dotnet ef database update'" "WARN"
            return $true
        }
        
        # Execute EF migrations
        & dotnet ef database update --verbose
        if ($LASTEXITCODE -ne 0) {
            throw "Entity Framework migration failed"
        }
        
        Write-Log "Entity Framework migrations completed successfully" "SUCCESS"
        return $true
    }
    catch {
        Write-Log "Entity Framework migrations failed: $($_.Exception.Message)" "ERROR"
        return $false
    }
    finally {
        Pop-Location
        Remove-Item env:ASPNETCORE_ENVIRONMENT -ErrorAction SilentlyContinue
    }
}

# Main execution
Write-Log "==============================================================================="
Write-Log "MeAndMyDoggyV2 Database Migration Script"
Write-Log "Environment: $Environment"
Write-Log "Phase: $Phase"
if ($WhatIf) { Write-Log "Mode: What-If (no actual changes will be made)" "WARN" }
if ($Force) { Write-Log "Mode: Force (skipping confirmations)" "WARN" }
Write-Log "==============================================================================="

# Pre-flight checks
Write-Log "Performing pre-flight checks..."

# Check if SqlServer module is available
try {
    Import-Module SqlServer -ErrorAction Stop
    Write-Log "SqlServer PowerShell module is available"
}
catch {
    Write-Log "SqlServer PowerShell module is not installed" "ERROR"
    Write-Log "Please install it: Install-Module -Name SqlServer" "ERROR"
    exit 1
}

# Check if .NET CLI is available
try {
    $dotnetVersion = & dotnet --version
    Write-Log ".NET CLI is available: $dotnetVersion"
}
catch {
    Write-Log ".NET CLI is not installed or not available in PATH" "ERROR"
    exit 1
}

# Get connection string
$connectionString = Get-ConnectionString -Environment $Environment -ProvidedConnectionString $ConnectionString
if ([string]::IsNullOrEmpty($connectionString)) {
    Write-Log "Connection string is required" "ERROR"
    exit 1
}

# Test database connection
if (-not (Test-DatabaseConnection -ConnectionString $connectionString)) {
    Write-Log "Cannot connect to database. Please check connection string and database availability." "ERROR"
    exit 1
}

# Security warning for production
if ($Environment -eq "prod" -and -not $Force) {
    Write-Log "WARNING: You are about to run migrations on PRODUCTION database!" "CRITICAL"
    $confirmation = Read-Host "Type 'CONFIRM-PRODUCTION-MIGRATION' to proceed"
    if ($confirmation -ne "CONFIRM-PRODUCTION-MIGRATION") {
        Write-Log "Production migration cancelled by user" "WARN"
        exit 0
    }
}

# Create database backup (skip in WhatIf mode)
if (-not $WhatIf.IsPresent) {
    $backupPath = Backup-Database -ConnectionString $connectionString -Environment $Environment
    if (-not $backupPath) {
        Write-Log "Database backup failed. Migration aborted." "ERROR"
        exit 1
    }
}

# Execute migrations based on phase
$migrationSuccess = $true
$migrationScriptPath = Join-Path $PSScriptRoot "..\database-scripts\complete-migration-scripts.sql"

switch ($Phase) {
    "All" {
        Write-Log "Executing all migration phases..."
        $migrationSuccess = Invoke-Migration -ConnectionString $connectionString -MigrationPath $migrationScriptPath -PhaseName "Complete Migration"
    }
    "Phase1" {
        Write-Log "Phase 1 migrations not yet separated. Executing Entity Framework migrations only."
        $migrationSuccess = Invoke-EFMigrations -Environment $Environment
    }
    default {
        Write-Log "Specific phase migrations not yet implemented. Executing all migrations."
        $migrationSuccess = Invoke-Migration -ConnectionString $connectionString -MigrationPath $migrationScriptPath -PhaseName "Complete Migration"
    }
}

# Execute Entity Framework migrations
if ($migrationSuccess) {
    $migrationSuccess = Invoke-EFMigrations -Environment $Environment
}

# Validate migration results
if ($migrationSuccess -and -not $WhatIf.IsPresent) {
    $migrationSuccess = Test-MigrationResults -ConnectionString $connectionString
}

# Report results
Write-Log "==============================================================================="
if ($migrationSuccess) {
    Write-Log "Database migration completed successfully!" "SUCCESS"
    
    if (-not $WhatIf.IsPresent) {
        Write-Log ""
        Write-Log "NEXT STEPS:"
        Write-Log "1. Test application startup: dotnet run --project src\API\MeAndMyDog.API"
        Write-Log "2. Verify database schema in SQL Server Management Studio"
        Write-Log "3. Run application health checks: curl https://localhost:5001/health"
        Write-Log "4. Execute application integration tests"
        if ($backupPath) {
            Write-Log "5. Backup file location: $backupPath"
        }
    }
} else {
    Write-Log "Database migration failed!" "ERROR"
    if ($backupPath -and -not $WhatIf.IsPresent) {
        Write-Log ""
        Write-Log "RECOVERY OPTIONS:"
        Write-Log "1. Restore from backup: $backupPath"
        Write-Log "2. Check migration logs for specific errors"
        Write-Log "3. Contact database administrator if needed"
    }
    exit 1
}
Write-Log "==============================================================================="