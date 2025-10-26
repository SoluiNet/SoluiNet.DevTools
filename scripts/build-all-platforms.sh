#!/bin/bash

# Build SoluiNet.DevTools.Console for all supported platforms and architectures
# This script builds the console application for Windows, Linux, and macOS on both x64 and ARM64 architectures

set -e  # Exit on any error

# Default parameters
CONFIGURATION="Release"
OUTPUT_PATH="./build/multi-platform"
SELF_CONTAINED=false
SINGLE_FILE=true

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
        --self-contained)
            SELF_CONTAINED=true
            shift
            ;;
        --no-single-file)
            SINGLE_FILE=false
            shift
            ;;
        -h|--help)
            echo "Usage: $0 [OPTIONS]"
            echo "Options:"
            echo "  -c, --configuration    Build configuration (Debug or Release). Default: Release"
            echo "  -o, --output          Output directory for build artifacts. Default: ./build/multi-platform"
            echo "  --self-contained      Create self-contained deployments"
            echo "  --no-single-file      Disable single file publishing"
            echo "  -h, --help           Show this help message"
            exit 0
            ;;
        *)
            echo "Unknown option: $1"
            exit 1
            ;;
    esac
done

# Define supported runtime identifiers
RUNTIME_IDENTIFIERS=(
    "win-x64"
    "win-arm64"
    "linux-x64"
    "linux-arm64"
    "osx-x64"
    "osx-arm64"
)

# Project path
PROJECT_PATH="SoluiNet.DevTools.Console/SoluiNet.DevTools.Console.csproj"

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
CYAN='\033[0;36m'
GRAY='\033[0;37m'
NC='\033[0m' # No Color

echo -e "${GREEN}Starting multi-platform build for SoluiNet.DevTools.Console${NC}"
echo -e "${YELLOW}Configuration: $CONFIGURATION${NC}"
echo -e "${YELLOW}Output Path: $OUTPUT_PATH${NC}"
echo -e "${YELLOW}Self-Contained: $SELF_CONTAINED${NC}"
echo -e "${YELLOW}Single File: $SINGLE_FILE${NC}"

# Ensure output directory exists
mkdir -p "$OUTPUT_PATH"

# Clean previous builds
echo -e "${YELLOW}Cleaning previous builds...${NC}"
dotnet clean "$PROJECT_PATH" --configuration "$CONFIGURATION" --verbosity minimal

# Restore dependencies
echo -e "${YELLOW}Restoring dependencies...${NC}"
dotnet restore "$PROJECT_PATH" --verbosity minimal

# Track build results
declare -a BUILD_RESULTS
SUCCESS_COUNT=0
FAILURE_COUNT=0

for rid in "${RUNTIME_IDENTIFIERS[@]}"; do
    echo -e "${CYAN}Building for $rid...${NC}"
    
    PLATFORM_OUTPUT_PATH="$OUTPUT_PATH/$rid"
    
    # Build arguments
    BUILD_ARGS=(
        "publish"
        "$PROJECT_PATH"
        "--configuration" "$CONFIGURATION"
        "--runtime" "$rid"
        "--output" "$PLATFORM_OUTPUT_PATH"
        "--verbosity" "minimal"
    )
    
    if [ "$SELF_CONTAINED" = true ]; then
        BUILD_ARGS+=("--self-contained")
    else
        BUILD_ARGS+=("--no-self-contained")
    fi
    
    if [ "$SINGLE_FILE" = true ]; then
        BUILD_ARGS+=("-p:PublishSingleFile=true")
    fi
    
    # Additional platform-specific optimizations
    BUILD_ARGS+=("-p:PublishTrimmed=false")  # Disable trimming to avoid plugin loading issues
    BUILD_ARGS+=("-p:PublishReadyToRun=true")  # Enable ReadyToRun for better startup performance
    
    if dotnet "${BUILD_ARGS[@]}"; then
        echo -e "${GREEN}✓ Successfully built for $rid${NC}"
        
        # Verify output files exist
        if [[ $rid == win* ]]; then
            EXPECTED_EXECUTABLE="sndt.exe"
        else
            EXPECTED_EXECUTABLE="sndt"
        fi
        
        EXECUTABLE_PATH="$PLATFORM_OUTPUT_PATH/$EXPECTED_EXECUTABLE"
        
        if [ -f "$EXECUTABLE_PATH" ]; then
            FILE_SIZE=$(stat -f%z "$EXECUTABLE_PATH" 2>/dev/null || stat -c%s "$EXECUTABLE_PATH" 2>/dev/null || echo "0")
            SIZE_MB=$(echo "scale=2; $FILE_SIZE / 1048576" | bc -l 2>/dev/null || echo "0")
            echo -e "${GRAY}  Executable: $EXPECTED_EXECUTABLE (${SIZE_MB} MB)${NC}"
            BUILD_RESULTS+=("$rid:Success:$EXECUTABLE_PATH:$FILE_SIZE")
            ((SUCCESS_COUNT++))
        else
            echo -e "${YELLOW}  Warning: Expected executable not found at $EXECUTABLE_PATH${NC}"
            BUILD_RESULTS+=("$rid:Warning::0")
            ((FAILURE_COUNT++))
        fi
    else
        echo -e "${RED}✗ Failed to build for $rid${NC}"
        BUILD_RESULTS+=("$rid:Failed::0")
        ((FAILURE_COUNT++))
    fi
    
    echo ""
done

# Summary
echo -e "${GREEN}Build Summary:${NC}"
echo -e "${GREEN}=============${NC}"

for result in "${BUILD_RESULTS[@]}"; do
    IFS=':' read -r platform status path size <<< "$result"
    
    case $status in
        "Success")
            COLOR=$GREEN
            ;;
        "Warning")
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
    
    echo -e "${COLOR}$platform: $status$SIZE_TEXT${NC}"
done

echo ""
if [ $FAILURE_COUNT -eq 0 ]; then
    echo -e "${GREEN}Total: $SUCCESS_COUNT successful, $FAILURE_COUNT failed${NC}"
    echo -e "${GREEN}All builds completed successfully!${NC}"
    echo -e "${YELLOW}Build artifacts are available in: $OUTPUT_PATH${NC}"
    exit 0
else
    echo -e "${YELLOW}Total: $SUCCESS_COUNT successful, $FAILURE_COUNT failed${NC}"
    echo -e "${RED}Some builds failed. Check the output above for details.${NC}"
    exit 1
fi