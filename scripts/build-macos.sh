#!/bin/bash

# Build SoluiNet.DevTools.Console for macOS platforms (x64 and ARM64)

set -e

# Default parameters
CONFIGURATION="Release"
OUTPUT_PATH="./build/macos"
ARCHITECTURE="both"

# Parse command line arguments
while [[ $# -gt 0 ]]; do
    case $1 in
        -c|--configuration)
            CONFIGURATION="$2"
            shift 2
            ;;
        -o|--output)
            OUTPUT_PATH="$2"
            shift 2
            ;;
        -a|--architecture)
            ARCHITECTURE="$2"
            shift 2
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  -c, --configuration    Build configuration (Debug or Release). Default: Release"
            echo "  -o, --output          Output directory. Default: ./build/macos"
            echo "  -a, --architecture    Target architecture (x64, arm64, or both). Default: both"
            echo "  -h, --help           Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

PROJECT_PATH="SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj"

# Determine runtime identifiers based on architecture parameter
case $ARCHITECTURE in
    "x64")
        RUNTIME_IDENTIFIERS=("osx-x64")
        ;;
    "arm64")
        RUNTIME_IDENTIFIERS=("osx-arm64")
        ;;
    "both")
        RUNTIME_IDENTIFIERS=("osx-x64" "osx-arm64")
        ;;
    *)
        echo "Invalid architecture: $ARCHITECTURE"
        exit 1
        ;;
esac

# Colors
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
RED='\033[0;31m'
GRAY='\033[0;37m'
NC='\033[0m'

echo -e "${GREEN}Building SoluiNet.DevTools.Console for macOS${NC}"
echo -e "${YELLOW}Configuration: $CONFIGURATION${NC}"
echo -e "${YELLOW}Architecture(s): ${RUNTIME_IDENTIFIERS[*]}${NC}"
echo -e "${YELLOW}Output Path: $OUTPUT_PATH${NC}"

# Ensure output directory exists
mkdir -p "$OUTPUT_PATH"

# Clean and restore
echo -e "${YELLOW}Cleaning and restoring...${NC}"
dotnet clean "$PROJECT_PATH" --configuration "$CONFIGURATION" --verbosity minimal
dotnet restore "$PROJECT_PATH" --verbosity minimal

ALL_SUCCESSFUL=true

for rid in "${RUNTIME_IDENTIFIERS[@]}"; do
    echo -e "${CYAN}Building for $rid...${NC}"
    
    PLATFORM_OUTPUT_PATH="$OUTPUT_PATH/$rid"
    
    BUILD_ARGS=(
        "publish"
        "$PROJECT_PATH"
        "--configuration" "$CONFIGURATION"
        "--runtime" "$rid"
        "--output" "$PLATFORM_OUTPUT_PATH"
        "--no-self-contained"
        "-p:PublishSingleFile=true"
        "-p:PublishReadyToRun=true"
        "--verbosity" "minimal"
    )
    
    if dotnet "${BUILD_ARGS[@]}"; then
        echo -e "${GREEN}✓ Successfully built for $rid${NC}"
        
        # Make executable
        chmod +x "$PLATFORM_OUTPUT_PATH/sndt"
        
        # Copy plugins if they exist
        PLUGINS_SOURCE="build/plugins/$CONFIGURATION/Plugins"
        PLUGINS_TARGET="$PLATFORM_OUTPUT_PATH/Plugins"
        
        if [ -d "$PLUGINS_SOURCE" ]; then
            echo -e "${GRAY}  Copying plugins...${NC}"
            cp -r "$PLUGINS_SOURCE" "$PLUGINS_TARGET"
        fi
        
        # Create macOS app bundle structure (optional)
        if command -v plutil &> /dev/null; then
            echo -e "${GRAY}  Creating macOS app bundle structure...${NC}"
            APP_BUNDLE_PATH="$PLATFORM_OUTPUT_PATH/SoluiNet.DevTools.Console.app"
            mkdir -p "$APP_BUNDLE_PATH/Contents/MacOS"
            mkdir -p "$APP_BUNDLE_PATH/Contents/Resources"
            
            # Move executable to app bundle
            mv "$PLATFORM_OUTPUT_PATH/sndt" "$APP_BUNDLE_PATH/Contents/MacOS/"
            
            # Create Info.plist
            cat > "$APP_BUNDLE_PATH/Contents/Info.plist" << EOF
<?xml version="1.0" encoding="UTF-8"?>
<!DOCTYPE plist PUBLIC "-//Apple//DTD PLIST 1.0//EN" "http://www.apple.com/DTDs/PropertyList-1.0.dtd">
<plist version="1.0">
<dict>
    <key>CFBundleExecutable</key>
    <string>sndt</string>
    <key>CFBundleIdentifier</key>
    <string>net.soluinet.devtools.console</string>
    <key>CFBundleName</key>
    <string>SoluiNet DevTools Console</string>
    <key>CFBundleVersion</key>
    <string>1.0</string>
    <key>CFBundleShortVersionString</key>
    <string>1.0</string>
    <key>CFBundlePackageType</key>
    <string>APPL</string>
    <key>LSMinimumSystemVersion</key>
    <string>10.15</string>
</dict>
</plist>
EOF
            
            # Copy other files to Resources
            find "$PLATFORM_OUTPUT_PATH" -maxdepth 1 -type f ! -name "*.app" -exec cp {} "$APP_BUNDLE_PATH/Contents/Resources/" \;
            
            # Copy plugins to Resources if they exist
            if [ -d "$PLUGINS_TARGET" ]; then
                cp -r "$PLUGINS_TARGET" "$APP_BUNDLE_PATH/Contents/Resources/"
            fi
        fi
    else
        echo -e "${RED}✗ Failed to build for $rid${NC}"
        ALL_SUCCESSFUL=false
    fi
done

if [ "$ALL_SUCCESSFUL" = true ]; then
    echo -e "${GREEN}macOS build completed successfully!${NC}"
    exit 0
else
    echo -e "${RED}Some macOS builds failed.${NC}"
    exit 1
fi