# Plan de Arquitectura - Sistema de Gestión de Flujo de Caja

## 1. Stack Tecnológico

### Frontend (Cross-Platform)
- **.NET 8 MAUI** - Aplicación nativa para:
  - Windows (Desktop)
  - macOS (Desktop)
  - iOS (Mobile)
  - Android (Mobile)
- **MVVM Pattern** - Patrón de diseño para UI
- **CommunityToolkit.Mvvm** - Helpers para MVVM
- **Syncfusion/DevExpress Community** - Componentes UI avanzados (opcional)

### Backend
- **ASP.NET Core 8 Web API**
  - RESTful API
  - JWT Authentication
  - SignalR para notificaciones en tiempo real
- **Entity Framework Core 8**
  - Code-First approach
  - Migrations
- **PostgreSQL 16** (Recomendado) o **SQL Server 2022**
  - Escalable y robusto
  - Soporte empresarial

### Arquitectura
```
┌─────────────────────────────────────────────────────┐
│           MAUI App (Mobile & Desktop)               │
│  ┌──────────┐  ┌──────────┐  ┌──────────────────┐ │
│  │  Views   │  │ViewModels│  │  Services/API    │ │
│  └──────────┘  └──────────┘  └──────────────────┘ │
└──────────────────────┬──────────────────────────────┘
                       │ HTTPS/REST
┌──────────────────────▼──────────────────────────────┐
│              ASP.NET Core Web API                   │
│  ┌─────────────────────────────────────────────┐   │
│  │         API Controllers/Endpoints           │   │
│  └─────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────┐   │
│  │      Application Layer (Use Cases)          │   │
│  │  - Commands (CQRS)                          │   │
│  │  - Queries (CQRS)                           │   │
│  │  - DTOs                                     │   │
│  │  - MediatR Pipeline                         │   │
│  └─────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────┐   │
│  │          Domain Layer (Core)                │   │
│  │  - Entities                                 │   │
│  │  - Value Objects                            │   │
│  │  - Domain Events                            │   │
│  │  - Business Rules                           │   │
│  └─────────────────────────────────────────────┘   │
│  ┌─────────────────────────────────────────────┐   │
│  │      Infrastructure Layer                   │   │
│  │  - EF Core DbContext                        │   │
│  │  - Repositories                             │   │
│  │  - External Services                        │   │
│  └─────────────────────────────────────────────┘   │
└──────────────────────┬──────────────────────────────┘
                       │
┌──────────────────────▼──────────────────────────────┐
│            PostgreSQL Database                      │
└─────────────────────────────────────────────────────┘
```

## 2. Estructura de Proyectos (Solution)

```
CashFlowSystem/
│
├── src/
│   ├── CashFlowSystem.Domain/              # Entidades, Value Objects, Interfaces
│   ├── CashFlowSystem.Application/         # Use Cases, DTOs, CQRS
│   ├── CashFlowSystem.Infrastructure/      # EF Core, Repositories, External Services
│   ├── CashFlowSystem.API/                 # ASP.NET Core Web API
│   └── CashFlowSystem.MAUI/                # Aplicación MAUI (Mobile & Desktop)
│       ├── Platforms/
│       │   ├── Android/
│       │   ├── iOS/
│       │   ├── Windows/
│       │   └── MacCatalyst/
│       ├── Views/
│       ├── ViewModels/
│       ├── Services/
│       └── Resources/
│
├── tests/
│   ├── CashFlowSystem.Domain.Tests/
│   ├── CashFlowSystem.Application.Tests/
│   ├── CashFlowSystem.Infrastructure.Tests/
│   └── CashFlowSystem.API.Tests/
│
└── docs/
    ├── api/
    └── user-guide/
```

## 3. Módulos del Sistema

### 3.1 Módulo de Autenticación y Seguridad
- Login/Registro
- Roles y permisos
- JWT Tokens
- Refresh tokens
- Multi-tenant (opcional para futuro)

### 3.2 Módulo de Flujo de Caja
#### Ingresos
- Registro de ingresos
- Categorización
- Métodos de pago
- Clientes (opcional)
- Comprobantes/Facturas

#### Egresos
- Registro de gastos
- Categorización
- Proveedores
- Comprobantes

#### Caja
- Apertura/Cierre de caja
- Arqueo de caja
- Movimientos del día
- Saldos

### 3.3 Módulo de Dashboard
- Resumen de flujo de caja (hoy, semana, mes)
- Gráficos:
  - Ingresos vs Egresos (líneas/barras)
  - Distribución por categorías (pie chart)
  - Tendencias mensuales
- Indicadores KPI:
  - Saldo actual
  - Ingresos del período
  - Egresos del período
  - Margen

### 3.4 Módulo de Informes
- Reporte de ingresos por período
- Reporte de egresos por período
- Reporte de flujo de caja consolidado
- Reporte por categorías
- Exportación (PDF, Excel)
- Filtros avanzados

### 3.5 Módulo de Configuración
- Categorías de ingresos/egresos
- Métodos de pago
- Moneda y formato
- Usuarios y permisos
- Respaldos

## 4. Entidades Principales (Domain)

### Transaction (Base)
- Id (Guid)
- Date (DateTime)
- Amount (decimal)
- Description (string)
- CategoryId (Guid)
- PaymentMethodId (Guid)
- UserId (Guid)
- CreatedAt (DateTime)
- UpdatedAt (DateTime)

### Income : Transaction
- CustomerId (Guid?) - opcional
- InvoiceNumber (string?)

### Expense : Transaction
- SupplierId (Guid?) - opcional
- ReceiptNumber (string?)

### Category
- Id (Guid)
- Name (string)
- Type (enum: Income/Expense)
- Color (string)
- Icon (string)
- IsActive (bool)

### PaymentMethod
- Id (Guid)
- Name (string)
- Type (enum: Cash, Card, Transfer, etc.)
- IsActive (bool)

### CashRegister
- Id (Guid)
- OpeningDate (DateTime)
- ClosingDate (DateTime?)
- OpeningBalance (decimal)
- ClosingBalance (decimal?)
- ExpectedBalance (decimal?)
- Difference (decimal?)
- UserId (Guid)
- Status (enum: Open, Closed)

### User
- Id (Guid)
- Username (string)
- Email (string)
- PasswordHash (string)
- Role (enum: Admin, Manager, Cashier)
- IsActive (bool)

## 5. Patrones y Prácticas

### CQRS (Command Query Responsibility Segregation)
```csharp
// Commands (Escritura)
public class CreateIncomeCommand : IRequest<IncomeDto>
{
    public decimal Amount { get; set; }
    public string Description { get; set; }
    // ...
}

// Queries (Lectura)
public class GetDashboardDataQuery : IRequest<DashboardDto>
{
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
}
```

### Repository Pattern
```csharp
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(Guid id);
    Task<IEnumerable<T>> GetAllAsync();
    Task<T> AddAsync(T entity);
    Task UpdateAsync(T entity);
    Task DeleteAsync(Guid id);
}
```

### Unit of Work
```csharp
public interface IUnitOfWork : IDisposable
{
    IRepository<Income> Incomes { get; }
    IRepository<Expense> Expenses { get; }
    IRepository<Category> Categories { get; }
    Task<int> SaveChangesAsync();
}
```

## 6. Seguridad

- **Autenticación**: JWT Bearer Tokens
- **Autorización**: Policy-based authorization
- **Encriptación**:
  - Passwords: BCrypt/Argon2
  - Data in transit: HTTPS/TLS
  - Data at rest: Transparent Data Encryption (TDE) en BD
- **Validación**: FluentValidation
- **Rate Limiting**: Protección contra ataques
- **CORS**: Configuración restrictiva

## 7. Escalabilidad

### Horizontal Scaling
- API stateless para múltiples instancias
- Load balancer (futuro)
- Database read replicas (futuro)

### Vertical Scaling
- Optimización de queries (índices)
- Caching (Redis - futuro)
- Compresión de respuestas

### Modularidad
- Cada módulo como Feature Folder
- Fácil agregar nuevos módulos:
  - Inventario
  - Facturación
  - CRM
  - Contabilidad completa
  - Multi-empresa

## 8. Tecnologías Complementarias

### Esenciales
- **AutoMapper** - Mapeo de objetos
- **FluentValidation** - Validación de datos
- **Serilog** - Logging estructurado
- **MediatR** - CQRS y mediator pattern
- **xUnit** - Testing

### UI/UX (MAUI)
- **CommunityToolkit.Maui** - Controles adicionales
- **CommunityToolkit.Mvvm** - MVVM helpers
- **SkiaSharp** - Gráficos avanzados
- **LiveChartsCore** - Gráficos para dashboard

### Opcional (Futuro)
- **SignalR** - Notificaciones en tiempo real
- **Hangfire** - Tareas programadas
- **Redis** - Caching distribuido
- **Docker** - Containerización
- **Elasticsearch** - Búsqueda avanzada

## 9. Plan de Desarrollo (Fases)

### Fase 1: Fundación (2-3 semanas)
- Configurar solución y proyectos
- Implementar arquitectura base
- Setup de base de datos
- Autenticación básica

### Fase 2: Core (3-4 semanas)
- Módulo de ingresos
- Módulo de egresos
- Módulo de caja
- CRUD de categorías y métodos de pago

### Fase 3: Visualización (2-3 semanas)
- Dashboard
- Gráficos básicos
- Informes simples

### Fase 4: Informes Avanzados (2 semanas)
- Reportes complejos
- Exportación PDF/Excel
- Filtros avanzados

### Fase 5: Pulido (1-2 semanas)
- Testing
- Optimización
- Documentación
- Deployment

## 10. Ventajas de esta Arquitectura

✅ **Escalable**: Fácil agregar nuevos módulos
✅ **Mantenible**: Separación clara de responsabilidades
✅ **Testeable**: Cada capa se puede testear independientemente
✅ **Cross-Platform**: Una sola base de código para todas las plataformas
✅ **Moderna**: Stack actualizado y con soporte a largo plazo
✅ **Performante**: Rendimiento nativo en todas las plataformas
✅ **Profesional**: Sigue los mejores estándares de la industria
✅ **C# Nativo**: Aprovechas tu experiencia existente

## 11. Alternativas Consideradas

### Si prefieres Web en lugar de nativa:
- **Blazor WebAssembly** - Para web
- **Electron + Blazor** - Para desktop
- **PWA** - Para móvil (experiencia limitada)

### Si quieres explorar fuera de .NET:
- **Flutter + Dart** - Excelente para mobile, desktop en desarrollo
- **React Native + TypeScript** - Mobile (desktop con Electron)
- **Avalonia UI** - Similar a MAUI pero más maduro en desktop

**Recomendación**: Mantener .NET MAUI por tu experiencia en C# y la madurez del ecosistema.
