using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.ViewModels.Recipes;

namespace AgroConnect.Mobile.Views.Recipes;

public partial class RecipeListPage : ContentPage
{
    private readonly RecipeListViewModel _vm;
    private readonly IAuthService _auth;

    public RecipeListPage(RecipeListViewModel vm, IAuthService auth)
    {
        InitializeComponent();
        BindingContext = _vm = vm;
        _auth = auth;
    }

    protected override async void OnAppearing()
    {
        base.OnAppearing();

        // No cargar datos si no hay sesión activa
        if (!await _auth.IsAuthenticatedAsync()) return;

        if (_vm.Recipes.Count == 0)
            _vm.LoadRecipesCommand.Execute(null);
    }
}
