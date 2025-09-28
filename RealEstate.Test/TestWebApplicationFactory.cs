using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace RealEstate.Test
{
    public class TestWebApplicationFactory<TProgram> : WebApplicationFactory<TProgram> where TProgram : class
    {
        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.ConfigureServices(services =>
            {
                var serviceDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(ILogger<>));
                if (serviceDescriptor != null)
                {
                    services.Remove(serviceDescriptor);
                }

                services.AddLogging(builder => builder.AddConsole().SetMinimumLevel(LogLevel.Warning));
            });

            builder.UseEnvironment("Testing");
        }
    }
}
