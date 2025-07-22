#!/usr/bin/env powershell
<#
.SYNOPSIS
    MeAndMyDog Code Quality Validation Runner
    
.DESCRIPTION
    Automatically runs code validation hooks to ensure compliance with coding standards.
    This script should be run after completing a set of tasks to maintain code quality.
    
.EXAMPLE
    .\run-validation.ps1
    
.NOTES
    Requires Python 3.7+ to be installed and available in PATH
#>

Write-Host ""
Write-Host "🚀 MeAndMyDog Code Quality Check" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""

# Check if Python is available
try {
    $pythonVersion = python --version 2>$null
    if ($LASTEXITCODE -ne 0) {
        throw "Python not found"
    }
    Write-Host "✅ Python detected: $pythonVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Python is not installed or not in PATH" -ForegroundColor Red
    Write-Host "Please install Python 3.7+ and try again" -ForegroundColor Yellow
    Read-Host "Press Enter to exit"
    exit 1
}

# Change to project root directory
$scriptPath = Split-Path -Parent $MyInvocation.MyCommand.Path
$projectRoot = Split-Path -Parent $scriptPath
Set-Location $projectRoot

Write-Host "📁 Working directory: $(Get-Location)" -ForegroundColor Cyan
Write-Host ""

# Check if the code validation hook exists
$hookPath = Join-Path "hooks" "code-validation-hook.py"
if (-not (Test-Path $hookPath)) {
    Write-Host "❌ Code validation hook not found at: $hookPath" -ForegroundColor Red
    Read-Host "Press Enter to exit"
    exit 1
}

# Run the code validation hook
Write-Host "🔍 Running code validation..." -ForegroundColor Yellow
try {
    $process = Start-Process -FilePath "python" -ArgumentList $hookPath -Wait -PassThru -NoNewWindow
    $exitCode = $process.ExitCode
    
    if ($exitCode -eq 0) {
        Write-Host ""
        Write-Host "✅ Code validation completed successfully!" -ForegroundColor Green
        Write-Host "📊 Check CODE_VALIDATION_REPORT.md for detailed results" -ForegroundColor Cyan
        
        # Display summary if report exists
        $reportPath = "CODE_VALIDATION_REPORT.md"
        if (Test-Path $reportPath) {
            Write-Host ""
            Write-Host "📋 Quick Summary:" -ForegroundColor Yellow
            $reportContent = Get-Content $reportPath -Raw
            if ($reportContent -match '\*\*Compliance Score\*\*: ([\d.]+)/100') {
                $score = $matches[1]
                Write-Host "   Compliance Score: $score/100" -ForegroundColor $(if ([double]$score -ge 90) { "Green" } elseif ([double]$score -ge 70) { "Yellow" } else { "Red" })
            }
            if ($reportContent -match '\*\*Total Violations\*\*: (\d+)') {
                $violations = $matches[1]
                Write-Host "   Total Violations: $violations" -ForegroundColor $(if ($violations -eq "0") { "Green" } else { "Yellow" })
            }
        }
    } else {
        Write-Host ""
        Write-Host "❌ Code validation found issues!" -ForegroundColor Red
        Write-Host "📊 Check CODE_VALIDATION_REPORT.md for details" -ForegroundColor Cyan
        Write-Host "Please fix critical errors before proceeding" -ForegroundColor Yellow
    }
} catch {
    Write-Host ""
    Write-Host "❌ Error running code validation: $($_.Exception.Message)" -ForegroundColor Red
    $exitCode = 1
}

Write-Host ""
Write-Host "🏁 Validation complete." -ForegroundColor Blue
if ($Host.Name -eq "ConsoleHost") {
    Read-Host "Press Enter to exit"
}

exit $exitCode