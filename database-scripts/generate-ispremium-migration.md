# Generate IsPremium Migration

## Entity Framework Migration Instructions

Since you've updated the ServiceProvider entity model to include the new IsPremium properties, you'll need to generate an Entity Framework migration to update the database schema.

### Step 1: Generate the Migration

Run the following command in your API project directory:

```bash
# Navigate to your API project
cd src/API/MeAndMyDog.API

# Generate the migration
dotnet ef migrations add AddIsPremiumToServiceProviders --context ApplicationDbContext

# Review the generated migration file
# It should be created in: Migrations/[TIMESTAMP]_AddIsPremiumToServiceProviders.cs
```

### Step 2: Review the Generated Migration

The migration should contain something similar to:

```csharp
public partial class AddIsPremiumToServiceProviders : Migration
{
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<bool>(
            name: "IsPremium",
            table: "ServiceProviders",
            type: "bit",
            nullable: false,
            defaultValue: false);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "PremiumStartDate",
            table: "ServiceProviders",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.AddColumn<DateTimeOffset>(
            name: "PremiumEndDate",
            table: "ServiceProviders",
            type: "datetimeoffset",
            nullable: true);

        migrationBuilder.AddColumn<string>(
            name: "PremiumSubscriptionId",
            table: "ServiceProviders",
            type: "nvarchar(50)",
            maxLength: 50,
            nullable: true);

        // Create index for performance
        migrationBuilder.CreateIndex(
            name: "IX_ServiceProviders_IsPremium",
            table: "ServiceProviders",
            columns: new[] { "IsPremium", "IsVerified", "CreatedAt" },
            descending: new[] { true, true, true });
    }

    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropIndex(
            name: "IX_ServiceProviders_IsPremium",
            table: "ServiceProviders");

        migrationBuilder.DropColumn(
            name: "IsPremium",
            table: "ServiceProviders");

        migrationBuilder.DropColumn(
            name: "PremiumStartDate",
            table: "ServiceProviders");

        migrationBuilder.DropColumn(
            name: "PremiumEndDate",
            table: "ServiceProviders");

        migrationBuilder.DropColumn(
            name: "PremiumSubscriptionId",
            table: "ServiceProviders");
    }
}
```

### Step 3: Apply the Migration

```bash
# Apply the migration to update the database
dotnet ef database update --context ApplicationDbContext

# Verify the migration was applied
dotnet ef migrations list --context ApplicationDbContext
```

### Step 4: Test the Changes

After applying the migration, test that:

1. ✅ The ServiceProviders table now has the new columns
2. ✅ Existing providers have IsPremium = false by default
3. ✅ The API can query and filter by IsPremium
4. ✅ The search results include the IsPremium field

### Alternative: Manual SQL Approach

If you prefer to run the SQL script instead of using Entity Framework migrations:

1. **Run the SQL script first**: Execute `add-ispremium-column.sql`
2. **Generate migration without applying**: 
   ```bash
   dotnet ef migrations add AddIsPremiumToServiceProviders --context ApplicationDbContext
   ```
3. **Mark migration as applied** (since you already ran the SQL):
   ```bash
   dotnet ef database update --context ApplicationDbContext
   ```

### Troubleshooting

**If migration generation fails:**
- Ensure you're in the correct directory with the .csproj file
- Check that your connection string is correct
- Verify that the ServiceProvider entity changes are saved

**If migration conflicts occur:**
- Remove the generated migration file
- Run `dotnet ef migrations remove` to remove the last migration
- Regenerate with a different name

**Performance Optimization:**
After applying the migration, consider running:
```sql
-- Update statistics for the new columns
UPDATE STATISTICS ServiceProviders;

-- Test the new index is being used
SET STATISTICS IO ON;
SELECT * FROM ServiceProviders WHERE IsPremium = 1 ORDER BY Rating DESC;
```

---

## Summary

✅ **Entity Model Updated**: ServiceProvider now includes IsPremium properties  
✅ **DTO Updated**: ProviderSearchResultDto includes IsPremium field  
✅ **Migration Ready**: Ready to generate EF migration  
✅ **SQL Script Available**: Alternative manual approach provided

The Entity Framework model is now ready to support the premium subscription functionality!