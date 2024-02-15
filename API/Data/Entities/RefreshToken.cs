using API.Data.Entities.Common;

namespace API.Data.Entities;

public sealed class RefreshToken : BaseAuditableEntity
{
    public required string Token { get; init; }
    public required DateTime Expires { get; init; }
    
    /*** Relations ***/
    public required User User { get; init; }
    public Guid UserId { get; init; }
}