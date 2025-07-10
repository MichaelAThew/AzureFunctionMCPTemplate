#!/bin/bash

# Template MCP Server Runner Script

# Set environment variables if not already set
export App__ApiUrl="${App__ApiUrl:-https://api.example.com}"
export App__ApiKey="${App__ApiKey:-your-api-key}"
export App__TimeoutSeconds="${App__TimeoutSeconds:-30}"
export PATH="/usr/local/share/dotnet:$PATH"
export HOME="${HOME:-$HOME}"

# Navigate to the project directory
cd "$(dirname "$0")/../src/Template.Functions"

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed. Please install .NET 8.0 SDK."
    echo "Visit: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check .NET version
DOTNET_VERSION=$(dotnet --version)
echo "Using .NET SDK version: $DOTNET_VERSION"

# Restore dependencies if needed
if [ ! -d "obj" ]; then
    echo "Restoring NuGet packages..."
    dotnet restore
fi

# Build the project
echo "Building Template MCP Server..."
dotnet build --configuration Release

if [ $? -ne 0 ]; then
    echo "Build failed. Please check the error messages above."
    exit 1
fi

# Run as MCP server
echo "Starting Template MCP Server..."
echo "============================================"
echo "MCP Server is starting in stdio mode"
echo "Use Ctrl+C to stop the server"
echo "============================================"

dotnet run --configuration Release -- --mcp