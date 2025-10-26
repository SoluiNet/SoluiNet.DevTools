#!/bin/bash

# Build SoluiNet.DevTools.Console for Linux platforms (x64 and ARM64)

set -e

# Default parameters
CONFIGURATION="Release"
OUTPUT_PATH="./build/linux"
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
            echo "  -o, --output          Output directory. Default: ./build/linux"
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
        RUNTIME_IDENTIFIERS=("linux-x64")
        ;;
    "arm64")
        RUNTIME_IDENTIFIERS=("linux-arm64")
        ;;
    "both")
        RUNTIME_IDENTIFIERS=("linux-x64" "linux-arm64")
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

echo -e "${GREEN}Building SoluiNet.DevTools.Console for Linux${NC}"
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
    else
        echo -e "${RED}✗ Failed to build for $rid${NC}"
        ALL_SUCCESSFUL=false
    fi
done

if [ "$ALL_SUCCESSFUL" = true ]; then
    echo -e "${GREEN}Linux build completed successfully!${NC}"
    exit 0
else
    echo -e "${RED}Some Linux builds failed.${NC}"
    exit 1
fi