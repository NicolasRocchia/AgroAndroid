using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.ViewModels.Account;
using AgroConnect.Mobile.ViewModels.Execution;
using AgroConnect.Mobile.ViewModels.Jobs;
using AgroConnect.Mobile.ViewModels.Lots;
using AgroConnect.Mobile.ViewModels.Profile;
using AgroConnect.Mobile.ViewModels.Recipes;
using AgroConnect.Mobile.Views.Account;
using AgroConnect.Mobile.Views.Execution;
using AgroConnect.Mobile.Views.Jobs;
using AgroConnect.Mobile.Views.Lots;
using AgroConnect.Mobile.Views.Profile;
using AgroConnect.Mobile.Views.Recipes;
using Microsoft.Extensions.Logging;

namespace AgroConnect.Mobile;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        var builder = MauiApp.CreateBuilder();

        builder
            .UseMauiApp<App>()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

        // ── Services ──────────────────────────────────────────
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
        builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();

        builder.Services.AddTransient<IRecipeService, RecipeService>();
        builder.Services.AddTransient<IExecutionService, ExecutionService>();
        builder.Services.AddTransient<ILotService, LotService>();
        builder.Services.AddTransient<IJobService, JobService>();
        builder.Services.AddTransient<INotificationService, NotificationService>();
        builder.Services.AddTransient<ILocationService, LocationService>();

        // ── ViewModels ────────────────────────────────────────
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<RecipeListViewModel>();
        builder.Services.AddTransient<RecipeDetailViewModel>();
        builder.Services.AddTransient<ExecutionViewModel>();
        builder.Services.AddTransient<ExecutionChecklistViewModel>();
        builder.Services.AddTransient<LotListViewModel>();
        builder.Services.AddTransient<LotDetailViewModel>();
        builder.Services.AddTransient<JobListViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // ── Pages ─────────────────────────────────────────────
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<RecipeListPage>();
        builder.Services.AddTransient<RecipeDetailPage>();
        builder.Services.AddTransient<ExecutionPage>();
        builder.Services.AddTransient<ExecutionChecklistPage>();
        builder.Services.AddTransient<LotListPage>();
        builder.Services.AddTransient<LotDetailPage>();
        builder.Services.AddTransient<JobListPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
