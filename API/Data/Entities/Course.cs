using API.Data.Entities.Common;

namespace API.Data.Entities;

public sealed class Course(
    string name,
    string description,
    int maxParticipants,
    Guid ownerId
    ) : BaseAuditableEntity
{
    public string Name { get; init; } = name;
    public string Description { get; init; } = description;
    public int MaxParticipants { get; init; } = maxParticipants;
    public Guid OwnerId { get; init; } = ownerId;

    /*** Relations ***/
    public List<Participant> Participants { get; init; } = [];
}