using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using RealEstate.Domain.Contract;
using RealEstate.Shared.DTO;
using RealEstate.Shared.DTO.Authentication;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace RealEstate.Infrastructure.Authentication
{
    public class TokenGenerator : ITokenGenerator
    {
        private readonly string _secret;
        private readonly string _expire;
        private readonly string _audience;
        private readonly string _issuer;

        private readonly ILogger<TokenGenerator> _logger;

        public TokenGenerator(IConfiguration configuration, ILogger<TokenGenerator> logger)
        {
            _secret = configuration["JWT:secret"] ?? throw new ArgumentNullException(nameof(configuration), "JWT:secret is missing in configuration.");
            _expire = configuration["JWT:expire"] ?? throw new ArgumentNullException(nameof(configuration), "JWT:expire is missing in configuration.");
            _audience = configuration["JWT:audience"] ?? throw new ArgumentNullException(nameof(configuration), "JWT:audience is missing in configuration.");
            _issuer = configuration["JWT:issuer"] ?? throw new ArgumentNullException(nameof(configuration), "JWT:issuer is missing in configuration.");
            _logger = logger;
        }

        public async Task<AuthenticationResponseDTO> GenerateTokenJwtAsync(OwnerDTO owner)
        {
            return await Task.Run(() =>
            {
                var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_secret));
                var signingCredentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

                var claims = new List<Claim>
                {
                    new Claim("IdOwner", owner.IdOwner.ToString()),
                    new Claim("Name", owner.Name.ToString()),
                    new Claim("IdOwner", owner.Address ?? ""),
                    new Claim("IdOwner", owner.Birthday.ToString() ?? ""),
                    new Claim("auth_time", DateTime.UtcNow.ToString("o"))
                };

                var tokenHandler = new JwtSecurityTokenHandler();
                var expireTime = DateTime.UtcNow.AddMinutes(Convert.ToInt32(_expire));

                var jwtSecurityToken = tokenHandler.CreateJwtSecurityToken(
                    audience: _audience,
                    issuer: _issuer,
                    subject: new ClaimsIdentity(claims),
                    notBefore: DateTime.UtcNow,
                    expires: expireTime,
                    signingCredentials: signingCredentials
                );

                AuthenticationResponseDTO authenticationResponseDTO = new AuthenticationResponseDTO
                {
                    Token = tokenHandler.WriteToken(jwtSecurityToken),
                    Successfull = true
                };

                return authenticationResponseDTO;
            });
        }
    }
}
