using API.Data.Entities;

namespace API.Dto.Users;

public sealed class UserResponseDto(User user)
{
    public Guid UserId { get; init; } = user.Id;
    public string Email { get; init; } = user.Email;
    public string FirstName { get; init; } = user.FirstName;
    public string LastName { get; init; } = user.LastName;
    public string UniversityNumber { get; init; } = user.UniversityNumber;
}