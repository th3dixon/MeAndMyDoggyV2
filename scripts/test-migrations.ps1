# Migration Testing Script for MeAndMyDoggyV2

param(
    [Parameter(Mandatory=$false)]
    [string]$Environment = "Testing",
    
    [Parameter(Mandatory=$false)]
    [string]$ProjectPath = "src/API/MeAndMyDog.API",
    
    [Parameter(Mandatory=$false)]
    [switch]$SkipBackup = $false,
    
    [Parameter(Mandatory=$false)]
    [switch]$TestRollback = $false
)

$ErrorActionPreference = "Stop"

Write-Host "=== MeAndMyDoggyV2 Migration Testing ===" -ForegroundColor Cyan
Write-Host "Environment: $Environment" -ForegroundColor Yellow
Write-Host "Project Path: $ProjectPath" -ForegroundColor Yellow

# Function to execute SQL command
function Execute-SqlCommand {
    param(
        [string]$ConnectionString,
        [string]$Query
    )
    
    $connection = New-Object System.Data.SqlClient.SqlConnection
    $connection.ConnectionString = $ConnectionString
    
    try {
        $connection.Open()
        $command = $connection.CreateCommand()
        $command.CommandText = $Query
        $command.ExecuteNonQuery()
    }
    finally {
        $connection.Close()
    }
}

# Function to backup database
function Backup-Database {
    param(
        [string]$ConnectionString,
        [string]$DatabaseName
    )
    
    $backupPath = "$PSScriptRoot\backups\$DatabaseName`_$(Get-Date -Format 'yyyyMMdd_HHmmss').bak"
    $backupDir = Split-Path $backupPath -Parent
    
    if (!(Test-Path $backupDir)) {
        New-Item -ItemType Directory -Path $backupDir | Out-Null
    }
    
    $query = "BACKUP DATABASE [$DatabaseName] TO DISK = '$backupPath' WITH FORMAT, INIT"
    
    Write-Host "Backing up database to: $backupPath" -ForegroundColor Yellow
    Execute-SqlCommand -ConnectionString $ConnectionString -Query $query
    
    return $backupPath
}

# Step 1: Backup current test database
if (!$SkipBackup) {
    Write-Host "`nStep 1: Backing up test database..." -ForegroundColor Green
    $connString = "Server=localhost;Database=master;User ID=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
    $backupPath = Backup-Database -ConnectionString $connString -DatabaseName "MeAndMyDoggy_Test"
}

# Step 2: List current migrations
Write-Host "`nStep 2: Listing current migrations..." -ForegroundColor Green
Push-Location $ProjectPath
try {
    dotnet ef migrations list --no-build
}
finally {
    Pop-Location
}

# Step 3: Generate migration script
Write-Host "`nStep 3: Generating migration script..." -ForegroundColor Green
Push-Location $ProjectPath
try {
    $scriptPath = "$PSScriptRoot\migration-scripts\pending-migrations_$(Get-Date -Format 'yyyyMMdd_HHmmss').sql"
    $scriptDir = Split-Path $scriptPath -Parent
    
    if (!(Test-Path $scriptDir)) {
        New-Item -ItemType Directory -Path $scriptDir | Out-Null
    }
    
    dotnet ef migrations script --idempotent --output $scriptPath --no-build
    Write-Host "Migration script generated at: $scriptPath" -ForegroundColor Yellow
}
finally {
    Pop-Location
}

# Step 4: Apply migrations to test database
Write-Host "`nStep 4: Applying migrations to test database..." -ForegroundColor Green
Push-Location $ProjectPath
try {
    $env:ASPNETCORE_ENVIRONMENT = $Environment
    dotnet ef database update --no-build
}
finally {
    Pop-Location
}

# Step 5: Verify migration success
Write-Host "`nStep 5: Verifying migration success..." -ForegroundColor Green
$verifyQuery = @"
SELECT 
    MigrationId,
    ProductVersion
FROM __EFMigrationsHistory
ORDER BY MigrationId DESC
"@

$connString = "Server=localhost;Database=MeAndMyDoggy_Test;User ID=sa;Password=YourStrong@Password123;TrustServerCertificate=True;"
Execute-SqlCommand -ConnectionString $connString -Query $verifyQuery

# Step 6: Test rollback if requested
if ($TestRollback) {
    Write-Host "`nStep 6: Testing rollback..." -ForegroundColor Green
    
    # Get the previous migration name
    Push-Location $ProjectPath
    try {
        $migrations = dotnet ef migrations list --no-build | Where-Object { $_ -match '^\s*\d+_' }
        if ($migrations.Count -gt 1) {
            $previousMigration = $migrations[-2] -replace '^\s*', ''
            $previousMigration = $previousMigration.Split(' ')[0]
            
            Write-Host "Rolling back to: $previousMigration" -ForegroundColor Yellow
            dotnet ef database update $previousMigration --no-build
            
            Write-Host "Rollback completed successfully!" -ForegroundColor Green
        }
        else {
            Write-Host "No previous migration to rollback to." -ForegroundColor Yellow
        }
    }
    finally {
        Pop-Location
    }
}

Write-Host "`n=== Migration testing completed successfully! ===" -ForegroundColor Cyan