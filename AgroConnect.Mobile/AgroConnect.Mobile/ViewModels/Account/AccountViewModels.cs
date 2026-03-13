using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.ViewModels.Account;

public partial class LoginViewModel : ObservableObject
{
    private readonly IAuthService _auth;
    public LoginViewModel(IAuthService auth) => _auth = auth;

    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    private async Task LoginAsync()
    {
        if (string.IsNullOrWhiteSpace(Email) || string.IsNullOrWhiteSpace(Password))
        {
            ErrorMessage = "Ingresá tu email y contraseña.";
            return;
        }

        try
        {
            IsBusy = true;
            ErrorMessage = null;

            var result = await _auth.LoginAsync(Email.Trim(), Password);

            if (result is null)
            {
                ErrorMessage = "Credenciales inválidas.";
                return;
            }

            // Login exitoso → ir al tab principal
            await Shell.Current.GoToAsync("//main/jobs");
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (HttpRequestException)
        {
            ErrorMessage = "Sin conexión al servidor.";
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error inesperado: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToRegisterAsync()
        => await Shell.Current.GoToAsync("register");
}

public partial class RegisterViewModel : ObservableObject { }
