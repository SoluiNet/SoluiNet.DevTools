# Deployment Guide

This guide covers deployment strategies and distribution methods for SoluiNet.DevTools.Console across different platforms and environments.

## Deployment Overview

SoluiNet.DevTools.Console supports multiple deployment models:

- **Framework-Dependent Deployment (FDD)**: Requires .NET 8.0 runtime on target system
- **Self-Contained Deployment (SCD)**: Includes .NET runtime, larger but no dependencies
- **Single-File Deployment**: Bundles application into single executable
- **Native AOT**: Ahead-of-time compiled native binaries (future enhancement)

## dotnet CLI Deployment Commands

### Framework-Dependent Deployment

Smaller deployment size, requires .NET runtime on target system:

```bash
# Publish framework-dependent
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime win-x64 \
  --no-self-contained \
  --output ./deploy/fdd/win-x64

# Cross-platform framework-dependent (portable)
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --no-self-contained \
  --output ./deploy/fdd/portable
```

### Self-Contained Deployment

Includes .NET runtime, larger but no dependencies:

```bash
# Self-contained for Windows x64
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  --output ./deploy/scd/win-x64

# Self-contained for Linux x64
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output ./deploy/scd/linux-x64

# Self-contained for macOS ARM64
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained true \
  --output ./deploy/scd/osx-arm64
```

### Single-File Deployment

Bundles everything into a single executable:

```bash
# Single-file self-contained
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  --publish-single-file true \
  --output ./deploy/single-file/win-x64

# Single-file framework-dependent
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --no-self-contained \
  --publish-single-file true \
  --output ./deploy/single-file/linux-x64
```

### Optimized Production Deployment

```bash
# Optimized self-contained single-file
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime win-x64 \
  --self-contained true \
  --publish-single-file true \
  --publish-trimmed true \
  --publish-ready-to-run true \
  --output ./deploy/optimized/win-x64
```

**Optimization Parameters:**
- `--publish-trimmed`: Remove unused code (reduces size)
- `--publish-ready-to-run`: Pre-compile for faster startup
- `--publish-single-file`: Bundle into single executable
- `--no-restore`: Skip package restore (if already done)

## Platform-Specific Deployment

### Windows Deployment

#### Using PowerShell Scripts
```powershell
# Deploy all Windows architectures
./packaging/windows/package.ps1 -BuildPath ./build/windows -Version 1.0.0

# Deploy specific architecture
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj `
  --configuration Release `
  --runtime win-x64 `
  --self-contained true `
  --publish-single-file true `
  --output "C:\Deploy\SoluiNet.DevTools.Console"
```

#### Windows-Specific Features
- **Windows Service**: Can be deployed as Windows Service using `sc create`
- **MSI Installer**: Use WiX Toolset for professional installation
- **ClickOnce**: For automatic updates (if using WPF components)
- **App-V**: For application virtualization

#### Registry Integration (Optional)
```powershell
# Register application in Windows Registry
New-Item -Path "HKLM:\SOFTWARE\SoluiNet\DevTools" -Force
Set-ItemProperty -Path "HKLM:\SOFTWARE\SoluiNet\DevTools" -Name "InstallPath" -Value "C:\Program Files\SoluiNet\DevTools"
Set-ItemProperty -Path "HKLM:\SOFTWARE\SoluiNet\DevTools" -Name "Version" -Value "1.0.0"
```

### Linux Deployment

#### Using Bash Scripts
```bash
# Deploy all Linux architectures
./packaging/linux/package.sh --build-path ./build/linux --version 1.0.0

# Deploy to system directories
sudo dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained true \
  --output /opt/soluinet-devtools

# Create symbolic link
sudo ln -sf /opt/soluinet-devtools/sndt /usr/local/bin/sndt
```

#### Linux-Specific Features
- **Systemd Service**: Deploy as system service
- **AppImage**: Portable application format
- **Flatpak**: Sandboxed application distribution
- **Snap**: Universal Linux packages

#### Systemd Service Configuration
```ini
# /etc/systemd/system/soluinet-devtools.service
[Unit]
Description=SoluiNet DevTools Console Service
After=network.target

[Service]
Type=simple
User=soluinet
WorkingDirectory=/opt/soluinet-devtools
ExecStart=/opt/soluinet-devtools/sndt --service
Restart=always
RestartSec=10

[Install]
WantedBy=multi-user.target
```

#### File Permissions
```bash
# Set appropriate permissions
sudo chown -R root:root /opt/soluinet-devtools
sudo chmod 755 /opt/soluinet-devtools/sndt
sudo chmod 644 /opt/soluinet-devtools/*.json
sudo chmod 755 /opt/soluinet-devtools/plugins
```

### macOS Deployment

#### Using Bash Scripts
```bash
# Deploy all macOS architectures
./packaging/macos/package.sh --build-path ./build/macos --version 1.0.0

# Deploy to Applications folder
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime osx-arm64 \
  --self-contained true \
  --output "/Applications/SoluiNet.DevTools.Console"
```

#### macOS-Specific Features
- **App Bundle**: Package as .app bundle
- **DMG Distribution**: Disk image for easy installation
- **Homebrew Formula**: Package manager integration
- **Code Signing**: Required for Gatekeeper compatibility

#### Code Signing (Required for Distribution)
```bash
# Sign the executable
codesign --sign "Developer ID Application: Your Name" \
  --options runtime \
  --entitlements entitlements.plist \
  /Applications/SoluiNet.DevTools.Console/sndt

# Create DMG and sign
hdiutil create -volname "SoluiNet DevTools" \
  -srcfolder "/Applications/SoluiNet.DevTools.Console" \
  -ov -format UDZO SoluiNet.DevTools.Console.dmg

codesign --sign "Developer ID Application: Your Name" \
  SoluiNet.DevTools.Console.dmg

# Notarize for Gatekeeper
xcrun notarytool submit SoluiNet.DevTools.Console.dmg \
  --keychain-profile "notarytool-profile" \
  --wait
```

## Container Deployment

### Docker Deployment

#### Multi-Stage Dockerfile
```dockerfile
# Build stage
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY ["SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj", "SoluiNet.DevTools.Console/"]
COPY ["SoluiNet.DevTools.Core/SoluiNet.DevTools.Core.csproj", "SoluiNet.DevTools.Core/"]
RUN dotnet restore "SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj"

COPY . .
WORKDIR "/src/SoluiNet.DevTools.Console"
RUN dotnet publish "SoluiNet.DevTools.Console.csproj" \
    --configuration Release \
    --no-restore \
    --output /app/publish

# Runtime stage
FROM mcr.microsoft.com/dotnet/runtime:8.0
WORKDIR /app
COPY --from=build /app/publish .

# Create non-root user
RUN groupadd -r soluinet && useradd -r -g soluinet soluinet
RUN chown -R soluinet:soluinet /app
USER soluinet

ENTRYPOINT ["./sndt"]
```

#### Build and Run Docker Container
```bash
# Build Docker image
docker build -t soluinet/devtools-console:latest .

# Run container
docker run -it --rm \
  -v $(pwd)/config:/app/config \
  -v $(pwd)/plugins:/app/plugins \
  soluinet/devtools-console:latest --version

# Run with persistent storage
docker run -d --name soluinet-devtools \
  -v soluinet-config:/app/config \
  -v soluinet-plugins:/app/plugins \
  -v soluinet-logs:/app/logs \
  soluinet/devtools-console:latest --service
```

### Kubernetes Deployment

#### Deployment Manifest
```yaml
apiVersion: apps/v1
kind: Deployment
metadata:
  name: soluinet-devtools-console
  labels:
    app: soluinet-devtools-console
spec:
  replicas: 1
  selector:
    matchLabels:
      app: soluinet-devtools-console
  template:
    metadata:
      labels:
        app: soluinet-devtools-console
    spec:
      containers:
      - name: console
        image: soluinet/devtools-console:latest
        ports:
        - containerPort: 8080
        env:
        - name: SOLUINET_DEVTOOLS_CONFIG_DIR
          value: "/app/config"
        - name: SOLUINET_DEVTOOLS_LOG_LEVEL
          value: "Information"
        volumeMounts:
        - name: config-volume
          mountPath: /app/config
        - name: plugins-volume
          mountPath: /app/plugins
        resources:
          requests:
            memory: "256Mi"
            cpu: "250m"
          limits:
            memory: "512Mi"
            cpu: "500m"
      volumes:
      - name: config-volume
        configMap:
          name: soluinet-devtools-config
      - name: plugins-volume
        persistentVolumeClaim:
          claimName: soluinet-devtools-plugins
```

## Cloud Deployment

### Azure Deployment

#### Azure Container Instances
```bash
# Deploy to Azure Container Instances
az container create \
  --resource-group myResourceGroup \
  --name soluinet-devtools \
  --image soluinet/devtools-console:latest \
  --cpu 1 \
  --memory 1 \
  --environment-variables \
    SOLUINET_DEVTOOLS_LOG_LEVEL=Information \
  --ports 8080
```

#### Azure App Service (Linux)
```bash
# Deploy to Azure App Service
az webapp create \
  --resource-group myResourceGroup \
  --plan myAppServicePlan \
  --name soluinet-devtools \
  --deployment-container-image-name soluinet/devtools-console:latest
```

### AWS Deployment

#### AWS ECS Fargate
```json
{
  "family": "soluinet-devtools-console",
  "networkMode": "awsvpc",
  "requiresCompatibilities": ["FARGATE"],
  "cpu": "256",
  "memory": "512",
  "executionRoleArn": "arn:aws:iam::account:role/ecsTaskExecutionRole",
  "containerDefinitions": [
    {
      "name": "console",
      "image": "soluinet/devtools-console:latest",
      "portMappings": [
        {
          "containerPort": 8080,
          "protocol": "tcp"
        }
      ],
      "environment": [
        {
          "name": "SOLUINET_DEVTOOLS_LOG_LEVEL",
          "value": "Information"
        }
      ],
      "logConfiguration": {
        "logDriver": "awslogs",
        "options": {
          "awslogs-group": "/ecs/soluinet-devtools",
          "awslogs-region": "us-east-1",
          "awslogs-stream-prefix": "ecs"
        }
      }
    }
  ]
}
```

#### AWS Lambda (Serverless)
```bash
# Package for Lambda deployment
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --self-contained false \
  --output ./lambda-package

# Create deployment package
cd lambda-package
zip -r ../soluinet-devtools-lambda.zip .
```

### Google Cloud Deployment

#### Google Cloud Run
```bash
# Deploy to Cloud Run
gcloud run deploy soluinet-devtools \
  --image soluinet/devtools-console:latest \
  --platform managed \
  --region us-central1 \
  --allow-unauthenticated \
  --memory 512Mi \
  --cpu 1 \
  --set-env-vars SOLUINET_DEVTOOLS_LOG_LEVEL=Information
```

## Package Distribution

### Package Managers

#### Windows Package Managers

**Chocolatey Package**
```xml
<!-- soluinet-devtools-console.nuspec -->
<?xml version="1.0" encoding="utf-8"?>
<package xmlns="http://schemas.microsoft.com/packaging/2015/06/nuspec.xsd">
  <metadata>
    <id>soluinet-devtools-console</id>
    <version>1.0.0</version>
    <title>SoluiNet DevTools Console</title>
    <authors>SoluiNet</authors>
    <description>Cross-platform developer tools console application</description>
    <tags>developer-tools console cross-platform dotnet</tags>
  </metadata>
  <files>
    <file src="tools\**" target="tools" />
  </files>
</package>
```

**WinGet Manifest**
```yaml
# manifests/s/SoluiNet/DevTools/Console/1.0.0/SoluiNet.DevTools.Console.yaml
PackageIdentifier: SoluiNet.DevTools.Console
PackageVersion: 1.0.0
PackageName: SoluiNet DevTools Console
Publisher: SoluiNet
License: MIT
ShortDescription: Cross-platform developer tools console
Installers:
- Architecture: x64
  InstallerType: zip
  InstallerUrl: https://github.com/SoluiNet/SoluiNet.DevTools/releases/download/v1.0.0/SoluiNet.DevTools.Console_windows_x64.zip
  InstallerSha256: [SHA256_HASH]
- Architecture: arm64
  InstallerType: zip
  InstallerUrl: https://github.com/SoluiNet/SoluiNet.DevTools/releases/download/v1.0.0/SoluiNet.DevTools.Console_windows_arm64.zip
  InstallerSha256: [SHA256_HASH]
```

#### Linux Package Managers

**DEB Package Control File**
```
Package: soluinet-devtools-console
Version: 1.0.0
Section: devel
Priority: optional
Architecture: amd64
Depends: libc6, libgcc1, libssl3
Maintainer: SoluiNet <support@soluinet.com>
Description: Cross-platform developer tools console
 SoluiNet DevTools Console is a cross-platform developer tools application
 built on .NET 8.0 that provides various utilities for software development.
```

**RPM Spec File**
```spec
Name:           soluinet-devtools-console
Version:        1.0.0
Release:        1%{?dist}
Summary:        Cross-platform developer tools console
License:        MIT
URL:            https://github.com/SoluiNet/SoluiNet.DevTools
Source0:        %{name}-%{version}.tar.gz

BuildRequires:  dotnet-sdk-8.0
Requires:       dotnet-runtime-8.0

%description
SoluiNet DevTools Console is a cross-platform developer tools application
built on .NET 8.0 that provides various utilities for software development.

%prep
%setup -q

%build
dotnet publish SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj \
  --configuration Release \
  --runtime linux-x64 \
  --no-self-contained \
  --output %{buildroot}/opt/soluinet-devtools

%install
mkdir -p %{buildroot}/usr/local/bin
ln -s /opt/soluinet-devtools/sndt %{buildroot}/usr/local/bin/sndt

%files
/opt/soluinet-devtools/
/usr/local/bin/sndt

%changelog
* Mon Oct 27 2025 SoluiNet <support@soluinet.com> - 1.0.0-1
- Initial package
```

#### macOS Package Managers

**Homebrew Formula**
```ruby
# Formula/soluinet-devtools-console.rb
class SoluinetDevtoolsConsole < Formula
  desc "Cross-platform developer tools console"
  homepage "https://github.com/SoluiNet/SoluiNet.DevTools"
  url "https://github.com/SoluiNet/SoluiNet.DevTools/archive/v1.0.0.tar.gz"
  sha256 "[SHA256_HASH]"
  license "MIT"

  depends_on "dotnet"

  def install
    system "dotnet", "publish", "SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj",
           "--configuration", "Release",
           "--runtime", "osx-#{Hardware::CPU.arch}",
           "--no-self-contained",
           "--output", libexec

    (bin/"sndt").write_env_script libexec/"sndt", DOTNET_ROOT: Formula["dotnet"].opt_libexec
  end

  test do
    assert_match "SoluiNet.DevTools.Console", shell_output("#{bin}/sndt --version")
  end
end
```

## Deployment Automation

### GitHub Actions Deployment

```yaml
name: Deploy Multi-Platform

on:
  release:
    types: [published]

jobs:
  deploy-packages:
    runs-on: ubuntu-latest
    steps:
    - uses: actions/checkout@v4
    
    - name: Setup .NET
      uses: actions/setup-dotnet@v3
      with:
        dotnet-version: '8.0.x'
    
    - name: Build and Package All Platforms
      run: |
        ./scripts/build-all-platforms.sh --configuration Release
        ./packaging/create-all-packages.sh --version ${{ github.event.release.tag_name }}
    
    - name: Upload Release Assets
      uses: actions/upload-release-asset@v1
      with:
        upload_url: ${{ github.event.release.upload_url }}
        asset_path: ./packages/
        asset_name: release-packages
        asset_content_type: application/zip
```

### Automated Package Publishing

```bash
# Publish to NuGet (for libraries)
dotnet nuget push SoluiNet.DevTools.Core.*.nupkg \
  --api-key $NUGET_API_KEY \
  --source https://api.nuget.org/v3/index.json

# Publish to Chocolatey
choco push soluinet-devtools-console.*.nupkg \
  --source https://push.chocolatey.org/ \
  --api-key $CHOCOLATEY_API_KEY

# Publish Docker image
docker push soluinet/devtools-console:latest
docker push soluinet/devtools-console:$VERSION
```

## Deployment Verification

### Post-Deployment Testing

```bash
# Verify deployment
sndt --version
sndt --help
sndt diagnostics --system

# Test plugin loading
sndt plugin list
sndt plugin validate --all

# Test configuration
sndt config show
sndt config validate

# Performance test
sndt diagnostics --performance
```

### Health Checks

```bash
# Basic health check
sndt health

# Detailed health check with metrics
sndt health --detailed --metrics

# Continuous monitoring
sndt health --monitor --interval 60
```

## Rollback Procedures

### Version Rollback

```bash
# Backup current version
cp -r /opt/soluinet-devtools /opt/soluinet-devtools.backup

# Rollback to previous version
./scripts/rollback.sh --to-version 0.9.0

# Verify rollback
sndt --version
sndt diagnostics --config
```

### Configuration Rollback

```bash
# Restore configuration from backup
sndt config import --file config-backup.json

# Reset to factory defaults if needed
sndt config reset --all
```

## Security Considerations

### Deployment Security

1. **Code Signing**: Sign all executables and packages
2. **Checksum Verification**: Provide SHA256 checksums for all packages
3. **Secure Distribution**: Use HTTPS for all downloads
4. **Access Control**: Restrict deployment pipeline access
5. **Vulnerability Scanning**: Scan containers and packages for vulnerabilities

### Runtime Security

1. **Principle of Least Privilege**: Run with minimal required permissions
2. **Sandboxing**: Use containers or sandboxed environments where possible
3. **Network Security**: Configure firewalls and network policies
4. **Monitoring**: Implement security monitoring and alerting
5. **Updates**: Establish regular update procedures

## Monitoring and Maintenance

### Deployment Monitoring

```bash
# Monitor deployment status
kubectl get deployments
docker ps
systemctl status soluinet-devtools

# Check resource usage
sndt diagnostics --resources
top -p $(pgrep sndt)
```

### Maintenance Tasks

```bash
# Update application
sndt update --check
sndt update --install

# Clean up old versions
sndt maintenance --cleanup-old-versions

# Optimize performance
sndt maintenance --optimize
```

This deployment guide provides comprehensive coverage of deployment strategies, platform-specific considerations, and automation approaches for SoluiNet.DevTools.Console across all supported platforms and environments.