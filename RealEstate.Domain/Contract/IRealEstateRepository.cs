using RealEstate.Shared.DTO;

namespace RealEstate.Domain.Contract
{
    public interface IRealEstateRepository
    {
        Task<OwnerDTO> GetOwnerAsync(int ownerId);
        Task<PropertyDTO> CreatePropertyBuildingAsync(PropertyDTO property, CancellationToken cancellationToken = default);
        Task<PropertyDTO> AddImageFromPropertyAsync(int propertyId, byte[] imageFile, bool enabled = true, CancellationToken cancellationToken = default);
        Task<PropertyDTO> ChangePriceAsync(int propertyId, decimal newPrice, decimal tax, string note, DateTime? dateSale = null, CancellationToken cancellationToken = default);
        Task<PropertyDTO> UpdatePropertyAsync(int propertyId, PropertyDTO dto, CancellationToken cancellationToken = default);
        Task<PagedResult<PropertyDTO>> ListPropertyWithFiltersAsync(PropertyFilterDTO filter, CancellationToken cancellationToken = default);
    }
}
