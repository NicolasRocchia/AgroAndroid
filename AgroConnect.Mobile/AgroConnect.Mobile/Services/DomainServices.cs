using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.Services;

public class RecipeService(IApiService api) : IRecipeService
{
    public async Task<List<RecipeListItem>> GetMyRecipesAsync(int page = 1, int pageSize = 20)
        => await api.GetAsync<List<RecipeListItem>>($"/recipes/mine?page={page}&pageSize={pageSize}") ?? [];

    public async Task<RecipeDetail?> GetRecipeDetailAsync(string code)
        => await api.GetAsync<RecipeDetail>($"/recipes/{code}");
}

public class ExecutionService(IApiService api) : IExecutionService
{
    public async Task<ExecutionSummary?> StartExecutionAsync(ExecutionStartRequest request)
        => await api.PostAsync<ExecutionStartRequest, ExecutionSummary>("/executions/start", request);

    public async Task<bool> PauseExecutionAsync(int id) => await api.PutAsync($"/executions/{id}/pause", new { });
    public async Task<bool> ResumeExecutionAsync(int id, double lat, double lng) => await api.PutAsync($"/executions/{id}/resume", new { lat, lng });
    public async Task<bool> CompleteExecutionAsync(int id) => await api.PutAsync($"/executions/{id}/complete", new { });
    public async Task<bool> SubmitChecklistAsync(int id, List<ChecklistItem> items) => await api.PutAsync($"/executions/{id}/checklist", items);
}

public class LotService(IApiService api) : ILotService
{
    public async Task<List<LotListItem>> GetMyLotsAsync()
        => await api.GetAsync<List<LotListItem>>("/lots/mine") ?? [];

    public async Task<LotDetail?> GetLotDetailAsync(string code)
        => await api.GetAsync<LotDetail>($"/lots/{code}");
}

public class JobService(IApiService api) : IJobService
{
    public async Task<List<JobListItem>> GetAvailableJobsAsync(double? lat = null, double? lng = null)
    {
        var q = lat.HasValue && lng.HasValue
            ? $"?lat={lat.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}&lng={lng.Value.ToString(System.Globalization.CultureInfo.InvariantCulture)}"
            : "";
        return await api.GetAsync<List<JobListItem>>($"/jobs/available{q}") ?? [];
    }

    public async Task<bool> PostulateAsync(int jobId)
    {
        return await api.PostAsync<object, bool?>($"/jobs/{jobId}/postulate", new { }) ?? false;
    }
}

public class NotificationService(IApiService api) : INotificationService
{
    public async Task<int> GetUnreadCountAsync()
        => await api.GetAsync<int>("/notifications/unread-count");
}
