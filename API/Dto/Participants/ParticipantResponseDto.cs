using API.Data.Entities;
using API.Dto.Courses;
using API.Dto.Users;

namespace API.Dto.Participants;

public sealed class ParticipantResponseDto()
{
    public CourseResponseDto Course { get; init; }
    public UserResponseDto User { get; init; }

    public ParticipantResponseDto(Participant participant) : this()
    {
        Course = new CourseResponseDto(participant.Course);
        User = new UserResponseDto(participant.User);
    }
}
