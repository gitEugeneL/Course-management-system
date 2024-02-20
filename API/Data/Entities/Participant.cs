using API.Data.Entities.Common;

namespace API.Data.Entities;

public class Participant : BaseAuditableEntity
{
    public int Grade { get; init; }
    public string ProfessorNote { get; init; } = string.Empty;
    
    /*** Relations ***/
    public required User User { get; init; }
    public Guid UserId { get; init; }

    public required Course Course { get; init; }
    public Guid CourseId { get; init; }
}