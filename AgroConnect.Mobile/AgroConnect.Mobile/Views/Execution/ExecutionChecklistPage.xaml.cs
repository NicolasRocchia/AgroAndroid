using AgroConnect.Mobile.ViewModels.Execution;

namespace AgroConnect.Mobile.Views.Execution;

public partial class ExecutionChecklistPage : ContentPage
{
    public ExecutionChecklistPage(ExecutionChecklistViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
