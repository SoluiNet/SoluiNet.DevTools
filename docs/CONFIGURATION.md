# Configuration Guide

This guide explains how to configure SoluiNet.DevTools.Console for optimal performance across different platforms and use cases.

## Configuration Overview

SoluiNet.DevTools.Console uses a hierarchical configuration system that adapts to platform conventions while maintaining consistency across operating systems.

## Configuration Locations

### Platform-Specific Directories

The application stores configuration files in platform-appropriate locations:

#### Windows
- **Primary Config**: `%APPDATA%\SoluiNet\DevTools\`
- **Alternative**: `%LOCALAPPDATA%\SoluiNet\DevTools\` (if APPDATA is not writable)
- **System-wide**: `%PROGRAMDATA%\SoluiNet\DevTools\` (requires administrator privileges)

**Example Paths:**
```
C:\Users\[Username]\AppData\Roaming\SoluiNet\DevTools\
├── config.json
├── plugins.json
├── logging.json
├── plugins\
└── logs\
```

#### Linux
- **Primary Config**: `~/.config/soluinet-devtools/`
- **Alternative**: `~/.soluinet-devtools/` (fallback for older systems)
- **System-wide**: `/etc/soluinet-devtools/` (requires root privileges)

**Example Paths:**
```
/home/[username]/.config/soluinet-devtools/
├── config.json
├── plugins.json
├── logging.json
├── plugins/
└── logs/
```

#### macOS
- **Primary Config**: `~/Library/Application Support/SoluiNet.DevTools/`
- **Alternative**: `~/.soluinet-devtools/` (fallback)
- **System-wide**: `/Library/Application Support/SoluiNet.DevTools/` (requires admin privileges)

**Example Paths:**
```
/Users/[username]/Library/Application Support/SoluiNet.DevTools/
├── config.json
├── plugins.json
├── logging.json
├── plugins/
└── logs/
```

### Configuration Files

| File | Purpose | Format |
|------|---------|--------|
| `config.json` | Main application configuration | JSON |
| `plugins.json` | Plugin settings and enabled/disabled state | JSON |
| `logging.json` | Logging configuration and levels | JSON |
| `connections.json` | Database and service connections | JSON (encrypted) |
| `user-preferences.json` | User interface and behavior preferences | JSON |

## Main Configuration (config.json)

### Basic Configuration

```json
{
  "version": "1.0.0",
  "platform": {
    "autoDetect": true,
    "overridePlatform": null,
    "architecture": "auto"
  },
  "application": {
    "enableTelemetry": false,
    "checkForUpdates": true,
    "updateChannel": "stable",
    "startupTimeout": 30000,
    "shutdownTimeout": 10000
  },
  "plugins": {
    "autoLoadEnabled": true,
    "loadTimeout": 15000,
    "enabledPlugins": [],
    "disabledPlugins": [],
    "pluginDirectories": [
      "./plugins",
      "{ConfigDir}/plugins"
    ]
  },
  "logging": {
    "level": "Information",
    "enableConsoleLogging": true,
    "enableFileLogging": true,
    "maxLogFiles": 10,
    "maxLogSizeBytes": 10485760
  },
  "performance": {
    "maxMemoryUsageMB": 512,
    "enableGarbageCollection": true,
    "threadPoolSize": "auto"
  }
}
```

### Platform-Specific Overrides

You can specify platform-specific settings using conditional sections:

```json
{
  "platform": {
    "windows": {
      "plugins": {
        "pluginDirectories": [
          "./plugins",
          "%APPDATA%\\SoluiNet\\DevTools\\plugins"
        ]
      },
      "logging": {
        "logDirectory": "%APPDATA%\\SoluiNet\\DevTools\\logs"
      }
    },
    "linux": {
      "plugins": {
        "pluginDirectories": [
          "./plugins",
          "~/.config/soluinet-devtools/plugins",
          "/usr/local/share/soluinet-devtools/plugins"
        ]
      },
      "logging": {
        "logDirectory": "~/.config/soluinet-devtools/logs"
      }
    },
    "macos": {
      "plugins": {
        "pluginDirectories": [
          "./plugins",
          "~/Library/Application Support/SoluiNet.DevTools/plugins",
          "/usr/local/share/soluinet-devtools/plugins"
        ]
      },
      "logging": {
        "logDirectory": "~/Library/Application Support/SoluiNet.DevTools/logs"
      }
    }
  }
}
```

## Plugin Configuration (plugins.json)

### Plugin Management

```json
{
  "autoDiscovery": true,
  "loadOrder": [
    "SoluiNet.DevTools.Core",
    "SoluiNet.DevTools.SqlPlugin.Example",
    "SoluiNet.DevTools.Utils.Json"
  ],
  "plugins": {
    "SoluiNet.DevTools.SqlPlugin.Example": {
      "enabled": true,
      "autoLoad": true,
      "configuration": {
        "defaultConnectionTimeout": 30,
        "maxQueryResults": 1000
      }
    },
    "SoluiNet.DevTools.Utils.Json": {
      "enabled": true,
      "autoLoad": true,
      "configuration": {
        "prettyPrint": true,
        "validateJson": true
      }
    }
  },
  "compatibility": {
    "requireExactVersion": false,
    "allowPrerelease": false,
    "minimumNetVersion": "8.0"
  }
}
```

### Plugin-Specific Settings

Each plugin can have its own configuration section:

```json
{
  "plugins": {
    "SoluiNet.DevTools.Communication.EMail": {
      "enabled": true,
      "configuration": {
        "smtpServer": "smtp.gmail.com",
        "smtpPort": 587,
        "enableSsl": true,
        "timeout": 30000
      }
    },
    "SoluiNet.DevTools.Utils.Crypto": {
      "enabled": true,
      "configuration": {
        "defaultAlgorithm": "AES256",
        "keySize": 256,
        "enableHardwareAcceleration": true
      }
    }
  }
}
```

## Logging Configuration (logging.json)

### NLog Configuration

```json
{
  "NLog": {
    "autoReload": true,
    "throwConfigExceptions": true,
    "targets": {
      "console": {
        "type": "ColoredConsole",
        "layout": "${longdate} ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}"
      },
      "file": {
        "type": "File",
        "fileName": "${configdir}/logs/soluinet-devtools-${shortdate}.log",
        "layout": "${longdate} ${level:uppercase=true} ${logger} ${message} ${exception:format=tostring}",
        "archiveFileName": "${configdir}/logs/archive/soluinet-devtools-{#}.log",
        "archiveEvery": "Day",
        "archiveNumbering": "Rolling",
        "maxArchiveFiles": 7,
        "concurrentWrites": true,
        "keepFileOpen": false
      }
    },
    "rules": [
      {
        "logger": "*",
        "minLevel": "Info",
        "writeTo": "console"
      },
      {
        "logger": "*",
        "minLevel": "Debug",
        "writeTo": "file"
      }
    ]
  },
  "platformSpecific": {
    "windows": {
      "targets": {
        "eventlog": {
          "type": "EventLog",
          "source": "SoluiNet.DevTools",
          "log": "Application",
          "layout": "${message} ${exception:format=tostring}"
        }
      }
    },
    "linux": {
      "targets": {
        "syslog": {
          "type": "Syslog",
          "facility": "Local0",
          "syslogServer": "127.0.0.1",
          "port": 514,
          "layout": "${level:uppercase=true} ${logger} ${message}"
        }
      }
    },
    "macos": {
      "targets": {
        "oslog": {
          "type": "OSLog",
          "subsystem": "com.soluinet.devtools",
          "category": "general",
          "layout": "${message}"
        }
      }
    }
  }
}
```

## Environment Variables

### Configuration Override Variables

| Variable | Description | Example |
|----------|-------------|---------|
| `SOLUINET_DEVTOOLS_CONFIG_DIR` | Override config directory | `/custom/config/path` |
| `SOLUINET_DEVTOOLS_PLUGIN_DIR` | Additional plugin directory | `/custom/plugins` |
| `SOLUINET_DEVTOOLS_LOG_LEVEL` | Override log level | `Debug`, `Info`, `Warning`, `Error` |
| `SOLUINET_DEVTOOLS_NO_TELEMETRY` | Disable telemetry | `true`, `1`, `yes` |
| `SOLUINET_DEVTOOLS_OFFLINE` | Disable network features | `true`, `1`, `yes` |

### Platform-Specific Variables

#### Windows
```powershell
# PowerShell
$env:SOLUINET_DEVTOOLS_CONFIG_DIR = "C:\MyConfig\SoluiNet"
$env:SOLUINET_DEVTOOLS_LOG_LEVEL = "Debug"

# Command Prompt
set SOLUINET_DEVTOOLS_CONFIG_DIR=C:\MyConfig\SoluiNet
set SOLUINET_DEVTOOLS_LOG_LEVEL=Debug
```

#### Linux/macOS
```bash
# Bash/Zsh
export SOLUINET_DEVTOOLS_CONFIG_DIR="/home/user/.config/soluinet-custom"
export SOLUINET_DEVTOOLS_LOG_LEVEL="Debug"

# Add to ~/.bashrc or ~/.zshrc for persistence
echo 'export SOLUINET_DEVTOOLS_LOG_LEVEL="Info"' >> ~/.bashrc
```

## Command-Line Configuration

### Configuration Commands

```bash
# View current configuration
sndt config show

# Show specific configuration section
sndt config show --section plugins
sndt config show --section logging

# Set configuration values
sndt config set application.enableTelemetry false
sndt config set logging.level Debug
sndt config set plugins.autoLoadEnabled true

# Reset configuration to defaults
sndt config reset
sndt config reset --section plugins

# Validate configuration
sndt config validate

# Export configuration
sndt config export --file config-backup.json

# Import configuration
sndt config import --file config-backup.json
```

### Plugin Management Commands

```bash
# List available plugins
sndt plugin list

# Enable/disable plugins
sndt plugin enable SoluiNet.DevTools.Utils.Json
sndt plugin disable SoluiNet.DevTools.Utils.Crypto

# Install plugins
sndt plugin install --name SoluiNet.DevTools.Utils.File
sndt plugin install --file /path/to/plugin.dll

# Update plugins
sndt plugin update --all
sndt plugin update --name SoluiNet.DevTools.Utils.Json

# Remove plugins
sndt plugin remove SoluiNet.DevTools.Utils.Crypto
```

## Advanced Configuration

### Performance Tuning

#### Memory Management
```json
{
  "performance": {
    "maxMemoryUsageMB": 1024,
    "enableGarbageCollection": true,
    "gcMode": "Server",
    "largeObjectHeapCompaction": true
  }
}
```

#### Threading Configuration
```json
{
  "performance": {
    "threadPoolSize": "auto",
    "maxConcurrentOperations": 10,
    "enableParallelProcessing": true,
    "workerThreads": {
      "min": 4,
      "max": 16
    }
  }
}
```

### Security Configuration

#### Plugin Security
```json
{
  "security": {
    "enablePluginSandbox": true,
    "allowUnsignedPlugins": false,
    "trustedPublishers": [
      "CN=SoluiNet, O=SoluiNet, C=US"
    ],
    "restrictedOperations": [
      "FileSystem.Write",
      "Network.External",
      "Registry.Write"
    ]
  }
}
```

#### Connection Security
```json
{
  "security": {
    "encryptConnections": true,
    "encryptionKey": "{generated-key}",
    "requireSsl": true,
    "certificateValidation": "strict"
  }
}
```

## Configuration Migration

### Automatic Migration

The application automatically migrates configuration from previous versions:

1. **Detection**: Checks for existing configuration files
2. **Backup**: Creates backup of current configuration
3. **Migration**: Converts settings to new format
4. **Validation**: Ensures migrated configuration is valid
5. **Cleanup**: Removes obsolete settings

### Manual Migration

```bash
# Migrate from specific version
sndt config migrate --from-version 0.9.0

# Migrate from different location
sndt config migrate --from-path /old/config/location

# Dry run migration (preview changes)
sndt config migrate --dry-run --from-version 0.9.0
```

## Troubleshooting Configuration

### Common Configuration Issues

#### Configuration File Not Found
```bash
# Create default configuration
sndt config init

# Specify custom config location
sndt --config-dir /custom/path config init
```

#### Invalid JSON Format
```bash
# Validate configuration syntax
sndt config validate

# Fix common JSON issues
sndt config fix --auto
```

#### Permission Issues
```bash
# Linux/macOS: Fix permissions
chmod 644 ~/.config/soluinet-devtools/config.json
chmod 755 ~/.config/soluinet-devtools/

# Windows: Run as Administrator or check folder permissions
```

#### Plugin Loading Issues
```bash
# Check plugin compatibility
sndt plugin validate --all

# Reset plugin configuration
sndt config reset --section plugins

# Rebuild plugin cache
sndt plugin refresh
```

### Configuration Debugging

#### Enable Debug Logging
```bash
# Temporary debug mode
sndt --log-level Debug [command]

# Persistent debug mode
sndt config set logging.level Debug
```

#### Configuration Diagnostics
```bash
# Run configuration diagnostics
sndt diagnostics --config

# Check platform detection
sndt diagnostics --platform

# Verify plugin paths
sndt diagnostics --plugins
```

## Best Practices

### Configuration Management

1. **Version Control**: Store configuration templates in version control
2. **Environment Separation**: Use different configurations for dev/test/prod
3. **Secrets Management**: Never store passwords in plain text
4. **Regular Backups**: Backup configuration before major changes
5. **Documentation**: Document custom configuration changes

### Security Best Practices

1. **Principle of Least Privilege**: Only enable necessary plugins
2. **Regular Updates**: Keep plugins and application updated
3. **Audit Logs**: Enable comprehensive logging for security events
4. **Network Security**: Use SSL/TLS for all network communications
5. **Access Control**: Restrict configuration file permissions

### Performance Optimization

1. **Resource Limits**: Set appropriate memory and thread limits
2. **Plugin Selection**: Only load necessary plugins
3. **Logging Levels**: Use appropriate logging levels for production
4. **Monitoring**: Monitor resource usage and performance metrics
5. **Cleanup**: Regularly clean up old log files and temporary data

## Configuration Examples

### Development Environment
```json
{
  "application": {
    "enableTelemetry": false,
    "checkForUpdates": false
  },
  "logging": {
    "level": "Debug",
    "enableConsoleLogging": true
  },
  "plugins": {
    "autoLoadEnabled": true
  },
  "performance": {
    "maxMemoryUsageMB": 2048
  }
}
```

### Production Environment
```json
{
  "application": {
    "enableTelemetry": true,
    "checkForUpdates": true,
    "updateChannel": "stable"
  },
  "logging": {
    "level": "Information",
    "enableConsoleLogging": false,
    "enableFileLogging": true
  },
  "plugins": {
    "autoLoadEnabled": false
  },
  "performance": {
    "maxMemoryUsageMB": 512
  },
  "security": {
    "enablePluginSandbox": true,
    "allowUnsignedPlugins": false
  }
}
```

### Minimal Configuration
```json
{
  "application": {
    "enableTelemetry": false
  },
  "logging": {
    "level": "Warning"
  },
  "plugins": {
    "autoLoadEnabled": false,
    "enabledPlugins": ["SoluiNet.DevTools.Core"]
  }
}
```

## Next Steps

- [Troubleshooting Guide](TROUBLESHOOTING.md) - Resolve configuration issues
- [Plugin Development Guide](PLUGIN_DEVELOPMENT.md) - Create custom plugins
- [API Reference](API_REFERENCE.md) - Integrate with other applications
- [Performance Tuning Guide](PERFORMANCE.md) - Optimize for your use case