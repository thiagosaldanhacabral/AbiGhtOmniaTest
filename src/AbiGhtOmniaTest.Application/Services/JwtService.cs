using AbiGhtOmniaTest.Application.Interfaces;
using AbiGhtOmniaTest.Domain.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Serilog;

namespace AbiGhtOmniaTest.Application.Services;

public class JwtService(IConfiguration configuration, ILogger logger) : IJwtService
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger _logger = logger;

    public string GenerateToken(User user)
    {
        try
        {
            _logger.Information("Generating token for user ID: {UserId}", user.Id);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Role, user.Role.ToString())
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                _configuration["Jwt:Issuer"],
                _configuration["Jwt:Audience"],
                claims,
                expires: DateTime.Now.AddHours(1),
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);
            _logger.Information("Token generated for user ID: {UserId}", user.Id);

            return tokenString;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error generating token for user ID: {UserId}", user.Id);
            throw;
        }
    }
}
