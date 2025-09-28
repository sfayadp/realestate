using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace RealEstate.Application.DepedencyInjection
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRealEstateApplication(this IServiceCollection services)
        {
            services.AddMediatR(configuration =>
            {
                configuration.RegisterServicesFromAssembly(typeof(DependencyInjection).Assembly);
            });

            services.AddValidatorsFromAssembly(typeof(DependencyInjection).Assembly);

            return services;
        }
    }
}
