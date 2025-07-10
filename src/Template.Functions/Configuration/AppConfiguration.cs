namespace Template.Functions.Configuration;

/// <summary>
/// Application configuration settings
/// </summary>
public class AppConfiguration
{
    /// <summary>
    /// External API URL
    /// </summary>
    public string ApiUrl { get; set; } = "https://api.example.com";

    /// <summary>
    /// API authentication key
    /// </summary>
    public string ApiKey { get; set; } = string.Empty;

    /// <summary>
    /// HTTP request timeout in seconds
    /// </summary>
    public int TimeoutSeconds { get; set; } = 30;

    /// <summary>
    /// Maximum retry attempts
    /// </summary>
    public int MaxRetryAttempts { get; set; } = 3;

    /// <summary>
    /// Circuit breaker failure threshold
    /// </summary>
    public int CircuitBreakerThreshold { get; set; } = 5;

    /// <summary>
    /// Circuit breaker duration in seconds
    /// </summary>
    public int CircuitBreakerDurationSeconds { get; set; } = 30;
}