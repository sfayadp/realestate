using MediatR;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.ListPropertyWithFilters.Querys
{
    public record ListPropertyWithFiltersQuery(PropertyFilterDTO PropertyFilterDTO) : IRequest<PagedResult<PropertyDTO>>;
}
