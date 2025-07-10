#!/bin/bash

# Template Renaming Script
# Usage: ./rename-template.sh NewProjectName

if [ $# -eq 0 ]; then
    echo "Usage: $0 <NewProjectName>"
    echo "Example: $0 MyAwesomeApp"
    exit 1
fi

OLD_NAME="Template"
NEW_NAME="$1"

echo "Renaming template from $OLD_NAME to $NEW_NAME..."

# Go to project root
cd "$(dirname "$0")/.."

# Rename solution file
if [ -f "$OLD_NAME.sln" ]; then
    mv "$OLD_NAME.sln" "$NEW_NAME.sln"
    echo "✓ Renamed solution file"
fi

# Rename project directories
for dir in src/$OLD_NAME.*; do
    if [ -d "$dir" ]; then
        newdir="${dir//$OLD_NAME/$NEW_NAME}"
        mv "$dir" "$newdir"
        echo "✓ Renamed directory: $dir → $newdir"
    fi
done

# Rename project files
find src -name "$OLD_NAME.*.csproj" -type f | while read file; do
    newfile="${file//$OLD_NAME/$NEW_NAME}"
    mv "$file" "$newfile"
    echo "✓ Renamed project file: $file → $newfile"
done

# Replace in all text files
echo "Updating file contents..."
find . -type f \( -name "*.cs" -o -name "*.csproj" -o -name "*.sln" -o -name "*.json" -o -name "*.md" \) \
    -not -path "./.git/*" -not -path "./bin/*" -not -path "./obj/*" | while read file; do
    if grep -q "$OLD_NAME" "$file"; then
        sed -i '' "s/$OLD_NAME/$NEW_NAME/g" "$file"
        echo "✓ Updated: $file"
    fi
done

echo ""
echo "✅ Template renamed successfully!"
echo ""
echo "Next steps:"
echo "1. Review the changes"
echo "2. Build the solution: dotnet build"
echo "3. Update configuration in local.settings.json"
echo "4. Implement your MCP tools in src/$NEW_NAME.Functions/MCP/McpServer.cs"
echo "5. Run as MCP server: ./scripts/run-mcp-server.sh"