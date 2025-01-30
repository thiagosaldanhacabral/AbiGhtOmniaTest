using AbiGhtOmniaTest.Application.Commands;
using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Serilog;

namespace AbiGhtOmniaTest.Application.Handlers;

public class CreateUserCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher, ILogger logger) : IRequestHandler<CreateUserCommand, Guid>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;
    private readonly ILogger _logger = logger;

    public async Task<Guid> Handle(CreateUserCommand request, CancellationToken cancellationToken)
    {
        try
        {
            var user = new User
            {
                Id = Guid.NewGuid(),
                Email = request.Email,
                Username = request.Username,
                PasswordHash = _passwordHasher.HashPassword(new User(), request.Password),
                Name = request.Name,
                Address = request.Address,
                Phone = request.Phone,
                Status = request.Status,
                Role = request.Role
            };

            await _userRepository.AddUserAsync(user);
            return user.Id;
        }
        catch (Exception ex)
        {
            _logger.Error(ex, "Error occurred while creating user.");
            throw;
        }
    }
}
