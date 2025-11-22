# Guía de Desarrollo - CashFlowSystem

## 📋 Índice

1. [Configuración Inicial](#configuración-inicial)
2. [Creación de Proyectos](#creación-de-proyectos)
3. [Implementación por Capas](#implementación-por-capas)
4. [Módulos del Sistema](#módulos-del-sistema)
5. [Testing](#testing)
6. [Deployment](#deployment)

## 🔧 Configuración Inicial

### 1. Instalar .NET 8 SDK

```bash
# Verificar instalación
dotnet --version
# Debe mostrar 8.0.x o superior
```

### 2. Instalar Workloads de MAUI

```bash
dotnet workload install maui
dotnet workload install android
dotnet workload install ios
dotnet workload install maccatalyst
```

### 3. Instalar Herramientas Adicionales

```bash
# Entity Framework Tools
dotnet tool install --global dotnet-ef

# User Secrets (para configuración local)
dotnet tool install --global dotnet-user-secrets
```

## 🏗️ Creación de Proyectos

### Paso 1: Crear la Solución

```bash
# Crear solución
dotnet new sln -n CashFlowSystem

# Crear carpetas
mkdir -p src tests docs
```

### Paso 2: Crear Proyecto Domain (Class Library)

```bash
cd src
dotnet new classlib -n CashFlowSystem.Domain -f net8.0
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.Domain/CashFlowSystem.Domain.csproj
```

**Paquetes necesarios:**
```bash
cd CashFlowSystem.Domain
# Ninguno necesario - debe ser puro, sin dependencias externas
```

### Paso 3: Crear Proyecto Application (Class Library)

```bash
cd ..
dotnet new classlib -n CashFlowSystem.Application -f net8.0
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.Application/CashFlowSystem.Application.csproj
```

**Paquetes necesarios:**
```bash
cd CashFlowSystem.Application
dotnet add package MediatR
dotnet add package FluentValidation
dotnet add package AutoMapper
dotnet add reference ../CashFlowSystem.Domain/CashFlowSystem.Domain.csproj
```

### Paso 4: Crear Proyecto Infrastructure (Class Library)

```bash
cd ..
dotnet new classlib -n CashFlowSystem.Infrastructure -f net8.0
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.Infrastructure/CashFlowSystem.Infrastructure.csproj
```

**Paquetes necesarios:**
```bash
cd CashFlowSystem.Infrastructure
dotnet add package Microsoft.EntityFrameworkCore
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Npgsql.EntityFrameworkCore.PostgreSQL
# O para SQL Server:
# dotnet add package Microsoft.EntityFrameworkCore.SqlServer
dotnet add package Microsoft.EntityFrameworkCore.Tools
dotnet add reference ../CashFlowSystem.Domain/CashFlowSystem.Domain.csproj
dotnet add reference ../CashFlowSystem.Application/CashFlowSystem.Application.csproj
```

### Paso 5: Crear Proyecto API (Web API)

```bash
cd ..
dotnet new webapi -n CashFlowSystem.API -f net8.0
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.API/CashFlowSystem.API.csproj
```

**Paquetes necesarios:**
```bash
cd CashFlowSystem.API
dotnet add package Microsoft.AspNetCore.Authentication.JwtBearer
dotnet add package Swashbuckle.AspNetCore
dotnet add package Serilog.AspNetCore
dotnet add package MediatR
dotnet add reference ../CashFlowSystem.Application/CashFlowSystem.Application.csproj
dotnet add reference ../CashFlowSystem.Infrastructure/CashFlowSystem.Infrastructure.csproj
```

### Paso 6: Crear Proyecto MAUI

```bash
cd ..
dotnet new maui -n CashFlowSystem.MAUI -f net8.0
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.MAUI/CashFlowSystem.MAUI.csproj
```

**Paquetes necesarios:**
```bash
cd CashFlowSystem.MAUI
dotnet add package CommunityToolkit.Maui
dotnet add package CommunityToolkit.Mvvm
dotnet add package Microsoft.Extensions.Http
dotnet add package Newtonsoft.Json
# Para gráficos
dotnet add package LiveChartsCore.SkiaSharpView.Maui
```

### Paso 7: Crear Proyectos de Testing

```bash
cd ../../tests

# Domain Tests
dotnet new xunit -n CashFlowSystem.Domain.Tests
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.Domain.Tests/CashFlowSystem.Domain.Tests.csproj

# Application Tests
dotnet new xunit -n CashFlowSystem.Application.Tests
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.Application.Tests/CashFlowSystem.Application.Tests.csproj

# Infrastructure Tests
dotnet new xunit -n CashFlowSystem.Infrastructure.Tests
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.Infrastructure.Tests/CashFlowSystem.Infrastructure.Tests.csproj

# API Tests
dotnet new xunit -n CashFlowSystem.API.Tests
dotnet sln ../CashFlowSystem.sln add CashFlowSystem.API.Tests/CashFlowSystem.API.Tests.csproj
```

**Paquetes comunes para tests:**
```bash
# Para cada proyecto de test
dotnet add package FluentAssertions
dotnet add package Moq
dotnet add package Microsoft.EntityFrameworkCore.InMemory
```

## 🔨 Implementación por Capas

### CAPA 1: Domain Layer

#### 1.1 Crear Enums Base

**src/CashFlowSystem.Domain/Enums/TransactionType.cs**
```csharp
namespace CashFlowSystem.Domain.Enums;

public enum TransactionType
{
    Income = 1,
    Expense = 2
}
```

**src/CashFlowSystem.Domain/Enums/PaymentMethodType.cs**
```csharp
namespace CashFlowSystem.Domain.Enums;

public enum PaymentMethodType
{
    Cash = 1,
    DebitCard = 2,
    CreditCard = 3,
    BankTransfer = 4,
    Check = 5,
    Other = 99
}
```

**src/CashFlowSystem.Domain/Enums/CashRegisterStatus.cs**
```csharp
namespace CashFlowSystem.Domain.Enums;

public enum CashRegisterStatus
{
    Open = 1,
    Closed = 2
}
```

**src/CashFlowSystem.Domain/Enums/UserRole.cs**
```csharp
namespace CashFlowSystem.Domain.Enums;

public enum UserRole
{
    Admin = 1,
    Manager = 2,
    Cashier = 3
}
```

#### 1.2 Crear Entidad Base

**src/CashFlowSystem.Domain/Entities/BaseEntity.cs**
```csharp
namespace CashFlowSystem.Domain.Entities;

public abstract class BaseEntity
{
    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime? UpdatedAt { get; set; }
    public bool IsDeleted { get; set; }
}
```

#### 1.3 Crear Entidades Principales

**src/CashFlowSystem.Domain/Entities/User.cs**
```csharp
using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class User : BaseEntity
{
    public string Username { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string PasswordHash { get; set; } = string.Empty;
    public UserRole Role { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
    public ICollection<CashRegister> CashRegisters { get; set; } = new List<CashRegister>();
}
```

**src/CashFlowSystem.Domain/Entities/Category.cs**
```csharp
using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class Category : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public TransactionType Type { get; set; }
    public string Color { get; set; } = "#000000";
    public string Icon { get; set; } = "default";
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
```

**src/CashFlowSystem.Domain/Entities/PaymentMethod.cs**
```csharp
using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class PaymentMethod : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public PaymentMethodType Type { get; set; }
    public bool IsActive { get; set; }

    // Navigation properties
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
```

**src/CashFlowSystem.Domain/Entities/Transaction.cs**
```csharp
using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class Transaction : BaseEntity
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public TransactionType Type { get; set; }

    // Foreign Keys
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid UserId { get; set; }
    public Guid? CashRegisterId { get; set; }

    // Navigation properties
    public Category Category { get; set; } = null!;
    public PaymentMethod PaymentMethod { get; set; } = null!;
    public User User { get; set; } = null!;
    public CashRegister? CashRegister { get; set; }

    // Specific fields for Income/Expense
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
```

**src/CashFlowSystem.Domain/Entities/CashRegister.cs**
```csharp
using CashFlowSystem.Domain.Enums;

namespace CashFlowSystem.Domain.Entities;

public class CashRegister : BaseEntity
{
    public DateTime OpeningDate { get; set; }
    public DateTime? ClosingDate { get; set; }
    public decimal OpeningBalance { get; set; }
    public decimal? ClosingBalance { get; set; }
    public decimal? ExpectedBalance { get; set; }
    public decimal? Difference { get; set; }
    public CashRegisterStatus Status { get; set; }
    public string? Notes { get; set; }

    // Foreign Keys
    public Guid UserId { get; set; }

    // Navigation properties
    public User User { get; set; } = null!;
    public ICollection<Transaction> Transactions { get; set; } = new List<Transaction>();
}
```

#### 1.4 Crear Interfaces de Repositorio

**src/CashFlowSystem.Domain/Interfaces/IRepository.cs**
```csharp
using CashFlowSystem.Domain.Entities;

namespace CashFlowSystem.Domain.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
    Task<bool> ExistsAsync(Guid id);
}
```

**src/CashFlowSystem.Domain/Interfaces/IUnitOfWork.cs**
```csharp
namespace CashFlowSystem.Domain.Interfaces;

public interface IUnitOfWork : IDisposable
{
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}
```

### CAPA 2: Application Layer

#### 2.1 Crear DTOs Base

**src/CashFlowSystem.Application/DTOs/TransactionDto.cs**
```csharp
namespace CashFlowSystem.Application.DTOs;

public class TransactionDto
{
    public Guid Id { get; set; }
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public Guid CategoryId { get; set; }
    public string CategoryName { get; set; } = string.Empty;
    public Guid PaymentMethodId { get; set; }
    public string PaymentMethodName { get; set; } = string.Empty;
    public string? ReferenceNumber { get; set; }
}
```

**src/CashFlowSystem.Application/DTOs/DashboardDto.cs**
```csharp
namespace CashFlowSystem.Application.DTOs;

public class DashboardDto
{
    public decimal CurrentBalance { get; set; }
    public decimal TotalIncome { get; set; }
    public decimal TotalExpense { get; set; }
    public decimal NetFlow { get; set; }
    public List<TransactionDto> RecentTransactions { get; set; } = new();
    public Dictionary<string, decimal> IncomeByCategory { get; set; } = new();
    public Dictionary<string, decimal> ExpenseByCategory { get; set; } = new();
    public List<DailyFlowDto> DailyFlow { get; set; } = new();
}

public class DailyFlowDto
{
    public DateTime Date { get; set; }
    public decimal Income { get; set; }
    public decimal Expense { get; set; }
    public decimal Balance { get; set; }
}
```

#### 2.2 Crear Commands (CQRS)

**src/CashFlowSystem.Application/Commands/CreateTransactionCommand.cs**
```csharp
using MediatR;
using CashFlowSystem.Application.DTOs;

namespace CashFlowSystem.Application.Commands;

public class CreateTransactionCommand : IRequest<TransactionDto>
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string Description { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty; // "Income" or "Expense"
    public Guid CategoryId { get; set; }
    public Guid PaymentMethodId { get; set; }
    public Guid UserId { get; set; }
    public string? ReferenceNumber { get; set; }
    public string? Notes { get; set; }
}
```

#### 2.3 Crear Queries (CQRS)

**src/CashFlowSystem.Application/Queries/GetDashboardDataQuery.cs**
```csharp
using MediatR;
using CashFlowSystem.Application.DTOs;

namespace CashFlowSystem.Application.Queries;

public class GetDashboardDataQuery : IRequest<DashboardDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public Guid UserId { get; set; }
}
```

### CAPA 3: Infrastructure Layer

#### 3.1 Crear DbContext

**src/CashFlowSystem.Infrastructure/Data/ApplicationDbContext.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using CashFlowSystem.Domain.Entities;

namespace CashFlowSystem.Infrastructure.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<User> Users => Set<User>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<PaymentMethod> PaymentMethods => Set<PaymentMethod>();
    public DbSet<Transaction> Transactions => Set<Transaction>();
    public DbSet<CashRegister> CashRegisters => Set<CashRegister>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Configuraciones de entidades
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
```

#### 3.2 Crear Repositorios

**src/CashFlowSystem.Infrastructure/Repositories/Repository.cs**
```csharp
using Microsoft.EntityFrameworkCore;
using CashFlowSystem.Domain.Entities;
using CashFlowSystem.Domain.Interfaces;
using CashFlowSystem.Infrastructure.Data;

namespace CashFlowSystem.Infrastructure.Repositories;

public class Repository<T> : IRepository<T> where T : BaseEntity
{
    protected readonly ApplicationDbContext _context;
    protected readonly DbSet<T> _dbSet;

    public Repository(ApplicationDbContext context)
    {
        _context = context;
        _dbSet = context.Set<T>();
    }

    public async Task<T?> GetByIdAsync(Guid id)
    {
        return await _dbSet.FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
    }

    public async Task<IEnumerable<T>> GetAllAsync()
    {
        return await _dbSet.Where(e => !e.IsDeleted).ToListAsync();
    }

    public async Task<T> AddAsync(T entity)
    {
        entity.CreatedAt = DateTime.UtcNow;
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public async Task UpdateAsync(T entity)
    {
        entity.UpdatedAt = DateTime.UtcNow;
        _dbSet.Update(entity);
        await Task.CompletedTask;
    }

    public async Task DeleteAsync(Guid id)
    {
        var entity = await GetByIdAsync(id);
        if (entity != null)
        {
            entity.IsDeleted = true;
            entity.UpdatedAt = DateTime.UtcNow;
        }
    }

    public async Task<bool> ExistsAsync(Guid id)
    {
        return await _dbSet.AnyAsync(e => e.Id == id && !e.IsDeleted);
    }
}
```

## 📱 Configuración de MAUI

### Estructura MVVM

#### App.xaml.cs
```csharp
using CashFlowSystem.MAUI.Services;
using CashFlowSystem.MAUI.ViewModels;
using CashFlowSystem.MAUI.Views;

namespace CashFlowSystem.MAUI;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Configurar Dependency Injection
        ConfigureServices();

        MainPage = new AppShell();
    }

    private void ConfigureServices()
    {
        // Registrar servicios, ViewModels, Views
    }
}
```

## 🔐 Configuración de Seguridad

### JWT Configuration en API

**appsettings.json**
```json
{
  "JwtSettings": {
    "SecretKey": "your-secret-key-min-32-characters-long",
    "Issuer": "CashFlowSystem",
    "Audience": "CashFlowSystemClient",
    "ExpirationMinutes": 60
  },
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=cashflow_db;Username=postgres;Password=yourpassword"
  }
}
```

## 🚀 Comandos Útiles

### Ejecutar Migraciones
```bash
# Crear migración
dotnet ef migrations add InitialCreate --project src/CashFlowSystem.Infrastructure --startup-project src/CashFlowSystem.API

# Actualizar base de datos
dotnet ef database update --project src/CashFlowSystem.Infrastructure --startup-project src/CashFlowSystem.API
```

### Ejecutar la Aplicación
```bash
# API
dotnet run --project src/CashFlowSystem.API

# MAUI (Windows)
dotnet build src/CashFlowSystem.MAUI -f net8.0-windows10.0.19041.0 -t:Run

# MAUI (Android)
dotnet build src/CashFlowSystem.MAUI -f net8.0-android -t:Run
```

### Testing
```bash
# Ejecutar todos los tests
dotnet test

# Con cobertura
dotnet test /p:CollectCoverage=true
```

## 📖 Próximos Pasos

1. ✅ Configurar proyectos
2. ✅ Implementar entidades del dominio
3. ⬜ Implementar handlers de MediatR
4. ⬜ Configurar DbContext y migraciones
5. ⬜ Implementar API Controllers
6. ⬜ Crear vistas MAUI
7. ⬜ Implementar ViewModels
8. ⬜ Configurar navegación
9. ⬜ Implementar dashboard
10. ⬜ Implementar módulo de informes

## 💡 Tips de Desarrollo

### Performance
- Usar `AsNoTracking()` en queries de solo lectura
- Implementar paginación en listas grandes
- Cachear datos de catálogos (categorías, métodos de pago)

### Seguridad
- Nunca guardar passwords en texto plano
- Validar siempre en el backend
- Usar HTTPS en producción
- Implementar rate limiting

### Mantenibilidad
- Seguir principios SOLID
- Mantener métodos pequeños y enfocados
- Escribir tests unitarios
- Documentar código complejo
- Usar convenciones de nombres consistentes

## 🐛 Troubleshooting

### Problemas Comunes

**Error: "Workload 'maui' not found"**
```bash
dotnet workload restore
dotnet workload install maui
```

**Error de conexión a base de datos**
- Verificar que PostgreSQL esté corriendo
- Revisar connection string en appsettings.json
- Verificar credenciales

**Error al ejecutar migraciones**
```bash
# Limpiar y reconstruir
dotnet clean
dotnet build
dotnet ef migrations add InitialCreate --force
```
