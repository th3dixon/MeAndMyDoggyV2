# UK Address Lookup Implementation Guide

## Overview
This guide explains how to implement the free UK address lookup functionality for MeAndMyDoggy platform using Open Postcode Geo data.

## Data Source
We're using **Open Postcode Geo** from GetTheData.com:
- **Website**: https://www.getthedata.com/open-postcode-geo
- **License**: Open Government License
- **Format**: CSV with 1.7 million UK postcodes
- **Updates**: Quarterly
- **Cost**: FREE

## Database Setup Steps

### 1. Create Database Tables
Run the following scripts in order:

```sql
-- Creates the core table structure
07-CreateUKAddressLookupTables.sql

-- Loads sample data for testing
08-SeedUKAddressData.sql

-- Creates optimized views for querying
09-CreateUKAddressViews.sql

-- Creates stored procedures for search functionality
10-CreateUKAddressStoredProcedures.sql
```

### 2. Download and Import Full UK Postcode Data

#### Option A: Using PowerShell Script
```powershell
# Create a file called Download-PostcodeData.ps1
$dataUrl = "https://www.getthedata.com/downloads/open_postcode_geo.csv.zip"
$downloadPath = "C:\Temp\open_postcode_geo.csv.zip"
$extractPath = "C:\Temp\"
$csvPath = "C:\Temp\open_postcode_geo.csv"

# Download the file
Invoke-WebRequest -Uri $dataUrl -OutFile $downloadPath

# Extract the ZIP file
Expand-Archive -Path $downloadPath -DestinationPath $extractPath -Force

Write-Host "Download complete. CSV file is at: $csvPath"
```

#### Option B: Manual Download
1. Go to https://www.getthedata.com/open-postcode-geo
2. Click "Download open_postcode_geo.csv.zip"
3. Extract the CSV file

### 3. Import Data to SQL Server

Run the import script:
```sql
-- This script handles the full import process
11-ImportOpenPostcodeGeoData.sql
```

The script will:
- Create a staging table
- Import CSV data (you'll need to update the file path)
- Process postcodes into normalized tables
- Create lookup cache entries
- Update statistics for optimal performance

## Key Features Implemented

### 1. Database Structure
- **Hierarchical**: Countries → Counties → Cities → Streets → Addresses
- **Postcode-centric**: All addresses linked to postcodes with coordinates
- **Cached lookups**: Pre-computed search results for fast autocomplete
- **Full-text search**: Enabled on key fields for flexible searching

### 2. Search Capabilities
- **Postcode lookup**: Find addresses by full or partial postcode
- **Address autocomplete**: Type-ahead search with ranked results
- **Distance-based search**: Find addresses within X miles of a location
- **City/Street search**: Search by city name or street name
- **Bulk operations**: Import/update addresses efficiently

### 3. Performance Optimizations
- **Indexed views**: Pre-aggregated data for common queries
- **Search cache**: Frequently searched addresses cached
- **Statistics**: Updated for query optimizer
- **Batch processing**: Large imports handled efficiently

## Integration with Registration Page

### Backend Service
Create `AddressLookupService.cs`:

```csharp
public interface IAddressLookupService
{
    Task<List<AddressSearchResult>> SearchAddressesAsync(string searchTerm, int maxResults = 20);
    Task<PostcodeInfo> LookupPostcodeAsync(string postcode);
    Task<Address> GetAddressByIdAsync(int addressId);
}

public class AddressLookupService : IAddressLookupService
{
    private readonly IDbConnection _connection;
    
    public async Task<List<AddressSearchResult>> SearchAddressesAsync(string searchTerm, int maxResults = 20)
    {
        using var cmd = _connection.CreateCommand();
        cmd.CommandText = "sp_AddressAutocomplete";
        cmd.CommandType = CommandType.StoredProcedure;
        cmd.Parameters.Add(new SqlParameter("@SearchTerm", searchTerm));
        cmd.Parameters.Add(new SqlParameter("@MaxResults", maxResults));
        
        // Execute and map results
    }
}
```

### API Endpoint
Add to controller:

```csharp
[HttpGet("api/address/search")]
public async Task<IActionResult> SearchAddresses([FromQuery] string q, [FromQuery] int max = 20)
{
    if (string.IsNullOrWhiteSpace(q) || q.Length < 3)
        return BadRequest("Search term must be at least 3 characters");
        
    var results = await _addressLookupService.SearchAddressesAsync(q, max);
    return Ok(results);
}

[HttpGet("api/address/postcode/{postcode}")]
public async Task<IActionResult> LookupPostcode(string postcode)
{
    var result = await _addressLookupService.LookupPostcodeAsync(postcode);
    if (result == null)
        return NotFound();
        
    return Ok(result);
}
```

### Frontend Integration
Add to Register.cshtml:

```javascript
// Alpine.js component for address lookup
function addressLookup() {
    return {
        searchTerm: '',
        results: [],
        selectedAddress: null,
        searching: false,
        
        async search() {
            if (this.searchTerm.length < 3) {
                this.results = [];
                return;
            }
            
            this.searching = true;
            try {
                const response = await fetch(`/api/address/search?q=${encodeURIComponent(this.searchTerm)}`);
                if (response.ok) {
                    this.results = await response.json();
                }
            } finally {
                this.searching = false;
            }
        },
        
        selectAddress(address) {
            this.selectedAddress = address;
            // Populate form fields
            document.getElementById('AddressLine1').value = address.addressLine1;
            document.getElementById('AddressLine2').value = address.addressLine2 || '';
            document.getElementById('City').value = address.city;
            document.getElementById('County').value = address.county;
            document.getElementById('PostCode').value = address.postcodeFormatted;
            this.results = [];
            this.searchTerm = address.displayText;
        }
    }
}
```

## Maintenance

### Updating Postcode Data
1. Download latest data from GetTheData quarterly
2. Re-run the import process
3. Refresh the address cache: `EXEC sp_RefreshAddressCache`

### Performance Monitoring
```sql
-- Check cache hit rate
SELECT 
    COUNT(*) as TotalSearches,
    AVG(UseCount) as AvgUseCount,
    MAX(UseCount) as MostSearched
FROM AddressLookupCache
WHERE LastUsed > DATEADD(day, -30, GETUTCDATE());

-- Find slow queries
SELECT TOP 10
    qs.execution_count,
    qs.total_elapsed_time / qs.execution_count as avg_elapsed_time,
    SUBSTRING(qt.text, (qs.statement_start_offset/2) + 1,
    ((CASE qs.statement_end_offset
        WHEN -1 THEN DATALENGTH(qt.text)
        ELSE qs.statement_end_offset
    END - qs.statement_start_offset)/2) + 1) as query_text
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
WHERE qt.text LIKE '%Address%'
ORDER BY avg_elapsed_time DESC;
```

## Cost Comparison

### Our Solution (Open Postcode Geo)
- **Data cost**: £0
- **Storage**: ~500MB for full UK dataset
- **Updates**: Free quarterly updates
- **API calls**: Unlimited (self-hosted)
- **Monthly cost**: £0

### Commercial Alternatives
- **Google Places API**: £14 per 1,000 requests
- **Postcodes.io**: Free but rate-limited
- **Royal Mail PAF**: £3,950+ per year
- **Loqate**: £0.04 per lookup

For 10,000 monthly registrations, we save approximately £140/month compared to Google Places API.

## Troubleshooting

### Import Errors
- **"Bulk insert permission denied"**: Grant bulk insert permissions or use Import Wizard
- **"Cannot find file"**: Update file path in BULK INSERT statement
- **"Data truncation"**: Check CSV format matches expected schema

### Performance Issues
- Run `UPDATE STATISTICS` on all address tables
- Check index fragmentation: `DBCC SHOWCONTIG`
- Consider partitioning PostcodeAreas table by region

### Search Quality
- Adjust SearchRank values for better results
- Add common misspellings to cache
- Consider fuzzy matching with Levenshtein distance

## Future Enhancements

1. **Address validation**: Cross-reference with Royal Mail PAF for accuracy
2. **What3Words integration**: Add W3W codes for precise location
3. **International support**: Add support for Irish postcodes (Eircode)
4. **Mobile optimization**: Implement native mobile address pickers
5. **Analytics**: Track popular search areas for business insights