# CajaApp - Sistema de Registro de Caja

Aplicación de escritorio para gestión de caja con arquitectura limpia, desarrollada con .NET 10, WPF, EF Core y SQLite.

## 🚀 Características

### Módulo de Caja
- Crear, editar y eliminar cajas
- Abrir y cerrar cajas con saldo inicial
- Visualizar estado de todas las cajas
- Solo una caja puede estar abierta a la vez

### Módulo de Clientes
- Crear, editar y eliminar clientes
- Búsqueda por nombre, apellidos o email
- Gestión completa de información de clientes

### Módulo de Movimientos
- Registrar ingresos y egresos
- Asociar movimientos a cajas y clientes
- Categorización de movimientos
- Visualización de saldo actual
- Filtros por caja, fecha, tipo y cliente

## 🏗️ Arquitectura

La aplicación sigue los principios de **Arquitectura Limpia** con las siguientes capas:

```
CajaApp/
├── CajaApp.Domain/          # Capa de Dominio (Entidades, Value Objects, Interfaces)
├── CajaApp.Application/     # Capa de Aplicación (Casos de uso, DTOs, Servicios)
├── CajaApp.Infrastructure/  # Capa de Infraestructura (EF Core, Repositorios, SQLite, DI)
├── CajaApp.UI/              # Capa de Presentación (WPF, MVVM, Views, ViewModels)
└── CajaApp.Tests/           # Pruebas (xUnit, Moq)
```

### Patrones Implementados

- **Result Pattern**: Manejo de errores tipados con detalles
- **Repository Pattern**: Acceso a datos abstracto
- **Unit of Work**: Gestión de transacciones
- **Dependency Injection**: Inyección de dependencias con Microsoft.Extensions
- **MVVM**: Patrones de diseño para WPF
- **CQRS-like**: Separación de comandos y consultas

## 🛠️ Tecnologías

- **.NET 10**: Framework principal
- **WPF**: Interfaz de usuario
- **Entity Framework Core**: ORM para SQLite
- **SQLite**: Base de datos embebida
- **CommunityToolkit.Mvvm**: Librería MVVM (ObservableObject, RelayCommand)
- **xUnit**: Framework de pruebas unitarias
- **Moq**: Mocking para pruebas

## 📁 Estructura del Proyecto

### Domain Layer
- `Entities/`: Caja, Cliente, MovimientoCaja
- `ValueObjects/`: Monto
- `Enums/`: TipoMovimiento, EstadoCaja
- `Interfaces/`: IRepository, IUnitOfWork
- `Shared/`: Result pattern

### Application Layer
- `DTOs/`: Objetos de transferencia de datos
- `UseCases/`: Servicios de aplicación (CajaService, ClienteService, MovimientoService)
- `Interfaces/`: Interfaces de servicios
- `DependencyInjection.cs`: Configuración de DI

### Infrastructure Layer
- `Data/`: AppDbContext, DatabaseConfig
- `Repositories/`: Implementaciones de repositorios
- `UnitOfWork.cs`: Implementación de Unit of Work
- `DependencyInjection.cs`: Configuración de DI

### UI Layer
- `ViewModels/`: ViewModels con MVVM
- `Views/`: Vistas WPF (XAML)
- `Resources/`: Estilos, colores
- `Converters/`: Convertidores de valores
- `App.xaml`: Configuración de la aplicación

### Tests
- `Fixtures/`: Configuración de pruebas
- `UnitTests/`: Pruebas unitarias
- `IntegrationTests/`: Pruebas de integración

## 📥 Requisitos Previos

- .NET 10 SDK
- Visual Studio 2022 (opcional, para desarrollo)

## 🚀 Instalación y Ejecución

1. **Clonar el repositorio**
   ```bash
   git clone <url-del-repositorio>
   cd CajaApp
   ```

2. **Restaurar paquetes NuGet**
   ```bash
   dotnet restore
   ```

3. **Compilar la solución**
   ```bash
   dotnet build
   ```

4. **Ejecución**
   - Desde Visual Studio: Abrir la solución y ejecutar el proyecto `CajaApp.UI`
   - Desde CLI: `dotnet run --project CajaApp.UI`

## 🧪 Ejecutar Pruebas

```bash
# Ejecutar todas las pruebas
dotnet test

# Ejecutar pruebas específicas
dotnet test --filter "CajaServiceTests"
dotnet test --filter "EntityTests"
```

## 📝 Uso

1. **Abrir una caja**: Navegue a "Abrir Caja", seleccione una caja y especifique el saldo inicial
2. **Registrar movimientos**: Vaya a "Movimientos" y registre ingresos/egresos
3. **Gestionar clientes**: Use la sección "Clientes" para administrar clientes
4. **Cerrar caja**: Desde la sección "Cajas", seleccione una caja abierta y ciérrela

## 🎨 Interfaz de Usuario

La aplicación cuenta con:
- Diseño moderno y profesional
- Tema de colores consistente
- Mensajes de éxito y error claros
- Diálogos modales para operaciones
- Tabla de datos con filtros y ordenamiento

## 📊 Base de Datos

La aplicación utiliza SQLite con persistencia en archivo:
- Ubicación: `%LocalAppData%\CajaApp\cajaapp.db`
- Las migraciones se aplican automáticamente al iniciar

## 🔧 Configuración

### Cambiar la ubicación de la base de datos

Modificar en `CajaApp.UI/App.xaml.cs`:
```csharp
var dbPath = Path.Combine(
    Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
    "CajaApp",
    "cajaapp.db");
```

## ✨ Buenas Prácticas

- Separación de responsabilidades
- Inyección de dependencias
- Manejo de errores estructurado
- Pruebas unitarias
- Código limpio y legible
- Validación de datos
- Transacciones atómicas

## 🤝 Contribución

1. Crear un fork del repositorio
2. Crear una rama con la nueva funcionalidad (`git checkout -b feature/nueva-funcionalidad`)
3. Realizar commits con mensajes descriptivos
4. Enviar un Pull Request

## 📄 Licencia

MIT License

## 📞 Contacto

Para preguntas o sugerencias, por favor abra un issue en el repositorio.

---

**Nota**: El entorno de desarrollo actual no tiene .NET SDK instalado. Para compilar y ejecutar, asegúrese de tener .NET 10 SDK instalado en su máquina.
