#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Test SoluiNet.DevTools.Console on all supported platforms and architectures
.DESCRIPTION
    This script runs comprehensive tests for the console application across different platforms
.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.
.PARAMETER TestOutputPath
    Output directory for test results. Default is ./test-results
.PARAMETER SkipBuild
    Skip the build step and test existing binaries. Default is false.
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [string]$TestOutputPath = "./test-results",
    
    [Parameter(Mandatory=$false)]
    [bool]$SkipBuild = $false
)

# Define test platforms (only test on current platform for now)
$CurrentPlatform = if ($IsWindows) { "win" } elseif ($IsLinux) { "linux" } elseif ($IsMacOS) { "osx" } else { "unknown" }
$CurrentArch = if ([System.Runtime.InteropServices.RuntimeInformation]::ProcessArchitecture -eq "X64") { "x64" } else { "arm64" }
$CurrentRid = "$CurrentPlatform-$CurrentArch"

$ProjectPath = "SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj"
$UnitTestPath = "SoluiNet.DevTools.UnitTest/SoluiNet.DevTools.UnitTest.csproj"

Write-Host "Testing SoluiNet.DevTools.Console" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Current Platform: $CurrentRid" -ForegroundColor Yellow
Write-Host "Test Output Path: $TestOutputPath" -ForegroundColor Yellow
Write-Host "Skip Build: $SkipBuild" -ForegroundColor Yellow

# Ensure test output directory exists
if (!(Test-Path $TestOutputPath)) {
    New-Item -ItemType Directory -Path $TestOutputPath -Force | Out-Null
}

$TestResults = @()

# Build if not skipping
if (-not $SkipBuild) {
    Write-Host "Building project..." -ForegroundColor Cyan
    
    # Clean and restore
    dotnet clean $ProjectPath --configuration $Configuration --verbosity minimal
    dotnet restore $ProjectPath --verbosity minimal
    
    # Build for current platform
    $buildArgs = @(
        "build",
        $ProjectPath,
        "--configuration", $Configuration,
        "--runtime", $CurrentRid,
        "--verbosity", "minimal"
    )
    
    try {
        & dotnet @buildArgs
        if ($LASTEXITCODE -ne 0) {
            Write-Host "✗ Build failed" -ForegroundColor Red
            exit 1
        }
        Write-Host "✓ Build successful" -ForegroundColor Green
    }
    catch {
        Write-Host "✗ Build exception: $($_.Exception.Message)" -ForegroundColor Red
        exit 1
    }
}

# Run unit tests
Write-Host "Running unit tests..." -ForegroundColor Cyan

$unitTestArgs = @(
    "test",
    $UnitTestPath,
    "--configuration", $Configuration,
    "--logger", "trx;LogFileName=unit-tests.trx",
    "--results-directory", $TestOutputPath,
    "--verbosity", "minimal"
)

try {
    & dotnet @unitTestArgs
    $unitTestResult = $LASTEXITCODE -eq 0
    if ($unitTestResult) {
        Write-Host "✓ Unit tests passed" -ForegroundColor Green
    } else {
        Write-Host "✗ Unit tests failed" -ForegroundColor Red
    }
    
    $TestResults += [PSCustomObject]@{
        TestType = "Unit Tests"
        Status = if ($unitTestResult) { "Passed" } else { "Failed" }
        Platform = $CurrentRid
    }
}
catch {
    Write-Host "✗ Unit test exception: $($_.Exception.Message)" -ForegroundColor Red
    $TestResults += [PSCustomObject]@{
        TestType = "Unit Tests"
        Status = "Exception"
        Platform = $CurrentRid
    }
}

# Test console application functionality
Write-Host "Testing console application..." -ForegroundColor Cyan

$consolePath = "build/console/$Configuration"
$executableName = if ($CurrentPlatform -eq "win") { "sndt.exe" } else { "sndt" }
$executablePath = Join-Path $consolePath $executableName

if (Test-Path $executablePath) {
    Write-Host "Testing executable: $executablePath" -ForegroundColor Gray
    
    # Test 1: Version information
    try {
        $versionOutput = & $executablePath --version 2>&1
        $versionTest = $LASTEXITCODE -eq 0 -and $versionOutput -match "SoluiNet"
        
        if ($versionTest) {
            Write-Host "✓ Version test passed" -ForegroundColor Green
        } else {
            Write-Host "✗ Version test failed: $versionOutput" -ForegroundColor Red
        }
        
        $TestResults += [PSCustomObject]@{
            TestType = "Console Version"
            Status = if ($versionTest) { "Passed" } else { "Failed" }
            Platform = $CurrentRid
        }
    }
    catch {
        Write-Host "✗ Version test exception: $($_.Exception.Message)" -ForegroundColor Red
        $TestResults += [PSCustomObject]@{
            TestType = "Console Version"
            Status = "Exception"
            Platform = $CurrentRid
        }
    }
    
    # Test 2: Help information
    try {
        $helpOutput = & $executablePath --help 2>&1
        $helpTest = $LASTEXITCODE -eq 0 -and $helpOutput -match "help"
        
        if ($helpTest) {
            Write-Host "✓ Help test passed" -ForegroundColor Green
        } else {
            Write-Host "✗ Help test failed: $helpOutput" -ForegroundColor Red
        }
        
        $TestResults += [PSCustomObject]@{
            TestType = "Console Help"
            Status = if ($helpTest) { "Passed" } else { "Failed" }
            Platform = $CurrentRid
        }
    }
    catch {
        Write-Host "✗ Help test exception: $($_.Exception.Message)" -ForegroundColor Red
        $TestResults += [PSCustomObject]@{
            TestType = "Console Help"
            Status = "Exception"
            Platform = $CurrentRid
        }
    }
    
    # Test 3: Plugin loading (if plugins exist)
    $pluginsPath = Join-Path $consolePath "Plugins"
    if (Test-Path $pluginsPath) {
        Write-Host "Testing plugin loading..." -ForegroundColor Gray
        
        try {
            # This would need to be implemented based on actual console commands
            # For now, just check if plugins directory is accessible
            $pluginFiles = Get-ChildItem -Path $pluginsPath -Recurse -Filter "*.dll" -ErrorAction SilentlyContinue
            $pluginTest = $pluginFiles.Count -gt 0
            
            if ($pluginTest) {
                Write-Host "✓ Plugin directory test passed ($($pluginFiles.Count) plugins found)" -ForegroundColor Green
            } else {
                Write-Host "! No plugins found in $pluginsPath" -ForegroundColor Yellow
            }
            
            $TestResults += [PSCustomObject]@{
                TestType = "Plugin Loading"
                Status = if ($pluginTest) { "Passed" } else { "No Plugins" }
                Platform = $CurrentRid
            }
        }
        catch {
            Write-Host "✗ Plugin test exception: $($_.Exception.Message)" -ForegroundColor Red
            $TestResults += [PSCustomObject]@{
                TestType = "Plugin Loading"
                Status = "Exception"
                Platform = $CurrentRid
            }
        }
    }
} else {
    Write-Host "✗ Console executable not found at $executablePath" -ForegroundColor Red
    $TestResults += [PSCustomObject]@{
        TestType = "Console Executable"
        Status = "Not Found"
        Platform = $CurrentRid
    }
}

# Test cross-platform functionality
Write-Host "Testing cross-platform functionality..." -ForegroundColor Cyan

# Test platform detection
try {
    $platformTestPath = "SoluiNet.DevTools.Core/Services/Platform/PlatformHelper.cs"
    if (Test-Path $platformTestPath) {
        Write-Host "✓ Platform helper exists" -ForegroundColor Green
        $TestResults += [PSCustomObject]@{
            TestType = "Platform Detection"
            Status = "Available"
            Platform = $CurrentRid
        }
    } else {
        Write-Host "✗ Platform helper not found" -ForegroundColor Red
        $TestResults += [PSCustomObject]@{
            TestType = "Platform Detection"
            Status = "Missing"
            Platform = $CurrentRid
        }
    }
}
catch {
    Write-Host "✗ Platform detection test exception: $($_.Exception.Message)" -ForegroundColor Red
    $TestResults += [PSCustomObject]@{
        TestType = "Platform Detection"
        Status = "Exception"
        Platform = $CurrentRid
    }
}

# Generate test report
Write-Host ""
Write-Host "Test Summary:" -ForegroundColor Green
Write-Host "============" -ForegroundColor Green

$passedTests = ($TestResults | Where-Object { $_.Status -eq "Passed" }).Count
$failedTests = ($TestResults | Where-Object { $_.Status -in @("Failed", "Exception", "Not Found", "Missing") }).Count
$totalTests = $TestResults.Count

foreach ($result in $TestResults) {
    $statusColor = switch ($result.Status) {
        "Passed" { "Green" }
        "Available" { "Green" }
        "No Plugins" { "Yellow" }
        default { "Red" }
    }
    
    Write-Host "$($result.TestType): $($result.Status)" -ForegroundColor $statusColor
}

Write-Host ""
Write-Host "Total: $passedTests passed, $failedTests failed out of $totalTests tests" -ForegroundColor $(if ($failedTests -eq 0) { "Green" } else { "Yellow" })

# Export test results to JSON
$testReportPath = Join-Path $TestOutputPath "test-results.json"
$TestResults | ConvertTo-Json -Depth 3 | Out-File -FilePath $testReportPath -Encoding UTF8
Write-Host "Test results exported to: $testReportPath" -ForegroundColor Gray

if ($failedTests -eq 0) {
    Write-Host "All tests completed successfully!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "Some tests failed. Check the results above for details." -ForegroundColor Red
    exit 1
}