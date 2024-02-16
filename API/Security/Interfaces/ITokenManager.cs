using API.Data.Entities;

namespace API.Security.Interfaces;

public interface ITokenManager
{
    string GenerateAccessToken(User user);

    RefreshToken GenerateRefreshToken(User user);
}