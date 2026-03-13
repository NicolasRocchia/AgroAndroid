using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.Helpers;

namespace AgroConnect.Mobile.ViewModels.Recipes;

public partial class RecipeListViewModel : ObservableObject
{
    private readonly IRecipeService _recipes;
    private int _currentPage = 1;
    public RecipeListViewModel(IRecipeService recipes) => _recipes = recipes;

    public ObservableCollection<RecipeListItem> Recipes { get; } = [];

    [ObservableProperty] private bool _isBusy;
    [ObservableProperty] private bool _isEmpty;

    [RelayCommand]
    private async Task LoadRecipesAsync()
    {
        if (IsBusy) return;
        try
        {
            IsBusy = true; _currentPage = 1; Recipes.Clear();
            var items = await _recipes.GetMyRecipesAsync(_currentPage, Constants.DefaultPageSize);
            foreach (var item in items) Recipes.Add(item);
            IsEmpty = Recipes.Count == 0;
        }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Error", ex.Message, "OK"); }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsBusy) return;
        try { IsBusy = true; _currentPage++; var items = await _recipes.GetMyRecipesAsync(_currentPage, Constants.DefaultPageSize); foreach (var i in items) Recipes.Add(i); }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task GoToDetailAsync(RecipeListItem recipe)
        => await Shell.Current.GoToAsync($"recipes/detail?code={recipe.Code}");
}

[QueryProperty(nameof(Code), "code")]
public partial class RecipeDetailViewModel : ObservableObject
{
    private readonly IRecipeService _recipes;
    public RecipeDetailViewModel(IRecipeService recipes) => _recipes = recipes;

    [ObservableProperty] private string _code = string.Empty;
    [ObservableProperty] private RecipeDetail? _recipe;
    [ObservableProperty] private bool _isBusy;

    partial void OnCodeChanged(string value) { if (!string.IsNullOrEmpty(value)) LoadRecipeCommand.Execute(null); }

    [RelayCommand]
    private async Task LoadRecipeAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(Code)) return;
        try { IsBusy = true; Recipe = await _recipes.GetRecipeDetailAsync(Code); }
        catch (Exception ex) { await Shell.Current.DisplayAlert("Error", ex.Message, "OK"); }
        finally { IsBusy = false; }
    }

    [RelayCommand]
    private async Task StartExecutionAsync()
    {
        if (Recipe is null) return;
        await Shell.Current.GoToAsync($"execution?recipeId={Recipe.Id}");
    }
}
