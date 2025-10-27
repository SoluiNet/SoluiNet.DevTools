# Quick Start Guide

Get up and running with SoluiNet.DevTools.Console in just a few minutes. This guide will walk you through installation, basic configuration, and your first commands.

## 5-Minute Setup

### Step 1: Install the Application

Choose the method that works best for your platform:

#### Windows (Recommended: Chocolatey)
```powershell
# Install Chocolatey if you haven't already
Set-ExecutionPolicy Bypass -Scope Process -Force
iex ((New-Object System.Net.WebClient).DownloadString('https://community.chocolatey.org/install.ps1'))

# Install SoluiNet DevTools Console
choco install soluinet-devtools-console
```

#### Linux (Ubuntu/Debian)
```bash
# Add repository and install
curl -fsSL https://packages.soluinet.com/gpg | sudo gpg --dearmor -o /usr/share/keyrings/soluinet-archive-keyring.gpg
echo "deb [signed-by=/usr/share/keyrings/soluinet-archive-keyring.gpg] https://packages.soluinet.com/apt stable main" | sudo tee /etc/apt/sources.list.d/soluinet.list
sudo apt update && sudo apt install soluinet-devtools-console
```

#### macOS (Recommended: Homebrew)
```bash
# Install Homebrew if you haven't already
/bin/bash -c "$(curl -fsSL https://raw.githubusercontent.com/Homebrew/install/HEAD/install.sh)"

# Install SoluiNet DevTools Console
brew tap soluinet/devtools
brew install soluinet-devtools-console
```

#### Alternative: Download Binary
If package managers aren't available, download the appropriate binary for your platform from the [releases page](https://github.com/SoluiNet/SoluiNet.DevTools/releases).

### Step 2: Verify Installation

```bash
# Check that the application is installed and working
sndt --version

# You should see output like:
# SoluiNet.DevTools.Console 1.0.0
# Platform: Linux x64
# .NET Version: 8.0.x
```

### Step 3: Run Initial Setup

```bash
# Initialize configuration (creates default config files)
sndt config init

# Run system diagnostics to ensure everything is working
sndt diagnostics

# You should see a green "✓ All systems operational" message
```

## Your First Commands

### Basic Information Commands

```bash
# Get help on available commands
sndt --help

# Show detailed system information
sndt info --system

# List available plugins
sndt plugin list

# Show current configuration
sndt config show
```

### Working with Plugins

```bash
# Enable a useful plugin (JSON utilities)
sndt plugin enable SoluiNet.DevTools.Utils.Json

# Check plugin status
sndt plugin list --enabled

# Get help for a specific plugin
sndt plugin help SoluiNet.DevTools.Utils.Json
```

### Configuration Basics

```bash
# Set your preferred log level
sndt config set logging.level Information

# Enable automatic plugin loading
sndt config set plugins.autoLoadEnabled true

# View your configuration
sndt config show --section application
```

## Common Use Cases

### 1. JSON Processing

```bash
# Enable JSON utilities plugin
sndt plugin enable SoluiNet.DevTools.Utils.Json

# Format JSON from file
sndt json format --file input.json --output formatted.json

# Validate JSON
sndt json validate --file data.json

# Minify JSON
sndt json minify --file large.json --output compact.json
```

### 2. File Operations

```bash
# Enable file utilities plugin
sndt plugin enable SoluiNet.DevTools.Utils.File

# Calculate file hash
sndt file hash --file document.pdf --algorithm SHA256

# Compare files
sndt file compare --file1 original.txt --file2 modified.txt

# Batch rename files
sndt file rename --pattern "*.txt" --replace "backup_*.txt"
```

### 3. Cryptographic Operations

```bash
# Enable crypto utilities plugin
sndt plugin enable SoluiNet.DevTools.Utils.Crypto

# Encrypt a file
sndt crypto encrypt --file sensitive.txt --password mypassword

# Generate secure password
sndt crypto password --length 16 --include-symbols

# Hash text
sndt crypto hash --text "Hello World" --algorithm SHA256
```

### 4. Database Operations (Example Plugin)

```bash
# Enable SQL plugin
sndt plugin enable SoluiNet.DevTools.SqlPlugin.Example

# Connect to database
sndt sql connect --server localhost --database mydb --username user

# Execute query
sndt sql query --sql "SELECT * FROM users LIMIT 10"

# Export data
sndt sql export --table users --format csv --output users.csv
```

## Configuration Examples

### Development Environment Setup

```bash
# Configure for development work
sndt config set logging.level Debug
sndt config set application.enableTelemetry false
sndt config set plugins.autoLoadEnabled true
sndt config set performance.maxMemoryUsageMB 1024

# Enable commonly used development plugins
sndt plugin enable SoluiNet.DevTools.Utils.Json
sndt plugin enable SoluiNet.DevTools.Utils.File
sndt plugin enable SoluiNet.DevTools.Utils.Crypto
sndt plugin enable SoluiNet.DevTools.Utils.Git
```

### Production Environment Setup

```bash
# Configure for production use
sndt config set logging.level Information
sndt config set application.enableTelemetry true
sndt config set plugins.autoLoadEnabled false
sndt config set performance.maxMemoryUsageMB 512

# Enable only necessary plugins
sndt plugin enable SoluiNet.DevTools.Core
sndt plugin enable SoluiNet.DevTools.SqlPlugin.Example
```

### Minimal Setup

```bash
# Minimal configuration for basic use
sndt config set logging.level Warning
sndt config set plugins.autoLoadEnabled false
sndt config set application.checkForUpdates false

# Enable only core functionality
sndt plugin enable SoluiNet.DevTools.Core
```

## Platform-Specific Tips

### Windows Tips

```powershell
# Add to PowerShell profile for easier access
Add-Content $PROFILE "Set-Alias sndt 'C:\Program Files\SoluiNet\DevTools\Console\sndt.exe'"

# Create desktop shortcut
$WshShell = New-Object -comObject WScript.Shell
$Shortcut = $WshShell.CreateShortcut("$Home\Desktop\SoluiNet DevTools.lnk")
$Shortcut.TargetPath = "C:\Program Files\SoluiNet\DevTools\Console\sndt.exe"
$Shortcut.Save()

# Windows-specific plugin for system utilities
sndt plugin enable SoluiNet.DevTools.Utils.Windows
```

### Linux Tips

```bash
# Add to shell profile for easier access
echo 'alias sndt="/usr/local/bin/sndt"' >> ~/.bashrc
source ~/.bashrc

# Create desktop entry (Ubuntu/GNOME)
cat > ~/.local/share/applications/soluinet-devtools.desktop << EOF
[Desktop Entry]
Name=SoluiNet DevTools Console
Exec=/usr/local/bin/sndt
Icon=utilities-terminal
Type=Application
Categories=Development;
EOF

# Linux-specific system integration
sndt plugin enable SoluiNet.DevTools.Utils.Linux
```

### macOS Tips

```bash
# Add to shell profile
echo 'alias sndt="/usr/local/bin/sndt"' >> ~/.zshrc
source ~/.zshrc

# Create Spotlight-searchable app (optional)
mkdir -p ~/Applications/SoluiNet.DevTools.Console.app/Contents/MacOS
ln -s /usr/local/bin/sndt ~/Applications/SoluiNet.DevTools.Console.app/Contents/MacOS/sndt

# macOS-specific integrations
sndt plugin enable SoluiNet.DevTools.Utils.macOS
```

## Troubleshooting Quick Fixes

### Application Won't Start

```bash
# Check .NET runtime installation
dotnet --version

# If .NET is missing, install it:
# Windows: winget install Microsoft.DotNet.Runtime.8
# Linux: sudo apt install dotnet-runtime-8.0
# macOS: brew install --cask dotnet
```

### Plugin Loading Issues

```bash
# Reset plugin configuration
sndt config reset --plugins

# Validate all plugins
sndt plugin validate --all

# Check plugin directory permissions
sndt diagnostics --plugins
```

### Configuration Problems

```bash
# Reset to factory defaults
sndt config reset --all

# Validate configuration
sndt config validate

# Check configuration file permissions
sndt diagnostics --config
```

### Performance Issues

```bash
# Check system resources
sndt diagnostics --performance

# Reduce memory usage
sndt config set performance.maxMemoryUsageMB 256

# Disable unnecessary plugins
sndt plugin disable [plugin-name]
```

## Next Steps

Now that you have SoluiNet.DevTools.Console up and running, explore these resources:

### Learn More
- **[Configuration Guide](CONFIGURATION.md)** - Customize the application for your needs
- **[Plugin Development](PLUGIN_DEVELOPMENT.md)** - Create your own plugins
- **[Troubleshooting Guide](TROUBLESHOOTING.md)** - Solve common issues

### Advanced Usage
- **[Build Guide](BUILD.md)** - Build from source code
- **[Deployment Guide](DEPLOYMENT.md)** - Deploy in different environments
- **[API Reference](API_REFERENCE.md)** - Integrate with other applications

### Get Involved
- **[Contributing Guide](../CONTRIBUTING.md)** - Contribute to the project
- **[GitHub Issues](https://github.com/SoluiNet/SoluiNet.DevTools/issues)** - Report bugs or request features
- **[Community Discussions](https://github.com/SoluiNet/SoluiNet.DevTools/discussions)** - Join the community

## Useful Commands Reference

### Information and Help
```bash
sndt --version                    # Show version
sndt --help                       # Show help
sndt info --system               # System information
sndt diagnostics                 # Run diagnostics
```

### Configuration Management
```bash
sndt config show                 # Show all configuration
sndt config show --section X     # Show specific section
sndt config set key value        # Set configuration value
sndt config reset               # Reset to defaults
sndt config validate            # Validate configuration
```

### Plugin Management
```bash
sndt plugin list                # List all plugins
sndt plugin list --enabled      # List enabled plugins
sndt plugin enable name         # Enable plugin
sndt plugin disable name        # Disable plugin
sndt plugin info name           # Show plugin information
```

### Maintenance
```bash
sndt health                     # Health check
sndt maintenance --cleanup      # Clean up old files
sndt update --check            # Check for updates
sndt diagnostics --report      # Generate diagnostic report
```

## Getting Help

If you run into issues or have questions:

1. **Check the documentation**: Most common questions are answered in the guides
2. **Run diagnostics**: `sndt diagnostics` often identifies problems
3. **Search existing issues**: Check [GitHub Issues](https://github.com/SoluiNet/SoluiNet.DevTools/issues)
4. **Ask the community**: Use [GitHub Discussions](https://github.com/SoluiNet/SoluiNet.DevTools/discussions)
5. **Contact support**: Email [support@soluinet.com](mailto:support@soluinet.com)

Welcome to SoluiNet.DevTools.Console! We hope you find it useful for your development workflow.