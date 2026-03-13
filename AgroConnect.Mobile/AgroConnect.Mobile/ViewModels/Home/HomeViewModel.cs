using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.ViewModels.Home;

public partial class HomeViewModel : ObservableObject
{
    private readonly IDashboardService _dashboard;

    public HomeViewModel(IDashboardService dashboard) => _dashboard = dashboard;

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _hasError;

    // ── Identidad ──
    [ObservableProperty] private string _displayName = string.Empty;
    [ObservableProperty] private string _role = string.Empty;
    [ObservableProperty] private bool _isVerified;
    [ObservableProperty] private bool _isApplicator;
    [ObservableProperty] private bool _isOperator;

    // ── KPIs principales ──
    [ObservableProperty] private int _activeExecutions;
    [ObservableProperty] private int _completedExecutions;
    [ObservableProperty] private int _availableJobs;
    [ObservableProperty] private int _unreadNotifications;

    // ── KPIs reputación ──
    [ObservableProperty] private double? _averageRating;
    [ObservableProperty] private double? _completionRate;
    [ObservableProperty] private string _totalHectaresDisplay = "0";

    // ── Listas ──
    public ObservableCollection<DashboardExecutionItem> RecentExecutions { get; } = [];
    public ObservableCollection<DashboardJobItem> NearbyJobs { get; } = [];

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            HasError = false;

            var data = await _dashboard.GetDashboardAsync();
            if (data is null) { HasError = true; return; }

            // Identidad
            DisplayName = data.DisplayName;
            Role = data.Role;
            IsVerified = data.IsVerified;
            IsApplicator = data.Role == "Aplicador";
            IsOperator = data.Role == "Operario";

            // KPIs
            ActiveExecutions = data.ActiveExecutions;
            CompletedExecutions = data.CompletedExecutions;
            AvailableJobs = data.AvailableJobs;
            UnreadNotifications = data.UnreadNotifications;

            // Reputación
            AverageRating = data.AverageRating;
            CompletionRate = data.CompletionRate;
            TotalHectaresDisplay = data.TotalHectares >= 1000
                ? $"{data.TotalHectares / 1000:F1}k"
                : $"{data.TotalHectares:F0}";

            // Listas
            RecentExecutions.Clear();
            foreach (var e in data.RecentActiveExecutions)
                RecentExecutions.Add(e);

            NearbyJobs.Clear();
            foreach (var j in data.NearbyJobs)
                NearbyJobs.Add(j);
        }
        catch (ApiException ex)
        {
            HasError = true;
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            HasError = true;
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToExecutionAsync(DashboardExecutionItem item)
        => await Shell.Current.GoToAsync($"executionDetail?id={item.Id}");

    /// <summary>Navegar al tab ejecuciones con filtro activas pre-seleccionado</summary>
    [RelayCommand]
    private async Task GoToActiveExecutionsAsync()
        => await Shell.Current.GoToAsync("///executions?filter=active");

    /// <summary>Navegar al tab ejecuciones sin filtro (todas)</summary>
    [RelayCommand]
    private async Task GoToExecutionsTabAsync()
        => await Shell.Current.GoToAsync("///executions");

    [RelayCommand]
    private async Task GoToJobsTabAsync()
        => await Shell.Current.GoToAsync("///jobs");
}
