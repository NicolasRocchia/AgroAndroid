using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.ViewModels.Account;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    public LoginViewModel(IAuthService auth) => _auth = auth;

    [ObservableProperty] private string _userName = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(UserName) || string.IsNullOrWhiteSpace(Password))
        { ErrorMessage = "Ingresá usuario y contraseña."; return; }

        try
        {
            IsBusy = true; ErrorMessage = null;
            var result = await _auth.LoginAsync(UserName, Password);
            if (result is null) { ErrorMessage = "Credenciales inválidas."; return; }
            await Shell.Current.GoToAsync("//main/recipes");
        }
        catch (HttpRequestException) { ErrorMessage = "Sin conexión al servidor."; }
        catch (Exception ex) { ErrorMessage = $"Error: {ex.Message}"; }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task GoToRegisterAsync() => await Shell.Current.GoToAsync("register");
}

public partial class RegisterViewModel : ObservableObject { }
