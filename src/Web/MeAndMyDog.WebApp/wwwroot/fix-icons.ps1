# PowerShell script to create properly sized icons for the manifest
Write-Host "Creating properly sized placeholder icons..." -ForegroundColor Green

function Create-PlaceholderIcon {
    param([string]$FilePath, [int]$Width, [int]$Height)
    Add-Type -AssemblyName System.Drawing
    $bitmap = New-Object System.Drawing.Bitmap($Width, $Height)
    $graphics = [System.Drawing.Graphics]::FromImage($bitmap)
    $orangeBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::FromArgb(255, 255, 140, 66))
    $graphics.FillRectangle($orangeBrush, 0, 0, $Width, $Height)
    $whiteBrush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
    $circleSize = [Math]::Min($Width, $Height) * 0.4
    $x = ($Width - $circleSize) / 2
    $y = ($Height - $circleSize) / 2
    $graphics.FillEllipse($whiteBrush, $x, $y, $circleSize, $circleSize)
    $bitmap.Save($FilePath, [System.Drawing.Imaging.ImageFormat]::Png)
    $graphics.Dispose(); $bitmap.Dispose(); $orangeBrush.Dispose(); $whiteBrush.Dispose()
    Write-Host "Created: $FilePath ($Width x $Height)" -ForegroundColor Blue
}

Create-PlaceholderIcon "favicon-16x16.png" 16 16
Create-PlaceholderIcon "favicon-32x32.png" 32 32
Create-PlaceholderIcon "apple-touch-icon.png" 180 180
Create-PlaceholderIcon "android-chrome-192x192.png" 192 192
Create-PlaceholderIcon "android-chrome-512x512.png" 512 512
Copy-Item "favicon-32x32.png" "favicon.ico" -Force
Write-Host "All icons created successfully\! Manifest size mismatch resolved." -ForegroundColor Green
