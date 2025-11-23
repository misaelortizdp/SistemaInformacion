# Guía de Implementación - Backends Pendientes

## ⚠️ Nota Importante sobre Repositorios

El sistema actual usa un patrón de acceso directo al DbContext a través de DbSet.
Para completar las funcionalidades, se necesita:

1. **Agregar propiedades al IUnitOfWork** (src/CashFlowSystem.Domain/Interfaces/IUnitOfWork.cs):
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Category> Categories { get; }
    IRepository<PaymentMethod> PaymentMethods { get; }
    IRepository<Transaction> Transactions { get; }
    IRepository<CashRegister> CashRegisters { get; }
    IRepository<Budget> Budgets { get; }

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    // ... métodos de transacción existentes
}
```

2. **Actualizar UnitOfWork** (src/CashFlowSystem.Infrastructure/Repositories/UnitOfWork.cs):
```csharp
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _context;
    private IDbContextTransaction? _transaction;

    private IRepository<User>? _users;
    private IRepository<Category>? _categories;
    private IRepository<PaymentMethod>? _paymentMethods;
    private IRepository<Transaction>? _transactions;
    private IRepository<CashRegister>? _cashRegisters;
    private IRepository<Budget>? _budgets;

    public UnitOfWork(ApplicationDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<PaymentMethod> PaymentMethods => _paymentMethods ??= new Repository<PaymentMethod>(_context);
    public IRepository<Transaction> Transactions => _transactions ??= new Repository<Transaction>(_context);
    public IRepository<CashRegister> CashRegisters => _cashRegisters ??= new Repository<CashRegister>(_context);
    public IRepository<Budget> Budgets => _budgets ??= new Repository<Budget>(_context);

    // ... resto del código existente
}
```

---

## 1. ✅ Sistema de Presupuestos (COMPLETADO)

**Estado**: 90% implementado

**Archivos creados**:
- ✅ Domain/Entities/Budget.cs
- ✅ Application/DTOs/BudgetDto.cs
- ✅ Application/Commands/Budgets/ (Create, Update, Delete)
- ✅ Application/Queries/Budgets/GetUserBudgetsQuery.cs
- ✅ Application/Handlers/Budgets/ (todos los handlers)
- ✅ API/Controllers/BudgetsController.cs
- ✅ ApplicationDbContext actualizado con DbSet<Budget>

**Falta**:
- ⏳ Agregar repositorio Budgets al IUnitOfWork/UnitOfWork (ver arriba)
- ⏳ Crear migración de BD: `dotnet ef migrations add AddBudgets`

**Endpoints disponibles**:
- GET `/api/budgets?userId={guid}&isActive={bool}`
- POST `/api/budgets`
- PUT `/api/budgets/{id}`
- DELETE `/api/budgets/{id}`

---

## 2. ⚪ Exportación PDF y Excel

**Paquetes NuGet necesarios**:
```xml
<PackageReference Include="QuestPDF" Version="2024.1.0" />
<PackageReference Include="ClosedXML" Version="0.102.0" />
```

**Archivos a crear**:

### Services/IExportService.cs
```csharp
namespace CashFlowSystem.Application.Services;

public interface IExportService
{
    Task<byte[]> ExportToPdfAsync(ReportDto report);
    Task<byte[]> ExportToExcelAsync(IEnumerable<TransactionDto> transactions);
}
```

### Services/PdfExportService.cs
```csharp
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace CashFlowSystem.Infrastructure.Services;

public class PdfExportService : IExportService
{
    public async Task<byte[]> ExportToPdfAsync(ReportDto report)
    {
        return await Task.Run(() =>
        {
            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(2, Unit.Centimetre);
                    page.PageColor(Colors.White);
                    page.DefaultTextStyle(x => x.FontSize(12));

                    page.Header()
                        .Text("Informe de Flujo de Caja")
                        .SemiBold().FontSize(20).FontColor(Colors.Blue.Medium);

                    page.Content()
                        .PaddingVertical(1, Unit.Centimetre)
                        .Column(x =>
                        {
                            x.Spacing(10);
                            x.Item().Text($"Período: {report.StartDate:dd/MM/yyyy} - {report.EndDate:dd/MM/yyyy}");
                            x.Item().Text($"Total Ingresos: ${report.TotalIncome:N2}");
                            x.Item().Text($"Total Egresos: ${report.TotalExpense:N2}");
                            x.Item().Text($"Flujo Neto: ${report.NetFlow:N2}");
                        });

                    page.Footer()
                        .AlignCenter()
                        .Text(x =>
                        {
                            x.Span("Página ");
                            x.CurrentPageNumber();
                        });
                });
            });

            return document.GeneratePdf();
        });
    }

    // Implementar ExportToExcelAsync con ClosedXML
}
```

### Commands/Reports/ExportReportCommand.cs
```csharp
public record ExportReportCommand : IRequest<byte[]>
{
    public Guid UserId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
    public ExportFormat Format { get; init; } // PDF, Excel
}

public enum ExportFormat
{
    PDF,
    Excel
}
```

### Controllers/ReportsController.cs - Agregar endpoint
```csharp
[HttpPost("export")]
public async Task<ActionResult> ExportReport([FromBody] ExportReportCommand command)
{
    var fileBytes = await _mediator.Send(command);
    var contentType = command.Format == ExportFormat.PDF
        ? "application/pdf"
        : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
    var fileName = $"reporte_{DateTime.Now:yyyyMMdd}.{(command.Format == ExportFormat.PDF ? "pdf" : "xlsx")}";

    return File(fileBytes, contentType, fileName);
}
```

---

## 3. ⚪ Transacciones Recurrentes

**Entidades a crear**:

### Domain/Entities/RecurringTransaction.cs
```csharp
public class RecurringTransaction : BaseEntity
{
    public Guid UserId { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public RecurrenceFrequency Frequency { get; set; }
    public int FrequencyValue { get; set; } // Every X days/weeks/months
    public DateTime StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? LastExecuted { get; set; }
    public DateTime NextExecution { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Notes { get; set; }

    // Navigation
    public virtual User User { get; set; } = null!;
    public virtual Category Category { get; set; } = null!;
    public virtual PaymentMethod PaymentMethod { get; set; } = null!;
}
```

### Domain/Enums/RecurrenceFrequency.cs
```csharp
public enum RecurrenceFrequency
{
    Daily,
    Weekly,
    Monthly,
    Yearly
}
```

**Service para ejecución automática**:

### Services/RecurringTransactionService.cs
```csharp
public class RecurringTransactionService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<RecurringTransactionService> _logger;
    private readonly TimeSpan _checkInterval = TimeSpan.FromHours(1);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await ProcessRecurringTransactions();
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task ProcessRecurringTransactions()
    {
        using var scope = _serviceProvider.CreateScope();
        var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

        var dueRecurrences = await unitOfWork.RecurringTransactions
            .FindAsync(r => r.IsActive && r.NextExecution <= DateTime.UtcNow);

        foreach (var recurrence in dueRecurrences)
        {
            // Create transaction
            var transaction = new Transaction
            {
                UserId = recurrence.UserId,
                Description = recurrence.Description,
                Amount = recurrence.Amount,
                Type = recurrence.Type,
                CategoryId = recurrence.CategoryId,
                PaymentMethodId = recurrence.PaymentMethodId,
                Date = DateTime.UtcNow,
                Notes = $"Generado automáticamente desde recurrencia: {recurrence.Description}"
            };

            await unitOfWork.Transactions.AddAsync(transaction);

            // Update next execution
            recurrence.LastExecuted = DateTime.UtcNow;
            recurrence.NextExecution = CalculateNextExecution(recurrence);
            unitOfWork.RecurringTransactions.Update(recurrence);
        }

        await unitOfWork.SaveChangesAsync();
    }

    private DateTime CalculateNextExecution(RecurringTransaction recurrence)
    {
        return recurrence.Frequency switch
        {
            RecurrenceFrequency.Daily => recurrence.NextExecution.AddDays(recurrence.FrequencyValue),
            RecurrenceFrequency.Weekly => recurrence.NextExecution.AddDays(7 * recurrence.FrequencyValue),
            RecurrenceFrequency.Monthly => recurrence.NextExecution.AddMonths(recurrence.FrequencyValue),
            RecurrenceFrequency.Yearly => recurrence.NextExecution.AddYears(recurrence.FrequencyValue),
            _ => recurrence.NextExecution.AddMonths(1)
        };
    }
}
```

**Registrar en Program.cs**:
```csharp
builder.Services.AddHostedService<RecurringTransactionService>();
```

---

## 4. ⚪ Multi-Negocio

**Cambio arquitectural mayor** - Requiere actualizar TODAS las entidades.

### Entidades a crear/modificar:

#### Domain/Entities/Business.cs (NUEVO)
```csharp
public class Business : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? Logo { get; set; }
    public string? Address { get; set; }
    public string? Phone { get; set; }
    public string? Email { get; set; }
    public bool IsActive { get; set; } = true;

    // Navigation
    public virtual ICollection<BusinessUser> BusinessUsers { get; set; } = new List<BusinessUser>();
    public virtual ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
```

#### Domain/Entities/BusinessUser.cs (NUEVO)
```csharp
public class BusinessUser : BaseEntity
{
    public Guid BusinessId { get; set; }
    public Guid UserId { get; set; }
    public UserRole RoleInBusiness { get; set; }
    public DateTime JoinedDate { get; set; }

    // Navigation
    public virtual Business Business { get; set; } = null!;
    public virtual User User { get; set; } = null!;
}
```

**Actualizar TODAS las entidades existentes** - Agregar `BusinessId`:
- ✏️ Transaction.BusinessId
- ✏️ Category.BusinessId
- ✏️ PaymentMethod.BusinessId
- ✏️ CashRegister.BusinessId
- ✏️ Budget.BusinessId

**Claims en JWT** - Agregar CurrentBusinessId:
```csharp
// En JwtService.cs
var claims = new[]
{
    new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
    new Claim(ClaimTypes.Name, user.Username),
    new Claim(ClaimTypes.Email, user.Email),
    new Claim(ClaimTypes.Role, user.Role.ToString()),
    new Claim("CurrentBusinessId", currentBusinessId.ToString()) // NUEVO
};
```

**Filtros globales** - Todas las queries deben filtrar por BusinessId del usuario actual.

---

## 5. ⚪ Roles y Permisos Granulares

### Expandir UserRole enum:
```csharp
public enum UserRole
{
    SuperAdmin,    // Acceso total sistema
    Admin,         // Admin de negocio
    Manager,       // Gestión sin configuración
    Cashier,       // Solo transacciones
    Accountant,    // Informes, solo lectura
    ReadOnly       // Solo visualización
}
```

### Crear Permission enum:
```csharp
[Flags]
public enum Permission
{
    None = 0,
    ViewTransactions = 1,
    CreateTransaction = 2,
    EditTransaction = 4,
    DeleteTransaction = 8,
    ViewReports = 16,
    ExportReports = 32,
    ManageCategories = 64,
    ManagePaymentMethods = 128,
    ManageCashRegister = 256,
    ManageBudgets = 512,
    ManageUsers = 1024,
    ManageSettings = 2048,
    All = 4095
}
```

### Mapeo Rol → Permisos:
```csharp
public static class RolePermissions
{
    public static Dictionary<UserRole, Permission> Mappings = new()
    {
        { UserRole.SuperAdmin, Permission.All },
        { UserRole.Admin, Permission.All & ~Permission.ManageUsers },
        { UserRole.Manager, Permission.ViewTransactions | Permission.CreateTransaction |
                           Permission.EditTransaction | Permission.ViewReports |
                           Permission.ManageCashRegister | Permission.ManageBudgets },
        { UserRole.Cashier, Permission.ViewTransactions | Permission.CreateTransaction |
                           Permission.ManageCashRegister },
        { UserRole.Accountant, Permission.ViewTransactions | Permission.ViewReports |
                              Permission.ExportReports },
        { UserRole.ReadOnly, Permission.ViewTransactions | Permission.ViewReports }
    };
}
```

### Authorization Policies:
```csharp
// En Program.cs
builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("CanDeleteTransaction", policy =>
        policy.RequireAssertion(context =>
            HasPermission(context.User, Permission.DeleteTransaction)));

    options.AddPolicy("CanExportReports", policy =>
        policy.RequireAssertion(context =>
            HasPermission(context.User, Permission.ExportReports)));
    // ... etc
});
```

### Uso en Controllers:
```csharp
[Authorize(Policy = "CanDeleteTransaction")]
[HttpDelete("{id}")]
public async Task<ActionResult> Delete(Guid id)
{
    // ...
}
```

---

## 6. ⚪ Reportes Avanzados

### Queries adicionales a crear:

#### GetCategoryAnalysisQuery.cs
```csharp
public record GetCategoryAnalysisQuery : IRequest<CategoryAnalysisDto>
{
    public Guid UserId { get; init; }
    public DateTime StartDate { get; init; }
    public DateTime EndDate { get; init; }
}

public class CategoryAnalysisDto
{
    public List<CategorySpending> TopExpenseCategories { get; set; } = new();
    public List<CategorySpending> TopIncomeCategories { get; set; } = new();
    public decimal TotalExpense { get; set; }
    public decimal TotalIncome { get; set; }
}

public class CategorySpending
{
    public string CategoryName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public decimal Percentage { get; set; }
    public int TransactionCount { get; set; }
}
```

#### GetTrendAnalysisQuery.cs
```csharp
public record GetTrendAnalysisQuery : IRequest<TrendAnalysisDto>
{
    public Guid UserId { get; init; }
    public int Months { get; init; } = 6; // Últimos 6 meses
}

public class TrendAnalysisDto
{
    public List<MonthlyData> MonthlyTrends { get; set; } = new();
    public decimal AverageDailyIncome { get; set; }
    public decimal AverageDailyExpense { get; set; }
    public decimal ProjectedNextMonth { get; set; }
}

public class MonthlyData
{
    public string Month { get; set; } = string.Empty; // "2025-01"
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal NetFlow { get; set; }
}
```

---

## Resumen de Archivos a Crear

| Funcionalidad | Archivos Backend | Líneas Est. |
|---------------|------------------|-------------|
| Presupuestos | ✅ COMPLETADO | ~800 |
| Exportación PDF/Excel | 6 archivos | ~600 |
| Recurrentes | 10 archivos | ~900 |
| Multi-Negocio | 15+ archivos (migración mayor) | ~1500 |
| Roles Avanzados | 5 archivos | ~400 |
| Reportes Avanzados | 8 archivos | ~700 |

**Total pendiente**: ~5100 líneas de código backend

---

## Próximos Pasos Recomendados

1. ✅ **Completar configuración de repositorio Budgets** (30 min)
2. ✅ **Migración de BD para Budgets** (10 min)
3. 🎯 **PRIORIDAD: Implementar Frontend MAUI completo** (40-60 horas)
4. ⏳ Exportación PDF/Excel (8 horas)
5. ⏳ Transacciones Recurrentes (16 horas)
6. ⏳ Reportes Avanzados (12 horas)
7. ⏳ Multi-Negocio (fase mayor, 24+ horas)
8. ⏳ Roles Granulares (8 horas)

---

**Nota**: El frontend MAUI es el componente más valioso porque permite **usar inmediatamente** las funcionalidades backend ya implementadas (CRUD Categorías, CRUD Métodos Pago, Búsqueda Avanzada, Presupuestos).
