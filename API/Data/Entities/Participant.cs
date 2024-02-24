using API.Data.Entities.Common;

namespace API.Data.Entities;

public class Participant : BaseAuditableEntity
{
    public int Grade { get; set; }
    public string ProfessorNote { get; set; } = string.Empty;
    
    /*** Relations ***/
    public required User User { get; init; }
    public Guid UserId { get; init; }

    public required Course Course { get; init; }
    public Guid CourseId { get; init; }
}