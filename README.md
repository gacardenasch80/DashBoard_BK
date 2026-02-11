# 🚀 Proyecto DashBoard - Sistema de Análisis de Facturas Médicas

## 📋 Descripción

Backend completo en **.NET Core 8** con arquitectura limpia, totalmente funcional y listo para ejecutar.

### ✨ Características

- ✅ **Arquitectura Limpia** (4 capas separadas)
- ✅ **Inyección de Dependencias** completa
- ✅ **SQLite** con creación automática de BD
- ✅ **Migraciones automáticas** al iniciar
- ✅ **JWT Authentication**
- ✅ **AutoMapper** para DTOs
- ✅ **Swagger UI** integrado
- ✅ **CRUD completo** de Usuarios y Análisis
- ✅ **Usuario Admin** por defecto (Admin/123456)

---

## 🏗️ Estructura del Proyecto

```
DashBoard/
├── DashBoard.sln                          # Solución principal
│
├── DashBoard.Core/                        # Capa de Dominio
│   ├── Entities/
│   │   ├── Usuario.cs                    ✅
│   │   └── Analisis.cs                   ✅
│   └── Interfaces/
│       ├── IRepository.cs                 ✅
│       └── IUnitOfWork.cs                 ✅
│
├── DashBoard.Application/                 # Capa de Aplicación
│   ├── DTOs/
│   │   ├── UsuarioDto.cs                 ✅
│   │   ├── AuthDto.cs                    ✅
│   │   └── AnalisisDto.cs                ✅
│   ├── Services/
│   │   ├── IAuthService.cs               ✅
│   │   ├── AuthService.cs                ✅
│   │   ├── IUsuarioService.cs            ✅
│   │   ├── UsuarioService.cs             ✅
│   │   ├── IAnalisisService.cs           ✅
│   │   └── AnalisisService.cs            ✅
│   └── Mappings/
│       └── MappingProfile.cs              ✅
│
├── DashBoard.Infrastructure/              # Capa de Infraestructura
│   ├── Data/
│   │   └── ApplicationDbContext.cs       ✅
│   └── Repositories/
│       ├── Repository.cs                  ✅
│       └── UnitOfWork.cs                  ✅
│
└── DashBoard.API/                         # Capa de Presentación
    ├── Controllers/
    │   ├── AuthController.cs             ✅
    │   ├── UsuariosController.cs         ✅
    │   └── AnalisisController.cs         ✅
    ├── Program.cs                         ✅
    ├── appsettings.json                   ✅
    └── appsettings.Development.json       ✅
```

**Total: 21 archivos de código + configuración**

---

## ⚡ INICIO RÁPIDO

### Requisitos Previos

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0) instalado
- Editor de código (Visual Studio, VS Code, Rider)

### Paso 1: Restaurar Paquetes NuGet

```bash
cd DashBoard
dotnet restore
```

### Paso 2: Compilar el Proyecto

```bash
dotnet build
```

### Paso 3: Ejecutar la API

```bash
cd DashBoard.API
dotnet run
```

O con hot reload:

```bash
dotnet watch run
```

**¡Eso es todo!** 🎉

La base de datos SQLite se crea automáticamente en `DashBoard.API/Database/dashboard.db`

**Nota:** El proyecto usa `EnsureCreated()` que crea la BD automáticamente sin necesidad de migraciones. Esto es perfecto para SQLite y desarrollo rápido.

---

## 🌐 Acceder a Swagger

Abre tu navegador en:

```
https://localhost:XXXX/
```

(El puerto se muestra en la consola al ejecutar)

Verás la interfaz de Swagger UI con todos los endpoints documentados.

---

## 🧪 Probar la API

### 1. Login (Sin autenticación)

**Endpoint:** `POST /api/auth/login`

**Body:**
```json
{
  "username": "Admin",
  "password": "123456"
}
```

**Respuesta:**
```json
{
  "token": "eyJhbGciOiJIUzI1NiIs...",
  "expiration": "2024-02-06T06:00:00Z",
  "usuario": {
    "id": "11111111-1111-1111-1111-111111111111",
    "nombres": "Administrador",
    "apellidos": "Sistema",
    "username": "Admin",
    "nombreCompleto": "Administrador Sistema",
    "activo": true
  }
}
```

### 2. Autenticarse en Swagger

1. Copia el token de la respuesta
2. Haz clic en el botón **"Authorize"** 🔒 (arriba a la derecha)
3. Ingresa: `Bearer TU_TOKEN_AQUI`
4. Haz clic en **"Authorize"**

### 3. Probar Endpoints Protegidos

Ahora puedes usar todos los endpoints:

- `GET /api/usuarios` - Listar usuarios
- `POST /api/usuarios` - Crear usuario
- `PUT /api/usuarios/{id}` - Actualizar usuario
- `DELETE /api/usuarios/{id}` - Eliminar usuario
- `GET /api/analisis` - Listar análisis
- `POST /api/analisis` - Crear análisis

---

## 📝 Configuración de Base de Datos

### SQLite (Por Defecto)

La configuración en `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "SqliteConnection": "Data Source=./Database/dashboard.db"
  },
  "DatabaseSettings": {
    "UseSqlServer": false,
    "DatabasePath": "./Database"
  }
}
```

La carpeta y la base de datos se crean automáticamente.

### Cambiar a SQL Server (Opcional)

```json
{
  "ConnectionStrings": {
    "SqlServerConnection": "Server=localhost;Database=DashBoard;User Id=sa;Password=TuPassword;TrustServerCertificate=True;"
  },
  "DatabaseSettings": {
    "UseSqlServer": true
  }
}
```

---

## 🔄 Migraciones (Opcional)

Si quieres crear migraciones manualmente:

### Crear Migración

```bash
dotnet ef migrations add InitialCreate --project DashBoard.Infrastructure --startup-project DashBoard.API
```

### Aplicar Migración

```bash
dotnet ef database update --project DashBoard.Infrastructure --startup-project DashBoard.API
```

### Ver Migraciones

```bash
dotnet ef migrations list --project DashBoard.Infrastructure --startup-project DashBoard.API
```

**Nota:** Las migraciones se aplican automáticamente al iniciar la API.

---

## 🗄️ Esquema de Base de Datos

### Tabla: Usuarios

| Columna | Tipo | Descripción |
|---------|------|-------------|
| Id | GUID | Clave primaria |
| Nombres | string(100) | Nombres del usuario |
| Apellidos | string(100) | Apellidos del usuario |
| Username | string(50) | Username único |
| Password | string(255) | Contraseña hasheada (BCrypt) |
| Activo | bool | Estado del usuario |
| FechaCreacion | DateTime | Fecha de creación |
| FechaModificacion | DateTime? | Última modificación |

**Usuario por defecto:**
- Username: `Admin`
- Password: `123456`
- ID: `11111111-1111-1111-1111-111111111111`

### Tabla: Analisis

| Columna | Tipo | Descripción |
|---------|------|-------------|
| Id | GUID | Clave primaria |
| NombreAnalisis | string(200) | Nombre del análisis |
| UsuarioId | GUID | FK a Usuario |
| JsonData | nvarchar(max) | Datos JSON |
| FiltrosAplicados | nvarchar(max) | Filtros (JSON) |
| TotalFacturas | int | Cantidad de facturas |
| ValorTotal | decimal(18,2) | Valor total |
| FechaCreacion | DateTime | Fecha de creación |
| FechaModificacion | DateTime? | Última modificación |

---

## 🔐 Seguridad JWT

El token JWT incluye:
- **NameIdentifier**: ID del usuario
- **Name**: Username
- **GivenName**: Nombres
- **Expiración**: 8 horas

Configuración en `appsettings.json`:

```json
{
  "JwtSettings": {
    "SecretKey": "ClaveSecretaMuySeguraDeAlMenos32CaracteresParaDashBoard2024!",
    "Issuer": "DashBoardAPI",
    "Audience": "DashBoardClient",
    "ExpirationHours": 8
  }
}
```

---

## 🌐 CORS

Orígenes permitidos por defecto:

```json
{
  "Cors": {
    "AllowedOrigins": [
      "http://localhost:5500",
      "http://127.0.0.1:5500",
      "http://localhost:3000"
    ]
  }
}
```

---

## 📦 Paquetes NuGet Utilizados

### DashBoard.Application
- AutoMapper 12.0.1
- AutoMapper.Extensions.Microsoft.DependencyInjection 12.0.1
- BCrypt.Net-Next 4.0.3
- System.IdentityModel.Tokens.Jwt 7.3.1

### DashBoard.Infrastructure
- Microsoft.EntityFrameworkCore 8.0.0
- Microsoft.EntityFrameworkCore.Sqlite 8.0.0
- Microsoft.EntityFrameworkCore.SqlServer 8.0.0
- Microsoft.EntityFrameworkCore.Design 8.0.0
- Microsoft.EntityFrameworkCore.Tools 8.0.0

### DashBoard.API
- Microsoft.AspNetCore.Authentication.JwtBearer 8.0.0
- Microsoft.AspNetCore.OpenApi 8.0.0
- Swashbuckle.AspNetCore 6.5.0

---

## 🐛 Solución de Problemas

### Error: "No se encuentra dotnet"

**Solución:** Instala .NET 8 SDK desde https://dotnet.microsoft.com/download

### Error: "Puerto en uso"

**Solución:** Cambia el puerto en `Properties/launchSettings.json` o deja que se asigne automáticamente.

### Error: "No se puede crear la carpeta Database"

**Solución:** Ejecuta la aplicación como administrador o verifica permisos del directorio.

### La base de datos no se crea

**Solución:** Verifica que la ruta en `appsettings.json` sea correcta. Por defecto usa `./Database/dashboard.db` (ruta relativa al ejecutable).

---

## 📚 Próximos Pasos

Una vez que tengas la API funcionando:

1. ✅ Prueba el login con Admin/123456
2. ✅ Crea usuarios desde Swagger
3. ✅ Crea análisis de facturas
4. 🔄 Conecta tu frontend JavaScript
5. 🚀 Despliega en Azure/Railway/Render

---

## 🤝 Arquitectura y Principios

Este proyecto sigue:

- **Clean Architecture** (Arquitectura Limpia)
- **SOLID Principles**
- **Repository Pattern**
- **Unit of Work Pattern**
- **Dependency Injection**
- **DTO Pattern** (AutoMapper)
- **JWT Authentication**
- **Code-First** con Entity Framework

---

## ✅ Checklist de Funcionalidades

- [x] Autenticación JWT
- [x] CRUD de Usuarios
- [x] CRUD de Análisis
- [x] AutoMapper para DTOs
- [x] BCrypt para contraseñas
- [x] SQLite con auto-creación
- [x] Migraciones automáticas
- [x] Swagger UI
- [x] CORS configurado
- [x] Inyección de dependencias
- [x] Manejo de errores
- [x] Seed Data (Usuario Admin)

---

## 📞 Soporte

Si tienes problemas o preguntas:

1. Verifica que .NET 8 SDK esté instalado: `dotnet --version`
2. Asegúrate de ejecutar `dotnet restore` primero
3. Revisa los logs en la consola al iniciar la API
4. Verifica que el puerto no esté en uso

---

¡Proyecto listo para usar! 🎉🚀

**Desarrollado con .NET 8, Entity Framework Core y Clean Architecture**
