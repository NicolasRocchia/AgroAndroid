using AgroConnect.Mobile.ViewModels.Recipes;

namespace AgroConnect.Mobile.Views.Recipes;

public partial class RecipeListPage : ContentPage
{
    private readonly RecipeListViewModel _vm;
    public RecipeListPage(RecipeListViewModel vm) { InitializeComponent(); BindingContext = _vm = vm; }
    protected override void OnAppearing() { base.OnAppearing(); if (_vm.Recipes.Count == 0) _vm.LoadRecipesCommand.Execute(null); }
}
