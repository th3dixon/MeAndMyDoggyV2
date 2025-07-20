# PowerShell script to convert Open Postcode Geo CSV to SQL INSERT statements
param(
    [string]$CsvPath = "C:\Temp\open_postcode_geo.csv",
    [string]$OutputPath = "C:\Temp\PostcodeInserts.sql",
    [int]$BatchSize = 1000
)

Write-Host "Starting CSV to SQL conversion..." -ForegroundColor Green
Write-Host "Reading from: $CsvPath"
Write-Host "Writing to: $OutputPath"

# Check if CSV exists
if (-not (Test-Path $CsvPath)) {
    Write-Host "CSV file not found at $CsvPath" -ForegroundColor Red
    exit 1
}

# Start the output file
$header = @"
-- =============================================
-- Open Postcode Geo Data INSERT Statements
-- Generated from CSV file
-- =============================================

SET NOCOUNT ON;
GO

"@

$header | Out-File -FilePath $OutputPath -Encoding UTF8

# Read and process CSV
$lineCount = 0
$batchCount = 0
$currentBatch = @()

# Create StreamReader for better performance with large files
$reader = [System.IO.StreamReader]::new($CsvPath)
$headerLine = $reader.ReadLine() # Skip header

Write-Host "Processing CSV file..." -ForegroundColor Yellow

while ($null -ne ($line = $reader.ReadLine())) {
    $lineCount++
    
    # Parse CSV line (handle quoted values)
    $values = $line | ConvertFrom-Csv -Header 'postcode','status','usertype','easting','northing','positional_quality_indicator','country','latitude','longitude','postcode_no_space','postcode_fixed_width_seven','postcode_fixed_width_eight','postcode_area','postcode_district','postcode_sector','outcode','incode'
    
    # Build INSERT statement
    $insertValues = @(
        if ($values.postcode) { "N'$($values.postcode.Replace("'", "''"))'" } else { "NULL" }
        if ($values.status) { "N'$($values.status.Replace("'", "''"))'" } else { "NULL" }
        if ($values.usertype) { "N'$($values.usertype.Replace("'", "''"))'" } else { "NULL" }
        if ($values.easting -match '^\d+$') { $values.easting } else { "NULL" }
        if ($values.northing -match '^\d+$') { $values.northing } else { "NULL" }
        if ($values.positional_quality_indicator -match '^\d+$') { $values.positional_quality_indicator } else { "NULL" }
        if ($values.country) { "N'$($values.country.Replace("'", "''"))'" } else { "NULL" }
        if ($values.latitude -match '^-?\d+\.?\d*$') { $values.latitude } else { "NULL" }
        if ($values.longitude -match '^-?\d+\.?\d*$') { $values.longitude } else { "NULL" }
        if ($values.postcode_no_space) { "N'$($values.postcode_no_space.Replace("'", "''"))'" } else { "NULL" }
        if ($values.postcode_fixed_width_seven) { "N'$($values.postcode_fixed_width_seven.Replace("'", "''"))'" } else { "NULL" }
        if ($values.postcode_fixed_width_eight) { "N'$($values.postcode_fixed_width_eight.Replace("'", "''"))'" } else { "NULL" }
        if ($values.postcode_area) { "N'$($values.postcode_area.Replace("'", "''"))'" } else { "NULL" }
        if ($values.postcode_district) { "N'$($values.postcode_district.Replace("'", "''"))'" } else { "NULL" }
        if ($values.postcode_sector) { "N'$($values.postcode_sector.Replace("'", "''"))'" } else { "NULL" }
        if ($values.outcode) { "N'$($values.outcode.Replace("'", "''"))'" } else { "NULL" }
        if ($values.incode) { "N'$($values.incode.Replace("'", "''"))'" } else { "NULL" }
    )
    
    $currentBatch += "($($insertValues -join ', '))"
    
    # Write batch when it reaches the batch size
    if ($currentBatch.Count -ge $BatchSize) {
        $batchCount++
        $insertStatement = @"
-- Batch $batchCount
INSERT INTO [dbo].[PostcodeImportStaging] 
    ([postcode], [status], [usertype], [easting], [northing], [positional_quality_indicator], 
     [country], [latitude], [longitude], [postcode_no_space], [postcode_fixed_width_seven], 
     [postcode_fixed_width_eight], [postcode_area], [postcode_district], [postcode_sector], 
     [outcode], [incode])
VALUES
$($currentBatch -join ",`n");
GO

"@
        $insertStatement | Out-File -FilePath $OutputPath -Encoding UTF8 -Append
        $currentBatch = @()
        
        if ($lineCount % 10000 -eq 0) {
            Write-Host "Processed $lineCount records..." -ForegroundColor Cyan
        }
    }
}

# Write any remaining records
if ($currentBatch.Count -gt 0) {
    $batchCount++
    $insertStatement = @"
-- Batch $batchCount (Final)
INSERT INTO [dbo].[PostcodeImportStaging] 
    ([postcode], [status], [usertype], [easting], [northing], [positional_quality_indicator], 
     [country], [latitude], [longitude], [postcode_no_space], [postcode_fixed_width_seven], 
     [postcode_fixed_width_eight], [postcode_area], [postcode_district], [postcode_sector], 
     [outcode], [incode])
VALUES
$($currentBatch -join ",`n");
GO

"@
    $insertStatement | Out-File -FilePath $OutputPath -Encoding UTF8 -Append
}

$reader.Close()

# Add footer
$footer = @"
-- =============================================
-- Import complete
-- Total records: $lineCount
-- Total batches: $batchCount
-- =============================================

PRINT 'Import complete. Total records inserted: $lineCount';
GO
"@

$footer | Out-File -FilePath $OutputPath -Encoding UTF8 -Append

Write-Host ""
Write-Host "Conversion complete!" -ForegroundColor Green
Write-Host "Total records processed: $lineCount"
Write-Host "Total batches created: $batchCount"
Write-Host "Output file: $OutputPath"
Write-Host ""
Write-Host "Next steps:" -ForegroundColor Yellow
Write-Host "1. Run the staging table creation script (00-CreatePostcodeImportStagingTable.sql)"
Write-Host "2. Run the generated SQL file: $OutputPath"
Write-Host "3. Run the import processing script (11-ImportOpenPostcodeGeoData.sql)"