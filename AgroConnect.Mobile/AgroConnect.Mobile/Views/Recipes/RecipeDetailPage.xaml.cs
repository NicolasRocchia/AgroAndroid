using AgroConnect.Mobile.ViewModels.Recipes;

namespace AgroConnect.Mobile.Views.Recipes;

public partial class RecipeDetailPage : ContentPage
{
    public RecipeDetailPage(RecipeDetailViewModel vm) { InitializeComponent(); BindingContext = vm; }
}
