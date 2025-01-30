using AbiGhtOmniaTest.Domain.Authentication;

namespace AbiGhtOmniaTest.Application.Interfaces;

public interface IAuthService
{
    Task<LoginResponse> LoginAsync(LoginRequest request);
}
