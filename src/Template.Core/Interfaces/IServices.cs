namespace Template.Core.Interfaces;

/// <summary>
/// Example service interface - replace with your domain-specific services
/// </summary>
public interface IExampleService
{
    /// <summary>
    /// Get data by ID
    /// </summary>
    Task<string> GetDataAsync(string id, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Create new data
    /// </summary>
    Task<string> CreateDataAsync(object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Update existing data
    /// </summary>
    Task<bool> UpdateDataAsync(string id, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Delete data
    /// </summary>
    Task<bool> DeleteDataAsync(string id, CancellationToken cancellationToken = default);
}

/// <summary>
/// External API client interface
/// </summary>
public interface IApiClient
{
    /// <summary>
    /// Make GET request
    /// </summary>
    Task<T?> GetAsync<T>(string endpoint, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Make POST request
    /// </summary>
    Task<T?> PostAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Make PUT request
    /// </summary>
    Task<T?> PutAsync<T>(string endpoint, object data, CancellationToken cancellationToken = default);
    
    /// <summary>
    /// Make DELETE request
    /// </summary>
    Task<bool> DeleteAsync(string endpoint, CancellationToken cancellationToken = default);
}