#!/bin/bash

# MCP Server Test Script

echo "Testing MCP Server..."

# Test initialize
echo "Testing initialize method:"
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"0.1.0","capabilities":{},"clientInfo":{"name":"test-client","version":"1.0.0"}}}' | \
  dotnet run --project "$(dirname "$0")/../src/Template.Functions" -- --mcp 2>/dev/null | \
  jq .

# Test tools/list
echo -e "\nTesting tools/list method:"
echo '{"jsonrpc":"2.0","id":2,"method":"tools/list","params":{}}' | \
  dotnet run --project "$(dirname "$0")/../src/Template.Functions" -- --mcp 2>/dev/null | \
  jq .

# Test tools/call
echo -e "\nTesting tools/call method (example_get):"
echo '{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"example_get","arguments":{"id":"test-123"}}}' | \
  dotnet run --project "$(dirname "$0")/../src/Template.Functions" -- --mcp 2>/dev/null | \
  jq .

echo -e "\nMCP Server test complete!"