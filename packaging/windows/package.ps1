#!/usr/bin/env pwsh
<#
.SYNOPSIS
    Create Windows-specific packages for SoluiNet.DevTools.Console
.DESCRIPTION
    This script creates Windows installer packages, portable packages, and Chocolatey packages
.PARAMETER BuildPath
    Path to the built binaries. Default is ./build/windows
.PARAMETER OutputPath
    Output directory for packages. Default is ./packages/windows
.PARAMETER Version
    Version string for the package. Default is 1.0.0
#>

param(
    [Parameter(Mandatory=$false)]
    [string]$BuildPath = "./build/windows",
    
    [Parameter(Mandatory=$false)]
    [string]$OutputPath = "./packages/windows",
    
    [Parameter(Mandatory=$false)]
    [string]$Version = "1.0.0"
)

Write-Host "Creating Windows packages for SoluiNet.DevTools.Console" -ForegroundColor Green
Write-Host "Build Path: $BuildPath" -ForegroundColor Yellow
Write-Host "Output Path: $OutputPath" -ForegroundColor Yellow
Write-Host "Version: $Version" -ForegroundColor Yellow

# Ensure output directory exists
if (!(Test-Path $OutputPath)) {
    New-Item -ItemType Directory -Path $OutputPath -Force | Out-Null
}

$PackageResults = @()

# Create portable ZIP packages for each architecture
foreach ($arch in @("win-x64", "win-arm64")) {
    $archBuildPath = Join-Path $BuildPath $arch
    
    if (Test-Path $archBuildPath) {
        Write-Host "Creating portable package for $arch..." -ForegroundColor Cyan
        
        $zipPath = Join-Path $OutputPath "SoluiNet.DevTools.Console_${arch}_v${Version}_Portable.zip"
        
        try {
            # Create ZIP archive
            Compress-Archive -Path "$archBuildPath\*" -DestinationPath $zipPath -Force
            
            if (Test-Path $zipPath) {
                $fileInfo = Get-Item $zipPath
                Write-Host "✓ Created portable package: $($fileInfo.Name) ($([math]::Round($fileInfo.Length / 1MB, 2)) MB)" -ForegroundColor Green
                
                $PackageResults += [PSCustomObject]@{
                    Architecture = $arch
                    PackageType = "Portable ZIP"
                    Status = "Success"
                    FilePath = $zipPath
                    Size = $fileInfo.Length
                }
            }
        }
        catch {
            Write-Host "✗ Failed to create portable package for $arch`: $($_.Exception.Message)" -ForegroundColor Red
            $PackageResults += [PSCustomObject]@{
                Architecture = $arch
                PackageType = "Portable ZIP"
                Status = "Failed"
                FilePath = $null
                Size = 0
            }
        }
    } else {
        Write-Host "! Build path not found for $arch`: $archBuildPath" -ForegroundColor Yellow
        $PackageResults += [PSCustomObject]@{
            Architecture = $arch
            PackageType = "Portable ZIP"
            Status = "Build Not Found"
            FilePath = $null
            Size = 0
        }
    }
}

# Create Chocolatey package specification
Write-Host "Creating Chocolatey package specification..." -ForegroundColor Cyan

$chocoPath = Join-Path $OutputPath "chocolatey"
if (!(Test-Path $chocoPath)) {
    New-Item -ItemType Directory -Path $chocoPath -Force | Out-Null
}

# Create nuspec file
$nuspecContent = @"
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd">
  <metadata>
    <id>soluinet-devtools-console</id>
    <version>$Version</version>
    <packageSourceUrl>https://github.com/SoluiNet/SoluiNet.DevTools</packageSourceUrl>
    <owners>SoluiNet</owners>
    <title>SoluiNet DevTools Console</title>
    <authors>SoluiNet</authors>
    <projectUrl>https://github.com/SoluiNet/SoluiNet.DevTools</projectUrl>
    <copyright>Copyright © SoluiNet 2018-2024</copyright>
    <licenseUrl>https://github.com/SoluiNet/SoluiNet.DevTools/blob/main/LICENSE</licenseUrl>
    <requireLicenseAcceptance>false</requireLicenseAcceptance>
    <tags>devtools development console cli cross-platform</tags>
    <summary>Cross-platform development tools console application</summary>
    <description>
SoluiNet DevTools Console is a cross-platform command-line application that provides various development tools and utilities. 
It supports plugin-based architecture and runs on Windows, Linux, and macOS.
    </description>
    <releaseNotes>Multi-platform support with .NET 8.0</releaseNotes>
  </metadata>
  <files>
    <file src="tools\**" target="tools" />
  </files>
</package>
"@

$nuspecPath = Join-Path $chocoPath "soluinet-devtools-console.nuspec"
$nuspecContent | Out-File -FilePath $nuspecPath -Encoding UTF8

# Create Chocolatey install script
$toolsPath = Join-Path $chocoPath "tools"
if (!(Test-Path $toolsPath)) {
    New-Item -ItemType Directory -Path $toolsPath -Force | Out-Null
}

$installScript = @"
`$ErrorActionPreference = 'Stop'

`$packageName = 'soluinet-devtools-console'
`$toolsDir = Split-Path -Parent `$MyInvocation.MyCommand.Definition
`$packageArgs = @{
    packageName    = `$packageName
    unzipLocation  = `$toolsDir
    url64bit       = 'https://github.com/SoluiNet/SoluiNet.DevTools/releases/download/v$Version/SoluiNet.DevTools.Console_win-x64_v${Version}_Portable.zip'
    checksum64     = 'CHECKSUM_PLACEHOLDER'
    checksumType64 = 'sha256'
}

Install-ChocolateyZipPackage @packageArgs

# Add to PATH
`$binPath = Join-Path `$toolsDir 'win-x64'
Install-ChocolateyPath `$binPath 'Machine'
"@

$installScriptPath = Join-Path $toolsPath "chocolateyinstall.ps1"
$installScript | Out-File -FilePath $installScriptPath -Encoding UTF8

Write-Host "✓ Created Chocolatey package specification" -ForegroundColor Green

$PackageResults += [PSCustomObject]@{
    Architecture = "All"
    PackageType = "Chocolatey Spec"
    Status = "Success"
    FilePath = $chocoPath
    Size = 0
}

# Create Windows Installer (MSI) specification using WiX (if available)
if (Get-Command "candle.exe" -ErrorAction SilentlyContinue) {
    Write-Host "Creating Windows Installer (MSI) specification..." -ForegroundColor Cyan
    
    $wixPath = Join-Path $OutputPath "wix"
    if (!(Test-Path $wixPath)) {
        New-Item -ItemType Directory -Path $wixPath -Force | Out-Null
    }
    
    # Create WiX source file
    $wixSource = @"
<?xml version="1.0" encoding="UTF-8"?>
<Wix xmlns="http://schemas.microsoft.com/wix/2006/wi">
  <Product Id="*" Name="SoluiNet DevTools Console" Language="1033" Version="$Version" 
           Manufacturer="SoluiNet" UpgradeCode="12345678-1234-1234-1234-123456789012">
    
    <Package InstallerVersion="200" Compressed="yes" InstallScope="perMachine" />
    
    <MajorUpgrade DowngradeErrorMessage="A newer version of [ProductName] is already installed." />
    
    <MediaTemplate EmbedCab="yes" />
    
    <Feature Id="ProductFeature" Title="SoluiNet DevTools Console" Level="1">
      <ComponentGroupRef Id="ProductComponents" />
    </Feature>
    
    <Directory Id="TARGETDIR" Name="SourceDir">
      <Directory Id="ProgramFiles64Folder">
        <Directory Id="INSTALLFOLDER" Name="SoluiNet DevTools Console" />
      </Directory>
    </Directory>
    
    <ComponentGroup Id="ProductComponents" Directory="INSTALLFOLDER">
      <Component Id="MainExecutable" Guid="*">
        <File Id="MainExe" Source="`$(var.SourceDir)\sndt.exe" KeyPath="yes" />
        <Environment Id="PATH" Name="PATH" Value="[INSTALLFOLDER]" Permanent="no" Part="last" Action="set" System="yes" />
      </Component>
    </ComponentGroup>
  </Product>
</Wix>
"@
    
    $wixSourcePath = Join-Path $wixPath "SoluiNet.DevTools.Console.wxs"
    $wixSource | Out-File -FilePath $wixSourcePath -Encoding UTF8
    
    Write-Host "✓ Created WiX installer specification" -ForegroundColor Green
    
    $PackageResults += [PSCustomObject]@{
        Architecture = "All"
        PackageType = "WiX MSI Spec"
        Status = "Success"
        FilePath = $wixPath
        Size = 0
    }
} else {
    Write-Host "! WiX Toolset not found, skipping MSI creation" -ForegroundColor Yellow
    
    $PackageResults += [PSCustomObject]@{
        Architecture = "All"
        PackageType = "WiX MSI Spec"
        Status = "WiX Not Available"
        FilePath = $null
        Size = 0
    }
}

# Summary
Write-Host ""
Write-Host "Windows Packaging Summary:" -ForegroundColor Green
Write-Host "=========================" -ForegroundColor Green

$successCount = ($PackageResults | Where-Object { $_.Status -eq "Success" }).Count
$totalCount = $PackageResults.Count

foreach ($result in $PackageResults) {
    $statusColor = switch ($result.Status) {
        "Success" { "Green" }
        "WiX Not Available" { "Yellow" }
        "Build Not Found" { "Yellow" }
        default { "Red" }
    }
    
    $sizeText = if ($result.Size -gt 0) { " ($([math]::Round($result.Size / 1MB, 2)) MB)" } else { "" }
    Write-Host "$($result.Architecture) $($result.PackageType): $($result.Status)$sizeText" -ForegroundColor $statusColor
}

Write-Host ""
Write-Host "Total: $successCount successful out of $totalCount package types" -ForegroundColor $(if ($successCount -eq $totalCount) { "Green" } else { "Yellow" })
Write-Host "Packages created in: $OutputPath" -ForegroundColor Yellow

if ($successCount -gt 0) {
    exit 0
} else {
    exit 1
}