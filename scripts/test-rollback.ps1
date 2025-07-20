# Rollback Testing Script for MeAndMyDoggyV2

param(
    [Parameter(Mandatory=$true)]
    [string]$TargetMigration,
    
    [Parameter(Mandatory=$false)]
    [string]$ProjectPath = "src/API/MeAndMyDog.API"
)

Write-Host "=== Testing Rollback to Migration: $TargetMigration ===" -ForegroundColor Cyan

# Step 1: Create database snapshot
Write-Host "`nStep 1: Creating database snapshot..." -ForegroundColor Green
$snapshotQuery = @"
CREATE DATABASE MeAndMyDoggy_Test_Snapshot ON
(NAME = 'MeAndMyDoggy_Test', FILENAME = 'C:\Program Files\Microsoft SQL Server\MSSQL15.MSSQLSERVER\MSSQL\DATA\MeAndMyDoggy_Test_Snapshot.ss')
AS SNAPSHOT OF MeAndMyDoggy_Test;
"@

try {
    Invoke-Sqlcmd -ServerInstance "localhost" -Database "master" -Query $snapshotQuery
    Write-Host "Snapshot created successfully" -ForegroundColor Green
}
catch {
    Write-Host "Warning: Could not create snapshot. Proceeding without snapshot." -ForegroundColor Yellow
}

# Step 2: Run data integrity tests before rollback
Write-Host "`nStep 2: Running pre-rollback data integrity tests..." -ForegroundColor Green
Invoke-Sqlcmd -ServerInstance "localhost" -Database "MeAndMyDoggy_Test" -InputFile "$PSScriptRoot\test-data-integrity.sql"

# Step 3: Perform rollback
Write-Host "`nStep 3: Rolling back to migration: $TargetMigration..." -ForegroundColor Green
Push-Location $ProjectPath
try {
    $env:ASPNETCORE_ENVIRONMENT = "Testing"
    dotnet ef database update $TargetMigration --no-build
}
finally {
    Pop-Location
}

# Step 4: Run data integrity tests after rollback
Write-Host "`nStep 4: Running post-rollback data integrity tests..." -ForegroundColor Green
Invoke-Sqlcmd -ServerInstance "localhost" -Database "MeAndMyDoggy_Test" -InputFile "$PSScriptRoot\test-data-integrity.sql"

# Step 5: Test application functionality
Write-Host "`nStep 5: Testing application functionality..." -ForegroundColor Green
Write-Host "Please run the application and verify:"
Write-Host "  - User authentication works"
Write-Host "  - Service provider search works"
Write-Host "  - Booking creation works"
Write-Host "  - Messaging works"

Write-Host "`n=== Rollback testing completed ===" -ForegroundColor Cyan