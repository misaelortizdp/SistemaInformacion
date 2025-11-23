using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.Collections.ObjectModel;

namespace CashFlowSystem.MAUI.ViewModels;

public partial class CategoriesViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private string filterType = "All";

    public ObservableCollection<CategoryDto> Categories { get; } = new();
    public List<string> FilterOptions { get; } = new() { "All", "Income", "Expense" };

    public CategoriesViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [RelayCommand]
    async Task LoadCategories()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            Categories.Clear();

            var endpoint = FilterType == "All" ? "/categories" : $"/categories?type={FilterType}";
            var categories = await _apiService.GetAsync<List<CategoryDto>>(endpoint);

            if (categories != null)
            {
                foreach (var category in categories)
                {
                    Categories.Add(category);
                }
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudieron cargar las categorías: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task AddCategory()
    {
        await Shell.Current.GoToAsync("addcategory");
    }

    [RelayCommand]
    async Task EditCategory(CategoryDto category)
    {
        if (category == null) return;

        var navigationParameter = new Dictionary<string, object>
        {
            { "Category", category }
        };

        await Shell.Current.GoToAsync("addcategory", navigationParameter);
    }

    [RelayCommand]
    async Task DeleteCategory(CategoryDto category)
    {
        if (category == null) return;

        var confirm = await Shell.Current.DisplayAlert("Confirmar",
            $"¿Eliminar la categoría '{category.Name}'?", "Eliminar", "Cancelar");

        if (!confirm) return;

        try
        {
            IsBusy = true;
            var success = await _apiService.DeleteAsync($"/categories/{category.Id}");

            if (success)
            {
                Categories.Remove(category);
                await Shell.Current.DisplayAlert("Éxito", "Categoría eliminada", "OK");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo eliminar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    partial void OnFilterTypeChanged(string value)
    {
        _ = LoadCategoriesCommand.ExecuteAsync(null);
    }
}
