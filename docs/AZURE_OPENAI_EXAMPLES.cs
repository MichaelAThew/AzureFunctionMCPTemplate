using System.Net.Http.Json;
using System.Text.Json;

namespace Template.Examples;

/// <summary>
/// Example usage of the Azure OpenAI adapter
/// </summary>
public class AzureOpenAIExample
{
    private readonly HttpClient _httpClient;

    public AzureOpenAIExample(HttpClient httpClient)
    {
        _httpClient = httpClient;
        _httpClient.BaseAddress = new Uri("http://localhost:7071/api/");
    }

    /// <summary>
    /// Example 1: Chat with function calling
    /// </summary>
    public async Task ChatWithToolsExample()
    {
        // Send a chat message that will trigger tool usage
        var chatRequest = new
        {
            message = "Can you get information about customer 12345 and then create a new customer named Jane Smith?",
            model = "gpt-4"
        };

        var response = await _httpClient.PostAsJsonAsync("openai/chat", chatRequest);
        var result = await response.Content.ReadFromJsonAsync<ChatResponse>();

        Console.WriteLine($"AI Response: {result.Message}");
        
        if (result.ToolCalls != null)
        {
            foreach (var toolCall in result.ToolCalls)
            {
                Console.WriteLine($"Tool used: {toolCall.ToolName}");
                Console.WriteLine($"Arguments: {toolCall.Arguments}");
                Console.WriteLine($"Result: {JsonSerializer.Serialize(toolCall.Result)}");
            }
        }
    }

    /// <summary>
    /// Example 2: Direct tool execution
    /// </summary>
    public async Task DirectToolExecutionExample()
    {
        // When you know exactly which tool to call
        var toolRequest = new
        {
            customerId = "12345"
        };

        var response = await _httpClient.PostAsJsonAsync("tools/get_customer/execute", toolRequest);
        var result = await response.Content.ReadFromJsonAsync<ToolExecutionResponse>();

        if (result.Success)
        {
            Console.WriteLine($"Customer data: {JsonSerializer.Serialize(result.Result)}");
        }
        else
        {
            Console.WriteLine($"Error: {result.Error}");
        }
    }

    /// <summary>
    /// Example 3: Batch operations
    /// </summary>
    public async Task BatchToolExecutionExample()
    {
        // Execute multiple tools in one request
        var batchRequest = new
        {
            tools = new[]
            {
                new { name = "get_customer", arguments = new { customerId = "12345" } },
                new { name = "get_customer", arguments = new { customerId = "67890" } },
                new { name = "create_customer", arguments = new { name = "Jane Doe", email = "jane@example.com" } }
            }
        };

        var response = await _httpClient.PostAsJsonAsync("tools/batch", batchRequest);
        var result = await response.Content.ReadFromJsonAsync<BatchToolResponse>();

        Console.WriteLine($"Total executed: {result.TotalRequested}");
        Console.WriteLine($"Succeeded: {result.TotalSucceeded}");
        Console.WriteLine($"Failed: {result.TotalFailed}");

        foreach (var toolResult in result.Results)
        {
            Console.WriteLine($"{toolResult.ToolName}: {(toolResult.Success ? "Success" : toolResult.Error)}");
        }
    }

    /// <summary>
    /// Example 4: Model selection based on task
    /// </summary>
    public async Task OptimizedModelSelectionExample()
    {
        // Simple query - use cheaper model
        var simpleRequest = new
        {
            message = "What is the status of customer 12345?",
            model = "gpt-35-turbo"  // Cheaper for simple queries
        };

        // Complex analysis - use advanced model
        var complexRequest = new
        {
            message = "Analyze our top 10 customers, identify patterns in their purchasing behavior, and suggest personalized retention strategies for each.",
            model = "gpt-4"  // Better for complex reasoning
        };

        // Vision + tools - use turbo model
        var visionRequest = new
        {
            message = "Look at this customer photo ID and update their profile accordingly",
            model = "gpt-4-turbo",  // Supports vision
            image = "base64_encoded_image_here"
        };
    }

    /// <summary>
    /// Example 5: Custom GPT integration
    /// </summary>
    public async Task CustomGptExample()
    {
        // This endpoint can be used as an action in Custom GPTs
        var customGptRequest = new
        {
            action = "get_customer",
            parameters = new { customerId = "12345" }
        };

        // Custom GPTs can call your tools through the OpenAPI spec
        var response = await _httpClient.PostAsJsonAsync("tools/get_customer/execute", customGptRequest.parameters);
        
        // The response will be automatically formatted for the Custom GPT
    }

    // Response models
    public class ChatResponse
    {
        public string Message { get; set; }
        public ToolCall[] ToolCalls { get; set; }
    }

    public class ToolCall
    {
        public string ToolName { get; set; }
        public string Arguments { get; set; }
        public object Result { get; set; }
    }

    public class ToolExecutionResponse
    {
        public bool Success { get; set; }
        public string ToolName { get; set; }
        public object Result { get; set; }
        public string Error { get; set; }
        public DateTime ExecutedAt { get; set; }
    }

    public class BatchToolResponse
    {
        public List<ToolExecutionResponse> Results { get; set; }
        public int TotalRequested { get; set; }
        public int TotalSucceeded { get; set; }
        public int TotalFailed { get; set; }
    }
}