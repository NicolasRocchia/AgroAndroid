using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.Views.Account;
using AgroConnect.Mobile.Views.Recipes;
using AgroConnect.Mobile.Views.Execution;
using AgroConnect.Mobile.Views.Lots;

namespace AgroConnect.Mobile;

public partial class AppShell : Shell
{
    private readonly IAuthService _authService;

    public AppShell(IAuthService authService)
    {
        InitializeComponent();
        _authService = authService;

        Routing.RegisterRoute("recipes/detail", typeof(RecipeDetailPage));
        Routing.RegisterRoute("execution", typeof(ExecutionPage));
        Routing.RegisterRoute("execution/checklist", typeof(ExecutionChecklistPage));
        Routing.RegisterRoute("lots/detail", typeof(LotDetailPage));
        Routing.RegisterRoute("login", typeof(LoginPage));
        Routing.RegisterRoute("register", typeof(RegisterPage));
    }

    protected override async void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        if (!await _authService.IsAuthenticatedAsync())
        {
            await GoToAsync("//login");
        }
    }
}
