# Multi-Platform Build System

This document describes the multi-platform build system for SoluiNet.DevTools.Console, which supports building and packaging for Windows, Linux, and macOS on both x64 and ARM64 architectures using the .NET 8.0 CLI.

## Overview

The build system consists of:
- **dotnet CLI Commands**: Modern .NET CLI-based build process
- **Cross-Platform Scripts**: Platform-agnostic build automation
- **CI/CD Pipeline**: GitHub Actions workflow for automated builds and testing
- **Packaging Scripts**: Platform-specific packaging for distribution
- **Testing Framework**: Automated testing across platforms and architectures

## dotnet CLI Build Commands

All builds use the modern dotnet CLI instead of MSBuild for cross-platform compatibility.

## Prerequisites

### All Platforms
- .NET 8.0 SDK or later
- Git

### Windows
- PowerShell 5.1 or PowerShell Core 7+
- Optional: WiX Toolset (for MSI installers)
- Optional: Chocolatey (for package testing)

### Linux
- Bash shell
- Optional: `dpkg-deb` (for DEB packages)
- Optional: `rpmbuild` (for RPM packages)
- Optional: `snapcraft` (for Snap packages)

### macOS
- Bash shell
- Xcode Command Line Tools
- Optional: Developer ID certificate (for code signing)

## Build Scripts

### Basic dotnet CLI Commands

#### Build for Current Platform
```bash
# Build for current platform and architecture
dotnet build SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj --configuration Release

# Restore dependencies first (if needed)
dotnet restore SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj
```

#### Build for Specific Runtime
```bash
# Build for specific runtime identifier
dotnet build SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime win-x64

# Publish self-contained for specific runtime
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output ./build/linux-x64
```

#### Build for All Supported Runtimes
```bash
# Build for all supported platforms (using scripts)
# PowerShell (Windows/Cross-platform)
./scripts/build-all-platforms.ps1 -Configuration Release

# Bash (Linux/macOS)
./scripts/build-all-platforms.sh --configuration Release
```

**Common dotnet CLI Parameters:**
- `--configuration`: Build configuration (Debug/Release)
- `--runtime`: Target runtime identifier (win-x64, linux-x64, osx-x64, etc.)
- `--output`: Output directory
- `--self-contained`: Create self-contained deployment (includes .NET runtime)
- `--no-self-contained`: Framework-dependent deployment (requires .NET runtime installed)
- `--verbosity`: Logging verbosity (quiet, minimal, normal, detailed, diagnostic)

### Platform-Specific dotnet CLI Commands

#### Windows Builds
```bash
# Windows x64
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime win-x64 \
  --output ./build/windows/win-x64

# Windows ARM64
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime win-arm64 \
  --output ./build/windows/win-arm64

# Using build script
./scripts/build-windows.ps1 -Configuration Release -Architecture both
```

#### Linux Builds
```bash
# Linux x64
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --output ./build/linux/linux-x64

# Linux ARM64
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime linux-arm64 \
  --output ./build/linux/linux-arm64

# Using build script
./scripts/build-linux.sh --configuration Release --architecture both
```

#### macOS Builds
```bash
# macOS x64 (Intel)
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime osx-x64 \
  --output ./build/macos/osx-x64

# macOS ARM64 (Apple Silicon)
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime osx-arm64 \
  --output ./build/macos/osx-arm64

# Using build script
./scripts/build-macos.sh --configuration Release --architecture both
```

## Testing with dotnet CLI

### Running Tests

```bash
# Run all tests
dotnet test SoluiNet.DevTools.UnitTest/SoluiNet.DevTools.UnitTest.csproj

# Run tests with specific configuration
dotnet test SoluiNet.DevTools.UnitTest/SoluiNet.DevTools.UnitTest.csproj \
  --configuration Release \
  --logger "trx;LogFileName=test-results.trx" \
  --results-directory ./test-results

# Run tests for specific runtime
dotnet test SoluiNet.DevTools.UnitTest/SoluiNet.DevTools.UnitTest.csproj \
  --runtime linux-x64 \
  --configuration Release

# Run tests with coverage
dotnet test SoluiNet.DevTools.UnitTest/SoluiNet.DevTools.UnitTest.csproj \
  --collect:"XPlat Code Coverage" \
  --results-directory ./test-results
```

### Automated Cross-Platform Testing

```bash
# PowerShell script for comprehensive testing
./scripts/test-all-platforms.ps1 -Configuration Release

# Bash script for comprehensive testing  
./scripts/test-all-platforms.sh --configuration Release
```

**dotnet test Parameters:**
- `--configuration`: Build configuration to test
- `--runtime`: Target runtime for tests
- `--logger`: Test result logger (trx, junit, console)
- `--results-directory`: Directory for test results
- `--collect`: Data collector (code coverage, etc.)
- `--verbosity`: Logging verbosity

### Manual Testing

Test the console application manually:

```bash
# Windows
./build/windows/win-x64/sndt.exe --version
./build/windows/win-x64/sndt.exe --help

# Linux
./build/linux/linux-x64/sndt --version
./build/linux/linux-x64/sndt --help

# macOS
./build/macos/osx-x64/sndt --version
./build/macos/osx-x64/sndt --help
```

## Packaging

### Windows Packaging

Create Windows-specific packages:

```powershell
./packaging/windows/package.ps1 -BuildPath ./build/windows -Version 1.0.0
```

**Creates:**
- Portable ZIP packages (x64 and ARM64)
- Chocolatey package specification
- WiX MSI installer specification (if WiX is available)

### Linux Packaging

Create Linux distribution packages:

```bash
./packaging/linux/package.sh --build-path ./build/linux --version 1.0.0
```

**Creates:**
- TAR.GZ archives (x64 and ARM64)
- DEB package (if `dpkg-deb` available)
- RPM package (if `rpmbuild` available)
- Snap package specification

### macOS Packaging

Create macOS-specific packages:

```bash
./packaging/macos/package.sh --build-path ./build/macos --version 1.0.0
```

**Creates:**
- TAR.GZ archives (x64 and ARM64)
- DMG disk image (if `hdiutil` available)
- PKG installer (if `pkgbuild` available)
- Homebrew formula
- MacPorts Portfile

**Optional Code Signing:**
```bash
./packaging/macos/package.sh --developer-id "Developer ID Application: Your Name"
```

## CI/CD Pipeline

### GitHub Actions Workflow

The CI/CD pipeline (`.github/workflows/build-project.yml`) automatically:

1. **Multi-Platform Console Build**
   - Builds on Windows, Linux, and macOS runners
   - Tests each platform and architecture
   - Creates platform-specific artifacts

2. **Legacy Windows Build**
   - Builds UI and Web components using MSBuild
   - Maintains compatibility with existing Windows-specific components

3. **Release Package Creation**
   - Combines artifacts from all platforms
   - Creates distribution-ready packages
   - Uploads release artifacts

### Workflow Triggers

- **Push to main/develop**: Full build and test
- **Pull requests to main**: Build and test validation
- **Manual dispatch**: On-demand builds

### Artifacts

The pipeline creates the following artifacts:

- `SoluiNet.DevTools.Console_windows_v{version}`: Windows binaries
- `SoluiNet.DevTools.Console_linux_v{version}`: Linux binaries  
- `SoluiNet.DevTools.Console_macos_v{version}`: macOS binaries
- `SoluiNet.DevTools.UI_v{version}`: Windows UI application
- `SoluiNet.DevTools.Web_v{version}`: Web server application
- `SoluiNet.DevTools.ReleasePackages_v{version}`: Distribution packages
- `test-results-{platform}`: Test results for each platform

## Architecture Support

### Supported Runtime Identifiers (RIDs)

| Platform | x64 | ARM64 |
|----------|-----|-------|
| Windows  | `win-x64` | `win-arm64` |
| Linux    | `linux-x64` | `linux-arm64` |
| macOS    | `osx-x64` | `osx-arm64` |

### Build Optimizations

- **ReadyToRun**: Enabled for faster startup performance
- **Single File**: Reduces deployment complexity
- **Framework-Dependent**: Smaller package size, requires .NET runtime
- **Architecture-Specific**: Optimized for target CPU architecture

## Configuration

### Project Configuration

The console project (`SoluiNet.DevTools.Console.csproj`) includes:

```xml
<PropertyGroup>
  <TargetFramework>net8.0</TargetFramework>
  <RuntimeIdentifiers>win-x64;win-arm64;linux-x64;linux-arm64;osx-x64;osx-arm64</RuntimeIdentifiers>
  <UseAppHost>true</UseAppHost>
  <PublishSingleFile>true</PublishSingleFile>
  <SelfContained>false</SelfContained>
</PropertyGroup>

<!-- Conditional compilation symbols -->
<PropertyGroup Condition="'$(RuntimeIdentifier)' == 'win-x64' OR '$(RuntimeIdentifier)' == 'win-arm64'">
  <DefineConstants>$(DefineConstants);WINDOWS</DefineConstants>
</PropertyGroup>
<!-- Similar for LINUX and MACOS -->
```

### Environment Variables

The build system recognizes these environment variables:

- `DOTNET_VERSION`: .NET SDK version (default: 8.0.x)
- `BUILD_VERSION`: Version string for packages
- `DEVELOPER_ID`: macOS code signing certificate

## Troubleshooting

### Common Issues

#### Build Failures

1. **Missing .NET SDK**
   ```bash
   dotnet --version  # Verify .NET 8.0+ is installed
   ```

2. **Runtime Identifier Issues**
   ```bash
   dotnet --info  # Check supported RIDs
   ```

3. **Plugin Loading Errors**
   - Ensure plugins are copied to output directory
   - Check plugin compatibility with target architecture

#### Platform-Specific Issues

**Windows:**
- PowerShell execution policy: `Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser`
- Long path support: Enable in Windows settings or Group Policy

**Linux:**
- Missing dependencies: Install build-essential, libc6-dev
- Permission issues: Ensure scripts are executable (`chmod +x`)

**macOS:**
- Xcode Command Line Tools: `xcode-select --install`
- Gatekeeper issues: `xattr -d com.apple.quarantine <file>`

### Debug Mode

Enable verbose logging:

```bash
# Add --verbosity detailed to any dotnet command
dotnet build --verbosity detailed

# Or set environment variable
export DOTNET_CLI_TELEMETRY_OPTOUT=1
```

### Performance Optimization

For faster builds:

1. **Use Build Cache**
   ```bash
   # Enable NuGet package caching
   export NUGET_PACKAGES=~/.nuget/packages
   ```

2. **Parallel Builds**
   ```bash
   # Use multiple CPU cores
   dotnet build -m
   ```

3. **Incremental Builds**
   ```bash
   # Skip clean for incremental builds
   ./scripts/build-all-platforms.sh --skip-clean
   ```

## Distribution

### Release Process

1. **Version Update**: Update version in project files
2. **Build All Platforms**: Run universal build script
3. **Run Tests**: Execute test suite on all platforms
4. **Create Packages**: Generate distribution packages
5. **Upload Artifacts**: Deploy to release channels

### Package Managers

#### Windows
- **Chocolatey**: Use generated `.nuspec` file
- **Scoop**: Create bucket with manifest
- **WinGet**: Submit to Microsoft Community Repository

#### Linux
- **APT**: Upload DEB to repository
- **YUM/DNF**: Upload RPM to repository
- **Snap Store**: Publish using generated `snapcraft.yaml`
- **Flatpak**: Create Flatpak manifest

#### macOS
- **Homebrew**: Submit formula to homebrew-core
- **MacPorts**: Submit Portfile to MacPorts
- **App Store**: Package as Mac App Store application

## Security Considerations

### Code Signing

- **Windows**: Use Authenticode certificate for EXE/MSI
- **macOS**: Use Developer ID for DMG/PKG, notarize for Gatekeeper
- **Linux**: Use GPG signing for repository packages

### Supply Chain Security

- Pin dependency versions in project files
- Use package lock files where available
- Verify checksums in package managers
- Scan for vulnerabilities in CI/CD pipeline

## Future Enhancements

### Planned Features

1. **Container Support**: Docker images for each platform
2. **Native AOT**: Ahead-of-time compilation for smaller binaries
3. **Auto-Update**: Built-in update mechanism
4. **Telemetry**: Usage analytics and crash reporting
5. **Plugin Store**: Centralized plugin distribution

### Performance Improvements

1. **Build Caching**: Distributed build cache
2. **Incremental Publishing**: Only rebuild changed components
3. **Parallel Testing**: Concurrent test execution
4. **ARM64 Optimization**: Native ARM64 performance tuning