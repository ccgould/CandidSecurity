@echo off
setlocal enabledelayedexpansion

:: === CONFIGURATION ===
set "sourceDir=C:\Users\SPV_s\Downloads\Airline Logos\temp"
set "destDir=C:\Users\SPV_s\source\repos\CandidSecurity\CandidQV\Resources\Images\lowercase"

echo Source: %sourceDir%
echo Destination: %destDir%

if not exist "%destDir%" (
    echo Creating destination folder...
    mkdir "%destDir%"
)

echo Starting file loop...

for %%F in ("%sourceDir%\*.*") do (
    echo Found file: %%~nxF
    for /f %%L in ('powershell -nologo -command "[System.IO.Path]::GetFileName('%%~nxF').ToLower()"') do (
        echo Renaming to: %%L
        copy "%%F" "%destDir%\%%L"
    )
)

echo All done.
pause
