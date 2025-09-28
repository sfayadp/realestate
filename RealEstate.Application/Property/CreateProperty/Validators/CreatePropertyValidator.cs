using FluentValidation;
using RealEstate.Application.Property.CreateProperty.Commands;

namespace RealEstate.Application.Property.CreateProperty.Validators
{
    public class CreatePropertyValidator : AbstractValidator<CreatePropertyCommand>
    {
        public CreatePropertyValidator()
        {
            RuleFor(x => x.PropertyDTO).NotNull().WithMessage("PropertyDTO no puede ser nulo.");
            
            When(x => x.PropertyDTO != null, () =>
            {
                RuleFor(x => x.PropertyDTO.Name)
                    .NotEmpty().WithMessage("El nombre de la propiedad es requerido.")
                    .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.");

                RuleFor(x => x.PropertyDTO.Address)
                    .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres.");

                RuleFor(x => x.PropertyDTO.Price)
                    .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.");

                RuleFor(x => x.PropertyDTO.CodeInternal)
                    .NotEmpty().WithMessage("El código interno es requerido.")
                    .MaximumLength(50).WithMessage("El código interno no puede exceder 50 caracteres.");

                RuleFor(x => x.PropertyDTO.Year)
                    .GreaterThan((short)1800).WithMessage("El año debe ser mayor a 1800.")
                    .LessThanOrEqualTo((short)DateTime.Now.Year).WithMessage("El año no puede ser mayor al año actual.")
                    .When(x => x.PropertyDTO.Year.HasValue);

                RuleFor(x => x.PropertyDTO.IdOwner)
                    .GreaterThan(0).WithMessage("El ID del propietario debe ser mayor a 0.");
            });
        }
    }
}
