# Azure Function MCP Server Template

A standardized template for building Azure Functions with MCP (Model Context Protocol) server capabilities in .NET 8.

## Features
- **Azure OpenAI integration**: Built-in adapters for GPT-4 and GPT-3.5 models
- **Dual-mode operation**: Run as Azure Function or standalone MCP server
- **Clean architecture**: Separated Core, Services, Functions, and Tests projects
- **MCP Protocol implementation**: Complete JSON-RPC based MCP server
- **HTTP resilience**: Built-in retry and circuit breaker policies
- **Comprehensive testing**: Unit and integration test infrastructure
- **Configuration management**: Environment-based configuration with Options pattern
- **Logging**: Structured logging with console and Application Insights support

## Project Structure

```
AzureFunctionMCPTemplate/
├── src/
│   ├── Template.Core/           # Domain models and interfaces
│   ├── Template.Functions/      # Azure Functions and MCP server
│   ├── Template.Services/       # Business logic and external clients
│   └── Template.Tests/          # Unit and integration tests
├── scripts/
│   ├── run-mcp-server.sh       # Run as MCP server
│   ├── run-azure-function.sh   # Run as Azure Function
│   └── test-mcp-server.sh      # Test MCP protocol
├── docs/                        # Additional documentation
└── Template.sln                 # Solution file
```

## Prerequisites

- [.NET 8.0 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Azure Functions Core Tools v4](https://docs.microsoft.com/azure/azure-functions/functions-run-local)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)

## 🚀 Converting Template to Your MCP Application

Follow these steps to transform this template into your own MCP application:

### Step 1: Clone and Rename

```bash
# Clone the template
cp -r ~/DevProjects/AzureFunctionMCPTemplate ~/DevProjects/YourAppMCP

# Navigate to your new project
cd ~/DevProjects/YourAppMCP

# Rename solution file
mv Template.sln YourApp.sln
```

### Step 2: Update Project Names

1. **Rename project directories and files:**
```bash
# Rename directories
mv src/Template.Core src/YourApp.Core
mv src/Template.Functions src/YourApp.Functions
mv src/Template.Services src/YourApp.Services
mv src/Template.Tests src/YourApp.Tests

# Rename project files
mv src/YourApp.Core/Template.Core.csproj src/YourApp.Core/YourApp.Core.csproj
mv src/YourApp.Functions/Template.Functions.csproj src/YourApp.Functions/YourApp.Functions.csproj
mv src/YourApp.Services/Template.Services.csproj src/YourApp.Services/YourApp.Services.csproj
mv src/YourApp.Tests/Template.Tests.csproj src/YourApp.Tests/YourApp.Tests.csproj
```

2. **Update namespaces in all C# files:**
```bash
# On macOS/Linux
find . -name "*.cs" -type f -exec sed -i '' 's/Template\.Core/YourApp.Core/g' {} +
find . -name "*.cs" -type f -exec sed -i '' 's/Template\.Functions/YourApp.Functions/g' {} +
find . -name "*.cs" -type f -exec sed -i '' 's/Template\.Services/YourApp.Services/g' {} +
find . -name "*.cs" -type f -exec sed -i '' 's/Template\.Tests/YourApp.Tests/g' {} +

# Update project references
find . -name "*.csproj" -type f -exec sed -i '' 's/Template\./YourApp./g' {} +
```

3. **Update solution file:**
```bash
sed -i '' 's/Template/YourApp/g' YourApp.sln
```

### Step 3: Define Your Domain

1. **Create your domain models** in `src/YourApp.Core/Models/`:
```csharp
// src/YourApp.Core/Models/YourDomainModels.cs
namespace YourApp.Core.Models;

public class Customer
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Email { get; set; }
    // Add your properties
}
```

2. **Define your service interfaces** in `src/YourApp.Core/Interfaces/`:
```csharp
// src/YourApp.Core/Interfaces/ICustomerService.cs
namespace YourApp.Core.Interfaces;

public interface ICustomerService
{
    Task<Customer> GetCustomerAsync(string id);
    Task<string> CreateCustomerAsync(Customer customer);
    Task<bool> UpdateCustomerAsync(string id, Customer customer);
    Task<bool> DeleteCustomerAsync(string id);
}
```

### Step 4: Implement Your MCP Tools

1. **Update the MCP server** in `src/YourApp.Functions/MCP/McpServer.cs`:
```csharp
private readonly List<Tool> _tools = new()
{
    new Tool
    {
        Name = "get_customer",
        Description = "Get customer information by ID",
        InputSchema = new
        {
            type = "object",
            properties = new
            {
                customerId = new { type = "string", description = "The customer ID" }
            },
            required = new[] { "customerId" }
        }
    },
    new Tool
    {
        Name = "create_customer",
        Description = "Create a new customer",
        InputSchema = new
        {
            type = "object",
            properties = new
            {
                name = new { type = "string", description = "Customer name" },
                email = new { type = "string", description = "Customer email" }
            },
            required = new[] { "name", "email" }
        }
    }
    // Add more tools as needed
};
```

2. **Implement tool handlers:**
```csharp
[JsonRpcMethod("tools/call")]
public async Task<object> CallToolAsync(ToolCallParams toolParams)
{
    var result = toolParams.Name switch
    {
        "get_customer" => await HandleGetCustomerAsync(toolParams.Arguments),
        "create_customer" => await HandleCreateCustomerAsync(toolParams.Arguments),
        // Add more tool handlers
        _ => throw new Exception($"Unknown tool: {toolParams.Name}")
    };
    
    // Return result...
}

private async Task<object> HandleGetCustomerAsync(Dictionary<string, object>? arguments)
{
    var customerId = arguments?["customerId"]?.ToString();
    var customer = await _customerService.GetCustomerAsync(customerId);
    return new { success = true, customer };
}
```

### Step 5: Implement Your Services

1. **Create service implementations** in `src/YourApp.Services/Services/`:
```csharp
// src/YourApp.Services/Services/CustomerService.cs
using YourApp.Core.Interfaces;
using YourApp.Core.Models;

namespace YourApp.Services.Services;

public class CustomerService : ICustomerService
{
    private readonly ILogger<CustomerService> _logger;
    private readonly IApiClient _apiClient;

    public CustomerService(ILogger<CustomerService> logger, IApiClient apiClient)
    {
        _logger = logger;
        _apiClient = apiClient;
    }

    public async Task<Customer> GetCustomerAsync(string id)
    {
        // Implement your business logic
        return await _apiClient.GetAsync<Customer>($"/customers/{id}");
    }
    
    // Implement other methods...
}
```

### Step 6: Configure Your Application

1. **Update configuration** in `src/YourApp.Functions/Configuration/AppConfiguration.cs`:
```csharp
public class YourAppConfiguration
{
    public string ApiUrl { get; set; } = "https://your-api.com";
    public string ApiKey { get; set; } = string.Empty;
    public string DatabaseConnection { get; set; } = string.Empty;
    // Add your configuration properties
}
```

2. **Update local settings** in `src/YourApp.Functions/local.settings.json`:
```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    "YourApp__ApiUrl": "https://your-api.com",
    "YourApp__ApiKey": "your-api-key",
    "YourApp__DatabaseConnection": "your-connection-string"
  }
}
```

### Step 7: Register Dependencies

Update `Program.cs` to register your services:
```csharp
private static void ConfigureServices(HostBuilderContext context, IServiceCollection services)
{
    // Register your configuration
    services.Configure<YourAppConfiguration>(context.Configuration.GetSection("YourApp"));

    // Register your services
    services.AddScoped<ICustomerService, CustomerService>();
    services.AddScoped<IOrderService, OrderService>();
    // Add more service registrations

    // Register MCP server with your services
    services.AddScoped<McpServer>();
    
    // Existing registrations...
}
```

### Step 8: Add Azure Functions (Optional)

Create HTTP-triggered functions in `src/YourApp.Functions/Functions/`:
```csharp
public class CustomerFunction
{
    private readonly ICustomerService _customerService;

    public CustomerFunction(ICustomerService customerService)
    {
        _customerService = customerService;
    }

    [Function("GetCustomer")]
    public async Task<HttpResponseData> GetCustomer(
        [HttpTrigger(AuthorizationLevel.Function, "get", Route = "customers/{id}")] 
        HttpRequestData req,
        string id)
    {
        var customer = await _customerService.GetCustomerAsync(id);
        var response = req.CreateResponse(HttpStatusCode.OK);
        await response.WriteAsJsonAsync(customer);
        return response;
    }
}
```

### Step 9: Test Your MCP Application

1. **Build the solution:**
```bash
dotnet build
```

2. **Run as MCP server:**
```bash
./scripts/run-mcp-server.sh
```

3. **Test with sample requests:**
```bash
# Initialize
echo '{"jsonrpc":"2.0","id":1,"method":"initialize","params":{"protocolVersion":"0.1.0"}}' | \
  dotnet run --project src/YourApp.Functions -- --mcp

# List tools
echo '{"jsonrpc":"2.0","id":2,"method":"tools/list"}' | \
  dotnet run --project src/YourApp.Functions -- --mcp

# Call a tool
echo '{"jsonrpc":"2.0","id":3,"method":"tools/call","params":{"name":"get_customer","arguments":{"customerId":"123"}}}' | \
  dotnet run --project src/YourApp.Functions -- --mcp
```

### Step 10: Configure MCP Client

Add to Claude Desktop's config (`~/Library/Application Support/Claude/claude_desktop_config.json`):
```json
{
  "mcpServers": {
    "yourapp-server": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/YourApp.Functions", "--", "--mcp"],
      "env": {
        "YourApp__ApiUrl": "https://your-api.com",
        "YourApp__ApiKey": "your-api-key"
      }
    }
  }
}
```

## Quick Start Guide

### From Template to Working App in 10 Minutes

1. **Copy template and rename:**
```bash
cp -r AzureFunctionMCPTemplate MyAwesomeMCP
cd MyAwesomeMCP
```

2. **Run the renaming script** (create this helper):
```bash
#!/bin/bash
OLD_NAME="Template"
NEW_NAME="$1"

# Rename files and directories
find . -depth -name "*${OLD_NAME}*" | while read file; do
    mv "$file" "${file//${OLD_NAME}/${NEW_NAME}}"
done

# Replace in files
find . -type f \( -name "*.cs" -o -name "*.csproj" -o -name "*.sln" -o -name "*.json" \) \
    -exec sed -i '' "s/${OLD_NAME}/${NEW_NAME}/g" {} +
```

3. **Define your first tool** in McpServer.cs
4. **Implement the tool handler**
5. **Run and test:**
```bash
./scripts/run-mcp-server.sh
```

## Development Guide

### Adding New MCP Tools

1. Define the tool in `McpServer.cs`:

```csharp
new Tool
{
    Name = "your_tool",
    Description = "Tool description",
    InputSchema = new
    {
        type = "object",
        properties = new
        {
            param1 = new { type = "string", description = "Parameter description" }
        },
        required = new[] { "param1" }
    }
}
```

2. Add the handler method:

```csharp
private async Task<object> HandleYourToolAsync(Dictionary<string, object>? arguments)
{
    // Tool implementation
}
```

3. Register in the switch statement:

```csharp
"your_tool" => await HandleYourToolAsync(toolParams.Arguments),
```

### Adding New Azure Functions

Create a new class in `src/Template.Functions/Functions/`:

```csharp
public class YourFunction
{
    private readonly ILogger<YourFunction> _logger;
    private readonly IYourService _service;

    public YourFunction(ILogger<YourFunction> logger, IYourService service)
    {
        _logger = logger;
        _service = service;
    }

    [Function("YourFunction")]
    public async Task<HttpResponseData> Run(
        [HttpTrigger(AuthorizationLevel.Function, "get", "post", Route = "your-route")] 
        HttpRequestData req)
    {
        // Function implementation
    }
}
```

### Adding Services

1. Define interface in `Template.Core/Interfaces/`:

```csharp
public interface IYourService
{
    Task<YourResult> DoSomethingAsync(YourRequest request);
}
```

2. Implement in `Template.Services/Services/`:

```csharp
public class YourService : IYourService
{
    // Implementation
}
```

3. Register in `Program.cs`:

```csharp
services.AddScoped<IYourService, YourService>();
```

## Testing

### Run All Tests

```bash
dotnet test
```

### Run with Coverage

```bash
dotnet test --collect:"XPlat Code Coverage"
```

### Test MCP Server

```bash
./scripts/test-mcp-server.sh
```

## Deployment

### Azure Deployment

1. Create Azure Function App:

```bash
az functionapp create --resource-group myResourceGroup \
  --consumption-plan-location westus \
  --runtime dotnet-isolated \
  --runtime-version 8 \
  --functions-version 4 \
  --name myFunctionApp \
  --storage-account mystorageaccount
```

2. Deploy:

```bash
cd src/Template.Functions
func azure functionapp publish myFunctionApp
```

### Docker Deployment

Create a `Dockerfile`:

```dockerfile
FROM mcr.microsoft.com/dotnet/sdk:8.0 AS build
WORKDIR /src
COPY . .
RUN dotnet publish "src/Template.Functions/Template.Functions.csproj" -c Release -o /app/publish

FROM mcr.microsoft.com/azure-functions/dotnet-isolated:4-dotnet-isolated8.0
WORKDIR /home/site/wwwroot
COPY --from=build /app/publish .
ENV AzureWebJobsScriptRoot=/home/site/wwwroot \
    AzureFunctionsJobHost__Logging__Console__IsEnabled=true
```

## AI Model Support

This template supports multiple AI models through different adapters:

### Claude (via MCP Protocol)
- Native MCP support through claude_desktop_config.json
- Direct tool integration
- Streaming support

### Azure OpenAI (via HTTP Adapters)
- **GPT-4** models (gpt-4, gpt-4-32k, gpt-4-turbo, gpt-4o)
- **GPT-3.5** models (gpt-35-turbo, gpt-35-turbo-16k)
- Function calling support
- Vision capabilities (gpt-4-turbo, gpt-4o)

### Other AI Services
- Generic HTTP endpoints for any AI service
- OpenAPI/Swagger compatible
- Batch tool execution

For detailed Azure OpenAI integration instructions, see [docs/AZURE_OPENAI_INTEGRATION.md](docs/AZURE_OPENAI_INTEGRATION.md).

## MCP Client Configuration

### Claude Desktop

Add to `claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "template-server": {
      "command": "dotnet",
      "args": ["run", "--project", "/path/to/Template.Functions", "--", "--mcp"],
      "env": {
        "App__ApiUrl": "https://your-api.com",
        "App__ApiKey": "your-api-key"
      }
    }
  }
}
```

## Troubleshooting

### Common Issues

1. **Build errors**: Ensure .NET 8.0 SDK is installed
2. **Function runtime errors**: Check Azure Functions Core Tools version
3. **MCP connection issues**: Verify JSON-RPC format and tool names
4. **HTTP client errors**: Check API configuration and network connectivity

### Debug MCP Server

Enable detailed logging:

```csharp
services.AddLogging(builder =>
{
    builder.AddConsole();
    builder.SetMinimumLevel(LogLevel.Debug);
});
```

### Debug Azure Functions

Use Visual Studio or VS Code debugging features with `launch.json` configuration.

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests
5. Submit a pull request

## License

This template is provided as-is for use in your projects. Customize the license as needed.

## Resources

- [Azure Functions Documentation](https://docs.microsoft.com/azure/azure-functions/)
- [MCP Specification](https://modelcontextprotocol.io/)
- [.NET 8 Documentation](https://docs.microsoft.com/dotnet/core/whats-new/dotnet-8)
- [Polly Resilience](https://github.com/App-vNext/Polly)