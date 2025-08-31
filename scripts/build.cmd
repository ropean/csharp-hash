@echo off
setlocal enabledelayedexpansion

rem Navigate to repo root
pushd "%~dp0.."

set SELF_CONTAINED=false
if /I "%~1"=="sc" set SELF_CONTAINED=true
if /I "%~1"=="self-contained" set SELF_CONTAINED=true

rem Ensure no running instance is locking files
taskkill /im CSharpHash.exe /f >nul 2>&1

echo Publishing single-file (Release, win-x64, trim=off, self-contained=%SELF_CONTAINED%)...
dotnet publish CSharpHash -c Release -r win-x64 -p:PublishSingleFile=true --self-contained %SELF_CONTAINED%
if errorlevel 1 goto :error

echo.
echo Success. Output:
for /f "delims=" %%A in ('powershell -NoProfile -Command "(Resolve-Path 'CSharpHash/bin/Release/net8.0-windows/win-x64/publish').Path"') do set PUBDIR=%%A
echo   %PUBDIR%\

popd
exit /b 0

:error
set ERR=%ERRORLEVEL%
echo Build failed with errorlevel %ERR%.
popd
exit /b %ERR%


