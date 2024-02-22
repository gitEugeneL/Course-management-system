using API.Data.Entities;

namespace API.Dto.Courses;

public sealed class CourseResponseDto()
{
    public Guid CourseId { get; init; }
    public Guid OwnerId { get; init; }
    public string Name { get; init; } = string.Empty;
    public string Description { get; init; } = string.Empty;
    public int MaxParticipants { get; init; }
    public int CountParticipants { get; init; }
    public bool Finalized { get; init; } 
    public DateTime CreatedAt { get; init; }
    
    public CourseResponseDto(Course course) : this()
    {
        CourseId = course.Id;
        OwnerId = course.OwnerId;
        Name = course.Name;
        Description = course.Description;
        MaxParticipants = course.MaxParticipants;
        CountParticipants = course.Participants.Count;
        Finalized = course.Finalized;
        CreatedAt = course.Created;
    }
}
