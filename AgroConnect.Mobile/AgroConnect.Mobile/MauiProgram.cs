using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.ViewModels.Account;
using AgroConnect.Mobile.ViewModels.Execution;
using AgroConnect.Mobile.ViewModels.Home;
using AgroConnect.Mobile.ViewModels.Jobs;
using AgroConnect.Mobile.ViewModels.Profile;
using AgroConnect.Mobile.Views.Account;
using AgroConnect.Mobile.Views.Execution;
using AgroConnect.Mobile.Views.Home;
using AgroConnect.Mobile.Views.Jobs;
using AgroConnect.Mobile.Views.Profile;
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

        // ── Services (Singleton = una instancia compartida) ──
        builder.Services.AddSingleton<IApiService, ApiService>();
        builder.Services.AddSingleton<IAuthService, AuthService>();
        builder.Services.AddSingleton<ISecureStorageService, SecureStorageService>();
        builder.Services.AddSingleton<IConnectivityService, ConnectivityService>();

        // ── Services (Transient = nueva instancia por inyección) ──
        builder.Services.AddTransient<IDashboardService, DashboardService>();
        builder.Services.AddTransient<IJobService, JobService>();
        builder.Services.AddTransient<IExecutionService, ExecutionService>();
        builder.Services.AddTransient<IApplicatorService, ApplicatorService>();
        builder.Services.AddTransient<INotificationService, NotificationService>();
        builder.Services.AddTransient<ILocationService, LocationService>();
        builder.Services.AddTransient<IOperatorService, OperatorService>();

        // ── ViewModels ──
        builder.Services.AddTransient<LoginViewModel>();
        builder.Services.AddTransient<RegisterViewModel>();
        builder.Services.AddTransient<ChangePasswordViewModel>();
        builder.Services.AddTransient<HomeViewModel>();
        builder.Services.AddTransient<JobListViewModel>();
        builder.Services.AddTransient<ExecutionListViewModel>();
        builder.Services.AddTransient<ExecutionDetailViewModel>();
        builder.Services.AddTransient<ExecutionChecklistViewModel>();
        builder.Services.AddTransient<ProfileViewModel>();

        // ── Pages ──
        builder.Services.AddTransient<LoginPage>();
        builder.Services.AddTransient<RegisterPage>();
        builder.Services.AddTransient<ChangePasswordPage>();
        builder.Services.AddTransient<HomePage>();
        builder.Services.AddTransient<JobListPage>();
        builder.Services.AddTransient<ExecutionListPage>();
        builder.Services.AddTransient<ExecutionPage>();
        builder.Services.AddTransient<ExecutionChecklistPage>();
        builder.Services.AddTransient<ProfilePage>();

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}
