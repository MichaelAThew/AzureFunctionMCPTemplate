using Polly;
using Polly.Extensions.Http;
using Polly.CircuitBreaker;
using Microsoft.Extensions.Logging;

namespace Template.Functions.Configuration;

/// <summary>
/// HTTP resilience policies using Polly
/// </summary>
public static class HttpPolicies
{
    /// <summary>
    /// Get retry policy for transient HTTP errors
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetRetryPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .OrResult(msg => !msg.IsSuccessStatusCode)
            .WaitAndRetryAsync(
                3,
                retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                onRetry: (outcome, timespan, retryCount, context) =>
                {
                    var logger = context.Values.ContainsKey("logger") ? context.Values["logger"] : null;
                    if (logger is ILogger log)
                    {
                        log.LogWarning("Retry {RetryCount} after {TimeSpan}ms", retryCount, timespan.TotalMilliseconds);
                    }
                });
    }

    /// <summary>
    /// Get circuit breaker policy
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCircuitBreakerPolicy()
    {
        return HttpPolicyExtensions
            .HandleTransientHttpError()
            .CircuitBreakerAsync(
                5,
                TimeSpan.FromSeconds(30),
                onBreak: (result, timespan) =>
                {
                    // Log circuit breaker opened
                },
                onReset: () =>
                {
                    // Log circuit breaker closed
                },
                onHalfOpen: () =>
                {
                    // Log circuit breaker half-open
                });
    }

    /// <summary>
    /// Get timeout policy
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetTimeoutPolicy(int timeoutSeconds = 30)
    {
        return Policy.TimeoutAsync<HttpResponseMessage>(timeoutSeconds);
    }

    /// <summary>
    /// Get combined policy with retry, circuit breaker, and timeout
    /// </summary>
    public static IAsyncPolicy<HttpResponseMessage> GetCombinedPolicy(int timeoutSeconds = 30)
    {
        var retryPolicy = GetRetryPolicy();
        var circuitBreakerPolicy = GetCircuitBreakerPolicy();
        var timeoutPolicy = GetTimeoutPolicy(timeoutSeconds);

        return Policy.WrapAsync(retryPolicy, circuitBreakerPolicy, timeoutPolicy);
    }
}