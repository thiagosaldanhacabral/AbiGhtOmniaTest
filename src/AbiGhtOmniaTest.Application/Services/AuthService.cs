using AbiGhtOmniaTest.Application.Interfaces;
using AbiGhtOmniaTest.Domain.Authentication;
using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AbiGhtOmniaTest.Application.Services;

public class AuthService(IUserRepository userRepository, IJwtService jwtService, IPasswordHasher<User> passwordHasher, ILogger logger) : IAuthService
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IJwtService _jwtService = jwtService;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly ILogger _logger = logger;

    public async Task<LoginResponse> LoginAsync(LoginRequest request)
    {
        try
        {
            _logger.Information("Login attempt for email: {Email}", request.Email);

            var user = await _userRepository.GetUserByEmailAsync(request.Email);
            if (user == null || _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, request.Password) == PasswordVerificationResult.Failed)
            {
                _logger.Warning("Invalid login attempt for email: {Email}", request.Email);
                throw new Exception("Invalid email or password.");
            }

            var token = _jwtService.GenerateToken(user);
            _logger.Information("Token generated for user ID: {UserId}", user.Id);

            return new LoginResponse { Token = token };
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "An error occurred during login attempt for email: {Email}", request.Email);
            throw;
        }
    }
}
