using MediatR;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.UpdateProperty.Commands
{
    public record UpdatePropertyCommand(UpdatePropertyRequest UpdatePropertyRequest) : IRequest<PropertyDTO>;
}
