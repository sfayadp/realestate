using FluentValidation;
using RealEstate.Application.Authentication.Authenticate.Querys;

namespace RealEstate.Application.Authentication.Authenticate.Validators
{
    public class AuthenticateQueryValidator : AbstractValidator<AuthenticateQuery>
    {
        public AuthenticateQueryValidator()
        {
            RuleFor(x => x.authenticationRequestDTO.OwnerId).NotNull().WithMessage("OwnerId no puede ser nulo.");
            RuleFor(x => x.authenticationRequestDTO.OwnerId).NotEmpty().WithMessage("OwnerId no puede ser vacio.");
        }
    }
}
