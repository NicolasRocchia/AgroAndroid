using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.Views.Account;
using AgroConnect.Mobile.Views.Execution;

namespace AgroConnect.Mobile;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    public AppShell(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        // Rutas de detalle (push navigation — nombres planos)
        Routing.RegisterRoute("executionDetail", typeof(ExecutionPage));
        Routing.RegisterRoute("executionChecklist", typeof(ExecutionChecklistPage));
        Routing.RegisterRoute("changePassword", typeof(ChangePasswordPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));

        // Ocultar ambos TabBars al inicio
        ApplicatorTabs.IsVisible = false;
        OperatorTabs.IsVisible = false;
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!await _authService.IsAuthenticatedAsync())
        {
            Dispatcher.Dispatch(async () => await GoToAsync("//login"));
            return;
        }

        await ConfigureTabsByRoleAsync();
    }

    /// <summary>
    /// Muestra los tabs correctos según el rol del usuario.
    /// Aplicador → 4 tabs (Inicio/Trabajos/Ejecuciones/Perfil)
    /// Operario  → 2 tabs (Inicio/Ejecuciones)
    /// </summary>
    public async Task ConfigureTabsByRoleAsync()
    {
        var role = await _authService.GetPrimaryRoleAsync();
        var isOperator = string.Equals(role, "Operario", StringComparison.OrdinalIgnoreCase);

        Dispatcher.Dispatch(() =>
        {
            if (isOperator)
            {
                ApplicatorTabs.IsVisible = false;
                OperatorTabs.IsVisible = true;
            }
            else
            {
                ApplicatorTabs.IsVisible = true;
                OperatorTabs.IsVisible = false;
            }
        });
    }
}
