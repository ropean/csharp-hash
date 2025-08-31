@echo off
setlocal

pushd "%~dp0.."

echo Cleaning bin/obj...
if exist CSharpHash\bin rmdir /s /q CSharpHash\bin
if exist CSharpHash\obj rmdir /s /q CSharpHash\obj

echo Running dotnet clean...
dotnet clean CSharpHash >nul 2>&1

echo Done.
popd
exit /b 0


