using AbiGhtOmniaTest.Domain.Entities;

namespace AbiGhtOmniaTest.Domain.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByEmailAsync(string email);
    Task<User?> GetUserByIdAsync(Guid id);
    Task<IEnumerable<User>> GetUsersAsync(int pageNumber, int pageSize);
    Task AddUserAsync(User user);
    Task UpdateUserAsync(User user);
    Task DeleteUserAsync(Guid id);
}
