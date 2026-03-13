using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AgroConnect.Mobile.Helpers;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.Services;

public class ApiService : IApiService
{
    private readonly HttpClient _http;
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public ApiService()
    {
        var handler = new HttpClientHandler();
#if DEBUG
        handler.ServerCertificateCustomValidationCallback = (_, _, _, _) => true;
#endif
        _http = new HttpClient(handler)
        {
            BaseAddress = new Uri(Constants.ApiBaseUrl),
            Timeout = Constants.HttpTimeout
        };
    }

    public void SetAuthToken(string token)
        => _http.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

    public void ClearAuthToken()
        => _http.DefaultRequestHeaders.Authorization = null;

    public async Task<T?> GetAsync<T>(string endpoint)
    {
        var response = await _http.GetAsync(endpoint);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var response = await _http.PostAsJsonAsync(endpoint, data, JsonOptions);
        response.EnsureSuccessStatusCode();
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    public async Task<bool> PutAsync<T>(string endpoint, T data)
    {
        var response = await _http.PutAsJsonAsync(endpoint, data, JsonOptions);
        return response.IsSuccessStatusCode;
    }

    public async Task<bool> DeleteAsync(string endpoint)
    {
        var response = await _http.DeleteAsync(endpoint);
        return response.IsSuccessStatusCode;
    }
}
