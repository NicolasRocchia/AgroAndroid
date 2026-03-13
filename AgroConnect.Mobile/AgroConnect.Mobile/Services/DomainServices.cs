using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.Services;

// ══════════════════════════════════════════════════════════════
// RECETAS
// ══════════════════════════════════════════════════════════════

public class RecipeService(IApiService api) : IRecipeService
{
    public async Task<PagedResponse<RecipeListItem>> GetMyRecipesAsync(
        int page = 1, int pageSize = 20, string? status = null)
    {
        var url = $"recipes?page={page}&pageSize={pageSize}";
        if (!string.IsNullOrEmpty(status))
            url += $"&status={status}";

        return await api.GetAsync<PagedResponse<RecipeListItem>>(url)
               ?? new PagedResponse<RecipeListItem>();
    }

    public async Task<RecipeDetail?> GetRecipeDetailAsync(string code)
        => await api.GetAsync<RecipeDetail>($"recipes/by-code/{code}");

    public async Task<RecipeDetail?> GetRecipeDetailByIdAsync(long id)
        => await api.GetAsync<RecipeDetail>($"recipes/{id}");
}

// ══════════════════════════════════════════════════════════════
// EJECUCIÓN
// ══════════════════════════════════════════════════════════════

public class ExecutionService(IApiService api) : IExecutionService
{
    public async Task<List<ExecutionListDto>> GetMyExecutionsAsync(string? status = null)
    {
        var url = "executions/my-executions";
        if (!string.IsNullOrEmpty(status)) url += $"?status={status}";
        return await api.GetAsync<List<ExecutionListDto>>(url) ?? [];
    }

    public async Task<List<ExecutionListDto>> GetMyOperatorExecutionsAsync(string? status = null)
    {
        var url = "executions/my-operator-executions";
        if (!string.IsNullOrEmpty(status)) url += $"?status={status}";
        return await api.GetAsync<List<ExecutionListDto>>(url) ?? [];
    }

    public async Task<List<ExecutionListDto>> GetMyAssignmentsAsync(string? status = null)
    {
        var url = "executions/my-assignments";
        if (!string.IsNullOrEmpty(status)) url += $"?status={status}";
        return await api.GetAsync<List<ExecutionListDto>>(url) ?? [];
    }

    public async Task<ExecutionDetailDto?> GetDetailAsync(long id)
        => await api.GetAsync<ExecutionDetailDto>($"executions/{id}");

    public async Task<ExecutionDetailDto?> GetByRecipeAsync(long recipeId)
        => await api.GetAsync<ExecutionDetailDto>($"executions/by-recipe/{recipeId}");

    // ── Transiciones ──
    public async Task<ExecutionDetailDto?> AcceptAsync(long id, ExecutionTransitionRequest r)
        => await api.PostAsync<ExecutionTransitionRequest, ExecutionDetailDto>($"executions/{id}/accept", r);

    public async Task<ExecutionDetailDto?> StartRouteAsync(long id, ExecutionTransitionRequest r)
        => await api.PostAsync<ExecutionTransitionRequest, ExecutionDetailDto>($"executions/{id}/en-route", r);

    public async Task<ExecutionDetailDto?> StartAsync(long id, ExecutionTransitionRequest r)
        => await api.PostAsync<ExecutionTransitionRequest, ExecutionDetailDto>($"executions/{id}/start", r);

    public async Task<ExecutionDetailDto?> PauseAsync(long id, ExecutionTransitionRequest r)
        => await api.PostAsync<ExecutionTransitionRequest, ExecutionDetailDto>($"executions/{id}/pause", r);

    public async Task<ExecutionDetailDto?> ResumeAsync(long id, ExecutionTransitionRequest r)
        => await api.PostAsync<ExecutionTransitionRequest, ExecutionDetailDto>($"executions/{id}/resume", r);

    public async Task<ExecutionDetailDto?> CompleteAsync(long id, ExecutionTransitionRequest r)
        => await api.PostAsync<ExecutionTransitionRequest, ExecutionDetailDto>($"executions/{id}/complete", r);

    public async Task<ExecutionDetailDto?> CancelAsync(long id, ExecutionTransitionRequest r)
        => await api.PostAsync<ExecutionTransitionRequest, ExecutionDetailDto>($"executions/{id}/cancel", r);

    public async Task<ExecutionDetailDto?> SubmitChecklistAsync(long id, SubmitChecklistRequest r)
        => await api.PostAsync<SubmitChecklistRequest, ExecutionDetailDto>($"executions/{id}/checklist", r);

    public async Task<ExecutionDetailDto?> CreateReviewAsync(long id, CreateReviewRequest r)
        => await api.PostAsync<CreateReviewRequest, ExecutionDetailDto>($"executions/{id}/review", r);
}

// ══════════════════════════════════════════════════════════════
// LOTES
// ══════════════════════════════════════════════════════════════

public class LotService(IApiService api) : ILotService
{
    public async Task<List<LotListItem>> GetMyLotsAsync()
        => await api.GetAsync<List<LotListItem>>("lots/my-lots") ?? [];

    public async Task<LotDetail?> GetLotDetailAsync(string code)
        => await api.GetAsync<LotDetail>($"lots/by-code/{code}");

    public async Task<LotDetail?> GetLotDetailByIdAsync(long id)
        => await api.GetAsync<LotDetail>($"lots/{id}");
}

// ══════════════════════════════════════════════════════════════
// BOLSA DE TRABAJO
// ══════════════════════════════════════════════════════════════

public class JobService(IApiService api) : IJobService
{
    public async Task<List<JobPostingListDto>> GetAvailableJobsAsync()
        => await api.GetAsync<List<JobPostingListDto>>("jobs/available") ?? [];

    public async Task<bool> ApplyToJobAsync(long jobId, ApplyToJobRequest request)
    {
        var result = await api.PostAsync<ApplyToJobRequest, object>($"jobs/{jobId}/apply", request);
        return result is not null;
    }

    public async Task<bool> WithdrawApplicationAsync(long jobId)
        => await api.DeleteAsync($"jobs/{jobId}/apply");
}

// ══════════════════════════════════════════════════════════════
// NOTIFICACIONES
// ══════════════════════════════════════════════════════════════

public class NotificationService(IApiService api) : INotificationService
{
    public async Task<int> GetUnreadCountAsync()
    {
        var result = await api.GetAsync<UnreadCountResponse>("notifications/unread-count");
        return result?.Count ?? 0;
    }

    public async Task<List<NotificationDto>> GetNotificationsAsync(int limit = 50)
        => await api.GetAsync<List<NotificationDto>>($"notifications?limit={limit}") ?? [];

    public async Task MarkAsReadAsync(long id)
        => await api.PutAsync($"notifications/{id}/read", new { });

    public async Task MarkAllAsReadAsync()
        => await api.PutAsync("notifications/read-all", new { });
}

// ══════════════════════════════════════════════════════════════
// PERFIL APLICADOR
// ══════════════════════════════════════════════════════════════

public class ApplicatorService(IApiService api) : IApplicatorService
{
    public async Task<ApplicatorProfileDto?> GetMyProfileAsync()
        => await api.GetAsync<ApplicatorProfileDto>("applicator/profile");

    public async Task<bool> HasApplicatorRoleAsync()
    {
        try
        {
            var result = await api.GetAsync<HasRoleResponse>("applicator/has-role");
            return result?.HasRole ?? false;
        }
        catch { return false; }
    }

    private class HasRoleResponse
    {
        public bool HasRole { get; set; }
        public bool HasProfile { get; set; }
        public bool IsVerified { get; set; }
    }
}

// ══════════════════════════════════════════════════════════════
// DASHBOARD
// ══════════════════════════════════════════════════════════════

public class DashboardService(IApiService api) : IDashboardService
{
    public async Task<ApplicatorDashboardDto?> GetDashboardAsync()
        => await api.GetAsync<ApplicatorDashboardDto>("dashboard/applicator");
}

// ══════════════════════════════════════════════════════════════
// OPERARIOS
// ══════════════════════════════════════════════════════════════

public class OperatorService(IApiService api) : IOperatorService
{
    public async Task<List<OperatorListDto>> GetMyOperatorsAsync()
        => await api.GetAsync<List<OperatorListDto>>("operators") ?? [];

    public async Task<OperatorDetailDto?> GetDetailAsync(long id)
        => await api.GetAsync<OperatorDetailDto>($"operators/{id}");

    public async Task<CreateOperatorResponse?> CreateAsync(CreateOperatorRequest request)
        => await api.PostAsync<CreateOperatorRequest, CreateOperatorResponse>("operators", request);

    public async Task<bool> UpdateAsync(long id, UpdateOperatorRequest request)
        => await api.PutAsync($"operators/{id}", request);

    public async Task<bool> DeactivateAsync(long id)
    {
        var result = await api.PostAsync<object, MessageResponse>($"operators/{id}/deactivate", new { });
        return result is not null;
    }

    public async Task<bool> AssignToExecutionAsync(long executionId, AssignOperatorRequest request)
    {
        var result = await api.PostAsync<AssignOperatorRequest, object>($"executions/{executionId}/assign-operator", request);
        return result is not null;
    }
}
