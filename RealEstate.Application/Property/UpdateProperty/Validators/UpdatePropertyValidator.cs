using FluentValidation;
using RealEstate.Application.Property.UpdateProperty.Commands;

namespace RealEstate.Application.Property.UpdateProperty.Validators
{
    public class UpdatePropertyValidator : AbstractValidator<UpdatePropertyCommand>
    {
        public UpdatePropertyValidator()
        {
            RuleFor(x => x.UpdatePropertyRequest.IdProperty)
                .GreaterThan(0).WithMessage("El ID de la propiedad debe ser mayor a 0.");

            RuleFor(x => x.UpdatePropertyRequest).NotNull().WithMessage("UpdatePropertyRequest no puede ser nulo.");
            
            When(x => x.UpdatePropertyRequest != null, () =>
            {
                RuleFor(x => x.UpdatePropertyRequest.Name)
                    .MaximumLength(100).WithMessage("El nombre no puede exceder 100 caracteres.")
                    .When(x => !string.IsNullOrEmpty(x.UpdatePropertyRequest.Name));

                RuleFor(x => x.UpdatePropertyRequest.Address)
                    .MaximumLength(200).WithMessage("La dirección no puede exceder 200 caracteres.")
                    .When(x => !string.IsNullOrEmpty(x.UpdatePropertyRequest.Address));

                RuleFor(x => x.UpdatePropertyRequest.Price)
                    .GreaterThan(0).WithMessage("El precio debe ser mayor a 0.")
                    .When(x => x.UpdatePropertyRequest.Price.HasValue);

                RuleFor(x => x.UpdatePropertyRequest.CodeInternal)
                    .MaximumLength(50).WithMessage("El código interno no puede exceder 50 caracteres.")
                    .When(x => !string.IsNullOrEmpty(x.UpdatePropertyRequest.CodeInternal));

                RuleFor(x => x.UpdatePropertyRequest.Year)
                    .GreaterThan((short)1800).WithMessage("El año debe ser mayor a 1800.")
                    .LessThanOrEqualTo((short)DateTime.Now.Year).WithMessage("El año no puede ser mayor al año actual.")
                    .When(x => x.UpdatePropertyRequest.Year.HasValue);

                RuleFor(x => x.UpdatePropertyRequest.IdOwner)
                    .GreaterThan(0).WithMessage("El ID del propietario debe ser mayor a 0.")
                    .When(x => x.UpdatePropertyRequest.IdOwner.HasValue);
            });
        }
    }
}
