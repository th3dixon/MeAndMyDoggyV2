# Migration Testing Quick Start Guide

## Prerequisites Checklist

- [ ] Docker Desktop installed (for Option 1)
- [ ] SQL Server 2019+ installed (for Option 2)
- [ ] .NET 9.0 SDK installed
- [ ] Entity Framework Core tools installed (`dotnet tool install --global dotnet-ef`)

## Quick Setup - Option 1: Using Docker

1. **Start SQL Server containers:**
   ```bash
   cd C:\kirorepo\MeAndMyDoggyV2\MeAndMyDoggyV2
   docker-compose -f docker-compose.dev.yml up -d
   ```

2. **Wait for SQL Server to start (about 30 seconds), then create databases:**
   ```bash
   docker exec -it meandmydoggy-sqlserver-dev /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P YourStrong@Password123 -i /docker-entrypoint-initdb.d/setup-dev-databases.sql
   ```

## Quick Setup - Option 2: Using Local SQL Server

1. **Run database setup script:**
   ```powershell
   cd C:\kirorepo\MeAndMyDoggyV2\MeAndMyDoggyV2
   sqlcmd -S localhost -U sa -P YourPassword -i scripts\setup-dev-databases.sql
   ```

## Running Migration Tests

### 1. Test Current Migrations
```powershell
cd C:\kirorepo\MeAndMyDoggyV2\MeAndMyDoggyV2
.\scripts\test-migrations.ps1
```

### 2. Create a New Migration
```bash
cd src/API/MeAndMyDog.API
dotnet ef migrations add YourMigrationName
```

### 3. Test the New Migration
```powershell
cd C:\kirorepo\MeAndMyDoggyV2\MeAndMyDoggyV2
.\scripts\test-migrations.ps1 -Environment Testing
```

### 4. Test Rollback
```powershell
.\scripts\test-migrations.ps1 -TestRollback
```

### 5. Run Data Integrity Tests
```bash
sqlcmd -S localhost -d MeAndMyDoggy_Test -U sa -P YourStrong@Password123 -i scripts\test-data-integrity.sql
```

### 6. Run Performance Tests
```bash
sqlcmd -S localhost -d MeAndMyDoggy_Test -U sa -P YourStrong@Password123 -i scripts\test-performance.sql
```

## Common Commands

### List all migrations:
```bash
cd src/API/MeAndMyDog.API
dotnet ef migrations list
```

### Generate SQL script for migrations:
```bash
dotnet ef migrations script -o migration-script.sql
```

### Update database to specific migration:
```bash
dotnet ef database update MigrationName
```

### Remove last migration (if not applied):
```bash
dotnet ef migrations remove
```

## Troubleshooting

### Connection Issues
1. Check SQL Server is running: `docker ps` or SQL Server Configuration Manager
2. Verify connection string in appsettings.Testing.json
3. Ensure firewall allows port 1433 (or 1434 for test instance)

### Migration Failures
1. Check for pending model changes: `dotnet ef migrations has-pending-model-changes`
2. Ensure all required packages are installed
3. Check for circular dependencies in entity relationships

### Performance Issues
1. Run index analysis from test-performance.sql
2. Check for missing indexes on foreign keys
3. Review query execution plans

## Safety Checklist Before Production

- [ ] All migrations tested in test environment
- [ ] Rollback procedures tested and documented
- [ ] Data integrity tests pass
- [ ] Performance baselines documented
- [ ] Backup of production database taken
- [ ] Migration scripts reviewed by team
- [ ] Deployment window scheduled
- [ ] Rollback plan communicated to team