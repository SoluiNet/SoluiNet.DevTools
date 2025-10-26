#!/bin/bash

# Create Linux-specific packages for SoluiNet.DevTools.Console
# This script creates DEB, RPM, and AppImage packages

set -e

# Default parameters
BUILD_PATH="./build/linux"
OUTPUT_PATH="./packages/linux"
VERSION="1.0.0"
MAINTAINER="SoluiNet <info@soluinet.net>"

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
        -m|--maintainer)
            MAINTAINER="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  -b, --build-path      Path to built binaries. Default: ./build/linux"
            echo "  -o, --output-path     Output directory for packages. Default: ./packages/linux"
            echo "  -v, --version         Version string. Default: 1.0.0"
            echo "  -m, --maintainer      Maintainer info. Default: SoluiNet <info@soluinet.net>"
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

echo -e "${GREEN}Creating Linux packages for SoluiNet.DevTools.Console${NC}"
echo -e "${YELLOW}Build Path: $BUILD_PATH${NC}"
echo -e "${YELLOW}Output Path: $OUTPUT_PATH${NC}"
echo -e "${YELLOW}Version: $VERSION${NC}"
echo -e "${YELLOW}Maintainer: $MAINTAINER${NC}"

# Ensure output directory exists
mkdir -p "$OUTPUT_PATH"

# Track package results
declare -a PACKAGE_RESULTS
SUCCESS_COUNT=0
TOTAL_COUNT=0

# Create TAR.GZ packages for each architecture
for arch in "linux-x64" "linux-arm64"; do
    ARCH_BUILD_PATH="$BUILD_PATH/$arch"
    
    if [ -d "$ARCH_BUILD_PATH" ]; then
        echo -e "${CYAN}Creating TAR.GZ package for $arch...${NC}"
        
        TAR_PATH="$OUTPUT_PATH/soluinet-devtools-console_${arch}_v${VERSION}.tar.gz"
        
        if tar -czf "$TAR_PATH" -C "$ARCH_BUILD_PATH" .; then
            FILE_SIZE=$(stat -c%s "$TAR_PATH" 2>/dev/null || stat -f%z "$TAR_PATH" 2>/dev/null)
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

# Create DEB package (if dpkg-deb is available)
if command -v dpkg-deb &> /dev/null; then
    echo -e "${CYAN}Creating DEB package...${NC}"
    
    DEB_DIR="$OUTPUT_PATH/deb"
    mkdir -p "$DEB_DIR/DEBIAN"
    mkdir -p "$DEB_DIR/usr/local/bin"
    mkdir -p "$DEB_DIR/usr/share/applications"
    mkdir -p "$DEB_DIR/usr/share/doc/soluinet-devtools-console"
    
    # Copy x64 binary as default
    if [ -d "$BUILD_PATH/linux-x64" ]; then
        cp -r "$BUILD_PATH/linux-x64"/* "$DEB_DIR/usr/local/bin/"
        chmod +x "$DEB_DIR/usr/local/bin/sndt"
        
        # Create control file
        cat > "$DEB_DIR/DEBIAN/control" << EOF
Package: soluinet-devtools-console
Version: $VERSION
Section: devel
Priority: optional
Architecture: amd64
Maintainer: $MAINTAINER
Description: SoluiNet DevTools Console
 Cross-platform development tools console application that provides
 various development tools and utilities with plugin-based architecture.
Homepage: https://github.com/SoluiNet/SoluiNet.DevTools
EOF
        
        # Create desktop entry
        cat > "$DEB_DIR/usr/share/applications/soluinet-devtools-console.desktop" << EOF
[Desktop Entry]
Name=SoluiNet DevTools Console
Comment=Development tools console application
Exec=/usr/local/bin/sndt
Icon=utilities-terminal
Terminal=true
Type=Application
Categories=Development;
EOF
        
        # Create copyright file
        cat > "$DEB_DIR/usr/share/doc/soluinet-devtools-console/copyright" << EOF
Format: https://www.debian.org/doc/packaging-manuals/copyright-format/1.0/
Upstream-Name: SoluiNet DevTools Console
Source: https://github.com/SoluiNet/SoluiNet.DevTools

Files: *
Copyright: 2018-2024 SoluiNet
License: MIT
EOF
        
        # Build DEB package
        DEB_PATH="$OUTPUT_PATH/soluinet-devtools-console_${VERSION}_amd64.deb"
        if dpkg-deb --build "$DEB_DIR" "$DEB_PATH"; then
            FILE_SIZE=$(stat -c%s "$DEB_PATH" 2>/dev/null || stat -f%z "$DEB_PATH" 2>/dev/null)
            SIZE_MB=$(echo "scale=2; $FILE_SIZE / 1048576" | bc -l 2>/dev/null || echo "0")
            echo -e "${GREEN}✓ Created DEB package: $(basename "$DEB_PATH") (${SIZE_MB} MB)${NC}"
            
            PACKAGE_RESULTS+=("amd64:DEB:Success:$DEB_PATH:$FILE_SIZE")
            ((SUCCESS_COUNT++))
        else
            echo -e "${RED}✗ Failed to create DEB package${NC}"
            PACKAGE_RESULTS+=("amd64:DEB:Failed::0")
        fi
        ((TOTAL_COUNT++))
        
        # Clean up
        rm -rf "$DEB_DIR"
    else
        echo -e "${YELLOW}! No linux-x64 build found for DEB package${NC}"
        PACKAGE_RESULTS+=("amd64:DEB:Build Not Found::0")
        ((TOTAL_COUNT++))
    fi
else
    echo -e "${YELLOW}! dpkg-deb not found, skipping DEB package creation${NC}"
    PACKAGE_RESULTS+=("amd64:DEB:Tool Not Available::0")
    ((TOTAL_COUNT++))
fi

# Create RPM package (if rpmbuild is available)
if command -v rpmbuild &> /dev/null; then
    echo -e "${CYAN}Creating RPM package...${NC}"
    
    RPM_BUILD_DIR="$OUTPUT_PATH/rpmbuild"
    mkdir -p "$RPM_BUILD_DIR"/{BUILD,RPMS,SOURCES,SPECS,SRPMS}
    
    if [ -d "$BUILD_PATH/linux-x64" ]; then
        # Create source tarball
        SOURCE_TAR="$RPM_BUILD_DIR/SOURCES/soluinet-devtools-console-$VERSION.tar.gz"
        tar -czf "$SOURCE_TAR" -C "$BUILD_PATH/linux-x64" .
        
        # Create RPM spec file
        cat > "$RPM_BUILD_DIR/SPECS/soluinet-devtools-console.spec" << EOF
Name:           soluinet-devtools-console
Version:        $VERSION
Release:        1%{?dist}
Summary:        SoluiNet DevTools Console
License:        MIT
URL:            https://github.com/SoluiNet/SoluiNet.DevTools
Source0:        %{name}-%{version}.tar.gz
BuildArch:      x86_64

%description
Cross-platform development tools console application that provides
various development tools and utilities with plugin-based architecture.

%prep
%setup -q -c

%install
mkdir -p %{buildroot}/usr/local/bin
cp -r * %{buildroot}/usr/local/bin/
chmod +x %{buildroot}/usr/local/bin/sndt

%files
/usr/local/bin/*

%changelog
* $(date +'%a %b %d %Y') SoluiNet <info@soluinet.net> - $VERSION-1
- Multi-platform release with .NET 8.0 support
EOF
        
        # Build RPM
        if rpmbuild --define "_topdir $RPM_BUILD_DIR" -ba "$RPM_BUILD_DIR/SPECS/soluinet-devtools-console.spec"; then
            RPM_PATH=$(find "$RPM_BUILD_DIR/RPMS" -name "*.rpm" | head -1)
            if [ -n "$RPM_PATH" ]; then
                FINAL_RPM_PATH="$OUTPUT_PATH/$(basename "$RPM_PATH")"
                cp "$RPM_PATH" "$FINAL_RPM_PATH"
                
                FILE_SIZE=$(stat -c%s "$FINAL_RPM_PATH" 2>/dev/null || stat -f%z "$FINAL_RPM_PATH" 2>/dev/null)
                SIZE_MB=$(echo "scale=2; $FILE_SIZE / 1048576" | bc -l 2>/dev/null || echo "0")
                echo -e "${GREEN}✓ Created RPM package: $(basename "$FINAL_RPM_PATH") (${SIZE_MB} MB)${NC}"
                
                PACKAGE_RESULTS+=("x86_64:RPM:Success:$FINAL_RPM_PATH:$FILE_SIZE")
                ((SUCCESS_COUNT++))
            else
                echo -e "${RED}✗ RPM build succeeded but no RPM file found${NC}"
                PACKAGE_RESULTS+=("x86_64:RPM:Failed::0")
            fi
        else
            echo -e "${RED}✗ Failed to create RPM package${NC}"
            PACKAGE_RESULTS+=("x86_64:RPM:Failed::0")
        fi
        ((TOTAL_COUNT++))
        
        # Clean up
        rm -rf "$RPM_BUILD_DIR"
    else
        echo -e "${YELLOW}! No linux-x64 build found for RPM package${NC}"
        PACKAGE_RESULTS+=("x86_64:RPM:Build Not Found::0")
        ((TOTAL_COUNT++))
    fi
else
    echo -e "${YELLOW}! rpmbuild not found, skipping RPM package creation${NC}"
    PACKAGE_RESULTS+=("x86_64:RPM:Tool Not Available::0")
    ((TOTAL_COUNT++))
fi

# Create Snap package specification
echo -e "${CYAN}Creating Snap package specification...${NC}"

SNAP_DIR="$OUTPUT_PATH/snap"
mkdir -p "$SNAP_DIR"

cat > "$SNAP_DIR/snapcraft.yaml" << EOF
name: soluinet-devtools-console
base: core22
version: '$VERSION'
summary: SoluiNet DevTools Console
description: |
  Cross-platform development tools console application that provides
  various development tools and utilities with plugin-based architecture.

grade: stable
confinement: strict

parts:
  soluinet-devtools-console:
    plugin: dump
    source: ../linux/linux-x64/
    organize:
      '*': bin/

apps:
  soluinet-devtools-console:
    command: bin/sndt
    plugs: [home, network, network-bind]
EOF

echo -e "${GREEN}✓ Created Snap package specification${NC}"
PACKAGE_RESULTS+=("All:Snap Spec:Success:$SNAP_DIR:0")
((SUCCESS_COUNT++))
((TOTAL_COUNT++))

# Summary
echo ""
echo -e "${GREEN}Linux Packaging Summary:${NC}"
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