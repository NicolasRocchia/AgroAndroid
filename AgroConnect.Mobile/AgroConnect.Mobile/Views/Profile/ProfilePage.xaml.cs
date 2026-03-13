using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.ViewModels.Profile;

namespace AgroConnect.Mobile.Views.Profile;

public partial class ProfilePage : ContentPage
{
    private readonly ProfileViewModel _vm;
    private readonly IAuthService _auth;

    public ProfilePage(ProfileViewModel vm, IAuthService auth)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _auth = auth;

        _vm.PropertyChanged += (_, args) =>
        {
            if (args.PropertyName == nameof(ProfileViewModel.Profile))
                UpdateVerificationBadge();
        };
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();
        if (!await _auth.IsAuthenticatedAsync()) return;
        _vm.LoadProfileCommand.Execute(null);
    }

    private void UpdateVerificationBadge()
    {
        if (_vm.Profile is null) return;

        if (_vm.Profile.IsVerified)
        {
            LblVerification.Text = "✅ Verificado";
            LblVerification.TextColor = Color.FromArgb("#166534");
        }
        else
        {
            LblVerification.Text = "⏳ Pendiente de verificación";
            LblVerification.TextColor = Color.FromArgb("#854D0E");
        }
    }
}
