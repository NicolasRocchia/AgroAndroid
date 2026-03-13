using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Models;
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

            // Configurar tabs según rol
            var shell = Shell.Current as AppShell;
            if (shell is not null)
                await shell.ConfigureTabsByRoleAsync();

            await Task.Delay(150);

            var role = await _auth.GetPrimaryRoleAsync();
            var isOperator = string.Equals(role, "Operario", StringComparison.OrdinalIgnoreCase);

            await Shell.Current.GoToAsync(isOperator ? "//operator/home" : "//main/home");
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

// ══════════════════════════════════════════════════════════════
// REGISTRO
// ══════════════════════════════════════════════════════════════

public partial class RegisterViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    public RegisterViewModel(IAuthService auth) => _auth = auth;

    [ObservableProperty] private string _userName = string.Empty;
    [ObservableProperty] private string _email = string.Empty;
    [ObservableProperty] private string _password = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private string _taxId = string.Empty;
    [ObservableProperty] private string _phoneNumber = string.Empty;
    [ObservableProperty] private string _selectedRoleType = "aplicador";
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string? _errorMessage;
    [ObservableProperty] private bool _registrationSuccess;

    [RelayCommand]
    private async Task RegisterAsync()
    {
        ErrorMessage = null;

        // Validaciones locales
        if (string.IsNullOrWhiteSpace(UserName))
        { ErrorMessage = "El nombre es obligatorio."; return; }

        if (string.IsNullOrWhiteSpace(Email) || !Email.Contains('@'))
        { ErrorMessage = "Ingresá un email válido."; return; }

        if (string.IsNullOrWhiteSpace(Password) || Password.Length < 6)
        { ErrorMessage = "La contraseña debe tener al menos 6 caracteres."; return; }

        if (Password != ConfirmPassword)
        { ErrorMessage = "Las contraseñas no coinciden."; return; }

        if (string.IsNullOrWhiteSpace(TaxId) || TaxId.Length < 10)
        { ErrorMessage = "El CUIT/CUIL debe tener 10 u 11 dígitos."; return; }

        try
        {
            IsBusy = true;

            var request = new RegisterRequest
            {
                UserName = UserName.Trim(),
                Email = Email.Trim(),
                Password = Password,
                ConfirmPassword = ConfirmPassword,
                TaxId = TaxId.Trim(),
                PhoneNumber = string.IsNullOrWhiteSpace(PhoneNumber) ? null : PhoneNumber.Trim(),
                RoleType = SelectedRoleType
            };

            var result = await _auth.RegisterAsync(request);
            if (result is not null)
            {
                RegistrationSuccess = true;
                await Shell.Current.DisplayAlert("✅ Cuenta creada",
                    $"Bienvenido {result.UserName}. Ya podés iniciar sesión.", "Ir al login");
                await Shell.Current.GoToAsync("..");
            }
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
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToLoginAsync()
        => await Shell.Current.GoToAsync("..");
}

// ══════════════════════════════════════════════════════════════
// CAMBIAR CONTRASEÑA
// ══════════════════════════════════════════════════════════════

public partial class ChangePasswordViewModel : ObservableObject
{
    private readonly IAuthService _auth;

    public ChangePasswordViewModel(IAuthService auth) => _auth = auth;

    [ObservableProperty] private string _currentPassword = string.Empty;
    [ObservableProperty] private string _newPassword = string.Empty;
    [ObservableProperty] private string _confirmPassword = string.Empty;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    private async Task ChangePasswordAsync()
    {
        ErrorMessage = null;

        if (string.IsNullOrWhiteSpace(CurrentPassword))
        { ErrorMessage = "Ingresá tu contraseña actual."; return; }

        if (string.IsNullOrWhiteSpace(NewPassword) || NewPassword.Length < 6)
        { ErrorMessage = "La nueva contraseña debe tener al menos 6 caracteres."; return; }

        if (NewPassword != ConfirmPassword)
        { ErrorMessage = "Las contraseñas nuevas no coinciden."; return; }

        try
        {
            IsBusy = true;

            var request = new ChangePasswordRequest
            {
                CurrentPassword = CurrentPassword,
                NewPassword = NewPassword,
                ConfirmPassword = ConfirmPassword
            };

            var success = await _auth.ChangePasswordAsync(request);
            if (success)
            {
                await Shell.Current.DisplayAlert("✅ Contraseña actualizada",
                    "Tu contraseña fue cambiada correctamente.", "OK");
                await Shell.Current.GoToAsync("..");
            }
            else
            {
                ErrorMessage = "No se pudo cambiar la contraseña. Verificá tu contraseña actual.";
            }
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsBusy = false;
        }
    }
}
