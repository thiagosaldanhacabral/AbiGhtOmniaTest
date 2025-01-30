using AbiGhtOmniaTest.Application.Commands;
using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace AbiGhtOmniaTest.Application.Handlers;

public class UpdateUserCommandHandler(IUserRepository userRepository, IPasswordHasher<User> passwordHasher) : IRequestHandler<UpdateUserCommand, Unit>
{
    private readonly IUserRepository _userRepository = userRepository;
    private readonly IPasswordHasher<User> _passwordHasher = passwordHasher;

    public async Task<Unit> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetUserByIdAsync(request.Id) ?? throw new Exception("User not found.");
        user.Email = request.Email;
        user.Username = request.Username;
        user.PasswordHash = _passwordHasher.HashPassword(user, request.Password);
        user.Name = request.Name;
        user.Address = request.Address;
        user.Phone = request.Phone;
        user.Status = request.Status;
        user.Role = request.Role;

        await _userRepository.UpdateUserAsync(user);
        return Unit.Value;
    }
}
