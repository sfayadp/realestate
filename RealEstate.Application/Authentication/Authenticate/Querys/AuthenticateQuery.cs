using MediatR;
using RealEstate.Shared.DTO.Authentication;

namespace RealEstate.Application.Authentication.Authenticate.Querys
{
    public record AuthenticateQuery(AuthenticationRequestDTO authenticationRequestDTO) : IRequest<AuthenticationResponseDTO>;
}
