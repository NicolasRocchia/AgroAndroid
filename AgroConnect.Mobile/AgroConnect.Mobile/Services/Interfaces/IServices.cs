using AgroConnect.Mobile.Models;

namespace AgroConnect.Mobile.Services.Interfaces;

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task<bool> PutAsync<T>(string endpoint, T data);
    Task<bool> DeleteAsync(string endpoint);
    void SetAuthToken(string token);
    void ClearAuthToken();
}

public interface IAuthService
{
    Task<bool> IsAuthenticatedAsync();
    Task<LoginResponse?> LoginAsync(string userName, string password);
    Task LogoutAsync();
    Task<UserInfo?> GetCurrentUserAsync();
    Task<string?> GetTokenAsync();
}

public interface ISecureStorageService
{
    Task SetAsync(string key, string value);
    Task<string?> GetAsync(string key);
    Task RemoveAsync(string key);
    Task ClearAllAsync();
}

public interface IConnectivityService
{
    bool IsConnected { get; }
    event EventHandler<bool> ConnectivityChanged;
}

public interface IRecipeService
{
    Task<List<RecipeListItem>> GetMyRecipesAsync(int page = 1, int pageSize = 20);
    Task<RecipeDetail?> GetRecipeDetailAsync(string code);
}

public interface IExecutionService
{
    Task<ExecutionSummary?> StartExecutionAsync(ExecutionStartRequest request);
    Task<bool> PauseExecutionAsync(int executionId);
    Task<bool> ResumeExecutionAsync(int executionId, double lat, double lng);
    Task<bool> CompleteExecutionAsync(int executionId);
    Task<bool> SubmitChecklistAsync(int executionId, List<ChecklistItem> items);
}

public interface ILotService
{
    Task<List<LotListItem>> GetMyLotsAsync();
    Task<LotDetail?> GetLotDetailAsync(string code);
}

public interface IJobService
{
    Task<List<JobListItem>> GetAvailableJobsAsync(double? lat = null, double? lng = null);
    Task<bool> PostulateAsync(int jobId);
}

public interface INotificationService
{
    Task<int> GetUnreadCountAsync();
}

public interface ILocationService
{
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
    Task<WeatherData?> GetWeatherAsync(double lat, double lng);
}
