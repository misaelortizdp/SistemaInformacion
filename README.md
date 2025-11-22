# CashFlowSystem - Sistema de Gestión de Flujo de Caja

Sistema de información multiplataforma (Windows, macOS, iOS, Android) para gestionar el flujo de caja de cualquier negocio.

## 🚀 Características

- ✅ **Cross-Platform**: Una sola base de código para escritorio y móvil
- 📊 **Dashboard Interactivo**: Visualización en tiempo real de tu flujo de caja
- 💰 **Gestión de Ingresos y Egresos**: Control completo de tus finanzas
- 📈 **Informes Detallados**: Exportación a PDF y Excel
- 🏗️ **Arquitectura Escalable**: Fácil de mantener y extender
- 🔒 **Seguridad**: Autenticación JWT y encriptación de datos

## 🛠️ Stack Tecnológico

### Frontend
- .NET 8 MAUI (Cross-platform UI)
- MVVM Pattern
- CommunityToolkit.Mvvm

### Backend
- ASP.NET Core 8 Web API
- Entity Framework Core 8
- PostgreSQL / SQL Server
- MediatR (CQRS Pattern)

### Arquitectura
- Clean Architecture
- CQRS Pattern
- Repository Pattern
- Unit of Work Pattern
- Dependency Injection

## 📁 Estructura del Proyecto

```
CashFlowSystem/
├── src/
│   ├── CashFlowSystem.Domain/          # Entidades, Value Objects, Interfaces
│   ├── CashFlowSystem.Application/     # Use Cases, DTOs, CQRS
│   ├── CashFlowSystem.Infrastructure/  # EF Core, Repositories
│   ├── CashFlowSystem.API/             # Web API
│   └── CashFlowSystem.MAUI/            # App Multiplataforma
├── tests/
│   ├── CashFlowSystem.Domain.Tests/
│   ├── CashFlowSystem.Application.Tests/
│   ├── CashFlowSystem.Infrastructure.Tests/
│   └── CashFlowSystem.API.Tests/
└── docs/
```

## 🚦 Comenzando

### Prerrequisitos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Visual Studio 2022](https://visualstudio.microsoft.com/) o [Visual Studio Code](https://code.visualstudio.com/)
- PostgreSQL 16+ o SQL Server 2022+

### Instalación

1. Clonar el repositorio
```bash
git clone [repository-url]
cd SistemaInformacion
```

2. Restaurar dependencias
```bash
dotnet restore
```

3. Configurar base de datos
```bash
cd src/CashFlowSystem.API
dotnet ef database update
```

4. Ejecutar la API
```bash
dotnet run --project src/CashFlowSystem.API
```

5. Ejecutar la aplicación MAUI
```bash
dotnet run --project src/CashFlowSystem.MAUI
```

## 📚 Documentación

- [Plan de Arquitectura](./PLAN_ARQUITECTURA.md)
- [Guía de Desarrollo](./GUIA_DESARROLLO.md)
- [API Documentation](./docs/api/)

## 🧪 Testing

```bash
dotnet test
```

## 🤝 Contribuir

Las contribuciones son bienvenidas. Por favor:

1. Fork el proyecto
2. Crea tu rama de feature (`git checkout -b feature/AmazingFeature`)
3. Commit tus cambios (`git commit -m 'Add some AmazingFeature'`)
4. Push a la rama (`git push origin feature/AmazingFeature`)
5. Abre un Pull Request

## 📄 Licencia

[Especificar licencia]

## 👥 Autores

[Tu nombre/organización]

## 🙏 Agradecimientos

- .NET MAUI Team
- ASP.NET Core Team
- Comunidad de desarrolladores
