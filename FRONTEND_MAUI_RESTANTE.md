# Frontend MAUI - Implementación Restante

## Estado Actual

### ✅ COMPLETADO
1. **CRUD Categorías** - 100% implementado
   - CategoriesViewModel.cs
   - AddEditCategoryViewModel.cs
   - CategoriesPage.xaml
   - AddEditCategoryPage.xaml
   - Funcionalidades: Listar, filtrar, crear, editar, eliminar
   - UI: Swipe-to-delete, selector de íconos/colores

### ⏳ PENDIENTE DE IMPLEMENTAR

## 2. CRUD Métodos de Pago

**Archivos a crear** (muy similar a Categorías):

### PaymentMethodsViewModel.cs
```csharp
public partial class PaymentMethodsViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    [ObservableProperty] private bool isBusy;

    public ObservableCollection<PaymentMethodDto> PaymentMethods { get; } = new();

    [RelayCommand] async Task LoadPaymentMethods()
    [RelayCommand] async Task AddPaymentMethod()
    [RelayCommand] async Task EditPaymentMethod(PaymentMethodDto pm)
    [RelayCommand] async Task DeletePaymentMethod(PaymentMethodDto pm)
}
```

### AddEditPaymentMethodViewModel.cs
```csharp
public partial class AddEditPaymentMethodViewModel : ObservableObject
{
    [ObservableProperty] private string name = string.Empty;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private string type = "Cash";

    public List<string> TypeOptions { get; } = new() { "Cash", "Card", "BankTransfer", "Other" };

    [RelayCommand] async Task Save()
    [RelayCommand] async Task Cancel()
}
```

### PaymentMethodsPage.xaml
```xml
Similar estructura a CategoriesPage con:
- Lista de métodos de pago
- SwipeView para eliminar
- Filtros por tipo
- Botón "Nueva Método de Pago"
```

---

## 3. Editar Transacciones

### EditTransactionViewModel.cs
```csharp
[QueryProperty(nameof(Transaction), "Transaction")]
public partial class EditTransactionViewModel : ObservableObject
{
    [ObservableProperty] private TransactionDto? transaction;
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private string description = string.Empty;
    [ObservableProperty] private DateTime date = DateTime.Now;
    [ObservableProperty] private string type = "Expense";
    [ObservableProperty] private CategoryDto? selectedCategory;
    [ObservableProperty] private PaymentMethodDto? selectedPaymentMethod;
    [ObservableProperty] private string referenceNumber = string.Empty;
    [ObservableProperty] private string notes = string.Empty;

    public ObservableCollection<CategoryDto> Categories { get; } = new();
    public ObservableCollection<PaymentMethodDto> PaymentMethods { get; } = new();

    [RelayCommand]
    async Task LoadData()
    {
        // Cargar categorías y métodos de pago
        // Popular campos si estamos editando
    }

    [RelayCommand]
    async Task UpdateTransaction()
    {
        var request = new
        {
            Id = Transaction.Id,
            Amount,
            Description,
            Date,
            Type,
            CategoryId = SelectedCategory.Id,
            PaymentMethodId = SelectedPaymentMethod.Id,
            ReferenceNumber,
            Notes
        };

        var result = await _apiService.PutAsync<TransactionDto>($"/transactions/{Transaction.Id}", request);
        // Navigate back
    }
}
```

### EditTransactionPage.xaml
```xml
<ContentPage Title="Editar Transacción">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="20">
            <!-- Mismos campos que AddTransactionPage pero con datos prellenados -->
            <!-- Tipo, Monto, Descripción, Fecha, Categoría, Método de Pago -->
            <Grid ColumnDefinitions="*,*" ColumnSpacing="15">
                <Button Text="Cancelar" Command="{Binding CancelCommand}"/>
                <Button Text="Actualizar" Command="{Binding UpdateTransactionCommand}"/>
            </Grid>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

**Integración**: Modificar TransactionsPage para navegar a EditTransaction al hacer tap en una transacción.

---

## 4. Búsqueda Avanzada

### AdvancedSearchViewModel.cs
```csharp
public partial class AdvancedSearchViewModel : ObservableObject
{
    [ObservableProperty] private string searchText = string.Empty;
    [ObservableProperty] private DateTime? startDate;
    [ObservableProperty] private DateTime? endDate;
    [ObservableProperty] private decimal? minAmount;
    [ObservableProperty] private decimal? maxAmount;
    [ObservableProperty] private string orderBy = "date";
    [ObservableProperty] private bool descending = true;

    public ObservableCollection<TransactionDto> SearchResults { get; } = new();
    public ObservableCollection<CategoryDto> SelectedCategories { get; } = new();
    public ObservableCollection<PaymentMethodDto> SelectedPaymentMethods { get; } = new();

    public List<string> OrderByOptions { get; } = new() { "date", "amount", "description" };

    [RelayCommand]
    async Task Search()
    {
        var request = new
        {
            SearchText,
            StartDate,
            EndDate,
            MinAmount,
            MaxAmount,
            CategoryIds = SelectedCategories.Select(c => c.Id).ToList(),
            PaymentMethodIds = SelectedPaymentMethods.Select(p => p.Id).ToList(),
            OrderBy,
            Descending,
            Take = 50
        };

        var results = await _apiService.PostAsync<List<TransactionDto>>("/transactions/search", request);
        // Popular SearchResults
    }

    [RelayCommand]
    async Task ClearFilters()
    {
        SearchText = string.Empty;
        StartDate = null;
        EndDate = null;
        MinAmount = null;
        MaxAmount = null;
        SelectedCategories.Clear();
        SelectedPaymentMethods.Clear();
    }
}
```

### AdvancedSearchPage.xaml
```xml
<ContentPage Title="Búsqueda Avanzada">
    <Grid RowDefinitions="Auto,*">
        <!-- Filters Section -->
        <ScrollView Grid.Row="0" MaximumHeightRequest="400">
            <VerticalStackLayout Padding="20" Spacing="15">
                <!-- Búsqueda por texto -->
                <Entry Placeholder="Buscar descripción, notas..." Text="{Binding SearchText}"/>

                <!-- Rango de fechas -->
                <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                    <DatePicker Grid.Column="0" Date="{Binding StartDate}"/>
                    <DatePicker Grid.Column="1" Date="{Binding EndDate}"/>
                </Grid>

                <!-- Rango de montos con Sliders -->
                <VerticalStackLayout>
                    <Label Text="Monto mínimo"/>
                    <Slider Minimum="0" Maximum="10000" Value="{Binding MinAmount}"/>
                    <Label Text="Monto máximo"/>
                    <Slider Minimum="0" Maximum="10000" Value="{Binding MaxAmount}"/>
                </VerticalStackLayout>

                <!-- Ordenamiento -->
                <Picker ItemsSource="{Binding OrderByOptions}" SelectedItem="{Binding OrderBy}"/>

                <Grid ColumnDefinitions="*,*" ColumnSpacing="10">
                    <Button Text="Buscar" Command="{Binding SearchCommand}"/>
                    <Button Text="Limpiar" Command="{Binding ClearFiltersCommand}"/>
                </Grid>
            </VerticalStackLayout>
        </ScrollView>

        <!-- Results List -->
        <CollectionView Grid.Row="1" ItemsSource="{Binding SearchResults}">
            <!-- Similar template a TransactionsPage -->
        </CollectionView>
    </Grid>
</ContentPage>
```

---

## 5. Presupuestos

### BudgetsViewModel.cs
```csharp
public partial class BudgetsViewModel : ObservableObject
{
    private readonly IApiService _apiService;
    private readonly IAuthService _authService;

    [ObservableProperty] private bool isBusy;

    public ObservableCollection<BudgetDto> Budgets { get; } = new();

    [RelayCommand]
    async Task LoadBudgets()
    {
        var user = await _authService.GetCurrentUserAsync();
        var budgets = await _apiService.GetAsync<List<BudgetDto>>($"/budgets?userId={user.Id}");
        // Popular Budgets
    }

    [RelayCommand]
    async Task AddBudget()
    {
        await Shell.Current.GoToAsync("addbudget");
    }

    [RelayCommand]
    async Task EditBudget(BudgetDto budget)
    {
        await Shell.Current.GoToAsync("addbudget", new Dictionary<string, object> { { "Budget", budget } });
    }

    [RelayCommand]
    async Task DeleteBudget(BudgetDto budget)
    {
        // Confirm y eliminar
    }
}
```

### AddEditBudgetViewModel.cs
```csharp
public partial class AddEditBudgetViewModel : ObservableObject
{
    [ObservableProperty] private BudgetDto? budget;
    [ObservableProperty] private CategoryDto? selectedCategory;
    [ObservableProperty] private decimal amount;
    [ObservableProperty] private DateTime startDate = DateTime.Now;
    [ObservableProperty] private DateTime endDate = DateTime.Now.AddMonths(1);
    [ObservableProperty] private string description = string.Empty;

    public ObservableCollection<CategoryDto> Categories { get; } = new();

    [RelayCommand]
    async Task Save()
    {
        var user = await _authService.GetCurrentUserAsync();
        var request = new
        {
            UserId = user.Id,
            CategoryId = SelectedCategory.Id,
            Amount,
            StartDate,
            EndDate,
            Description
        };

        if (IsEditMode)
            await _apiService.PutAsync<BudgetDto>($"/budgets/{Budget.Id}", request);
        else
            await _apiService.PostAsync<BudgetDto>("/budgets", request);
    }
}
```

### BudgetsPage.xaml
```xml
<ContentPage Title="Presupuestos">
    <Grid RowDefinitions="*,Auto">
        <CollectionView Grid.Row="0" ItemsSource="{Binding Budgets}">
            <CollectionView.ItemTemplate>
                <DataTemplate x:DataType="models:BudgetDto">
                    <Border Padding="15" Margin="10,5">
                        <VerticalStackLayout Spacing="10">
                            <Label Text="{Binding CategoryName}" FontSize="18" FontAttributes="Bold"/>
                            <Label Text="{Binding Description}"/>

                            <!-- Progress Bar -->
                            <ProgressBar Progress="{Binding PercentageUsed}"
                                        ProgressColor="{Binding PercentageUsed, Converter={StaticResource PercentageToColorConverter}}"/>

                            <Grid ColumnDefinitions="*,*,*">
                                <VerticalStackLayout Grid.Column="0">
                                    <Label Text="Presupuesto" FontSize="12"/>
                                    <Label Text="{Binding Amount, StringFormat='${0:N2}'}" FontAttributes="Bold"/>
                                </VerticalStackLayout>
                                <VerticalStackLayout Grid.Column="1">
                                    <Label Text="Gastado" FontSize="12"/>
                                    <Label Text="{Binding SpentAmount, StringFormat='${0:N2}'}" TextColor="{StaticResource Error}"/>
                                </VerticalStackLayout>
                                <VerticalStackLayout Grid.Column="2">
                                    <Label Text="Restante" FontSize="12"/>
                                    <Label Text="{Binding RemainingAmount, StringFormat='${0:N2}'}" TextColor="{StaticResource Success}"/>
                                </VerticalStackLayout>
                            </Grid>

                            <!-- Alert si está cerca del límite -->
                            <Label Text="⚠️ Cerca del límite!"
                                   IsVisible="{Binding PercentageUsed, Converter={StaticResource PercentageToWarningConverter}}"
                                   TextColor="{StaticResource Warning}"/>
                        </VerticalStackLayout>
                    </Border>
                </DataTemplate>
            </CollectionView.ItemTemplate>
        </CollectionView>

        <Button Grid.Row="1" Text="+ Nuevo Presupuesto" Command="{Binding AddBudgetCommand}"/>
    </Grid>
</ContentPage>
```

---

## 6. Gráficos con LiveCharts

### Instalar paquete NuGet:
```xml
<PackageReference Include="LiveChartsCore.SkiaSharpView.Maui" Version="2.0.0-rc2" />
```

### ChartsViewModel.cs
```csharp
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;

public partial class ChartsViewModel : ObservableObject
{
    [ObservableProperty] private ISeries[] categorySeries = Array.Empty<ISeries>();
    [ObservableProperty] private ISeries[] trendSeries = Array.Empty<ISeries>();

    [RelayCommand]
    async Task LoadCharts()
    {
        // Cargar datos del dashboard
        var dashboard = await _apiService.GetAsync<DashboardDto>("/dashboard?...");

        // Gráfico de pastel - Gastos por categoría
        CategorySeries = new ISeries[]
        {
            new PieSeries<decimal> { Values = new[] { 450 }, Name = "Alimentación" },
            new PieSeries<decimal> { Values = new[] { 300 }, Name = "Transporte" },
            new PieSeries<decimal> { Values = new[] { 200 }, Name = "Entretenimiento" }
        };

        // Gráfico de líneas - Tendencia mensual
        TrendSeries = new ISeries[]
        {
            new LineSeries<decimal>
            {
                Values = new[] { 1000m, 1200m, 1100m, 1300m, 1250m, 1400m },
                Name = "Ingresos"
            },
            new LineSeries<decimal>
            {
                Values = new[] { 800m, 900m, 850m, 950m, 900m, 1000m },
                Name = "Egresos"
            }
        };
    }
}
```

### ChartsPage.xaml
```xml
<?xml version="1.0" encoding="utf-8" ?>
<ContentPage xmlns:lvc="clr-namespace:LiveChartsCore.SkiaSharpView.Maui;assembly=LiveChartsCore.SkiaSharpView.Maui">
    <ScrollView>
        <VerticalStackLayout Padding="20" Spacing="30">
            <Label Text="Análisis Visual" FontSize="24" FontAttributes="Bold"/>

            <!-- Gráfico de Pastel -->
            <VerticalStackLayout Spacing="10">
                <Label Text="Gastos por Categoría" FontSize="18" FontAttributes="Bold"/>
                <lvc:PieChart Series="{Binding CategorySeries}" HeightRequest="300"/>
            </VerticalStackLayout>

            <!-- Gráfico de Líneas -->
            <VerticalStackLayout Spacing="10">
                <Label Text="Tendencia Mensual" FontSize="18" FontAttributes="Bold"/>
                <lvc:CartesianChart Series="{Binding TrendSeries}" HeightRequest="300"/>
            </VerticalStackLayout>

            <!-- Gráfico de Barras -->
            <VerticalStackLayout Spacing="10">
                <Label Text="Comparativa Ingresos vs Egresos" FontSize="18" FontAttributes="Bold"/>
                <lvc:CartesianChart Series="{Binding ComparativeSeries}" HeightRequest="300"/>
            </VerticalStackLayout>
        </VerticalStackLayout>
    </ScrollView>
</ContentPage>
```

---

## 7. Converters Adicionales Necesarios

### BoolToTextConverter.cs
```csharp
public class BoolToTextConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is bool boolValue && parameter is string options)
        {
            var parts = options.Split('|');
            return boolValue ? parts[0] : parts[1];
        }
        return string.Empty;
    }
}
```

### PercentageToColorConverter.cs
```csharp
public class PercentageToColorConverter : IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        if (value is decimal percentage)
        {
            return percentage switch
            {
                >= 100 => Colors.Red,
                >= 90 => Colors.Orange,
                >= 75 => Colors.Yellow,
                _ => Colors.Green
            };
        }
        return Colors.Gray;
    }
}
```

---

## 8. Actualizar MauiProgram.cs

Registrar todos los ViewModels y Views:

```csharp
// ViewModels
builder.Services.AddTransient<CategoriesViewModel>();
builder.Services.AddTransient<AddEditCategoryViewModel>();
builder.Services.AddTransient<PaymentMethodsViewModel>();
builder.Services.AddTransient<AddEditPaymentMethodViewModel>();
builder.Services.AddTransient<EditTransactionViewModel>();
builder.Services.AddTransient<AdvancedSearchViewModel>();
builder.Services.AddTransient<BudgetsViewModel>();
builder.Services.AddTransient<AddEditBudgetViewModel>();
builder.Services.AddTransient<ChartsViewModel>();

// Views
builder.Services.AddTransient<CategoriesPage>();
builder.Services.AddTransient<AddEditCategoryPage>();
builder.Services.AddTransient<PaymentMethodsPage>();
builder.Services.AddTransient<AddEditPaymentMethodPage>();
builder.Services.AddTransient<EditTransactionPage>();
builder.Services.AddTransient<AdvancedSearchPage>();
builder.Services.AddTransient<BudgetsPage>();
builder.Services.AddTransient<AddEditBudgetPage>();
builder.Services.AddTransient<ChartsPage>();
```

---

## 9. Actualizar AppShell.xaml

Agregar rutas de navegación:

```xml
<Shell>
    <ShellContent Route="login" ContentTemplate="{DataTemplate views:LoginPage}" />

    <TabBar Route="main">
        <ShellContent Route="dashboard" Title="Dashboard" Icon="home.png" ContentTemplate="{DataTemplate views:DashboardPage}" />
        <ShellContent Route="transactions" Title="Transacciones" Icon="list.png" ContentTemplate="{DataTemplate views:TransactionsPage}" />
        <ShellContent Route="reports" Title="Informes" Icon="chart.png" ContentTemplate="{DataTemplate views:ReportsPage}" />
        <ShellContent Route="budgets" Title="Presupuestos" Icon="budget.png" ContentTemplate="{DataTemplate views:BudgetsPage}" />
        <ShellContent Route="categories" Title="Categorías" Icon="folder.png" ContentTemplate="{DataTemplate views:CategoriesPage}" />
        <ShellContent Route="cashregister" Title="Caja" Icon="cash.png" ContentTemplate="{DataTemplate views:CashRegisterPage}" />
    </TabBar>
</Shell>
```

Registrar rutas modales en AppShell.xaml.cs:

```csharp
Routing.RegisterRoute("addtransaction", typeof(AddTransactionPage));
Routing.RegisterRoute("edittransaction", typeof(EditTransactionPage));
Routing.RegisterRoute("addcategory", typeof(AddEditCategoryPage));
Routing.RegisterRoute("addpaymentmethod", typeof(AddEditPaymentMethodPage));
Routing.RegisterRoute("advancedsearch", typeof(AdvancedSearchPage));
Routing.RegisterRoute("addbudget", typeof(AddEditBudgetPage));
Routing.RegisterRoute("charts", typeof(ChartsPage));
```

---

## 10. Actualizar IApiService

Agregar método DELETE:

```csharp
public interface IApiService
{
    // Existentes
    Task<T?> GetAsync<T>(string endpoint);
    Task<T?> PostAsync<T>(string endpoint, object data);
    Task<T?> PutAsync<T>(string endpoint, object data);

    // Nuevo
    Task<bool> DeleteAsync(string endpoint);
}

// ApiService.cs
public async Task<bool> DeleteAsync(string endpoint)
{
    try
    {
        var response = await _httpClient.DeleteAsync($"{BaseUrl}{endpoint}");
        return response.IsSuccessStatusCode;
    }
    catch
    {
        return false;
    }
}
```

---

## Resumen de Archivos a Crear

| Funcionalidad | ViewModels | Views | Total |
|---------------|------------|-------|-------|
| ✅ Categorías | 2 | 2 | 4 |
| ⏳ Métodos Pago | 2 | 2 | 4 |
| ⏳ Editar Transaction | 1 | 1 | 2 |
| ⏳ Búsqueda Avanzada | 1 | 1 | 2 |
| ⏳ Presupuestos | 2 | 2 | 4 |
| ⏳ Gráficos | 1 | 1 | 2 |
| **TOTAL** | **9** | **9** | **18** |

**Archivos adicionales**:
- 3 Converters
- Actualizar MauiProgram.cs
- Actualizar AppShell.xaml
- Actualizar IApiService

---

## Próximos Pasos

1. ✅ CRUD Categorías (COMPLETADO)
2. ⏳ Implementar archivos restantes (16-20 horas)
3. ⏳ Testing de navegación y funcionalidades
4. ⏳ Agregar íconos a Resources
5. ⏳ Compilar y probar en Android/iOS/Windows

**Estimación**: El CRUD de Categorías representa ~25% del trabajo frontend total. Faltan ~15 archivos MAUI más converters y configuración.
