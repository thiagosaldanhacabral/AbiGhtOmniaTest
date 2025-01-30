using AbiGhtOmniaTest.Domain.Entities;
using AbiGhtOmniaTest.Domain.Interfaces;
using AbiGhtOmniaTest.Infraestructure.Data;
using Microsoft.EntityFrameworkCore;

namespace AbiGhtOmniaTest.Infraestructure.Repository;

public class UserRepository(DeveloperStoreDbContext context) : IUserRepository
{
    private readonly DeveloperStoreDbContext _context = context;

    public async Task<User?> GetUserByEmailAsync(string email)
    {
        return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
    }

    public async Task<User?> GetUserByIdAsync(Guid id)
    {
        return await _context.Users.FindAsync(id);
    }

    public async Task<IEnumerable<User>> GetUsersAsync(int pageNumber, int pageSize)
    {
        return await _context.Users
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync();
    }

    public async Task AddUserAsync(User user)
    {
        _context.Users.Add(user);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateUserAsync(User user)
    {
        _context.Users.Update(user);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteUserAsync(Guid id)
    {
        var user = await _context.Users.FindAsync(id);
        if (user != null)
        {
            _context.Users.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
