using API.Data.Entities;

namespace API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateUser(User user);

    Task<bool> UserExist(string email);

    Task<User?> FindUserByEmail(string email);

    Task<User?> FindUserByRefreshToken(string refreshToken);

    Task<bool> UpdateUser(User user);
}