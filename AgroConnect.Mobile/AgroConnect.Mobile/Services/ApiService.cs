using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;
using AgroConnect.Mobile.Helpers;
using AgroConnect.Mobile.Models;
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
        await EnsureSuccessOrThrowAsync(response);
        return await response.Content.ReadFromJsonAsync<T>(JsonOptions);
    }

    public async Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data)
    {
        var response = await _http.PostAsJsonAsync(endpoint, data, JsonOptions);
        await EnsureSuccessOrThrowAsync(response);
        return await response.Content.ReadFromJsonAsync<TResponse>(JsonOptions);
    }

    /// <summary>POST sin body (para endpoints que no requieren request body)</summary>
    public async Task<TResponse?> PostEmptyAsync<TResponse>(string endpoint)
    {
        var response = await _http.PostAsync(endpoint, null);
        await EnsureSuccessOrThrowAsync(response);
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

    // ── Error handling ──────────────────────────────────────

    /// <summary>
    /// Parsea el body de error de la API ({ error: "..." } o { errors: [...] })
    /// y lanza ApiException con el mensaje legible.
    /// </summary>
    private static async Task EnsureSuccessOrThrowAsync(HttpResponseMessage response)
    {
        if (response.IsSuccessStatusCode) return;

        var message = response.StatusCode switch
        {
            HttpStatusCode.Unauthorized => "Sesión expirada. Iniciá sesión nuevamente.",
            HttpStatusCode.Forbidden => "No tenés permisos para esta acción.",
            HttpStatusCode.NotFound => "No se encontró el recurso solicitado.",
            _ => null
        };

        // Intentar parsear el error del body
        if (message is null)
        {
            try
            {
                var body = await response.Content.ReadAsStringAsync();
                if (!string.IsNullOrWhiteSpace(body))
                {
                    var error = JsonSerializer.Deserialize<ApiErrorResponse>(body, JsonOptions);
                    if (!string.IsNullOrEmpty(error?.Error))
                        message = error.Error;
                    else if (error?.Errors is { Count: > 0 })
                        message = string.Join(". ", error.Errors);
                }
            }
            catch { /* si no puede parsear, usa mensaje genérico */ }
        }

        message ??= $"Error del servidor ({(int)response.StatusCode}).";
        throw new ApiException(message, response.StatusCode);
    }
}

/// <summary>Excepción tipada para errores de API con StatusCode</summary>
public class ApiException : Exception
{
    public HttpStatusCode StatusCode { get; }

    public ApiException(string message, HttpStatusCode statusCode) : base(message)
    {
        StatusCode = statusCode;
    }

    public bool IsUnauthorized => StatusCode == HttpStatusCode.Unauthorized;
}
