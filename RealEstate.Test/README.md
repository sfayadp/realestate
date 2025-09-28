# RealEstate API Tests

Este proyecto contiene tests unitarios para los endpoints de la API de RealEstate usando NUnit.

## Estructura de Tests

### AuthenticationEndpointsTests
- `Authenticate_WithValidRequest_ReturnsOk` - Test para autenticación exitosa (200 OK)
- `Authenticate_WithInvalidRequest_ReturnsBadRequest` - Test para request inválido (500 InternalServerError)
- `Authenticate_WithNullRequest_ReturnsBadRequest` - Test para request nulo (400 BadRequest)
- `Authenticate_WhenOwnerNotFound_ReturnsBadRequest` - Test para owner no encontrado (500 InternalServerError)

### RealEstateEndpointsTests
- **CreateProperty Tests**
  - `CreateProperty_WithValidRequest_ReturnsOk` - Test para creación exitosa (200 OK)
  - `CreateProperty_WithInvalidRequest_ReturnsBadRequest` - Test para datos inválidos (500 InternalServerError)

## Tecnologías Utilizadas

- **NUnit** - Framework de testing
- **Moq** - Para mocking de dependencias (ISender/MediatR)
- **FluentAssertions** - Para assertions más legibles
- **Microsoft.AspNetCore.Mvc.Testing** - Para testing de endpoints HTTP
- **TestAuthHandler** - Handler de autenticación personalizado para tests

## Cómo Ejecutar los Tests

### Desde Visual Studio
1. Abrir el proyecto en Visual Studio
2. Ir a Test Explorer
3. Ejecutar todos los tests o tests individuales

### Desde línea de comandos
```bash
# Ejecutar todos los tests
dotnet test

# Ejecutar tests con detalle
dotnet test --verbosity normal

# Ejecutar tests específicos
dotnet test --filter "TestCategory=Authentication"

# Ejecutar tests con cobertura
dotnet test --collect:"XPlat Code Coverage"
```

## Configuración de Tests

Los tests utilizan:
- **WebApplicationFactory** para crear una instancia de la API en memoria
- **Moq** para mockear el `ISender` (MediatR) y simular respuestas
- **TestAuthHandler** para manejar la autenticación en tests (siempre exitosa)
- **FluentAssertions** para assertions más expresivas
- **JsonSerializer** para deserializar respuestas HTTP

## Manejo de Autenticación en Tests

Los endpoints de RealEstate requieren autenticación (`.RequireAuthorization()`). Para los tests:

1. **Se remueven** todos los servicios de autenticación existentes
2. **Se agrega** un `TestAuthHandler` que siempre autentica exitosamente
3. **Se proporcionan** claims de prueba para simular un usuario autenticado

```csharp
public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.NameIdentifier, "1"),
            new Claim(ClaimTypes.Name, "Test User")
        };
        
        var identity = new ClaimsIdentity(claims, "Test");
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, "Test");
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}
```

## Códigos de Estado Esperados

### AuthenticationEndpoints
- **200 OK**: Request válido con autenticación exitosa
- **400 BadRequest**: Request nulo (model binding falla)
- **500 InternalServerError**: ValidationException (sin global exception handler)

### RealEstateEndpoints
- **200 OK**: Request válido con autenticación y datos correctos
- **500 InternalServerError**: ValidationException (sin global exception handler)

## Notas Importantes

- Los tests **no requieren** base de datos real
- Se utilizan **mocks** para todas las dependencias externas (MediatR)
- Cada test es **independiente** y puede ejecutarse en paralelo
- Los tests cubren **casos exitosos y de error** para cada endpoint
- La autenticación se **simula** con TestAuthHandler
- Los **ValidationExceptions** resultan en 500 porque no hay global exception handler activo

## Próximos Pasos

Para expandir la cobertura de tests, se pueden agregar:
- Tests para `AddImageProperty`, `ChangePrice`, `UpdateProperty`, `ListPropertyWithFilters`
- Tests de integración con base de datos real
- Tests de performance
- Tests de seguridad adicionales
