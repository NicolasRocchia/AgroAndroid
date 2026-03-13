using AgroConnect.Mobile.Models;

namespace AgroConnect.Mobile.Services.Interfaces;

// ══════════════════════════════════════════════════════════════
// INFRAESTRUCTURA
// ══════════════════════════════════════════════════════════════

public interface IApiService
{
    Task<T?> GetAsync<T>(string endpoint);
    Task<TResponse?> PostAsync<TRequest, TResponse>(string endpoint, TRequest data);
    Task<TResponse?> PostEmptyAsync<TResponse>(string endpoint);
    Task<bool> PutAsync<T>(string endpoint, T data);
    Task<bool> DeleteAsync(string endpoint);
    void SetAuthToken(string token);
    void ClearAuthToken();
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

// ══════════════════════════════════════════════════════════════
// AUTH — POST /api/auth/login, GET /api/auth/profile, etc.
// ══════════════════════════════════════════════════════════════

public interface IAuthService
{
    Task<bool> IsAuthenticatedAsync();
    Task<LoginResponse?> LoginAsync(string email, string password);
    Task LogoutAsync();
    Task<LoginResponse?> GetCachedLoginAsync();
    Task<string?> GetTokenAsync();
    /// <summary>Rol principal del usuario (primer rol de la lista)</summary>
    Task<string?> GetPrimaryRoleAsync();
}

// ══════════════════════════════════════════════════════════════
// RECETAS — GET /api/recipes (paginado), GET /api/recipes/by-code/{code}
// ══════════════════════════════════════════════════════════════

public interface IRecipeService
{
    /// <summary>
    /// GET /api/recipes?page=X&amp;pageSize=Y
    /// La API filtra automáticamente por CreatedByUserId si el rol es Productor
    /// </summary>
    Task<PagedResponse<RecipeListItem>> GetMyRecipesAsync(int page = 1, int pageSize = 20, string? status = null);

    /// <summary>GET /api/recipes/by-code/{code}</summary>
    Task<RecipeDetail?> GetRecipeDetailAsync(string code);

    /// <summary>GET /api/recipes/{id}</summary>
    Task<RecipeDetail?> GetRecipeDetailByIdAsync(long id);
}

// ══════════════════════════════════════════════════════════════
// EJECUCIÓN — /api/executions/*
// ══════════════════════════════════════════════════════════════

public interface IExecutionService
{
    /// <summary>GET /api/executions/my-executions (aplicador)</summary>
    Task<List<ExecutionListDto>> GetMyExecutionsAsync(string? status = null);

    /// <summary>GET /api/executions/my-assignments (productor)</summary>
    Task<List<ExecutionListDto>> GetMyAssignmentsAsync(string? status = null);

    /// <summary>GET /api/executions/{id}</summary>
    Task<ExecutionDetailDto?> GetDetailAsync(long id);

    /// <summary>GET /api/executions/by-recipe/{recipeId}</summary>
    Task<ExecutionDetailDto?> GetByRecipeAsync(long recipeId);

    // ── Transiciones (todas POST con ExecutionTransitionRequest) ──
    Task<ExecutionDetailDto?> AcceptAsync(long id, ExecutionTransitionRequest request);
    Task<ExecutionDetailDto?> StartRouteAsync(long id, ExecutionTransitionRequest request);
    Task<ExecutionDetailDto?> StartAsync(long id, ExecutionTransitionRequest request);
    Task<ExecutionDetailDto?> PauseAsync(long id, ExecutionTransitionRequest request);
    Task<ExecutionDetailDto?> ResumeAsync(long id, ExecutionTransitionRequest request);
    Task<ExecutionDetailDto?> CompleteAsync(long id, ExecutionTransitionRequest request);
    Task<ExecutionDetailDto?> CancelAsync(long id, ExecutionTransitionRequest request);

    /// <summary>POST /api/executions/{id}/checklist</summary>
    Task<ExecutionDetailDto?> SubmitChecklistAsync(long id, SubmitChecklistRequest request);

    /// <summary>POST /api/executions/{id}/review</summary>
    Task<ExecutionDetailDto?> CreateReviewAsync(long id, CreateReviewRequest request);
}

// ══════════════════════════════════════════════════════════════
// LOTES — /api/lots/*
// ══════════════════════════════════════════════════════════════

public interface ILotService
{
    /// <summary>GET /api/lots/my-lots</summary>
    Task<List<LotListItem>> GetMyLotsAsync();

    /// <summary>GET /api/lots/by-code/{code}</summary>
    Task<LotDetail?> GetLotDetailAsync(string code);

    /// <summary>GET /api/lots/{id}</summary>
    Task<LotDetail?> GetLotDetailByIdAsync(long id);
}

// ══════════════════════════════════════════════════════════════
// BOLSA DE TRABAJO — /api/jobs/*
// ══════════════════════════════════════════════════════════════

public interface IJobService
{
    /// <summary>GET /api/jobs/available (aplicador, usa ubicación del perfil)</summary>
    Task<List<JobPostingListDto>> GetAvailableJobsAsync();

    /// <summary>POST /api/jobs/{id}/apply</summary>
    Task<bool> ApplyToJobAsync(long jobId, ApplyToJobRequest request);

    /// <summary>DELETE /api/jobs/{id}/apply</summary>
    Task<bool> WithdrawApplicationAsync(long jobId);
}

// ══════════════════════════════════════════════════════════════
// NOTIFICACIONES — /api/notifications/*
// ══════════════════════════════════════════════════════════════

public interface INotificationService
{
    /// <summary>GET /api/notifications/unread-count → { count: int }</summary>
    Task<int> GetUnreadCountAsync();

    /// <summary>GET /api/notifications?limit=50</summary>
    Task<List<NotificationDto>> GetNotificationsAsync(int limit = 50);

    /// <summary>PUT /api/notifications/{id}/read</summary>
    Task MarkAsReadAsync(long id);

    /// <summary>PUT /api/notifications/read-all</summary>
    Task MarkAllAsReadAsync();
}

// ══════════════════════════════════════════════════════════════
// PERFIL APLICADOR — /api/applicator/*
// ══════════════════════════════════════════════════════════════

public interface IApplicatorService
{
    /// <summary>GET /api/applicator/profile</summary>
    Task<ApplicatorProfileDto?> GetMyProfileAsync();

    /// <summary>GET /api/applicator/has-role</summary>
    Task<bool> HasApplicatorRoleAsync();
}

// ══════════════════════════════════════════════════════════════
// UBICACIÓN + METEO
// ══════════════════════════════════════════════════════════════

public interface ILocationService
{
    Task<(double Latitude, double Longitude)?> GetCurrentLocationAsync();
    Task<WeatherData?> GetWeatherAsync(double lat, double lng);
}
