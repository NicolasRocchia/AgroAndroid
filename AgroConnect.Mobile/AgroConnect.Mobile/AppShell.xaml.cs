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

        // Rutas de detalle (push navigation)
        Routing.RegisterRoute("executions/detail", typeof(ExecutionPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!await _authService.IsAuthenticatedAsync())
        {
            Dispatcher.Dispatch(async () =>
            {
                await GoToAsync("//login");
            });
        }
    }
}
