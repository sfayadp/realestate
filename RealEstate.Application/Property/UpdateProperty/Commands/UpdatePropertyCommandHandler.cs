using FluentValidation;
using MediatR;
using RealEstate.Domain.Contract;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.UpdateProperty.Commands
{
    public class UpdatePropertyCommandHandler : IRequestHandler<UpdatePropertyCommand, PropertyDTO>
    {
        private readonly IValidator<UpdatePropertyCommand> _validator;
        private readonly IRealEstateRepository _realEstateRepository;

        public UpdatePropertyCommandHandler(IValidator<UpdatePropertyCommand> validator, IRealEstateRepository realEstateRepository)
        {
            _validator = validator;
            _realEstateRepository = realEstateRepository;
        }

        public async Task<PropertyDTO> Handle(UpdatePropertyCommand request, CancellationToken cancellationToken)
        {
            await ValidateCommand(request, cancellationToken);

            var propertyDTO = new PropertyDTO
            {
                Name = request.UpdatePropertyRequest.Name ?? string.Empty,
                Address = request.UpdatePropertyRequest.Address,
                Price = request.UpdatePropertyRequest.Price ?? 0,
                CodeInternal = request.UpdatePropertyRequest.CodeInternal ?? string.Empty,
                Year = request.UpdatePropertyRequest.Year,
                IdOwner = request.UpdatePropertyRequest.IdOwner ?? 0
            };

            return await _realEstateRepository.UpdatePropertyAsync(request.UpdatePropertyRequest.IdProperty, propertyDTO, cancellationToken);
        }

        private async Task ValidateCommand(UpdatePropertyCommand request, CancellationToken cancellationToken)
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
