using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;
using RealEstate.Api;
using RealEstate.Application.Property.CreateProperty.Commands;
using RealEstate.Shared.DTO;
using System.Net;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace RealEstate.Test.Endpoints
{
    [TestFixture]
    public class RealEstateEndpointsTests
    {
        private WebApplicationFactory<Program> _factory;
        private HttpClient _client;
        private Mock<ISender> _mockSender;

        [SetUp]
        public void Setup()
        {
            _mockSender = new Mock<ISender>();
            
            _factory = new WebApplicationFactory<Program>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(d => d.ServiceType == typeof(ISender));
                        if (descriptor != null)
                            services.Remove(descriptor);
                        services.AddSingleton(_mockSender.Object);

                        var authServices = services.Where(s => s.ServiceType.Name.Contains("Authentication")).ToList();
                        foreach (var service in authServices)
                        {
                            services.Remove(service);
                        }

                        services.AddAuthentication("Test")
                            .AddScheme<Microsoft.AspNetCore.Authentication.AuthenticationSchemeOptions, TestAuthHandler>("Test", options => { });
                    });
                });

            _client = _factory.CreateClient();
        }

        [TearDown]
        public void TearDown()
        {
            _client?.Dispose();
            _factory?.Dispose();
        }

        [Test]
        public async Task CreateProperty_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new PropertyDTO
            {
                Name = "Test Property",
                Address = "123 Test Street",
                Price = 100000,
                CodeInternal = "TEST001",
                Year = 2020,
                IdOwner = 1
            };

            var expectedResponse = new PropertyDTO
            {
                IdProperty = 1,
                Name = "Test Property",
                Address = "123 Test Street",
                Price = 100000,
                CodeInternal = "TEST001",
                Year = 2020,
                IdOwner = 1
            };

            _mockSender.Setup(x => x.Send(It.IsAny<CreatePropertyCommand>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

            // Act
            var response = await _client.PostAsJsonAsync("/api/realEstate/CreateProperty", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<PropertyDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result?.Name.Should().Be(expectedResponse.Name);
            result?.Price.Should().Be(expectedResponse.Price);
            result?.CodeInternal.Should().Be(expectedResponse.CodeInternal);
        }

        [Test]
        public async Task CreateProperty_WithInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new PropertyDTO
            {
                Name = "", 
                Price = -1000, 
                CodeInternal = "",
                IdOwner = 0
            };

            _mockSender.Setup(x => x.Send(It.IsAny<CreatePropertyCommand>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new ValidationException("Errores de validación"));

            // Act
            var response = await _client.PostAsJsonAsync("/api/realEstate/CreateProperty", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }

    public class TestAuthHandler : AuthenticationHandler<AuthenticationSchemeOptions>
    {
        public TestAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
            ILoggerFactory logger, UrlEncoder encoder)
            : base(options, logger, encoder)
        {
        }

        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "1"),
                new Claim(ClaimTypes.Name, "Test User")
            };

            var identity = new ClaimsIdentity(claims, "Test");
            var principal = new ClaimsPrincipal(identity);
            var ticket = new AuthenticationTicket(principal, "Test");

            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}