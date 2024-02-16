using API.Data.Entities;

namespace API.Models.Dto.Auth;

public sealed class LoginResponseDto(string accessToken, RefreshToken refreshToken)
{
    public string AccessTokenType { get; init; } = "Bearer";
    public string AccessToken { get; init; } = accessToken;
    public string RefreshToken { get; init; } = refreshToken.Token;
    public DateTime RefreshTokenExpires { get; init; } = refreshToken.Expires;
}