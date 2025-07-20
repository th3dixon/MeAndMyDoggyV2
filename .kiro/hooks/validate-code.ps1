# MeAndMyDog Code Validation Hook
# Validates code against established standards and architecture rules

Write-Host "Code Validation Hook" -ForegroundColor Cyan
Write-Host "===================" -ForegroundColor Cyan

$violationsFound = 0
$filesChecked = 0

# Function to check for multiple classes in a C# file
function Test-MultipleClasses {
    param([string]$FilePath)
    
    if (-not (Test-Path $FilePath)) {
        return $false
    }
    
    $content = Get-Content $FilePath -Raw -ErrorAction SilentlyContinue
    if (-not $content) {
        return $false
    }
    
    # Find all public class declarations
    $classPattern = 'public\s+(?:partial\s+)?class\s+(\w+)'
    $matches = [regex]::Matches($content, $classPattern)
    
    if ($matches.Count -gt 1) {
        $classNames = $matches | ForEach-Object { $_.Groups[1].Value }
        Write-Host "VIOLATION: Multiple classes in $FilePath" -ForegroundColor Red
        Write-Host "  Classes found: $($classNames -join ', ')" -ForegroundColor Yellow
        Write-Host "  Suggestion: Separate each class into its own file" -ForegroundColor Blue
        return $true
    }
    
    return $false
}

# Function to check for console statements in TypeScript files
function Test-ConsoleStatements {
    param([string]$FilePath)
    
    if (-not (Test-Path $FilePath) -or -not ($FilePath.EndsWith('.ts') -or $FilePath.EndsWith('.tsx'))) {
        return $false
    }
    
    $content = Get-Content $FilePath -Raw -ErrorAction SilentlyContinue
    if (-not $content) {
        return $false
    }
    
    $consolePattern = 'console\.(log|error|warn|info|debug)'
    $matches = [regex]::Matches($content, $consolePattern)
    
    if ($matches.Count -gt 0) {
        Write-Host "VIOLATION: Console statements found in $FilePath" -ForegroundColor Red
        Write-Host "  Found $($matches.Count) console statements" -ForegroundColor Yellow
        Write-Host "  Suggestion: Replace with proper logging service" -ForegroundColor Blue
        return $true
    }
    
    return $false
}

# Check specific files we know might have issues
$keyFiles = @(
    "src/API/MeAndMyDog.API/Models/DTOs/Auth/AuthResponseDto.cs",
    "src/API/MeAndMyDog.API/Models/DTOs/ServiceCatalog/ServiceCategoryDto.cs",
    "src/API/MeAndMyDog.API/Controllers/AuthController.cs",
    "src/API/MeAndMyDog.API/Controllers/ServiceCatalogController.cs",
    "src/API/MeAndMyDog.API/Controllers/LogsController.cs",
    "src/Web/MeAndMyDog.WebApp/src/stores/signalr.ts",
    "src/Web/MeAndMyDog.WebApp/src/stores/auth.ts"
)

$filesToCheck = $keyFiles | Where-Object { Test-Path $_ }

if (-not $filesToCheck) {
    Write-Host "No files found to validate." -ForegroundColor Blue
    exit 0
}

Write-Host "Found $($filesToCheck.Count) files to validate:" -ForegroundColor Green
$filesToCheck | ForEach-Object { Write-Host "  - $_" -ForegroundColor Gray }
Write-Host ""

# Run validations
foreach ($file in $filesToCheck) {
    $filesChecked++
    Write-Host "Validating: $file" -ForegroundColor Cyan
    
    $hasViolation = $false
    
    # Check for multiple classes (C# files)
    if (Test-MultipleClasses $file) {
        $violationsFound++
        $hasViolation = $true
    }
    
    # Check for console statements (TypeScript files)
    if (Test-ConsoleStatements $file) {
        $violationsFound++
        $hasViolation = $true
    }
    
    if (-not $hasViolation) {
        Write-Host "  OK: No violations found" -ForegroundColor Green
    }
    
    Write-Host ""
}

# Summary
Write-Host "Validation Summary:" -ForegroundColor Cyan
Write-Host "  Files checked: $filesChecked" -ForegroundColor White
Write-Host "  Critical violations: $violationsFound" -ForegroundColor White

if ($violationsFound -gt 0) {
    Write-Host ""
    Write-Host "Critical coding standards violations found!" -ForegroundColor Red
    Write-Host "Please fix the violations before proceeding." -ForegroundColor Yellow
    exit 1
} else {
    Write-Host ""
    Write-Host "All files comply with coding standards!" -ForegroundColor Green
    exit 0
}