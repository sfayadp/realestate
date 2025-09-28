using FluentValidation;
using MediatR;
using RealEstate.Domain.Contract;
using RealEstate.Shared.DTO;
using RealEstate.Shared.DTO.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RealEstate.Application.Authentication.Authenticate.Querys
{
    public class AuthenticateQueryHandler : IRequestHandler<AuthenticateQuery, AuthenticationResponseDTO?>
    {
        private readonly IValidator<AuthenticateQuery> _validator;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IRealEstateRepository _realEstateRepository;

        public AuthenticateQueryHandler(IValidator<AuthenticateQuery> validator, ITokenGenerator tokenGenerator, IRealEstateRepository realEstateRepository)
        {
            _validator = validator;
            _tokenGenerator = tokenGenerator;
            _realEstateRepository = realEstateRepository;
        }

        public async Task<AuthenticationResponseDTO?> Handle(AuthenticateQuery request, CancellationToken cancellationToken)
        {
            AuthenticationResponseDTO responseDTO = new AuthenticationResponseDTO();

            bool validationRequest = await ValidateQuery(request, cancellationToken);

            if (validationRequest)
            {
                OwnerDTO ownerDTO = await _realEstateRepository.GetOwnerAsync(request.authenticationRequestDTO.OwnerId);
                responseDTO = await _tokenGenerator.GenerateTokenJwtAsync(ownerDTO);
            }

            return responseDTO;
        }

        private async Task<bool> ValidateQuery(AuthenticateQuery request, CancellationToken cancellationToken)
        {
            var validationRequest = await _validator.ValidateAsync(request, cancellationToken);

            if (!validationRequest.IsValid)
            {
                var errors = string.Join("; ", validationRequest.Errors.Select(e => e.ErrorMessage));
                throw new ValidationException($"Errores de validación: {errors}");
            }

            return true;
        }
    }
}
