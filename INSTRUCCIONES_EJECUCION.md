# 🚀 Instrucciones de Ejecución - CashFlowSystem

## ✅ Prerrequisitos Instalados

Antes de ejecutar el proyecto, asegúrate de tener instalado:

### 1. .NET 8 SDK
```bash
# Descargar e instalar desde:
https://dotnet.microsoft.com/download/dotnet/8.0

# Verificar instalación
dotnet --version
# Debe mostrar: 8.0.x
```

### 2. SQL Server (Base de Datos) ⭐ RECOMENDADO

**Opción A: SQL Server Express** (Gratuito)
```bash
# Descargar e instalar desde:
https://www.microsoft.com/en-us/sql-server/sql-server-downloads

# Seleccionar: Express Edition (gratuita)
```

**Opción B: SQL Server Developer Edition** (Gratuito, con todas las características)
```bash
# Descargar desde:
https://www.microsoft.com/en-us/sql-server/sql-server-downloads

# Requiere registro gratuito
```

**Opción C: LocalDB** (Incluido con Visual Studio)
```bash
# Si tienes Visual Studio instalado, LocalDB ya está disponible
# Connection String: Server=(localdb)\\mssqllocaldb;...
```

**Opción D: SQL Server con Docker**
```bash
docker run -e "ACCEPT_EULA=Y" -e "SA_PASSWORD=YourPassword123!" \
  -p 1433:1433 --name sqlserver-cashflow \
  -d mcr.microsoft.com/mssql/server:2022-latest
```

### 3. Visual Studio 2022 o VS Code
- **Visual Studio 2022**: https://visualstudio.microsoft.com/
  - Incluye: .NET 8, SQL Server LocalDB, Entity Framework Tools
- **VS Code**: https://code.visualstudio.com/ + Extensión C#

---

## 📦 Paso 1: Restaurar Paquetes NuGet

Abre una terminal en la carpeta raíz del proyecto y ejecuta:

```bash
cd /path/to/SistemaInformacion

# Restaurar todos los paquetes NuGet
dotnet restore
```

---

## 🗄️ Paso 2: Configurar Base de Datos

### Opción A: SQL Server (Configuración Actual) ⭐

El proyecto está configurado para usar **SQL Server** por defecto.

1. **Configurar connection string**:

Edita el archivo: `src/CashFlowSystem.API/appsettings.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=cashflow_db;User Id=sa;Password=TU_PASSWORD_AQUI;TrustServerCertificate=True;MultipleActiveResultSets=True"
  }
}
```

**Configuraciones comunes:**

- **SQL Server Express Local**:
  ```
  Server=localhost\\SQLEXPRESS;Database=cashflow_db;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;MultipleActiveResultSets=True
  ```

- **SQL Server LocalDB**:
  ```
  Server=(localdb)\\mssqllocaldb;Database=cashflow_db;Integrated Security=True;TrustServerCertificate=True;MultipleActiveResultSets=True
  ```

- **SQL Server en Docker**:
  ```
  Server=localhost,1433;Database=cashflow_db;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;MultipleActiveResultSets=True
  ```

2. **Crear la base de datos** (opcional - se crea automáticamente):

```sql
-- Si quieres crearla manualmente
CREATE DATABASE cashflow_db;
GO
```

### Opción B: PostgreSQL (Configuración Alternativa)

Si prefieres usar PostgreSQL en lugar de SQL Server:

1. **Instalar PostgreSQL**:
```bash
# Descargar e instalar desde:
https://www.postgresql.org/download/

# O usar Docker:
docker run --name postgres-cashflow -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16
```

2. **Modificar el proyecto Infrastructure**:

Edita: `src/CashFlowSystem.Infrastructure/CashFlowSystem.Infrastructure.csproj`

Reemplaza:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
```

Con:
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```

3. **Modificar Program.cs**:

En `src/CashFlowSystem.API/Program.cs`, reemplaza:
```csharp
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
```

Con:
```csharp
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
```

4. **Actualizar connection string**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=cashflow_db;Username=postgres;Password=postgres"
  }
}
```

5. **Restaurar paquetes**:
```bash
dotnet restore
```

---

## 🔨 Paso 3: Crear y Aplicar Migraciones

```bash
# Navegar a la carpeta del proyecto API
cd src/CashFlowSystem.API

# Crear la primera migración
dotnet ef migrations add InitialCreate --project ../CashFlowSystem.Infrastructure

# Aplicar la migración a la base de datos
dotnet ef database update --project ../CashFlowSystem.Infrastructure
```

**Nota**: Las migraciones crearán todas las tablas y agregarán los datos iniciales (categorías y métodos de pago).

**Si ya existen migraciones**, simplemente aplica:
```bash
dotnet ef database update --project ../CashFlowSystem.Infrastructure
```

---

## ▶️ Paso 4: Ejecutar la API Backend

### Opción A: Desde la Terminal

```bash
# Asegúrate de estar en la carpeta del proyecto API
cd src/CashFlowSystem.API

# Ejecutar la API
dotnet run
```

### Opción B: Desde Visual Studio

1. Abre `CashFlowSystem.sln` en Visual Studio
2. Establece `CashFlowSystem.API` como proyecto de inicio (clic derecho → Set as Startup Project)
3. Presiona `F5` o el botón "Start"

### Opción C: Desde VS Code

1. Abre la carpeta `SistemaInformacion` en VS Code
2. Presiona `F5` y selecciona ".NET Core Launch (web)"

---

## 🌐 Paso 5: Probar la API

Una vez que la API esté corriendo, verás en la consola algo como:

```
info: Microsoft.Hosting.Lifetime[14]
      Now listening on: https://localhost:7000
```

### Acceder a Swagger

Abre tu navegador y ve a:
```
https://localhost:7000
```

o

```
https://localhost:7000/swagger
```

Deberías ver la interfaz de Swagger con la documentación de la API.

### Endpoints Disponibles

La API incluye los siguientes módulos completos:

#### 🔐 Autenticación
- `POST /api/auth/register` - Registro de usuarios
- `POST /api/auth/login` - Login y generación de JWT

#### 📊 Categorías
- `GET /api/categories` - Listar categorías
- `GET /api/categories/{id}` - Obtener categoría por ID
- `POST /api/categories` - Crear categoría
- `PUT /api/categories/{id}` - Actualizar categoría
- `DELETE /api/categories/{id}` - Eliminar categoría

#### 💳 Métodos de Pago
- `GET /api/paymentmethods` - Listar métodos de pago
- `GET /api/paymentmethods/{id}` - Obtener método por ID
- `POST /api/paymentmethods` - Crear método de pago
- `PUT /api/paymentmethods/{id}` - Actualizar método de pago
- `DELETE /api/paymentmethods/{id}` - Eliminar método de pago

#### 💰 Transacciones
- `GET /api/transactions` - Listar transacciones
- `GET /api/transactions/{id}` - Obtener transacción por ID
- `POST /api/transactions` - Crear transacción
- `PUT /api/transactions/{id}` - Actualizar transacción
- `DELETE /api/transactions/{id}` - Eliminar transacción
- `POST /api/transactions/search` - Búsqueda avanzada con filtros

#### 💼 Caja Registradora
- `GET /api/cashregisters` - Listar cajas
- `POST /api/cashregisters/open` - Abrir caja
- `POST /api/cashregisters/close` - Cerrar caja

#### 📈 Presupuestos
- `GET /api/budgets/user/{userId}` - Obtener presupuestos del usuario
- `POST /api/budgets` - Crear presupuesto
- `PUT /api/budgets/{id}` - Actualizar presupuesto
- `DELETE /api/budgets/{id}` - Eliminar presupuesto

---

## 📱 Paso 6: Ejecutar la Aplicación MAUI (Opcional)

### Prerrequisitos para MAUI

**Windows (Desarrollo para Windows/Android)**:
```bash
# Instalar workloads de MAUI
dotnet workload install maui

# Verificar instalación
dotnet workload list
```

**macOS (Desarrollo para iOS/macOS/Android)**:
```bash
# Instalar workloads de MAUI
dotnet workload install maui

# Instalar Xcode desde App Store (para iOS/macOS)
```

### Configurar API URL

Edita: `src/CashFlowSystem.MAUI/Services/ApiService.cs`

```csharp
// Para Windows/macOS local
private readonly string _baseUrl = "https://localhost:7000/api";

// Para Android Emulator
private readonly string _baseUrl = "https://10.0.2.2:7000/api";

// Para dispositivo físico en la misma red
private readonly string _baseUrl = "https://TU_IP_LOCAL:7000/api";
```

### Ejecutar MAUI

```bash
# Navegar al proyecto MAUI
cd src/CashFlowSystem.MAUI

# Ejecutar en Windows
dotnet build -t:Run -f net8.0-windows10.0.19041.0

# Ejecutar en Android
dotnet build -t:Run -f net8.0-android

# Ejecutar en iOS (solo macOS)
dotnet build -t:Run -f net8.0-ios

# Ejecutar en macOS Catalyst (solo macOS)
dotnet build -t:Run -f net8.0-maccatalyst
```

### Funcionalidades MAUI Implementadas

La aplicación MAUI incluye las siguientes pantallas completamente funcionales:

#### ✅ Autenticación
- Login con validación JWT
- Registro de nuevos usuarios
- Almacenamiento seguro de tokens

#### ✅ Dashboard
- Resumen de ingresos y egresos
- Balance actual
- Transacciones recientes

#### ✅ Transacciones
- Lista de transacciones con filtros (Todas/Ingresos/Egresos)
- **NUEVO**: Búsqueda avanzada con múltiples criterios
  - Búsqueda por texto (descripción, notas, referencia)
  - Filtro por rango de fechas
  - Filtro por rango de montos
  - Filtro por tipo (Income/Expense)
  - Ordenamiento configurable
- Crear nueva transacción
- **NUEVO**: Editar transacciones existentes
  - Swipe izquierdo para editar
  - Tap en transacción para editar
  - Pre-población de todos los campos
- Eliminar transacciones (swipe derecho)
- Pull-to-refresh

#### ✅ Categorías
- **NUEVO**: CRUD completo de categorías
- Lista con filtro por tipo
- Crear/editar con selección de ícono y color
- Swipe-to-delete con confirmación
- Personalización visual (16 íconos, 8 colores)

#### ✅ Métodos de Pago
- **NUEVO**: CRUD completo de métodos de pago
- Lista con filtro por tipo (Cash/BankTransfer/Card/Other)
- Crear/editar con selección de ícono y color
- Swipe-to-delete con confirmación
- Personalización visual (16 íconos, 8 colores)

#### ✅ Informes
- Gráficos de ingresos y egresos
- Informes por período

#### ✅ Caja Registradora
- Abrir y cerrar caja
- Registro de movimientos
- Balance de caja

---

## 🧪 Paso 7: Ejecutar Tests

```bash
# Volver a la raíz del proyecto
cd ../..

# Ejecutar todos los tests
dotnet test

# Ejecutar tests con detalles
dotnet test --verbosity detailed

# Ejecutar tests con cobertura
dotnet test /p:CollectCoverage=true
```

---

## 🏗️ Compilar para Producción

### Backend API

```bash
# Compilar en modo Release
dotnet build --configuration Release

# Publicar la API
dotnet publish src/CashFlowSystem.API/CashFlowSystem.API.csproj -c Release -o ./publish
```

### MAUI App

```bash
# Compilar para Windows
dotnet publish src/CashFlowSystem.MAUI/CashFlowSystem.MAUI.csproj -f net8.0-windows10.0.19041.0 -c Release

# Compilar para Android (APK)
dotnet publish src/CashFlowSystem.MAUI/CashFlowSystem.MAUI.csproj -f net8.0-android -c Release

# Compilar para iOS (solo macOS)
dotnet publish src/CashFlowSystem.MAUI/CashFlowSystem.MAUI.csproj -f net8.0-ios -c Release
```

---

## 🐛 Troubleshooting

### Error: "No se puede conectar a la base de datos"

**Para SQL Server**:

1. Verifica que SQL Server esté corriendo:
```bash
# En Windows (Services)
services.msc
# Buscar: SQL Server (SQLEXPRESS o MSSQLSERVER)

# O usar SQL Server Configuration Manager
```

2. Verifica el connection string en `appsettings.json`

3. Prueba la conexión con SQL Server Management Studio (SSMS)

4. Verifica que SQL Server esté escuchando en el puerto correcto:
```sql
-- En SSMS, ejecutar:
SELECT DISTINCT local_net_address, local_tcp_port
FROM sys.dm_exec_connections
WHERE local_net_address IS NOT NULL
```

5. Si usas autenticación de SQL Server, asegúrate de que esté habilitada:
```sql
-- Habilitar autenticación mixta
USE master;
GO
EXEC xp_instance_regwrite
  N'HKEY_LOCAL_MACHINE',
  N'Software\Microsoft\MSSQLServer\MSSQLServer',
  N'LoginMode', REG_DWORD, 2;
GO
```

**Para PostgreSQL**:

1. Verifica que PostgreSQL esté corriendo:
```bash
# En Windows
pg_ctl status

# En Linux/Mac
sudo systemctl status postgresql
```

2. Verifica el connection string en `appsettings.json`

3. Verifica que el usuario y contraseña sean correctos

### Error: "dotnet command not found"

- .NET SDK no está instalado o no está en el PATH
- Reinstala .NET SDK y reinicia la terminal

### Error: "Cannot find project"

- Asegúrate de estar en la carpeta correcta
- Verifica que todos los archivos `.csproj` existan

### Error al crear migraciones

```bash
# Limpiar y reconstruir
dotnet clean
dotnet build

# Eliminar migraciones anteriores si es necesario
dotnet ef migrations remove --project src/CashFlowSystem.Infrastructure

# Crear nuevamente
dotnet ef migrations add InitialCreate --project src/CashFlowSystem.Infrastructure
```

### Error: "TrustServerCertificate=True" requerido

Si obtienes errores de certificado SSL con SQL Server, asegúrate de incluir `TrustServerCertificate=True` en tu connection string:

```json
"DefaultConnection": "Server=localhost;Database=cashflow_db;User Id=sa;Password=YourPassword123!;TrustServerCertificate=True;MultipleActiveResultSets=True"
```

### MAUI no compila

```bash
# Reinstalar workloads
dotnet workload uninstall maui
dotnet workload install maui

# Limpiar y restaurar
dotnet clean
dotnet restore
dotnet build
```

---

## 📊 Verificar que Todo Funciona

### 1. Base de Datos Creada

**SQL Server**:
```sql
-- Conectar con SSMS o Azure Data Studio
-- Ver tablas
SELECT TABLE_NAME
FROM INFORMATION_SCHEMA.TABLES
WHERE TABLE_TYPE = 'BASE TABLE'
ORDER BY TABLE_NAME;

-- Deberías ver:
-- Budgets
-- Categories
-- CashRegisters
-- PaymentMethods
-- Transactions
-- Users
```

**PostgreSQL**:
```sql
-- Conectar a PostgreSQL
psql -U postgres -d cashflow_db

-- Listar tablas
\dt

-- Deberías ver las mismas tablas
```

### 2. Datos Iniciales Cargados

```sql
-- Ver categorías iniciales
SELECT * FROM Categories;

-- Ver métodos de pago iniciales
SELECT * FROM PaymentMethods;
```

### 3. API Funcionando
- ✅ Swagger accesible en `https://localhost:7000`
- ✅ Todos los endpoints responden correctamente
- ✅ Autenticación JWT funciona
- ✅ CRUD completo para todas las entidades

### 4. MAUI App Funcionando
- ✅ Login exitoso
- ✅ Navegación entre pantallas
- ✅ CRUD de transacciones
- ✅ CRUD de categorías
- ✅ CRUD de métodos de pago
- ✅ Búsqueda avanzada funcional
- ✅ Edición de transacciones

---

## 🎯 Estado del Proyecto

### ✅ Completado (Backend)

1. ✅ **Infraestructura Base** (Clean Architecture + CQRS)
2. ✅ **Autenticación y Autorización** (JWT + BCrypt)
3. ✅ **CRUD Usuarios**
4. ✅ **CRUD Categorías** (con validaciones)
5. ✅ **CRUD Métodos de Pago** (con validaciones)
6. ✅ **CRUD Transacciones** (crear, leer, actualizar, eliminar)
7. ✅ **Búsqueda Avanzada de Transacciones** (múltiples filtros)
8. ✅ **Sistema de Presupuestos** (con cálculos automáticos)
9. ✅ **Caja Registradora** (abrir/cerrar)
10. ✅ **Migraciones EF Core**
11. ✅ **Seed Data** (categorías y métodos de pago iniciales)
12. ✅ **Logging con Serilog**
13. ✅ **Documentación Swagger**
14. ✅ **Migración a SQL Server**

### ✅ Completado (Frontend MAUI)

1. ✅ **Autenticación** (Login/Register con JWT)
2. ✅ **Dashboard** (resumen financiero)
3. ✅ **Transacciones** (lista, crear, **editar**, eliminar)
4. ✅ **Búsqueda Avanzada** (filtros múltiples)
5. ✅ **CRUD Categorías** (completo con UI personalizable)
6. ✅ **CRUD Métodos de Pago** (completo con UI personalizable)
7. ✅ **Informes** (gráficos básicos)
8. ✅ **Caja Registradora** (apertura/cierre)
9. ✅ **Navegación Shell**
10. ✅ **Almacenamiento Seguro** (tokens)
11. ✅ **Pull-to-refresh**
12. ✅ **Swipe gestures** (edit/delete)

### 🚧 Pendiente (Mejoras Opcionales)

1. ⏳ **Frontend: Visualización de Presupuestos** (UI para budgets)
2. ⏳ **Frontend: Gráficos Avanzados** (LiveCharts)
3. ⏳ **Backend: Exportación PDF/Excel** (QuestPDF + ClosedXML)
4. ⏳ **Backend: Transacciones Recurrentes** (Background Service)
5. ⏳ **Backend: Multi-Empresa** (arquitectura multi-tenant)
6. ⏳ **Backend: Roles Granulares** (permisos detallados)
7. ⏳ **Backend: Informes Avanzados** (analytics)

---

## 📞 Ayuda Adicional

Si encuentras algún problema:

1. Revisa los logs en la consola
2. Revisa los archivos de log en `src/CashFlowSystem.API/logs/`
3. Consulta la documentación oficial:
   - .NET: https://learn.microsoft.com/en-us/dotnet/
   - EF Core: https://learn.microsoft.com/en-us/ef/core/
   - SQL Server: https://learn.microsoft.com/en-us/sql/
   - PostgreSQL: https://www.postgresql.org/docs/
   - .NET MAUI: https://learn.microsoft.com/en-us/dotnet/maui/

---

## 🎉 ¡Listo!

Si todo funcionó correctamente, ya tienes:

### Backend API
- ✅ Clean Architecture + CQRS implementada
- ✅ Base de datos SQL Server configurada
- ✅ API REST completa con 6 módulos
- ✅ Autenticación JWT funcional
- ✅ Búsqueda avanzada de transacciones
- ✅ Sistema de presupuestos
- ✅ 40+ endpoints documentados en Swagger

### Frontend MAUI
- ✅ Aplicación multiplataforma (Windows/Android/iOS/macOS)
- ✅ 8 pantallas completas implementadas
- ✅ CRUD completo para 3 entidades principales
- ✅ Búsqueda avanzada con filtros
- ✅ Edición inline de transacciones
- ✅ UI moderna con gestos táctiles

**¡El sistema está completamente funcional y listo para usar!** 🎊
