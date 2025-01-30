using AbiGhtOmniaTest.Application.Interfaces;
using AbiGhtOmniaTest.Domain.Authentication;
using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using NSubstitute;
using System.Net;
using System.Net.Http.Json;

namespace AbiGhtOmniaTest.Test
{
    public class ApiTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ApiTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    var authServiceMock = Substitute.For<IAuthService>();
                    authServiceMock.LoginAsync(Arg.Any<LoginRequest>()).Returns(new LoginResponse { Token = "fake-jwt-token" });

                    var userRepositoryMock = Substitute.For<IUserRepository>();
                    userRepositoryMock.AddUserAsync(Arg.Any<User>()).Returns(Task.CompletedTask);

                    services.AddSingleton(authServiceMock);
                    services.AddSingleton(userRepositoryMock);
                });
            });
        }

        [Fact]
        public async Task Login_ShouldReturnOk_WhenCredentialsAreValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var loginRequest = new LoginRequest
            {
                Email = "test@example.com",
                Password = "password"
            };

            // Act
            var response = await client.PostAsJsonAsync("/auth/login", loginRequest);

            // Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        }

        [Fact]
        public async Task CreateUser_ShouldReturnCreated_WhenUserIsValid()
        {
            // Arrange
            var client = _factory.CreateClient();
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = "newuser@example.com",
                Username = "newuser",
                PasswordHash = "hashedpassword",
                Name = new Name { FirstName = "New", LastName = "User" },
                Address = new Address { Street = "123 Main St", City = "Anytown", Zipcode = "12345" },
                Phone = "123-456-7890",
                Status = UserStatus.Active,
                Role = UserRole.Customer
            };

            // Act
            var response = await client.PostAsJsonAsync("/users", user);

            // Assert
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        }
    }
}