#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Build SoluiNet.DevTools.Console for Windows platforms (x64 and ARM64)
.DESCRIPTION
    This script builds the console application specifically for Windows x64 and ARM64 architectures
.PARAMETER Configuration
    Build configuration (Debug or Release). Default is Release.
.PARAMETER OutputPath
    Output directory for build artifacts. Default is ./build/windows
.PARAMETER Architecture
    Target architecture (x64, arm64, or both). Default is both.
#>

param(
    [Parameter(Mandatory=$false)]
    [ValidateSet("Debug", "Release")]
    [string]$Configuration = "Release",
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = "./build/windows",
    
    [Parameter(Mandatory=$false)]
    [ValidateSet("x64", "arm64", "both")]
    [string]$Architecture = "both"
)

$ProjectPath = "SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj"

# Determine runtime identifiers based on architecture parameter
$RuntimeIdentifiers = switch ($Architecture) {
    "x64" { @("win-x64") }
    "arm64" { @("win-arm64") }
    "both" { @("win-x64", "win-arm64") }
}

Write-Host "Building SoluiNet.DevTools.Console for Windows" -ForegroundColor Green
Write-Host "Configuration: $Configuration" -ForegroundColor Yellow
Write-Host "Architecture(s): $($RuntimeIdentifiers -join ', ')" -ForegroundColor Yellow
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow

# Ensure output directory exists
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

# Clean and restore
dotnet clean $ProjectPath --configuration $Configuration --verbosity minimal
dotnet restore $ProjectPath --verbosity minimal

$allSuccessful = $true

foreach ($rid in $RuntimeIdentifiers) {
    Write-Host "Building for $rid..." -ForegroundColor Cyan
    
    $platformOutputPath = Join-Path $OutputPath $rid
    
    $buildArgs = @(
        "publish",
        $ProjectPath,
        "--configuration", $Configuration,
        "--runtime", $rid,
        "--output", $platformOutputPath,
        "--no-self-contained",
        "-p:PublishSingleFile=true",
        "-p:PublishReadyToRun=true",
        "--verbosity", "minimal"
    )
    
    try {
        & dotnet @buildArgs
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✓ Successfully built for $rid" -ForegroundColor Green
            
            # Copy plugins if they exist
            $pluginsSource = "build/plugins/$Configuration/Plugins"
            $pluginsTarget = Join-Path $platformOutputPath "Plugins"
            
            if (Test-Path $pluginsSource) {
                Write-Host "  Copying plugins..." -ForegroundColor Gray
                Copy-Item -Path $pluginsSource -Destination $pluginsTarget -Recurse -Force
            }
        } else {
            Write-Host "✗ Failed to build for $rid" -ForegroundColor Red
            $allSuccessful = $false
        }
    }
    catch {
        Write-Host "✗ Exception during build for $rid`: $($_.Exception.Message)" -ForegroundColor Red
        $allSuccessful = $false
    }
}

if ($allSuccessful) {
    Write-Host "Windows build completed successfully!" -ForegroundColor Green
    exit 0
} else {
    Write-Host "Some Windows builds failed." -ForegroundColor Red
    exit 1
}