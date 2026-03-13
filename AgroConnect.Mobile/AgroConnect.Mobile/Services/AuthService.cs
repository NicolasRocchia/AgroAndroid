using System.Text.Json;
using AgroConnect.Mobile.Helpers;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.Services;

public class AuthService : IAuthService
{
    private readonly IApiService _api;
    private readonly ISecureStorageService _storage;
    private LoginResponse? _cached;

    private static readonly JsonSerializerOptions JsonOpts = new()
    {
        PropertyNameCaseInsensitive = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public AuthService(IApiService api, ISecureStorageService storage)
    {
        _api = api;
        _storage = storage;
    }

    public async Task<bool> IsAuthenticatedAsync()
    {
        var token = await _storage.GetAsync(Constants.TokenKey);
        if (string.IsNullOrEmpty(token)) return false;

        var expiresStr = await _storage.GetAsync(Constants.TokenExpiresKey);
        if (!string.IsNullOrEmpty(expiresStr)
            && DateTime.TryParse(expiresStr, out var expires)
            && expires <= DateTime.UtcNow)
        {
            return false;
        }

        _api.SetAuthToken(token);
        return true;
    }

    public async Task<LoginResponse?> LoginAsync(string email, string password)
    {
        var response = await _api.PostAsync<LoginRequest, LoginResponse>(
            "auth/login", new(email, password));

        if (response is null) return null;

        await _storage.SetAsync(Constants.TokenKey, response.Token);
        await _storage.SetAsync(Constants.TokenExpiresKey, response.ExpiresAt.ToString("O"));
        await _storage.SetAsync(Constants.LoginDataKey, JsonSerializer.Serialize(response, JsonOpts));

        _api.SetAuthToken(response.Token);
        _cached = response;
        return response;
    }

    public async Task<RegisterResponse?> RegisterAsync(RegisterRequest request)
    {
        return await _api.PostAsync<RegisterRequest, RegisterResponse>("auth/register", request);
    }

    public async Task LogoutAsync()
    {
        _api.ClearAuthToken();
        _cached = null;
        await _storage.ClearAllAsync();
    }

    public async Task<LoginResponse?> GetCachedLoginAsync()
    {
        if (_cached is not null) return _cached;

        var json = await _storage.GetAsync(Constants.LoginDataKey);
        if (string.IsNullOrEmpty(json)) return null;

        try
        {
            _cached = JsonSerializer.Deserialize<LoginResponse>(json, JsonOpts);
            return _cached;
        }
        catch
        {
            return null;
        }
    }

    public Task<string?> GetTokenAsync()
        => _storage.GetAsync(Constants.TokenKey);

    public async Task<string?> GetPrimaryRoleAsync()
    {
        var login = await GetCachedLoginAsync();
        return login?.Roles.FirstOrDefault();
    }

    public async Task<UserProfileDto?> GetProfileAsync()
        => await _api.GetAsync<UserProfileDto>("auth/profile");

    public async Task<UserProfileDto?> UpdateProfileAsync(UpdateProfileRequest request)
    {
        // PutAsync throws ApiException on failure
        await _api.PutAsync("auth/profile", request);
        return await GetProfileAsync();
    }

    public async Task<bool> ChangePasswordAsync(ChangePasswordRequest request)
    {
        // PutAsync now throws ApiException on failure with the API error message
        return await _api.PutAsync("auth/change-password", request);
    }
}
