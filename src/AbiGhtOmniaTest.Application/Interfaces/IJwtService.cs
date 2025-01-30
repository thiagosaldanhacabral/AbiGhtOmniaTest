using AbiGhtOmniaTest.Domain.Entities;

namespace AbiGhtOmniaTest.Application.Interfaces;

public interface IJwtService
{
    string GenerateToken(User user);
}
