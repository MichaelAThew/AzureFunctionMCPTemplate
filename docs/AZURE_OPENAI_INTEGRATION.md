# Azure OpenAI Integration Guide

This guide explains how to use the Azure Function MCP Server template with Azure OpenAI models (GPT-4, GPT-3.5, etc.).

## Overview

The template includes adapters that allow Azure OpenAI models to interact with your MCP tools through function calling. This enables you to use the same tool implementations with both Claude (via MCP) and Azure OpenAI models (via HTTP/Function Calling).

## Architecture

```
┌─────────────────┐     ┌──────────────────┐     ┌──────────────┐
│  Azure OpenAI   │────▶│   HTTP Adapter   │────▶│  MCP Server  │
│  (GPT-4, etc)   │     │ (Function Calls) │     │   (Tools)    │
└─────────────────┘     └──────────────────┘     └──────────────┘
         │                                                 │
         └────────────── Function Calling ─────────────────┘
```

## Supported Azure OpenAI Models

All models that support function calling work with this template:

| Model | Function Calling | Vision | Max Tokens | Best For |
|-------|-----------------|--------|------------|----------|
| gpt-4 | ✅ | ❌ | 8,192 | Complex reasoning tasks |
| gpt-4-32k | ✅ | ❌ | 32,768 | Long context tasks |
| gpt-4-turbo | ✅ | ✅ | 128,000 | Latest features, vision |
| gpt-4o | ✅ | ✅ | 128,000 | Optimized for speed |
| gpt-4o-mini | ✅ | ✅ | 128,000 | Cost-effective |
| gpt-35-turbo | ✅ | ❌ | 16,384 | Fast, economical |
| gpt-35-turbo-16k | ✅ | ❌ | 16,384 | Larger context |

## Configuration

### 1. Azure OpenAI Setup

First, create an Azure OpenAI resource and deploy a model:

```bash
# Create Azure OpenAI resource
az cognitiveservices account create \
  --name "your-openai-resource" \
  --resource-group "your-rg" \
  --kind "OpenAI" \
  --sku "S0" \
  --location "eastus"

# Deploy a model (e.g., GPT-4)
az cognitiveservices account deployment create \
  --name "your-openai-resource" \
  --resource-group "your-rg" \
  --deployment-name "gpt-4" \
  --model-name "gpt-4" \
  --model-version "0613" \
  --model-format "OpenAI" \
  --scale-settings-scale-type "Standard"
```

### 2. Update Configuration

Add your Azure OpenAI credentials to `local.settings.json`:

```json
{
  "Values": {
    "AzureOpenAI__Endpoint": "https://your-resource.openai.azure.com/",
    "AzureOpenAI__ApiKey": "your-api-key-here",
    "AzureOpenAI__DeploymentName": "gpt-4"
  }
}
```

For production, use Azure Key Vault or App Configuration:

```bash
# Using Key Vault
az keyvault secret set \
  --vault-name "your-keyvault" \
  --name "AzureOpenAI--ApiKey" \
  --value "your-api-key"
```

## Usage Examples

### 1. Chat with Function Calling

Send a message that will trigger tool usage:

```bash
# Request
curl -X POST http://localhost:7071/api/openai/chat \
  -H "Content-Type: application/json" \
  -d '{
    "message": "Get information about customer ID 12345",
    "model": "gpt-4"
  }'

# Response
{
  "message": "I found the customer information for ID 12345. The customer is John Doe...",
  "toolCalls": [
    {
      "toolName": "get_customer",
      "arguments": "{\"customerId\":\"12345\"}",
      "result": {
        "id": "12345",
        "name": "John Doe",
        "email": "john@example.com"
      }
    }
  ]
}
```

### 2. Direct Tool Execution

Execute tools directly without chat:

```bash
# Execute a single tool
curl -X POST http://localhost:7071/api/tools/get_customer/execute \
  -H "Content-Type: application/json" \
  -d '{
    "customerId": "12345"
  }'

# Response
{
  "success": true,
  "toolName": "get_customer",
  "result": {
    "id": "12345",
    "name": "John Doe",
    "email": "john@example.com"
  },
  "executedAt": "2024-01-10T10:30:00Z"
}
```

### 3. Batch Tool Execution

Execute multiple tools in one request:

```bash
curl -X POST http://localhost:7071/api/tools/batch \
  -H "Content-Type: application/json" \
  -d '{
    "tools": [
      {
        "name": "get_customer",
        "arguments": {"customerId": "12345"}
      },
      {
        "name": "get_customer",
        "arguments": {"customerId": "67890"}
      }
    ]
  }'
```

### 4. List Available Tools

Get all available tools with their schemas:

```bash
curl http://localhost:7071/api/tools

# Returns MCP tools in OpenAI function format
curl http://localhost:7071/api/openai/functions
```

## Integration Patterns

### Pattern 1: Conversational AI with Tools

```csharp
// Your application
var client = new HttpClient();
var response = await client.PostAsJsonAsync(
    "http://localhost:7071/api/openai/chat",
    new
    {
        message = "Create a new customer named Jane Smith with email jane@example.com",
        model = "gpt-4"
    });

var result = await response.Content.ReadFromJsonAsync<ChatResponse>();
// Result includes both the AI response and tool execution details
```

### Pattern 2: Direct Tool Integration

```csharp
// When you know which tool to call
var response = await client.PostAsJsonAsync(
    "http://localhost:7071/api/tools/create_customer/execute",
    new
    {
        name = "Jane Smith",
        email = "jane@example.com"
    });
```

### Pattern 3: Custom GPT Actions

Create a Custom GPT in ChatGPT with this OpenAPI spec:

```yaml
openapi: 3.1.0
info:
  title: Your MCP Tools API
  version: 1.0.0
servers:
  - url: https://your-function.azurewebsites.net/api
paths:
  /tools/{toolName}/execute:
    post:
      operationId: executeTool
      parameters:
        - name: toolName
          in: path
          required: true
          schema:
            type: string
            enum: [get_customer, create_customer, update_customer, delete_customer]
      requestBody:
        required: true
        content:
          application/json:
            schema:
              type: object
      responses:
        '200':
          description: Tool executed successfully
```

## Advanced Usage

### 1. Model Selection

Choose the right model based on your needs:

```json
{
  "message": "Your request here",
  "model": "gpt-35-turbo"  // For cost-effective simple tasks
}

{
  "message": "Complex analysis request",
  "model": "gpt-4"  // For complex reasoning
}

{
  "message": "Analyze this image and update customer record",
  "model": "gpt-4-turbo"  // For vision + tools
}
```

### 2. Streaming Responses

For long responses, implement streaming:

```csharp
// In AzureOpenAIAdapter.cs
var streamingResponse = await _openAIClient.GetChatCompletionsStreamingAsync(options);
await foreach (var update in streamingResponse)
{
    // Stream updates back to client
}
```

### 3. Error Handling

The adapter includes comprehensive error handling:

- Azure OpenAI not configured → 503 Service Unavailable
- Invalid request → 400 Bad Request
- Tool execution errors → Error details in response
- Rate limiting → Automatic retry with Polly

### 4. Monitoring and Logging

Monitor usage and performance:

```csharp
_logger.LogInformation("Azure OpenAI request: Model={Model}, Tokens={Tokens}", 
    model, response.Usage.TotalTokens);
```

## Cost Optimization

### 1. Model Selection Strategy

```csharp
// Choose model based on complexity
var model = DetermineOptimalModel(request);

string DetermineOptimalModel(ChatRequest request)
{
    if (request.Message.Length < 100 && !RequiresComplexReasoning(request))
        return "gpt-35-turbo";  // $0.0015 per 1K tokens
    
    if (request.RequiresVision)
        return "gpt-4-turbo";   // $0.01 per 1K tokens
    
    return "gpt-4o-mini";       // Balance of cost and capability
}
```

### 2. Caching Strategies

Implement caching for repeated queries:

```csharp
// Add to your service
private readonly IMemoryCache _cache;

var cacheKey = $"tool_{toolName}_{GetArgumentsHash(arguments)}";
if (_cache.TryGetValue(cacheKey, out var cachedResult))
{
    return cachedResult;
}
```

## Troubleshooting

### Common Issues

1. **"Azure OpenAI not configured" error**
   - Ensure `AzureOpenAI__Endpoint` and `AzureOpenAI__ApiKey` are set
   - Check the endpoint format: `https://your-resource.openai.azure.com/`

2. **"Model not found" error**
   - Verify the deployment name matches your Azure OpenAI deployment
   - Ensure the model is deployed in your region

3. **Function calling not working**
   - Confirm you're using a model that supports functions (see table above)
   - Check that your tool schemas are properly formatted

4. **Rate limiting errors**
   - Implement retry logic (already included via Polly)
   - Consider using multiple deployments for load distribution

### Debug Mode

Enable detailed logging:

```json
{
  "Logging": {
    "LogLevel": {
      "Template.Functions.Adapters": "Debug"
    }
  }
}
```

## Security Considerations

1. **API Key Protection**
   - Never commit API keys to source control
   - Use Azure Key Vault in production
   - Rotate keys regularly

2. **Function Authorization**
   - Default uses Function-level auth
   - Consider Azure AD for production:

```csharp
[Function("SecureChat")]
[Authorize]
public async Task<HttpResponseData> SecureChat(
    [HttpTrigger(AuthorizationLevel.Anonymous, "post")] HttpRequestData req)
```

3. **Input Validation**
   - All inputs are validated before tool execution
   - Implement additional validation as needed

## Next Steps

1. **Implement your tools** in `McpServer.cs`
2. **Test with Azure OpenAI** using the provided endpoints
3. **Deploy to Azure** with proper configuration
4. **Monitor usage** through Application Insights
5. **Optimize costs** by selecting appropriate models

For more information:
- [Azure OpenAI Service Documentation](https://learn.microsoft.com/azure/cognitive-services/openai/)
- [Function Calling Guide](https://platform.openai.com/docs/guides/function-calling)
- [MCP Specification](https://modelcontextprotocol.io/)