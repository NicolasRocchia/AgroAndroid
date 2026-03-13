using AgroConnect.Mobile.ViewModels.Account;

namespace AgroConnect.Mobile.Views.Account;

public partial class RegisterPage : ContentPage
{
    private readonly RegisterViewModel _vm;

    public RegisterPage(RegisterViewModel vm)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        RolePicker.SelectedIndex = 0; // Aplicador por defecto
    }

    private void OnRoleChanged(object? sender, EventArgs e)
    {
        if (RolePicker.SelectedIndex < 0) return;
        _vm.SelectedRoleType = RolePicker.SelectedIndex switch
        {
            0 => "aplicador",
            1 => "productor",
            2 => "ambos",
            _ => "aplicador"
        };
    }
}
