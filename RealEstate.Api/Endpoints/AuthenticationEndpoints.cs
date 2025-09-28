using MediatR;
using RealEstate.Application.Authentication.Authenticate.Querys;
using RealEstate.Shared.DTO;
using RealEstate.Shared.DTO.Authentication;

public static class AuthenticationEndpoints
{
    public static void MapAuthenticationEndpoints(this IEndpointRouteBuilder endpoints)
    {
        RouteGroupBuilder group = endpoints.MapGroup("/api/authentication")
                             .WithTags("Authentication");

        /// <summary>
        /// Autentica un usuario y genera un token JWT para acceder a los endpoints protegidos
        /// </summary>
        /// <param name="request">Datos de autenticación del usuario</param>
        /// <returns>Token JWT y estado de autenticación</returns>
        /// <response code="200">Autenticación exitosa</response>
        /// <response code="400">Request inválido o owner no encontrado</response>
        group.MapPost("/Authenticate", async (ISender sender, AuthenticationRequestDTO request) =>
        {
            var response = await sender.Send(new AuthenticateQuery(request));
            return Results.Ok(response);
        })
        .Accepts<AuthenticationResponseDTO>("application/json")
        .Produces<object>(200, "application/json")
        .Produces(400);
    }
}
