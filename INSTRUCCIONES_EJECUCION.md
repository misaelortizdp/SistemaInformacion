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

### 2. PostgreSQL (Base de Datos)
```bash
# Descargar e instalar desde:
https://www.postgresql.org/download/

# Alternativa: Usar Docker
docker run --name postgres-cashflow -e POSTGRES_PASSWORD=postgres -p 5432:5432 -d postgres:16
```

### 3. Visual Studio 2022 o VS Code
- **Visual Studio 2022**: https://visualstudio.microsoft.com/
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

### Opción A: PostgreSQL Local

1. **Crear la base de datos**:
```bash
# Conectar a PostgreSQL
psql -U postgres

# Dentro de psql:
CREATE DATABASE cashflow_db;
\q
```

2. **Configurar connection string**:

Edita el archivo: `src/CashFlowSystem.API/appsettings.Development.json`

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=cashflow_db;Username=postgres;Password=TU_PASSWORD_AQUI"
  }
}
```

### Opción B: SQL Server

Si prefieres usar SQL Server en lugar de PostgreSQL:

1. **Modificar el proyecto Infrastructure**:

Edita: `src/CashFlowSystem.Infrastructure/CashFlowSystem.Infrastructure.csproj`

Reemplaza:
```xml
<PackageReference Include="Npgsql.EntityFrameworkCore.PostgreSQL" Version="8.0.0" />
```

Con:
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
```

2. **Modificar Program.cs**:

En `src/CashFlowSystem.API/Program.cs`, reemplaza:
```csharp
options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"))
```

Con:
```csharp
options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection"))
```

3. **Actualizar connection string**:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=CashFlowDb;Trusted_Connection=True;"
  }
}
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

---

## ▶️ Paso 4: Ejecutar la API

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

### Probar el endpoint de Health

```bash
curl https://localhost:7000/api/health
```

Deberías obtener una respuesta como:
```json
{
  "status": "Healthy",
  "timestamp": "2024-01-15T10:30:00Z",
  "version": "1.0.0",
  "service": "CashFlow System API"
}
```

---

## 🧪 Paso 6: Ejecutar Tests

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

```bash
# Compilar en modo Release
dotnet build --configuration Release

# Publicar la API
dotnet publish src/CashFlowSystem.API/CashFlowSystem.API.csproj -c Release -o ./publish
```

---

## 🐛 Troubleshooting

### Error: "No se puede conectar a la base de datos"

1. Verifica que PostgreSQL esté corriendo:
```bash
# En Windows
pg_ctl status

# En Linux/Mac
sudo systemctl status postgresql
```

2. Verifica el connection string en `appsettings.Development.json`

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

---

## 📊 Verificar que Todo Funciona

### 1. Base de Datos Creada
```sql
-- Conectar a PostgreSQL
psql -U postgres -d cashflow_db

-- Listar tablas
\dt

-- Deberías ver:
-- users
-- categories
-- payment_methods
-- transactions
-- cash_registers
```

### 2. Datos Iniciales Cargados
```sql
-- Ver categorías iniciales
SELECT * FROM categories;

-- Ver métodos de pago iniciales
SELECT * FROM payment_methods;
```

### 3. API Funcionando
- Swagger accesible en `https://localhost:7000`
- Health endpoint responde correctamente

---

## 🎯 Próximos Pasos

Una vez que la API esté corriendo correctamente:

1. ✅ **Implementar Autenticación** (Fase 2 del roadmap)
2. ✅ **Crear endpoints de transacciones** (Fase 3)
3. ✅ **Desarrollar la aplicación MAUI** (móvil/escritorio)
4. ✅ **Implementar Dashboard** (Fase 4)

---

## 📞 Ayuda Adicional

Si encuentras algún problema:

1. Revisa los logs en la consola
2. Revisa los archivos de log en `src/CashFlowSystem.API/logs/`
3. Consulta la documentación oficial:
   - .NET: https://learn.microsoft.com/en-us/dotnet/
   - EF Core: https://learn.microsoft.com/en-us/ef/core/
   - PostgreSQL: https://www.postgresql.org/docs/

---

## 🎉 ¡Listo!

Si todo funcionó correctamente, ya tienes:
- ✅ Infraestructura base implementada
- ✅ Base de datos configurada
- ✅ API corriendo
- ✅ Listo para comenzar a agregar funcionalidades

**¡Felicitaciones! Estás listo para comenzar con la Fase 2 del desarrollo.**
