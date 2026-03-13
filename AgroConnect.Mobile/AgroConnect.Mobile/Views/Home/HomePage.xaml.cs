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
        if (_vm.LoadCommand.CanExecute(null))
            _vm.LoadCommand.Execute(null);
    }
}
