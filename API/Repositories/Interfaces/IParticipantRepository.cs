using API.Data.Entities;

namespace API.Repositories.Interfaces;

public interface IParticipantRepository
{
    Task<Participant?> FindParticipantByUserIdAndCourseId(Guid userId, Guid courseId);

    Task<bool> UpdateParticipant(Participant participant);
    
    Task<IEnumerable<Participant>> FindParticipantsByUserId(Guid userId);
    
    Task<(IEnumerable<Participant> List, int Count)> FindAllByCoursePagination(
        int pageNumber,
        int pageSize,
        Course course);
}