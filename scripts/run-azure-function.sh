#!/bin/bash

# Template Azure Function Runner Script

# Navigate to the project directory
cd "$(dirname "$0")/../src/Template.Functions"

# Check if .NET is installed
if ! command -v dotnet &> /dev/null; then
    echo "Error: .NET SDK is not installed. Please install .NET 8.0 SDK."
    echo "Visit: https://dotnet.microsoft.com/download/dotnet/8.0"
    exit 1
fi

# Check if Azure Functions Core Tools is installed
if ! command -v func &> /dev/null; then
    echo "Error: Azure Functions Core Tools is not installed."
    echo "Install with: npm install -g azure-functions-core-tools@4"
    exit 1
fi

# Restore dependencies if needed
if [ ! -d "obj" ]; then
    echo "Restoring NuGet packages..."
    dotnet restore
fi

# Build the project
echo "Building Azure Function..."
dotnet build --configuration Debug

if [ $? -ne 0 ]; then
    echo "Build failed. Please check the error messages above."
    exit 1
fi

# Start the Azure Function locally
echo "Starting Azure Function locally..."
echo "============================================"
echo "Azure Function Host is starting"
echo "Default URL: http://localhost:7071/api/health"
echo "Use Ctrl+C to stop the function"
echo "============================================"

func start