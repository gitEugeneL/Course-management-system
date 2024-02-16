using API.Data.Entities;
using API.Data.Persistence;
using API.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace API.Repositories;

public class UserRepository(AppDbContext context) : IUserRepository
{
    public async Task<bool> CreateUser(User user)
    {
        await context
            .Users
            .AddAsync(user);
        return await context
            .SaveChangesAsync() > 0;
    }

    public async Task<bool> UserExists(string email)
    {
        return await context
            .Users
            .AnyAsync(u => u.Email.ToLower()
                .Equals(email.ToLower()));
    }

    public async Task<User?> FindUserByEmail(string email)
    {
        return await context
            .Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.Email.ToLower().Equals(email.ToLower()));
    }

    public async Task<User?> FindUserByRefreshToken(string refreshToken)
    {
        return await context
            .Users
            .Include(u => u.RefreshTokens)
            .SingleOrDefaultAsync(u => u.RefreshTokens
                .Any(rt => rt.Token == refreshToken));
    }

    public async Task<bool> UpdateUser(User user)
    {
        context
            .Users
            .Update(user);
        return await context
            .SaveChangesAsync() > 0;
    }
}