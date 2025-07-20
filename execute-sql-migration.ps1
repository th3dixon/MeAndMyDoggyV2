# Quick script to execute the comprehensive SQL migration
param(
    [string]$ConnectionString = "Server=tcp:senseilive.uksouth.cloudapp.azure.com,1433;Initial Catalog=MeAndMyDoggy;Persist Security Info=False;User ID=DojoAdmin;Password=K1lledkenny#1;MultipleActiveResultSets=True;Encrypt=True;TrustServerCertificate=True;Connection Timeout=30;"
)

try {
    # Install SqlServer module if not available
    if (-not (Get-Module -ListAvailable -Name SqlServer)) {
        Write-Host "Installing SqlServer module..."
        Install-Module -Name SqlServer -Force -Scope CurrentUser
    }

    Import-Module SqlServer
    
    Write-Host "Connecting to database..."
    
    # Read the SQL script
    $sqlScript = Get-Content "database-scripts\complete-migration-scripts.sql" -Raw
    
    Write-Host "Executing comprehensive database migration..."
    
    # Execute the script
    Invoke-Sqlcmd -ConnectionString $ConnectionString -Query $sqlScript -QueryTimeout 600
    
    Write-Host "Database migration completed successfully!" -ForegroundColor Green
}
catch {
    Write-Error "Migration failed: $($_.Exception.Message)"
    exit 1
}