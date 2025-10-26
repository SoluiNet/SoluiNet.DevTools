#!/bin/bash

# Create macOS-specific packages for SoluiNet.DevTools.Console
# This script creates DMG, PKG, and Homebrew formula

set -e

# Default parameters
BUILD_PATH="./build/macos"
OUTPUT_PATH="./packages/macos"
VERSION="1.0.0"
BUNDLE_ID="net.soluinet.devtools.console"
DEVELOPER_ID=""

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -b|--build-path)
            BUILD_PATH="$2"
            shift 2
            ;;
        -o|--output-path)
            OUTPUT_PATH="$2"
            shift 2
            ;;
        -v|--version)
            VERSION="$2"
            shift 2
            ;;
        --bundle-id)
            BUNDLE_ID="$2"
            shift 2
            ;;
        --developer-id)
            DEVELOPER_ID="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  -b, --build-path      Path to built binaries. Default: ./build/macos"
            echo "  -o, --output-path     Output directory for packages. Default: ./packages/macos"
            echo "  -v, --version         Version string. Default: 1.0.0"
            echo "  --bundle-id          Bundle identifier. Default: net.soluinet.devtools.console"
            echo "  --developer-id       Developer ID for code signing (optional)"
            echo "  -h, --help           Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
RED='\033[0;31m'
NC='\033[0m'

echo -e "${GREEN}Creating macOS packages for SoluiNet.DevTools.Console${NC}"
echo -e "${YELLOW}Build Path: $BUILD_PATH${NC}"
echo -e "${YELLOW}Output Path: $OUTPUT_PATH${NC}"
echo -e "${YELLOW}Version: $VERSION${NC}"
echo -e "${YELLOW}Bundle ID: $BUNDLE_ID${NC}"

# Ensure output directory exists
mkdir -p "$OUTPUT_PATH"

# Track package results
declare -a PACKAGE_RESULTS
SUCCESS_COUNT=0
TOTAL_COUNT=0

# Create TAR.GZ packages for each architecture
for arch in "osx-x64" "osx-arm64"; do
    ARCH_BUILD_PATH="$BUILD_PATH/$arch"
    
    if [ -d "$ARCH_BUILD_PATH" ]; then
        echo -e "${CYAN}Creating TAR.GZ package for $arch...${NC}"
        
        TAR_PATH="$OUTPUT_PATH/soluinet-devtools-console_${arch}_v${VERSION}.tar.gz"
        
        if tar -czf "$TAR_PATH" -C "$ARCH_BUILD_PATH" .; then
            FILE_SIZE=$(stat -f%z "$TAR_PATH" 2>/dev/null || stat -c%s "$TAR_PATH" 2>/dev/null)
            SIZE_MB=$(echo "scale=2; $FILE_SIZE / 1048576" | bc -l 2>/dev/null || echo "0")
            echo -e "${GREEN}✓ Created TAR.GZ package: $(basename "$TAR_PATH") (${SIZE_MB} MB)${NC}"
            
            PACKAGE_RESULTS+=("$arch:TAR.GZ:Success:$TAR_PATH:$FILE_SIZE")
            ((SUCCESS_COUNT++))
        else
            echo -e "${RED}✗ Failed to create TAR.GZ package for $arch${NC}"
            PACKAGE_RESULTS+=("$arch:TAR.GZ:Failed::0")
        fi
        ((TOTAL_COUNT++))
    else
        echo -e "${YELLOW}! Build path not found for $arch: $ARCH_BUILD_PATH${NC}"
        PACKAGE_RESULTS+=("$arch:TAR.GZ:Build Not Found::0")
        ((TOTAL_COUNT++))
    fi
done

# Create universal DMG (if hdiutil is available)
if command -v hdiutil &> /dev/null; then
    echo -e "${CYAN}Creating DMG package...${NC}"
    
    DMG_DIR="$OUTPUT_PATH/dmg_staging"
    mkdir -p "$DMG_DIR"
    
    # Use x64 build as primary (or ARM64 if x64 not available)
    PRIMARY_ARCH=""
    if [ -d "$BUILD_PATH/osx-x64" ]; then
        PRIMARY_ARCH="osx-x64"
    elif [ -d "$BUILD_PATH/osx-arm64" ]; then
        PRIMARY_ARCH="osx-arm64"
    fi
    
    if [ -n "$PRIMARY_ARCH" ]; then
        # Check if we have an app bundle or just the executable
        if [ -d "$BUILD_PATH/$PRIMARY_ARCH/SoluiNet.DevTools.Console.app" ]; then
            # Copy app bundle
            cp -r "$BUILD_PATH/$PRIMARY_ARCH/SoluiNet.DevTools.Console.app" "$DMG_DIR/"
            APP_NAME="SoluiNet.DevTools.Console.app"
        else
            # Create a simple app bundle structure
            APP_NAME="SoluiNet DevTools Console.app"
            mkdir -p "$DMG_DIR/$APP_NAME/Contents/MacOS"
            mkdir -p "$DMG_DIR/$APP_NAME/Contents/Resources"
            
            # Copy executable and resources
            cp -r "$BUILD_PATH/$PRIMARY_ARCH"/* "$DMG_DIR/$APP_NAME/Contents/Resources/"
            
            # Move executable to MacOS folder
            if [ -f "$DMG_DIR/$APP_NAME/Contents/Resources/sndt" ]; then
                mv "$DMG_DIR/$APP_NAME/Contents/Resources/sndt" "$DMG_DIR/$APP_NAME/Contents/MacOS/"
                chmod +x "$DMG_DIR/$APP_NAME/Contents/MacOS/sndt"
            fi
            
            # Create Info.plist
            cat > "$DMG_DIR/$APP_NAME/Contents/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>sndt</string>
    <key>CFBundleIdentifier</key>
    <string>$BUNDLE_ID</string>
    <key>CFBundleName</key>
    <string>SoluiNet DevTools Console</string>
    <key>CFBundleVersion</key>
    <string>$VERSION</string>
    <key>CFBundleShortVersionString</key>
    <string>$VERSION</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
    <key>LSApplicationCategoryType</key>
    <string>public.app-category.developer-tools</string>
</dict>
</plist>
EOF
        fi
        
        # Create Applications symlink
        ln -s /Applications "$DMG_DIR/Applications"
        
        # Create DMG
        DMG_PATH="$OUTPUT_PATH/SoluiNet.DevTools.Console_v${VERSION}.dmg"
        
        if hdiutil create -volname "SoluiNet DevTools Console" -srcfolder "$DMG_DIR" -ov -format UDZO "$DMG_PATH"; then
            FILE_SIZE=$(stat -f%z "$DMG_PATH" 2>/dev/null || stat -c%s "$DMG_PATH" 2>/dev/null)
            SIZE_MB=$(echo "scale=2; $FILE_SIZE / 1048576" | bc -l 2>/dev/null || echo "0")
            echo -e "${GREEN}✓ Created DMG package: $(basename "$DMG_PATH") (${SIZE_MB} MB)${NC}"
            
            PACKAGE_RESULTS+=("Universal:DMG:Success:$DMG_PATH:$FILE_SIZE")
            ((SUCCESS_COUNT++))
        else
            echo -e "${RED}✗ Failed to create DMG package${NC}"
            PACKAGE_RESULTS+=("Universal:DMG:Failed::0")
        fi
        ((TOTAL_COUNT++))
        
        # Clean up
        rm -rf "$DMG_DIR"
    else
        echo -e "${YELLOW}! No macOS builds found for DMG package${NC}"
        PACKAGE_RESULTS+=("Universal:DMG:Build Not Found::0")
        ((TOTAL_COUNT++))
    fi
else
    echo -e "${YELLOW}! hdiutil not found, skipping DMG package creation${NC}"
    PACKAGE_RESULTS+=("Universal:DMG:Tool Not Available::0")
    ((TOTAL_COUNT++))
fi

# Create PKG installer (if pkgbuild is available)
if command -v pkgbuild &> /dev/null; then
    echo -e "${CYAN}Creating PKG installer...${NC}"
    
    PKG_DIR="$OUTPUT_PATH/pkg_staging"
    mkdir -p "$PKG_DIR/usr/local/bin"
    
    # Use x64 build as primary
    if [ -d "$BUILD_PATH/osx-x64" ]; then
        cp -r "$BUILD_PATH/osx-x64"/* "$PKG_DIR/usr/local/bin/"
        chmod +x "$PKG_DIR/usr/local/bin/sndt"
        
        PKG_PATH="$OUTPUT_PATH/SoluiNet.DevTools.Console_v${VERSION}.pkg"
        
        if pkgbuild --root "$PKG_DIR" --identifier "$BUNDLE_ID" --version "$VERSION" "$PKG_PATH"; then
            FILE_SIZE=$(stat -f%z "$PKG_PATH" 2>/dev/null || stat -c%s "$PKG_PATH" 2>/dev/null)
            SIZE_MB=$(echo "scale=2; $FILE_SIZE / 1048576" | bc -l 2>/dev/null || echo "0")
            echo -e "${GREEN}✓ Created PKG installer: $(basename "$PKG_PATH") (${SIZE_MB} MB)${NC}"
            
            PACKAGE_RESULTS+=("x64:PKG:Success:$PKG_PATH:$FILE_SIZE")
            ((SUCCESS_COUNT++))
        else
            echo -e "${RED}✗ Failed to create PKG installer${NC}"
            PACKAGE_RESULTS+=("x64:PKG:Failed::0")
        fi
        ((TOTAL_COUNT++))
        
        # Clean up
        rm -rf "$PKG_DIR"
    else
        echo -e "${YELLOW}! No osx-x64 build found for PKG installer${NC}"
        PACKAGE_RESULTS+=("x64:PKG:Build Not Found::0")
        ((TOTAL_COUNT++))
    fi
else
    echo -e "${YELLOW}! pkgbuild not found, skipping PKG installer creation${NC}"
    PACKAGE_RESULTS+=("x64:PKG:Tool Not Available::0")
    ((TOTAL_COUNT++))
fi

# Create Homebrew formula
echo -e "${CYAN}Creating Homebrew formula...${NC}"

HOMEBREW_DIR="$OUTPUT_PATH/homebrew"
mkdir -p "$HOMEBREW_DIR"

cat > "$HOMEBREW_DIR/soluinet-devtools-console.rb" << EOF
class SoluinetDevtoolsConsole < Formula
  desc "Cross-platform development tools console application"
  homepage "https://github.com/SoluiNet/SoluiNet.DevTools"
  version "$VERSION"
  
  if Hardware::CPU.intel?
    url "https://github.com/SoluiNet/SoluiNet.DevTools/releases/download/v#{version}/soluinet-devtools-console_osx-x64_v#{version}.tar.gz"
    sha256 "CHECKSUM_X64_PLACEHOLDER"
  elsif Hardware::CPU.arm?
    url "https://github.com/SoluiNet/SoluiNet.DevTools/releases/download/v#{version}/soluinet-devtools-console_osx-arm64_v#{version}.tar.gz"
    sha256 "CHECKSUM_ARM64_PLACEHOLDER"
  end

  depends_on "dotnet" => :runtime

  def install
    bin.install "sndt"
    
    # Install plugins if they exist
    if Dir.exist?("Plugins")
      (libexec/"Plugins").install Dir["Plugins/*"]
    end
    
    # Install other resources
    Dir["*"].each do |file|
      next if file == "sndt" || file == "Plugins"
      (libexec/file).install file if File.file?(file)
    end
  end

  test do
    system "#{bin}/sndt", "--version"
  end
end
EOF

echo -e "${GREEN}✓ Created Homebrew formula${NC}"
PACKAGE_RESULTS+=("All:Homebrew:Success:$HOMEBREW_DIR:0")
((SUCCESS_COUNT++))
((TOTAL_COUNT++))

# Create MacPorts Portfile
echo -e "${CYAN}Creating MacPorts Portfile...${NC}"

MACPORTS_DIR="$OUTPUT_PATH/macports"
mkdir -p "$MACPORTS_DIR"

cat > "$MACPORTS_DIR/Portfile" << EOF
# -*- coding: utf-8; mode: tcl; tab-width: 4; indent-tabs-mode: nil; c-basic-offset: 4 -*- vim:fenc=utf-8:ft=tcl:et:sw=4:ts=4:sts=4

PortSystem          1.0

name                soluinet-devtools-console
version             $VERSION
categories          devel
platforms           darwin
maintainers         {soluinet.net:info @soluinet}
license             MIT

description         SoluiNet DevTools Console

long_description    Cross-platform development tools console application that provides \\
                    various development tools and utilities with plugin-based architecture.

homepage            https://github.com/SoluiNet/SoluiNet.DevTools

master_sites        https://github.com/SoluiNet/SoluiNet.DevTools/releases/download/v\${version}/

distfiles           soluinet-devtools-console_osx-x64_v\${version}.tar.gz

checksums           rmd160  CHECKSUM_RMD160_PLACEHOLDER \\
                    sha256  CHECKSUM_SHA256_PLACEHOLDER \\
                    size    SIZE_PLACEHOLDER

depends_run         port:dotnet-cli

use_configure       no

build {}

destroot {
    xinstall -m 755 \${worksrcpath}/sndt \${destroot}\${prefix}/bin/
    
    if {[file exists \${worksrcpath}/Plugins]} {
        xinstall -d \${destroot}\${prefix}/libexec/soluinet-devtools-console
        copy \${worksrcpath}/Plugins \${destroot}\${prefix}/libexec/soluinet-devtools-console/
    }
}
EOF

echo -e "${GREEN}✓ Created MacPorts Portfile${NC}"
PACKAGE_RESULTS+=("All:MacPorts:Success:$MACPORTS_DIR:0")
((SUCCESS_COUNT++))
((TOTAL_COUNT++))

# Code signing (if Developer ID is provided)
if [ -n "$DEVELOPER_ID" ]; then
    echo -e "${CYAN}Code signing packages...${NC}"
    
    # Sign DMG if it exists
    DMG_PATH="$OUTPUT_PATH/SoluiNet.DevTools.Console_v${VERSION}.dmg"
    if [ -f "$DMG_PATH" ]; then
        if codesign --sign "$DEVELOPER_ID" "$DMG_PATH"; then
            echo -e "${GREEN}✓ Signed DMG package${NC}"
        else
            echo -e "${YELLOW}! Failed to sign DMG package${NC}"
        fi
    fi
    
    # Sign PKG if it exists
    PKG_PATH="$OUTPUT_PATH/SoluiNet.DevTools.Console_v${VERSION}.pkg"
    if [ -f "$PKG_PATH" ]; then
        if productsign --sign "$DEVELOPER_ID" "$PKG_PATH" "${PKG_PATH%.pkg}_signed.pkg"; then
            mv "${PKG_PATH%.pkg}_signed.pkg" "$PKG_PATH"
            echo -e "${GREEN}✓ Signed PKG installer${NC}"
        else
            echo -e "${YELLOW}! Failed to sign PKG installer${NC}"
        fi
    fi
else
    echo -e "${YELLOW}! No Developer ID provided, skipping code signing${NC}"
fi

# Summary
echo ""
echo -e "${GREEN}macOS Packaging Summary:${NC}"
echo -e "${GREEN}=======================${NC}"

for result in "${PACKAGE_RESULTS[@]}"; do
    IFS=':' read -r arch package_type status path size <<< "$result"
    
    case $status in
        "Success")
            COLOR=$GREEN
            ;;
        "Tool Not Available"|"Build Not Found")
            COLOR=$YELLOW
            ;;
        *)
            COLOR=$RED
            ;;
    esac
    
    if [ "$size" -gt 0 ]; then
        SIZE_MB=$(echo "scale=2; $size / 1048576" | bc -l 2>/dev/null || echo "0")
        SIZE_TEXT=" (${SIZE_MB} MB)"
    else
        SIZE_TEXT=""
    fi
    
    echo -e "${COLOR}$arch $package_type: $status$SIZE_TEXT${NC}"
done

echo ""
echo -e "${GREEN}Total: $SUCCESS_COUNT successful out of $TOTAL_COUNT package types${NC}"
echo -e "${YELLOW}Packages created in: $OUTPUT_PATH${NC}"

if [ $SUCCESS_COUNT -gt 0 ]; then
    exit 0
else
    exit 1
fi