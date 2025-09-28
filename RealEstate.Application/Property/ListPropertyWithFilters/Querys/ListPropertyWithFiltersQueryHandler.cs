using FluentValidation;
using MediatR;
using RealEstate.Domain.Contract;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.ListPropertyWithFilters.Querys
{
    public class ListPropertyWithFiltersQueryHandler : IRequestHandler<ListPropertyWithFiltersQuery, PagedResult<PropertyDTO>>
    {
        private readonly IValidator<ListPropertyWithFiltersQuery> _validator;
        private readonly IRealEstateRepository _realEstateRepository;

        public ListPropertyWithFiltersQueryHandler(IValidator<ListPropertyWithFiltersQuery> validator, IRealEstateRepository realEstateRepository)
        {
            _validator = validator;
            _realEstateRepository = realEstateRepository;
        }

        public async Task<PagedResult<PropertyDTO>> Handle(ListPropertyWithFiltersQuery request, CancellationToken cancellationToken)
        {
            await ValidateQuery(request, cancellationToken);

            return await _realEstateRepository.ListPropertyWithFiltersAsync(request.PropertyFilterDTO, cancellationToken);
        }

        private async Task ValidateQuery(ListPropertyWithFiltersQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                var errors = string.Join("; ", validationResult.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Errores de validación: {errors}");
            }
        }
    }
}
