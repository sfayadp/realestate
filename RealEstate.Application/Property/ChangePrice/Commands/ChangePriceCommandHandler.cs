using FluentValidation;
using MediatR;
using RealEstate.Domain.Contract;
using RealEstate.Shared.DTO;

namespace RealEstate.Application.Property.ChangePrice.Commands
{
    public class ChangePriceCommandHandler : IRequestHandler<ChangePriceCommand, PropertyDTO>
    {
        private readonly IValidator<ChangePriceCommand> _validator;
        private readonly IRealEstateRepository _realEstateRepository;

        public ChangePriceCommandHandler(IValidator<ChangePriceCommand> validator, IRealEstateRepository realEstateRepository)
        {
            _validator = validator;
            _realEstateRepository = realEstateRepository;
        }

        public async Task<PropertyDTO> Handle(ChangePriceCommand request, CancellationToken cancellationToken)
        {
            await ValidateCommand(request, cancellationToken);

            return await _realEstateRepository.ChangePriceAsync(
                request.ChangePriceRequestDTO.PropertyId,
                request.ChangePriceRequestDTO.NewPrice,
                request.ChangePriceRequestDTO.Tax,
                request.ChangePriceRequestDTO.Note ?? "",
                request.ChangePriceRequestDTO.DateSale,
                cancellationToken);
        }

        private async Task ValidateCommand(ChangePriceCommand request, CancellationToken cancellationToken)
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
