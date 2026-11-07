@echo off
REM SerialCOM - Windows-only clean-first build.
REM Usage: build-windows.bat [Debug|Release]  (default: Release)
REM Always cleans old outputs before building; no Linux/macOS RIDs.

setlocal
pushd "%~dp0"
set CONFIG=%~1
if "%CONFIG%"=="" set CONFIG=Release
if /I "%CONFIG%"=="debug" set CONFIG=Debug
if /I "%CONFIG%"=="release" set CONFIG=Release
if /I not "%CONFIG%"=="Debug" if /I not "%CONFIG%"=="Release" (
  echo Usage: build-windows.bat [Debug^|Release]
  exit /b 2
)
where dotnet >nul 2>nul
if errorlevel 1 (
  echo ERROR: dotnet SDK not found on PATH. Install .NET 8 SDK.
  exit /b 1
)

echo Cleaning previous %CONFIG% outputs...
dotnet clean SerialCOM.sln -c "%CONFIG%"

REM Remove stale bin/obj so every build starts fresh on Windows.
REM Scoped to build outputs only - never touches logs/, publish/, or settings.
IF EXIST src\SerialCOM\bin rmdir /s /q src\SerialCOM\bin
IF EXIST src\SerialCOM\obj rmdir /s /q src\SerialCOM\obj
IF EXIST src\SerialCOM.Tests\bin rmdir /s /q src\SerialCOM.Tests\bin
IF EXIST src\SerialCOM.Tests\obj rmdir /s /q src\SerialCOM.Tests\obj

echo Building SerialCOM.sln -c %CONFIG% (Windows only)...
dotnet build SerialCOM.sln -c "%CONFIG%"
if errorlevel 1 (
  echo Build FAILED.
  exit /b 1
)

echo Build succeeded (%CONFIG%, Windows only).
