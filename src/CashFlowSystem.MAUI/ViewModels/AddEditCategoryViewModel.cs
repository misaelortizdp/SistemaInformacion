using CashFlowSystem.MAUI.Models;
using CashFlowSystem.MAUI.Services;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace CashFlowSystem.MAUI.ViewModels;

[QueryProperty(nameof(Category), "Category")]
public partial class AddEditCategoryViewModel : ObservableObject
{
    private readonly IApiService _apiService;

    [ObservableProperty]
    private CategoryDto? category;

    [ObservableProperty]
    private string name = string.Empty;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private string type = "Expense";

    [ObservableProperty]
    private string icon = "📁";

    [ObservableProperty]
    private string color = "#512BD4";

    [ObservableProperty]
    private bool isBusy;

    [ObservableProperty]
    private bool isEditMode;

    public List<string> TypeOptions { get; } = new() { "Income", "Expense" };
    public List<string> IconOptions { get; } = new()
    {
        "📁", "🍔", "🏠", "🚗", "💼", "🎮", "📱", "👕",
        "💊", "📚", "✈️", "🎭", "🏋️", "💰", "🎁", "🔧"
    };
    public List<string> ColorOptions { get; } = new()
    {
        "#512BD4", "#FF6B6B", "#4ECDC4", "#45B7D1",
        "#FFA07A", "#98D8C8", "#F7DC6F", "#BB8FCE"
    };

    public AddEditCategoryViewModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    partial void OnCategoryChanged(CategoryDto? value)
    {
        if (value != null)
        {
            IsEditMode = true;
            Name = value.Name;
            Description = value.Description ?? string.Empty;
            Type = value.Type;
            Icon = value.Icon ?? "📁";
            Color = value.Color ?? "#512BD4";
        }
        else
        {
            IsEditMode = false;
        }
    }

    [RelayCommand]
    async Task Save()
    {
        if (string.IsNullOrWhiteSpace(Name))
        {
            await Shell.Current.DisplayAlert("Error", "El nombre es requerido", "OK");
            return;
        }

        try
        {
            IsBusy = true;

            var request = new
            {
                Id = Category?.Id ?? Guid.Empty,
                Name,
                Description = string.IsNullOrWhiteSpace(Description) ? null : Description,
                Type,
                Icon,
                Color
            };

            CategoryDto? result;

            if (IsEditMode && Category != null)
            {
                result = await _apiService.PutAsync<CategoryDto>($"/categories/{Category.Id}", request);
            }
            else
            {
                result = await _apiService.PostAsync<CategoryDto>("/categories", request);
            }

            if (result != null)
            {
                await Shell.Current.DisplayAlert("Éxito",
                    IsEditMode ? "Categoría actualizada" : "Categoría creada", "OK");
                await Shell.Current.GoToAsync("..");
            }
        }
        catch (Exception ex)
        {
            await Shell.Current.DisplayAlert("Error", $"No se pudo guardar: {ex.Message}", "OK");
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    async Task Cancel()
    {
        await Shell.Current.GoToAsync("..");
    }
}
