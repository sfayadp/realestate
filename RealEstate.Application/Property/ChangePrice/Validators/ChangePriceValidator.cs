using FluentValidation;
using RealEstate.Application.Property.ChangePrice.Commands;

namespace RealEstate.Application.Property.ChangePrice.Validators
{
    public class ChangePriceValidator : AbstractValidator<ChangePriceCommand>
    {
        public ChangePriceValidator()
        {
            RuleFor(x => x.ChangePriceRequestDTO).NotNull().WithMessage("ChangePriceRequestDTO no puede ser nulo.");
            
            When(x => x.ChangePriceRequestDTO != null, () =>
            {
                RuleFor(x => x.ChangePriceRequestDTO.PropertyId)
                    .GreaterThan(0).WithMessage("El ID de la propiedad debe ser mayor a 0.");

                RuleFor(x => x.ChangePriceRequestDTO.NewPrice)
                    .GreaterThan(0).WithMessage("El nuevo precio debe ser mayor a 0.");

                RuleFor(x => x.ChangePriceRequestDTO.Tax)
                    .GreaterThanOrEqualTo(0).WithMessage("El impuesto no puede ser negativo.");

                RuleFor(x => x.ChangePriceRequestDTO.Note)
                    .MaximumLength(500).WithMessage("La nota no puede exceder 500 caracteres.");

                RuleFor(x => x.ChangePriceRequestDTO.DateSale)
                    .LessThanOrEqualTo(DateTime.Now).WithMessage("La fecha de venta no puede ser futura.");
            });
        }
    }
}
