using AgroConnect.Mobile.ViewModels.Account;

namespace AgroConnect.Mobile.Views.Account;

public partial class RegisterPage : ContentPage
{
    public RegisterPage(RegisterViewModel vm) { InitializeComponent(); BindingContext = vm; }
}
