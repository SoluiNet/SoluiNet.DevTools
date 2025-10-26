# Multi-Platform Build Scripts

This directory contains build scripts for creating multi-platform deployments of SoluiNet.DevTools.Console.

## Quick Start

### Build All Platforms
```bash
# PowerShell (Windows/Cross-platform)
./scripts/build-all-platforms.ps1

# Bash (Linux/macOS)
./scripts/build-all-platforms.sh
```

### Platform-Specific Builds
```bash
# Windows only
./scripts/build-windows.ps1

# Linux only
./scripts/build-linux.sh

# macOS only
./scripts/build-macos.sh
```

### Testing
```bash
# Run comprehensive tests
./scripts/test-all-platforms.ps1
```

## Supported Platforms

- **Windows**: x64, ARM64
- **Linux**: x64, ARM64  
- **macOS**: x64 (Intel), ARM64 (Apple Silicon)

## Output Structure

```
build/
├── multi-platform/
│   ├── win-x64/
│   ├── win-arm64/
│   ├── linux-x64/
│   ├── linux-arm64/
│   ├── osx-x64/
│   └── osx-arm64/
├── windows/
├── linux/
└── macos/
```

## Requirements

- .NET 8.0 SDK
- PowerShell (for .ps1 scripts)
- Bash (for .sh scripts)

## CI/CD Integration

The GitHub Actions workflow (`.github/workflows/build-project.yml`) automatically builds and tests all platforms on every push to main/develop branches.

For detailed documentation, see [docs/BUILD.md](../docs/BUILD.md).