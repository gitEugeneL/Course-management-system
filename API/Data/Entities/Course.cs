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
    public string Description { get; set; } = description;
    public int MaxParticipants { get; set; } = maxParticipants;
    public Guid OwnerId { get; init; } = ownerId;
    public bool Finalized { get; set; } = false;
    
    /*** Relations ***/
    public List<Participant> Participants { get; init; } = [];
}