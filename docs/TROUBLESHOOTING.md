# Troubleshooting Guide

This guide helps you diagnose and resolve common issues with SoluiNet.DevTools.Console across different platforms and configurations.

## Quick Diagnostics

### Built-in Diagnostic Tools

```bash
# Run comprehensive diagnostics
sndt diagnostics

# Check specific components
sndt diagnostics --platform
sndt diagnostics --config
sndt diagnostics --plugins
sndt diagnostics --network

# Generate diagnostic report
sndt diagnostics --report --output diagnostics-report.json
```

### System Information

```bash
# Display system and application information
sndt --version --verbose
sndt info --system
sndt info --runtime
```

## Common Issues by Platform

### Windows Issues

#### Issue: Application Won't Start

**Symptoms:**
- Double-clicking executable does nothing
- Command prompt shows "Access denied" or "File not found"
- Windows Defender or antivirus blocks execution

**Solutions:**

1. **Check .NET Runtime Installation**
   ```powershell
   # Check installed .NET versions
   dotnet --list-runtimes
   
   # Install .NET 8.0 Runtime if missing
   winget install Microsoft.DotNet.Runtime.8
   ```

2. **Run as Administrator**
   ```powershell
   # Right-click PowerShell -> "Run as Administrator"
   # Navigate to application directory
   cd "C:\Program Files\SoluiNet\DevTools\Console"
   .\sndt.exe --version
   ```

3. **Antivirus Exclusion**
   - Add application directory to Windows Defender exclusions
   - Add executable to antivirus whitelist
   - Temporarily disable real-time protection for testing

4. **PowerShell Execution Policy**
   ```powershell
   # Check current policy
   Get-ExecutionPolicy
   
   # Set policy to allow script execution
   Set-ExecutionPolicy -ExecutionPolicy RemoteSigned -Scope CurrentUser
   ```

#### Issue: Plugin Loading Failures

**Symptoms:**
- "Could not load plugin" errors
- Missing functionality
- Assembly loading exceptions

**Solutions:**

1. **Check Plugin Architecture**
   ```powershell
   # Verify plugin architecture matches application
   sndt plugin validate --file "path\to\plugin.dll"
   
   # List plugin dependencies
   sndt plugin info --file "path\to\plugin.dll"
   ```

2. **Unblock Downloaded Files**
   ```powershell
   # Unblock all files in plugins directory
   Get-ChildItem -Path ".\plugins" -Recurse | Unblock-File
   ```

3. **Check File Permissions**
   ```powershell
   # Grant read/execute permissions
   icacls ".\plugins" /grant Users:RX /T
   ```

#### Issue: Configuration Access Denied

**Symptoms:**
- Cannot save configuration
- "Access to path denied" errors
- Configuration changes not persisting

**Solutions:**

1. **Check Directory Permissions**
   ```powershell
   # Check current permissions
   icacls "%APPDATA%\SoluiNet\DevTools"
   
   # Grant full control to current user
   icacls "%APPDATA%\SoluiNet\DevTools" /grant %USERNAME%:F /T
   ```

2. **Use Alternative Configuration Location**
   ```powershell
   # Set custom config directory
   $env:SOLUINET_DEVTOOLS_CONFIG_DIR = "C:\MyConfig\SoluiNet"
   sndt config init
   ```

### Linux Issues

#### Issue: Permission Denied

**Symptoms:**
- "Permission denied" when running executable
- Cannot access configuration files
- Plugin loading failures

**Solutions:**

1. **Make Executable**
   ```bash
   # Add execute permission
   chmod +x /path/to/sndt
   
   # Check current permissions
   ls -la /path/to/sndt
   ```

2. **Fix Configuration Directory Permissions**
   ```bash
   # Create config directory with correct permissions
   mkdir -p ~/.config/soluinet-devtools
   chmod 755 ~/.config/soluinet-devtools
   
   # Fix existing permissions
   chmod -R 644 ~/.config/soluinet-devtools/*.json
   chmod 755 ~/.config/soluinet-devtools/plugins
   ```

3. **SELinux Issues (RHEL/CentOS)**
   ```bash
   # Check SELinux status
   sestatus
   
   # Temporarily disable SELinux for testing
   sudo setenforce 0
   
   # Set proper SELinux context
   sudo setsebool -P allow_execheap 1
   sudo chcon -t bin_t /path/to/sndt
   ```

#### Issue: Missing Dependencies

**Symptoms:**
- "libicu" or other library not found
- Segmentation faults
- Core dumps

**Solutions:**

1. **Install Required Libraries (Ubuntu/Debian)**
   ```bash
   # Install common dependencies
   sudo apt update
   sudo apt install libc6 libgcc1 libgssapi-krb5-2 libicu70 libssl3 libstdc++6 zlib1g
   
   # For older Ubuntu versions
   sudo apt install libicu66 libssl1.1
   ```

2. **Install Required Libraries (RHEL/CentOS)**
   ```bash
   # Install common dependencies
   sudo yum install glibc libgcc krb5-libs libicu openssl-libs libstdc++ zlib
   
   # For newer versions using DNF
   sudo dnf install glibc libgcc krb5-libs libicu openssl-libs libstdc++ zlib
   ```

3. **Check Library Versions**
   ```bash
   # Check available libraries
   ldconfig -p | grep libicu
   ldd /path/to/sndt
   ```

#### Issue: Path Resolution Problems

**Symptoms:**
- Cannot find plugins
- Configuration files not found
- Relative path issues

**Solutions:**

1. **Use Absolute Paths**
   ```bash
   # Set absolute paths in environment
   export SOLUINET_DEVTOOLS_CONFIG_DIR="/home/$USER/.config/soluinet-devtools"
   export SOLUINET_DEVTOOLS_PLUGIN_DIR="/usr/local/share/soluinet-devtools/plugins"
   ```

2. **Check Current Working Directory**
   ```bash
   # Run from application directory
   cd /opt/SoluiNet.DevTools.Console
   ./sndt --version
   ```

3. **Create Symbolic Links**
   ```bash
   # Create symlink in PATH
   sudo ln -s /opt/SoluiNet.DevTools.Console/sndt /usr/local/bin/sndt
   ```

### macOS Issues

#### Issue: Gatekeeper Blocking Execution

**Symptoms:**
- "App can't be opened because it is from an unidentified developer"
- Quarantine attribute preventing execution
- Code signing verification failures

**Solutions:**

1. **Remove Quarantine Attribute**
   ```bash
   # Remove quarantine from application
   xattr -d com.apple.quarantine /path/to/sndt
   
   # Remove quarantine from entire directory
   xattr -dr com.apple.quarantine /Applications/SoluiNet.DevTools.Console
   ```

2. **Allow in System Preferences**
   - Open System Preferences → Security & Privacy
   - Click "Allow Anyway" next to the blocked application
   - Try running the application again

3. **Bypass Gatekeeper (Advanced)**
   ```bash
   # Temporarily disable Gatekeeper (requires admin)
   sudo spctl --master-disable
   
   # Re-enable after installation
   sudo spctl --master-enable
   ```

#### Issue: Rosetta 2 Required (Apple Silicon)

**Symptoms:**
- "Bad CPU type in executable" on Apple Silicon Macs
- x64 binary won't run on ARM64 macOS

**Solutions:**

1. **Install Rosetta 2**
   ```bash
   # Install Rosetta 2 for x64 compatibility
   /usr/sbin/softwareupdate --install-rosetta --agree-to-license
   ```

2. **Use Native ARM64 Binary**
   ```bash
   # Download ARM64-specific version
   curl -L -o SoluiNet.DevTools.Console_macos_arm64.tar.gz \
     https://github.com/SoluiNet/SoluiNet.DevTools/releases/latest/download/SoluiNet.DevTools.Console_macos_arm64.tar.gz
   ```

3. **Check Architecture**
   ```bash
   # Check system architecture
   uname -m
   
   # Check binary architecture
   file /path/to/sndt
   ```

#### Issue: Library Loading Problems

**Symptoms:**
- "dylib not found" errors
- Missing framework dependencies
- Runtime linking failures

**Solutions:**

1. **Install Xcode Command Line Tools**
   ```bash
   # Install development tools
   xcode-select --install
   ```

2. **Check Library Paths**
   ```bash
   # Check dynamic library dependencies
   otool -L /path/to/sndt
   
   # Check library search paths
   echo $DYLD_LIBRARY_PATH
   ```

3. **Install Missing Libraries via Homebrew**
   ```bash
   # Install common libraries
   brew install icu4c openssl zlib
   
   # Link libraries if needed
   brew link --force icu4c
   ```

## Cross-Platform Issues

### Plugin Compatibility Problems

#### Issue: Architecture Mismatch

**Symptoms:**
- "BadImageFormatException" errors
- Plugin fails to load on specific architecture
- Mixed x64/ARM64 plugin issues

**Solutions:**

1. **Check Plugin Architecture**
   ```bash
   # Validate plugin compatibility
   sndt plugin validate --file plugin.dll --architecture current
   
   # List plugin requirements
   sndt plugin info --file plugin.dll --verbose
   ```

2. **Download Correct Architecture**
   ```bash
   # Check system architecture
   sndt info --system | grep Architecture
   
   # Download matching plugin version
   sndt plugin install --name PluginName --architecture arm64
   ```

3. **Use AnyCPU Plugins**
   - Prefer plugins compiled with "Any CPU" target
   - Avoid architecture-specific native dependencies
   - Use managed-only plugin implementations

#### Issue: .NET Version Compatibility

**Symptoms:**
- "Could not load file or assembly" errors
- Version mismatch warnings
- Plugin initialization failures

**Solutions:**

1. **Check .NET Versions**
   ```bash
   # Check application .NET version
   sndt info --runtime
   
   # Check plugin .NET version
   sndt plugin info --file plugin.dll --framework
   ```

2. **Update Plugins**
   ```bash
   # Update all plugins to latest versions
   sndt plugin update --all
   
   # Update specific plugin
   sndt plugin update --name PluginName
   ```

3. **Enable Compatibility Mode**
   ```json
   // In plugins.json
   {
     "compatibility": {
       "allowOlderFrameworks": true,
       "enableLegacySupport": true
     }
   }
   ```

### Configuration Issues

#### Issue: Path Separator Problems

**Symptoms:**
- Configuration paths not found
- Plugin directories not accessible
- Cross-platform path issues

**Solutions:**

1. **Use Platform-Agnostic Paths**
   ```json
   // Use forward slashes or environment variables
   {
     "pluginDirectories": [
       "./plugins",
       "{ConfigDir}/plugins",
       "{UserHome}/.soluinet-devtools/plugins"
     ]
   }
   ```

2. **Environment Variable Expansion**
   ```bash
   # Set platform-appropriate paths
   export SOLUINET_DEVTOOLS_PLUGIN_DIR="$HOME/.config/soluinet-devtools/plugins"
   ```

3. **Automatic Path Normalization**
   ```bash
   # Enable automatic path normalization
   sndt config set platform.autoNormalizePaths true
   ```

#### Issue: Character Encoding Problems

**Symptoms:**
- Configuration files corrupted
- Special characters not displayed correctly
- JSON parsing errors

**Solutions:**

1. **Use UTF-8 Encoding**
   ```bash
   # Check file encoding
   file -bi config.json
   
   # Convert to UTF-8 if needed
   iconv -f ISO-8859-1 -t UTF-8 config.json > config-utf8.json
   ```

2. **Set Locale Environment**
   ```bash
   # Linux/macOS
   export LC_ALL=en_US.UTF-8
   export LANG=en_US.UTF-8
   
   # Windows PowerShell
   $env:LC_ALL = "en_US.UTF-8"
   ```

## Performance Issues

### High Memory Usage

**Symptoms:**
- Application consuming excessive memory
- Out of memory exceptions
- System slowdown

**Solutions:**

1. **Configure Memory Limits**
   ```json
   {
     "performance": {
       "maxMemoryUsageMB": 512,
       "enableGarbageCollection": true,
       "gcMode": "Workstation"
     }
   }
   ```

2. **Disable Unnecessary Plugins**
   ```bash
   # List memory usage by plugin
   sndt diagnostics --memory --plugins
   
   # Disable high-memory plugins
   sndt plugin disable HighMemoryPlugin
   ```

3. **Monitor Memory Usage**
   ```bash
   # Monitor application memory usage
   sndt diagnostics --monitor --interval 5
   ```

### Slow Startup

**Symptoms:**
- Application takes long time to start
- Plugin loading timeouts
- Unresponsive during initialization

**Solutions:**

1. **Optimize Plugin Loading**
   ```json
   {
     "plugins": {
       "autoLoadEnabled": false,
       "loadTimeout": 30000,
       "parallelLoading": true
     }
   }
   ```

2. **Reduce Startup Operations**
   ```bash
   # Disable startup checks
   sndt config set application.checkForUpdates false
   sndt config set application.validatePlugins false
   ```

3. **Use Lazy Loading**
   ```json
   {
     "plugins": {
       "lazyLoading": true,
       "loadOnDemand": true
     }
   }
   ```

## Network and Connectivity Issues

### Proxy Configuration

**Symptoms:**
- Cannot connect to update servers
- Plugin downloads fail
- Network timeouts

**Solutions:**

1. **Configure System Proxy**
   ```bash
   # Windows
   netsh winhttp set proxy proxy-server:port
   
   # Linux/macOS
   export http_proxy=http://proxy-server:port
   export https_proxy=https://proxy-server:port
   ```

2. **Application Proxy Settings**
   ```json
   {
     "network": {
       "proxy": {
         "enabled": true,
         "server": "proxy-server",
         "port": 8080,
         "username": "user",
         "password": "encrypted-password"
       }
     }
   }
   ```

3. **Bypass Proxy for Local Addresses**
   ```json
   {
     "network": {
       "proxy": {
         "bypass": [
           "localhost",
           "127.0.0.1",
           "*.local"
         ]
       }
     }
   }
   ```

### SSL/TLS Issues

**Symptoms:**
- Certificate validation errors
- SSL handshake failures
- Secure connection problems

**Solutions:**

1. **Update Certificate Store**
   ```bash
   # Windows
   certlm.msc  # Update certificates manually
   
   # Linux
   sudo apt update && sudo apt install ca-certificates
   
   # macOS
   # Certificates updated automatically via System Updates
   ```

2. **Configure SSL Settings**
   ```json
   {
     "network": {
       "ssl": {
         "validateCertificates": true,
         "allowSelfSigned": false,
         "tlsVersion": "1.2"
       }
     }
   }
   ```

## Logging and Debugging

### Enable Verbose Logging

```bash
# Temporary verbose logging
sndt --log-level Debug --verbose [command]

# Persistent debug logging
sndt config set logging.level Debug
sndt config set logging.enableConsoleLogging true
```

### Log File Locations

- **Windows**: `%APPDATA%\SoluiNet\DevTools\logs\`
- **Linux**: `~/.config/soluinet-devtools/logs/`
- **macOS**: `~/Library/Application Support/SoluiNet.DevTools/logs/`

### Common Log Patterns

#### Plugin Loading Issues
```
ERROR SoluiNet.DevTools.Core.Plugin.PluginLoader Could not load plugin 'PluginName': System.IO.FileNotFoundException
```

#### Configuration Problems
```
WARN SoluiNet.DevTools.Core.Configuration.ConfigurationManager Configuration file not found, using defaults
```

#### Platform Detection Issues
```
INFO SoluiNet.DevTools.Core.Services.Platform.PlatformHelper Detected platform: Linux, Architecture: X64
```

## Getting Help

### Diagnostic Information to Collect

When reporting issues, include:

1. **System Information**
   ```bash
   sndt info --system > system-info.txt
   ```

2. **Configuration**
   ```bash
   sndt config show > config-dump.json
   ```

3. **Plugin Information**
   ```bash
   sndt plugin list --verbose > plugin-info.txt
   ```

4. **Log Files**
   - Recent log files from the logs directory
   - Any error messages or stack traces

5. **Diagnostic Report**
   ```bash
   sndt diagnostics --report --output diagnostic-report.json
   ```

### Support Channels

- **GitHub Issues**: [https://github.com/SoluiNet/SoluiNet.DevTools/issues](https://github.com/SoluiNet/SoluiNet.DevTools/issues)
- **Discussions**: [https://github.com/SoluiNet/SoluiNet.DevTools/discussions](https://github.com/SoluiNet/SoluiNet.DevTools/discussions)
- **Documentation**: [https://docs.soluinet.com/devtools](https://docs.soluinet.com/devtools)
- **Email Support**: [support@soluinet.com](mailto:support@soluinet.com)

### Before Reporting Issues

1. **Search Existing Issues**: Check if the problem has been reported
2. **Try Latest Version**: Update to the latest release
3. **Minimal Reproduction**: Create minimal steps to reproduce the issue
4. **Environment Details**: Include OS, architecture, and .NET version
5. **Log Files**: Attach relevant log files and diagnostic information

## Emergency Recovery

### Reset to Factory Defaults

```bash
# Backup current configuration
sndt config export --file config-backup.json

# Reset all configuration
sndt config reset --all

# Reset specific components
sndt config reset --plugins
sndt config reset --logging
```

### Safe Mode

```bash
# Start in safe mode (minimal plugins)
sndt --safe-mode

# Start with specific configuration
sndt --config-dir /path/to/safe/config

# Start without plugins
sndt --no-plugins
```

### Recovery Commands

```bash
# Repair installation
sndt repair --all

# Rebuild plugin cache
sndt plugin refresh --rebuild

# Validate and fix configuration
sndt config validate --fix
```