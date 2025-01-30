using AbiGhtOmniaTest.Domain.Entities;
using MediatR;

namespace AbiGhtOmniaTest.Application.Commands;

public class UpdateUserCommand : IRequest<Unit>
{
    public Guid Id { get; set; }
    public string Email { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public Name Name { get; set; } = new();
    public Address Address { get; set; } = new();
    public string Phone { get; set; } = string.Empty;
    public UserStatus Status { get; set; } = new();
    public UserRole Role { get; set; } = new();
}
