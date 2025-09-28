using MediatR;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.ChangePrice.Commands
{
    public record ChangePriceCommand(ChangePriceRequestDTO ChangePriceRequestDTO) : IRequest<PropertyDTO>;
}
