# Installation Guide

This guide provides platform-specific installation instructions for SoluiNet.DevTools.Console, a cross-platform developer tools application built on .NET 8.0.

## System Requirements

### Minimum Requirements
- **Operating System**: Windows 10/11, Linux (Ubuntu 18.04+, CentOS 7+, RHEL 7+), macOS 10.15+
- **Architecture**: x64 or ARM64
- **.NET Runtime**: .NET 8.0 Runtime (automatically installed with framework-dependent deployment)
- **Memory**: 512 MB RAM
- **Storage**: 100 MB available space

### Recommended Requirements
- **Memory**: 2 GB RAM or more
- **Storage**: 500 MB available space (for plugins and logs)
- **.NET SDK**: .NET 8.0 SDK (for plugin development)

## Installation Methods

### Windows Installation

#### Method 1: Chocolatey Package Manager (Recommended)

```powershell
# Install Chocolatey if not already installed
Set-ExecutionPolicy Bypass -Scope Process -Force
[System.Net.ServicePointManager]::SecurityProtocol = [System.Net.ServicePointManager]::SecurityProtocol -bor 3072
iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Install SoluiNet DevTools Console
choco install soluinet-devtools-console
```

#### Method 2: Windows Package Manager (WinGet)

```cmd
# Install using WinGet
winget install SoluiNet.DevTools.Console
```

#### Method 3: MSI Installer

1. Download the latest MSI installer from [Releases](https://github.com/SoluiNet/SoluiNet.DevTools/releases)
2. Run the installer as Administrator
3. Follow the installation wizard
4. The application will be installed to `C:\Program Files\SoluiNet\DevTools\Console`

#### Method 4: Portable ZIP

1. Download the Windows ZIP package (`SoluiNet.DevTools.Console_windows_x64.zip` or `SoluiNet.DevTools.Console_windows_arm64.zip`)
2. Extract to your preferred directory (e.g., `C:\Tools\SoluiNet.DevTools.Console`)
3. Add the directory to your PATH environment variable
4. Run `sndt.exe --version` to verify installation

### Linux Installation

#### Method 1: APT Package Manager (Ubuntu/Debian)

```bash
# Add SoluiNet repository
curl -fsSL https://packages.soluinet.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/soluinet-archive-keyring.gpg
echo "deb [arch=$(dpkg --print-architecture) signed-by=/usr/share/keyrings/soluinet-archive-keyring.gpg] https://packages.soluinet.com/apt stable main" | sudo tee /etc/apt/sources.list.d/soluinet.list

# Update package list and install
sudo apt update
sudo apt install soluinet-devtools-console
```

#### Method 2: YUM/DNF Package Manager (RHEL/CentOS/Fedora)

```bash
# Add SoluiNet repository
sudo tee /etc/yum.repos.d/soluinet.repo << EOF
[soluinet]
name=SoluiNet Repository
baseurl=https://packages.soluinet.com/rpm
enabled=1
gpgcheck=1
gpgkey=https://packages.soluinet.com/gpg
EOF

# Install using YUM (RHEL/CentOS)
sudo yum install soluinet-devtools-console

# Or install using DNF (Fedora)
sudo dnf install soluinet-devtools-console
```

#### Method 3: Snap Package

```bash
# Install from Snap Store
sudo snap install soluinet-devtools-console

# Grant necessary permissions
sudo snap connect soluinet-devtools-console:home
sudo snap connect soluinet-devtools-console:removable-media
```

#### Method 4: Flatpak

```bash
# Add Flathub repository if not already added
flatpak remote-add --if-not-exists flathub https://flathub.org/repo/flathub.flatpakrepo

# Install from Flathub
flatpak install flathub com.soluinet.DevTools.Console
```

#### Method 5: TAR.GZ Archive

```bash
# Download and extract
wget https://github.com/SoluiNet/SoluiNet.DevTools/releases/latest/download/SoluiNet.DevTools.Console_linux_x64.tar.gz
tar -xzf SoluiNet.DevTools.Console_linux_x64.tar.gz -C /opt/
sudo ln -s /opt/SoluiNet.DevTools.Console/sndt /usr/local/bin/sndt

# Verify installation
sndt --version
```

### macOS Installation

#### Method 1: Homebrew (Recommended)

```bash
# Install Homebrew if not already installed
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Add SoluiNet tap and install
brew tap soluinet/devtools
brew install soluinet-devtools-console
```

#### Method 2: MacPorts

```bash
# Install MacPorts if not already installed (see https://www.macports.org/install.php)

# Install SoluiNet DevTools Console
sudo port install soluinet-devtools-console
```

#### Method 3: PKG Installer

1. Download the macOS PKG installer from [Releases](https://github.com/SoluiNet/SoluiNet.DevTools/releases)
2. Double-click the PKG file to run the installer
3. Follow the installation wizard
4. The application will be installed to `/Applications/SoluiNet.DevTools.Console`

#### Method 4: DMG Disk Image

1. Download the macOS DMG file from [Releases](https://github.com/SoluiNet/SoluiNet.DevTools/releases)
2. Double-click to mount the disk image
3. Drag the application to your Applications folder or preferred location
4. Add the application directory to your PATH in `~/.zshrc` or `~/.bash_profile`

#### Method 5: TAR.GZ Archive

```bash
# Download and extract
curl -L -o SoluiNet.DevTools.Console_macos_x64.tar.gz https://github.com/SoluiNet/SoluiNet.DevTools/releases/latest/download/SoluiNet.DevTools.Console_macos_x64.tar.gz
tar -xzf SoluiNet.DevTools.Console_macos_x64.tar.gz -C /usr/local/
ln -s /usr/local/SoluiNet.DevTools.Console/sndt /usr/local/bin/sndt

# Verify installation
sndt --version
```

## Architecture-Specific Installation

### ARM64 Devices

#### Windows ARM64 (Surface Pro X, etc.)
```powershell
# Use ARM64-specific package
choco install soluinet-devtools-console --params="--architecture=arm64"
```

#### Linux ARM64 (Raspberry Pi, ARM servers)
```bash
# Download ARM64 package
wget https://github.com/SoluiNet/SoluiNet.DevTools/releases/latest/download/SoluiNet.DevTools.Console_linux_arm64.tar.gz
```

#### macOS ARM64 (Apple Silicon Macs)
```bash
# Homebrew automatically detects architecture
brew install soluinet-devtools-console

# Or download ARM64-specific package
curl -L -o SoluiNet.DevTools.Console_macos_arm64.tar.gz https://github.com/SoluiNet/SoluiNet.DevTools/releases/latest/download/SoluiNet.DevTools.Console_macos_arm64.tar.gz
```

## Post-Installation Setup

### 1. Verify Installation

```bash
# Check version and basic functionality
sndt --version
sndt --help

# List available plugins
sndt --list-plugins

# Run basic diagnostics
sndt --diagnostics
```

### 2. Configuration Directory Setup

The application will automatically create configuration directories on first run:

- **Windows**: `%APPDATA%\SoluiNet\DevTools`
- **Linux**: `~/.config/soluinet-devtools`
- **macOS**: `~/Library/Application Support/SoluiNet.DevTools`

### 3. Plugin Installation

```bash
# Install plugins from the official repository
sndt plugin install --name "SqlPlugin.Example"
sndt plugin install --name "Utils.Json"

# Install plugin from file
sndt plugin install --file "/path/to/plugin.dll"

# List installed plugins
sndt plugin list
```

### 4. Environment Variables (Optional)

Add these environment variables for enhanced functionality:

```bash
# Windows (PowerShell)
$env:SOLUINET_DEVTOOLS_HOME = "C:\Tools\SoluiNet.DevTools"
$env:SOLUINET_DEVTOOLS_PLUGINS = "C:\Tools\SoluiNet.DevTools\Plugins"

# Linux/macOS (Bash/Zsh)
export SOLUINET_DEVTOOLS_HOME="/opt/SoluiNet.DevTools"
export SOLUINET_DEVTOOLS_PLUGINS="/opt/SoluiNet.DevTools/Plugins"
```

## Upgrading

### Package Manager Upgrades

```bash
# Windows (Chocolatey)
choco upgrade soluinet-devtools-console

# Windows (WinGet)
winget upgrade SoluiNet.DevTools.Console

# Linux (APT)
sudo apt update && sudo apt upgrade soluinet-devtools-console

# Linux (YUM/DNF)
sudo yum update soluinet-devtools-console
# or
sudo dnf update soluinet-devtools-console

# macOS (Homebrew)
brew update && brew upgrade soluinet-devtools-console

# macOS (MacPorts)
sudo port selfupdate && sudo port upgrade soluinet-devtools-console
```

### Manual Upgrade

1. Backup your configuration directory
2. Download the latest version
3. Stop any running instances
4. Replace the application files
5. Restart the application

The application will automatically migrate configuration files if needed.

## Uninstallation

### Package Manager Uninstallation

```bash
# Windows (Chocolatey)
choco uninstall soluinet-devtools-console

# Windows (WinGet)
winget uninstall SoluiNet.DevTools.Console

# Linux (APT)
sudo apt remove soluinet-devtools-console

# Linux (YUM/DNF)
sudo yum remove soluinet-devtools-console
# or
sudo dnf remove soluinet-devtools-console

# macOS (Homebrew)
brew uninstall soluinet-devtools-console

# macOS (MacPorts)
sudo port uninstall soluinet-devtools-console
```

### Manual Uninstallation

1. Delete the application directory
2. Remove configuration directories (optional):
   - Windows: `%APPDATA%\SoluiNet\DevTools`
   - Linux: `~/.config/soluinet-devtools`
   - macOS: `~/Library/Application Support/SoluiNet.DevTools`
3. Remove from PATH environment variable
4. Delete any desktop shortcuts or menu entries

## Troubleshooting Installation Issues

### Common Issues

#### .NET Runtime Not Found
```bash
# Install .NET 8.0 Runtime
# Windows
winget install Microsoft.DotNet.Runtime.8

# Linux (Ubuntu/Debian)
sudo apt install dotnet-runtime-8.0

# macOS
brew install --cask dotnet
```

#### Permission Denied Errors
```bash
# Linux/macOS: Make executable
chmod +x /path/to/sndt

# Windows: Run as Administrator
# Right-click PowerShell/Command Prompt -> "Run as Administrator"
```

#### Path Not Found
```bash
# Add to PATH temporarily
export PATH=$PATH:/path/to/soluinet-devtools

# Add to PATH permanently (Linux/macOS)
echo 'export PATH=$PATH:/path/to/soluinet-devtools' >> ~/.bashrc
source ~/.bashrc
```

#### Plugin Loading Issues
```bash
# Check plugin compatibility
sndt plugin validate --file "/path/to/plugin.dll"

# Reset plugin configuration
sndt config reset --plugins
```

### Getting Help

- **Documentation**: [https://docs.soluinet.com/devtools](https://docs.soluinet.com/devtools)
- **Issues**: [https://github.com/SoluiNet/SoluiNet.DevTools/issues](https://github.com/SoluiNet/SoluiNet.DevTools/issues)
- **Discussions**: [https://github.com/SoluiNet/SoluiNet.DevTools/discussions](https://github.com/SoluiNet/SoluiNet.DevTools/discussions)
- **Support**: [support@soluinet.com](mailto:support@soluinet.com)

## Next Steps

After installation, see:
- [Configuration Guide](CONFIGURATION.md) - Configure the application for your needs
- [Plugin Development Guide](PLUGIN_DEVELOPMENT.md) - Create custom plugins
- [Troubleshooting Guide](TROUBLESHOOTING.md) - Resolve common issues
- [API Reference](API_REFERENCE.md) - Integrate with other tools