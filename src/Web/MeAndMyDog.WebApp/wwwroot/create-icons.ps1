# PowerShell script to create icon files for Me and My Doggy

# Base64 encoded 32x32 orange paw print icon
$icon32Base64 = "iVBORw0KGgoAAAANSUhEUgAAACAAAAAgCAYAAABzenr0AAAACXBIWXMAAAsTAAALEwEAmpwYAAADI0lEQVRYhe2Xz0sUYRjHP7O7s7szuzM7s7OzszuzO+ihlBKkHjp06NChQ4cuHTp06NChQ0EQBEEQdOjQoUNBEARBEARBh6BDQRBEERQUFRUVFRUVGRkZGRkZ+e19Z3Zmd2Z3Z3d21yXwgQdm3vd5n+/zPO/zvO8I/ucSxWF1Op1RQRCivN4qYlEUo4Ig2Lnt3DqVRr9Go7ssCIJOxaaqCIKgE0XxssgTZDJZRBCEyEAgEGi12+1mKpXKFotFFovFLCWTyWwikbCkUilrsVi0FAqFYLFYDIZCIbtOp4sKgqDT6/VWr9cbSyaTgVQqFU6lUuF0Oh1OpVJuv9/vFkXR5XQ6nQ6Hw+FwOBx+v9/vcDgcGo1GFwgEAoFAwFkqlUrJZNKdTCZdqVQqmMlkwtlsNpzJZIKlUimQz+d9+Xw+UCgUQvl8PpjP54PFYjFUKpXC5XI5XCqVQuVyOVSr1YLVajVcqVTC1Wo1XK1Ww7VaLSyXy8FyuRyoVqt+r9fr8Xq9bq/X6/F4PC6Px+P0eDxOj8fj9ng87nYLdMJut9vtdrvDZrO57Ha722azuaxWq8tisdhsNpvTZrO5TCaT02QyOUwmk8NoNDoMBoNDr9fb9Xq9w2AwOPR6vUOv1zt0Op1dp9PZtVqtXaPR2LRarU2j0dg0Go1No9HY1Gq1VaVSWVQqlVmlUllFUbSIomgWRdEiiqJZEASzUqk0aTQak0ajMWq1WqNGozFqNBqTWq02q9Vqs0qlMiuVSpMgCEZRFI2CIBhEUTQoCQQC7pWVlbeHh4dvd3d3XywvL79aWVl5ubm5+WZzc/PNwsLC89nZ2afT09PPZmZmnk1PT78YGxt7MD4+fn98fPzB+Pj4/fHx8fuxsbF7o6Ojd0dHR++Ojo7eHR0dvTMyMnJnZGTkzvDw8O3h4eHbw8PDt4eGhm4NDQ3dGhwcvDkwMHCjv7//en9//3W5hxIWi8VcKBTS+Xw+nclkEslkMrG9vR3b2dmJbW9vJzY2NuIrKyvxxcXF+OLiYnxubi42MzMTm5qais3MzMRmZmZi09PTsampqdjU1FRscnIyNjk5GZuYmIhNTEzErl27Fh0bG4va7fZosVhkb28vuru7G93b28P6B/kDHgIM+xOQeL4AAAAASUVORK5CYII="

# Create directories if they don't exist
$wwwrootPath = Split-Path -Parent $MyInvocation.MyCommand.Path

# Decode and save 32x32 icon
$iconBytes = [System.Convert]::FromBase64String($icon32Base64)

# For smaller icon (16x16), we'll create a simple orange square with white paw
$bitmap16 = New-Object System.Drawing.Bitmap(16, 16)
$graphics16 = [System.Drawing.Graphics]::FromImage($bitmap16)
$graphics16.Clear([System.Drawing.Color]::FromArgb(255, 140, 66)) # Orange background

# Draw simple white circle for paw at 16x16
$brush = New-Object System.Drawing.SolidBrush([System.Drawing.Color]::White)
$graphics16.FillEllipse($brush, 4, 6, 8, 6) # Main pad
$graphics16.FillEllipse($brush, 4, 3, 3, 3)  # Left toe
$graphics16.FillEllipse($brush, 9, 3, 3, 3)  # Right toe
$graphics16.FillEllipse($brush, 6, 1, 3, 3)  # Top toe

$bitmap16.Save("$wwwrootPath\favicon-16x16.png", [System.Drawing.Imaging.ImageFormat]::Png)

# Save the 32x32 icon
[System.IO.File]::WriteAllBytes("$wwwrootPath\favicon-32x32.png", $iconBytes)

Write-Host "Icons created successfully!"
Write-Host "Note: For larger icons (180x180, 192x192, 512x512), you'll need to:"
Write-Host "1. Open generate-icons.html in a browser"
Write-Host "2. Click 'Download All Icons'"
Write-Host "3. Move the downloaded files to the wwwroot folder"