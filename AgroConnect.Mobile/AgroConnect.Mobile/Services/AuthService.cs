using System.Text.Json;
using AgroConnect.Mobile.Helpers;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _api;
    private readonly ISecureStorageService _storage;
    private UserInfo? _cachedUser;

    public AuthService(IApiService api, ISecureStorageService storage)
    {
        _api = api;
        _storage = storage;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _storage.GetAsync(Constants.TokenKey);
        if (string.IsNullOrEmpty(token)) return false;
        _api.SetAuthToken(token);
        return true;
    }

    public async Task<LoginResponse?> LoginAsync(string userName, string password)
    {
        var response = await _api.PostAsync<LoginRequest, LoginResponse>("/auth/login", new(userName, password));
        if (response is null) return null;

        await _storage.SetAsync(Constants.TokenKey, response.Token);
        await _storage.SetAsync(Constants.RefreshTokenKey, response.RefreshToken);
        await _storage.SetAsync(Constants.UserInfoKey, JsonSerializer.Serialize(response.User));
        _api.SetAuthToken(response.Token);
        _cachedUser = response.User;
        return response;
    }

    public async Task LogoutAsync()
    {
        _api.ClearAuthToken();
        _cachedUser = null;
        await _storage.ClearAllAsync();
    }

    public async Task<UserInfo?> GetCurrentUserAsync()
    {
        if (_cachedUser is not null) return _cachedUser;
        var json = await _storage.GetAsync(Constants.UserInfoKey);
        if (string.IsNullOrEmpty(json)) return null;
        _cachedUser = JsonSerializer.Deserialize<UserInfo>(json);
        return _cachedUser;
    }

    public Task<string?> GetTokenAsync() => _storage.GetAsync(Constants.TokenKey);
}
