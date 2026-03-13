using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;

namespace AgroConnect.Mobile.ViewModels.Profile;

public partial class ProfileViewModel : ObservableObject
{
    private readonly IApplicatorService _applicator;
    private readonly IAuthService _auth;
    private readonly IOperatorService _operators;

    public ProfileViewModel(IApplicatorService applicator, IAuthService auth, IOperatorService operators)
    {
        _applicator = applicator;
        _auth = auth;
        _operators = operators;
    }

    [ObservableProperty] private ApplicatorProfileDto? _profile;
    [ObservableProperty] private LoginResponse? _user;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _hasProfile;
    [ObservableProperty] private string? _errorMessage;

    // ── Computed (notificados manualmente) ──
    [ObservableProperty] private string _verificationText = "";
    [ObservableProperty] private string _machinesText = "";

    // ── Operarios ──
    public ObservableCollection<OperatorListDto> Operators { get; } = [];
    [ObservableProperty] private bool _hasOperators;
    [ObservableProperty] private bool _isLoadingOperators;

    partial void OnProfileChanged(ApplicatorProfileDto? value)
    {
        // Recalcular computed properties cuando cambia Profile
        VerificationText = value?.IsVerified == true
            ? "✅ Verificado"
            : "⏳ Pendiente de verificación";

        MachinesText = value?.MachineTypes is { Count: > 0 }
            ? string.Join(", ", value.MachineTypes.Select(MachineLabel))
            : "Sin maquinaria registrada";
    }

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            ErrorMessage = null;

            User = await _auth.GetCachedLoginAsync();

            try
            {
                Profile = await _applicator.GetMyProfileAsync();
                HasProfile = Profile is not null;
            }
            catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                HasProfile = false;
            }

            // Cargar operarios si tiene perfil de aplicador
            if (HasProfile)
                await LoadOperatorsAsync();
        }
        catch (ApiException ex)
        {
            ErrorMessage = ex.Message;
        }
        catch (Exception ex)
        {
            ErrorMessage = ex.Message;
        }
        finally
        {
            IsBusy = false;
        }
    }

    private async Task LoadOperatorsAsync()
    {
        try
        {
            IsLoadingOperators = true;
            var ops = await _operators.GetMyOperatorsAsync();
            Operators.Clear();
            foreach (var op in ops)
                Operators.Add(op);
            HasOperators = Operators.Count > 0;
        }
        catch
        {
            // Si falla (no tiene perfil, etc.) no mostrar error — simplemente no hay operarios
            HasOperators = false;
        }
        finally
        {
            IsLoadingOperators = false;
        }
    }

    [RelayCommand]
    private async Task CreateOperatorAsync()
    {
        var name = await Shell.Current.DisplayPromptAsync(
            "Nuevo operario", "Nombre completo:",
            accept: "Siguiente", cancel: "Cancelar", maxLength: 150);
        if (string.IsNullOrWhiteSpace(name)) return;

        var email = await Shell.Current.DisplayPromptAsync(
            "Nuevo operario", "Email del operario:",
            accept: "Siguiente", cancel: "Cancelar", maxLength: 200, keyboard: Keyboard.Email);
        if (string.IsNullOrWhiteSpace(email)) return;

        var phone = await Shell.Current.DisplayPromptAsync(
            "Nuevo operario", "Teléfono (opcional):",
            accept: "Crear", cancel: "Cancelar", maxLength: 30, keyboard: Keyboard.Telephone);
        if (phone is null) return; // canceló

        try
        {
            var result = await _operators.CreateAsync(new CreateOperatorRequest
            {
                Name = name.Trim(),
                Email = email.Trim(),
                Phone = string.IsNullOrWhiteSpace(phone) ? null : phone.Trim()
            });

            if (result?.OperatorProfile is not null)
            {
                await Shell.Current.DisplayAlert("✅ Operario creado",
                    $"Se creó la cuenta para {result.OperatorProfile.Name}.\n\n" +
                    $"📧 Email: {result.OperatorProfile.Email}\n" +
                    $"🔑 Contraseña temporal: {result.TempPassword}\n\n" +
                    "El operario debe cambiar su contraseña al iniciar sesión.",
                    "Entendido");

                // Refrescar lista
                await LoadOperatorsAsync();
            }
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"Error al crear operario: {ex.Message}", "OK");
        }
    }

    [RelayCommand]
    private async Task DeactivateOperatorAsync(OperatorListDto op)
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Desactivar operario",
            $"¿Desactivar a {op.Name}? No podrá recibir nuevas ejecuciones.",
            "Desactivar", "Cancelar");

        if (!confirm) return;

        try
        {
            await _operators.DeactivateAsync(op.Id);
            await LoadOperatorsAsync();
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
    }

    [RelayCommand]
    private async Task LogoutAsync()
    {
        var confirm = await Shell.Current.DisplayAlert(
            "Cerrar sesión", "¿Estás seguro?", "Sí", "Cancelar");

        if (!confirm) return;

        await _auth.LogoutAsync();
        await Shell.Current.GoToAsync("//login");
    }

    [RelayCommand]
    private async Task GoToChangePasswordAsync()
        => await Shell.Current.GoToAsync("changePassword");

    private static string MachineLabel(string type) => type.ToLowerInvariant() switch
    {
        "drone" => "🛸 Drone",
        "mosquito" => "🦟 Mosquito",
        "arrastre" => "🚜 Arrastre",
        "autopropulsada" => "🚛 Autopropulsada",
        "avion" => "✈️ Avión",
        _ => type
    };
}
