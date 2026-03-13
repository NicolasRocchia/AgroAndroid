using AgroConnect.Mobile.ViewModels.Execution;

namespace AgroConnect.Mobile.Views.Execution;

public partial class ExecutionPage : ContentPage
{
    private readonly ExecutionDetailViewModel _vm;

    public ExecutionPage(ExecutionDetailViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
    }

    protected override void OnAppearing()
    {
        base.OnAppearing();
        // Recargar al volver (ej: después del checklist)
        if (_vm.ExecutionId > 0)
            _vm.LoadCommand.Execute(null);
    }
}
