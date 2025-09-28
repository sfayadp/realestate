using MediatR;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.CreateProperty.Commands
{
    public record CreatePropertyCommand(PropertyDTO PropertyDTO) : IRequest<PropertyDTO>;
}
