using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.ViewModels.Execution;

namespace AgroConnect.Mobile.Views.Execution;

public partial class ExecutionListPage : ContentPage
{
    private readonly ExecutionListViewModel _vm;
    private readonly IAuthService _auth;

    public ExecutionListPage(ExecutionListViewModel vm, IAuthService auth)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!await _auth.IsAuthenticatedAsync()) return;
        if (_vm.Executions.Count == 0)
            _vm.LoadExecutionsCommand.Execute(null);
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
