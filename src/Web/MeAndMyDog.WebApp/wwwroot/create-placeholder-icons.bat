@echo off
echo Creating placeholder icon files...

REM Create minimal valid PNG files (1x1 pixel orange)
REM These are base64 encoded minimal PNGs

echo Creating favicon-16x16.png...
powershell -Command "[System.IO.File]::WriteAllBytes('favicon-16x16.png', [System.Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg=='))"

echo Creating favicon-32x32.png...
powershell -Command "[System.IO.File]::WriteAllBytes('favicon-32x32.png', [System.Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg=='))"

echo Creating apple-touch-icon.png...
powershell -Command "[System.IO.File]::WriteAllBytes('apple-touch-icon.png', [System.Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg=='))"

echo Creating android-chrome-192x192.png...
powershell -Command "[System.IO.File]::WriteAllBytes('android-chrome-192x192.png', [System.Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg=='))"

echo Creating android-chrome-512x512.png...
powershell -Command "[System.IO.File]::WriteAllBytes('android-chrome-512x512.png', [System.Convert]::FromBase64String('iVBORw0KGgoAAAANSUhEUgAAAAEAAAABCAYAAAAfFcSJAAAADUlEQVR42mP8/5+hHgAHggJ/PchI7wAAAABJRU5ErkJggg=='))"

echo.
echo Placeholder icons created successfully!
echo.
echo To create proper icons:
echo 1. Open generate-icons.html in a web browser
echo 2. Click "Download All Icons"
echo 3. Replace these placeholder files with the downloaded icons
echo.
pause