using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.ViewModels.Execution;

namespace AgroConnect.Mobile.Views.Execution;

[QueryProperty(nameof(Filter), "filter")]
public partial class ExecutionListPage : ContentPage
{
    private readonly ExecutionListViewModel _vm;
    private readonly IAuthService _auth;
    private string? _pendingFilter;

    public ExecutionListPage(ExecutionListViewModel vm, IAuthService auth)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _auth = auth;
    }

    /// <summary>Query param "filter" recibido desde navegación (ej: ?filter=active)</summary>
    public string? Filter
    {
        get => _pendingFilter;
        set => _pendingFilter = value;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!await _auth.IsAuthenticatedAsync()) return;

        // Aplicar filtro pendiente si viene de un KPI del Home
        var filterToApply = _pendingFilter;
        _pendingFilter = null; // consumir una sola vez

        if (!string.IsNullOrEmpty(filterToApply))
        {
            _vm.SetFilter(filterToApply);
            UpdateTabStyles(filterToApply);
        }

        // Siempre recargar datos al aparecer (podría haber cambiado algo)
        await _vm.LoadExecutionsCommand.ExecuteAsync(null);

        // Reaplicar filtro visual después de cargar datos
        UpdateTabStyles(_vm.SelectedFilter);
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await _vm.LoadExecutionsCommand.ExecuteAsync(null);
        if (sender is RefreshView rv)
            rv.IsRefreshing = false;
    }

    private void OnFilterAll(object? sender, EventArgs e)
    {
        _vm.FilterAllCommand.Execute(null);
        UpdateTabStyles("all");
    }

    private void OnFilterActive(object? sender, EventArgs e)
    {
        _vm.FilterActiveCommand.Execute(null);
        UpdateTabStyles("active");
    }

    private void OnFilterDone(object? sender, EventArgs e)
    {
        _vm.FilterDoneCommand.Execute(null);
        UpdateTabStyles("done");
    }

    private void UpdateTabStyles(string active)
    {
        var primary = (Color)Application.Current!.Resources["Primary"];
        var white = Colors.White;
        var transparent = Colors.Transparent;

        // Reset all to secondary
        BtnAll.BackgroundColor = transparent;
        BtnAll.TextColor = primary;
        BtnAll.BorderColor = primary;
        BtnAll.BorderWidth = 1;

        BtnActive.BackgroundColor = transparent;
        BtnActive.TextColor = primary;
        BtnActive.BorderColor = primary;
        BtnActive.BorderWidth = 1;

        BtnDone.BackgroundColor = transparent;
        BtnDone.TextColor = primary;
        BtnDone.BorderColor = primary;
        BtnDone.BorderWidth = 1;

        // Set active
        var btn = active switch
        {
            "active" => BtnActive,
            "done" => BtnDone,
            _ => BtnAll
        };
        btn.BackgroundColor = primary;
        btn.TextColor = white;
        btn.BorderWidth = 0;
    }
}
