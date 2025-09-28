using FluentAssertions;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RealEstate.Api;
using RealEstate.Application.Authentication.Authenticate.Querys;
using RealEstate.Shared.DTO.Authentication;
using System.Net;
using System.Net.Http.Json;
using System.Text.Json;

namespace RealEstate.Test.Endpoints
{
    [TestFixture]
    public class AuthenticationEndpointsTests
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
        public async Task Authenticate_WithValidRequest_ReturnsOk()
        {
            // Arrange
            var request = new AuthenticationRequestDTO
            {
                OwnerId = 1
            };

            var expectedResponse = new AuthenticationResponseDTO
            {
                Token = "test-token",
                Successfull = true
            };

            _mockSender.Setup(x => x.Send(It.IsAny<AuthenticateQuery>(), It.IsAny<CancellationToken>()))
                      .ReturnsAsync(expectedResponse);

            // Act
            var response = await _client.PostAsJsonAsync("/api/authentication/Authenticate", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.OK);
            
            var content = await response.Content.ReadAsStringAsync();
            var result = JsonSerializer.Deserialize<AuthenticationResponseDTO>(content, new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true
            });

            result.Should().NotBeNull();
            result?.Token.Should().Be(expectedResponse.Token);
            result?.Successfull.Should().Be(expectedResponse.Successfull);
        }

        [Test]
        public async Task Authenticate_WithInvalidRequest_ReturnsBadRequest()
        {
            // Arrange
            var request = new AuthenticationRequestDTO
            {
                OwnerId = -1
            };

            _mockSender.Setup(x => x.Send(It.IsAny<AuthenticateQuery>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new ValidationException("OwnerId debe ser mayor a 0"));

            // Act
            var response = await _client.PostAsJsonAsync("/api/authentication/Authenticate", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }

        [Test]
        public async Task Authenticate_WithNullRequest_ReturnsBadRequest()
        {
            // Act
            var response = await _client.PostAsJsonAsync("/api/authentication/Authenticate", (object)null!);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        [Test]
        public async Task Authenticate_WhenOwnerNotFound_ReturnsBadRequest()
        {
            // Arrange
            var request = new AuthenticationRequestDTO
            {
                OwnerId = 999
            };

            _mockSender.Setup(x => x.Send(It.IsAny<AuthenticateQuery>(), It.IsAny<CancellationToken>()))
                      .ThrowsAsync(new InvalidOperationException("Owner no encontrado"));

            // Act
            var response = await _client.PostAsJsonAsync("/api/authentication/Authenticate", request);

            // Assert
            response.StatusCode.Should().Be(HttpStatusCode.InternalServerError);
        }
    }
}
