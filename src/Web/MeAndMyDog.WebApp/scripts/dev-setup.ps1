# Development Setup Script for MeAndMyDoggy Vue.js Frontend

Write-Host "🐕 Setting up MeAndMyDoggy Vue.js Development Environment" -ForegroundColor Green

# Check if Node.js is installed
try {
    $nodeVersion = node --version
    Write-Host "✅ Node.js version: $nodeVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ Node.js is not installed. Please install Node.js 18+ from https://nodejs.org/" -ForegroundColor Red
    exit 1
}

# Check if npm is available
try {
    $npmVersion = npm --version
    Write-Host "✅ npm version: $npmVersion" -ForegroundColor Green
} catch {
    Write-Host "❌ npm is not available" -ForegroundColor Red
    exit 1
}

# Install dependencies
Write-Host "📦 Installing npm dependencies..." -ForegroundColor Yellow
npm install

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Failed to install dependencies" -ForegroundColor Red
    exit 1
}

Write-Host "✅ Dependencies installed successfully" -ForegroundColor Green

# Create necessary directories
$directories = @(
    "wwwroot/dist",
    "wwwroot/images",
    "src/views",
    "src/views/auth",
    "src/views/dogs",
    "src/views/services",
    "src/views/bookings",
    "src/views/messages",
    "src/views/profile",
    "src/views/settings",
    "src/views/admin",
    "src/views/errors"
)

foreach ($dir in $directories) {
    if (!(Test-Path $dir)) {
        New-Item -ItemType Directory -Path $dir -Force | Out-Null
        Write-Host "📁 Created directory: $dir" -ForegroundColor Blue
    }
}

# Copy placeholder images if they don't exist
$placeholderImages = @(
    "logo.svg",
    "logo-white.svg",
    "default-avatar.png",
    "hero-dog.jpg"
)

foreach ($image in $placeholderImages) {
    $imagePath = "wwwroot/images/$image"
    if (!(Test-Path $imagePath)) {
        # Create placeholder file
        New-Item -ItemType File -Path $imagePath -Force | Out-Null
        Write-Host "🖼️  Created placeholder: $imagePath" -ForegroundColor Blue
    }
}

Write-Host ""
Write-Host "🎉 Setup complete! You can now run:" -ForegroundColor Green
Write-Host "   npm run dev    - Start Vue.js development server" -ForegroundColor Cyan
Write-Host "   npm run build  - Build for production" -ForegroundColor Cyan
Write-Host ""
Write-Host "🔗 Development URLs:" -ForegroundColor Yellow
Write-Host "   Vue.js Dev Server: http://localhost:5173" -ForegroundColor Cyan
Write-Host "   ASP.NET Core App:  https://localhost:56682" -ForegroundColor Cyan
Write-Host "   API Server:        https://localhost:7010" -ForegroundColor Cyan