using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RealEstate.Domain.Contract;
using RealEstate.Infrastructure.Authentication;
using RealEstate.Infrastructure.Contexts;
using RealEstate.Infrastructure.Repository;
using RealEstate.Shared.Utils;

namespace RealEstate.Infrastructure.DependencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRealEstateInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddTransient<ITokenGenerator, TokenGenerator>();
            services.AddScoped<IRealEstateRepository, RealEstateRepository>();

            services.AddDbContext<RealEstateDbContext>(options =>
            {
                string encryptedConn = configuration["DbSetting:ConnectionString"] ?? "";
                string key = configuration["DbSetting:KeyEncrypt"] ?? "";

                string connectionString = SecureCryptoHelper.Decrypt(encryptedConn, key);
                options.UseSqlServer(connectionString);
            });

            return services;
        }
    }
}
