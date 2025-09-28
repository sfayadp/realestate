using FluentValidation;
using MediatR;
using RealEstate.Domain.Contract;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.AddImageProperty.Commands
{
    public class AddImagePropertyCommandHandler : IRequestHandler<AddImagePropertyCommand, PropertyDTO>
    {
        private readonly IValidator<AddImagePropertyCommand> _validator;
        private readonly IRealEstateRepository _realEstateRepository;

        public AddImagePropertyCommandHandler(IValidator<AddImagePropertyCommand> validator, IRealEstateRepository realEstateRepository)
        {
            _validator = validator;
            _realEstateRepository = realEstateRepository;
        }

        public async Task<PropertyDTO> Handle(AddImagePropertyCommand request, CancellationToken cancellationToken)
        {
            await ValidateCommand(request, cancellationToken);

            return await _realEstateRepository.AddImageFromPropertyAsync(
                request.AddImageRequestDTO.PropertyId, 
                request.AddImageRequestDTO.ImageFile, 
                request.AddImageRequestDTO.Enabled, 
                cancellationToken);
        }

        private async Task ValidateCommand(AddImagePropertyCommand request, CancellationToken cancellationToken)
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
