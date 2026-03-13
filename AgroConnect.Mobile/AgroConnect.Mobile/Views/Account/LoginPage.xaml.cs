using AgroConnect.Mobile.ViewModels.Account;

namespace AgroConnect.Mobile.Views.Account;

public partial class LoginPage : ContentPage
{
    public LoginPage(LoginViewModel vm) { InitializeComponent(); BindingContext = vm; }
}
