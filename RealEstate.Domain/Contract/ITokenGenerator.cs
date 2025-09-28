using RealEstate.Shared.DTO;
using RealEstate.Shared.DTO.Authentication;

namespace RealEstate.Domain.Contract
{
    public interface ITokenGenerator
    {
        Task<AuthenticationResponseDTO> GenerateTokenJwtAsync(OwnerDTO owner);
    }
}
