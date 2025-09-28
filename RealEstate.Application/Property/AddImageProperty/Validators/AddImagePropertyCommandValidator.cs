using FluentValidation;
using RealEstate.Application.Property.AddImageProperty.Commands;

namespace RealEstate.Application.Property.AddImageProperty.Validators
{
    public class AddImagePropertyCommandValidator : AbstractValidator<AddImagePropertyCommand>
    {
        public AddImagePropertyCommandValidator()
        {
            RuleFor(x => x.AddImageRequestDTO).NotNull().WithMessage("AddImageRequestDTO no puede ser nulo.");
            
            When(x => x.AddImageRequestDTO != null, () =>
            {
                RuleFor(x => x.AddImageRequestDTO.PropertyId)
                    .GreaterThan(0).WithMessage("El ID de la propiedad debe ser mayor a 0.");

                RuleFor(x => x.AddImageRequestDTO.ImageFile)
                    .NotNull().WithMessage("El archivo de imagen es requerido.")
                    .NotEmpty().WithMessage("El archivo de imagen no puede estar vacío.");
            });
        }
    }
}
