using API.Data.Entities;

namespace API.Repositories.Interfaces;

public interface IUserRepository
{
    Task<bool> CreateUser(User user);

    Task<bool> UserExists(string email);
}