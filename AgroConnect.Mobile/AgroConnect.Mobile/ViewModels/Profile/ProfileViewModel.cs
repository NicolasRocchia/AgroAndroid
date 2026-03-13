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

    public ProfileViewModel(IApplicatorService applicator, IAuthService auth)
    {
        _applicator = applicator;
        _auth = auth;
    }

    [ObservableProperty] private ApplicatorProfileDto? _profile;
    [ObservableProperty] private LoginResponse? _user;
    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _hasProfile;
    [ObservableProperty] private string? _errorMessage;

    [RelayCommand]
    private async Task LoadProfileAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true;
            ErrorMessage = null;

            User = await _auth.GetCachedLoginAsync();

            Profile = await _applicator.GetMyProfileAsync();
            HasProfile = Profile is not null;
        }
        catch (ApiException ex) when (ex.StatusCode == System.Net.HttpStatusCode.NotFound)
        {
            HasProfile = false;
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

    // ── Computed ──

    public string VerificationText => Profile?.IsVerified == true
        ? "✅ Verificado"
        : "⏳ Pendiente de verificación";

    public string MachinesText => Profile?.MachineTypes is { Count: > 0 }
        ? string.Join(", ", Profile.MachineTypes.Select(MachineLabel))
        : "Sin maquinaria registrada";

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
