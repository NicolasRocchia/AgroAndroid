using AgroConnect.Mobile.ViewModels.Home;

namespace AgroConnect.Mobile.Views.Home;

public partial class HomePage : ContentPage
{
    private readonly HomeViewModel _vm;

    public HomePage(HomeViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        if (!_vm.IsBusy)
            _ = _vm.LoadCommand.ExecuteAsync(null);
    }

    private async void OnRefreshing(object? sender, EventArgs e)
    {
        await _vm.LoadCommand.ExecuteAsync(null);
        if (sender is RefreshView rv)
            rv.IsRefreshing = false;
    }
}
