# 🏠 RealEstate API

Una API REST moderna para la gestión de propiedades inmobiliarias construida con .NET 8, siguiendo arquitectura Clean Architecture y principios CQRS.

## 📋 Tabla de Contenidos

- [Características](#-características)
- [Tecnologías](#-tecnologías)
- [Arquitectura](#-arquitectura)
- [Instalación](#-instalación)
- [Configuración](#-configuración)
- [Uso de la API](#-uso-de-la-api)
- [Endpoints](#-endpoints)
- [Autenticación](#-autenticación)
- [Ejemplos](#-ejemplos)
- [Health Checks](#-health-checks)
- [Observabilidad y Métricas](#-observabilidad-y-métricas)
- [Manejo de Errores](#-manejo-de-errores)
- [Performance y Cache](#-performance-y-cache)
- [Testing](#-testing)
- [Contribución](#-contribución)

## ✨ Características

- 🏗️ **Clean Architecture** - Separación clara de responsabilidades
- 🔄 **CQRS Pattern** - Comandos y Queries separados
- 🔐 **JWT Authentication** - Autenticación segura con tokens
- 📊 **Entity Framework Core** - ORM para acceso a datos
- ✅ **FluentValidation** - Validación robusta de datos
- 🧪 **Unit Testing** - Cobertura completa con NUnit
- 📚 **Swagger/OpenAPI** - Documentación interactiva
- 🚀 **Minimal APIs** - Endpoints modernos y eficientes
- 🏥 **Health Checks** - Monitoreo de sistema y base de datos
- 🛡️ **Global Exception Handler** - Manejo robusto de errores con logging estructurado
- ⚡ **Performance Optimization** - Cache inteligente y paginación optimizada
- 📊 **Observabilidad** - Métricas en tiempo real y logging estructurado

## 🛠️ Tecnologías

### Backend
- **.NET 8** - Framework principal
- **ASP.NET Core** - Web API
- **Entity Framework Core** - ORM
- **MediatR** - Patrón Mediator para CQRS
- **FluentValidation** - Validación de datos
- **JWT Bearer** - Autenticación
- **Memory Cache** - Cache en memoria para performance
- **Health Checks** - Monitoreo de sistema

### Testing
- **NUnit** - Framework de testing
- **Moq** - Mocking de dependencias
- **FluentAssertions** - Assertions expresivas
- **Microsoft.AspNetCore.Mvc.Testing** - Testing de endpoints

### Base de Datos
- **SQL Server** - Base de datos principal
- **Database First** - Generación de modelos desde BD existente
- **Cadena de Conexión Encriptada** - Seguridad en configuración de BD usando `SecureCryptoHelper`

## 🏛️ Arquitectura

```
RealEstate/
├── RealEstate.Api/           # Capa de presentación (Endpoints)
├── RealEstate.Application/   # Capa de aplicación (CQRS)
├── RealEstate.Domain/        # Capa de dominio (Contratos)
├── RealEstate.Infrastructure/# Capa de infraestructura (BD, Repos)
├── RealEstate.Shared/        # DTOs y utilidades compartidas
└── RealEstate.Test/          # Tests unitarios
```

### Principios Arquitectónicos
- **Clean Architecture** - Dependencias apuntan hacia adentro
- **CQRS** - Separación de comandos y queries
- **Repository Pattern** - Abstracción del acceso a datos
- **Dependency Injection** - Inversión de control

## 🚀 Instalación

### Prerrequisitos
- .NET 8 SDK
- SQL Server (LocalDB o instancia completa)
- Visual Studio 2022 o VS Code

### Pasos de Instalación

1. **Clonar el repositorio**
```bash
git clone <repository-url>
cd realestate
```

2. **Restaurar dependencias**
```bash
dotnet restore
```

3. **Configurar la base de datos**
```bash
# PASO 1: Crear la base de datos ejecutando el script SQL
# El archivo RealEstateDB.sql está en la raíz del proyecto
# Ejecutar este script en SQL Server Management Studio o tu herramienta SQL preferida

# PASO 2: Generar los modelos Entity Framework (Database First)
# Establecer RealEstate.Infrastructure como proyecto de inicio
# Ejecutar el archivo RealState.BAT en Infrastructure/BAT/ para generar los modelos desde la BD existente
```

4. **Ejecutar la aplicación**
```bash
dotnet run --project RealEstate.Api
```

## ⚙️ Configuración

### appsettings.json
```json
{
  "JWT": {
    "secret": "7h3=Dr34m=734m=Sk4nD14=api_RealEstate2024!",
    "expire": "30",
    "issuer": "https://localhost:7288/",
    "audience": "https://localhost:7288/",
    "scopes": "AQ"
  },
  "Password": "B9KhlTBu8K2N/XaoT/rxUQ==",
  "DbSetting": {
    "ConnectionString": "yD00gz9wj1cyI2AClEgGknc244AFidCgXkZRuCzMLSebNgWz53WMQOr0/TqZO/rNiQ8hXSBd4bKyD+5OkqmrNFLr/rc5X3GExIQze9AlOX2UpTOrUzLH8+FqvRuv+Kq5w/1NFnMdu5FWpKa/Ja6Pww==",
    "KeyEncrypt": "_RealEstate"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*"
}
```

### Configuración de Base de Datos (Database First)
El proyecto utiliza un enfoque **Database First** con los siguientes pasos:

1. **Crear Base de Datos**: Ejecutar `RealEstateDB.sql` desde la raíz del proyecto
2. **Generar Modelos**: Usar `RealState.BAT` en `Infrastructure/BAT/` para generar los modelos Entity Framework desde la BD existente
3. **Cadena de Conexión Encriptada**: La conexión se maneja a través de `DbSetting.ConnectionString` con encriptación usando `SecureCryptoHelper`

#### Encriptar Cadena de Conexión
Para configurar tu propia cadena de conexión encriptada:

```csharp
using RealEstate.Shared.Utils;

// Encriptar tu cadena de conexión
string connectionString = "Server=localhost;Database=RealEstateDB;Trusted_Connection=true;";
string encryptedConnection = SecureCryptoHelper.Encrypt(connectionString, "_RealEstate");

// Usar el resultado en appsettings.json
// "ConnectionString": "tu-cadena-encriptada-aqui"
```

#### Desencriptar Cadena de Conexión
La aplicación desencripta automáticamente la cadena usando:

```csharp
string decryptedConnection = SecureCryptoHelper.Decrypt(encryptedConnection, "_RealEstate");
```

### Variables de Entorno
- `ASPNETCORE_ENVIRONMENT` - Ambiente (Development/Production)
- `DbSetting__ConnectionString` - Cadena de conexión encriptada de BD (usar SecureCryptoHelper.Encrypt)
- `DbSetting__KeyEncrypt` - Clave para desencriptar la cadena de conexión (por defecto: "_RealEstate")

## 📖 Uso de la API

### Swagger UI
Una vez ejecutada la aplicación, accede a:
- **Swagger UI**: `https://localhost:7000/swagger`
- **API Base URL**: `https://localhost:7000/api`
- **Health Checks**: `https://localhost:7000/health/`
- **Métricas**: `https://localhost:7000/metrics/`

### Autenticación
La mayoría de endpoints requieren autenticación JWT. Primero auténticate:

```bash
POST /api/authentication/Authenticate
{
  "ownerId": 1
}
```

Usa el token devuelto en el header `Authorization: Bearer <token>`

## 🔗 Endpoints

### 🔐 Autenticación
| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| POST | `/api/authentication/Authenticate` | Autenticar usuario | ❌ |

### 🏠 Propiedades
| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| POST | `/api/realEstate/CreateProperty` | Crear propiedad | ✅ |
| POST | `/api/realEstate/AddImageProperty` | Agregar imagen | ✅ |
| POST | `/api/realEstate/ChangePrice` | Cambiar precio | ✅ |
| POST | `/api/realEstate/UpdateProperty` | Actualizar propiedad | ✅ |
| POST | `/api/realEstate/ListPropertyWithFilters` | Listar propiedades | ✅ |

### 🏥 Health Checks
| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| GET | `/health/` | Health check básico | ❌ |
| GET | `/health/detailed` | Health check detallado | ❌ |
| GET | `/health/ping` | Health check simple para load balancers | ❌ |

### 📊 Métricas
| Método | Endpoint | Descripción | Autenticación |
|--------|----------|-------------|---------------|
| GET | `/metrics/` | Métricas completas del sistema | ❌ |
| GET | `/metrics/cache` | Métricas de cache | ❌ |
| GET | `/metrics/requests` | Métricas de requests | ❌ |

## 🔐 Autenticación

### Obtener Token
```bash
curl -X POST "https://localhost:7000/api/authentication/Authenticate" \
  -H "Content-Type: application/json" \
  -d '{"ownerId": 1}'
```

### Usar Token
```bash
curl -X POST "https://localhost:7000/api/realEstate/CreateProperty" \
  -H "Authorization: Bearer <your-token>" \
  -H "Content-Type: application/json" \
  -d '{"name": "Casa Ejemplo", "price": 150000, ...}'
```

## 💡 Ejemplos

### 1. Crear una Propiedad
```json
POST /api/realEstate/CreateProperty
{
  "name": "Casa Moderna",
  "address": "123 Calle Principal",
  "price": 250000,
  "codeInternal": "CASA001",
  "year": 2020,
  "idOwner": 1,
  "images": [
    {
      "file": "data:image/jpeg;base64,/9j/4AAQSkZJR...",
      "enabled": true
    }
  ]
}
```

### 2. Agregar Imagen a Propiedad
```json
POST /api/realEstate/AddImageProperty
{
  "propertyId": 1,
  "imageFile": "base64-encoded-image-data",
  "enabled": true
}
```

### 3. Cambiar Precio
```json
POST /api/realEstate/ChangePrice
{
  "propertyId": 1,
  "newPrice": 275000,
  "tax": 5000,
  "note": "Actualización por mejoras",
  "dateSale": "2024-01-15T10:30:00Z"
}
```

### 4. Listar Propiedades con Filtros
```json
POST /api/realEstate/ListPropertyWithFilters
{
  "idOwner": 1,
  "minPrice": 100000,
  "maxPrice": 500000,
  "minYear": 2015,
  "page": 1,
  "pageSize": 10
}
```

## 🏥 Health Checks

### Monitoreo del Sistema
Los health checks proporcionan información en tiempo real sobre el estado de la aplicación:

```bash
# Health check básico
curl https://localhost:7000/health/

# Health check detallado
curl https://localhost:7000/health/detailed

# Health check para load balancers
curl https://localhost:7000/health/ping
```

### Componentes Monitoreados
- **Base de Datos**: Conectividad, existencia de tablas, conteo de registros
- **Sistema**: Memoria, uptime, espacio en disco
- **Memoria**: Uso actual vs límite configurado

## 📊 Observabilidad y Métricas

### Métricas Disponibles
```bash
# Métricas completas
curl https://localhost:7000/metrics/

# Métricas de cache
curl https://localhost:7000/metrics/cache

# Métricas de requests
curl https://localhost:7000/metrics/requests
```

### Información Recopilada
- **Request Metrics**: Counts por endpoint, duraciones promedio
- **Cache Metrics**: Hit/miss rates, performance
- **Database Metrics**: Duración de queries
- **System Metrics**: Uso de memoria, uptime

## 🛡️ Manejo de Errores

### Global Exception Handler
El sistema incluye manejo robusto de errores con:

- **Correlation IDs**: Para tracking de requests
- **Logging Estructurado**: Con contexto completo
- **Códigos de Error Específicos**: Para cada tipo de excepción
- **Headers Útiles**: X-Correlation-ID, X-Error-Code

### Tipos de Errores Manejados
- `ValidationException` → 400 Bad Request
- `DbUpdateException` → 500 Internal Server Error
- `InvalidOperationException` → 404 Not Found
- `ArgumentException` → 400 Bad Request
- `TimeoutException` → 408 Request Timeout
- `UnauthorizedAccessException` → 401 Unauthorized

## ⚡ Performance y Cache

### Cache Inteligente
- **Memory Cache**: Cache en memoria con expiración automática
- **Sliding Expiration**: Renovación automática de cache activo
- **Cache Keys**: Estrategia de nomenclatura consistente
- **Logging**: Tracking de hits/misses

### Paginación Optimizada
- **Consultas Paralelas**: Count y data en paralelo
- **Validación de Parámetros**: Límites automáticos
- **Cache de Resultados**: Para consultas frecuentes

## 🧪 Testing

### Ejecutar Tests
```bash
# Todos los tests
dotnet test

# Tests específicos
dotnet test --filter "TestCategory=Authentication"

# Con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

### Estructura de Tests
- **AuthenticationEndpointsTests** - Tests de autenticación
- **RealEstateEndpointsTests** - Tests de endpoints de propiedades
- **TestAuthHandler** - Handler de autenticación para tests

Ver [README de Tests](./RealEstate.Test/README.md) para más detalles.

## 📊 Modelos de Datos

### PropertyDTO
```csharp
{
  "idProperty": 1,
  "name": "string",
  "address": "string",
  "price": 0,
  "codeInternal": "string",
  "year": 2024,
  "idOwner": 1,
  "images": [
    {
      "file": "string",
      "enabled": true
    }
  ]
}
```

### PagedResult<T>
```csharp
{
  "items": [...],
  "total": 0,
  "page": 1,
  "pageSize": 20
}
```

## 🚨 Códigos de Error

| Código | Descripción | Error Code |
|--------|-------------|------------|
| 200 | OK - Operación exitosa | - |
| 400 | Bad Request - Datos inválidos | VALIDATION_ERROR, INVALID_ARGUMENT, INVALID_FORMAT |
| 401 | Unauthorized - Token inválido/faltante | UNAUTHORIZED |
| 404 | Not Found - Recurso no encontrado | RESOURCE_NOT_FOUND |
| 408 | Request Timeout - Tiempo agotado | TIMEOUT, OPERATION_CANCELLED |
| 500 | Internal Server Error - Error del servidor | DATABASE_ERROR, INTERNAL_SERVER_ERROR |
| 501 | Not Implemented - Funcionalidad no disponible | NOT_IMPLEMENTED |
| 503 | Service Unavailable - Health check fallido | - |

## 🔧 Desarrollo

### Estructura de Comandos/Queries
```
RealEstate.Application/
├── Property/
│   ├── CreateProperty/
│   │   ├── Commands/
│   │   ├── Validators/
│   │   └── Handlers/
│   └── ListPropertyWithFilters/
│       ├── Querys/
│       ├── Validators/
│       └── Handlers/
```

### Agregar Nuevo Endpoint
1. Crear Command/Query en Application
2. Implementar Handler
3. Agregar Validator
4. Registrar en DependencyInjection
5. Crear Endpoint en Api
6. Agregar Tests

### SecureCryptoHelper
El proyecto incluye un helper personalizado para encriptación segura:

```csharp
using RealEstate.Shared.Utils;

// Encriptar texto
string encrypted = SecureCryptoHelper.Encrypt("texto-plano", "clave-secreta");

// Desencriptar texto
string decrypted = SecureCryptoHelper.Decrypt(encrypted, "clave-secreta");
```

**Características**:
- Usa **AES-256** con modo CBC
- **PBKDF2** con 100,000 iteraciones para derivar claves
- **Salt fijo** para consistencia
- **SHA-256** para hash de passphrase


## 👥 Contribución
- **Desarrollador Principal** - Stefan Fayad
- **Arquitectura** - Clean Architecture + CQRS
- **Framework** - .NET 8 + ASP.NET Core

---

⭐ **¡Dale una estrella al proyecto si te gusta!** ⭐
