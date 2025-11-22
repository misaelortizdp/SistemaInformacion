# Mejores Prácticas - CashFlowSystem

## 📌 Introducción

Este documento establece las mejores prácticas y convenciones de código para el proyecto CashFlowSystem.

## 🎯 Principios SOLID

### Single Responsibility Principle (SRP)
```csharp
// ❌ MAL - Clase con múltiples responsabilidades
public class TransactionService
{
    public void CreateTransaction() { }
    public void SendEmail() { }
    public void GeneratePDF() { }
    public void ValidateData() { }
}

// ✅ BIEN - Cada clase tiene una responsabilidad
public class TransactionService
{
    public void CreateTransaction() { }
}

public class EmailService
{
    public void SendEmail() { }
}

public class PdfGenerator
{
    public void GeneratePDF() { }
}
```

### Open/Closed Principle (OCP)
```csharp
// ✅ BIEN - Abierto para extensión, cerrado para modificación
public interface IExportService
{
    Task<byte[]> Export(IEnumerable<Transaction> data);
}

public class PdfExportService : IExportService
{
    public async Task<byte[]> Export(IEnumerable<Transaction> data)
    {
        // Implementación PDF
    }
}

public class ExcelExportService : IExportService
{
    public async Task<byte[]> Export(IEnumerable<Transaction> data)
    {
        // Implementación Excel
    }
}
```

### Liskov Substitution Principle (LSP)
```csharp
// ✅ BIEN - Las clases derivadas pueden sustituir a la base
public abstract class BaseRepository<T> where T : BaseEntity
{
    public virtual async Task<T> GetByIdAsync(Guid id)
    {
        // Implementación base
    }
}

public class TransactionRepository : BaseRepository<Transaction>
{
    // Mantiene el contrato de la clase base
    public override async Task<Transaction> GetByIdAsync(Guid id)
    {
        // Puede agregar funcionalidad pero respeta el contrato
        return await base.GetByIdAsync(id);
    }
}
```

### Interface Segregation Principle (ISP)
```csharp
// ❌ MAL - Interfaz muy grande
public interface IRepository
{
    Task Create();
    Task Update();
    Task Delete();
    Task Export();
    Task Import();
    Task Backup();
}

// ✅ BIEN - Interfaces específicas
public interface IRepository<T>
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
}

public interface IExportable
{
    Task ExportAsync();
}

public interface IImportable
{
    Task ImportAsync();
}
```

### Dependency Inversion Principle (DIP)
```csharp
// ✅ BIEN - Depender de abstracciones, no de implementaciones
public class TransactionService
{
    private readonly IRepository<Transaction> _repository;
    private readonly IEmailService _emailService;

    public TransactionService(
        IRepository<Transaction> repository,
        IEmailService emailService)
    {
        _repository = repository;
        _emailService = emailService;
    }
}
```

## 🏗️ Clean Architecture

### Dependencias de Capas

```
┌─────────────────────────────────────────┐
│         Presentation (MAUI)             │
│              ↓                          │
│         API Controllers                 │
│              ↓                          │
│      Application Layer (CQRS)           │
│              ↓                          │
│         Domain Layer (Core)             │
│              ↑                          │
│     Infrastructure Layer                │
└─────────────────────────────────────────┘
```

**Reglas**:
- Domain no debe depender de nadie
- Application solo depende de Domain
- Infrastructure depende de Application y Domain
- Presentation depende de Application

## 📝 Convenciones de Código

### Naming Conventions

```csharp
// Clases, Interfaces, Enums: PascalCase
public class TransactionService { }
public interface ITransactionRepository { }
public enum TransactionType { }

// Métodos: PascalCase
public async Task<Transaction> GetByIdAsync(Guid id) { }

// Variables locales, parámetros: camelCase
public void ProcessTransaction(Transaction transaction)
{
    var totalAmount = transaction.Amount;
    int categoryCount = 0;
}

// Constantes: UPPER_CASE o PascalCase
public const string DEFAULT_CURRENCY = "USD";
public const int MAX_RETRIES = 3;

// Private fields: _camelCase
private readonly IRepository<Transaction> _repository;
private decimal _totalAmount;

// Propiedades: PascalCase
public string UserName { get; set; }
public decimal Amount { get; set; }
```

### Async/Await

```csharp
// ✅ BIEN - Métodos async terminan en Async
public async Task<Transaction> GetTransactionAsync(Guid id)
{
    return await _repository.GetByIdAsync(id);
}

// ✅ BIEN - Usar ConfigureAwait(false) en librerías
public async Task<Transaction> GetTransactionAsync(Guid id)
{
    return await _repository
        .GetByIdAsync(id)
        .ConfigureAwait(false);
}

// ❌ MAL - Async void (solo en event handlers)
public async void ProcessTransaction() { }

// ✅ BIEN - Async Task
public async Task ProcessTransactionAsync() { }
```

### Null Safety

```csharp
// ✅ BIEN - Usar null-conditional operators
var userName = user?.Name ?? "Unknown";

// ✅ BIEN - Null checks explícitos
if (transaction is null)
{
    throw new ArgumentNullException(nameof(transaction));
}

// ✅ BIEN - Usar nullable reference types
public class User
{
    public string Name { get; set; } = string.Empty;
    public string? MiddleName { get; set; }
}
```

### Exception Handling

```csharp
// ✅ BIEN - Excepciones específicas
public async Task<Transaction> GetTransactionAsync(Guid id)
{
    var transaction = await _repository.GetByIdAsync(id);

    if (transaction is null)
    {
        throw new NotFoundException($"Transaction {id} not found");
    }

    return transaction;
}

// ✅ BIEN - Custom exceptions
public class NotFoundException : Exception
{
    public NotFoundException(string message) : base(message)
    {
    }
}

// ❌ MAL - Catch genérico sin re-throw
try
{
    // código
}
catch (Exception ex)
{
    // silenciar excepción
}

// ✅ BIEN - Catch específico con logging
try
{
    // código
}
catch (DbUpdateException ex)
{
    _logger.LogError(ex, "Database error");
    throw;
}
```

## 🎨 MVVM Pattern (MAUI)

### ViewModel

```csharp
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

public partial class TransactionViewModel : ObservableObject
{
    private readonly ITransactionService _transactionService;

    [ObservableProperty]
    private string description = string.Empty;

    [ObservableProperty]
    private decimal amount;

    [ObservableProperty]
    private bool isBusy;

    public TransactionViewModel(ITransactionService transactionService)
    {
        _transactionService = transactionService;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (IsBusy) return;

        try
        {
            IsBusy = true;
            await _transactionService.CreateAsync(new Transaction
            {
                Description = Description,
                Amount = Amount
            });
        }
        finally
        {
            IsBusy = false;
        }
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        // cargar datos
    }
}
```

### View (XAML)

```xml
<ContentPage xmlns="http://schemas.microsoft.com/dotnet/2021/maui"
             xmlns:x="http://schemas.microsoft.com/winfx/2009/xaml"
             x:Class="CashFlowSystem.MAUI.Views.TransactionPage"
             Title="Nueva Transacción">
    <StackLayout Padding="20">
        <Entry Text="{Binding Description}"
               Placeholder="Descripción" />

        <Entry Text="{Binding Amount}"
               Keyboard="Numeric"
               Placeholder="Monto" />

        <Button Text="Guardar"
                Command="{Binding SaveCommand}"
                IsEnabled="{Binding IsBusy, Converter={StaticResource InvertedBoolConverter}}" />

        <ActivityIndicator IsRunning="{Binding IsBusy}"
                          IsVisible="{Binding IsBusy}" />
    </StackLayout>
</ContentPage>
```

## 🔐 Seguridad

### Password Hashing

```csharp
// ✅ BIEN - Usar BCrypt o Argon2
public class PasswordHasher
{
    public string HashPassword(string password)
    {
        return BCrypt.Net.BCrypt.HashPassword(password, 12);
    }

    public bool VerifyPassword(string password, string hash)
    {
        return BCrypt.Net.BCrypt.Verify(password, hash);
    }
}
```

### SQL Injection Prevention

```csharp
// ✅ BIEN - Usar parámetros (EF Core lo hace automáticamente)
var transactions = await _context.Transactions
    .Where(t => t.UserId == userId)
    .ToListAsync();

// ❌ MAL - Concatenación de strings
var query = $"SELECT * FROM Transactions WHERE UserId = '{userId}'";
```

### JWT Configuration

```csharp
// ✅ BIEN - Configuración segura
services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = configuration["JwtSettings:Issuer"],
            ValidAudience = configuration["JwtSettings:Audience"],
            IssuerSigningKey = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(configuration["JwtSettings:SecretKey"]))
        };
    });
```

## 📊 Performance

### Database Queries

```csharp
// ❌ MAL - N+1 Problem
var transactions = await _context.Transactions.ToListAsync();
foreach (var transaction in transactions)
{
    var category = await _context.Categories.FindAsync(transaction.CategoryId);
}

// ✅ BIEN - Eager Loading
var transactions = await _context.Transactions
    .Include(t => t.Category)
    .Include(t => t.PaymentMethod)
    .ToListAsync();

// ✅ BIEN - Select solo lo necesario
var transactionDtos = await _context.Transactions
    .Select(t => new TransactionDto
    {
        Id = t.Id,
        Amount = t.Amount,
        CategoryName = t.Category.Name
    })
    .ToListAsync();

// ✅ BIEN - AsNoTracking para queries de solo lectura
var transactions = await _context.Transactions
    .AsNoTracking()
    .ToListAsync();
```

### Caching

```csharp
// ✅ BIEN - Cachear datos que no cambian frecuentemente
public class CategoryService
{
    private readonly IMemoryCache _cache;

    public async Task<IEnumerable<Category>> GetCategoriesAsync()
    {
        return await _cache.GetOrCreateAsync("categories", async entry =>
        {
            entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1);
            return await _repository.GetAllAsync();
        });
    }
}
```

### Paginación

```csharp
// ✅ BIEN - Implementar paginación
public async Task<PagedResult<Transaction>> GetTransactionsAsync(
    int page,
    int pageSize)
{
    var query = _context.Transactions.AsQueryable();

    var total = await query.CountAsync();

    var items = await query
        .Skip((page - 1) * pageSize)
        .Take(pageSize)
        .ToListAsync();

    return new PagedResult<Transaction>
    {
        Items = items,
        TotalCount = total,
        Page = page,
        PageSize = pageSize
    };
}
```

## 🧪 Testing

### Unit Tests

```csharp
public class TransactionServiceTests
{
    private readonly Mock<IRepository<Transaction>> _mockRepository;
    private readonly TransactionService _service;

    public TransactionServiceTests()
    {
        _mockRepository = new Mock<IRepository<Transaction>>();
        _service = new TransactionService(_mockRepository.Object);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsTransaction()
    {
        // Arrange
        var id = Guid.NewGuid();
        var expected = new Transaction { Id = id, Amount = 100 };
        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync(expected);

        // Act
        var result = await _service.GetByIdAsync(id);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(id);
        result.Amount.Should().Be(100);
    }

    [Fact]
    public async Task GetByIdAsync_NonExistingId_ThrowsNotFoundException()
    {
        // Arrange
        var id = Guid.NewGuid();
        _mockRepository
            .Setup(r => r.GetByIdAsync(id))
            .ReturnsAsync((Transaction?)null);

        // Act & Assert
        await Assert.ThrowsAsync<NotFoundException>(
            async () => await _service.GetByIdAsync(id));
    }
}
```

### Integration Tests

```csharp
public class TransactionApiTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public TransactionApiTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetTransactions_ReturnsSuccessStatusCode()
    {
        // Act
        var response = await _client.GetAsync("/api/transactions");

        // Assert
        response.EnsureSuccessStatusCode();
        var content = await response.Content.ReadAsStringAsync();
        content.Should().NotBeNullOrEmpty();
    }
}
```

## 📝 Logging

```csharp
// ✅ BIEN - Logging estructurado con Serilog
public class TransactionService
{
    private readonly ILogger<TransactionService> _logger;

    public async Task CreateAsync(Transaction transaction)
    {
        _logger.LogInformation(
            "Creating transaction {TransactionId} for user {UserId}",
            transaction.Id,
            transaction.UserId);

        try
        {
            await _repository.AddAsync(transaction);

            _logger.LogInformation(
                "Transaction {TransactionId} created successfully",
                transaction.Id);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex,
                "Error creating transaction {TransactionId}",
                transaction.Id);
            throw;
        }
    }
}
```

## 🎯 Validation

### FluentValidation

```csharp
public class CreateTransactionCommandValidator
    : AbstractValidator<CreateTransactionCommand>
{
    public CreateTransactionCommandValidator()
    {
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than zero");

        RuleFor(x => x.Description)
            .NotEmpty()
            .WithMessage("Description is required")
            .MaximumLength(500)
            .WithMessage("Description must not exceed 500 characters");

        RuleFor(x => x.Date)
            .NotEmpty()
            .WithMessage("Date is required")
            .LessThanOrEqualTo(DateTime.UtcNow)
            .WithMessage("Date cannot be in the future");

        RuleFor(x => x.CategoryId)
            .NotEmpty()
            .WithMessage("Category is required");
    }
}
```

## 🔄 CQRS Pattern

### Command

```csharp
public class CreateTransactionCommand : IRequest<TransactionDto>
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
}

public class CreateTransactionCommandHandler
    : IRequestHandler<CreateTransactionCommand, TransactionDto>
{
    private readonly IRepository<Transaction> _repository;
    private readonly IMapper _mapper;

    public CreateTransactionCommandHandler(
        IRepository<Transaction> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TransactionDto> Handle(
        CreateTransactionCommand request,
        CancellationToken cancellationToken)
    {
        var transaction = new Transaction
        {
            Date = request.Date,
            Amount = request.Amount,
            Description = request.Description,
            CategoryId = request.CategoryId,
            CreatedAt = DateTime.UtcNow
        };

        await _repository.AddAsync(transaction);

        return _mapper.Map<TransactionDto>(transaction);
    }
}
```

### Query

```csharp
public class GetTransactionByIdQuery : IRequest<TransactionDto>
{
    public Guid Id { get; set; }
}

public class GetTransactionByIdQueryHandler
    : IRequestHandler<GetTransactionByIdQuery, TransactionDto>
{
    private readonly IRepository<Transaction> _repository;
    private readonly IMapper _mapper;

    public GetTransactionByIdQueryHandler(
        IRepository<Transaction> repository,
        IMapper mapper)
    {
        _repository = repository;
        _mapper = mapper;
    }

    public async Task<TransactionDto> Handle(
        GetTransactionByIdQuery request,
        CancellationToken cancellationToken)
    {
        var transaction = await _repository.GetByIdAsync(request.Id);

        if (transaction is null)
        {
            throw new NotFoundException(
                $"Transaction {request.Id} not found");
        }

        return _mapper.Map<TransactionDto>(transaction);
    }
}
```

## 📚 Documentación de Código

```csharp
/// <summary>
/// Servicio para gestionar transacciones financieras.
/// </summary>
public class TransactionService : ITransactionService
{
    /// <summary>
    /// Obtiene una transacción por su identificador.
    /// </summary>
    /// <param name="id">Identificador único de la transacción.</param>
    /// <returns>La transacción encontrada.</returns>
    /// <exception cref="NotFoundException">
    /// Se lanza cuando la transacción no existe.
    /// </exception>
    public async Task<Transaction> GetByIdAsync(Guid id)
    {
        // implementación
    }
}
```

## ✅ Code Review Checklist

Antes de hacer commit:

- [ ] Código sigue las convenciones de nombrado
- [ ] Sin warnings de compilación
- [ ] Tests unitarios pasando
- [ ] Sin código comentado innecesariamente
- [ ] Sin TODOs sin issue tracker
- [ ] Logging apropiado
- [ ] Manejo de errores implementado
- [ ] Validaciones en su lugar
- [ ] Sin hardcoded values (usar configuración)
- [ ] Documentación actualizada
- [ ] Sin secretos en el código

## 🚀 Deployment Checklist

- [ ] appsettings.Production.json configurado
- [ ] Connection strings en variables de entorno
- [ ] HTTPS configurado
- [ ] CORS configurado apropiadamente
- [ ] Rate limiting activado
- [ ] Logging en producción configurado
- [ ] Health checks implementados
- [ ] Backup strategy definida
- [ ] Monitoring configurado
- [ ] Secretos en Azure Key Vault / AWS Secrets Manager
