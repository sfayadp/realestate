using FluentValidation;
using RealEstate.Application.Property.ListPropertyWithFilters.Querys;

namespace RealEstate.Application.Property.ListPropertyWithFilters.Validators
{
    public class ListPropertyWithFiltersValidator : AbstractValidator<ListPropertyWithFiltersQuery>
    {
        public ListPropertyWithFiltersValidator()
        {
            RuleFor(x => x.PropertyFilterDTO).NotNull().WithMessage("PropertyFilterDTO no puede ser nulo.");
            
            When(x => x.PropertyFilterDTO != null, () =>
            {
                RuleFor(x => x.PropertyFilterDTO.IdOwner)
                    .GreaterThan(0).WithMessage("El ID del propietario debe ser mayor a 0.")
                    .When(x => x.PropertyFilterDTO.IdOwner.HasValue);

                RuleFor(x => x.PropertyFilterDTO.CodeInternal)
                    .MaximumLength(50).WithMessage("El código interno no puede exceder 50 caracteres.")
                    .When(x => !string.IsNullOrEmpty(x.PropertyFilterDTO.CodeInternal));

                RuleFor(x => x.PropertyFilterDTO.NameContains)
                    .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.")
                    .When(x => !string.IsNullOrEmpty(x.PropertyFilterDTO.NameContains));

                RuleFor(x => x.PropertyFilterDTO.MinPrice)
                    .GreaterThan(0).WithMessage("El precio mínimo debe ser mayor a 0.")
                    .When(x => x.PropertyFilterDTO.MinPrice.HasValue);

                RuleFor(x => x.PropertyFilterDTO.MaxPrice)
                    .GreaterThan(0).WithMessage("El precio máximo debe ser mayor a 0.")
                    .When(x => x.PropertyFilterDTO.MaxPrice.HasValue);

                RuleFor(x => x.PropertyFilterDTO.MinYear)
                    .GreaterThan((short)1800).WithMessage("El año mínimo debe ser mayor a 1800.")
                    .When(x => x.PropertyFilterDTO.MinYear.HasValue);

                RuleFor(x => x.PropertyFilterDTO.MaxYear)
                    .LessThanOrEqualTo((short)DateTime.Now.Year).WithMessage("El año máximo no puede ser mayor al año actual.")
                    .When(x => x.PropertyFilterDTO.MaxYear.HasValue);

                RuleFor(x => x.PropertyFilterDTO.Page)
                    .GreaterThan(0).WithMessage("La página debe ser mayor a 0.");

                RuleFor(x => x.PropertyFilterDTO.PageSize)
                    .GreaterThan(0).WithMessage("El tamaño de página debe ser mayor a 0.")
                    .LessThanOrEqualTo(100).WithMessage("El tamaño de página no puede exceder 100.");
            });
        }
    }
}
