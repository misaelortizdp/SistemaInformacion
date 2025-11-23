# Roadmap de Mejoras - Sistema de Gestión de Flujo de Caja

## Estado Actual de Implementación

### ✅ COMPLETADAS (Backend)

#### 1. CRUD Completo de Categorías
**Estado**: ✅ 100% Completado

**Implementado**:
- ✅ `CreateCategoryCommand` + Handler
- ✅ `UpdateCategoryCommand` + Handler
- ✅ `DeleteCategoryCommand` + Handler (con validación de transacciones asociadas)
- ✅ Endpoints POST, PUT, DELETE en `CategoriesController`
- ✅ Validación de integridad referencial

**Endpoints**:
- `GET /api/categories` - Listar categorías
- `POST /api/categories` - Crear categoría
- `PUT /api/categories/{id}` - Actualizar categoría
- `DELETE /api/categories/{id}` - Eliminar categoría

**Pendiente Frontend MAUI**:
- Pantalla de gestión de categorías
- Formulario crear/editar categoría
- Selector de íconos y colores

---

#### 2. CRUD Completo de Métodos de Pago
**Estado**: ✅ 100% Completado

**Implementado**:
- ✅ `CreatePaymentMethodCommand` + Handler
- ✅ `UpdatePaymentMethodCommand` + Handler
- ✅ `DeletePaymentMethodCommand` + Handler (con validación)
- ✅ Endpoints POST, PUT, DELETE en `PaymentMethodsController`

**Endpoints**:
- `GET /api/paymentmethods` - Listar métodos de pago
- `POST /api/paymentmethods` - Crear método de pago
- `PUT /api/paymentmethods/{id}` - Actualizar método de pago
- `DELETE /api/paymentmethods/{id}` - Eliminar método de pago

**Pendiente Frontend MAUI**:
- Pantalla de gestión de métodos de pago
- Formulario crear/editar método de pago

---

#### 3. Edición de Transacciones
**Estado**: ✅ 100% Completado (ya existía)

**Implementado**:
- ✅ `UpdateTransactionCommand` + Handler
- ✅ Endpoint PUT `/api/transactions/{id}`
- ✅ Validación de no editar transacciones de caja cerrada

**Pendiente Frontend MAUI**:
- Pantalla de edición de transacciones
- Validaciones de formulario

---

#### 4. Búsqueda Avanzada de Transacciones
**Estado**: ✅ 100% Completado

**Implementado**:
- ✅ `SearchTransactionsQuery` con filtros múltiples:
  - Búsqueda por texto (descripción, notas, referencia)
  - Rango de fechas
  - Rango de montos (min/max)
  - Múltiples categorías
  - Múltiples métodos de pago
  - Ordenamiento (fecha, monto, descripción)
  - Paginación (skip/take)
- ✅ `SearchTransactionsHandler` con lógica de filtrado
- ✅ Endpoint `POST /api/transactions/search`

**Endpoints**:
- `POST /api/transactions/search` - Búsqueda avanzada con JSON body

**Ejemplo de uso**:
```json
{
  "searchText": "compra",
  "startDate": "2025-01-01",
  "endDate": "2025-12-31",
  "minAmount": 100,
  "maxAmount": 500,
  "categoryIds": ["guid1", "guid2"],
  "orderBy": "date",
  "descending": true,
  "take": 20,
  "skip": 0
}
```

**Pendiente Frontend MAUI**:
- Pantalla de búsqueda avanzada
- Filtros UI (sliders para montos, date pickers, checkboxes de categorías)
- Lista de resultados con paginación

---

### 🟡 EN PROGRESO (Backend)

#### 5. Sistema de Presupuestos
**Estado**: 🟡 40% Completado

**Implementado**:
- ✅ Entidad `Budget` en Domain
- ✅ `BudgetDto` con cálculos (SpentAmount, RemainingAmount, PercentageUsed)
- ✅ `CreateBudgetCommand`
- ✅ `GetUserBudgetsQuery`

**Falta Backend**:
- ⏳ Handler para `CreateBudgetCommand`
- ⏳ Handler para `GetUserBudgetsQuery` con cálculo de gastos
- ⏳ `UpdateBudgetCommand` + Handler
- ⏳ `DeleteBudgetCommand` + Handler
- ⏳ Configuración EF Core para `Budget`
- ⏳ Agregar DbSet en ApplicationDbContext
- ⏳ `BudgetsController` con todos los endpoints
- ⏳ Migración de base de datos

**Pendiente Frontend MAUI**:
- Pantalla de presupuestos
- Formulario crear/editar presupuesto
- Visualización de progreso (barras de progreso, alertas de límite)
- Dashboard de presupuestos

**Funcionalidad esperada**:
- Crear presupuesto mensual por categoría
- Alertas cuando se acerca al límite (80%, 90%, 100%)
- Histórico de cumplimiento de presupuestos
- Visualización gráfica de % utilizado

---

### ⚪ PENDIENTES (Backend + Frontend)

#### 6. Exportación a PDF y Excel
**Estado**: ⚪ 0% Completado

**Requerimientos Backend**:
- Instalar paquetes NuGet:
  - `QuestPDF` o `iTextSharp` para PDF
  - `ClosedXML` para Excel
- Crear `IExportService` interface
- Implementar `PdfExportService`
- Implementar `ExcelExportService`
- Crear `ExportReportCommand` con opciones (PDF/Excel)
- Endpoint `POST /api/reports/export`

**Requerimientos Frontend MAUI**:
- Botón "Exportar" en pantalla de Reports
- Selector de formato (PDF/Excel)
- Descargar archivo y guardar/compartir

**Funcionalidades**:
- Exportar informe de transacciones a PDF con formato profesional
- Exportar a Excel con fórmulas y formato
- Compartir por email/WhatsApp
- Logo y branding en PDF

---

#### 7. Transacciones Recurrentes
**Estado**: ⚪ 0% Completado

**Requerimientos Backend**:
- Entidad `RecurringTransaction` en Domain:
  ```csharp
  public class RecurringTransaction : BaseEntity
  {
      public Guid UserId { get; set; }
      public string Description { get; set; }
      public decimal Amount { get; set; }
      public TransactionType Type { get; set; }
      public Guid CategoryId { get; set; }
      public Guid PaymentMethodId { get; set; }
      public RecurrenceType Frequency { get; set; } // Daily, Weekly, Monthly, Yearly
      public int FrequencyValue { get; set; } // Every X days/weeks/months
      public DateTime StartDate { get; set; }
      public DateTime? EndDate { get; set; }
      public DateTime? LastExecuted { get; set; }
      public DateTime? NextExecution { get; set; }
      public bool IsActive { get; set; }
  }
  ```
- Enum `RecurrenceType` (Daily, Weekly, Monthly, Yearly)
- CRUD Commands y Queries
- `RecurringTransactionService` para generar transacciones automáticas
- Background job/timer para ejecutar recurrentes
- `RecurringTransactionsController`

**Requerimientos Frontend MAUI**:
- Pantalla de transacciones recurrentes
- Formulario con configuración de frecuencia
- Lista de próximas ejecuciones
- Pausar/reactivar recurrentes

**Funcionalidades**:
- Configurar pagos mensuales (renta, servicios)
- Recordatorios antes de ejecutar
- Generación automática de transacciones
- Historial de ejecuciones

---

#### 8. Multi-Negocio / Cuentas Múltiples
**Estado**: ⚪ 0% Completado

**Requerimientos Backend**:
- Entidad `Business` en Domain:
  ```csharp
  public class Business : BaseEntity
  {
      public string Name { get; set; }
      public string? Description { get; set; }
      public string? Logo { get; set; }
      public string? Address { get; set; }
      public string? Phone { get; set; }
      public bool IsActive { get; set; }
  }
  ```
- Entidad `BusinessUser` (many-to-many):
  ```csharp
  public class BusinessUser
  {
      public Guid BusinessId { get; set; }
      public Guid UserId { get; set; }
      public UserRole Role { get; set; }
      public DateTime JoinedDate { get; set; }
  }
  ```
- Actualizar todas las entidades para agregar `BusinessId`:
  - `Transaction.BusinessId`
  - `Category.BusinessId`
  - `PaymentMethod.BusinessId`
  - `CashRegister.BusinessId`
  - `Budget.BusinessId`
- CRUD de `Business`
- Queries filtradas por `BusinessId`
- Endpoint para cambiar de negocio actual
- Claim de "CurrentBusinessId" en JWT

**Requerimientos Frontend MAUI**:
- Selector de negocio en Dashboard
- Pantalla de gestión de negocios
- Invitar usuarios a un negocio
- Dashboard consolidado (todos los negocios)

**Funcionalidades**:
- Gestionar varios negocios desde una app
- Cambio rápido entre negocios
- Dashboard consolidado
- Permisos por negocio

---

#### 9. Roles y Permisos Granulares
**Estado**: ⚪ 20% Completado (UserRole básico existe)

**Implementado actualmente**:
- ✅ Enum `UserRole` (Admin, User)
- ✅ Propiedad `User.Role`

**Requerimientos Backend**:
- Expandir `UserRole` enum:
  - Admin (acceso total)
  - Manager (gestión sin configuración)
  - Cashier (solo transacciones)
  - Accountant (informes, solo lectura de transacciones)
  - ReadOnly (solo visualización)
- Crear `Permission` enum con acciones específicas:
  - ViewTransactions, CreateTransaction, EditTransaction, DeleteTransaction
  - ViewReports, ExportReports
  - ManageCategories, ManagePaymentMethods
  - ManageCashRegister, ManageBudgets
  - ManageUsers, ManageSettings
- Crear `RolePermission` mapping
- Implementar `[Authorize(Policy = "CanDeleteTransaction")]`
- Middleware de validación de permisos
- Tablas de auditoría

**Requerimientos Frontend MAUI**:
- UI condicional según permisos
- Pantalla de gestión de usuarios (solo Admin)
- Asignación de roles

**Funcionalidades**:
- Control granular de acceso
- Múltiples usuarios por negocio
- Logs de auditoría
- Permisos personalizados

---

#### 10. Reportes Avanzados e Inteligencia
**Estado**: ⚪ 30% Completado (reporte básico existe)

**Implementado actualmente**:
- ✅ `GenerateReportQuery` básico (ingresos/egresos por rango)
- ✅ Dashboard con métricas simples

**Requerimientos Backend**:
- Nuevas queries de análisis:
  - `GetCategoryAnalysisQuery` - Top categorías de gasto
  - `GetTrendAnalysisQuery` - Tendencias mes a mes
  - `GetCashFlowProjectionQuery` - Proyección de flujo
  - `GetComparativeReportQuery` - Comparar períodos
  - `GetProfitLossQuery` - Estado de resultados
  - `GetBreakEvenQuery` - Punto de equilibrio
- Cálculos avanzados:
  - Promedio diario/mensual
  - Varianza y desviación estándar
  - Predicción con regresión lineal simple
- `AdvancedReportsController`

**Requerimientos Frontend MAUI**:
- Gráficos con `LiveCharts` o `Syncfusion`:
  - Gráfico de pastel (gastos por categoría)
  - Gráfico de líneas (tendencia temporal)
  - Gráfico de barras (comparativas)
- Dashboard analítico
- Exportación de gráficos

**Funcionalidades**:
- Análisis de patrones de gasto
- Detección de anomalías
- Sugerencias de optimización
- Predicción de flujo de caja
- Reportes comparativos
- KPIs financieros

---

## Frontend MAUI - Todas las Funcionalidades

### Pendiente de Implementar

1. **Gestión de Categorías**:
   - CategoriesPage.xaml
   - CategoriesViewModel
   - AddEditCategoryPage.xaml
   - AddEditCategoryViewModel

2. **Gestión de Métodos de Pago**:
   - PaymentMethodsPage.xaml
   - PaymentMethodsViewModel
   - AddEditPaymentMethodPage.xaml

3. **Edición de Transacciones**:
   - EditTransactionPage.xaml
   - EditTransactionViewModel
   - Integrar con TransactionsPage (tap para editar)

4. **Búsqueda Avanzada**:
   - AdvancedSearchPage.xaml
   - AdvancedSearchViewModel
   - Filtros UI (sliders, pickers, checkboxes)

5. **Presupuestos**:
   - BudgetsPage.xaml
   - BudgetsViewModel
   - AddEditBudgetPage.xaml
   - ProgressBars para visualización
   - Alertas de límite

6. **Gráficos**:
   - Instalar `LiveCharts.Maui` o `Syncfusion.Maui.Charts`
   - ChartsPage.xaml
   - Gráficos: Pastel, Líneas, Barras

7. **Exportación**:
   - Botón en ReportsPage
   - Selector de formato
   - File saver/share

8. **Multi-Negocio**:
   - BusinessSelectorControl
   - BusinessesPage.xaml
   - AddEditBusinessPage.xaml

9. **Transacciones Recurrentes**:
   - RecurringTransactionsPage.xaml
   - AddEditRecurringPage.xaml
   - Configurador de frecuencia

10. **Mejoras UI/UX**:
    - Modo oscuro
    - Onboarding tutorial
    - Notificaciones push
    - Caché offline
    - Skeleton loaders

---

## Priorización Sugerida

### Fase 1 - Fundamentos (Sprint 1-2)
1. ✅ CRUD Categorías + MAUI
2. ✅ CRUD Métodos de Pago + MAUI
3. ✅ Edición de Transacciones + MAUI
4. ✅ Búsqueda Avanzada + MAUI

### Fase 2 - Valor Alto (Sprint 3-4)
5. Sistema de Presupuestos completo (Backend + MAUI)
6. Gráficos y visualización (MAUI)
7. Exportación PDF/Excel (Backend + MAUI)

### Fase 3 - Automatización (Sprint 5-6)
8. Transacciones Recurrentes (Backend + MAUI)
9. Reportes Avanzados (Backend + MAUI)

### Fase 4 - Escalabilidad (Sprint 7-8)
10. Multi-Negocio (Backend + MAUI)
11. Roles y Permisos granulares (Backend + MAUI)

### Fase 5 - Polish (Sprint 9-10)
12. Modo oscuro
13. Notificaciones push
14. Modo offline
15. Onboarding
16. Tests E2E

---

## Estimación de Esfuerzo

| Funcionalidad | Backend | Frontend | Total | Prioridad |
|---------------|---------|----------|-------|-----------|
| CRUD Categorías | ✅ 0h | 8h | 8h | Alta |
| CRUD Métodos Pago | ✅ 0h | 6h | 6h | Alta |
| Edición Transacciones | ✅ 0h | 6h | 6h | Alta |
| Búsqueda Avanzada | ✅ 0h | 12h | 12h | Alta |
| Presupuestos | 8h | 16h | 24h | Alta |
| Exportación PDF/Excel | 12h | 4h | 16h | Media |
| Transacciones Recurrentes | 16h | 12h | 28h | Media |
| Multi-Negocio | 20h | 16h | 36h | Baja |
| Roles Granulares | 12h | 8h | 20h | Media |
| Reportes Avanzados | 16h | 20h | 36h | Alta |
| **TOTAL** | **84h** | **108h** | **192h** | |

**Total estimado**: ~5-6 semanas (1 desarrollador full-time)

---

## Stack Tecnológico Adicional Requerido

### Backend
- `QuestPDF` o `iTextSharp` - Generación de PDFs
- `ClosedXML` - Generación de Excel
- `Hangfire` - Background jobs para recurrentes

### Frontend MAUI
- `LiveCharts.Maui` o `Syncfusion.Maui.Charts` - Gráficos
- `CommunityToolkit.Maui.MediaElement` - Para onboarding videos
- `Plugin.LocalNotifications` - Notificaciones locales

### Infraestructura
- PostgreSQL (ya configurado)
- Redis (para caché de consultas pesadas)
- Azure Blob Storage o AWS S3 (para PDFs generados)

---

## Próximos Pasos Inmediatos

1. ✅ Commit de CRUD Categorías/Métodos Pago + Búsqueda Avanzada
2. ⏳ Completar Backend de Presupuestos
3. ⏳ Implementar Frontend MAUI de las 4 funcionalidades completadas
4. ⏳ Testing de integración
5. ⏳ Continuar con Fase 2

---

**Última actualización**: 2025-11-23
**Estado del proyecto**: Fase 1 en progreso (60% backend, 0% frontend de mejoras)
