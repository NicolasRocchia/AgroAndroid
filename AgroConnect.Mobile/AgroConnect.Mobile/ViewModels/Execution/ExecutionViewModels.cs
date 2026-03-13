using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.Helpers;

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
        => await Shell.Current.GoToAsync($"executionDetail?id={execution.Id}");
}

// ══════════════════════════════════════════════════════════════
// DETALLE — State machine UI completa
// ══════════════════════════════════════════════════════════════

[QueryProperty(nameof(ExecutionId), "id")]
public partial class ExecutionDetailViewModel : ObservableObject
{
    private readonly IExecutionService _executions;
    private readonly ILocationService _location;

    public ExecutionDetailViewModel(IExecutionService executions, ILocationService location)
    {
        _executions = executions;
        _location = location;
    }

    // ── Datos base ──
    [ObservableProperty] private long _executionId;
    [ObservableProperty] private ExecutionDetailDto? _execution;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isTransitioning;
    [ObservableProperty] private string _gpsStatus = "";

    // ── State machine: visibilidad de botones ──
    [ObservableProperty] private bool _canAccept;
    [ObservableProperty] private bool _canStartRoute;
    [ObservableProperty] private bool _canStart;       // requiere checklist
    [ObservableProperty] private bool _canPause;
    [ObservableProperty] private bool _canResume;
    [ObservableProperty] private bool _canComplete;
    [ObservableProperty] private bool _canCancel;
    [ObservableProperty] private bool _isActive;       // cualquier estado activo
    [ObservableProperty] private bool _isFinished;     // completada o cancelada

    // ── Labels dinámicos ──
    [ObservableProperty] private string _statusLabel = "";
    [ObservableProperty] private Color _statusColor = Colors.Grey;
    [ObservableProperty] private string _primaryActionText = "";
    [ObservableProperty] private bool _hasPrimaryAction;

    // ── Secciones visibilidad ──
    [ObservableProperty] private bool _hasChecklist;
    [ObservableProperty] private bool _hasReview;
    [ObservableProperty] private bool _hasOperator;
    [ObservableProperty] private bool _hasEvents;
    [ObservableProperty] private bool _hasWeatherStart;
    [ObservableProperty] private bool _hasWeatherEnd;
    [ObservableProperty] private bool _hasNotes;
    [ObservableProperty] private bool _needsChecklist;  // checklist requerido pero no completado

    // ── Timeline ──
    public ObservableCollection<EventTimelineItem> TimelineItems { get; } = [];

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
            if (Execution != null)
                UpdateStateUI();
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo cargar la ejecución: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    /// <summary>Recalcula toda la UI según el estado actual de Execution</summary>
    private void UpdateStateUI()
    {
        var e = Execution;
        if (e is null) return;

        var st = e.Status?.ToUpperInvariant() ?? "";

        // Labels
        StatusLabel = StatusHelper.GetExecutionStatusLabel(st);
        StatusColor = StatusHelper.GetExecutionStatusColor(st);

        // State machine: qué puede hacer el aplicador
        CanAccept = st == "PENDIENTE";
        CanStartRoute = st == "ACEPTADA";
        CanStart = st == "EN_CAMINO";
        CanPause = st == "EN_CURSO";
        CanResume = st == "PAUSADA";
        CanComplete = st == "EN_CURSO";
        CanCancel = st is "PENDIENTE" or "ACEPTADA" or "EN_CAMINO" or "EN_CURSO" or "PAUSADA";
        IsActive = st is "PENDIENTE" or "ACEPTADA" or "EN_CAMINO" or "EN_CURSO" or "PAUSADA";
        IsFinished = st is "COMPLETADA" or "COMPLETADA_ADMIN" or "CANCELADA";

        // Checklist: requerido antes de start, entre EN_CAMINO y EN_CURSO
        NeedsChecklist = st == "EN_CAMINO" && e.Checklist is null;

        // Botón primario (la acción más lógica según estado)
        (HasPrimaryAction, PrimaryActionText) = st switch
        {
            "PENDIENTE" => (true, "✅ Aceptar ejecución"),
            "ACEPTADA" => (true, "🚗 Ir en camino"),
            "EN_CAMINO" when e.Checklist is null => (true, "📋 Completar checklist"),
            "EN_CAMINO" => (true, "▶️ Iniciar aplicación"),
            "EN_CURSO" => (true, "⏸️ Pausar"),
            "PAUSADA" => (true, "▶️ Reanudar"),
            _ => (false, "")
        };

        // Secciones
        HasChecklist = e.Checklist is not null;
        HasReview = e.Review is not null;
        HasOperator = e.Operator is not null;
        HasEvents = e.Events.Count > 0;
        HasWeatherStart = !string.IsNullOrEmpty(e.WeatherAtStart);
        HasWeatherEnd = !string.IsNullOrEmpty(e.WeatherAtEnd);
        HasNotes = !string.IsNullOrEmpty(e.Notes);

        // Timeline
        BuildTimeline();
    }

    private void BuildTimeline()
    {
        TimelineItems.Clear();
        if (Execution?.Events is null) return;

        foreach (var ev in Execution.Events.OrderByDescending(e => e.Timestamp))
        {
            var icon = ev.EventType?.ToUpperInvariant() switch
            {
                "CREADA" => "📝",
                "ACEPTADA" => "✅",
                "EN_CAMINO" => "🚗",
                "CHECKLIST" => "📋",
                "INICIADA" or "EN_CURSO" => "▶️",
                "PAUSADA" => "⏸️",
                "REANUDADA" => "▶️",
                "COMPLETADA" => "🏁",
                "CANCELADA" => "❌",
                _ => "📌"
            };

            var label = ev.EventType?.ToUpperInvariant() switch
            {
                "CREADA" => "Ejecución creada",
                "ACEPTADA" => "Aceptada",
                "EN_CAMINO" => "En camino al lote",
                "CHECKLIST" => "Checklist completado",
                "INICIADA" or "EN_CURSO" => "Aplicación iniciada",
                "PAUSADA" => $"Pausada{(string.IsNullOrEmpty(ev.PauseReason) ? "" : $": {ev.PauseReason}")}",
                "REANUDADA" => "Reanudada",
                "COMPLETADA" => "Aplicación completada",
                "CANCELADA" => "Cancelada",
                _ => ev.EventType ?? "Evento"
            };

            TimelineItems.Add(new EventTimelineItem
            {
                Icon = icon,
                Label = label,
                Timestamp = ev.Timestamp.ToArgentinaDateTimeString(),
                HasGps = ev.GpsLat.HasValue,
                GpsText = ev.GpsLat.HasValue ? $"📍 {ev.GpsLat:F4}, {ev.GpsLng:F4}" : "",
                DistanceText = ev.DistanceToLotKm.HasValue ? $"({ev.DistanceToLotKm:F1} km del lote)" : "",
                Notes = ev.Notes ?? ""
            });
        }
    }

    // ══════════════════════════════════════════════════════════
    // TRANSICIONES — Cada una captura GPS y llama a la API
    // ══════════════════════════════════════════════════════════

    [RelayCommand]
    private async Task PrimaryActionAsync()
    {
        var st = Execution?.Status?.ToUpperInvariant() ?? "";
        switch (st)
        {
            case "PENDIENTE": await AcceptAsync(); break;
            case "ACEPTADA": await StartRouteAsync(); break;
            case "EN_CAMINO" when Execution?.Checklist is null: await GoToChecklistAsync(); break;
            case "EN_CAMINO": await StartAsync(); break;
            case "EN_CURSO": await PauseAsync(); break;
            case "PAUSADA": await ResumeAsync(); break;
        }
    }

    [RelayCommand]
    private async Task AcceptAsync()
    {
        if (!await Confirm("¿Aceptás esta ejecución?")) return;
        await DoTransitionAsync("accept", _executions.AcceptAsync);
    }

    [RelayCommand]
    private async Task StartRouteAsync()
    {
        await DoTransitionAsync("en-route", _executions.StartRouteAsync);
    }

    [RelayCommand]
    private async Task GoToChecklistAsync()
    {
        await Shell.Current.GoToAsync($"executionChecklist?id={ExecutionId}");
    }

    [RelayCommand]
    private async Task StartAsync()
    {
        if (Execution?.Checklist is null)
        {
            await Shell.Current.DisplayAlert("Checklist requerido",
                "Debés completar el checklist pre-aplicación antes de iniciar.", "OK");
            return;
        }
        await DoTransitionAsync("start", _executions.StartAsync);
    }

    [RelayCommand]
    private async Task PauseAsync()
    {
        var reason = await Shell.Current.DisplayPromptAsync(
            "Pausar aplicación", "Motivo de la pausa (opcional):",
            accept: "Pausar", cancel: "Cancelar", maxLength: 200);

        if (reason is null) return; // canceló el prompt

        await DoTransitionAsync("pause", _executions.PauseAsync, pauseReason: reason);
    }

    [RelayCommand]
    private async Task ResumeAsync()
    {
        await DoTransitionAsync("resume", _executions.ResumeAsync);
    }

    [RelayCommand]
    private async Task CompleteAsync()
    {
        if (!await Confirm("¿Confirmar que la aplicación está COMPLETA?")) return;
        await DoTransitionAsync("complete", _executions.CompleteAsync);
    }

    [RelayCommand]
    private async Task CancelAsync()
    {
        var reason = await Shell.Current.DisplayPromptAsync(
            "Cancelar ejecución", "Motivo de la cancelación:",
            accept: "Confirmar cancelación", cancel: "Volver", maxLength: 500);

        if (string.IsNullOrWhiteSpace(reason)) return;

        await DoTransitionAsync("cancel", _executions.CancelAsync, notes: reason);
    }

    /// <summary>
    /// Ejecuta una transición: captura GPS → llama API → refresca UI
    /// </summary>
    private async Task DoTransitionAsync(
        string actionName,
        Func<long, ExecutionTransitionRequest, Task<ExecutionDetailDto?>> apiCall,
        string? pauseReason = null,
        string? notes = null)
    {
        if (IsTransitioning) return;
        try
        {
            IsTransitioning = true;
            GpsStatus = "📍 Obteniendo ubicación...";

            // Capturar GPS
            var gps = await _location.GetCurrentLocationAsync();
            GpsStatus = gps.HasValue ? $"📍 {gps.Value.Latitude:F4}, {gps.Value.Longitude:F4}" : "📍 Sin GPS";

            var request = new ExecutionTransitionRequest
            {
                GpsLat = gps.HasValue ? (decimal)gps.Value.Latitude : null,
                GpsLng = gps.HasValue ? (decimal)gps.Value.Longitude : null,
                PauseReason = pauseReason,
                Notes = notes
            };

            var result = await apiCall(ExecutionId, request);
            if (result != null)
            {
                Execution = result;
                UpdateStateUI();
            }
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error en {actionName}: {ex.Message}", "OK");
        }
        finally
        {
            IsTransitioning = false;
            GpsStatus = "";
        }
    }

    private static async Task<bool> Confirm(string message)
        => await Shell.Current.DisplayAlert("Confirmar", message, "Sí", "No");
}

/// <summary>Item para la timeline visual de eventos</summary>
public class EventTimelineItem
{
    public string Icon { get; set; } = "";
    public string Label { get; set; } = "";
    public string Timestamp { get; set; } = "";
    public bool HasGps { get; set; }
    public string GpsText { get; set; } = "";
    public string DistanceText { get; set; } = "";
    public string Notes { get; set; } = "";
}

// ══════════════════════════════════════════════════════════════
// CHECKLIST — Formulario pre-aplicación
// ══════════════════════════════════════════════════════════════

[QueryProperty(nameof(ExecutionId), "id")]
public partial class ExecutionChecklistViewModel : ObservableObject
{
    private readonly IExecutionService _executions;

    public ExecutionChecklistViewModel(IExecutionService executions) => _executions = executions;

    [ObservableProperty] private long _executionId;
    [ObservableProperty] private bool _isBusy;

    // ── Campos del checklist ──
    [ObservableProperty] private bool _equipmentCalibrated;
    [ObservableProperty] private bool _ppeEquipped;
    [ObservableProperty] private bool _mixturePrepared;
    [ObservableProperty] private bool _exclusionZonesVerified;
    [ObservableProperty] private bool _windConditionsOk;
    [ObservableProperty] private string _customNotes = "";

    /// <summary>Todos los checks obligatorios marcados</summary>
    public bool CanSubmit => EquipmentCalibrated && PpeEquipped
                          && MixturePrepared && ExclusionZonesVerified
                          && WindConditionsOk;

    // Notificar cambio de CanSubmit cuando cambia cualquier check
    partial void OnEquipmentCalibratedChanged(bool value) => OnPropertyChanged(nameof(CanSubmit));
    partial void OnPpeEquippedChanged(bool value) => OnPropertyChanged(nameof(CanSubmit));
    partial void OnMixturePreparedChanged(bool value) => OnPropertyChanged(nameof(CanSubmit));
    partial void OnExclusionZonesVerifiedChanged(bool value) => OnPropertyChanged(nameof(CanSubmit));
    partial void OnWindConditionsOkChanged(bool value) => OnPropertyChanged(nameof(CanSubmit));

    [RelayCommand]
    private async Task SubmitAsync()
    {
        if (!CanSubmit)
        {
            await Shell.Current.DisplayAlert("Checklist incompleto",
                "Debés marcar todos los ítems obligatorios.", "OK");
            return;
        }

        if (IsBusy) return;
        try
        {
            IsBusy = true;
            var request = new SubmitChecklistRequest
            {
                EquipmentCalibrated = EquipmentCalibrated,
                PPEEquipped = PpeEquipped,
                MixturePrepared = MixturePrepared,
                ExclusionZonesVerified = ExclusionZonesVerified,
                WindConditionsOk = WindConditionsOk,
                CustomNotes = string.IsNullOrWhiteSpace(CustomNotes) ? null : CustomNotes.Trim()
            };

            var result = await _executions.SubmitChecklistAsync(ExecutionId, request);
            if (result != null)
            {
                await Shell.Current.DisplayAlert("✅ Checklist enviado",
                    "Ya podés iniciar la aplicación.", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al enviar checklist: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }
}
