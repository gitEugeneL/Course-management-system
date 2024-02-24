using API.Data.Entities;

namespace API.Dto.Users;

public sealed class UserResponseDto()
{
    public Guid UserId { get; init; }
    public string Email { get; init; } = string.Empty;
    public string FirstName { get; init; } = string.Empty;
    public string LastName { get; init; } = string.Empty;
    public string UniversityNumber { get; init; } = string.Empty;

    public UserResponseDto(User user) : this()
    {
        UserId = user.Id;
        Email = user.Email;
        FirstName = user.FirstName;
        LastName = user.LastName;
        UniversityNumber = user.UniversityNumber;
    }
}