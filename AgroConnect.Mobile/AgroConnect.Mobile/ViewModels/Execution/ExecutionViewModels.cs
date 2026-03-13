using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.ViewModels.Execution;

public partial class ExecutionListViewModel : ObservableObject
{
    private readonly IExecutionService _executions;

    public ExecutionListViewModel(IExecutionService executions) => _executions = executions;

    public ObservableCollection<ExecutionListDto> Executions { get; } = [];

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isEmpty;
    [ObservableProperty] private string _selectedFilter = "all";

    // Contadores para tabs
    [ObservableProperty] private int _countAll;
    [ObservableProperty] private int _countActive;
    [ObservableProperty] private int _countDone;

    private List<ExecutionListDto> _allExecutions = [];

    private static readonly HashSet<string> ActiveStatuses =
        ["PENDIENTE", "ACEPTADA", "EN_CAMINO", "EN_CURSO", "PAUSADA"];

    private static readonly HashSet<string> DoneStatuses =
        ["COMPLETADA", "COMPLETADA_ADMIN", "CANCELADA"];

    [RelayCommand]
    private async Task LoadExecutionsAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            _allExecutions = await _executions.GetMyExecutionsAsync();

            CountAll = _allExecutions.Count;
            CountActive = _allExecutions.Count(e => ActiveStatuses.Contains(e.Status));
            CountDone = _allExecutions.Count(e => DoneStatuses.Contains(e.Status));

            ApplyFilter();
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private void FilterAll() { SelectedFilter = "all"; ApplyFilter(); }

    [RelayCommand]
    private void FilterActive() { SelectedFilter = "active"; ApplyFilter(); }

    [RelayCommand]
    private void FilterDone() { SelectedFilter = "done"; ApplyFilter(); }

    private void ApplyFilter()
    {
        var filtered = SelectedFilter switch
        {
            "active" => _allExecutions.Where(e => ActiveStatuses.Contains(e.Status)),
            "done" => _allExecutions.Where(e => DoneStatuses.Contains(e.Status)),
            _ => _allExecutions.AsEnumerable()
        };

        Executions.Clear();
        foreach (var item in filtered)
            Executions.Add(item);

        IsEmpty = Executions.Count == 0;
    }

    [RelayCommand]
    private async Task GoToDetailAsync(ExecutionListDto execution)
        => await Shell.Current.GoToAsync($"///executions/detail?id={execution.Id}");
}

// ── Detalle (placeholder para Sprint 2, pero con estructura) ──

[QueryProperty(nameof(ExecutionId), "id")]
public partial class ExecutionDetailViewModel : ObservableObject
{
    private readonly IExecutionService _executions;

    public ExecutionDetailViewModel(IExecutionService executions) => _executions = executions;

    [ObservableProperty] private long _executionId;
    [ObservableProperty] private ExecutionDetailDto? _execution;
    [ObservableProperty] private bool _isBusy;

    partial void OnExecutionIdChanged(long value)
    {
        if (value > 0) LoadCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadAsync()
    {
        if (IsBusy || ExecutionId <= 0) return;
        try
        {
            IsBusy = true;
            Execution = await _executions.GetDetailAsync(ExecutionId);
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}

// ── Checklist (placeholder) ──
public partial class ExecutionChecklistViewModel : ObservableObject { }