using FluentValidation;
using MediatR;
using RealEstate.Domain.Contract;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.CreateProperty.Commands
{
    public class CreatePropertyCommandHandler : IRequestHandler<CreatePropertyCommand, PropertyDTO>
    {
        private readonly IValidator<CreatePropertyCommand> _validator;
        private readonly IRealEstateRepository _realEstateRepository;

        public CreatePropertyCommandHandler(IValidator<CreatePropertyCommand> validator, IRealEstateRepository realEstateRepository)
        {
            _validator = validator;
            _realEstateRepository = realEstateRepository;
        }

        public async Task<PropertyDTO> Handle(CreatePropertyCommand request, CancellationToken cancellationToken)
        {
            await ValidateCommand(request, cancellationToken);

            return await _realEstateRepository.CreatePropertyBuildingAsync(request.PropertyDTO, cancellationToken);
        }

        private async Task ValidateCommand(CreatePropertyCommand request, CancellationToken cancellationToken)
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
