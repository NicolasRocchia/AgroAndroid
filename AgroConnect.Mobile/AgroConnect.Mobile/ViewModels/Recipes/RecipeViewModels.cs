using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using AgroConnect.Mobile.Models;
using AgroConnect.Mobile.Services;
using AgroConnect.Mobile.Services.Interfaces;
using AgroConnect.Mobile.Helpers;

namespace AgroConnect.Mobile.ViewModels.Recipes;

public partial class RecipeListViewModel : ObservableObject
{
    private readonly IRecipeService _recipes;
    private int _currentPage = 1;
    private bool _hasMore = true;

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
            IsBusy = true;
            _currentPage = 1;
            _hasMore = true;
            Recipes.Clear();

            var result = await _recipes.GetMyRecipesAsync(_currentPage, Constants.DefaultPageSize);
            foreach (var item in result.Items)
                Recipes.Add(item);

            _hasMore = result.HasNextPage;
            IsEmpty = Recipes.Count == 0;
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadMoreAsync()
    {
        if (IsBusy || !_hasMore) return;
        try
        {
            IsBusy = true;
            _currentPage++;
            var result = await _recipes.GetMyRecipesAsync(_currentPage, Constants.DefaultPageSize);
            foreach (var item in result.Items)
                Recipes.Add(item);
            _hasMore = result.HasNextPage;
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task GoToDetailAsync(RecipeListItem recipe)
        => await Shell.Current.GoToAsync($"recipes/detail?code={recipe.PublicCode}");
}

[QueryProperty(nameof(Code), "code")]
public partial class RecipeDetailViewModel : ObservableObject
{
    private readonly IRecipeService _recipes;
    public RecipeDetailViewModel(IRecipeService recipes) => _recipes = recipes;

    [ObservableProperty] private string _code = string.Empty;
    [ObservableProperty] private RecipeDetail? _recipe;
    [ObservableProperty] private bool _isBusy;

    partial void OnCodeChanged(string value)
    {
        if (!string.IsNullOrEmpty(value))
            LoadRecipeCommand.Execute(null);
    }

    [RelayCommand]
    private async Task LoadRecipeAsync()
    {
        if (IsBusy || string.IsNullOrEmpty(Code)) return;
        try
        {
            IsBusy = true;
            Recipe = await _recipes.GetRecipeDetailAsync(Code);
        }
        catch (ApiException ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", ex.Message, "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task StartExecutionAsync()
    {
        if (Recipe is null) return;
        await Shell.Current.GoToAsync($"execution?recipeId={Recipe.Id}");
    }

    /// <summary>¿Tiene ejecución activa?</summary>
    public bool HasActiveExecution => Recipe?.Executions
        .Any(e => e.Status is "ASIGNADA" or "ACEPTADA" or "EN_CAMINO" or "EN_PROGRESO" or "PAUSADA") ?? false;

    /// <summary>Ejecución activa (si existe)</summary>
    public RecipeExecutionDto? ActiveExecution => Recipe?.Executions
        .FirstOrDefault(e => e.Status is "ASIGNADA" or "ACEPTADA" or "EN_CAMINO" or "EN_PROGRESO" or "PAUSADA");
}
