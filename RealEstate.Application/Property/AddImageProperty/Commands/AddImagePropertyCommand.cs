using MediatR;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.AddImageProperty.Commands
{
    public record AddImagePropertyCommand(AddImageRequestDTO AddImageRequestDTO) : IRequest<PropertyDTO>;
}
