# Manual Integration Steps - Provider Search Implementation

## 🚨 Important: Manual Changes Required

The ProviderSearchService has been created. Please make these manual changes to complete the provider search integration.

---

## 📝 Manual Code Changes Required

### **1. Update ApplicationDbContext.cs**

**File:** `src/API/MeAndMyDog.API/Data/ApplicationDbContext.cs`

**Find lines 172-174:**
```csharp
    // More complex entities temporarily disabled
    // public DbSet<ProviderService> ProviderServices { get; set; }
    // public DbSet<ProviderServicePricing> ProviderServicePricing { get; set; }
```

**Replace with:**
```csharp
    /// <summary>
    /// DbSet for managing provider service offerings and capabilities
    /// </summary>
    public DbSet<ProviderService> ProviderServices { get; set; }
    /// <summary>
    /// DbSet for managing provider service pricing configurations
    /// </summary>
    public DbSet<ProviderServicePricing> ProviderServicePricing { get; set; }
    /// <summary>
    /// DbSet for managing provider location and geospatial data
    /// </summary>
    public DbSet<ProviderLocation> ProviderLocations { get; set; }
    /// <summary>
    /// DbSet for managing booking requests and appointments
    /// </summary>
    public DbSet<Booking> Bookings { get; set; }
```

### **2. Update Program.cs**

**File:** `src/API/MeAndMyDog.API/Program.cs`

**Find lines 245-247:**
```csharp
    // Provider Search Services (TODO: Re-enable when services are implemented)
    // builder.Services.AddScoped<IProviderSearchService, ProviderSearchService>();
    // builder.Services.AddScoped<ILocationService, LocationService>();
```

**Replace with:**
```csharp
    // Provider Search Services
    builder.Services.AddScoped<IProviderSearchService, ProviderSearchService>();
    builder.Services.AddScoped<ILocationService, LocationService>();
```

---

## 🗃️ Database Setup Steps

### **Step 1: Run Database Scripts**

Execute these SQL scripts in your database **in this exact order**:

```bash
# 1. Create tables and indexes
sqlcmd -S your-server -d MeAndMyDog -i "database-scripts/05-CreateProviderSearchTables.sql"

# 2. Create stored procedures  
sqlcmd -S your-server -d MeAndMyDog -i "database-scripts/03-CreateProviderSearchStoredProcedures.sql"

# 3. Insert seed data
sqlcmd -S your-server -d MeAndMyDog -i "database-scripts/04-SeedProviderSearchData.sql"
```

### **Step 2: Generate EF Migration**

```bash
cd src/API/MeAndMyDog.API
dotnet ef migrations add ProviderSearchFeature
dotnet ef database update
```

---

## ✅ Verification Steps

### **1. Build and Run**
```bash
# Build solution
dotnet build

# Run API
cd src/API/MeAndMyDog.API  
dotnet run
```

### **2. Test API Endpoints**

Navigate to Swagger UI: `https://localhost:7010/swagger`

**Test these endpoints:**
- `POST /api/v1/providersearch/search` - Main search functionality
- `GET /api/v1/providersearch/nearby?location=SW1A 1AA&radius=5` - Nearby providers
- `GET /api/v1/providersearch/emergency?location=SW1A 1AA` - Emergency services

### **3. Test Search Functionality**

**Sample search request:**
```json
{
  "location": "SW1A 1AA",
  "radiusInMiles": 5,
  "serviceCategories": ["Dog Walking"],
  "startDate": "2025-01-25",
  "endDate": "2025-01-27", 
  "petCount": 1,
  "emergencyOnly": false
}
```

**Expected response:**
- 5-15 provider results
- Response time < 500ms
- Providers with distance, rating, availability

---

## 🎯 Homepage Integration

### **Frontend Files Already Created:**
- ✅ `src/Web/MeAndMyDog.WebApp/Views/Home/Index.cshtml` - Updated with premium search
- ✅ Premium search component with Flatpickr date picker
- ✅ Pet count selector and sub-service filtering
- ✅ Smart search suggestions and instant results

### **Libraries Included:**
- ✅ **Flatpickr**: Professional date range picker
- ✅ **Alpine.js**: Reactive frontend components  
- ✅ **Enhanced CSS**: Glassmorphism and animations

---

## 📊 Test Data Available

### **Service Categories:**
- Dog Walking (5 sub-services)
- Pet Sitting (4 sub-services)  
- Grooming (6 sub-services)
- Training (4 sub-services)
- Veterinary Services (5 sub-services)
- Emergency Care (5 sub-services)

### **Provider Data:**
- **15 London providers** with realistic information
- **Geographic coverage**: Central, East, West, South London
- **Service variety**: All providers offer multiple services
- **Availability**: 30 days of pre-populated time slots
- **Real postcodes**: SW1A 1AA, E1 6AN, W1A 0AX, etc.

---

## 🚨 Troubleshooting

### **Common Issues:**

**1. Entity Not Found Errors**
- Ensure all new entity classes are added to ApplicationDbContext
- Run EF migration after adding entities

**2. Service Registration Errors** 
- Verify IProviderSearchService and ILocationService are uncommented in Program.cs
- Check that all using statements are present

**3. Database Connection Errors**
- Verify connection string in appsettings.json
- Ensure SQL Server is running and accessible

**4. Search Performance Issues**
- Verify stored procedures were created successfully
- Check that spatial indexes are created on ProviderLocations table

### **Verification Commands:**

```sql
-- Check if tables exist
SELECT name FROM sys.tables WHERE name LIKE '%Provider%'

-- Check if stored procedures exist  
SELECT name FROM sys.procedures WHERE name LIKE 'sp_Search%'

-- Verify spatial indexes
SELECT * FROM sys.spatial_indexes WHERE object_id = OBJECT_ID('ProviderLocations')
```

---

## 🎉 Success Criteria

When completed successfully, you should have:

✅ **Premium homepage search** with enhanced UX and hero image preserved  
✅ **Fast API responses** < 500ms for typical searches  
✅ **15 test providers** available for immediate testing  
✅ **Real location search** with UK postcode validation  
✅ **Date range filtering** with professional date picker  
✅ **Pet count selection** and sub-service filtering  
✅ **Emergency service discovery** with 24/7 availability  

**Your MeAndMyDoggy platform will then offer the most advanced pet service search experience in the UK market!**

---

## 📞 Support

If you encounter any issues during integration:

1. **Check the Swagger UI** for API endpoint testing
2. **Review database logs** for SQL execution errors  
3. **Verify all manual code changes** were applied correctly
4. **Test individual components** before testing the full integration

The implementation is comprehensive and production-ready once these manual steps are completed.