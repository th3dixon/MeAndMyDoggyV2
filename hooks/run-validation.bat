@echo off
REM MeAndMyDog Code Validation Runner
REM Runs code validation hooks to ensure compliance with coding standards

echo.
echo 🚀 MeAndMyDog Code Quality Check
echo ================================
echo.

REM Check if Python is available
python --version >nul 2>&1
if %errorlevel% neq 0 (
    echo ❌ Python is not installed or not in PATH
    echo Please install Python 3.7+ and try again
    pause
    exit /b 1
)

REM Change to project root directory
cd /d "%~dp0\.."
echo 📁 Working directory: %CD%
echo.

REM Run the code validation hook
echo 🔍 Running code validation...
python hooks\code-validation-hook.py

REM Check the result
if %errorlevel% equ 0 (
    echo.
    echo ✅ Code validation completed successfully!
    echo 📊 Check CODE_VALIDATION_REPORT.md for detailed results
) else (
    echo.
    echo ❌ Code validation found issues!
    echo 📊 Check CODE_VALIDATION_REPORT.md for details
    echo Please fix critical errors before proceeding
)

echo.
echo 🏁 Validation complete. Press any key to exit...
pause >nul
exit /b %errorlevel%