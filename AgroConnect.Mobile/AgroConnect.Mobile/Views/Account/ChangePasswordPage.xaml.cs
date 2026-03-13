using AgroConnect.Mobile.ViewModels.Account;

namespace AgroConnect.Mobile.Views.Account;

public partial class ChangePasswordPage : ContentPage
{
    public ChangePasswordPage(ChangePasswordViewModel vm)
    {
        InitializeComponent();
        BindingContext = vm;
    }
}
