using API.Data.Entities.Common;
using API.Data.Enums;

namespace API.Data.Entities;

public sealed class User(
    string email,
    byte[] pwdHash,
    byte[] pwdSalt,
    Role role,
    string universityNumber,
    string firstName,
    string lastName
    ) : BaseAuditableEntity
{
    public string Email { get; init; } = email;
    public byte[] PwdHash { get; init; } = pwdHash;
    public byte[] PwdSalt { get; init; } = pwdSalt;
    public Role Role { get; init; } = role;
    public string UniversityNumber { get; init; } = universityNumber;
    public string FirstName { get; init; } = firstName;
    public string LastName { get; init; } = lastName;

    /*** Relations ***/
    public List<RefreshToken> RefreshTokens { get; init; } = [];
    public List<Participant> Participants { get; init; } = [];
}
