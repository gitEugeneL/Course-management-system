using API.Data.Entities;

namespace API.Dto.Courses;

public sealed class CourseResponseDto(Course course)
{
    public Guid CourseId { get; init; } = course.Id;
    public Guid OwnerId { get; init; } = course.OwnerId;
    public string Name { get; init; } = course.Name;
    public string Description { get; init; } = course.Description;
    public int MaxParticipants { get; init; } = course.MaxParticipants;
    public int CountParticipants { get; init; } = course.Participants.Count;
}
