#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build SoluiNet.DevTools.Console for all supported platforms and architectures
.DESCRIPTION
    This script builds the console application for Windows, Linux, and macOS on both x64 and ARM64 architectures
.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.
.PARAMETER OutputPath
    Output directory for build artifacts. Default is ./build/multi-platform
.PARAMETER SelfContained
    Whether to create self-contained deployments. Default is false.
.PARAMETER SingleFile
    Whether to publish as single file. Default is true.
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = "./build/multi-platform",
    
    [Parameter(Mandatory=$false)]
    [bool]$SelfContained = $false,
    
    [Parameter(Mandatory=$false)]
    [bool]$SingleFile = $true
)

# Define supported runtime identifiers
$RuntimeIdentifiers = @(
    "win-x64",
    "win-arm64", 
    "linux-x64",
    "linux-arm64",
    "osx-x64",
    "osx-arm64"
)

# Project path
$ProjectPath = "SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj"

Write-Host "Starting multi-platform build for SoluiNet.DevTools.Console" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow
Write-Host "Self-Contained: $SelfContained" -ForegroundColor Yellow
Write-Host "Single File: $SingleFile" -ForegroundColor Yellow

# Ensure output directory exists
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# Clean previous builds
Write-Host "Cleaning previous builds..." -ForegroundColor Yellow
dotnet clean $ProjectPath --configuration $Configuration --verbosity minimal

# Restore dependencies
Write-Host "Restoring dependencies..." -ForegroundColor Yellow
dotnet restore $ProjectPath --verbosity minimal

$BuildResults = @()

foreach ($rid in $RuntimeIdentifiers) {
    Write-Host "Building for $rid..." -ForegroundColor Cyan
    
    $platformOutputPath = Join-Path $OutputPath $rid
    
    # Build arguments
    $buildArgs = @(
        "publish",
        $ProjectPath,
        "--configuration", $Configuration,
        "--runtime", $rid,
        "--output", $platformOutputPath,
        "--verbosity", "minimal"
    )
    
    if ($SelfContained) {
        $buildArgs += "--self-contained"
    } else {
        $buildArgs += "--no-self-contained"
    }
    
    if ($SingleFile) {
        $buildArgs += "-p:PublishSingleFile=true"
    }
    
    # Additional platform-specific optimizations
    $buildArgs += "-p:PublishTrimmed=false"  # Disable trimming to avoid plugin loading issues
    $buildArgs += "-p:PublishReadyToRun=true"  # Enable ReadyToRun for better startup performance
    
    try {
        $result = & dotnet @buildArgs
        $exitCode = $LASTEXITCODE
        
        if ($exitCode -eq 0) {
            Write-Host "✓ Successfully built for $rid" -ForegroundColor Green
            
            # Verify output files exist
            $expectedExecutable = if ($rid.StartsWith("win")) { "sndt.exe" } else { "sndt" }
            $executablePath = Join-Path $platformOutputPath $expectedExecutable
            
            if (Test-Path $executablePath) {
                $fileInfo = Get-Item $executablePath
                Write-Host "  Executable: $($fileInfo.Name) ($([math]::Round($fileInfo.Length / 1MB, 2)) MB)" -ForegroundColor Gray
                
                $BuildResults += [PSCustomObject]@{
                    Platform = $rid
                    Status = "Success"
                    ExecutablePath = $executablePath
                    Size = $fileInfo.Length
                }
            } else {
                Write-Host "  Warning: Expected executable not found at $executablePath" -ForegroundColor Yellow
                $BuildResults += [PSCustomObject]@{
                    Platform = $rid
                    Status = "Warning"
                    ExecutablePath = $null
                    Size = 0
                }
            }
        } else {
            Write-Host "✗ Failed to build for $rid (Exit code: $exitCode)" -ForegroundColor Red
            $BuildResults += [PSCustomObject]@{
                Platform = $rid
                Status = "Failed"
                ExecutablePath = $null
                Size = 0
            }
        }
    }
    catch {
        Write-Host "✗ Exception during build for $rid`: $($_.Exception.Message)" -ForegroundColor Red
        $BuildResults += [PSCustomObject]@{
            Platform = $rid
            Status = "Exception"
            ExecutablePath = $null
            Size = 0
        }
    }
    
    Write-Host ""
}

# Summary
Write-Host "Build Summary:" -ForegroundColor Green
Write-Host "=============" -ForegroundColor Green

$successCount = ($BuildResults | Where-Object { $_.Status -eq "Success" }).Count
$failureCount = ($BuildResults | Where-Object { $_.Status -ne "Success" }).Count

foreach ($result in $BuildResults) {
    $statusColor = switch ($result.Status) {
        "Success" { "Green" }
        "Warning" { "Yellow" }
        default { "Red" }
    }
    
    $sizeText = if ($result.Size -gt 0) { " ($([math]::Round($result.Size / 1MB, 2)) MB)" } else { "" }
    Write-Host "$($result.Platform): $($result.Status)$sizeText" -ForegroundColor $statusColor
}

Write-Host ""
Write-Host "Total: $successCount successful, $failureCount failed" -ForegroundColor $(if ($failureCount -eq 0) { "Green" } else { "Yellow" })

if ($failureCount -gt 0) {
    Write-Host "Some builds failed. Check the output above for details." -ForegroundColor Red
    exit 1
} else {
    Write-Host "All builds completed successfully!" -ForegroundColor Green
    Write-Host "Build artifacts are available in: $OutputPath" -ForegroundColor Yellow
    exit 0
}